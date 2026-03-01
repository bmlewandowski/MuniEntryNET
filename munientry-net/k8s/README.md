# Kubernetes Manifests

> **Status:** Development / pre-deployment scaffolding.  
> These manifests are ready to use but the application has not yet been deployed to a cluster.

## Structure

```
k8s/
├── namespace.yaml           # munientry namespace
├── configmap.yaml           # Non-sensitive environment config
├── secret.yaml              # Secret TEMPLATE — never commit with real values
├── ingress.yaml             # nginx Ingress (path-based routing on munientry.local)
├── kustomization.yaml       # Kustomize root — apply everything at once
├── api/
│   ├── deployment.yaml      # ASP.NET Core API (port 80)
│   └── service.yaml         # ClusterIP service for the API
└── client/
    ├── deployment.yaml      # Blazor WASM client served by nginx (port 80)
    └── service.yaml         # ClusterIP service for the client
```

## Prerequisites

| Tool | Purpose |
|---|---|
| `kubectl` | Apply manifests |
| `kustomize` (or `kubectl` ≥ 1.14) | Kustomize support |
| Ingress controller | Route external traffic (nginx recommended) |
| Container registry | Host built images |

## Local Development with Minikube

### 1. Start Minikube and enable the Ingress addon

```bash
minikube start
minikube addons enable ingress
```

### 2. Build images and load them into Minikube (no registry needed)

```bash
# From munientry-net/
docker build -t munientry-api:latest -f api/Dockerfile .
docker build -t munientry-client:latest ./client

minikube image load munientry-api:latest
minikube image load munientry-client:latest
```

### 3. Create the Secret from the template

```bash
# Copy the template, fill in real values, then apply.
# Do NOT commit the secrets file.
cp k8s/secret.yaml k8s/secret.local.yaml
# Edit secret.local.yaml with real values, then:
kubectl apply -f k8s/secret.local.yaml
```

### 4. Apply all other manifests via Kustomize

```bash
kubectl apply -k k8s/
```

### 5. Add a hosts entry for local testing

```
# Windows: C:\Windows\System32\drivers\etc\hosts
# Linux/Mac: /etc/hosts
127.0.0.1  munientry.local
```

Then run `minikube tunnel` in a separate terminal and open http://munientry.local.

---

## Local Development with kind

```bash
kind create cluster --name munientry
# Install nginx ingress controller for kind:
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml

# Build and load images
docker build -t munientry-api:latest -f api/Dockerfile .
docker build -t munientry-client:latest ./client
kind load docker-image munientry-api:latest --name munientry
kind load docker-image munientry-client:latest --name munientry

kubectl apply -f k8s/secret.local.yaml
kubectl apply -k k8s/
```

---

## Routing

All traffic enters through the Ingress at `munientry.local`:

| Path | Service | Notes |
|---|---|---|
| `/api/*` | `munientry-api:80` | ASP.NET Core; routes already prefixed with `/api/` |
| `/*` | `munientry-client:80` | Blazor WASM SPA via nginx; SPA fallback to `index.html` |

---

## Secrets Management

`secret.yaml` is a **template only**. For real deployments consider:

- **Sealed Secrets** — encrypt secrets at rest in Git  
- **Azure Key Vault** + the CSI driver (natural fit if deploying to AKS)  
- **External Secrets Operator** — generic secret sync from any vault

---

## Next Steps Before Production

- [ ] Replace `image: munientry-api:latest` / `munientry-client:latest` with fully-qualified registry paths
- [ ] Set `imagePullPolicy: Always` and configure `imagePullSecrets` if using a private registry
- [ ] Choose a secrets management strategy (Sealed Secrets / Key Vault / ESO)
- [ ] Add TLS to the Ingress (`cert-manager` + Let's Encrypt or an internal CA)
- [ ] Tune resource `requests` / `limits` based on load testing
- [ ] Add `PodDisruptionBudget` and increase `replicas` for production HA
- [ ] Integrate with a CI/CD pipeline (GitHub Actions, Azure DevOps) to build, push, and deploy
