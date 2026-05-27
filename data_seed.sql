-- =============================================================================
-- SEED DATA - MyTrainingWeb
-- Generado para PostgreSQL
-- =============================================================================

-- IMPORTANTE: Ejecutar los inserts en el siguiente orden para respetar las
-- claves foráneas (Foreign Keys).

-- ------------------------------------------------------------------------------
-- 1. USUARIOS (Tabla: users)
-- ------------------------------------------------------------------------------
-- Usuario principal: Alberto Rodriguez Penalva
-- Password hasheado con BCrypt para: Ab11072004.
INSERT INTO users (id, name, email, passwordhash, level, strength, endurance, consistencystreak, gold, role, experience, "equippedStrengthId", "equippedEnduranceId", "avatarUrl", "isEmailVerified")
VALUES 
(1, 'Alberto Rodriguez Penalva', 'albertorope04@gmail.com', '$2b$12$XqWp/EJBglLz.0S7wxqeeO2hRxg3SsCyX5dRcYK7hDFX8nU5auAOC', 5, 10, 8, 3, 500, 'user', 450, NULL, NULL, NULL, TRUE),
(2, 'Maria Garcia Lopez', 'maria.garcia@example.com', '$2b$12$XqWp/EJBglLz.0S7wxqeeO2hRxg3SsCyX5dRcYK7hDFX8nU5auAOC', 3, 5, 6, 1, 200, 'user', 150, NULL, NULL, NULL, TRUE);

-- ------------------------------------------------------------------------------
-- 2. ITEMS (Tabla: items)
-- ------------------------------------------------------------------------------
INSERT INTO items (id, name, type, bonus, price, "imageUrl")
VALUES 
(1, 'Guantes de Cuero', 'Strength', 5, 100, 'https://example.com/gloves.png'),
(2, 'Pesas de Tobillo', 'Endurance', 3, 75, 'https://example.com/ankle_weights.png'),
(3, 'Cinturon de Powerlifting', 'Strength', 8, 250, 'https://example.com/belt.png'),
(4, 'Botella de Agua Pro', 'Endurance', 2, 50, 'https://example.com/bottle.png'),
(5, 'Proteina Whey', 'Strength', 4, 150, 'https://example.com/protein.png');

-- ------------------------------------------------------------------------------
-- 3. LOGROS (Tabla: achievements)
-- ------------------------------------------------------------------------------
INSERT INTO achievements ("Id", "Name", "Description", "IconUrl", "Category", "RequirementType", "RequirementValue", "RewardGold", "RewardXP", "RewardItemId", "IsSecret")
VALUES 
(1, 'Primeros Pasos', 'Completa tu primera tarea de entrenamiento.', 'https://example.com/first_steps.png', 'general', 'tasks_completed', 1, 50, 100, NULL, FALSE),
(2, 'Maestro de la Consistencia', 'Mantén una racha de 7 dias consecutivos.', 'https://example.com/streak.png', 'consistency', 'streak_days', 7, 100, 250, NULL, FALSE),
(3, 'Cazador de Gimnasios', 'Busca 5 gimnasios diferentes en la app.', 'https://example.com/gym_hunter.png', 'exploration', 'gyms_searched', 5, 75, 150, NULL, FALSE),
(4, 'Fortaleza de Hierro', 'Alcanza 20 puntos de fuerza.', 'https://example.com/iron_strength.png', 'strength', 'strength_reached', 20, 200, 500, NULL, FALSE),
(5, 'Corredor de Fondo', 'Alcanza 20 puntos de resistencia.', 'https://example.com/endurance_runner.png', 'endurance', 'endurance_reached', 20, 200, 500, NULL, FALSE);

-- ------------------------------------------------------------------------------
-- 4. SALAS (Tabla: rooms)
-- ------------------------------------------------------------------------------
INSERT INTO rooms (id, name, minlevel, minstats, minconsistency, description, date, localization, "creatorRole")
VALUES 
(1, 'Sala de Principiantes', 1, 0, 0, 'Un lugar perfecto para empezar tu aventura fitness.', '2026-05-26', 'Zaragoza, Espana', 'user'),
(2, 'Guarida de los Fuertes', 5, 15, 2, 'Solo para aquellos que han demostrado su fuerza.', '2026-05-26', 'Madrid, Espana', 'user'),
(3, 'Camara de la Resistencia', 3, 5, 5, 'Para los guerreros que nunca se rinden.', '2026-05-26', 'Barcelona, Espana', 'user');

-- ------------------------------------------------------------------------------
-- 5. EJERCICIOS (Tabla: exercises)
-- ------------------------------------------------------------------------------
INSERT INTO exercises ("Id", "Name", "Description", "MuscleGroup", "Difficulty", "ImageUrl", "VideoUrl", "Equipment", "Instructions", "CommonMistakes", "BodyPart", "Target", "Category", "SecondaryMuscles", "InstructionSteps")
VALUES 
(1, 'Flexiones', 'Ejercicio clasico para pecho y triceps.', 'pecho', 'beginner', 'https://example.com/pushup.jpg', 'https://example.com/pushup.mp4', 'peso corporal', 'Mantén el cuerpo recto y baja controladamente.', 'No dejar caer la cadera.', 'upper body', 'pectoralis major', 'strength', ARRAY['triceps', 'hombros']::text[], ARRAY['Colocate en posicion de plancha.', 'Baja el cuerpo flexionando los codos.', 'Sube empujando con las palmas.']::text[]),
(2, 'Sentadilla', 'Rey de los ejercicios de piernas.', 'piernas', 'beginner', 'https://example.com/squat.jpg', 'https://example.com/squat.mp4', 'peso corporal', 'Baja las caderas como si te fueras a sentar.', 'No dejar que las rodillas se desplomen hacia dentro.', 'lower body', 'quadriceps', 'strength', ARRAY['gluteos', 'isquiotibiales']::text[], ARRAY['Ponte de pie con los pies a la altura de los hombros.', 'Baja las caderas hacia atras y abajo.', 'Empuje a traves de los talones para subir.']::text[]),
(3, 'Plancha', 'Ejercicio isometrico para el core.', 'core', 'beginner', 'https://example.com/plank.jpg', 'https://example.com/plank.mp4', 'peso corporal', 'Mantén la posicion sin moverte.', 'Arquear la espalda.', 'core', 'abdominales', 'strength', ARRAY['hombros', 'espalda baja']::text[], ARRAY['Apoya los antebrazos en el suelo.', 'Mantén el cuerpo en linea recta.', 'Aguanta el tiempo indicado.']::text[]),
(4, 'Burpees', 'Ejercicio cardiovascular de cuerpo completo.', 'cardio', 'intermediate', 'https://example.com/burpee.jpg', 'https://example.com/burpee.mp4', 'peso corporal', 'Combinacion de sentadilla, flexion y salto.', 'Perder la forma al hacerlo rapido.', 'full body', 'cuerpo completo', 'cardio', ARRAY['pecho', 'piernas']::text[], ARRAY['Baja a posicion de sentadilla.', 'Lanza los pies atras en plancha.', 'Haz una flexion, vuelve a sentadilla y salta.']::text[]);

-- ------------------------------------------------------------------------------
-- 6. PLANES (Tabla: plans)
-- ------------------------------------------------------------------------------
INSERT INTO plans (id, userid, description)
VALUES 
(1, 1, 'Plan de fuerza para principiantes - 4 semanas'),
(2, 2, 'Plan de resistencia y cardio - 6 semanas');

-- ------------------------------------------------------------------------------
-- 7. COMPRAS (Tabla: purchases)
-- ------------------------------------------------------------------------------
INSERT INTO purchases (id, userid, itemid, purchasedate)
VALUES 
(1, 1, 1, NOW()),
(2, 1, 3, NOW()),
(3, 2, 2, NOW());

-- ------------------------------------------------------------------------------
-- 8. SUSCRIPCIONES (Tabla: subscriptions)
-- ------------------------------------------------------------------------------
INSERT INTO subscriptions (id, userid, startdate, enddate, isactive, "planType", "monthlyPrice")
VALUES 
(1, 1, NOW(), NOW() + INTERVAL '1 month', TRUE, 'Premium', 10.00),
(2, 2, NOW(), NOW() + INTERVAL '1 month', TRUE, 'Premium', 10.00);

-- ------------------------------------------------------------------------------
-- 9. RELACION USUARIO-SALA (Tabla: usersrooms)
-- ------------------------------------------------------------------------------
INSERT INTO usersrooms (userid, roomid)
VALUES 
(1, 1),
(1, 2),
(2, 1);

-- ------------------------------------------------------------------------------
-- 10. TAREAS (Tabla: tasks)
-- ------------------------------------------------------------------------------
INSERT INTO tasks (id, name, description, difficulty, reward, iscompleted, createdat, "userId", trainingfocus)
VALUES 
(1, 'Rutina Matutina', 'Completa 3 series de flexiones y sentadillas.', 2, 50, FALSE, NOW(), 1, 'fuerza'),
(2, 'Cardio Express', 'Realiza 15 minutos de burpees y saltos.', 3, 75, TRUE, NOW(), 1, 'resistencia'),
(3, 'Fortalece el Core', 'Mantén la plancha durante 3 minutos en total.', 2, 60, FALSE, NOW(), 2, 'core');

-- ------------------------------------------------------------------------------
-- 11. VERIFICACIONES DE EMAIL (Tabla: emailverifications)
-- ------------------------------------------------------------------------------
INSERT INTO emailverifications (id, userid, code, createdat, expiresat, isused)
VALUES 
(1, 1, '123456', NOW(), NOW() + INTERVAL '15 minutes', TRUE),
(2, 2, '654321', NOW(), NOW() + INTERVAL '15 minutes', TRUE);

-- ------------------------------------------------------------------------------
-- 12. PREFERENCIAS DE NOTIFICACION (Tabla: notificationpreferences)
-- ------------------------------------------------------------------------------
INSERT INTO notificationpreferences (id, userid, "inactivityEnabled", "inactivityDays", "roomsEnabled", "purchasesEnabled", "subscriptionExpiryEnabled", createdat, updatedat)
VALUES 
(1, 1, TRUE, 3, TRUE, TRUE, TRUE, NOW(), NOW()),
(2, 2, TRUE, 3, TRUE, TRUE, TRUE, NOW(), NOW());

-- ------------------------------------------------------------------------------
-- 13. LOGROS DE USUARIO (Tabla: userachievements)
-- ------------------------------------------------------------------------------
INSERT INTO userachievements ("Id", "UserId", "AchievementId", "UnlockedAt", "IsNew")
VALUES 
(1, 1, 1, NOW(), FALSE),
(2, 1, 2, NOW(), TRUE),
(3, 2, 1, NOW(), FALSE);

-- ------------------------------------------------------------------------------
-- 14. METRICAS CORPORALES (Tabla: bodymetrics)
-- ------------------------------------------------------------------------------
INSERT INTO bodymetrics ("Id", "UserId", "Date", "Weight", "Height", "BodyFat", "Chest", "Waist", "Arms", "Thighs", "ProgressPhotoUrl", "Notes")
VALUES 
(1, 1, NOW(), 75.5, 180.0, 15.0, 105.0, 85.0, 38.0, 58.0, NULL, 'Primera medicion inicial.'),
(2, 1, NOW() - INTERVAL '7 days', 76.0, 180.0, 15.5, 104.0, 86.0, 37.5, 57.5, NULL, 'Medicion de la semana pasada.'),
(3, 2, NOW(), 62.0, 165.0, 22.0, 90.0, 70.0, 30.0, 50.0, NULL, 'Medicion inicial de Maria.');

-- ------------------------------------------------------------------------------
-- 15. EJERCICIOS DE TAREA (Tabla: taskexercises)
-- ------------------------------------------------------------------------------
INSERT INTO taskexercises ("Id", "TaskId", "ExerciseId", "Sets", "Reps", "Weight", "RestSeconds", "IsCompleted", "OrderIndex")
VALUES 
(1, 1, 1, 3, 12, NULL, 60, FALSE, 0),
(2, 1, 2, 3, 15, NULL, 90, FALSE, 1),
(3, 2, 4, 4, 10, NULL, 45, TRUE, 0),
(4, 3, 3, 3, NULL, NULL, 30, FALSE, 0);

-- =============================================================================
-- FIN DEL SCRIPT
-- =============================================================================