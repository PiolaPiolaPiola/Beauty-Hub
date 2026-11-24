-- =====================================================================
-- Beauty Hub - Datos iniciales de servicios
-- Son los mismos servicios que hoy están escritos en el JavaScript del Index.
-- Córrelo UNA sola vez (si lo repites, se duplican los servicios).
-- =====================================================================

USE beauty_hub;

INSERT INTO servicios (nombre_servicio, categoria, descripcion, precio, duracion_minutos, popular) VALUES
-- Barbería
('Corte clásico',          'Barberia',   'Tijera y máquina, con acabado a navaja en el contorno.',        25000,  40, FALSE),
('Degradado (fade)',       'Barberia',   'Bajo, medio o alto, con diseño de líneas si lo deseas.',        30000,  50, TRUE),
('Arreglo de barba',       'Barberia',   'Perfilado, recorte y toalla caliente.',                          20000,  30, FALSE),
('Afeitado tradicional',   'Barberia',   'Navaja, espuma caliente y bálsamo después del afeitado.',       25000,  35, FALSE),
('Corte infantil',         'Barberia',   'Para menores de 12 años.',                                       20000,  30, FALSE),
('Cejas y contorno',       'Barberia',   'Limpieza con navaja o hilo.',                                     8000,  15, FALSE),
-- Peluquería
('Corte y peinado',        'Peluqueria', 'Lavado, corte según tu tipo de cabello y secado.',              45000,  60, FALSE),
('Color completo',         'Peluqueria', 'Aplicación en todo el cabello con productos sin amoníaco.',    120000, 120, FALSE),
('Mechas y balayage',      'Peluqueria', 'Aclarado a mano alzada, con matizado incluido.',               220000, 180, TRUE),
('Tratamiento de keratina','Peluqueria', 'Reduce el frizz y deja el cabello suave por semanas.',         180000, 150, FALSE),
('Hidratación profunda',   'Peluqueria', 'Mascarilla nutritiva con masaje de cuero cabelludo.',           55000,  45, FALSE),
('Peinado para evento',    'Peluqueria', 'Recogidos, ondas o trenzas. Pide tu cita con anticipación.',   70000,  60, FALSE),
-- Combos
('Corte + barba',          'Combo',      'Degradado o corte clásico con arreglo de barba.',               42000,  75, FALSE),
('Corte + color',          'Combo',      'Corte y peinado con color completo.',                          150000, 170, FALSE),
('Día de novia',           'Combo',      'Peinado, hidratación previa y retoque de color.',              240000, 210, FALSE);

SELECT categoria, COUNT(*) AS total FROM servicios GROUP BY categoria;
