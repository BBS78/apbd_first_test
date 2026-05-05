-- Created by Redgate Data Modeler (https://datamodeler.redgate-platform.com)
-- Last modification date: 2026-04-30 03:57:24.092

-- foreign keys
ALTER TABLE Products DROP CONSTRAINT Product_Maker;

ALTER TABLE Products DROP CONSTRAINT Product_ProductType;

ALTER TABLE VendorProducts DROP CONSTRAINT Table_4_Product;

ALTER TABLE VendorProducts DROP CONSTRAINT VendorProducts_Vendors;

-- tables
DROP TABLE Makers;

DROP TABLE ProductTypes;

DROP TABLE Products;

DROP TABLE VendorProducts;

DROP TABLE Vendors;

-- End of file.

