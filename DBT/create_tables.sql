

CREATE TABLE address (
	AddressID INT
	,AddressLine1 String
	,AddressLine2 String
	,City String
	,StateProvince String
	,CountryRegion String
	,PostalCode String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE customer (
	CustomerID INT
	,NameStyle String
	,Title String
	,FirstName String
	,MiddleName String
	,LastName String
	,Suffix String
	,CompanyName String
	,SalesPerson String
	,EmailAddress String
	,Phone String
	,PasswordHash String
	,PasswordSalt String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE customeraddress (
	CustomerID INT
	,AddressID INT
	,AddressType String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE product (
	ProductID INT
	,Name String
	,ProductNumber String
	,Color String
	,StandardCost DECIMAL
	,ListPrice DECIMAL
	,Size String
	,Weight DECIMAL
	,ProductCategoryID INT
	,ProductModelID INT
	,SellStartDate TIMESTAMP
	,SellEndDate TIMESTAMP
	,DiscontinuedDate TIMESTAMP
	,ThumbNailPhoto String
	,ThumbnailPhotoFileName String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE productcategory (
	ProductCategoryID INT
	,ParentProductCategoryID INT
	,Name String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE productdescription (
	ProductDescriptionID INT
	,Description String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE productmodel (
	ProductModelID INT
	,Name String
	,CatalogDescription String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE productmodelproductdescription (
	ProductModelID INT
	,ProductDescriptionID INT
	,Culture String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE salesorderdetail (
	SalesOrderID INT
	,SalesOrderDetailID INT
	,OrderQty INT
	,ProductID INT
	,UnitPrice DECIMAL
	,UnitPriceDiscount DECIMAL
	,LineTotal DECIMAL
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

CREATE TABLE salesorderheader (
	SalesOrderID INT
	,RevisionNumber TINYINT
	,OrderDate TIMESTAMP
	,DueDate TIMESTAMP
	,ShipDate TIMESTAMP
	,STATUS TINYINT
	,OnlineOrderFlag String
	,SalesOrderNumber String
	,PurchaseOrderNumber String
	,AccountNumber String
	,CustomerID INT
	,ShipToAddressID INT
	,BillToAddressID INT
	,ShipMethod String
	,CreditCardApprovalCode String
	,SubTotal DECIMAL
	,TaxAmt DECIMAL
	,Freight DECIMAL
	,TotalDue DECIMAL
	,Comment String
	,rowguid String
	,ModifiedDate TIMESTAMP
	);

