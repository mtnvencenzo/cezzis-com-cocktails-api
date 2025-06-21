resource "azurerm_key_vault_secret" "recaptcha_cezzi_site_secret" {
  name         = "recaptcha-cezzi-site-secret"
  value        = "n/a"
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id

  lifecycle {
    ignore_changes = [value]
  }

  tags = local.tags
}


# ----------------------------------------
# Backend apim host key
# ----------------------------------------

resource "random_password" "cocktails_api_apimhostkey" {
  length  = 32
  special = true
  upper   = true
}

resource "azurerm_key_vault_secret" "cocktails_api_apimhostkey" {
  name         = "cocktails-api-apimhostkey"
  value        = random_password.cocktails_api_apimhostkey.result
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id
}



# ----------------------------------------
# Subscription Cezzis.com apim primary key
# ----------------------------------------

resource "random_password" "cocktails_api_cezzis_com_subscription_keys" {
  count   = 2
  length  = 24
  special = true
  upper   = false
}

resource "azurerm_key_vault_secret" "cocktails_api_cezzis_com_subscription_primary_key" {
  name         = "cocktails-api-cezzis-com-subscription-primary-key"
  value        = random_password.cocktails_api_cezzis_com_subscription_keys[0].result
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id
  tags         = local.tags
}


# ----------------------------------------
# Subscription Devops apim primary key
# ----------------------------------------

resource "random_password" "cocktails_api_devops_subscription_keys" {
  count   = 2
  length  = 24
  special = true
  upper   = false
}

resource "azurerm_key_vault_secret" "cocktails_api_devops_subscription_primary_key" {
  name         = "cocktails-api-devops-subscription-primary-key"
  value        = random_password.cocktails_api_devops_subscription_keys[0].result
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id
  tags         = local.tags
}

# ----------------------------------------
# Zoho Email cezzi@cezzis.com App Password
# ----------------------------------------

resource "azurerm_key_vault_secret" "zoho_email_cezzi_email_app_password" {
  name         = "zoho-email-cezzi-email-app-password"
  value        = "n/a"
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id
  tags         = local.tags

  lifecycle {
    ignore_changes = [value]
  }
}

# ----------------------------------------
# Cocktails API App Registration Graph API Secret
# ----------------------------------------

resource "azurerm_key_vault_secret" "cocktails_api_app_registration_graph_api_secret" {
  name         = "cocktails-api-app-registration-graph-api-secret"
  value        = module.api_b2c_tenant.cocktails_api_app_registration_graph_api_secret
  key_vault_id = data.azurerm_key_vault.cocktails_keyvault.id
  tags         = local.tags
}
