
# Transaction Service

## Overview
The **Transaction Service** is a microservice within the Expense Tracker application that manages financial transactions, including income and expenses. It provides APIs for adding, updating, deleting, and retrieving transactions, and includes pagination and filtering capabilities.

---

## Features
- **Add Transactions:** Record new income or expense transactions.
- **Retrieve Transactions:** Fetch transactions with filtering, sorting, and pagination.
- **Update Transactions:** Modify details of an existing transaction.
- **Delete Transactions:** Remove transactions from the database.

---

## API Endpoints

### Transaction Management
1. **Add Transaction**
   - **Endpoint:** `POST /api/transactions/add`
   - **Description:** Adds a new transaction to the database.
   - **Request Body:**
     ```json
     {
       "userId": 1,
       "transactionType": "Income",
       "amount": 100.00,
       "date": "2024-12-01",
       "description": "Salary",
       "category": "Work"
     }
     ```
   - **Response:** Success or error message.

2. **Get Transactions for a User**
   - **Endpoint:** `GET /api/transactions/user/{userId}`
   - **Description:** Retrieves transactions for a user with optional filters.

   - **Query Parameters:**
     - `startDate` (optional): Filter transactions from this date.

     - `endDate` (optional): Filter transactions up to this date.
     - `category` (optional): Filter by category.
     - `page` (default: 1): Page number for pagination.
     - `pageSize` (default: 10): Number of transactions per page.
     - `sortBy` (default: "date"): Sort field (`date` or `amount`).
     - `sortOrder` (default: "desc"): Sort order (`asc` or `desc`).
   - **Response:** Paginated list of transactions with metadata.

3. **Get Transaction by ID**
   - **Endpoint:** `GET /api/transactions/{id}`
   - **Description:** Retrieves a single transaction by its ID.

4. **Update Transaction**
   - **Endpoint:** `PUT /api/transactions/{id}`
   - **Description:** Updates an existing transaction.
   - **Request Body:**
     ```json
     {
       "transactionType": "Expense",
       "amount": 50.00,
       "date": "2024-12-02",
       "description": "Groceries",
       "category": "Food"
     }
     ```
   - **Response:** Success or error message.

5. **Delete Transaction**
   - **Endpoint:** `DELETE /api/transactions/{id}`
   - **Description:** Deletes a transaction by its ID.

---

## Technologies Used
- **ASP.NET Core**
- **Entity Framework Core**
- **Swagger/OpenAPI:** For API documentation.

---

## Database Schema

### Transaction Table
| Field         | Type     | Constraints           |
|---------------|----------|-----------------------|
| Id            | int      | Primary Key           |
| UserId        | int      | Required, Foreign Key |
| TransactionType | string  | Required (`Income` or `Expense`) |
| Amount        | decimal  | Required, Positive    |
| Date          | DateTime | Required              |
| Description   | string   | Optional, MaxLength: 255 |
| Category      | string   | Optional, MaxLength: 50 |
| CreatedAt     | DateTime | Auto-set to UTC now   |
| UpdatedAt     | DateTime | Auto-set to UTC now   |

---

## Setup Instructions

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/ExpenseTracker
   cd ExpenseTracker/TransactionService
   ```

2. **Configure Environment:**
   Update `appsettings.json` with your database connection string.

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
