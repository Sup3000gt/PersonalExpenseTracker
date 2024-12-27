
# User Service

## Overview
The **User Service** is a microservice within the Expense Tracker application that manages user-related functionalities such as registration, authentication, profile management, and email verification. It is implemented using ASP.NET Core and interacts with a SQL Server database.

---

## Features
- **User Registration:** Create new user accounts with email confirmation.
- **Login:** Authenticate users and generate JWT tokens.
- **Profile Management:** Retrieve and update user profiles.
- **Password Reset:** Generate temporary passwords and send via email.
- **Email Verification:** Confirm user emails with expiration tokens.

---

## API Endpoints

### Authentication & Registration
1. **Register User**
   - **Endpoint:** `POST /api/users/register`
   - **Description:** Registers a new user with validation and sends an email confirmation.
   - **Request Body:**
     ```json
     {
       "username": "string",
       "passwordHash": "string",
       "email": "string",
       "firstName": "string",
       "lastName": "string",
       "phoneNumber": "string",
       "dateOfBirth": "YYYY-MM-DD"
     }
     ```
   - **Response:** Success or error message.

2. **Login**
   - **Endpoint:** `POST /api/users/login`
   - **Description:** Authenticates the user and returns a JWT token.
   - **Request Body:**
     ```json
     {
       "username": "string",
       "password": "string"
     }
     ```
   - **Response:** JWT token and user ID.

### Email Verification
3. **Confirm Email**
   - **Endpoint:** `GET /api/users/confirm-email`
   - **Description:** Confirms the user's email using a token and email address.
   - **Query Parameters:**
     - `token`: Confirmation token.
     - `email`: User's email.

### Password Management
4. **Request Password Reset**
   - **Endpoint:** `POST /api/users/request-password-reset`
   - **Description:** Sends a temporary password to the user's email.
   - **Request Body:**
     ```json
     {
       "username": "string",
       "email": "string"
     }
     ```

5. **Change Password**
   - **Endpoint:** `POST /api/users/change-password`
   - **Description:** Changes the user's password.
   - **Request Body:**
     ```json
     {
       "username": "string",
       "newPassword": "string"
     }
     ```

### Profile Management
6. **Get User Profile**
   - **Endpoint:** `GET /api/users/profile`
   - **Description:** Retrieves user profile details.
   - **Query Parameter:** `username`

7. **Update User Profile**
   - **Endpoint:** `PUT /api/users/update-profile`
   - **Description:** Updates user profile details.
   - **Request Body:**
     ```json
     {
       "username": "string",
       "email": "string",
       "firstName": "string",
       "lastName": "string",
       "phoneNumber": "string",
       "dateOfBirth": "YYYY-MM-DD"
     }
     ```

---

## Technologies Used
- **ASP.NET Core**
- **Entity Framework Core**
- **SendGrid:** For sending email notifications.
- **JWT:** For authentication tokens.

---

## Database Schema

### User Table
| Field                  | Type         | Constraints          |
|------------------------|--------------|----------------------|
| Id                     | int          | Primary Key          |
| Username               | string       | Required, Unique     |
| PasswordHash           | string       | Required             |
| Email                  | string       | Required, Unique     |
| FirstName              | string       | Required             |
| LastName               | string       | Required             |
| PhoneNumber            | string       | Required             |
| DateOfBirth            | DateTime     | Optional             |
| EmailConfirmed         | bool         | Default: false       |
| EmailConfirmationToken | string       | Optional             |
| EmailConfirmationTokenExpires | DateTime | Optional         |
| PasswordResetToken     | string       | Optional             |
| PasswordResetTokenExpires | DateTime  | Optional             |

---

## Setup Instructions

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/ExpenseTracker
   cd ExpenseTracker/UserService
   ```

2. **Configure Environment:**
   Update `appsettings.json` with the following keys:
   - `ConnectionStrings:AzureSqlConnection`
   - `SendGrid:ApiKey`
   - `Jwt:Key`
   - `Jwt:Issuer`
   - `Jwt:Audience`

3. **Run Migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the Service:**
   ```bash
   dotnet run
   ```

---

## Contribution Guidelines
1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Description of changes"
   ```
4. Push to your branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request.

---

## License
This project is licensed under the MIT License. See the LICENSE file for details.
