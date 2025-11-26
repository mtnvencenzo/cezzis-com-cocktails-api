# module "cocktails_servicebus_account_topic" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-topic"

#   namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
#   name_discriminator  = "account"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_account_topic" {
#   scope                = module.cocktails_servicebus_account_topic.id
#   role_definition_name = "Azure Service Bus Data Sender"
#   principal_id         = module.aca_cocktails_api.managed_identity_principal_id
# }

# # ----------------------------------------------------------
# # Primary Queue and Subscription for Account Update Messages
# # ----------------------------------------------------------
# module "cocktails_servicebus_account_queue" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-queue"

#   namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
#   name_discriminator  = "account"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_account_queue" {
#   scope                = module.cocktails_servicebus_account_queue.id
#   role_definition_name = "Azure Service Bus Data Receiver"
#   principal_id         = module.aca_cocktails_api.managed_identity_principal_id
# }

# module "cocktails_servicebus_account_svc_subscription" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription"

#   topic_id            = module.cocktails_servicebus_account_topic.id
#   forward_to          = module.cocktails_servicebus_account_queue.name
#   name_discriminator  = "account-svc"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# module "cocktails_servicebus_account_subscription_rule" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription-rule"

#   subscription_id          = module.cocktails_servicebus_account_svc_subscription.id
#   correlation_filter_label = "account-svc"

#   providers = {
#     azurerm = azurerm
#   }
# }


# # ----------------------------------------------------------
# # Primary Queue and Subscription for Account Email Update Messages
# # ----------------------------------------------------------
# module "cocktails_servicebus_account_email_queue" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-queue"

#   namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
#   name_discriminator  = "account-email"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_account_email_queue" {
#   scope                = module.cocktails_servicebus_account_email_queue.id
#   role_definition_name = "Azure Service Bus Data Receiver"
#   principal_id         = module.aca_cocktails_api.managed_identity_principal_id
# }

# module "cocktails_servicebus_account_email_svc_subscription" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription"

#   topic_id            = module.cocktails_servicebus_account_topic.id
#   forward_to          = module.cocktails_servicebus_account_email_queue.name
#   name_discriminator  = "account-email-svc"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# module "cocktails_servicebus_account_email_subscription_rule" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription-rule"

#   subscription_id          = module.cocktails_servicebus_account_email_svc_subscription.id
#   correlation_filter_label = "account-email-svc"

#   providers = {
#     azurerm = azurerm
#   }
# }


# # ----------------------------------------------------------
# # Primary Queue and Subscription for Account Password Update Messages
# # ----------------------------------------------------------
# module "cocktails_servicebus_account_password_queue" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-queue"

#   namespace_id        = data.azurerm_servicebus_namespace.servicebus_namespace.id
#   name_discriminator  = "account-password"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# resource "azurerm_role_assignment" "cocktails_api_access_cocktails_servicebus_account_password_queue" {
#   scope                = module.cocktails_servicebus_account_password_queue.id
#   role_definition_name = "Azure Service Bus Data Receiver"
#   principal_id         = module.aca_cocktails_api.managed_identity_principal_id
# }

# module "cocktails_servicebus_account_password_svc_subscription" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription"

#   topic_id            = module.cocktails_servicebus_account_topic.id
#   forward_to          = module.cocktails_servicebus_account_password_queue.name
#   name_discriminator  = "account-password-svc"
#   sub                 = var.sub
#   region              = var.region
#   environment         = var.environment
#   domain              = var.domain
#   resource_group_name = data.azurerm_resource_group.cocktails_resource_group.name
#   location            = data.azurerm_resource_group.cocktails_resource_group.location

#   providers = {
#     azurerm = azurerm
#   }

#   tags = local.tags
# }

# module "cocktails_servicebus_account_password_subscription_rule" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/servicebus-subscription-rule"

#   subscription_id          = module.cocktails_servicebus_account_password_svc_subscription.id
#   correlation_filter_label = "account-password-svc"

#   providers = {
#     azurerm = azurerm
#   }
# }
