-- Script de creación de esquema de base de datos
-- Base de datos: catalogoproductos

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
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP
);

CREATE INDEX idx_productos_categoria ON productos(categoria);
CREATE INDEX idx_productos_esta_activo ON productos(esta_activo);
CREATE UNIQUE INDEX idx_productos_codigo_sku ON productos(codigo_sku);
