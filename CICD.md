# CI/CD setup
In order to ease testing and integration, all code is automatically built and deployed to private Kubernetes cluster. This doc describes what happens and how to set it up for new projects.

## Dockerfile
_Example Dockerfile: [api/ApiGatewayApi/Dockerfile](https://github.com/luka-j/matf-rs2-api-gateway/blob/master/api/ApiGatewayApi/Dockerfile)_

_Quickstart: copy Dockerfile, find and replace ApiGatewayApi with your project name._

First step to creating a usable deployment is packaging the app to a container. We utilize a pretty standard Dockerfile, but with one important quirk:
- Image building is done on amd64 machine, but the final image is run on **arm**64 hardware. 

This architectural difference _is not_ abstracted away by containers: during build process, we cannot execute any arm64 code; similarly, we cannot include any amd64 code in the final image. Luckily, `dotnet build` and `dotnet publish` accept arguments which tell it for which architecture to build the program, regardless of the architecture they're running on, so all we need to do is pick appropriate base image(s).

## Kubernetes resources
_Example resources: [api/k8s-resources](https://github.com/luka-j/matf-rs2-api-gateway/tree/master/api/k8s-resources)_

_Quickstart: Copy deployment.yaml, find and replace `api` with your project key/short name except in `metadata.namespace`, change ports at the bottom of the file appropriately (api microservice exposes 5000 for gRPC, 8080 for HTTP). Copy service.yaml, find and replace `api` with your project key/short name except in `metadata.namespace`, change `targetPort`s to your exposed ports, delete unused one._

In order to actually deploy anything on Kubernetes, it needs to know some details about what should be deployed. Deployment resource specifies what containers should be deployed and how many of them should be deployed ([docs](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)). Service resource tells it how to reach the pods in a deployment: it acts as an internal load balancer which can be reached by other apps inside Kubernetes cluster by its name and namespace ([docs](https://kubernetes.io/docs/concepts/services-networking/service/)). For example, to call gRPC endpoints on API microservice, you'd use `api-service:grpc` as a target host (or `api-service.api-gateway:grpc` if your app is not deployed in `api-gateway` namespace). 

Additionally, if the app needs to be exposed externally (i.e. to the internet), an Ingress resource needs to be defined. This tells nginx, which is running in cluster as a reverse proxy/load balancer, under what circumstances to route traffic to which service. For example, API microservice wants to have all traffic which hits `api-rs2.luka-j.rocks` to be routed to it, so it defines an [ingress resource](https://github.com/luka-j/matf-rs2-api-gateway/blob/master/api/k8s-resources/ingress.yaml). DNS records for this were set up manually (TLS cert is, however, fully managed). You can find Ingress docs [here](https://kubernetes.io/docs/concepts/services-networking/ingress/). Do **NOT** define Services with `type: LoadBalancer`: this provisions LoadBalancer on cloud provider, which makes my billing alerts go off, as it's a paid resource. 

_N. B. this was a **very** brief and imprecise summary of how specific parts of Kubernetes work._

## Argo application
_Example resource: [infra/2-argo-apps/api-app.yaml](https://github.com/luka-j/matf-rs2-api-gateway/blob/master/infra/2-argo-apps/api-app.yaml)_

_Quickstart: Make a copy of api-app.yaml in the same directory and replace `api` in its name with your project code. Change `metadata.name` to reflect your project name, and `spec.source.path` to the path where you saved your Kubernetes resources._

Kubernetes resources from the last section unfortunately don't apply themselves automatically. For that purpose, we use [ArgoCD](https://argoproj.github.io/cd/), which monitors paths inside a repo we tell it to, takes all yaml files from there, and applies them to a cluster. By creating an Application resource inside `infra/2-argo-apps` directory, you tell Argo to monitor one more path. GitHub repo is set up to notify ArgoCD (via webhook) when anything is pushed to it, so Argo knows when to pull the files. To monitor this process using a GUI, ping luka-j for dashboard credentials.

Files in `infra/2-argo-apps` _do_ get applied automatically: there's another Application in `infra/1-kubernetes-setup-resources/35-argo-app-of-apps` which tells Argo to monitor this directory. It was applied to cluster manually during setup. It's Argo Applications all the way down.

## GitHub Action
_Example resource: [.github/workflows/api-pipeline.yaml](https://github.com/luka-j/matf-rs2-api-gateway/blob/master/.github/workflows/api-pipeline.yaml)_

_Quickstart: Make a copy of api-pipeline.yaml in the same directory and replace `api` in its name with your project code. Replace `on.push.paths` with the paths relevant to your microservice. Replace `-api` in `env.IMAGE_NAME` with your microservice code. Set values of `env.DOCKERFILE_DIR` and `env.DEPLOYMENT_PATH` to appropriate values for your microservice._ 

To tie everything together, we use [GitHub Actions](https://docs.github.com/en/actions). An action ordinarily triggers on push to master and, optionally, some feature branch, when any modification to microservice code or action itself is made. It consists of two jobs: 
- `build-and-push-image` builds an image using Dockerfile you wrote in first step, tags it with commit hash, and pushes it to GitHub's registry.
- `trigger-deployment` one edits Deployment resource (from second step) to tell it to pull the newly built image. It finds sets the `image` property in `spec.template.spec.containers` to reference the new image. This in turn triggers Argo to pull the new image and deploy new version of the app.

Note that there's no concept of "release", "snapshot" or "staging"/"preview" whatsoever. It's unnecessarily burdensome and provides no benefits to a project with zero live traffic: the main goal of this setup is to speed up the development process. This means that latest version is always served. If a rollback is needed, one can edit the deployment file manually and push it.
