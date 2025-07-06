locals {
  tags = {
    Product     = var.product
    Environment = var.environment
    Application = var.domain
    Owner       = var.owner
  }

  ai_search_service_host_name = "srch-${var.sub}-${var.region}-${var.global_environment}-${var.global_domain}-${var.sequence}"

  apim_anonomous_operation_policy = <<XML
      <policies>
        <inbound>
          <set-backend-service backend-id="${var.environment}-${var.domain}-api-backend" />
          <include-fragment fragment-id="${var.environment}-${var.domain}-api-cors-policy" />
        </inbound>
        <backend><base /></backend>
        <outbound><base /></outbound>
        <on-error><base /></on-error>
      </policies>
    XML

  apim_b2c_auth_operation_policy = <<XML
      <policies>
        <inbound>
          <include-fragment fragment-id="${var.environment}-${var.domain}-api-b2c-policy" />
          <set-backend-service backend-id="${var.environment}-${var.domain}-api-backend" />
          <include-fragment fragment-id="${var.environment}-${var.domain}-api-cors-policy" />
        </inbound>
        <backend><base /></backend>
        <outbound><base /></outbound>
        <on-error><base /></on-error>
      </policies>
    XML

  apim_anonomous_docs_operation_policy = <<XML
      <policies>
        <inbound>
          <set-backend-service backend-id="${var.environment}-${var.domain}-api-docs-backend" />
        </inbound>
        <backend><base /></backend>
        <outbound><base /></outbound>
        <on-error><base /></on-error>
      </policies>
    XML
}