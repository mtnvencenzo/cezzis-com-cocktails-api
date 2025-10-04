# Dapr
https://docs.dapr.io  
https://docs.dapr.io/getting-started  
https://docs.dapr.io/getting-started/quickstarts/  

## State management with dapr
### Create state key/value in dapr

``` bash
curl -X POST -H "Content-Type: application/json" -d '[{ "key": "name", "value": "Bruce Wayne"}]' http://localhost:3500/v1.0/state/statestore
```

### Get the state value for a given key
``` bash
curl http://localhost:3500/v1.0/state/statestore/name
```

### Check into the redis container at how state is stored

``` bash
winpty docker exec -it dapr_redis redis-cli
# then enter to view the keys
keys *
# then enter to view values of a given key
hgetall "myapp||name"
# to exit redis
exit
```
* note winpty is needed for TTY exec in gitbash

## Dapr Run (multi instance)
In a directory that contains a dapr.yaml file
``` bash
dapr run -f .
```


# Dapr test endpoints
https://localhost:7176/api/v1/dapr/invoke/func/http
https://localhost:7176/api/v1/dapr/invoke/func/grpc  
https://localhost:7176/api/v1/dapr/verify/state  