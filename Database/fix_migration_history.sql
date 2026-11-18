-- La base Cimitarra ya tenía el esquema de ContribuyenteMinimal, pero la fila
-- no estaba registrada en __EFMigrationsHistory. Sin este registro,
-- `dotnet ef database update` intenta crear de nuevo la tabla Contribuyentes
-- y la columna ContribuyenteId, y falla.
--
-- Ejecutar una sola vez por base antes de aplicar la migración
-- AgregarTipoDocumento.
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261106221900_ContribuyenteMinimal', '10.0.0')
ON CONFLICT DO NOTHING;
