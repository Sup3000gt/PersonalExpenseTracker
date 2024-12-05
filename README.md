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

## Project Structure



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
  
### 2. Register User
## Confirm Email

### Method
`GET`

### Endpoint
`/api/Users/confirm-email`

### Query Parameters
| Parameter | Type   | Description                    | Required |
|-----------|--------|--------------------------------|----------|
| `token`   | String | Email confirmation token       | Yes      |
| `email`   | String | User's email address           | Yes      |

### Response
- **200 OK**: Email confirmation is successful.

### Example Request
