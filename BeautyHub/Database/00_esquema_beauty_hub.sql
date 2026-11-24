-- =====================================================================
-- Beauty Hub - Esquema completo (para crear la BD desde cero)
-- Si ya tienes la BD creada con el script original, NO corras este:
-- corre solo 01_agregar_categoria_popular.sql
-- =====================================================================

CREATE DATABASE IF NOT EXISTS beauty_hub
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE beauty_hub;

CREATE TABLE IF NOT EXISTS clientes (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    correo VARCHAR(100),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS servicios (
    id_servicio INT AUTO_INCREMENT PRIMARY KEY,
    nombre_servicio VARCHAR(100) NOT NULL,
    categoria ENUM('Barberia', 'Peluqueria', 'Combo') NOT NULL,
    descripcion TEXT,
    precio DECIMAL(10,2) NOT NULL,
    duracion_minutos INT NOT NULL,
    estado BOOLEAN DEFAULT TRUE,
    popular BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS citas (
    id_cita INT AUTO_INCREMENT PRIMARY KEY,
    id_cliente INT NOT NULL,
    id_servicio INT NOT NULL,
    fecha_cita DATE NOT NULL,
    hora_cita TIME NOT NULL,
    observaciones TEXT,
    estado ENUM(
        'Solicitada',
        'Confirmada',
        'Cancelada',
        'Atendida'
    ) DEFAULT 'Solicitada',

    FOREIGN KEY (id_cliente)
        REFERENCES clientes(id_cliente),

    FOREIGN KEY (id_servicio)
        REFERENCES servicios(id_servicio)
);
