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
      name                  = "graph-api-client-registration-secret"
      key_vault_secret_name = azurerm_key_vault_secret.cocktails_api_app_registration_graph_api_secret.name
    }
  ]

  env_vars = [
    {
      name  = "APPLICATIONINSIGHTS_CONNECTION_STRING"
      value = data.azurerm_application_insights.appi.connection_string
    },
    {
      name  = "APPLICATIONINSIGHTS_INSTRUMENTATIONKEY"
      value = data.azurerm_application_insights.appi.instrumentation_key
    },
    {
      name  = "ApplicationInsightsAgent_EXTENSION_VERSION"
      value = "~2"
    },
    {
      name  = "AllowedOrigins"
      value = join(",", var.allowed_origins)
    },
    {
      name  = "AzureAdB2C__Instance"
      value = "https://${var.login_subdomain}.cezzis.com"
    },
    {
      name  = "AzureAdB2C__Domain"
      value = "${var.b2c_tenant_name}.onmicrosoft.com"
    },
    {
      name  = "AzureAdB2C__ClientId"
      value = module.api_b2c_tenant.cocktails_api_app_registration_client_id
    },
    {
      name  = "AzureAdB2C__SignUpSignInPolicyId"
      value = var.b2c_signin_policy
    },
    {
      name  = "AzureAdB2C__Audience"
      value = module.api_b2c_tenant.cocktails_api_app_registration_client_id
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
      name  = "Scalar__AuthorizationCodeFlow__ClientId"
      value = var.cezzis_web_client_id
    },
    {
      name  = "Scalar__AuthorizationCodeFlow__Scopes__0"
      value = "https://${var.b2c_tenant_domain_name}/cocktailsapi/Account.Read"
    },
    {
      name  = "Scalar__AuthorizationCodeFlow__Scopes__1"
      value = "https://${var.b2c_tenant_domain_name}/cocktailsapi/Account.Write"
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
    },
    {
      name  = "MsGraph__ClientId"
      value = module.api_b2c_tenant.cocktails_api_app_registration_client_id
    },
    {
      name  = "MsGraph__TenantId"
      value = var.b2c_tenant_id
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
      name        = "MsGraph__ClientSecret"
      secret_name = "graph-api-client-registration-secret"
    }
  ]
}
