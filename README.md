# API REST - Catálogo de Productos

## Descripción del Proyecto

API REST desarrollada en .NET 10 para la gestión de un catálogo de productos con control de inventario. Implementa Clean Architecture y el patrón Repository, proporcionando endpoints RESTful para operaciones CRUD completas y ajuste de stock.

### Problema Resuelto

El sistema resuelve la necesidad de gestionar un catálogo de productos con las siguientes capacidades:

- **Gestión de Productos**: Crear, consultar, actualizar y eliminar productos del catálogo
- **Control de Inventario**: Ajuste de stock con trazabilidad de movimientos
- **Consultas Paginadas**: Listado eficiente de productos con paginación
- **Validaciones Robustas**: Protección contra inputs maliciosos y validación de reglas de negocio
- **Manejo de Errores**: Respuestas HTTP estandarizadas con códigos específicos y mensajes descriptivos

---

## Arquitectura

### Clean Architecture

El proyecto implementa Clean Architecture con separación de responsabilidades en 4 capas:

```
src/
├── CatalogoProductos.Domain/          # Capa de Dominio
│   └── Entidades, Interfaces, Excepciones de negocio
├── CatalogoProductos.Application/     # Capa de Aplicación
│   └── DTOs, Comandos CQRS, Validadores, Servicios
├── CatalogoProductos.Infrastructure/  # Capa de Infraestructura
│   └── EF Core, Repositorios, Configuración de BD
└── CatalogoProductos.API/             # Capa de Presentación
    └── Controllers, Middleware, Configuración
```

### Tecnologías Utilizadas

- **.NET 10**: Framework principal
- **ASP.NET Core Web API**: Para construcción de la API REST
- **Entity Framework Core 10**: ORM para acceso a datos
- **PostgreSQL**: Base de datos relacional (Supabase)
- **FluentValidation**: Validación de DTOs
- **AutoMapper**: Mapeo entre entidades y DTOs
- **Swagger/OpenAPI**: Documentación interactiva de la API
- **xUnit**: Framework de testing
- **Docker**: Containerización para despliegue
- **Railway**: Plataforma de hosting cloud

---

## Requisitos Previos

Para ejecutar este proyecto localmente necesitas:

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) o superior
- [PostgreSQL 14+](https://www.postgresql.org/download/) o acceso a Supabase
- [Git](https://git-scm.com/downloads)
- Un IDE compatible (.NET SDK, Visual Studio, VS Code, Rider)

---

## Instalación y Ejecución

### 1. Clonar el Repositorio

```bash
git clone https://github.com/SoyKranek/PTecnica_FDMApiRest.git
cd PTecnica_FDMApiRest
```

### 2. Configurar la Base de Datos

#### Opción A: Usar Supabase (Recomendado)

1. Crear un proyecto en [Supabase](https://supabase.com)
2. En el SQL Editor, ejecutar:

```sql
-- Crear tabla
CREATE TABLE productos (
    id_producto UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    precio DECIMAL(18,2) NOT NULL CHECK (precio >= 0),
    cantidad_stock INTEGER NOT NULL CHECK (cantidad_stock >= 0),
    categoria VARCHAR(100) NOT NULL,
    url_imagen VARCHAR(500),
    codigo_sku VARCHAR(50) NOT NULL UNIQUE,
    esta_activo BOOLEAN NOT NULL DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP WITH TIME ZONE
);

CREATE INDEX idx_productos_categoria ON productos(categoria);
CREATE INDEX idx_productos_esta_activo ON productos(esta_activo);
```

3. Obtener la cadena de conexión (Connection Pooling):
   - Settings → Database → Connection string → Transaction Mode

#### Opción B: PostgreSQL Local

```bash
# Crear base de datos
createdb catalogoproductos

# Ejecutar scripts
psql -d catalogoproductos -f database/schema.sql
psql -d catalogoproductos -f database/seed-data.sql
```

### 3. Configurar Variables de Entorno

Editar `src/CatalogoProductos.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Tu-cadena-de-conexión-aquí"
  }
}
```

O usar variable de entorno:

```bash
# Linux/Mac
export DATABASE_URL="postgresql://user:password@host:port/database"

# Windows PowerShell
$env:DATABASE_URL="postgresql://user:password@host:port/database"
```

### 4. Restaurar Dependencias y Compilar

```bash
dotnet restore
dotnet build
```

### 5. Ejecutar la API

```bash
cd src/CatalogoProductos.API
dotnet run
```

La API estará disponible en:
- **HTTP**: `http://localhost:5000`
- **Swagger**: `http://localhost:5000/swagger`

---

## Endpoints Disponibles

### Productos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/v1/productos` | Listar productos paginados |
| GET | `/api/v1/productos/{id}` | Obtener producto por ID |
| POST | `/api/v1/productos` | Crear nuevo producto |
| PUT | `/api/v1/productos/{id}` | Actualizar producto |
| DELETE | `/api/v1/productos/{id}` | Eliminar producto (soft delete) |

### Stock

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/v1/productos/{id}/stock/ajustar` | Ajustar stock del producto |

### Health Check

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/health` | Verificar estado de la API |

---

## Ejemplos de Uso

### Listar Productos (Paginado)

```bash
curl -X GET "http://localhost:5000/api/v1/productos?pagina=1&tamano=10"
```

**Respuesta:**
```json
{
  "elementos": [...],
  "numeroPagina": 1,
  "tamanoPagina": 10,
  "totalRegistros": 15
}
```

### Crear Producto

```bash
curl -X POST "http://localhost:5000/api/v1/productos" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Laptop HP Pavilion",
    "descripcion": "Laptop profesional 16GB RAM",
    "precio": 2500000,
    "cantidadStock": 10,
    "categoria": "Tecnología",
    "urlImagen": "https://example.com/laptop.jpg",
    "codigoSKU": "LAP-HP-001"
  }'
```

### Ajustar Stock

```bash
curl -X POST "http://localhost:5000/api/v1/productos/{id}/stock/ajustar" \
  -H "Content-Type: application/json" \
  -d '{
    "cantidadAjuste": -5,
    "motivo": "Venta realizada"
  }'
```

---

## Códigos de Respuesta HTTP

| Código | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Operación exitosa |
| 201 | Created | Recurso creado |
| 204 | No Content | Eliminación exitosa |
| 400 | Bad Request | Datos inválidos |
| 404 | Not Found | Recurso no encontrado |
| 409 | Conflict | SKU duplicado |
| 422 | Unprocessable Entity | Regla de negocio violada |
| 500 | Internal Server Error | Error del servidor |

---

## Testing

El proyecto incluye una suite completa de tests de seguridad y validación.

### Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test tests/CatalogoProductos.Application.Tests/

# Con detalles
dotnet test --logger "console;verbosity=detailed"
```

### Cobertura de Tests

- **57 tests automatizados**
- Validación contra inyección SQL, XSS, buffer overflow
- Tests de reglas de negocio del dominio
- Tests de valores extremos y casos límite

Ver documentación completa en: `TESTS_SEGURIDAD.md`

---

## Despliegue con Docker

### Construir Imagen

```bash
docker build -t catalogo-productos-api .
```

### Ejecutar Contenedor

```bash
docker run -d \
  -p 8080:8080 \
  -e DATABASE_URL="postgresql://..." \
  --name api-productos \
  catalogo-productos-api
```

---

## Despliegue en Railway

1. Conectar repositorio de GitHub con Railway
2. Configurar variable de entorno: `DATABASE_URL`
3. Railway detectará automáticamente el Dockerfile
4. El deploy se realizará automáticamente

**URL Pública**: https://ptecnicafdmapirest-production.up.railway.app

---

## Estructura del Proyecto

```
.
├── src/
│   ├── CatalogoProductos.API/
│   │   ├── Controllers/           # Endpoints REST
│   │   ├── Middleware/           # Manejo de excepciones
│   │   ├── Models/              # Modelos de respuesta
│   │   └── Program.cs           # Configuración de la aplicación
│   ├── CatalogoProductos.Application/
│   │   ├── Comandos/           # CQRS Commands
│   │   ├── Consultas/          # CQRS Queries
│   │   ├── Contratos/          # DTOs
│   │   ├── Servicios/          # Lógica de aplicación
│   │   └── Validadores/        # FluentValidation
│   ├── CatalogoProductos.Domain/
│   │   ├── Entidades/          # Entidades de dominio
│   │   ├── Excepciones/        # Excepciones de negocio
│   │   └── Interfaces/         # Contratos de repositorios
│   └── CatalogoProductos.Infrastructure/
│       ├── Datos/              # DbContext, Configuraciones EF
│       └── Repositorios/       # Implementaciones de repositorios
├── tests/
│   └── CatalogoProductos.Application.Tests/
│       ├── TestsValidacionMaliciosa.cs
│       └── TestsDominioProducto.cs
├── database/
│   ├── schema.sql             # Estructura de BD
│   └── seed-data.sql          # Datos de prueba
├── Dockerfile                 # Configuración Docker
└── README.md                 # Este archivo
```

---

## Convenciones del Código

### Nomenclatura

- **Variables y métodos**: Español descriptivo (`ObtenerProductoPorId`)
- **Contratos JSON**: camelCase (`idProducto`, `nombreProducto`)
- **Clases**: PascalCase (`ProductoResponse`, `ServicioGestionProductos`)
- **Interfaces**: Prefijo `I` (`IRepositorioProducto`)

### Validaciones

- Nombres: 1-200 caracteres
- Descripción: 1-2000 caracteres
- Precio: 0.01 - 9,999,999.99
- Stock: 0 - 999,999
- SKU: Alfanumérico con guiones, 3-50 caracteres
- URL Imágenes: Solo http:// o https://

---

## Patrones y Principios Aplicados

- **Clean Architecture**: Separación de responsabilidades por capas
- **Repository Pattern**: Abstracción del acceso a datos
- **CQRS**: Separación de comandos y consultas
- **Dependency Injection**: Inversión de dependencias
- **SOLID Principles**: Código mantenible y escalable
- **Fail-Fast Validation**: Validación temprana de inputs

---

## Seguridad

La API implementa múltiples capas de seguridad:

✅ **Validación de entrada**: FluentValidation con reglas estrictas  
✅ **Protección contra SQL Injection**: Entity Framework con queries parametrizadas  
✅ **Validación de caracteres**: Bloqueo de caracteres peligrosos (<, >, ;, --, etc.)  
✅ **Límites de tamaño**: Prevención de buffer overflow  
✅ **Validación de URLs**: Solo http/https permitidos  
✅ **Manejo de errores**: Sin exposición de stack traces en producción  

---

## Troubleshooting

### Error: "Connection refused"

Verificar que PostgreSQL esté corriendo y la cadena de conexión sea correcta.

### Error: "Assembly not found"

Ejecutar `dotnet restore` y `dotnet build`.

### Error: "Port already in use"

Cambiar el puerto en `launchSettings.json` o usar variable de entorno `ASPNETCORE_URLS`.

### Tests fallan

Verificar que las dependencias de test estén instaladas: `dotnet restore tests/`

---

## Licencia

Este proyecto fue desarrollado como prueba técnica para Fundación de la Mujer.

---

## Contacto

Para preguntas o soporte técnico sobre este proyecto, contactar al equipo de desarrollo.
