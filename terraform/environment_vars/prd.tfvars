environment                    = "prd"
b2c_tenant_id                  = "fb512a9c-5b44-41ab-8042-6465769f16c9"
b2c_tenant_name                = "cezzis"
b2c_tenant_domain_name         = "cezzis.onmicrosoft.com"
cezzis_site_hostname           = "www.cezzis.com"
cezzis_web_client_id           = "84744194-da27-410f-ae0e-74f5589d4c96"
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