# SmartBarcodePOS

Modern Barcode Based Retail Billing & Inventory Management System built using ASP.NET Core MVC, Entity Framework Core and SQL Server.

SmartBarcodePOS is designed for retail shops, grocery stores, clothing stores, electronics stores and other businesses that require fast barcode billing, inventory tracking and sales management.

---

## Features

### Authentication & Security

- Admin Login
- Sub Admin Login
- Forgot Password
- Reset Password
- Change Password
- User Profile Management
- Role Based Authorization
- Secure Password Hashing

---

### Dashboard

- Total Products
- Total Sales
- Total Bills
- Today's Sales
- Low Stock Products
- Quick Statistics Cards

---

### Brand Management

- Create Brand
- Update Brand
- Delete Brand
- Activate / Deactivate Brand
- Duplicate Brand Validation

---

### Category Management

- Create Category
- Update Category
- Delete Category
- Brand-wise Categories
- Category Status Management

---

### Product Management

- Product Creation
- Product Editing
- Product Deletion
- Product Search
- Product SKU Management
- Product Barcode Generation
- Stock Management
- Low Stock Alerts

---

### Barcode Management

- Automatic Barcode Generation
- Barcode Preview
- Barcode Download
- Barcode Printing

---

### POS Billing

- Camera Barcode Scanner
- Mobile Barcode Scanner
- Continuous Barcode Scanning
- Automatic Cart Addition
- Quantity Adjustment
- Discount Calculation
- Bill Generation
- Receipt Printing
- Automatic Stock Deduction

---

### Customer Management

- Add Customer
- Edit Customer
- Customer Search
- Customer Listing

---

### Reports

- Sales Report
- Product Sales Report
- Top Selling Products
- Stock Report

---

### Settings

Business Settings:

- Business Name
- Address
- Contact Number
- GST Number
- Currency Symbol

Receipt Settings:

- Receipt Footer Message
- Print Configuration

SMTP Settings:

- Email Configuration
- Forgot Password Email Support

---

## User Roles

### Admin

Administrator has complete system access.

Permissions:

* Dashboard
* Brands
* Categories
* Products
* Customers
* Billing
* Reports
* Settings
* User Management
* Error Logs
* Profile Management

Responsibilities:

* Configure business settings
* Manage inventory
* Create users
* Monitor reports
* Manage system operations

---

### Sub Admin

Store Manager role.

Permissions:

* Dashboard
* Brands
* Categories
* Products
* Customers
* Billing
* Reports
* Profile Management

Restrictions:

* No User Management
* No Error Logs

Responsibilities:

* Manage products
* Manage inventory
* Manage daily operations
* View reports

---

### Cashier

Counter billing operator role.

Permissions:

* Cashier Portal
* Barcode Scanner
* Product Search
* POS Billing
* Bill Generation
* Receipt Printing

Restrictions:

* No Dashboard
* No Product Management
* No Category Management
* No Brand Management
* No Reports
* No Settings
* No User Management
* No Error Logs

Responsibilities:

* Scan products
* Generate bills
* Print receipts
* Process customer purchases

---

## Technology Stack

### Backend

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity

### Frontend    

- Bootstrap 5
- jQuery
- DataTables
- Font Awesome
- Toastr Notifications

---

## Installation

### Clone Repository

```bash
git clone https://github.com/harshitsonani/SmartBarcodePOS.git
```

### Configure Database

Update:

```json
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SmartBarcodePOS;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

### Configure SMTP

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

---

### Apply Database Migration

```powershell
Update-Database
```

---

### Run Project

```powershell
dotnet run
```

---


---

## Business Workflow

Admin Creates Brand

↓

Admin Creates Category

↓

Admin Creates Product

↓

System Generates Barcode

↓

Barcode Printed

↓

Admin Creates Cashier

↓

Cashier Logs In

↓

Cashier Scans Products

↓

Products Automatically Added To Cart

↓

Bill Generated

↓

Receipt Printed

↓

Stock Reduced Automatically

↓

Reports Updated

↓

Admin Reviews Sales Reports

---

## License

This project is licensed under the MIT License.