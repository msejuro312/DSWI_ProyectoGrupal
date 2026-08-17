CREATE DATABASE DB_SERFAGAB_DSWI
GO
use DB_SERFAGAB_DSWI
go

CREATE TABLE tbl_tipo_material
(
    id_tipo_material INT IDENTITY PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(255),
    activo BIT DEFAULT 1
);

CREATE TABLE tbl_material
(
    id_material INT IDENTITY PRIMARY KEY,
    id_tipo_material INT references tbl_tipo_material(id_tipo_material),
    nombre NVARCHAR(150) NOT NULL,
    unidad_medida NVARCHAR(20) NOT NULL,
    stock_actual DECIMAL(10,2) DEFAULT 0.00,
    precio_referencial DECIMAL(10,2) DEFAULT 0.00,
    descripcion NVARCHAR(255),
    activo BIT DEFAULT 1
);

INSERT INTO tbl_tipo_material (nombre, descripcion)
VALUES
('Plancha', 'Son materiales metalicos para tablero'),
('Pintura', 'Gris ANSI 61');

INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
VALUES
(1, 'Perno 3x3', 'unidad', 10, 3, 'para tablero'),
(2, 'Perno 4x3', 'unidad', 12, 4, 'para tablero');
go

create or alter procedure sp_list_tipo_material
as
begin
    select
    id_tipo_material,
    nombre,
    descripcion
    from tbl_tipo_material
    order by nombre
end
go

create or alter procedure sp_list_materiales
as
begin
    select
    m.id_material,
    m.id_tipo_material,
    m.nombre,
    m.unidad_medida,
    m.stock_actual,
    m.precio_referencial,
    m.descripcion,
    t.nombre 'TipoMaterial'
    from tbl_material m
    join tbl_tipo_material t on m.id_tipo_material = t.id_tipo_material
end
go

create or alter procedure sp_find_material_by_id
@idMaterial int
as
begin
    select
    m.id_material,
    m.id_tipo_material,
    m.nombre,
    m.unidad_medida,
    m.stock_actual,
    m.precio_referencial,
    m.descripcion,
    t.nombre 'TipoMaterial'
    from tbl_material m
    join tbl_tipo_material t on m.id_tipo_material = t.id_tipo_material
    where m.id_material = @idMaterial
end
go

create or alter procedure sp_insert_material
@idTipoMaterial INT,
@nombre NVARCHAR(150),
@unidadMedida NVARCHAR(20),
@stockActual DECIMAL(10,2),
@precioReferencial DECIMAL(10,2),
@descripcion NVARCHAR(255)
as
begin
    insert tbl_material(id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    values(@idTipoMaterial, @nombre, @unidadMedida, @stockActual, @precioReferencial, @descripcion)
end
go

create or alter procedure sp_update_material
@idMaterial INT,
@idTipoMaterial INT,
@nombre NVARCHAR(150),
@unidadMedida NVARCHAR(20),
@stockActual DECIMAL(10,2),
@precioReferencial DECIMAL(10,2),
@descripcion NVARCHAR(255)
as
begin
    update tbl_material
    set
    id_tipo_material = @idTipoMaterial,
    nombre = @nombre,
    unidad_medida = @unidadMedida,
    stock_actual = @stockActual,
    precio_referencial = @precioReferencial,
    descripcion = @descripcion
    where id_material = @idMaterial
end
go

create or alter procedure sp_delete_material
@idMaterial INT
as
begin
    delete from tbl_material
    where id_material = @idMaterial
end
go

-- =====================================================================
-- AVANCES: PROVEEDOR + ORDEN DE COMPRA + TIPO MATERIAL CRUD
-- Ejecutar SOLO desde aqui en adelante si la base ya existe
-- (el CREATE DATABASE del inicio fallaria porque la base ya esta creada)
-- =====================================================================

-- 1) Tablas nuevas ------------------------------------------------
CREATE TABLE tbl_proveedor
(
    id_proveedor INT IDENTITY PRIMARY KEY,
    razon_social NVARCHAR(200) NOT NULL,
    ruc NVARCHAR(11) NOT NULL,
    celular NVARCHAR(20),
    email NVARCHAR(100),
    descripcion NVARCHAR(255),
    activo BIT DEFAULT 1
);
GO

CREATE TABLE tbl_orden_compra
(
    id_orden_compra INT IDENTITY PRIMARY KEY,
    id_proveedor INT references tbl_proveedor(id_proveedor),
    fecha DATETIME DEFAULT GETDATE(),
    estado NVARCHAR(20) DEFAULT 'PENDIENTE',
    total DECIMAL(10,2) DEFAULT 0.00,
    observaciones NVARCHAR(255)
);
GO

CREATE TABLE tbl_detalle_orden_compra
(
    id_detalle INT IDENTITY PRIMARY KEY,
    id_orden_compra INT references tbl_orden_compra(id_orden_compra),
    id_material INT references tbl_material(id_material),
    cantidad DECIMAL(10,2) NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL
);
GO

-- 2) Datos iniciales de proveedores --------------------------------
INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion)
VALUES
('SERFAGAB Distribuciones SAC', '20512345678', '987654321', 'ventas@serfagab.com', 'Proveedor principal de materiales'),
('Metales del Peru EIRL', '20123456789', '912345678', 'contacto@metales.com', 'Planchas y pintura');
GO

-- 3) SPs de Proveedor ----------------------------------------------
create or alter procedure sp_list_proveedores
as
begin
    select
    id_proveedor,
    razon_social,
    ruc,
    celular,
    email,
    descripcion
    from tbl_proveedor
    order by razon_social
end
GO

create or alter procedure sp_find_proveedor_by_id
@idProveedor int
as
begin
    select
    id_proveedor,
    razon_social,
    ruc,
    celular,
    email,
    descripcion
    from tbl_proveedor
    where id_proveedor = @idProveedor
end
GO

create or alter procedure sp_insert_proveedor
@razonSocial NVARCHAR(200),
@ruc NVARCHAR(11),
@celular NVARCHAR(20),
@email NVARCHAR(100),
@descripcion NVARCHAR(255)
as
begin
    insert tbl_proveedor(razon_social, ruc, celular, email, descripcion)
    values(@razonSocial, @ruc, @celular, @email, @descripcion)
end
GO

create or alter procedure sp_update_proveedor
@idProveedor INT,
@razonSocial NVARCHAR(200),
@ruc NVARCHAR(11),
@celular NVARCHAR(20),
@email NVARCHAR(100),
@descripcion NVARCHAR(255)
as
begin
    update tbl_proveedor
    set
    razon_social = @razonSocial,
    ruc = @ruc,
    celular = @celular,
    email = @email,
    descripcion = @descripcion
    where id_proveedor = @idProveedor
end
GO

create or alter procedure sp_delete_proveedor
@idProveedor INT
as
begin
    delete from tbl_proveedor
    where id_proveedor = @idProveedor
end
GO

-- 4) SPs de TipoMaterial (completar CRUD) ---------------------------
create or alter procedure sp_find_tipo_material_by_id
@idTipoMaterial int
as
begin
    select
    id_tipo_material,
    nombre,
    descripcion
    from tbl_tipo_material
    where id_tipo_material = @idTipoMaterial
end
GO

create or alter procedure sp_insert_tipo_material
@nombre NVARCHAR(100),
@descripcion NVARCHAR(255)
as
begin
    insert tbl_tipo_material(nombre, descripcion)
    values(@nombre, @descripcion)
end
GO

create or alter procedure sp_update_tipo_material
@idTipoMaterial INT,
@nombre NVARCHAR(100),
@descripcion NVARCHAR(255)
as
begin
    update tbl_tipo_material
    set
    nombre = @nombre,
    descripcion = @descripcion
    where id_tipo_material = @idTipoMaterial
end
GO

create or alter procedure sp_delete_tipo_material
@idTipoMaterial INT,
@resultado INT OUTPUT
as
begin
    if not exists(select 1 from tbl_tipo_material where id_tipo_material = @idTipoMaterial)
    begin
        set @resultado = 0  -- no existe
    end
    else if exists(select 1 from tbl_material where id_tipo_material = @idTipoMaterial)
    begin
        set @resultado = -1  -- bloqueado: tiene materiales asociados
    end
    else
    begin
        delete from tbl_tipo_material where id_tipo_material = @idTipoMaterial
        set @resultado = 1   -- eliminado
    end
end
GO

-- 5) SPs de Orden de Compra ----------------------------------------
create or alter procedure sp_list_ordenes
as
begin
    select
    o.id_orden_compra,
    o.id_proveedor,
    p.razon_social 'Proveedor',
    o.fecha,
    o.estado,
    o.total,
    o.observaciones
    from tbl_orden_compra o
    join tbl_proveedor p on o.id_proveedor = p.id_proveedor
    order by o.id_orden_compra desc
end
GO

create or alter procedure sp_find_orden_by_id
@idOrdenCompra int
as
begin
    select
    o.id_orden_compra,
    o.id_proveedor,
    p.razon_social 'Proveedor',
    o.fecha,
    o.estado,
    o.total,
    o.observaciones
    from tbl_orden_compra o
    join tbl_proveedor p on o.id_proveedor = p.id_proveedor
    where o.id_orden_compra = @idOrdenCompra
end
GO

create or alter procedure sp_list_detalle_orden
@idOrdenCompra int
as
begin
    select
    d.id_detalle,
    d.id_orden_compra,
    d.id_material,
    m.nombre 'Material',
    d.cantidad,
    d.precio_unitario,
    d.subtotal
    from tbl_detalle_orden_compra d
    join tbl_material m on d.id_material = m.id_material
    where d.id_orden_compra = @idOrdenCompra
end
GO

create or alter procedure sp_insert_orden_compra
@idProveedor INT,
@fecha DATETIME,
@estado NVARCHAR(20),
@total DECIMAL(10,2),
@observaciones NVARCHAR(255)
as
begin
    if @fecha is null
        set @fecha = GETDATE()
    if @estado is null or @estado = ''
        set @estado = 'PENDIENTE'
    insert tbl_orden_compra(id_proveedor, fecha, estado, total, observaciones)
    values(@idProveedor, @fecha, @estado, @total, @observaciones)
    select SCOPE_IDENTITY() as 'IdOrdenCompra'
end
GO

create or alter procedure sp_insert_detalle_orden
@idOrdenCompra INT,
@idMaterial INT,
@cantidad DECIMAL(10,2),
@precioUnitario DECIMAL(10,2)
as
begin
    declare @subtotal decimal(10,2)
    set @subtotal = @cantidad * @precioUnitario
    insert tbl_detalle_orden_compra(id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
    values(@idOrdenCompra, @idMaterial, @cantidad, @precioUnitario, @subtotal)
end
GO

create or alter procedure sp_delete_orden_compra
@idOrdenCompra INT
as
begin
    delete from tbl_detalle_orden_compra where id_orden_compra = @idOrdenCompra
    delete from tbl_orden_compra where id_orden_compra = @idOrdenCompra
end
GO


--NUEVOS INSERTS

-- 1) Tipos de Material adicionales (evita duplicar si ya existen)
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Perno')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Perno', 'Sujetadores metalicos');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Cable')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Cable', 'Conductores electricos');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Interruptor')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Interruptor', 'Dispositivos de proteccion electrica');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Riel DIN')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Riel DIN', 'Riel de montaje para tablero');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Prensaestopa')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Prensaestopa', 'Sellado de ingreso de cables');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Conector')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Conector', 'Conectores electricos');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Borne')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Borne', 'Bornes de conexion');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Contactor')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Contactor', 'Contactores electricos');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Fusible')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Fusible', 'Proteccion de sobrecorriente');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Canaleta')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Canaleta', 'Canalizacion de cables');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Terminal')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Terminal', 'Terminales para cable');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Tornilleria')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Tornilleria', 'Tornillos y accesorios de sujecion');
IF NOT EXISTS (SELECT 1 FROM tbl_tipo_material WHERE nombre = 'Empaquetadura')
    INSERT INTO tbl_tipo_material (nombre, descripcion) VALUES ('Empaquetadura', 'Sellos y empaques para gabinetes');
GO

-- 2) Materiales adicionales
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Plancha LAF 1/16"')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Plancha LAF 1/16"', 'plancha', 25, 145.00, 'Plancha metalica para gabinete' FROM tbl_tipo_material WHERE nombre = 'Plancha';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Plancha LAF 1/8"')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Plancha LAF 1/8"', 'plancha', 18, 210.00, 'Plancha metalica reforzada' FROM tbl_tipo_material WHERE nombre = 'Plancha';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Pintura Gris ANSI 61')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Pintura Gris ANSI 61', 'galon', 30, 85.50, 'Pintura electrostatica gris' FROM tbl_tipo_material WHERE nombre = 'Pintura';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Pintura Negro Mate')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Pintura Negro Mate', 'galon', 12, 78.00, 'Pintura acabado mate' FROM tbl_tipo_material WHERE nombre = 'Pintura';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Perno 3x3')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Perno 3x3', 'unidad', 500, 0.35, 'Para tablero' FROM tbl_tipo_material WHERE nombre = 'Perno';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Perno 4x3')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Perno 4x3', 'unidad', 480, 0.42, 'Para tablero' FROM tbl_tipo_material WHERE nombre = 'Perno';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Perno Hexagonal M6')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Perno Hexagonal M6', 'unidad', 600, 0.28, 'Sujecion estructural' FROM tbl_tipo_material WHERE nombre = 'Perno';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Cable THW 12 AWG')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Cable THW 12 AWG', 'metro', 350, 2.10, 'Cable para instalaciones internas' FROM tbl_tipo_material WHERE nombre = 'Cable';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Cable THW 14 AWG')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Cable THW 14 AWG', 'metro', 420, 1.65, 'Cable para instalaciones internas' FROM tbl_tipo_material WHERE nombre = 'Cable';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Interruptor Termomagnetico 2x20A')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Interruptor Termomagnetico 2x20A', 'unidad', 45, 38.00, 'Proteccion de circuitos' FROM tbl_tipo_material WHERE nombre = 'Interruptor';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Interruptor Termomagnetico 3x32A')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Interruptor Termomagnetico 3x32A', 'unidad', 30, 62.00, 'Proteccion de circuitos' FROM tbl_tipo_material WHERE nombre = 'Interruptor';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Riel DIN 35mm')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Riel DIN 35mm', 'metro', 90, 6.50, 'Montaje de dispositivos' FROM tbl_tipo_material WHERE nombre = 'Riel DIN';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Prensaestopa PG13.5')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Prensaestopa PG13.5', 'unidad', 200, 1.20, 'Sellado de cables' FROM tbl_tipo_material WHERE nombre = 'Prensaestopa';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Conector Rapido 3 vias', 'unidad', 150, 1.80, 'Conexion electrica' FROM tbl_tipo_material WHERE nombre = 'Conector';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Borne de Conexion 4mm')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Borne de Conexion 4mm', 'unidad', 300, 0.95, 'Conexion de conductores' FROM tbl_tipo_material WHERE nombre = 'Borne';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Contactor 25A 220V')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Contactor 25A 220V', 'unidad', 20, 55.00, 'Control de motores' FROM tbl_tipo_material WHERE nombre = 'Contactor';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Fusible Cilindrico 10A')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Fusible Cilindrico 10A', 'unidad', 100, 2.50, 'Proteccion de sobrecorriente' FROM tbl_tipo_material WHERE nombre = 'Fusible';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Canaleta Ranurada 40x40', 'metro', 60, 8.90, 'Canalizacion de cableado' FROM tbl_tipo_material WHERE nombre = 'Canaleta';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Terminal Tipo Ojo 12mm')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Terminal Tipo Ojo 12mm', 'unidad', 250, 0.60, 'Terminacion de cable' FROM tbl_tipo_material WHERE nombre = 'Terminal';
IF NOT EXISTS (SELECT 1 FROM tbl_material WHERE nombre = 'Tornillo Autorroscante 1/2"')
    INSERT INTO tbl_material (id_tipo_material, nombre, unidad_medida, stock_actual, precio_referencial, descripcion)
    SELECT id_tipo_material, 'Tornillo Autorroscante 1/2"', 'unidad', 800, 0.15, 'Fijacion de laminas' FROM tbl_tipo_material WHERE nombre = 'Tornilleria';
GO

-- 3) Proveedores adicionales
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20456789123')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Electro Industrial SAC', '20456789123', '945612378', 'ventas@electroindustrial.pe', 'Componentes electricos');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20678912345')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Aceros y Planchas del Sur SAC', '20678912345', '923456789', 'contacto@acerosdelsur.pe', 'Planchas metalicas laminadas');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20789123456')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Pinturas Industriales Lima SAC', '20789123456', '934567891', 'ventas@pinturaslima.pe', 'Pinturas electrostaticas');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20891234567')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Cables y Conductores del Peru SAC', '20891234567', '956789123', 'ventas@cablesperu.pe', 'Cables y conductores electricos');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20912345678')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Fijaciones y Pernos SAC', '20912345678', '967891234', 'ventas@fijacionesperu.pe', 'Pernos y tornilleria industrial');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20234567891')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Componentes Electricos del Norte SAC', '20234567891', '978912345', 'contacto@cen.pe', 'Interruptores y contactores');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20345678912')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Ferreteria Industrial Callao SAC', '20345678912', '989123456', 'ventas@ferreteriacallao.pe', 'Ferreteria industrial general');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20567891234')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Distribuidora Electrotec SAC', '20567891234', '990123456', 'ventas@electrotec.pe', 'Distribucion de material electrico');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20678123459')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Suministros Metalmecanicos SAC', '20678123459', '911234567', 'contacto@sumetal.pe', 'Suministros metalmecanicos');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20789234561')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Importadora Riel y Canaleta SAC', '20789234561', '922345678', 'ventas@rielycanaleta.pe', 'Importacion de riel DIN y canaletas');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20891345672')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Terminales y Conectores SAC', '20891345672', '933456789', 'ventas@terminalesyconectores.pe', 'Terminales y bornes de conexion');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20912456783')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Proteccion Electrica del Peru SAC', '20912456783', '944567890', 'ventas@protecper.pe', 'Fusibles y proteccion electrica');
IF NOT EXISTS (SELECT 1 FROM tbl_proveedor WHERE ruc = '20123567894')
    INSERT INTO tbl_proveedor (razon_social, ruc, celular, email, descripcion) VALUES ('Acabados Industriales SAC', '20123567894', '955678901', 'ventas@acabadosindustriales.pe', 'Empaques y acabados para gabinetes');
GO

-- 4) Ordenes de Compra con su Detalle (19 ordenes nuevas, con fechas y estados variados)
-- Usa nombres para resolver IDs, de forma que funcione sin importar el orden de insercion previo.
-- Orden 1: 2026-02-03 - SERFAGAB Distribuciones SAC - PENDIENTE
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260203', 'PENDIENTE', 0, 'Orden de compra generada para pruebas de reportes y paginacion #1' FROM tbl_proveedor WHERE razon_social = 'SERFAGAB Distribuciones SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 76.01, 10*76.01 FROM tbl_material WHERE nombre = 'Pintura Negro Mate';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 148.43, 5*148.43 FROM tbl_material WHERE nombre = 'Plancha LAF 1/16"';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 2: 2026-02-10 - Metales del Peru EIRL - APROBADA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260210', 'APROBADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #2' FROM tbl_proveedor WHERE razon_social = 'Metales del Peru EIRL';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 8.48, 2*8.48 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 83.21, 8*83.21 FROM tbl_material WHERE nombre = 'Pintura Gris ANSI 61';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 1.81, 2*1.81 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 3: 2026-02-18 - Electro Industrial SAC - RECIBIDA - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260218', 'RECIBIDA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #3' FROM tbl_proveedor WHERE razon_social = 'Electro Industrial SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 9.18, 10*9.18 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 1.85, 2*1.85 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 2.14, 5*2.14 FROM tbl_material WHERE nombre = 'Cable THW 12 AWG';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 0.93, 12*0.93 FROM tbl_material WHERE nombre = 'Borne de Conexion 4mm';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 4: 2026-02-25 - Aceros y Planchas del Sur SAC - ANULADA - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260225', 'ANULADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #4' FROM tbl_proveedor WHERE razon_social = 'Aceros y Planchas del Sur SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 0.27, 3*0.27 FROM tbl_material WHERE nombre = 'Perno Hexagonal M6';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 61.13, 3*61.13 FROM tbl_material WHERE nombre = 'Interruptor Termomagnetico 3x32A';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 5: 2026-03-02 - Pinturas Industriales Lima SAC - PENDIENTE
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260302', 'PENDIENTE', 0, 'Orden de compra generada para pruebas de reportes y paginacion #5' FROM tbl_proveedor WHERE razon_social = 'Pinturas Industriales Lima SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 6.52, 20*6.52 FROM tbl_material WHERE nombre = 'Riel DIN 35mm';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 15, 1.58, 15*1.58 FROM tbl_material WHERE nombre = 'Cable THW 14 AWG';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 216.92, 10*216.92 FROM tbl_material WHERE nombre = 'Plancha LAF 1/8"';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 6: 2026-03-09 - Cables y Conductores del Peru SAC - APROBADA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260309', 'APROBADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #6' FROM tbl_proveedor WHERE razon_social = 'Cables y Conductores del Peru SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 0.15, 2*0.15 FROM tbl_material WHERE nombre = 'Tornillo Autorroscante 1/2"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 6.82, 10*6.82 FROM tbl_material WHERE nombre = 'Riel DIN 35mm';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 0.29, 8*0.29 FROM tbl_material WHERE nombre = 'Perno Hexagonal M6';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 15, 83.60, 15*83.60 FROM tbl_material WHERE nombre = 'Pintura Gris ANSI 61';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 7: 2026-03-16 - Fijaciones y Pernos SAC - RECIBIDA - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260316', 'RECIBIDA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #7' FROM tbl_proveedor WHERE razon_social = 'Fijaciones y Pernos SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 6.41, 12*6.41 FROM tbl_material WHERE nombre = 'Riel DIN 35mm';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 0.43, 10*0.43 FROM tbl_material WHERE nombre = 'Perno 4x3';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 8: 2026-03-23 - Componentes Electricos del Norte SAC - ANULADA -
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260323', 'ANULADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #8' FROM tbl_proveedor WHERE razon_social = 'Componentes Electricos del Norte SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 82.62, 8*82.62 FROM tbl_material WHERE nombre = 'Pintura Gris ANSI 61';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 15, 0.41, 15*0.41 FROM tbl_material WHERE nombre = 'Perno 4x3';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 9.06, 8*9.06 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 9: 2026-03-30 - Ferreteria Industrial Callao SAC - PENDIENTE - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260330', 'PENDIENTE', 0, 'Orden de compra generada para pruebas de reportes y paginacion #9' FROM tbl_proveedor WHERE razon_social = 'Ferreteria Industrial Callao SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 15, 205.12, 15*205.12 FROM tbl_material WHERE nombre = 'Plancha LAF 1/8"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 2.19, 8*2.19 FROM tbl_material WHERE nombre = 'Cable THW 12 AWG';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 0.15, 12*0.15 FROM tbl_material WHERE nombre = 'Tornillo Autorroscante 1/2"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 61.35, 20*61.35 FROM tbl_material WHERE nombre = 'Interruptor Termomagnetico 3x32A';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 10: 2026-04-06 - Distribuidora Electrotec SAC - APROBADA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260406', 'APROBADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #10' FROM tbl_proveedor WHERE razon_social = 'Distribuidora Electrotec SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 0.92, 10*0.92 FROM tbl_material WHERE nombre = 'Borne de Conexion 4mm';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 0.36, 10*0.36 FROM tbl_material WHERE nombre = 'Perno 3x3';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 11: 2026-04-13 - Suministros Metalmecanicos SAC - RECIBIDA  --- 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260413', 'RECIBIDA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #11' FROM tbl_proveedor WHERE razon_social = 'Suministros Metalmecanicos SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 1.75, 12*1.75 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 0.60, 5*0.60 FROM tbl_material WHERE nombre = 'Terminal Tipo Ojo 12mm';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 1.23, 3*1.23 FROM tbl_material WHERE nombre = 'Prensaestopa PG13.5';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 12: 2026-04-20 - Importadora Riel y Canaleta SAC - ANULADA - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260420', 'ANULADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #12' FROM tbl_proveedor WHERE razon_social = 'Importadora Riel y Canaleta SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 77.10, 3*77.10 FROM tbl_material WHERE nombre = 'Pintura Negro Mate';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 0.35, 20*0.35 FROM tbl_material WHERE nombre = 'Perno 3x3';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 0.43, 2*0.43 FROM tbl_material WHERE nombre = 'Perno 4x3';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 1.83, 3*1.83 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 13: 2026-04-27 - Terminales y Conectores SAC - PENDIENTE - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260427', 'PENDIENTE', 0, 'Orden de compra generada para pruebas de reportes y paginacion #13' FROM tbl_proveedor WHERE razon_social = 'Terminales y Conectores SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 8.55, 12*8.55 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 15, 1.59, 15*1.59 FROM tbl_material WHERE nombre = 'Cable THW 14 AWG';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 14: 2026-05-04 - Proteccion Electrica del Peru SAC - APROBADA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260504', 'APROBADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #14' FROM tbl_proveedor WHERE razon_social = 'Proteccion Electrica del Peru SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 145.11, 5*145.11 FROM tbl_material WHERE nombre = 'Plancha LAF 1/16"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 1.71, 3*1.71 FROM tbl_material WHERE nombre = 'Cable THW 14 AWG';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 2.59, 10*2.59 FROM tbl_material WHERE nombre = 'Fusible Cilindrico 10A';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 15: 2026-05-11 - Acabados Industriales SAC - RECIBIDA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260511', 'RECIBIDA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #15' FROM tbl_proveedor WHERE razon_social = 'Acabados Industriales SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 2.51, 5*2.51 FROM tbl_material WHERE nombre = 'Fusible Cilindrico 10A';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 0.28, 2*0.28 FROM tbl_material WHERE nombre = 'Perno Hexagonal M6';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 0.33, 20*0.33 FROM tbl_material WHERE nombre = 'Perno 3x3';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 6.75, 12*6.75 FROM tbl_material WHERE nombre = 'Riel DIN 35mm';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 16: 2026-05-18 - SERFAGAB Distribuciones SAC - ANULADA - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260518', 'ANULADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #16' FROM tbl_proveedor WHERE razon_social = 'SERFAGAB Distribuciones SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 37.02, 2*37.02 FROM tbl_material WHERE nombre = 'Interruptor Termomagnetico 2x20A';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 3, 2.01, 3*2.01 FROM tbl_material WHERE nombre = 'Cable THW 12 AWG';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 17: 2026-05-25 - Metales del Peru EIRL - PENDIENTE - 
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260525', 'PENDIENTE', 0, 'Orden de compra generada para pruebas de reportes y paginacion #17' FROM tbl_proveedor WHERE razon_social = 'Metales del Peru EIRL';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 52.96, 5*52.96 FROM tbl_material WHERE nombre = 'Contactor 25A 220V';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 89.32, 20*89.32 FROM tbl_material WHERE nombre = 'Pintura Gris ANSI 61';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 5, 8.69, 5*8.69 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 18: 2026-06-01 - Electro Industrial SAC - APROBADA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260601', 'APROBADA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #18' FROM tbl_proveedor WHERE razon_social = 'Electro Industrial SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 10, 0.15, 10*0.15 FROM tbl_material WHERE nombre = 'Tornillo Autorroscante 1/2"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 12, 1.79, 12*1.79 FROM tbl_material WHERE nombre = 'Conector Rapido 3 vias';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 20, 0.27, 20*0.27 FROM tbl_material WHERE nombre = 'Perno Hexagonal M6';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 8.51, 8*8.51 FROM tbl_material WHERE nombre = 'Canaleta Ranurada 40x40';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO

-- Orden 19: 2026-06-08 - Aceros y Planchas del Sur SAC - RECIBIDA
DECLARE @idOrden INT;
INSERT INTO tbl_orden_compra (id_proveedor, fecha, estado, total, observaciones)
SELECT id_proveedor, '20260608', 'RECIBIDA', 0, 'Orden de compra generada para pruebas de reportes y paginacion #19' FROM tbl_proveedor WHERE razon_social = 'Aceros y Planchas del Sur SAC';
SET @idOrden = SCOPE_IDENTITY();
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 8, 146.28, 8*146.28 FROM tbl_material WHERE nombre = 'Plancha LAF 1/16"';
INSERT INTO tbl_detalle_orden_compra (id_orden_compra, id_material, cantidad, precio_unitario, subtotal)
SELECT @idOrden, id_material, 2, 0.57, 2*0.57 FROM tbl_material WHERE nombre = 'Terminal Tipo Ojo 12mm';
UPDATE tbl_orden_compra SET total = (SELECT SUM(subtotal) FROM tbl_detalle_orden_compra WHERE id_orden_compra = @idOrden) WHERE id_orden_compra = @idOrden;
GO
