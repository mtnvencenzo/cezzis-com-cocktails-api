module "storage_container_account_avatars" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/storage-container"

  container_name        = var.account_avatars_storage_container
  container_access_type = "blob"
  storage_account_name  = data.azurerm_storage_account.cocktails_storage_account.name

  tags = local.tags

  role_assignments = [
    {
      role_definition_name = "Storage Blob Data Contributor"
      principal_id         = module.aca_cocktails_api.managed_identity_principal_id
    },
    {
      role_definition_name = "Storage Blob Data Reader"
      principal_id         = module.aca_cocktails_api.managed_identity_principal_id
    }
  ]

  providers = {
    azurerm = azurerm
  }
}