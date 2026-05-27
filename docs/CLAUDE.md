# TFG - The Training Hub

## Estructura
- Backend (C#/.NET): /Backend/
- Frontend (Vue 3/Pinia): /Frontend/

## Red
- API: http://127.0.0.1:6873
- Front: http://localhost:5173

## Backend (C#)
- Arquitectura: Controller -> Service -> Repository -> EF DB
- Rutas: Modelos (Model/), Seguridad (JWT/)
- Regla: No modificar Migrations/ salvo extrema necesidad. Usar inyección de dependencias.

## Frontend (Vue 3)
- Rutas: Vistas (src/Views/), Componentes (src/Componentes/), Estado/API (src/Stores/), JWT (src/JWT/), i18n (src/i18n/), Estilos (src/Styles/)
- Reglas: Composition API, PascalCase en componentes.

## General
- Idioma: Español (código y comentarios).
- Ignorar: bin/, obj/, node_modules/, .git/, dist/.
    