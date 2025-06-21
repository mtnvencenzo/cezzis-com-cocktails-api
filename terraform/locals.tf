locals {
  tags = {
    Product     = var.product
    Environment = var.environment
    Application = var.domain
    Owner       = var.owner
  }
  ai_search_service_host_name = "srch-${var.sub}-${var.region}-${var.global_environment}-${var.global_domain}-${var.sequence}"
}