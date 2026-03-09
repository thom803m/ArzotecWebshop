INSERT INTO Brands (Name)
VALUES ('Dell');

INSERT INTO Categories (Name)
VALUES ('Laptops');

INSERT INTO Products (Sku, Name, Price, StockQuantity, BrandId, CategoryId, LastSynced)
VALUES ('ABC123', 'Test Laptop', 9999, 5, 1, 1, GETDATE());