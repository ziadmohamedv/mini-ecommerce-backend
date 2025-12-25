Mini E-Commerce Backend API
A clean, layered ASP.NET Core Web API for a simplified e-commerce system.

Architecture :

Clean Architecture with 4 layers (API, Application, Domain, Infrastructure)
Repository Pattern for data access abstraction
Entity Framework Core with SQLite database

Tech Stack :

ASP.NET Core 9.0 Web API
Entity Framework Core 9.0.11
SQLite Database 9.0.11

Prerequisites :

.NET 9.0 SDK
Visual Studio 2022
SQLite Browser (optional, for viewing database)

Installation :

Clone the repository
Restore dependencies
Run the application

SQLite Database :

The application uses SQLite with automatic database creation on first run
Location: API/miniecommerce.db

Project Structure :

API Layer (Controllers)
Application Layer (Services, DTOs)
Domain Layer (Entities)
Infrastructure Layer (Data, Repositories)

Endpoints :

Products
POST	/api/products	Create a new product	Parameters: name, price, quantity
GET		/api/products	Get all products		Parameters: none

Orders
POST	/api/orders			Create a new order			Parameters: customerName, customerEmail, items[]
GET		/api/orders/{id}	Get order details by ID		Parameters: id

Validation Rules :

Product Creation
Name: Required, max 100 characters
Price: Required, must be > 0
Quantity: Required, must be >= 0

Order Creation
Customer Name: Required, max 100 characters
Customer Email: Required, valid email format
Items: Minimum 1 item
Stock: Must have enough quantity for all products
Product IDs: Must exist in database

Discount Logic
1 item		0%
2-4	items	5%
5+ items	10%	

Layers Description :

Domain Layer : Defines core entities (Product, Order, OrderItem) and repositories contracts.
Infrastructure Layer : Implements data access using Entity Framework Core with SQLite. (implements the repositories contracts)
Application Layer : Implements business logic, validation, and services for products and orders. (services use repositories for data access abstraction)
API Layer : Handles HTTP requests, routing, and response formatting. (controllers use services for business logic and DTOs for data control)

All Test Cases Examples : //Tested on Postman

>Test Case 1: Create Product (Valid)
Request:
POST {{baseUrl}}/api/products
Content-Type: application/json
Body:
json
{
  "name": "Test Laptop",
  "price": 999.99,
  "quantity": 10
}
Expected Response:
Status: 201 Created

>Test Case 2: Create Product (Invalid Price)
Request:
POST {{baseUrl}}/api/products
Body:
json
{
  "name": "Invalid Product",
  "price": 0,
  "quantity": 5
}
Expected Response:
Status: 400 Bad Request

>Test Case 3: Create Product (Invalid Quantity)
Request:
POST {{baseUrl}}/api/products
Body:
json
{
  "name": "Invalid Product 2",
  "price": 50.00,
  "quantity": -1
}
Expected Response:
Status: 400 Bad Request

>Test Case 4: Get All Products
Request:
GET {{baseUrl}}/api/products
Expected Response:
Status: 200 OK

>Test Case 5: Create Order (Single Item - No Discount)
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": [
    {
      "productId": {{testProductId}},
      "quantity": 1
    }
  ]
}
Expected Response:
Status: 201 Created

>Test Case 6: Create Order (3 Items - 5% Discount)
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
Expected Response:
Status: 201 Created

>Test Case 7: Create Order (5 Items - 10% Discount)
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": [
    {
      "productId": 1,
      "quantity": 3
    },
    {
      "productId": 2,
      "quantity": 2
    }
  ]
}
Expected Response:
Status: 201 Created

>Test Case 8: Create Order (Insufficient Stock)
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": [
    {
      "productId": {{testProductId}},
      "quantity": 1000
    }
  ]
}
Expected Response:
Status: 400 Bad Request

>Test Case 9: Create Order (Invalid Product ID)
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": [
    {
      "productId": 99999,
      "quantity": 1
    }
  ]
}
Expected Response:
Status: 400 Bad Request

>Test Case 10: Get Order Details
Request:
GET {{baseUrl}}/api/orders/1
Expected Response:
Status: 200 OK

>Test Case 11: Get Non-Existent Order
Request:
GET {{baseUrl}}/api/orders/99999
Expected Response:
Status: 404 Not Found

>Test Case 12: Create Product with Duplicate Name
Request:
POST {{baseUrl}}/api/products
Body:
{
  "name": "Test Laptop",
  "price": 799.99,
  "quantity": 5
}
Expected Response:
Status: 400 Bad Request

>Test Case 13: Create Order with Empty Items
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "items": []
}
Expected Response:
Status: 400 Bad Request

>Test Case 14: Create Order with Invalid Email
Request:
POST {{baseUrl}}/api/orders
Body:
{
  "customerName": "Test Customer",
  "customerEmail": "invalid-email",
  "items": [
    {
      "productId": {{testProductId}},
      "quantity": 1
    }
  ]
}
Expected Response:
Status: 400 Bad Request