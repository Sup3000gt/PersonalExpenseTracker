# UserService - Personal Expense Tracker

The **UserService** is a core component of the Personal Expense Tracker application, responsible for managing user accounts and authentication. It provides endpoints for user registration, email confirmation, and login. This microservice uses **Entity Framework Core** for database management and **SendGrid** for email notifications.

## Features

- **User Registration**: 
  - Register a new user with a username, password, email, and other personal details.
  - Hashes passwords securely before storing them in the database.
  - Generates a one-time email confirmation token for account activation.

- **Email Confirmation**:
  - Sends a confirmation email using SendGrid.
  - Validates the confirmation token and activates the user's account.

- **Login**:
  - Authenticates users using a username and password.
  - Ensures that only accounts with verified emails can log in.
