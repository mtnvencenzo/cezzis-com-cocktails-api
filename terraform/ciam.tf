module "api_ciam_tenant" {
  source             = "./ciam_tenant"
  environment        = var.environment
  region             = var.region
  domain             = var.domain
  tenant_id          = var.ciam_tenant_id
  tenant_domain_name = var.ciam_tenant_domain_name
  sub                = var.sub
  sequence           = var.sequence
  ciam_tenant_name    = var.ciam_tenant_name

  login_subdomain          = var.login_subdomain
  cdn_frontdoor_profile_id = data.azurerm_cdn_frontdoor_profile.global_shared_cdn.id
  dns_zone_id              = data.azurerm_dns_zone.cezzis_dns_zone.id
  dns_zone_resource_group  = data.azurerm_resource_group.cocktails_global_resource_group.name
  dns_zone_name            = data.azurerm_dns_zone.cezzis_dns_zone.name
  allowed_origins          = var.allowed_origins

  tags = local.tags

  providers = {
    azuread = azuread
    azurerm = azurerm
  }
}