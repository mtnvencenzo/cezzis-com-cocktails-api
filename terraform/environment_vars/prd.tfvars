environment                    = "prd"
cezzis_site_hostname           = "www.cezzis.com"
cocktail_images_route_hostname = "cdn.cezzis.com"

custom_domain = {
  sub_domain                    = "api"
  host_name                     = "api.cezzis.com"
  custom_domain_verification_id = "ON/RjTfRSsktKH7V4C72GfBS43q8kLLDeZKVNLjohvA="
}

allowed_origins = [
  "https://www.cezzis.com",
  "https://cezzis.com",
  "http://localhost:4000",
  "https://localhost:4001"
]


# Auth0 Configuration
auth0_domain                  = "cezzis.us.auth0.com"
auth0_audience                = "https://cezzis-cocktails-api"     # Your API Identifier
auth0_client_id               = "JtxZM9RiJVHejv6xqkFYdFtdfUrX6N09" # M2M Client Api for API
auth0_frontend_client_id      = "HZXyiZxjHgkQqeb6UJxmikCGYpSz5iPb" # For Swagger/Scalar UI
auth0_custom_domain_cname     = "cezzis-cd-skhnnuczfndoqumy.edge.tenants.us.auth0.com"
auth0_custom_domain_subdomain = "login"