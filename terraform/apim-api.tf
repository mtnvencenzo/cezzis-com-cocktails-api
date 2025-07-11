resource "azurerm_key_vault_access_policy" "apim_keyvault_policy" {
  key_vault_id       = data.azurerm_key_vault.cocktails_keyvault.id
  tenant_id          = data.azurerm_client_config.current.tenant_id
  object_id          = data.azurerm_api_management.apim_shared.identity[0].principal_id
  key_permissions    = ["Get", "List"]
  secret_permissions = ["Get", "List"]
}

module "apim_cocktails_api_jwtvalidate_policy" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/apim-jwtvalidate-policy-fragment"
  providers = {
    azurerm = azurerm
  }
  environment        = var.environment
  domain             = var.domain
  name_discriminator = "api"
  apim_instance_id   = data.azurerm_api_management.apim_shared.id
  oidc_config_url    = "https://${var.b2c_tenant_name}.b2clogin.com/${var.b2c_tenant_name}.onmicrosoft.com/${var.b2c_signin_policy}/v2.0/.well-known/openid-configuration"
  audiences          = [module.api_b2c_tenant.cocktails_api_app_registration_client_id]
  issuers            = ["https://${var.b2c_tenant_name}.b2clogin.com/${var.b2c_tenant_id}/v2.0/"]
}

module "apim_cocktails_api_cors_policy" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/apim-cors-policy-fragment"
  providers = {
    azurerm = azurerm
  }
  environment        = var.environment
  domain             = var.domain
  name_discriminator = "api"
  apim_instance_id   = data.azurerm_api_management.apim_shared.id
  allowed_origins    = var.allowed_origins
}


module "apim_cocktails_api" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/apim-api"
  providers = {
    azurerm = azurerm
  }

  tags                           = local.tags
  environment                    = var.environment
  domain                         = var.domain
  name_discriminator             = "api"
  keyvault_apimhostkey_secret_id = azurerm_key_vault_secret.cocktails_api_apimhostkey.id

  apim_instance = {
    id                  = data.azurerm_api_management.apim_shared.id
    name                = data.azurerm_api_management.apim_shared.name
    resource_group_name = data.azurerm_api_management.apim_shared.resource_group_name
  }

  api = {
    version      = 1
    service_fqdn = module.aca_cocktails_api.ingress_fqdn
    ingress_fqdn = module.aca_cocktails_api.ingress_fqdn
  }

  application_insights = {
    id                  = data.azurerm_application_insights.appi.id
    instrumentation_key = data.azurerm_application_insights.appi.instrumentation_key
  }

  subscriptions = [
    {
      name          = "${title(var.environment)} Cezzis.Com Standard ${title(var.domain)} Api Subscription"
      primary_key   = random_password.cocktails_api_cezzis_com_subscription_keys[0].result
      secondary_key = random_password.cocktails_api_cezzis_com_subscription_keys[1].result
    },
    {
      name          = "${title(var.environment)} Cezzis.Com Devops ${title(var.domain)} Api Subscription"
      primary_key   = random_password.cocktails_api_devops_subscription_keys[0].result
      secondary_key = random_password.cocktails_api_devops_subscription_keys[1].result
    },
    {
      name          = "${title(var.environment)} Cezzis.Com LocalUsage ${title(var.domain)} Api Subscription"
      primary_key   = random_password.cocktails_api_localusage_subscription_keys[0].result
      secondary_key = random_password.cocktails_api_localusage_subscription_keys[1].result
    },
    {
      name          = "${title(var.environment)} Cezzis.Com McpUsage ${title(var.domain)} Api Subscription"
      primary_key   = random_password.cocktails_api_mcp_subscription_keys[0].result
      secondary_key = random_password.cocktails_api_mcp_subscription_keys[1].result
    }
  ]

  operations = [
    {
      display_name        = "Get Cocktail"
      method              = "GET"
      url_template        = "/cocktails/{id}"
      description         = "Gets a cocktail recipe by id"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy

      template_parameters = [
        {
          name = "id"
          type = "string"
        }
      ]
    },
    {
      display_name        = "Get Cocktails List"
      method              = "GET"
      url_template        = "/cocktails"
      description         = "Gets a list of cocktails"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Cocktails List"
      method              = "PUT"
      url_template        = "/cocktails"
      description         = "Puts the list of cocktails"
      success_status_code = 204
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Privacy Policy"
      method              = "GET"
      url_template        = "/legal/documents/privacy-policy"
      description         = "Gets the privacy policy"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Terms Of Service"
      method              = "GET"
      url_template        = "/legal/documents/terms-of-service"
      description         = "Gets the terms of service"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Ping"
      method              = "GET"
      url_template        = "/health/ping"
      description         = "Get health ping"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Version"
      method              = "GET"
      url_template        = "/health/version"
      description         = "Get version"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Ingredients List"
      method              = "PUT"
      url_template        = "/cocktails/ingredients"
      description         = "Puts the list of ingredients"
      success_status_code = 204
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Cocktails Ingredients Filters"
      method              = "GET"
      url_template        = "/cocktails/ingredients/filters"
      description         = "Get cocktails ingredients filters"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Account Owned Profile"
      method              = "GET"
      url_template        = "/accounts/owned/profile"
      description         = "Get the logged in users account profile"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Account Owned Profile"
      method              = "PUT"
      url_template        = "/accounts/owned/profile"
      description         = "Updates the account profile for the user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Post Account Owned Profile Image"
      method              = "POST"
      url_template        = "/accounts/owned/profile/image"
      description         = "Uploads an account profile image for to the user represented within the authenticated bearer token"
      success_status_code = 201
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Account Owned Profile Email"
      method              = "PUT"
      url_template        = "/accounts/owned/profile/email"
      description         = "Updates the account profile email address for the user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Account Owned Profile Accessibility Settings"
      method              = "PUT"
      url_template        = "/accounts/owned/profile/accessibility"
      description         = "Updates the account profile accessibility settings for the user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Put Account Owned Profile Cocktails Favorites"
      method              = "PUT"
      url_template        = "/accounts/owned/profile/cocktails/favorites"
      description         = "Manages the account profile favorite cocktails list for the user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Post Account Owned Profile Cocktails Ratings"
      method              = "POST"
      url_template        = "/accounts/owned/profile/cocktails/ratings"
      description         = "Rates a cocktail for the account profile user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Get Account Owned Profile Cocktails Ratings"
      method              = "GET"
      url_template        = "/accounts/owned/profile/cocktails/ratings"
      description         = "Gets the account cocktail ratings for the account profile user represented within the authenticated bearer token"
      success_status_code = 200
      policy_xml_content  = local.apim_anonomous_operation_policy
    },
    {
      display_name        = "Send Recommendation"
      method              = "POST"
      url_template        = "/accounts/owned/profile/cocktails/recommendations"
      description         = "Send a cocktail recommendation"
      success_status_code = 202
      policy_xml_content  = local.apim_anonomous_operation_policy
    }
  ]

  depends_on = [
    module.api_b2c_tenant,
    module.apim_cocktails_api_jwtvalidate_policy,
    module.apim_cocktails_api_cors_policy,
  azurerm_key_vault_access_policy.apim_keyvault_policy]
}

# When managed cetificates work with APIM unfinished as of now
# Right now just adding the dns records
# ================================
# API SUB DOMAIN
# ================================
module "apim_cocktails_api_custom_domain" {
  source                          = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/dns-sub-domain-record"
  custom_domain_verification_id   = var.custom_domain.custom_domain_verification_id # this should be pulled when tf supports it
  custom_domain_verification_name = "apimuid.${var.custom_domain.sub_domain}"
  sub_domain                      = var.custom_domain.sub_domain
  record_fqdn                     = replace(data.azurerm_api_management.apim_shared.gateway_url, "https://", "")

  tags = {
    Environment = var.environment
    Application = var.domain
  }

  dns_zone = {
    name                = data.azurerm_dns_zone.cezzis_dns_zone.name
    resource_group_name = data.azurerm_dns_zone.cezzis_dns_zone.resource_group_name
  }

  providers = {
    azurerm = azurerm
  }
}