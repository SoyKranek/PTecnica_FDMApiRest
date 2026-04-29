# API REST Catálogo de Productos

API REST construida con .NET 10 para la gestión de productos y control de inventario, implementando Clean Architecture y patrón Repository.

## Arquitectura

Este proyecto implementa Clean Architecture con cuatro capas claramente separadas:

```
├── CatalogoProductos.Domain/          # Entidades de dominio, interfaces, excepciones
├── CatalogoProductos.Application/      # Casos de uso, DTOs, comandos, queries, validadores
├── CatalogoProductos.Infrastructure/   # Implementación EF Core, repositorios, configuración BD
└── CatalogoProductos.API/              # Controllers, middleware, configuración API
```

### Flujo de Dependencias

```
API → Application → Domain ← Infrastructure
```

El dominio no depende de ninguna otra capa, manteniendo la lógica de negocio aislada.

## Stack Tecnológico

- **.NET 10** - Runtime y SDK
- **ASP.NET Core** - Framework Web API
- **Entity Framework Core 10** - ORM para acceso a datos
- **PostgreSQL** - Base de datos relacional (Supabase)
- **FluentValidation** - Validación de entrada
- **AutoMapper** - Mapeo entre entidades y DTOs
- **Swashbuckle** - Documentación OpenAPI/Swagger

## Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (local) o cuenta en [Supabase](https://supabase.com/)
- [Docker](https://www.docker.com/) (opcional, para contenedorización)

## Instalación Local

### 1. Clonar el Repositorio

```bash
git clone https://github.com/SoyKranek/PTecnica_FDMApiRest.git
cd PTecnica_FDMApiRest
```

### 2. Configurar Cadena de Conexión

Editar `src/CatalogoProductos.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=catalogoproductos;Username=postgres;Password=tu_password"
  }
}
```

### 3. Crear Base de Datos

Ejecutar el script SQL incluido o crear la base de datos manualmente:

```sql
CREATE DATABASE catalogoproductos;
```

Aplicar migraciones:

```bash
dotnet ef database update --project src/CatalogoProductos.Infrastructure --startup-project src/CatalogoProductos.API
```

### 4. Compilar y Ejecutar

```bash
dotnet build
dotnet run --project src/CatalogoProductos.API
```

La API estará disponible en `https://localhost:7013` y `http://localhost:5002`

### 5. Acceder a Swagger

Abrir en el navegador: `https://localhost:7013/swagger`

## Endpoints Disponibles

### Productos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/v1/productos` | Obtener lista paginada de productos |
| GET | `/api/v1/productos/{id}` | Obtener producto por ID |
| POST | `/api/v1/productos` | Crear nuevo producto |
| PUT | `/api/v1/productos/{id}` | Actualizar producto existente |
| DELETE | `/api/v1/productos/{id}` | Eliminar producto (soft delete) |

### Stock

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/v1/productos/{id}/stock/ajustar` | Ajustar inventario de producto |

### Health Check

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/health` | Estado de salud de la API |

## Ejemplos de Uso

### Crear Producto

```bash
curl -X POST https://localhost:7013/api/v1/productos \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Laptop HP Pavilion",
    "descripcion": "Laptop profesional con procesador Intel i7",
    "precio": 2500000.00,
    "cantidadStock": 15,
    "categoria": "Tecnología",
    "urlImagen": "https://example.com/laptop.jpg",
    "codigoSKU": "LAP-HP-001"
  }'
```

### Obtener Productos Paginados

```bash
curl -X GET "https://localhost:7013/api/v1/productos?pagina=1&tamano=10"
```

### Ajustar Stock

```bash
curl -X POST https://localhost:7013/api/v1/productos/{id}/stock/ajustar \
  -H "Content-Type: application/json" \
  -d '{
    "cantidadAjuste": -5
  }'
```

## Códigos de Respuesta HTTP

| Código | Significado |
|--------|-------------|
| 200 OK | Consulta o actualización exitosa |
| 201 CREATED | Recurso creado exitosamente |
| 204 NO CONTENT | Eliminación exitosa |
| 400 BAD REQUEST | Error de validación o regla de negocio |
| 404 NOT FOUND | Recurso no encontrado |
| 500 INTERNAL SERVER ERROR | Error interno del servidor |

## Despliegue con Docker

### Construir Imagen

```bash
docker build -t catalogo-productos-api .
```

### Ejecutar Contenedor

```bash
docker run -d -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=tu-host;Database=catalogoproductos;Username=user;Password=pass" \
  catalogo-productos-api
```

## Despliegue en Railway

### 1. Conectar Repositorio

Crear un nuevo proyecto en [Railway](https://railway.app/) y conectar el repositorio de GitHub.

### 2. Configurar Variables de Entorno

En el dashboard de Railway, agregar:

```
ConnectionStrings__DefaultConnection=Host=tu-supabase-host;Database=postgres;Username=postgres;Password=tu-password
ASPNETCORE_ENVIRONMENT=Production
```

### 3. Configurar Health Check

Endpoint: `/health`

### 4. Desplegar

Railway detectará automáticamente el Dockerfile y desplegará la aplicación.

## Variables de Entorno

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a PostgreSQL | `Host=localhost;Database=catalogoproductos;...` |
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | `Development`, `Production` |

## Convenciones de Código

- **Variables/Propiedades**: Español, descriptivas (`cantidadStock`, `nombreProducto`)
- **Contratos API**: camelCase en JSON
- **Clases/Métodos**: PascalCase
- **Nombres de métodos**: Verbos descriptivos (`ObtenerProductoPorId`, `AjustarStock`)

## Estructura de Proyecto

```
.
├── src/
│   ├── CatalogoProductos.API/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── CatalogoProductos.Application/
│   │   ├── Comandos/
│   │   ├── Consultas/
│   │   ├── Contratos/
│   │   ├── Servicios/
│   │   └── Validadores/
│   ├── CatalogoProductos.Domain/
│   │   ├── Entidades/
│   │   ├── Excepciones/
│   │   └── Interfaces/
│   └── CatalogoProductos.Infrastructure/
│       ├── Datos/
│       └── Repositorios/
├── tests/
│   ├── CatalogoProductos.Domain.Tests/
│   ├── CatalogoProductos.Application.Tests/
│   └── CatalogoProductos.Infrastructure.Tests/
├── Dockerfile
└── README.md
```

## Licencia

Este proyecto fue desarrollado como parte de una prueba técnica para Fundación de la Mujer.

## Contacto

Para consultas sobre este proyecto, contactar al desarrollador asignado.
