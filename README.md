# UserService - Personal Expense Tracker

The **UserService** is a core component of the Personal Expense Tracker application, responsible for managing user accounts and authentication. It provides endpoints for user registration, email confirmation, and login.

## Features

- **User Registration**: 
  - Register new users with comprehensive personal details
  - Secure password hashing before database storage
  - Generate one-time email confirmation tokens

- **Email Confirmation**:
  - Automated email sending via SendGrid
  - Token-based account activation process

- **Secure Authentication**:
  - Login with username and password
  - Email verification required for account access

## API Endpoints

### 1. Register User
- **Method**: POST
- **Endpoint**: `/api/Users/register`
- **Request Body**:
  ```json
  {
    "username": "string",
    "password": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "phoneNumber": "string",
    "dateOfBirth": "date"
  }

- **Response**: 200 OK with registration confirmation
---
### 2. Confirm Email

**Method**: `GET`
**Endpoint**: `/api/Users/confirm-email`
**Query Parameters**:
- `token`: Email confirmation token
- `email`: User's email address
  
- **Response**: `200 OK` upon successful email confirmation
---
### 3. Login
- **Method**: `POST`
- **Endpoint**: `/api/Users/login`
- **Request Body**:
  ```json
  {
    "username": "string",
    "password": "string"
  }

- **Response**: 200 OK upon successful authentication
---
## Configuration
### Database Connection
Configure in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "AzureSqlConnection": "Server=<your-server>;Database=<your-database>;User ID=<your-username>;Password=<your-password>;"
  }
}
```

### Email Service Configuration
Configure in `appsettings.json`:

```json
{
  "SendGrid": {
    "ApiKey": "your_sendgrid_api_key"
  }
}
```