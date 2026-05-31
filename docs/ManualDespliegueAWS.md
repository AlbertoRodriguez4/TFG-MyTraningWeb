# Manual de Despliegue en AWS

## 1. Resumen de la Arquitectura

El proyecto se compone de tres capas principales desplegadas en Amazon Web Services (AWS):

- **Frontend (Vue 3 + Nginx)**: Contenedor Docker servido mediante un Deployment de Kubernetes (EKS) expuesto por un LoadBalancer.
- **Backend (.NET 8 / C#)**: API REST contenerizada y desplegada en EKS, expuesta por un LoadBalancer independiente.
- **Base de Datos (PostgreSQL)**: Se recomienda usar **Amazon RDS for PostgreSQL** en produccion. En los manifiestos de prueba existe un contenedor de Postgres y pgadmin, pero estos son solo para desarrollo local o entornos de pruebas temporales.

## 2. Servicios de AWS Utilizados

| Servicio AWS | Proposito |
|--------------|-----------|
| **Amazon EKS** | Cluster de Kubernetes que orquesta los contenedores del Front, Back y utilidades. |
| **Amazon RDS** | Base de datos PostgreSQL gestionada (recomendado en produccion). |
| **Elastic Load Balancing (ELB)** | Balanceo de trafico generado automaticamente por los Services de tipo LoadBalancer en Kubernetes. |
| **Amazon ECR / Docker Hub** | Registro de imagenes Docker. El pipeline usa Docker Hub. |
| **AWS IAM** | Gestion de permisos y credenciales para el cluster y los pipelines. |
| **AWS Secrets Manager** (opcional) | Almacenamiento seguro de credenciales y connection strings. |

## 3. Prerrequisitos

Antes de iniciar el despliegue, asegurate de contar con lo siguiente:

1. **Cuenta de AWS activa**.
2. **Cluster EKS creado** (por ejemplo, `mi-cluster` en `us-east-1`).
3. **AWS CLI** instalado y configurado localmente:
   ```bash
   aws configure
   ```
4. **kubectl** instalado y configurado para comunicarse con el cluster:
   ```bash
   aws eks update-kubeconfig --name mi-cluster --region us-east-1
   ```
5. **Docker Hub** con las imagenes creadas:
   - Frontend: `albertorodriguezpenalva/front-aws:latest`
   - Backend: `albertorodriguezpenalva/back-aws:latest`
6. **Instancia RDS de PostgreSQL** (recomendado para produccion) con la base de datos `postgres` creada.

## 4. Configuracion de Secrets en GitHub

El pipeline de GitHub Actions requiere los siguientes secrets configurados en el repositorio:

| Secret | Descripcion |
|--------|-------------|
| `DOCKER_USERNAME` | Usuario de Docker Hub. |
| `DOCKER_PASSWORD` | Token o contrasena de Docker Hub. |
| `AWS_ACCESS_KEY_ID` | Access Key de un usuario IAM con permisos sobre EKS y ELB. |
| `AWS_SECRET_ACCESS_KEY` | Secret Key correspondiente. |
| `AWS_SESSION_TOKEN` | Token de sesion (requerido para cuentas de AWS Academy / estudiantes). |

### Como configurarlos:
1. En GitHub, ve a **Settings > Secrets and variables > Actions > New repository secret**.
2. Agrega cada uno de los secrets listados arriba.

## 5. Estructura de Manifiestos de Kubernetes

Los manifiestos se encuentran en:

```
Frontend/manifest/front-deployment.yml
Backend/manifest/api-deployment.yml
Backend/manifest/postgres-deployment.yml   # Solo dev/local
Backend/manifest/pgadmin-deployment.yml      # Solo dev/local
```

### 5.1 Frontend (`front-deployment.yml`)
- **Deployment**: 1 replica del contenedor Nginx con la SPA compilada.
- **Service**: Tipo `LoadBalancer` que expone el puerto 80.

### 5.2 Backend (`api-deployment.yml`)
- **Deployment**: 1 replica del contenedor .NET 8.
- **Service**: Tipo `LoadBalancer` que expone el puerto 6873.
- **Variables de entorno**:
  - `ASPNETCORE_ENVIRONMENT=Development` (cambiar a `Production` si es necesario).
  - `ConnectionStrings__DefaultConnection`: Connection string a PostgreSQL.

> **Nota de seguridad**: En produccion, no incluyas la contrasena directamente en el manifiesto. Usa **AWS Secrets Manager** o **Kubernetes Secrets**.

## 6. Despliegue Manual (sin CI/CD)

Si necesitas desplegar manualmente desde tu maquina local:

### 6.1 Frontend
```bash
# Construir y subir imagen
cd Frontend
docker build -t albertorodriguezpenalva/front-aws:latest .
docker push albertorodriguezpenalva/front-aws:latest

# Desplegar en EKS
kubectl apply -f manifest/front-deployment.yml
kubectl rollout restart deployment front
```

### 6.2 Backend
```bash
# Construir y subir imagen
cd Backend
docker build -t albertorodriguezpenalva/back-aws:latest .
docker push albertorodriguezpenalva/back-aws:latest

# Desplegar en EKS
kubectl apply -f manifest/api-deployment.yml
kubectl rollout restart deployment api
```

## 7. Despliegue Automatico con GitHub Actions (CI/CD)

El repositorio incluye dos workflows en `.github/workflows/`:

1. **`docker-image-cicd-front.yml`**: Se ejecuta cuando hay cambios en la carpeta `Frontend/**`.
2. **`docker-image-cicd-back.yml`**: Se ejecuta cuando hay cambios en la carpeta `Backend/**`.

### Flujo del pipeline:
1. **Checkout** del codigo.
2. **Buildx** de Docker.
3. **Login** en Docker Hub.
4. **Build y Push** de la imagen con tag `latest`.
5. **Configuracion de credenciales AWS**.
6. **Update de kubeconfig** para conectar con EKS.
7. **Aplicacion de manifiestos** (`kubectl apply`).
8. **Restart del deployment** para forzar la descarga de la nueva imagen.

### Activar el pipeline:
Simplemente realiza un `push` o `merge` a la rama `main` con cambios en `Frontend/` o `Backend/`.

## 8. Verificacion del Despliegue

Una vez desplegado, verifica que todo este funcionando:

```bash
# Ver pods en ejecucion
kubectl get pods

# Ver servicios y URLs publicas (LoadBalancers)
kubectl get svc

# Ver logs del backend
kubectl logs -l app=api --tail=100

# Ver logs del frontend
kubectl logs -l app=front --tail=100
```

Los servicios de tipo `LoadBalancer` mostraran una columna `EXTERNAL-IP`. Esa IP o DNS es la URL publica de cada componente.

## 9. Conexion Frontend <> Backend

Asegurate de que el frontend apunte a la URL publica del backend. En el proyecto Vue, esto suele estar configurado en las variables de entorno o en el store de API:

```
VITE_API_URL=http://<EXTERNAL-IP-DEL-BACKEND>:6873
```

Si usas el mismo dominio y un Ingress, podrias rutar `/api` al backend y `/` al frontend.

## 10. Escalado

Para escalar el numero de replicas:

```bash
kubectl scale deployment front --replicas=3
kubectl scale deployment api --replicas=3
```

## 11. Limpieza (opcional)

Para eliminar los recursos del cluster:

```bash
kubectl delete -f Frontend/manifest/front-deployment.yml
kubectl delete -f Backend/manifest/api-deployment.yml
```

## 12. Checklist de Produccion

- [ ] Cambiar `ASPNETCORE_ENVIRONMENT` a `Production`.
- [ ] Usar **Amazon RDS** en lugar del contenedor de Postgres local.
- [ ] Mover el connection string a **AWS Secrets Manager** o **Kubernetes Secrets**.
- [ ] Configurar **dominio personalizado** y **SSL/TLS** (AWS Certificate Manager + Ingress).
- [ ] Habilitar **monitoring** con CloudWatch o Prometheus/Grafana.
- [ ] Revisar politicas de **IAM** para aplicar el minimo privilegio.
