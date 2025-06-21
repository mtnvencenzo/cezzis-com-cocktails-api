variable "domain" {
  type        = string
  description = "Domain name. Use only lowercase letters and numbers"
  default     = "cocktails"
}

variable "shortdomain" {
  type        = string
  description = "Short domain name. Use only lowercase letters and numbers"
  default     = "cockti"
}

variable "owner" {
  type    = string
  default = "cocktails.api"
}

variable "product" {
  type    = string
  default = "cezzis"
}

variable "environment" {
  type        = string
  description = "Environment name, e.g. 'dev' or 'stage' or 'prod'"
}

variable "region" {
  type    = string
  default = "eus"
}

variable "sub" {
  type        = string
  description = "Subscription short identitifer to be used in resource naming"
  default     = "vec"
}

variable "sequence" {
  type        = string
  description = "The sequence number of the resource typically starting with 001"
  default     = "001"
}
variable "short_sequence" {
  type        = string
  description = "The short sequence number of the resource typically starting with 1"
  default     = "1"
}

variable "global_environment" {
  type        = string
  description = "The global environment name"
  default     = "glo"
}

variable "global_domain" {
  type        = string
  description = "The global domain name"
  default     = "shared"
}

variable "b2c_tenant_id" {
  type        = string
  description = "The b2c tenant id for this environment"
}

variable "b2c_tenant_name" {
  type        = string
  description = "The b2c tenant name (typeically the first part of the hostname)"
}

variable "b2c_signin_policy" {
  type        = string
  description = "The main signin policy for the cocktails webapp"
  default     = "B2C_1_SignInSignUp_Policy"
}

variable "b2c_tenant_domain_name" {
  type        = string
  description = "The b2c tenant domain name for this environment"
}

variable "b2c_terraform_app_registration_client_id" {
  type = string
}

variable "b2c_terraform_app_registration_client_secret" {
  type = string
}

variable "image_tag" {
  type = string
}

# variable "rvecchi_user_account_object_id" {
#   type = string
#   description = "The object id of the user account for rvecchi"
#   default = "83829dad-b43d-486f-9a35-a037afa75d84"
# }

variable "cezzis_site_hostname" {
  type        = string
  description = "The preferred hostname for the website.  (www.cezzis.com)"
}

variable "cezzis_web_client_id" {
  type = string
}

variable "pubsub_sb_topics_cocktails_email" {
  type    = string
  default = "pubsub-sb-topics-cocktails-email"
}

variable "pubsub_sb_queues_cocktails_email" {
  type    = string
  default = "pubsub-sb-queues-cocktails-email"
}

variable "pubsub_sb_topics_cocktails_account" {
  type    = string
  default = "pubsub-sb-topics-cocktails-account"
}

variable "pubsub_sb_queues_cocktails_account" {
  type    = string
  default = "pubsub-sb-queues-cocktails-account"
}

variable "pubsub_sb_topics_cocktail_ratings" {
  type    = string
  default = "pubsub-sb-topics-cocktail-ratings"
}

variable "pubsub_sb_queues_cocktail_ratings" {
  type    = string
  default = "pubsub-sb-queues-cocktail-ratings"
}

variable "binding_cocktails_blob_account_avatars" {
  type    = string
  default = "binding-cocktails-blob-account-avatars"
}

variable "account_avatars_storage_container" {
  type    = string
  default = "account-avatars"
}

variable "cocktail_images_route_hostname" {
  type = string
}

variable "zoho_email_sender_address" {
  type    = string
  default = "cezzi@cezzis.com"
}

variable "allowed_origins" {
  type    = list(string)
  default = []
}

variable "cocktails_cosmosdb_database_name" {
  type    = string
  default = "cocktails-db"
}

variable "custom_domain" {
  type = object({
    sub_domain                    = string
    host_name                     = string
    custom_domain_verification_id = string
  })
}

variable "login_subdomain" {
  type = string
}
