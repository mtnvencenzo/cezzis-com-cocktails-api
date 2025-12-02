# Regenerate ssl
```bash
# 1. Generate the certificate with Subject Alternative Names (SANs):
openssl req -x509 -nodes -days 9999 -newkey rsa:2048 -keyout local.key -out local.crt -config localhost.conf -extensions v3_req

# 2. Install to system trust store (Linux):
sudo cp ./local.crt /usr/local/share/ca-certificates/local-dev.crt
sudo update-ca-certificates

# 3. Install to Chrome's certificate database (NSS):
sudo apt update && sudo apt install -y libnss3-tools
certutil -d sql:$HOME/.pki/nssdb -A -t "CP,CP," -n "localhost-dev" -i ./local.crt

# 4. Verify it was added:
certutil -d sql:$HOME/.pki/nssdb -L | grep localhost

# 5. Optionally convert to a pfx for use with .net and kestrel
openssl pkcs12 -export -out local.pfx -inkey local.key -in local.crt -passout pass:password
```