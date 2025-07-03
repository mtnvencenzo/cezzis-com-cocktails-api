module "aca_cocktails_api_availability_test" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/appinsights-availability-test"
  providers = {
    azurerm = azurerm
  }

  sub                     = var.sub
  region                  = var.region
  environment             = var.environment
  domain                  = var.domain
  name_discriminator      = "api"
  sequence                = var.sequence
  resource_group_name     = data.azurerm_resource_group.cocktails_resource_group.name
  location                = data.azurerm_resource_group.cocktails_resource_group.location
  application_insights_id = data.azurerm_application_insights.appi.id
  description             = "Availability test for the cocktails api"
  http_url                = "https://${var.custom_domain.host_name}/${var.environment}/${var.domain}/api/v1/cocktails/absinthe-frappe"

  create_alert = false

  headers = [
    {
      name  = "X-Key"
      value = random_password.cocktails_api_devops_subscription_keys[0].result
    }
  ]
}