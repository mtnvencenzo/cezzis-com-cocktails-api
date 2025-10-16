
## Instructions for linux based image
https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux


## Download the cosmos db image
``` bash
docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview
```

## Run the cosmos container
``` bash
docker run --restart=always -d --name cosmos-cocktails -p 8081:8081 -p 1234:1234 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview --protocol https

docker ps
```

Note that the data explorer runs on the 1234 port http://localhost:1234/

The db runs on 8081 and its good practice to trust the certificate issued from CosmosDb

``` Trust the cosmos cert
curl --insecure https://localhost:8081/_explorer/emulator.pem > ~/emulatorcert.crt
sudo cp ~/emulatorcert.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates

```

Https is required - see the --protocol https argument.  Note that we are disabling the certificate check in the cosmos setup when running locally though.

```csharp
                optionsBuilder.UseCosmos(
                    connectionString: options.ConnectionString,
                    databaseName: options.DatabaseName,
                    cosmosOptionsAction: options =>
                    {
                        options.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
                        options.LimitToEndpoint(true);
                        options.HttpClientFactory(() =>
                        {
                            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                            };

                            return new HttpClient(httpMessageHandler);
                        });
                    });
```

## Setup the database
- Create a db called **cocktails-db**
- Create container called **cezzis-\<env\>-accounts-account** *(w/partionkey=/subjectId)*
- Create container called **cezzis-\<env\>-cocktails-cocktail** *(w/partionKey=/id)*
- Create container called **cezzis-\<env\>-cocktails-ingredient** *(w/partionKey=/id)*


- Connection string:
   ```
   AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;
   ```
   - For detailed configuration and troubleshooting, see the [Cosmos DB Emulator Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/linux-emulator)


## Random Az Cli
**Cosmos Control Plane Roles / Info**

https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-grant-control-plane-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli

``` bash
az login

## List role definitions
az cosmosdb sql role definition list --resource-group "rg-vec-eus-prd-cocktails-001" --account-name "cosmos-vec-eus-prd-cocktails-001"

## Add the standard built in reader
az cosmosdb sql role assignment create --account-name cosmos-vec-eus-prd-cocktails-001 --resource-group 'rg-vec-eus-prd-cocktails-001' --role-definition-name 'Cosmos DB Built-in Data Reader' --scope '/subscriptions/1d9ecc00-242a-460d-8b08-b71db19f094e/resourceGroups/rg-vec-eus-prd-cocktails-001/providers/Microsoft.DocumentDB/databaseAccounts/cosmos-vec-eus-prd-cocktails-001' --principal-id 'd532fa56-2a0d-4dc0-82e1-b35ca21a6709'

# Add the custom data reader role
az cosmosdb sql role assignment create --account-name cosmos-vec-eus-prd-cocktails-001 --resource-group 'rg-vec-eus-prd-cocktails-001' --role-definition-name 'Cosmos DB Custom Data Reader' --scope '/subscriptions/1d9ecc00-242a-460d-8b08-b71db19f094e/resourceGroups/rg-vec-eus-prd-cocktails-001/providers/Microsoft.DocumentDB/databaseAccounts/cosmos-vec-eus-prd-cocktails-001' --principal-id 'd532fa56-2a0d-4dc0-82e1-b35ca21a6709'

# List role assignments
az cosmosdb sql role assignment list --account-name cosmos-vec-eus-prd-cocktails-001 --resource-group rg-vec-eus-prd-cocktails-001


# Delete a role assignment
az cosmosdb sql role assignment delete --account-name cosmos-vec-eus-prd-cocktails-001 --resource-group 'rg-vec-eus-prd-cocktails-001' --role-assignment-id '/subscriptions/1d9ecc00-242a-460d-8b08-b71db19f094e/resourceGroups/rg-vec-eus-prd-cocktails-001/providers/Microsoft.DocumentDB/databaseAccounts/cosmos-vec-eus-prd-cocktails-001/sqlRoleAssignments/52661a55-c24a-4c89-89ef-c8c89f9baee4'

```