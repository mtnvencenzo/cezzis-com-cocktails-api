variable "tenant_id" {
  type = string
}

variable "tenant_domain_name" {
  type = string
}

variable "b2c_tenant_name" {
  type = string
}

variable "domain" {
  type        = string
  description = "Domain name. Use only lowercase letters and numbers"
}

variable "environment" {
  type        = string
  description = "Environment name, e.g. 'dev' or 'stage' or 'prod'"
}

variable "region" {
  type = string
}

variable "sub" {
  type        = string
  description = "Subscription short identitifer to be used in resource naming"
}

variable "sequence" {
  type        = string
  description = "The sequence number of the resource typically starting with 001"
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "login_subdomain" {
  type        = string
  description = "The subdomain to be used for the custom domain"
}

variable "cdn_frontdoor_profile_id" {
  type        = string
  description = "The ID of the CDN frontdoor profile to be used for the custom domain"
}

variable "dns_zone_id" {
  type        = string
  description = "The ID of the DNS zone to be used for the custom domain"
}

variable "dns_zone_name" {
  type        = string
  description = "The name of the DNS zone to be used for the custom domain"
}

variable "dns_zone_resource_group" {
  type        = string
  description = "The resource group of the DNS zone to be used for the custom domain setup"
}

variable "allowed_origins" {
  type    = list(string)
  default = []
}