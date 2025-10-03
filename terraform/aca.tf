module "aca_cocktails_api" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/container-app"
  providers = {
    azurerm = azurerm
  }

  sub                          = var.sub
  region                       = var.region
  environment                  = var.environment
  domain                       = var.domain
  name_discriminator           = "api"
  sequence                     = var.sequence
  tenant_id                    = data.azurerm_client_config.current.tenant_id
  resource_group_name          = data.azurerm_resource_group.cocktails_resource_group.name
  resource_group_location      = data.azurerm_resource_group.cocktails_resource_group.location
  container_app_environment_id = data.azurerm_container_app_environment.cae_shared.id
  ingress_target_port          = "8080"
  startup_probe_relative_url   = "/api/v1/health/ping"

  tags = local.tags

  container_registry = {
    id           = data.azurerm_container_registry.shared_acr.id
    login_server = data.azurerm_container_registry.shared_acr.login_server
  }

  key_vault = {
    id   = data.azurerm_key_vault.cocktails_keyvault.id
    name = data.azurerm_key_vault.cocktails_keyvault.name
  }

  container = {
    name       = "cocktails-api"
    image_name = "cocktailsapi"
    image_tag  = var.image_tag
    cpu        = 0.25
    memory     = "0.5Gi"
  }

  dapr = {
    app_id = "cocktails-api"
    components = [
      {
        name           = var.pubsub_sb_queues_cocktails_email
        component_type = "pubsub.azure.servicebus.queues"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "queueName"
            value = module.cocktails_servicebus_email_queue.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_topics_cocktails_email
        component_type = "pubsub.azure.servicebus.topics"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "topicName"
            value = module.cocktails_servicebus_email_topic.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_topics_cocktails_account
        component_type = "pubsub.azure.servicebus.topics"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "topicName"
            value = module.cocktails_servicebus_account_topic.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_queues_cocktails_account
        component_type = "pubsub.azure.servicebus.queues"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "queueName"
            value = module.cocktails_servicebus_account_queue.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_queues_cocktails_account_email
        component_type = "pubsub.azure.servicebus.queues"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "queueName"
            value = module.cocktails_servicebus_account_email_queue.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_queues_cocktails_account_password
        component_type = "pubsub.azure.servicebus.queues"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "queueName"
            value = module.cocktails_servicebus_account_password_queue.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_queues_cocktail_ratings
        component_type = "pubsub.azure.servicebus.queues"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "queueName"
            value = module.cocktails_servicebus_cocktail_ratings_queue.name
          }
        ]
      },
      {
        name           = var.pubsub_sb_topics_cocktail_ratings
        component_type = "pubsub.azure.servicebus.topics"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "namespaceName"
            value = "${data.azurerm_servicebus_namespace.servicebus_namespace.name}.servicebus.windows.net"
          },
          {
            name  = "topicName"
            value = module.cocktails_servicebus_cocktail_ratings_topic.name
          }
        ]
      },
      {
        name           = var.binding_cocktails_blob_account_avatars
        component_type = "bindings.azure.blobstorage"
        metadata = [
          {
            name = "azureClientId"
          },
          {
            name  = "containerName"
            value = var.account_avatars_storage_container
          },
          {
            name  = "accountName"
            value = data.azurerm_storage_account.cocktails_storage_account.name
          }
        ]
      }
    ]
  }

  secrets = [
    {
      name                  = "apim-host-key"
      key_vault_secret_name = azurerm_key_vault_secret.cocktails_api_apimhostkey.name
    },
    {
      name                  = "recaptcha-site-secret"
      key_vault_secret_name = azurerm_key_vault_secret.recaptcha_cezzi_site_secret.name
    },
    {
      name                  = "zoho-email-cezzi-app-password"
      key_vault_secret_name = azurerm_key_vault_secret.zoho_email_cezzi_email_app_password.name
    },
    {
      name                  = "auth0-client-secret"
      key_vault_secret_name = azurerm_key_vault_secret.auth0_client_secret.name
    }
  ]

  env_vars = [
    {
      name  = "AllowedOrigins"
      value = join(",", var.allowed_origins)
    },
    {
      name  = "Auth0__Domain"
      value = "https://${var.auth0_domain}"
    },
    {
      name  = "Auth0__ClientId"
      value = "https://${var.auth0_frontend_client_id}"
    },
    {
      name  = "Auth0__Audience"
      value = var.auth0_audience
    },
    {
      name  = "Auth0__ManagementDomain"
      value = "https://${var.auth0_management_domain}"
    },
    {
      name  = "Auth0__ManagementM2MClientId"
      value = var.auth0_management_client_id
    },
    {
      name  = "BlobStorage__AccountAvatars__DaprBuildingBlock"
      value = var.binding_cocktails_blob_account_avatars
    },
    {
      name  = "BlobStorage__CdnHostName"
      value = var.cocktail_images_route_hostname
    },
    {
      name  = "CocktailsApi__BaseImageUri"
      value = "https://${var.cocktail_images_route_hostname}/cocktails"
    },
    {
      name  = "CocktailsApi__BaseOpenApiUri"
      value = "https://${var.custom_domain.host_name}/${var.environment}/${var.domain}"
    },
    {
      name  = "CocktailsWeb__SiteMap__CockailsPageFormat"
      value = "https://${var.cezzis_site_hostname}/cocktails/:id"
    },
    {
      name  = "OTel__OtlpExporter__Endpoint"
      value = "https://${data.azurerm_container_app.otel_collector.ingress[0].fqdn}"
    },
    {
      name  = "OTel__OtlpExporter__Headers"
      value = "Authorization=Bearer ${data.azurerm_key_vault_secret.otel_collector_api_key.value}"
    },
    {
      name  = "CosmosDb__ConnectionString"
      value = ""
    },
    {
      name  = "CosmosDb__AccountEndpoint"
      value = module.cocktails_cosmosdb_account.cosmosdb_enpdpoint
    },
    {
      name  = "CosmosDb__DatabaseName"
      value = var.cocktails_cosmosdb_database_name
    },
    {
      name  = "PubSub__EmailPublisher__DaprBuildingBlock"
      value = var.pubsub_sb_topics_cocktails_email
    },
    {
      name  = "PubSub__EmailPublisher__TopicName"
      value = module.cocktails_servicebus_email_topic.name
    },
    {
      name  = "PubSub__EmailSubscriber__DaprBuildingBlock"
      value = var.pubsub_sb_queues_cocktails_email
    },
    {
      name  = "PubSub__EmailSubscriber__QueueName"
      value = module.cocktails_servicebus_email_queue.name
    },
    {
      name  = "PubSub__AccountPublisher__DaprBuildingBlock"
      value = var.pubsub_sb_topics_cocktails_account
    },
    {
      name  = "PubSub__AccountPublisher__TopicName"
      value = module.cocktails_servicebus_account_topic.name
    },
    {
      name  = "PubSub__AccountSubscriber__DaprBuildingBlock"
      value = var.pubsub_sb_queues_cocktails_account
    },
    {
      name  = "PubSub__AccountSubscriber__QueueName"
      value = module.cocktails_servicebus_account_queue.name
    },
    {
      name  = "PubSub__AccountEmailPublisher__DaprBuildingBlock"
      value = var.pubsub_sb_topics_cocktails_account_email
    },
    {
      name  = "PubSub__AccountEmailPublisher__TopicName"
      value = module.cocktails_servicebus_account_topic.name
    },
    {
      name  = "PubSub__AccountEmailSubscriber__DaprBuildingBlock"
      value = var.pubsub_sb_queues_cocktails_account_email
    },
    {
      name  = "PubSub__AccountEmailSubscriber__QueueName"
      value = module.cocktails_servicebus_account_email_queue.name
    },
    {
      name  = "PubSub__AccountPasswordPublisher__DaprBuildingBlock"
      value = var.pubsub_sb_topics_cocktails_account_password
    },
    {
      name  = "PubSub__AccountPasswordPublisher__TopicName"
      value = module.cocktails_servicebus_account_topic.name
    },
    {
      name  = "PubSub__AccountPasswordSubscriber__DaprBuildingBlock"
      value = var.pubsub_sb_queues_cocktails_account_password
    },
    {
      name  = "PubSub__AccountPasswordSubscriber__QueueName"
      value = module.cocktails_servicebus_account_password_queue.name
    },
    {
      name  = "PubSub__CocktailRatingPublisher__DaprBuildingBlock"
      value = var.pubsub_sb_topics_cocktail_ratings
    },
    {
      name  = "PubSub__CocktailRatingPublisher__TopicName"
      value = module.cocktails_servicebus_cocktail_ratings_topic.name
    },
    {
      name  = "PubSub__CocktailRatingSubscriber__DaprBuildingBlock"
      value = var.pubsub_sb_queues_cocktail_ratings
    },
    {
      name  = "PubSub__CocktailRatingSubscriber__QueueName"
      value = module.cocktails_servicebus_cocktail_ratings_queue.name
    },
    {
      name  = "Search__Endpoint"
      value = module.ai_search_cosmos_index.fqdn_host_name
    },
    {
      name  = "Search__IndexName"
      value = module.ai_search_cosmos_index.index_name
    },
    {
      name  = "Search__QueryKey"
      value = ""
    },
    {
      name  = "Search__UseSearchIndex"
      value = "true"
    },
    {
      name  = "ZohoEmail__DefaultSender__EmailAddress"
      value = var.zoho_email_sender_address
    }
  ]

  env_secret_vars = [
    {
      name        = "CocktailsApi__ApimHostKey"
      secret_name = "apim-host-key"
    },
    {
      name        = "Recaptcha__SiteSecret"
      secret_name = "recaptcha-site-secret"
    },
    {
      name        = "ZohoEmail__DefaultSender__AppPassword"
      secret_name = "zoho-email-cezzi-app-password"
    },
    {
      name        = "Auth0__ManagementM2MClientSecret"
      secret_name = "auth0-client-secret"
    }
  ]
}
