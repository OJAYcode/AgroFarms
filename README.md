# AgroFarms - Agricultural Marketplace

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Database Schema](#database-schema)
- [Authentication and Authorization](#authentication-and-authorization)
- [Services](#services)
- [Controllers](#controllers)
- [Models](#models)
- [Views](#views)
- [API Endpoints](#api-endpoints)
- [Styling and UI](#styling-and-ui)
- [Development Setup](#development-setup)
- [Common Issues and Solutions](#common-issues-and-solutions)
- [Future Enhancements](#future-enhancements)

## Overview

AgroFarms is a comprehensive digital marketplace designed to connect local farmers directly with consumers. The platform enables farmers to list their agricultural products while allowing buyers to browse, search, and purchase fresh produce directly from local growers. This direct farm-to-table approach aims to reduce the intermediary chain, ensuring fresher products for consumers and better profit margins for farmers.

The application is built using ASP.NET Core MVC with MongoDB as the database, providing a robust, scalable solution that can handle a wide range of agricultural products and user interactions.

## Features

### For Buyers

- **User Authentication**: Secure registration and login system
- **Product Browsing**: View all available products with filtering options by category, price, and organic status
- **Product Search**: Search functionality to find specific products
- **Detailed Product View**: View detailed information about products including descriptions, pricing, and farmer information
- **Shopping Cart**: Add products to cart, update quantities, and remove items
- **Checkout Process**: Complete purchases with delivery information
- **Order History**: View past orders and their statuses
- **User Dashboard**: Personalized dashboard showing order statistics and recent orders

### For Farmers

- **Seller Authentication**: Secure registration and login system for farmers
- **Product Management**: Add, edit, and remove products from their inventory
- **Inventory Control**: Manage product availability and stock quantities
- **Order Management**: View and process orders placed by buyers
- **Sales Analytics**: View sales statistics and customer patterns
- **Farmer Dashboard**: Personalized dashboard showing sales statistics and recent orders

### General Features

- **Responsive Design**: Mobile-friendly interface that works across devices
- **Session Management**: Persistent user sessions for a seamless experience
- **Error Handling**: Comprehensive error handling and user feedback

## Project Structure

The AgroFarms project follows a standard ASP.NET Core MVC structure with some specific organization for services and models:

```
AgroFarms/
├── Controllers/           # Contains all controller classes that handle HTTP requests
├── Data/                  # Database context and configuration
├── Models/                # Domain models representing the core business entities
│   └── ViewModels/        # Specialized models for views
├── Services/              # Business logic services that controllers depend on
├── Utilities/             # Helper classes and utilities
├── Views/                 # Razor views organized by controller
│   ├── Account/           # Authentication views
│   ├── Buyer/             # Buyer-specific views
│   ├── Farmer/            # Farmer-specific views
│   ├── Home/              # General views for home pages
│   └── Shared/            # Shared layouts and partial views
└── wwwroot/               # Static files (CSS, JavaScript, images)
    ├── css/               # Custom stylesheets
    ├── js/                # JavaScript files
    └── lib/               # Third-party libraries
```

## Technology Stack

### Backend

- **Framework**: ASP.NET Core MVC 9.0
- **Language**: C# 12
- **Database**: MongoDB 6.0
- **Authentication**: Session-based authentication
- **ORM**: MongoDB .NET Driver

### Frontend

- **View Engine**: Razor
- **CSS Framework**: Bootstrap 5
- **JavaScript Libraries**: jQuery, Bootstrap JS
- **Icons**: Font Awesome

### Development Tools

- **IDE**: Visual Studio 2022 or Visual Studio Code
- **Version Control**: Git
- **Package Management**: NuGet
- **Testing Framework**: xUnit (planned)

## Database Schema

AgroFarms uses MongoDB as its database, with the following collections:

### Users Collection

Contains both buyer and farmer users, differentiated by the `UserType` field.

**Buyer Schema:**

```
{
  "_id": ObjectId,
  "firstName": string,
  "lastName": string,
  "email": string,
  "passwordHash": string,
  "phoneNumber": string,
  "userType": "Buyer",
  "address": string,
  "city": string,
  "zipCode": string,
  "company": string,
  "createdAt": Date
}
```

**Farmer Schema:**

```
{
  "_id": ObjectId,
  "firstName": string,
  "lastName": string,
  "email": string,
  "passwordHash": string,
  "phoneNumber": string,
  "userType": "Farmer",
  "farmName": string,
  "farmLocation": string,
  "farmSize": number,
  "farmingMethods": string,
  "bio": string,
  "createdAt": Date
}
```

### Products Collection

```
{
  "_id": ObjectId,
  "name": string,
  "description": string,
  "price": decimal,
  "category": string,
  "unit": string,
  "farmerId": string,
  "farmerName": string,
  "imageUrl": string,
  "isAvailable": boolean,
  "createdAt": Date,
  "isOrganic": boolean,
  "stockQuantity": number
}
```

### Cart Collection

```
{
  "_id": ObjectId,
  "buyerId": string,
  "productId": string,
  "productName": string,
  "price": decimal,
  "quantity": number,
  "unit": string,
  "farmerId": string,
  "farmerName": string,
  "imageUrl": string,
  "addedAt": Date
}
```

### Orders Collection

```
{
  "_id": ObjectId,
  "buyerId": string,
  "buyerName": string,
  "items": [
    {
      "productId": string,
      "productName": string,
      "price": decimal,
      "quantity": number,
      "unit": string,
      "farmerId": string,
      "farmerName": string,
      "imageUrl": string
    }
  ],
  "totalAmount": decimal,
  "status": string, // "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
  "createdAt": Date,
  "deliveryAddress": string,
  "notes": string
}
```

## Authentication and Authorization

The application uses a session-based authentication system:

- **User Registration**:

  - Separate registration flows for buyers and farmers
  - Password hashing using BCrypt
  - Email validation

- **User Login**:

  - Authentication against stored credentials
  - Session initialization with user context
  - Role-based access using the `UserType` field

- **Session Management**:

  - User sessions maintained with 30-minute idle timeout
  - Session data includes user ID and user type
  - Session used for maintaining cart state

- **Authorization**:
  - Controller methods check user type before allowing access
  - `IsValidBuyer()` and `IsValidFarmer()` helper methods
  - Redirect to login for unauthenticated access attempts

## Services

The application follows a service-oriented architecture with the following service implementations:

### UserService

Handles user-related operations including:

- User registration for both buyers and farmers
- User authentication
- User profile management
- Password management

### ProductService

Manages all product-related functionality:

- Adding new products (for farmers)
- Updating product information
- Retrieving products with filtering
- Managing product availability

### StaticProductService

A specialized service for managing a static set of products:

- Provides consistent product data across the application
- Includes methods for filtering products by various criteria
- Handles product availability checks
- Used by EnhancedCartService to maintain data consistency

### CartService

Manages shopping cart operations:

- Adding items to cart
- Updating cart item quantities
- Removing items from cart
- Retrieving cart contents

### EnhancedCartService

An improved version of the cart service with additional features:

- Loading product data alongside cart items using StaticProductService
- Calculating cart totals
- Managing cart item counts
- Enhanced error handling
- Maintains data consistency with product operations

### OrderService

Handles order processing and management:

- Creating new orders from cart contents
- Retrieving order history for buyers
- Retrieving orders for farmers
- Updating order status

## Controllers

### HomeController

Handles general navigation and informational pages:

- Home page
- About page
- Privacy page
- Error handling

### AccountController

Manages user authentication and registration:

- User login
- Buyer registration
- Farmer registration
- Logout functionality

### BuyerController

Handles all buyer-specific functionality:

- Dashboard display with statistics
- Product browsing and searching
- Product details view using StaticProductService
- Cart management through EnhancedCartService
- Add to cart operations with product availability checking
- Checkout process
- Order history

### FarmerController

Manages farmer-specific functionality:

- Dashboard with sales statistics
- Product management (create, edit, delete)
- Order management
- Inventory control

### DiagnosticsController

Provides system diagnostic information:

- Connection status
- Database health checks
- Error logging

## Models

### Core Models

#### User

Base class for all users with common properties:

- ID, first name, last name, email
- Password hash, phone number
- Creation date and user type

#### Buyer

Extends User with buyer-specific properties:

- Shipping address
- City and zip code
- Company (optional)

#### Farmer

Extends User with farmer-specific properties:

- Farm name and location
- Farm size and farming methods
- Biographical information

#### Product

Represents agricultural products:

- Basic details (name, description, price)
- Categorization and units
- Farmer information
- Availability and stock tracking
- Organic status

#### CartItem

Represents items in a buyer's shopping cart:

- Product reference
- Quantity and pricing
- Farmer information
- Addition timestamp

#### Order

Represents completed purchases:

- Buyer information
- List of ordered items
- Total amount and status
- Delivery information and notes

### ViewModels

#### AuthViewModels

Specialized models for authentication operations:

- Login model with credentials
- Registration models for buyers and farmers

#### ProductViewModels

Models for product-related views:

- Product listings with pagination
- Product details display
- AddToCart model for cart operations

#### OrderViewModels

Models for order-related views:

- Order listings with filtering
- Order details with status tracking
- Checkout model for order placement

## Views

### Layout

- **\_Layout.cshtml**: Main layout template with navigation and footer
- **\_LoginPartial.cshtml**: Partial view for authentication UI
- **\_ValidationScriptsPartial.cshtml**: Validation scripts for forms

### Home Views

- **Index.cshtml**: Landing page with featured products
- **About.cshtml**: Information about the marketplace
- **Privacy.cshtml**: Privacy policy information

### Account Views

- **Login.cshtml**: User login form
- **RegisterBuyer.cshtml**: Buyer registration form
- **RegisterFarmer.cshtml**: Farmer registration form

### Buyer Views

- **Dashboard.cshtml**: Buyer's personalized dashboard
- **Products.cshtml**: Product listing with filters
- **ProductDetails.cshtml**: Detailed product view with add to cart
- **Cart.cshtml**: Shopping cart management
- **Checkout.cshtml**: Order placement form
- **Orders.cshtml**: Order history listing

### Farmer Views

- **Dashboard.cshtml**: Farmer's sales dashboard
- **Products.cshtml**: Product management listing
- **CreateProduct.cshtml**: Form for adding new products
- **EditProduct.cshtml**: Form for updating product details
- **Orders.cshtml**: Incoming order management

### Shared Views

- **Error.cshtml**: Error display page
- **OrderDetails.cshtml**: Shared order details view

## API Endpoints

### Authentication Endpoints

- `POST /Account/Login`: Authenticate user credentials
- `POST /Account/RegisterBuyer`: Register a new buyer account
- `POST /Account/RegisterFarmer`: Register a new farmer account
- `GET /Account/Logout`: Log out the current user

### Buyer Endpoints

- `GET /Buyer/Dashboard`: Buyer dashboard with statistics
- `GET /Buyer/Products`: Browse available products
- `GET /Buyer/ProductDetails/{id}`: View detailed product information
- `POST /Buyer/AddToCart`: Add a product to the cart
- `GET /Buyer/Cart`: View the current shopping cart
- `POST /Buyer/UpdateCartItem`: Update cart item quantity
- `POST /Buyer/RemoveFromCart`: Remove an item from the cart
- `GET /Buyer/Checkout`: Proceed to checkout
- `POST /Buyer/PlaceOrder`: Complete the purchase
- `GET /Buyer/Orders`: View order history
- `GET /Buyer/OrderDetails/{id}`: View specific order details

### Farmer Endpoints

- `GET /Farmer/Dashboard`: Farmer dashboard with sales statistics
- `GET /Farmer/Products`: Manage product inventory
- `GET /Farmer/CreateProduct`: Form to create a new product
- `POST /Farmer/CreateProduct`: Submit a new product
- `GET /Farmer/EditProduct/{id}`: Form to edit a product
- `POST /Farmer/EditProduct/{id}`: Update a product
- `POST /Farmer/DeleteProduct/{id}`: Remove a product
- `GET /Farmer/Orders`: View incoming orders
- `POST /Farmer/UpdateOrderStatus`: Change an order's status

### Ajax Endpoints

- `POST /Buyer/AddToCart`: Add to cart (JSON)
- `POST /Buyer/UpdateCartQuantity`: Update cart quantity (JSON)
- `GET /Buyer/GetCartCount`: Get the current cart count (JSON)
- `GET /Buyer/GetCartSummary`: Get cart totals (JSON)

## Styling and UI

The application uses a combination of Bootstrap 5 and custom CSS for styling:

### CSS Structure

- **site.css**: Global styles and overrides
- **form-fixes.css**: Styles for form elements
- **form-responsive.css**: Responsive adjustments for forms
- **login-fixes.css**: Styles specific to authentication pages
- **simple-forms.css**: Styles for simplified form layouts

### UI Components

- **Cards**: Used for product displays and dashboard widgets
- **Forms**: Structured forms with validation
- **Tables**: Used for order listings and cart display
- **Navigation**: Responsive navbar with dropdown menus
- **Modals**: Used for confirmations and quick actions
- **Alerts**: Feedback messages for user actions

### Icons

Font Awesome icons are used throughout the application for:

- Navigation elements
- Action buttons
- Status indicators
- Category identification

## Development Setup

### Prerequisites

- .NET 9.0 SDK
- MongoDB 6.0 or higher
- Visual Studio 2022 or Visual Studio Code
- Git (optional for version control)

### Installation Steps

1. **Clone the repository** (if using Git):

   ```
   git clone https://github.com/your-username/AgroFarms.git
   cd AgroFarms
   ```

2. **Restore dependencies**:

   ```
   dotnet restore
   ```

3. **Configure MongoDB**:

   - Install MongoDB locally or use a cloud instance
   - Update the connection string in `appsettings.json`:
     ```json
     "MongoDbSettings": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "AgroFarmsDB"
     }
     ```

4. **Build the project**:

   ```
   dotnet build
   ```

5. **Run the application**:

   ```
   dotnet run --project Farms
   ```

6. **Access the application**:
   Open a web browser and navigate to `https://localhost:7159` or `http://localhost:5159`

### Development Workflow

1. **Make changes to the codebase**
2. **Build and run the application**:
   ```
   dotnet build
   dotnet run --project Farms
   ```
3. **Test your changes**
4. **Important considerations**:
   - Maintain data source consistency between controllers and services
   - When working with product data, ensure StaticProductService is the single source of truth
   - Check for data inconsistencies when troubleshooting cart or order issues
5. **Commit changes** (if using Git):
   ```
   git add .
   git commit -m "Description of changes"
   ```

## Common Issues and Solutions

### Authentication Issues

- **Problem**: "User not authenticated" error when trying to access protected pages
- **Solution**: Ensure you're logged in and your session hasn't expired (30-minute timeout)

### Cart Functionality

- **Problem**: "An error occurred while adding to cart" message
- **Solution**:
  - Verify MongoDB connection is active
  - Check that the product ID is valid and product is in stock
  - Ensure you're logged in as a buyer
  - Confirm consistency in data sources between BuyerController and EnhancedCartService
  - Verify the StaticProductService is used consistently for product retrieval and availability checking

### Database Connection

- **Problem**: Application fails to start with MongoDB connection errors
- **Solution**:
  - Verify MongoDB service is running
  - Check connection string in `appsettings.json`
  - Ensure network allows connections to MongoDB port (27017 by default)

### Product Display Issues

- **Problem**: Products not showing images or missing details
- **Solution**:
  - Verify image URLs are accessible
  - Check that product data is complete
  - Clear browser cache if necessary

## Future Enhancements

### Recent Fixes

#### Add to Cart Functionality

- **Issue**: Users were encountering an error message "An error occurred while adding to cart" when trying to add products to their cart
- **Root Cause**: Data source inconsistency between BuyerController and EnhancedCartService
  - BuyerController was using local GetStaticProducts() (30 products)
  - EnhancedCartService was using StaticProductService (10 products)
  - This led to products existing in one data source but not in the other
- **Fix**:
  - Updated BuyerController to use StaticProductService consistently for both product details and cart operations
  - Modified AddToCart method to use IStaticProductService.GetProductById() instead of local GetStaticProducts()
  - Updated stock availability check to use StaticProductService.IsProductAvailable
  - Added detailed logging to trace the issue
  - Fixed ProductDetails.cshtml to include proper event parameter to addToCart() function call
  - Enhanced error handling in the JavaScript code

#### Build Error in temp_addtocart.cs

- **Issue**: Build error with "modifier 'public' is not valid for this item" in temp_addtocart.cs
- **Fix**: Properly structured the file as a valid class with necessary imports, class declaration, and helper methods

### Planned Features

- **Payment Integration**: Add support for online payments
- **Reviews and Ratings**: Allow buyers to rate products and farmers
- **Subscription Model**: Enable recurring deliveries for regular customers
- **Advanced Search**: Implement more sophisticated search capabilities
- **Mobile App**: Develop companion mobile applications
- **Analytics Dashboard**: More comprehensive analytics for farmers
- **Multi-language Support**: Localization for different languages
- **Notification System**: Email and in-app notifications

### Technical Improvements

- **Caching**: Implement Redis caching for improved performance
- **Unit Testing**: Add comprehensive test coverage
- **CI/CD Pipeline**: Set up automated build and deployment
- **Image Optimization**: Improve image handling and processing
- **API Documentation**: Generate Swagger documentation for APIs
- **Performance Monitoring**: Add APM tools for monitoring
- **Microservices Architecture**: Transition to microservices (long-term)

---

© 2025 AgroFarms. All rights reserved.
