
# Expense Tracker Application

## Overview
The **Expense Tracker Application** is a full-stack solution designed to help users manage their personal finances effectively. The system is built using a microservices architecture, comprising three main services:
- **User Service**: Handles user authentication, registration, and profile management.
- **Transaction Service**: Manages financial transactions such as income and expenses.
- **Report Service**: Generates detailed financial reports.

---

## Microservices

### 1. User Service
**Purpose**: Manages user-related functionalities, including authentication, registration, email verification, and profile updates.

- **Key Features**:
  - User registration with email confirmation.
  - Login with JWT-based authentication.
  - Profile retrieval and updates.
  - Password reset with email notifications.

- **Key Endpoints**:
  - `POST /api/users/register` - Register a new user.
  - `POST /api/users/login` - Authenticate user and generate JWT token.
  - `GET /api/users/profile` - Retrieve user profile details.
  - `PUT /api/users/update-profile` - Update user profile.

For more details, refer to the [User Service README](UserService/README.md).

### 2. Transaction Service
**Purpose**: Handles all transaction-related operations, such as recording, updating, deleting, and retrieving financial transactions.

- **Key Features**:
  - Add, update, and delete transactions.
  - Retrieve transactions with pagination and filters.
  - Categorize transactions as income or expense.

- **Key Endpoints**:
  - `POST /api/transactions/add` - Add a new transaction.
  - `GET /api/transactions/user/{userId}` - Retrieve transactions for a user with filters.
  - `PUT /api/transactions/{id}` - Update a transaction.
  - `DELETE /api/transactions/{id}` - Delete a transaction.

For more details, refer to the [Transaction Service README](TransactionService/README.md).

### 3. Report Service
**Purpose**: Provides financial insights through various reports, such as monthly summaries and category breakdowns.

- **Key Features**:
  - Generate monthly summaries for income and expenses.
  - Analyze transactions by category.
  - Custom date range reports.

- **Key Endpoints**:
  - `GET /api/report/monthly-summary` - Monthly transaction summary.
  - `GET /api/report/category-breakdown` - Category-wise transaction breakdown.
  - `GET /api/report/custom-date-range` - Transactions within a custom date range.

For more details, refer to the [Report Service README](ReportService/README.md).

---

## Technologies Used
- **Backend Framework:** ASP.NET Core
- **Database:** SQL Server
- **Authentication:** JWT (JSON Web Tokens)
- **Email Service:** SendGrid
- **API Documentation:** Swagger/OpenAPI

---

## Setup Instructions

### Prerequisites
- .NET SDK
- SQL Server
- SendGrid API Key

### Steps
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/ExpenseTracker
   cd ExpenseTracker
   ```

2. **Configure Environment for Each Service:**
   - Update the `appsettings.json` file in each service with appropriate database connection strings and API keys.

3. **Run Migrations:**
   Navigate to each service directory and apply database migrations:
   ```bash
   dotnet ef database update
   ```

4. **Start Services:**
   Run each service independently using the following command in their respective directories:
   ```bash
   dotnet run
   ```

5. **Access Swagger Documentation:**
   Open `http://localhost:{port}/swagger` for each service to explore available APIs.

---

## Project Structure
```
ExpenseTracker/
├── UserService/
│   ├── README.md
│   ├── Controllers/
│   └── ...
├── TransactionService/
│   ├── README.md
│   ├── Controllers/
│   └── ...
├── ReportService/
│   ├── README.md
│   ├── Controllers/
│   └── ...
└── README.md (This file)
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

---

## Future Enhancements
- Integration with budgeting tools.
- Enhanced analytics and visualization dashboards.
- Mobile application support.
