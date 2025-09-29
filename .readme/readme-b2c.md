
# Azure Entra External ID Tenant Setup
The Azure Entra External Id Tenant needs to first be created manually in the resource group of your choice.

> This is largely due to the way the tenant needs to be provisioned before terraform can even access any of the values.
> Also, at the time I beleive the newly create tenant would be owned by the pipeline principal which will cause issues later on.
> So it's better to just login to Azure manually and create the tenant using an admin account.

### Step 1 (create the tenant)
Login to Azure portal as an admin and create the Azure Entra External Id tenant in the resource group of your choosing.

### Step 2 (setup a terraform app registration in the tenant)
Terraform will need to have it's own app registration in the tenant so
it can authenticate and manage resources in the tenant. This should be done manually  

> Later on, Terraform will need to authenticate to the tenant using the `azuread` provider.

1. Create the app registration in the new tenant. (name similar to: `appr-vec-eus-prd-cocktailsterraform-001`)
	- Use `Accounts in this organizational directory only`	
	- Use `Allow public client flows (native/web) - No need to provide a redirect uri`  
<!-- 2. Create a service principal for the app by clicking 'Create Service Principal' from the overview page. -->
3. Create a secret in the app registration (name: Entra External Id Tenant Terraform Client Secret)
4. Set the following api permissions
	- `Azure Active Directory Graph`
		- Application.ReadWrite.All (Application)
		- Application.ReadWrite.OwnedBy (Application)
		- Directory.Read.All (Application)
	- `Microsoft.Graph`
		- Application.ReadWrite.All (Application)
		- Application.ReadWrite.OwnedBy (Application)
		- AppRoleAssignment.ReadWrite.All (Application)
		- Directory.Read.All (Application)
		- Directory.ReadWrite.All (Application)
		<!-- - Group.ReadWrite.All (Application)
		- GroupMember.ReadWrite.All (Application) -->
		- offline_access (Delegated)
		- openid (Delegated)


### Step 3 (create the Entra External Id Tenant User Flows and Login Experience)
This must be done before running any application terraforms because APIM policy fragments referencing the Login Policys of the tenant will validate that it exists first (terraform will fail during applying if the user flow has not already been setup)

> The user flow must be manually configured outside of terraform

1. Open the tenant and click on `Policies > User Flows` and then click on `New User Flow`.
2. Choose the `Sign up and Sign in` user flow type and then choose the `Recommended` version (not the legacy) Then click `Create`
- Use the name `sisu-p` which should give it a full name of `sisu-p`.
- Select `Local accounts` > `Email signup` as the identity provider.
- Type of method shoud be `Email`
- Mfa enforcement should be `Off`
- Enforce conditional access policies should not be checked
- Select these user attributes (for both collecting and returning in claims)
  - Country/Region
  - Email address
  - Given name
  - Postal Code
  - Surname
- Click `Create`
- Click on the newly created policy to further configure it.
- Click on properties and verify these settings
  - **Token Lifetime**
    - Access and Id Token lifetime > `60` minutes
    - Refresh token lifetime > `14` days
    - Refresh token sliding window lifetime `Bounded`
    - Lifetime length `90` days
  - **Token compatibility settings**
    - Issuer (iss) claim > `https://<domain>/<<The entra external id tenant id>>`
	- Subject (sub) claim > `ObjectId`
	- Claim representing user flow > `tfp`
  - **Session Behavior**
    - Web app session lifetime minutes > `1440`
	- Web app session timeout > `Rolling`
	- Single Signon Configuration > `Tenant`
	- Require ID token in logout requests > `No`
	- Enforce SSO logout validation > `No`
	- Enable keep me signed in session > `unchecked`
  - **Password Configuration**
    - Self service password reset > `checked`
	- Forced password reset > `checked`
	- Password Complexity > `strong`
  - **Captcha**
    - All disabled...
- Click on Application claims and verify these claims are checked
  - City
  - Country/Region
  - Display Name
  - Email Addresses
  - Given Name
  - Identity Provider (defaulted)
  - Identity Provider Access Token
  - Postal Code
  - State/Province
  - Street Address
  - Surname
  - User is new
  - User's Object ID
- Click on `Customize` > `Page Layouts`
  - Customize `Unified sign up or sign in page`
    - Use Custom Page Content > `Yes`
	- Custom Page Uri > `https://stvecprdcocktailsimages1.blob.core.windows.net/cocktails/azure-ciam-signinsignup-content.html`
	- Page Layout Version `2.1.9`
  - Use version `2.1.9` for the following pages (no custom content)
    - Local account sign up page
	- Multifactor authentication page using email
	- Blocked page due to Conditional Access
	- Blocked page due to API connector ShowBlockPage action
	- Update expired password page
	- Forgot password page
	- Change password page
  - Use version `1.2.6` for the following pages (no custom content)
    - Error page
- Click on `Company Branding` > `Default sign-in` and click `Edit`
  - In the `Basics` tab
    - Change the background image to `D:\GIT\Latest\Cocktails-Images\Images\AzureCiam\azure-ciam-background.png`
	- The background color should be white #ffffff (255,255,255)
  - In the `Sign-in form` tab
    - Change the banner logo image to > `D:\GIT\Latest\Cocktails-Images\Images\AzureCiam\azure-ciam-banner-245x60.png`
	- Square logo (light theme) should be set to `D:\GIT\Latest\Cocktails-Images\Images\AzureCiam\azureciamsquare-240x240.png`
	- Sign-in page text should be set to `Cancel and go back to [Cezzis.com](https://www.cezzis.com)` 
- Everything under the Security Menu is defaulted and not customized



### Step 4 (update the tenant and terraform app environment variables)
Update all the $/deployment/iac/terraform/environment_vars/\<env>.tfvars files with the values from the newly setup environment tenant from the first run of terraform.

Update the $/deployment/iac/terraform/main.tf file and uncomment the `azuread` provider

``` text
ciam_tenant_id					= "d96c6cca-626a-49e7-98c8-fcc2f7220123"
ciam_tenant_name					= "cezzis"
ciam_tenant_domain_name			= "cezzis.onmicrosoft.com"
terraform_app_client_id			= "2957a4f0-bd36-4a35-90a1-96322a5eb321"
terraform_app_client_secret		= "123Q~cdMjE-2346d_5fHD-SifIQrFNAfg234"
```


### Step 5 (good to go)
If all goes well, should be able to run any piplines/apps that need the ciam setup