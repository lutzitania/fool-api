PRINT(N'Add Brand Table and Keys')
IF OBJECT_ID(N'[dbo].[Brand]') IS NULL
CREATE TABLE [dbo].[Brand] (
	id INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1,1),
	BrandName NVARCHAR(5)
)

PRINT(N'Insert Values into Brand table')
INSERT INTO [dbo].[Brand] ([BrandName]) VALUES ('US')
INSERT INTO [dbo].[Brand] ([BrandName]) VALUES ('AU')
INSERT INTO [dbo].[Brand] ([BrandName]) VALUES ('GB')
INSERT INTO [dbo].[Brand] ([BrandName]) VALUES ('DE')

PRINT(N'Update the Brand column in [dbo].[Product] and establish the FK')
UPDATE Product
	SET Brand = '1'
	WHERE Brand = 'US' 
UPDATE Product
	SET Brand = '2'
	WHERE Brand = 'AU'
UPDATE Product
	SET Brand = '3'
	WHERE Brand = 'GB'
UPDATE Product
	SET Brand = '4'
	WHERE Brand = 'DE'
ALTER TABLE Product
	ALTER COLUMN Brand INT

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Product_Brand]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Product]', 'U'))
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_Product_Brand] FOREIGN KEY ([Brand]) REFERENCES [dbo].[Brand] ([id])

PRINT(N'Add a Status Table')
IF OBJECT_ID(N'[dbo].[Status]') IS NULL
CREATE TABLE [dbo].[Status] (
	id INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1,1),
	StatusName NVARCHAR(20)
)

PRINT(N'Insert Values into the Status Table')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Active')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Pending')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Deleted')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Expired')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Discontinued')
INSERT INTO [dbo].[Status] ([StatusName]) VALUES ('Completed')

PRINT(N'Add Status Column to [dbo].[Customer] and add Foreign Key Constraint')
ALTER TABLE [dbo].[Customer]
ADD Status INT NOT NULL DEFAULT(1)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Customer_Status]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Customer]', 'U'))
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [FK_Customer_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Status] ([id])

PRINT(N'Add Status Column to [dbo].[Product] and add Foreign Key Constraint')
ALTER TABLE [dbo].[Product]
ADD Status INT NOT NULL DEFAULT(1)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Product_Status]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Product]', 'U'))
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_Product_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Status] ([id])

PRINT(N'Update [dbo].[Product] New Schema to Reflect [IsActive] in Seed Data')
UPDATE [dbo].[Product]
SET Status = 5
WHERE IsActive = 0

PRINT(N'Add Status Column to [dbo].[Offer] and add Foreign Key Constraint')
ALTER TABLE [dbo].[Offer]
ADD Status INT NOT NULL DEFAULT(1)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Offer_Status]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Offer]', 'U'))
ALTER TABLE [dbo].[Offer] ADD CONSTRAINT [FK_Offer_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Status] ([id])

PRINT(N'Add Status Column to [dbo].[Order] and add Foreign Key Constraint')
ALTER TABLE [dbo].[Order]
ADD Status INT NOT NULL DEFAULT(6)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Status]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]', 'U'))
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Status] ([id])

PRINT(N'Update [dbo].[Order] Schema')
ALTER TABLE [dbo].[Order]
ADD PurchasePrice DECIMAL NULL DEFAULT(NULL)

ALTER TABLE [dbo].[Order]
ADD PurchaseDate DATETIME NULL DEFAULT(NULL)

ALTER TABLE [dbo].[Order]
ADD ProductId INT NULL DEFAULT(NULL)

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Product]', 'F') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]', 'U'))
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])

PRINT(N'Update the [dbo].[Offer].[Status] field to reflect expired orders')
DECLARE @temp TABLE(
	ProductId INT,
	OfferId INT,
	Term NVARCHAR(10),
	DateCreated DATETIME,
	NumberOfTerms INT,
	Expiry DATETIME
)

INSERT INTO @temp
SELECT p.ProductId, o.OfferId, p.Term, o.DateCreated, o.NumberOfTerms, DATEADD(mm, o.NumberOfTerms, o.DateCreated)
FROM [dbo].[Product] p
INNER JOIN [dbo].[Offer] o ON p.ProductId = o.ProductId
WHERE p.Term = 'monthly'

INSERT INTO @temp
SELECT p.ProductId, o.OfferId, p.Term, o.DateCreated, o.NumberOfTerms, DATEADD(yy, o.NumberOfTerms, o.DateCreated)
FROM [dbo].[Product] p
INNER JOIN [dbo].[Offer] o ON p.ProductId = o.ProductId
WHERE p.Term = 'annually'

UPDATE Offer
SET Status = 4
WHERE OfferId IN (SELECT OfferId FROM @temp WHERE Expiry < GETDATE()) -- Executed at 2018-04-24 19:01:00