-- Script para arreglar auto-increment en tablas tasks y purchases
-- Ejecutar este script en tu cliente PostgreSQL (pgAdmin, DBeaver, etc.)

-- 1. Verificar el valor máximo actual de id en tasks
SELECT MAX(id) FROM tasks;

-- 2. Crear secuencia para tasks si no existe
CREATE SEQUENCE IF NOT EXISTS tasks_id_seq;

-- 3. Configurar la columna id de tasks para usar la secuencia
ALTER TABLE tasks ALTER COLUMN id SET DEFAULT nextval('tasks_id_seq');

-- 4. Hacer que la secuencia sea propiedad de la columna
ALTER SEQUENCE tasks_id_seq OWNED BY tasks.id;

-- 5. Sincronizar la secuencia con el valor máximo actual
SELECT setval('tasks_id_seq', COALESCE((SELECT MAX(id) FROM tasks), 0) + 1, false);

-- 6. Repetir para purchases
SELECT MAX(id) FROM purchases;

CREATE SEQUENCE IF NOT EXISTS purchases_id_seq;

ALTER TABLE purchases ALTER COLUMN id SET DEFAULT nextval('purchases_id_seq');

ALTER SEQUENCE purchases_id_seq OWNED BY purchases.id;

SELECT setval('purchases_id_seq', COALESCE((SELECT MAX(id) FROM purchases), 0) + 1, false);

-- 7. Verificar que funcionó
\d tasks
\d purchases
