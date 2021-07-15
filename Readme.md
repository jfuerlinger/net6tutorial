# .net core 6.0 tutorial with kubernetes

## Commands

### Start the aks cluster

```ps
az aks start --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg
```

### Stop the aks cluster

```ps
az aks stop --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg
```

### Gget the state of the aks cluster

```ps
az aks show --name k8s-cluster-01 --resource-group kubernetes-gettingstarted-rg | ConvertFrom-Json | Select-Object Powerstate
```