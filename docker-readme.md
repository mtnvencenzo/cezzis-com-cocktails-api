# Docker Builds

## Cocktails Api
``` bash
docker build -t cocktails-api -f ./src/Cocktails.Api/Dockerfile .

docker run -p 3005:8080 cocktails-api
```
