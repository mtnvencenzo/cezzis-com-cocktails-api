output "index_name" {
  value = var.index_name
}

output "fqdn_host_name" {
  value = "https://${var.ai_search_service_host_name}.search.windows.net"
}