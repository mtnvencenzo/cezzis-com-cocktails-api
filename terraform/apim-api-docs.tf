module "apim_cocktails_api_docs" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/apim-simple-api"
  providers = {
    azurerm = azurerm
  }

  tags = local.tags

  environment        = var.environment
  domain             = var.domain
  name_discriminator = "api-docs"

  apim_instance = {
    id                  = data.azurerm_api_management.apim_shared.id
    name                = data.azurerm_api_management.apim_shared.name
    resource_group_name = data.azurerm_api_management.apim_shared.resource_group_name
    principal_id        = data.azurerm_api_management.apim_shared.identity[0].principal_id
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

  operations = [
    {
      display_name        = "Get Scalar Docs v1"
      method              = "GET"
      url_template        = "/scalar/*"
      description         = "Get Scalar Docs v1"
      success_status_code = 200
      security_type       = "anonymous"
    }
  ]
}