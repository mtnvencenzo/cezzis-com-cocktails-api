output "cocktails_api_app_registration_client_id" {
  value = azuread_application.cocktails_api_app_registration.client_id
}

output "cocktails_api_app_registration_graph_api_secret" {
  value     = azuread_service_principal_password.cocktails_api_app_registration_graph_api_secret.value
  sensitive = true
}