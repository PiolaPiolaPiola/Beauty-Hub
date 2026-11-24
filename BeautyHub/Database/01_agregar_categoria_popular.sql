-- =====================================================================
-- Beauty Hub - Cambio 01
-- Agrega a `servicios` las columnas que usa la página de servicios:
--   categoria: para agrupar en Barbería / Peluquería / Combos
--   popular:   para mostrar la etiqueta "Más pedido"
-- Córrelo UNA sola vez sobre la BD que ya creaste.
-- =====================================================================

USE beauty_hub;

ALTER TABLE servicios
    ADD COLUMN categoria ENUM('Barberia', 'Peluqueria', 'Combo') NOT NULL AFTER nombre_servicio,
    ADD COLUMN popular BOOLEAN NOT NULL DEFAULT FALSE;

DESCRIBE servicios;
