# TFG - The Training Hub

Aplicación web full-stack para la gestión de entrenamientos, salas colaborativas, seguimiento de progreso y asistencia deportiva con inteligencia artificial.

---

## Tecnologías

| Capa | Tecnología |
|------|------------|
| **Backend** | C# / .NET 8 |
| **Frontend** | Vue 3 + TypeScript |
| **Estado** | Pinia |
| **UI** | Vuetify 3 |
| **Base de datos** | PostgreSQL (EF Core) |
| **Autenticación** | JWT (Bearer) |
| **Build** | Vite |
| **Tests** | xUnit (Backend) |

---

## Estructura del proyecto

```
TFG-MyTraningWeb/
├── Backend/              # API REST (.NET 8)
│   ├── Controllers/      # Endpoints (Auth, Users, Rooms, Exercises, Plans, ...)
│   ├── Model/            # Entidades y DTOs
│   ├── Database/         # AppDbContext y Factory
│   ├── JWT/              # Configuración y utilidades JWT
│   ├── Migrations/       # Migraciones de EF Core
│   └── Tests/            # Tests unitarios (xUnit)
│
├── Frontend/             # Aplicación SPA (Vue 3)
│   ├── src/
│   │   ├── views/        # Vistas de páginas
│   │   ├── components/   # Componentes reutilizables
│   │   ├── stores/       # Stores de Pinia (estado + API)
│   │   ├── JWT/          # Gestión de tokens en cliente
│   │   ├── i18n/         # Internacionalización
│   │   └── styles/       # Estilos globales (SASS)
│   └── package.json
│
└── README.md
```

---

## Red y entornos

| Servicio | URL |
|----------|-----|
| API (Backend) | `http://127.0.0.1:6873` |
| Frontend (dev) | `http://localhost:5173` |

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (versión LTS recomendada)
- [PostgreSQL](https://www.postgresql.org/)

---

## Configuración y ejecución

### 1. Base de datos

Asegúrate de tener una instancia de PostgreSQL en ejecución y configura la cadena de conexión en el backend si es necesario.

### 2. Backend

```bash
cd Backend

# Restaurar dependencias
dotnet restore

# Aplicar migraciones
dotnet ef database update

# Ejecutar en modo desarrollo
dotnet run
```

La API estará disponible en: `http://127.0.0.1:6873`

### 3. Frontend

```bash
cd Frontend

# Instalar dependencias
npm install

# Ejecutar servidor de desarrollo
npm run dev
```

La aplicación estará disponible en: `http://localhost:5173`

---

## Scripts útiles

### Backend

| Comando | Descripción |
|---------|-------------|
| `dotnet run` | Ejecutar la API en modo desarrollo |
| `dotnet test` | Ejecutar tests unitarios |
| `dotnet ef migrations add <Nombre>` | Crear nueva migración |
| `dotnet ef database update` | Aplicar migraciones pendientes |

### Frontend

| Comando | Descripción |
|---------|-------------|
| `npm run dev` | Servidor de desarrollo (Vite) |
| `npm run build` | Compilación para producción |
| `npm run preview` | Previsualizar build de producción |
| `npm run type-check` | Verificación de tipos TypeScript |
| `npm run format` | Formatear código con Prettier |

---

## Funcionalidades principales

- **Autenticación y autorización**: registro, login, verificación de correo y roles (JWT).
- **Gestión de usuarios**: perfiles, estadísticas, preferencias de notificación.
- **Salas de entrenamiento**: creación, unión, gestión de miembros y chat en tiempo real.
- **Ejercicios y rutinas**: catálogo de ejercicios, planificación de tareas y calendario de entrenamiento.
- **Métricas corporales**: seguimiento de peso, IMC y progreso físico con historial.
- **Logros y recompensas**: sistema de gamificación basado en objetivos cumplidos.
- **Tienda y suscripciones**: artículos, compras y planes de suscripción.
- **Coach IA**: asistente virtual para dudas y recomendaciones deportivas.
- **Localización y geocodificación**: integración con mapas para establecimientos deportivos.
- **Internacionalización (i18n)**: soporte multiidioma en el cliente.

---

## Arquitectura

### Backend

Arquitectura por capas con inyección de dependencias:

```
Controller → Service → Repository → Entity Framework → PostgreSQL
```

- Los controladores exponen la API REST.
- Los servicios contienen la lógica de negocio.
- Los repositorios gestionan el acceso a datos.
- EF Core actúa como ORM sobre PostgreSQL.

> **Nota**: no modificar manualmente las migraciones salvo extrema necesidad.

### Frontend

Desarrollado con **Vue 3 Composition API** y componentes en **PascalCase**:

- **Vistas** (`src/views/`): páginas completas.
- **Componentes** (`src/components/`): piezas reutilizables.
- **Stores** (`src/stores/`): estado global y llamadas a la API.
- **JWT** (`src/JWT/`): manejo de tokens y autenticación.
- **i18n** (`src/i18n/`): traducciones y localización.
- **Estilos** (`src/styles/`): hojas de estilo globales con SASS.

---

## Tests

El backend incluye tests unitarios organizados en:

```
Backend/Tests/Backend.Tests/
```

Para ejecutarlos:

```bash
cd Backend
dotnet test
```

---

## Autor

**Alberto Rodríguez Penalva**

Proyecto de Trabajo de Fin de Grado (TFG).

---

## Licencia

Este proyecto es de carácter académico. Consulta con el autor antes de su reutilización.
