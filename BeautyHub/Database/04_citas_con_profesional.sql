USE beauty_hub;

ALTER TABLE citas
    ADD COLUMN id_profesional INT NULL AFTER id_servicio,
    ADD CONSTRAINT fk_citas_profesional
        FOREIGN KEY (id_profesional) REFERENCES profesionales(id_profesional);

CREATE INDEX ix_citas_profesional_fecha ON citas (id_profesional, fecha_cita, hora_cita);
CREATE INDEX ix_citas_cliente ON citas (id_cliente);

DESCRIBE citas;
