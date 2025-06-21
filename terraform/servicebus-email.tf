module "cocktails_servicebus_email_topic" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-topic"

  namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
  name_discriminator  = "email"
  sub                 = var.sub
  region              = var.region
  environment         = var.environment
  domain              = var.domain
  resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
  location            = data.azurerm_resource_group.cocktails_resource_group.location

  providers = {
    azurerm = azurerm
  }

  tags = local.tags
}

module "cocktails_servicebus_email_queue" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-queue"

  namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
  name_discriminator  = "email"
  sub                 = var.sub
  region              = var.region
  environment         = var.environment
  domain              = var.domain
  resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
  location            = data.azurerm_resource_group.cocktails_resource_group.location

  providers = {
    azurerm = azurerm
  }

  tags = local.tags
}

module "cocktails_servicebus_email_svc_subscription" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription"

  topic_id            = module.cocktails_servicebus_email_topic.id
  forward_to          = module.cocktails_servicebus_email_queue.name
  name_discriminator  = "email-svc"
  sub                 = var.sub
  region              = var.region
  environment         = var.environment
  domain              = var.domain
  resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
  location            = data.azurerm_resource_group.cocktails_resource_group.location

  providers = {
    azurerm = azurerm
  }

  tags = local.tags
}

module "cocktails_servicebus_subscription_rule" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription-rule"

  subscription_id          = module.cocktails_servicebus_email_svc_subscription.id
  correlation_filter_label = "email-svc"

  providers = {
    azurerm = azurerm
  }
}

resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_email_topic" {
  scope                = module.cocktails_servicebus_email_topic.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = module.aca_cocktails_api.managed_identity_principal_id
}

resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_email_queue" {
  scope                = module.cocktails_servicebus_email_queue.id
  role_definition_name = "Azure Service Bus Data Receiver"
  principal_id         = module.aca_cocktails_api.managed_identity_principal_id
}
