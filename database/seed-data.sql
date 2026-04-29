-- Script de datos de prueba para la tabla productos
-- Base de datos: catalogoproductos

INSERT INTO productos (nombre, descripcion, precio, cantidad_stock, categoria, url_imagen, codigo_sku, esta_activo, fecha_creacion)
VALUES
('Laptop HP Pavilion 15', 'Laptop profesional con procesador Intel Core i7, 16GB RAM, SSD 512GB', 2500000.00, 15, 'Tecnología', 'https://example.com/laptop-hp.jpg', 'LAP-HP-001', true, NOW()),
('Mouse Logitech MX Master 3', 'Mouse inalámbrico ergonómico de alta precisión', 180000.00, 50, 'Periféricos', 'https://example.com/mouse-logitech.jpg', 'MOU-LOG-001', true, NOW()),
('Teclado Mecánico Keychron K2', 'Teclado mecánico inalámbrico con switches Gateron Brown', 350000.00, 30, 'Periféricos', 'https://example.com/teclado-keychron.jpg', 'TEC-KEY-001', true, NOW()),
('Monitor LG UltraWide 34"', 'Monitor panorámico 34 pulgadas resolución 2560x1080', 1200000.00, 12, 'Monitores', 'https://example.com/monitor-lg.jpg', 'MON-LG-001', true, NOW()),
('Audífonos Sony WH-1000XM5', 'Audífonos inalámbricos con cancelación de ruido activa', 980000.00, 25, 'Audio', 'https://example.com/audifonos-sony.jpg', 'AUD-SON-001', true, NOW()),
('Webcam Logitech C920', 'Cámara web Full HD 1080p con micrófono estéreo', 250000.00, 40, 'Periféricos', 'https://example.com/webcam-logitech.jpg', 'WEB-LOG-001', true, NOW()),
('SSD Samsung 970 EVO 1TB', 'Disco de estado sólido NVMe M.2 con velocidad de lectura 3500MB/s', 420000.00, 60, 'Almacenamiento', 'https://example.com/ssd-samsung.jpg', 'SSD-SAM-001', true, NOW()),
('Router TP-Link Archer AX6000', 'Router WiFi 6 de alto rendimiento para gaming y streaming', 680000.00, 18, 'Redes', 'https://example.com/router-tplink.jpg', 'ROU-TPL-001', true, NOW()),
('Impresora HP LaserJet Pro', 'Impresora láser monocromática con WiFi y dúplex automático', 890000.00, 10, 'Impresoras', 'https://example.com/impresora-hp.jpg', 'IMP-HP-001', true, NOW()),
('Tablet Samsung Galaxy Tab S8', 'Tablet Android 11 pulgadas con S Pen incluido', 1850000.00, 22, 'Tabletas', 'https://example.com/tablet-samsung.jpg', 'TAB-SAM-001', true, NOW()),
('Hub USB-C Anker 7 en 1', 'Hub multipuerto con HDMI, USB 3.0, lector SD y carga PD', 120000.00, 75, 'Accesorios', 'https://example.com/hub-anker.jpg', 'HUB-ANK-001', true, NOW()),
('Micrófono Blue Yeti', 'Micrófono USB condensador profesional para streaming', 450000.00, 28, 'Audio', 'https://example.com/microfono-blue.jpg', 'MIC-BLU-001', true, NOW()),
('Silla Gamer DXRacer Formula', 'Silla ergonómica para gaming con soporte lumbar ajustable', 1100000.00, 8, 'Muebles', 'https://example.com/silla-dxracer.jpg', 'SIL-DXR-001', true, NOW()),
('Cámara Web 4K Razer Kiyo Pro', 'Cámara 4K con sensor CMOS para streaming profesional', 720000.00, 15, 'Periféricos', 'https://example.com/camara-razer.jpg', 'CAM-RAZ-001', true, NOW()),
('Adaptador WiFi USB TP-Link', 'Adaptador inalámbrico dual band AC1300 con antena de alto rendimiento', 95000.00, 100, 'Redes', 'https://example.com/adaptador-tplink.jpg', 'ADP-TPL-001', true, NOW());
