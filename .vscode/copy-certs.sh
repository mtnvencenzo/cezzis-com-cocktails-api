#!/bin/bash
mkdir -p "${1}/.certs"
find "${HOME}/Github/dev-certs/docker-local/" -maxdepth 1 -type f -name "*.crt" -exec cp {} "${1}/.certs/" \;
find "${HOME}/Github/dev-certs/azurite-local/" -maxdepth 1 -type f -name "*.crt" -exec cp {} "${1}/.certs/" \;
