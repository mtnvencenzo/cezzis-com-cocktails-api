# module "apim_cocktails_api_docs" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/apim-api"
#   providers = {
#     azurerm = azurerm
#   }

#   tags                           = local.tags
#   environment                    = var.environment
#   domain                         = var.domain
#   name_discriminator             = "api-docs"
#   keyvault_apimhostkey_secret_id = azurerm_key_vault_secret.cocktails_api_apimhostkey.id
#   subscription_required          = false
#   backend_url_override           = "https://${module.aca_cocktails_api.ingress_fqdn}"
#   service_url_override           = "https://${module.aca_cocktails_api.ingress_fqdn}"

#   apim_instance = {
#     id                  = data.azurerm_api_management.apim_shared.id
#     name                = data.azurerm_api_management.apim_shared.name
#     resource_group_name = data.azurerm_api_management.apim_shared.resource_group_name
#   }

#   api = {
#     version      = 1
#     service_fqdn = module.aca_cocktails_api.ingress_fqdn
#     ingress_fqdn = module.aca_cocktails_api.ingress_fqdn
#   }

#   application_insights = {
#     id                  = data.azurerm_application_insights.appi.id
#     instrumentation_key = data.azurerm_application_insights.appi.instrumentation_key
#   }

#   operations = [
#     {
#       display_name        = "Get Scalar Docs v1"
#       method              = "GET"
#       url_template        = "/scalar/*"
#       description         = "Get Scalar Docs v1"
#       success_status_code = 200
#       policy_xml_content  = local.apim_anonomous_docs_operation_policy
#     }
#   ]
# }