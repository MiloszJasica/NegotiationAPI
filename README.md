# NegotiationAPI

## Menu
- [Description](#description)
- [Features](#features)
- [Technologies](#technologies)
- [Installation](#installation)
- [Authentication & Login](#authentication--login)
- [API Documentation](#api-documentation)
- [Usage Examples](#usage-examples)
  
## Description
NegotiationAPI is a REST API that enables users to submit negotiation offers for products.
User can create an account and log in to get more features.

## Features
- **For unauthenticated users:**
  - View products.
  - Submit negotiation offers for products. (you only have 3 attempts for a product)

- **For authenticated users:**
  - View, create, edit, and delete products.
  - View received negotiation offers.
  - Accept or reject negotiation offers.

## Technologies
NegotiationAPI is built using the following technologies:

- **Backend:** ASP.NET Core 9.0
- **Authentication & Authorization:**
  - JwtBearer Authentication
  - IdentityModel.Tokens
- **Database Management:** Entity Framework Core
- **API Documentation:** Swagger

## Installation

Follow these steps to set up and run the project locally:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/MiloszJasica/NegotiationAPI.git
   ```
2. **Navigate to the project directory:**
   ```bash
   cd NegotiationAPI
   ```
3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
4. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```
5. **Run the application:**
   ```bash
   dotnet run
   ```

## API Documentation

The API documentation is available at `/swagger` after running the application. It provides details about available endpoints and allows for API testing.

## Authentication & Login

To access protected endpoints, follow these steps:

1. **Register a new account:**
   ```http
   POST /api/User/register
   {
     "username": "testUser",
     "passwordHash": "1234abcd"
   }
   ```
2. **Log in with your credentials:**
   ```http
   POST /api/User/login
   {
     "username": "testUser",
     "passwordHash": "1234abcd"
   }
   ```
   The response will contain a JWT token. Copy the token, go to the top of the Swagger page, click the Authorize button, and enter the token in the format: Bearer {your_token}. After authorizing, you will be able to access protected endpoints.

3. Use the token for authorization:

    - In API testing tools (for example Swagger), go to the Authorize button.

    - Enter the token in the format: Bearer {your_token}.

    - Now, you can access protected endpoints.

## Usage Examples

- **Submit a negotiation offer:**
  ```http
  POST /api/NegotiationOffer
  {
  "productId": 5,
  "proposedPrice": 10,
  "offerDetails": "test",
  "createdAt": "2025-03-13T18:43:29.836Z",
  "respondedAt": "2025-03-13T18:43:29.836Z",
  "senderUsername": "testUser",
  "status": "test"
  }
  ```
- **Create a new product (authenticated users only):**
  ```http
  POST /api/Product
  {
  "name": "testProduct",
  "price": 10,
  "ownerUsername": "testUser"
  }
  ```

