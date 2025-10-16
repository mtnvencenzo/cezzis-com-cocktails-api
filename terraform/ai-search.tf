module "ai_search_cosmos_index" {
  source                      = "./ai_search_index"
  ai_search_service_host_name = local.ai_search_service_host_name
  ai_search_service_id        = data.azurerm_search_service.ai_search_service.id
  ai_search_service_key       = data.azurerm_search_service.ai_search_service.primary_key
  cosmosdb_account_id         = data.azurerm_cosmosdb_account.cocktails_cosmosdb_account.id
  cosmos_database_name        = var.cocktails_cosmosdb_database_name
  cosmos_container_name       = "cezzis-${var.environment}-cocktails-cocktail"
  tags                        = local.tags

  search_index_reader_role_assignment = [module.aca_cocktails_api.managed_identity_principal_id]
}