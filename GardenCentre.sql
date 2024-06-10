create database Garden_Centre;
use Garden_Centre;

CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY,
    Name VARCHAR(150),
    Address VARCHAR(255),
    UNIQUE(Name, Address)
);


CREATE TABLE Products (
    ProductID INT PRIMARY KEY,
    DutchName VARCHAR(150),
    ScientificName VARCHAR(150),
    Description VARCHAR(255),
    Price DECIMAL(10, 2) CHECK (Price > 0)
);


CREATE TABLE Quotations (
    QuotationID INT PRIMARY KEY,
    Date DATE,
    CustomerID INT,
    Pickup BIT,
    Installation BIT,
    TotalCost DECIMAL(10, 2),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

CREATE TABLE QuotationProducts (
    QuotationID INT,
    ProductID INT,
    Quantity INT,
    PRIMARY KEY (QuotationID, ProductID),
    FOREIGN KEY (QuotationID) REFERENCES Quotations(QuotationID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);


Select* from Customers;
Select* from Products;
Select* from Quotations;
Select* from QuotationProducts;

Delete from QuotationProducts;
Delete from Quotations;
Delete from Customers;
Delete from Products;

