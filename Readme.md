# .net core 6.0 tutorial with kubernetes

## Pipeline

[![Docker Image CI](https://github.com/jfuerlinger/net6tutorial/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/jfuerlinger/net6tutorial/actions/workflows/build-and-deploy.yml)


## Commands

### Start the aks cluster

```ps
az aks start --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg
```

### Stop the aks cluster

```ps
az aks stop --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg
```

### Get the state of the aks cluster

```ps
az aks show --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg | ConvertFrom-Json | Select-Object Powerstate
```

## Setup script

[Setup](https://gist.github.com/jfuerlinger/e0b4eca486c311451e17dfebe354bd9a)