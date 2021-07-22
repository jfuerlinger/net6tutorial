# .net core 6.0 tutorial with kubernetes

- [.net core 6.0 tutorial with kubernetes](#net-core-60-tutorial-with-kubernetes)
  - [Quality](#quality)
  - [Pipeline](#pipeline)
  - [Commands](#commands)
    - [Start the aks cluster](#start-the-aks-cluster)
    - [Stop the aks cluster](#stop-the-aks-cluster)
    - [Get the state of the aks cluster](#get-the-state-of-the-aks-cluster)
    - [Switch between kubernetes contexts](#switch-between-kubernetes-contexts)
      - [-> Context: docker-desktop](#--context-docker-desktop)
      - [-> Context: k8s-cluster-01](#--context-k8s-cluster-01)
  - [Setup script](#setup-script)
  - [Resources](#resources)


## Quality

The quality of the source is checked by SonarQube which is hosted by sonarcloud:

[Dashboard](https://sonarcloud.io/project/overview?id=jfuerlinger_net6tutorial)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=alert_status)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=coverage)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=bugs)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=jfuerlinger_net6tutorial&metric=code_smells)](https://sonarcloud.io/dashboard?id=jfuerlinger_net6tutorial)

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

### Switch between kubernetes contexts

#### -> Context: docker-desktop

```ps
kubectl config use-context docker-desktop
```

#### -> Context: k8s-cluster-01

```ps
kubectl config use-context k8s-cluster-01
```

## Setup script

[Setup](https://gist.github.com/jfuerlinger/e0b4eca486c311451e17dfebe354bd9a)


## Resources

* [Flux - Getting Started](https://fluxcd.io/docs/get-started/)
* [kubectl Spickzettel](https://kubernetes.io/de/docs/reference/kubectl/cheatsheet/)
