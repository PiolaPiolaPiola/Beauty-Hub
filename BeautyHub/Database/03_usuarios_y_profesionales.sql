USE beauty_hub;

CREATE TABLE IF NOT EXISTS profesionales (
    id_profesional INT AUTO_INCREMENT PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    correo VARCHAR(100),
    especialidad VARCHAR(100),
    estado BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL,
    contrasena_hash VARCHAR(255) NOT NULL,
    rol ENUM('Cliente', 'Profesional', 'Admin') NOT NULL,
    id_cliente INT NULL,
    id_profesional INT NULL,
    estado BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE KEY uq_usuarios_usuario (usuario),
    UNIQUE KEY uq_usuarios_cliente (id_cliente),
    UNIQUE KEY uq_usuarios_profesional (id_profesional),

    CONSTRAINT fk_usuarios_cliente
        FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),

    CONSTRAINT fk_usuarios_profesional
        FOREIGN KEY (id_profesional) REFERENCES profesionales(id_profesional)
);

SELECT 'profesionales' AS tabla, COUNT(*) AS total FROM profesionales
UNION ALL
SELECT 'usuarios', COUNT(*) FROM usuarios;
