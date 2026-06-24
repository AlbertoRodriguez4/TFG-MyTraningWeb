# 🏋️‍♂️ The Training Hub

![Vue.js](https://img.shields.io/badge/Vue.js_3-4FC08D?style=for-the-badge&logo=vuedotjs&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL_17-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**The Training Hub** es una plataforma web integral de gestión deportiva y fitness desarrollada como Trabajo de Fin de Grado (TFG). Diseñada bajo una arquitectura SaaS, la aplicación gamifica la experiencia del entrenamiento personal, combinando rutinas de alta intensidad, seguimiento biométrico, y la asistencia de un entrenador impulsado por Inteligencia Artificial.

---

## 🚀 Características Principales

### 🤖 Inteligencia Artificial & Entrenamiento
- **Coach AI:** Asistente inteligente integrado para la generación y autorregulación de rutinas de entrenamiento basadas en el progreso del usuario.
- **Planes Personalizados:** Motor de asignación, creación y seguimiento detallado de volumen, intensidad (RIR/RPE) y tareas.
- **Métricas Corporales:** Dashboard analítico interactivo para el registro y visualización del progreso físico (peso, medidas, RM).

### 🎮 Comunidad y Gamificación
- **Salas (Rooms):** Espacios virtuales para la creación de grupos de entrenamiento, permitiendo a los usuarios interactuar, competir y compartir estadísticas.
- **Logros (Achievements):** Sistema de recompensas e hitos diseñado para maximizar la retención y motivación.
- **Geolocalización Deportiva:** Integración de mapas interactivos para ubicar centros de alto rendimiento o rutas al aire libre.

### ⚙️ Core y Monetización
- **Gestión Segura:** Autenticación robusta basada en JWT y encriptación BCrypt, con gestión de perfiles de usuario.
- **Suscripciones y Marketplace:** Pasarela de pagos integrada para membresías premium y tienda de ítems virtuales.

---

## 🛠️ Stack Tecnológico

El proyecto está diseñado con una separación de responsabilidades clara entre el cliente y la API, orquestado mediante contenedores para facilitar su despliegue y escalabilidad.

### Frontend (Cliente Web)
- **Framework:** [Vue.js 3](https://vuejs.org/) (Composition API) + TypeScript
- **Bundler:** [Vite](https://vitejs.dev/)
- **UI & UX:** [Vuetify 3](https://vuetifyjs.com/) y SCSS
- **State Management:** [Pinia](https://pinia.vuejs.org/)
- **Librerías Clave:** `chart.js` (Analítica), `leaflet` (Mapas), `vue-i18n` (Multi-idioma).

### Backend (API REST)
- **Framework:** .NET 8.0 (ASP.NET Core Web API) + C#
- **Base de Datos:** PostgreSQL 17
- **ORM:** Entity Framework Core
- **Seguridad:** JSON Web Tokens (JWT)
- **Documentación:** Swagger (Swashbuckle)

---

## 📁 Arquitectura del Proyecto

```text
TFG-MyTraningWeb/
├── Backend/                 # Código fuente de la API (.NET 8)
│   ├── Controllers/         # Endpoints REST
│   ├── Database/            # DbContext y migraciones
│   ├── JWT/                 # Servicios de seguridad y tokens
│   └── Model/               # Entidades y DTOs
├── Frontend/                # Código fuente del cliente web (Vue 3 / Vite)
│   ├── src/                 # Componentes, vistas, stores (Pinia), composables
│   └── public/              # Assets estáticos
├── .env.example             # Plantilla de variables de entorno (API Keys, DB)
└── docker-compose.yml       # Orquestación de infraestructura
## ⚙️ Requisitos Previos
Para ejecutar este proyecto en tu entorno local, necesitarás tener instalado:
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (o Docker Engine + Docker Compose)
- (Opcional si se desarrolla localmente sin Docker) [Node.js 22+](https://nodejs.org/) y [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
## 🏃‍♂️ Cómo Ejecutar el Proyecto
La forma más rápida y sencilla de levantar toda la aplicación es utilizando Docker Compose. Esto levantará la base de datos, el backend y el frontend de forma automatizada e interconectada.
1. Abre una terminal y navega hasta la raíz del proyecto.
2. Ejecuta el siguiente comando para construir y levantar los contenedores en segundo plano:
   ```bash
   docker-compose up -d --build
   ```
3. Una vez que los contenedores estén corriendo, podrás acceder a los siguientes servicios:
   - **Frontend (Aplicación Web):** [http://localhost:5173](http://localhost:5173)
   - **Backend (API):** [http://localhost:6873](http://localhost:6873)
   - **Swagger (Documentación API):** [http://localhost:6873/swagger](http://localhost:6873/swagger) *(si está habilitado en entorno de desarrollo)*
   - **pgAdmin (Gestor de BD visual):** [http://localhost:5050](http://localhost:5050)
     - *Login:* `admin@local.dev`
     - *Password:* `password`
## 🔑 Credenciales por Defecto
**Base de datos (PostgreSQL):**
- **Usuario:** `postgres`
- **Contraseña:** `password`
- **Base de datos:** `thetraininghub`
## 👨‍💻 Autor
- **Alberto Rodríguez Penalva** (TFG)
