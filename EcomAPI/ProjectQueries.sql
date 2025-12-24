-- ALWAYS CREATE A JUNCTION TABLE IN CASE OF MANY-TO-MANY RELATIONSHIPS
-- ADD A FOREIGN KEY IN THE MANY SIDE IN CASE OF ONE-TO-MANY RELATIONSHIPS

--CREATE TABLE ProductCategories (
--    Id INT PRIMARY KEY IDENTITY(1,1),
--    Title NVARCHAR(100) NOT NULL,
--    Description NVARCHAR(500),
--    ImageUrl NVARCHAR(500),
--    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
--    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
--);

--CREATE TABLE Products (
--    Id INT PRIMARY KEY IDENTITY(1,1),
--    Name NVARCHAR(200) NOT NULL,
--    Description NVARCHAR(1000),
--    Price DECIMAL(18,2) NOT NULL,
--    CategoryId INT NOT NULL,
--    ImageUrl NVARCHAR(500),
--    StockQuantity INT NOT NULL DEFAULT 0,
--    IsAvailable BIT NOT NULL DEFAULT 1,
--    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
--    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

--    CONSTRAINT FK_Products_Category FOREIGN KEY (CategoryId) REFERENCES ProductCategories(Id)
--);


--CREATE TABLE Orders (
--    Id INT PRIMARY KEY IDENTITY(1,1),
--    UserId INT NOT NULL,
--    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
--    TotalAmount DECIMAL(18,2) NOT NULL,
--    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
--    ShippingAddress NVARCHAR(500) NOT NULL,
--    PaymentMethod NVARCHAR(50),
--    PaymentStatus NVARCHAR(50) DEFAULT 'Unpaid',
--    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
--    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

--    CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES Users(Id)

--);

--CREATE TABLE OrderItems (
--    Id INT PRIMARY KEY IDENTITY(1,1),
--    OrderId INT NOT NULL,
--    ProductId INT NOT NULL,
--    Quantity INT NOT NULL,
--    UnitPrice DECIMAL(18,2) NOT NULL,
--    Subtotal DECIMAL(18,2) NOT NULL,

--    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
--    CONSTRAINT FK_OrderItems_Product FOREIGN KEY (ProductId) REFERENCES Products(Id)
--);

--INSERT INTO ProductCategories (Title, Description, ImageUrl) VALUES
--('Electronics', 'Electronic devices and accessories', 'https://example.com/electronics.jpg'),
--('Clothing', 'Fashion and apparel', 'https://example.com/clothing.jpg'),
--('Books', 'Books and publications', 'https://example.com/books.jpg'),
--('Home & Garden', 'Home improvement and garden supplies', 'https://example.com/home.jpg');


--INSERT INTO Products (Name, Description, Price, CategoryId, ImageUrl, StockQuantity) VALUES
--('Wireless Mouse', 'Ergonomic wireless mouse with USB receiver', 29.99, 1, 'https://example.com/mouse.jpg', 50),
--('Bluetooth Headphones', 'Noise-cancelling over-ear headphones', 149.99, 1, 'https://example.com/headphones.jpg', 30),
--('Cotton T-Shirt', 'Comfortable 100% cotton t-shirt', 19.99, 2, 'https://example.com/tshirt.jpg', 100),
--('Programming Book', 'Learn C# and .NET Core', 49.99, 3, 'https://example.com/book.jpg', 20);

--INSERT INTO Orders (UserId, OrderNumber, TotalAmount, Status, ShippingAddress, PaymentMethod, PaymentStatus) 
--VALUES (16, 'ORD-2025-0001', 199.97, 'Pending', '123 Main St, Dubai, UAE', 'Credit Card', 'Paid');

--INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, Subtotal) VALUES
--(2, 1, 2, 29.99, 59.98),
--(2, 2, 1, 149.99, 149.99);