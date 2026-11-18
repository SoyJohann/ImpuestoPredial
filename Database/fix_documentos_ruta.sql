-- Limpia Documentos.Ruta: deja solo el nombre del archivo, quitando cualquier prefijo de ruta absoluta.
-- Ejemplo: ':/root/D/prueba/Predial_Cimitarra/000100140211000_mdto.pdf' -> '000100140211000_mdto.pdf'

-- 1) Ver registros que se van a afectar (ejecutar primero para revisar):
SELECT Id, "Ruta" AS ruta_actual, regexp_replace("Ruta", '^.*[/:\\\\]', '', 'g') AS ruta_limpia
FROM "Documentos"
WHERE "Ruta" IS NOT NULL
  AND "Ruta" <> ''
  AND "Ruta" ~ '[/:\\\\]';

-- 2) Ejecutar la limpieza (descomentar tras revisar el paso 1):
-- UPDATE "Documentos"
-- SET "Ruta" = regexp_replace("Ruta", '^.*[/:\\\\]', '', 'g')
-- WHERE "Ruta" IS NOT NULL
--   AND "Ruta" <> ''
--   AND "Ruta" ~ '[/:\\\\]';

-- 3) Verificar que no queden rutas con prefijos de carpeta:
-- SELECT Id, "Ruta" FROM "Documentos" WHERE "Ruta" ~ '[/:\\\\]' LIMIT 20;
-- (debería devolver 0 filas)
