### Azurite Local Docker Setup (HTTP, No Cert Required)

This guide will help you set up Azurite in a Docker container and connect to it using Azure Storage Explorer.

#### 1. Start Azurite Docker Container (HTTP)

Open PowerShell (recommended for volume mapping on Windows) and run:

```powershell
docker run --restart=always -d --name azurite-cocktails -p 10000:10000 -v $HOME/.azurite/cezzis/:/workspace mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0
```
- Make sure the path `$HOME/.azurite/cezzis/` exists on your system.
- This will start Azurite and persist data in the `$HOME/.azurite/cezzis/` directory.

#### 2. Connect with Azure Storage Explorer

1. Open Azure Storage Explorer.
2. Click **Connect to Azure Storage**.
3. Select **Attach to a local emulator** or **Use a connection string**.
4. Use the following connection string:

```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;
```

#### 3. Create Blob Containers
- You can now create blob containers and manage blobs without certificate or HTTPS issues.

#### 4. (Optional) Public Access
- By default, blob containers are private. To set public access, right-click the container in Storage Explorer and set the desired access level.

---

### Troubleshooting
- If you need HTTPS/certificates, see the official Azurite docs, but note that certificate trust issues can cause problems with Storage Explorer.
- If you get `net::ERR_EMPTY_RESPONSE` or similar errors, try using HTTP as above.

---

### Well-known Storage Account and Key
Azurite accepts the same well-known account and key used by the legacy Azure Storage Emulator.

```
Account name: devstoreaccount1
Account key: Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==
```

---

For more info:
- https://github.com/Azure/Azurite/blob/main/README.md
- https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite
- https://azure.microsoft.com/en-us/products/storage/storage-explorer
