module "login_cname_record" {
  source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/dns-sub-domain-record"

  dnszone {
    resource_group_name = data.azurerm_dns_zone.cezzis_dns_zone.resource_group_name
    name                = data.azurerm_dns_zone.cezzis_dns_zone.name
  }


  dns_zone                      = data.azurerm_dns_zone.cezzis_dns_zone.name
  ttl                           = 300
  sub_domain                    = var.auth0_custom_domain_subdomain
  record_fqdn                   = var.auth0_custom_domain_cname
  custom_domain_verification_id = "random-string" # Not used for CNAME but required by module interface

  tags = local.tags
}
