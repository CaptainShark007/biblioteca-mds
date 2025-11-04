-- Script SQL para actualizar la base de datos según el diagrama de clases
-- Ejecutar en SQL Server Management Studio

USE BibliotecaMDS;
GO

-- Agregar campo 'estado' a tbl_libro
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'tbl_libro') AND name = 'estado')
BEGIN
    ALTER TABLE tbl_libro
    ADD estado NVARCHAR(20) NOT NULL DEFAULT 'Disponible';
END
GO

-- Agregar campo 'estado' a tbl_prestamo
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'tbl_prestamo') AND name = 'estado')
BEGIN
    ALTER TABLE tbl_prestamo
    ADD estado NVARCHAR(20) NOT NULL DEFAULT 'Activo';
END
GO

-- Agregar campo 'monto' a tbl_multa
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'tbl_multa') AND name = 'monto')
BEGIN
    ALTER TABLE tbl_multa
    ADD monto DECIMAL(10,2) NOT NULL DEFAULT 0;
END
GO

-- Agregar campo 'estado' a tbl_multa
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'tbl_multa') AND name = 'estado')
BEGIN
    ALTER TABLE tbl_multa
    ADD estado NVARCHAR(20) NOT NULL DEFAULT 'Activa';
END
GO

-- Actualizar estados existentes
UPDATE tbl_libro SET estado = 'Disponible' WHERE cantidad_disponible > 0 AND estado IS NULL;
UPDATE tbl_libro SET estado = 'No Disponible' WHERE cantidad_disponible = 0 AND estado IS NULL;

UPDATE tbl_prestamo SET estado = 'Devuelto' WHERE fecha_devolucion IS NOT NULL AND estado IS NULL;
UPDATE tbl_prestamo SET estado = 'Activo' WHERE fecha_devolucion IS NULL AND estado IS NULL;

UPDATE tbl_multa 
SET estado = CASE 
    WHEN DATEADD(DAY, dias_restringido, fecha) <= GETDATE() THEN 'Expirada'
    ELSE 'Activa'
END
WHERE estado IS NULL;

GO

PRINT 'Base de datos actualizada correctamente según el diagrama de clases.';
GO
