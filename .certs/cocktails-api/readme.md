# Regenerate ssl
```bash
# 1. Generate the certificate with Subject Alternative Names (SANs):
openssl req -x509 -nodes -days 9999 -newkey rsa:2048 -keyout cocktails-api.key -out cocktails-api.crt -config cocktails-api.conf -extensions v3_req

# 2. Install to system trust store (Linux):
sudo cp ./cocktails-api.crt /usr/local/share/ca-certificates/cocktails-api.crt
sudo update-ca-certificates

# 3. Install to Chrome's certificate database (NSS):
sudo apt update && sudo apt install -y libnss3-tools
certutil -d sql:$HOME/.pki/nssdb -A -t "CP,CP," -n "cocktails-api" -i ./cocktails-api.crt

# 4. Verify it was added:
certutil -d sql:$HOME/.pki/nssdb -L | grep cocktails-api

# 5. Optionally convert to a pfx for use with .net and kestrel
openssl pkcs12 -export -out cocktails-api.pfx -inkey cocktails-api.key -in cocktails-api.crt -passout pass:password

# 6. Make sure everythings readable by all users
chmod 644 ./cocktails-api.crt
chmod 644 ./cocktails-api.key
chmod 644 ./cocktails-api.pfx
```