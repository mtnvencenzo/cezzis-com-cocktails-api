terraform {
  required_providers {
    restapi = {
      source  = "Mastercard/restapi"
      version = "=2.0.1"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.24.0"
    }
  }
}

provider "restapi" {
  uri                  = "https://${var.ai_search_service_host_name}.search.windows.net"
  write_returns_object = true
  debug                = true

  headers = {
    "api-key"      = var.ai_search_service_key,
    "Content-Type" = "application/json"
  }

  create_method  = "POST"
  update_method  = "PUT"
  destroy_method = "DELETE"

  copy_keys = [
    "@odata.context",
    "@odata.etag"
  ]
}