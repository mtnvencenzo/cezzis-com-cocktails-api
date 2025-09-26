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

login_subdomain = "login"

# Auth0 Configuration
auth0_domain              = "your-domain.auth0.com"
auth0_audience           = "https://api.yourapp.com"  # Your API Identifier
auth0_client_id          = "your-api-client-id"
auth0_frontend_client_id = "your-spa-client-id"  # For Swagger/Scalar UI