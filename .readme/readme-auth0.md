# Auth0 Setup Guide

This guide walks you through setting up Auth0 authentication for the Cocktails API.

## Overview

The Cocktails API uses Auth0 for authentication and authorization. This provides a more streamlined and developer-friendly authentication experience compared to Azure B2C.

## Prerequisites

- An Auth0 account (free tier available at [auth0.com](https://auth0.com))
- Admin access to your Auth0 dashboard
- Basic understanding of OAuth 2.0 and JWT tokens

## Step 1: Create Auth0 API

1. **Login to Auth0 Dashboard**
   - Go to [manage.auth0.com](https://manage.auth0.com)
   - Sign in to your Auth0 account

2. **Create a New API**
   - Navigate to **Applications** → **APIs**
   - Click **Create API**
   - Fill in the details:
     - **Name**: `Cocktails API`
     - **Identifier**: `https://api.cocktails.com` (or your preferred identifier)
     - **Signing Algorithm**: `RS256`
   - Click **Create**

3. **Configure API Scopes**
   - In your newly created API, go to the **Scopes** tab
   - Add the following scopes:
     - `read:owned-account` - Read account information
     - `write:owned-account` - Write account information,
     - `admin:cezzi-cocktails` - Write account information,
   - Save changes

## Step 2: Create Auth0 Applications

### Machine-to-Machine Application (for API access)

1. **Create M2M Application**
   - Navigate to **Applications** → **Applications**
   - Click **Create Application**
   - Choose **Machine to Machine Applications**
   - Name: `Cocktails API Client`
   - Select your Cocktails API from the dropdown
   - Click **Create**

2. **Configure Permissions**
   - Select the scopes you want this application to have:
     - `read:owned-account`
     - `write:owned-account`
     - `admin:cezzi-cocktails`
   - Click **Authorize**

3. **Note Credentials**
   - Go to the **Settings** tab
   - Note down:
     - **Client ID**
     - **Client Secret**
     - **Domain**

### Single Page Application (for Swagger/Scalar UI)

1. **Create SPA Application**
   - Click **Create Application**
   - Choose **Single Page Applications**
   - Name: `Cocktails API Swagger UI`
   - Click **Create**

2. **Configure Application**
   - In **Settings** tab:
     - **Allowed Callback URLs**: `https://your-api-domain/scalar/v1/oauth-receiver`
     - **Allowed Logout URLs**: `https://your-api-domain`
     - **Allowed Web Origins**: `https://your-api-domain`
     - **Allowed Origins (CORS)**: `https://your-api-domain`
   - Save changes

3. **Note Client ID**
   - This will be used for the Swagger UI authentication

## Step 3: Configure Application Settings

### Update appsettings.local.json

```json
{
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "Audience": "https://api.cocktails.com",
    "ClientId": "your-m2m-client-id",
    "ClientSecret": "your-m2m-client-secret"
  },
  "Scalar": {
    "AuthorizationCodeFlow": {
      "ClientId": "your-spa-client-id",
      "Scopes": [
        "read:owned-account",
        "write:owned-account",
        "admin:cezzi-cocktails"
      ]
    }
  }
}
```

### Update Terraform Variables

Add to your `.tfvars` files:

```hcl
auth0_domain              = "login.cezzis.com"
auth0_audience           = "https://api.cocktails.com"
auth0_client_id          = "your-m2m-client-id"
auth0_frontend_client_id = "your-spa-client-id"
```

## Step 4: Test Authentication

1. **Start the API**
   ```bash
   dotnet run --project src/Cocktails.Api
   ```

2. **Access Swagger UI**
   - Navigate to `https://localhost:7176/scalar/v1`
   - Click on the **Authorize** button
   - You should be redirected to Auth0 for authentication

3. **Test API Endpoints**
   - Try accessing protected endpoints like `/api/v1/accounts/owned/profile`
   - Verify that authentication is required and scopes are properly validated

## Step 5: User Management (Optional)

### Enable User Registration

1. **Database Connection**
   - Go to **Authentication** → **Database**
   - Enable **Username-Password-Authentication** if not already enabled

2. **Configure Sign-up**
   - In the database connection settings
   - Enable **Disable Sign Ups** if you want invite-only
   - Configure password policy as needed

### Custom Login Pages (Optional)

1. **Universal Login**
   - Go to **Branding** → **Universal Login**
   - Customize the login page appearance
   - Add your branding and styling

## Security Best Practices

1. **Environment Variables**
   - Never commit Auth0 credentials to source control
   - Use Azure Key Vault for production secrets
   - Use environment variables for configuration

2. **Token Validation**
   - The API automatically validates JWT tokens from Auth0
   - Tokens are verified against Auth0's public keys
   - Audience and issuer claims are validated

3. **Scope-Based Authorization**
   - Use the minimum required scopes for each endpoint
   - Implement proper error handling for insufficient permissions
   - Consider implementing role-based access control if needed

## Troubleshooting

### Common Issues

1. **CORS Errors**
   - Ensure your domain is added to Allowed Origins in Auth0
   - Check that CORS is properly configured in your API

2. **Token Validation Failures**
   - Verify the audience matches your API identifier
   - Check that the domain is correctly configured
   - Ensure clock synchronization between systems

3. **Scope Errors**
   - Verify scopes are properly defined in your Auth0 API
   - Check that the application has been granted the required scopes
   - Ensure scope names match exactly (case-sensitive)

## Resources

- [Auth0 Documentation](https://auth0.com/docs)
- [Auth0 .NET Quickstart](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi)
- [OAuth 2.0 Scopes](https://auth0.com/docs/get-started/apis/scopes)
- [JWT Token Validation](https://auth0.com/docs/secure/tokens/json-web-tokens/validate-json-web-tokens)