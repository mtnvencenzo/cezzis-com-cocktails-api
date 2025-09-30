module "login_cname_record" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/dns-sub-domain-record"

  dns_zone = data.azurerm_dns_zone.cezzis_dns_zone.name
  ttl      = 300
  name     = var.auth0_custom_domain_subdomain
  value    = var.auth0_custom_domain_cname

  tags = local.tags
}
