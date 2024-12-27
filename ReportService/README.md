
# Report Service

## Overview
The **Report Service** is a microservice within the Expense Tracker application that generates various reports based on user transactions. It supports monthly summaries, category breakdowns, and custom date range reports.

---

## Features
- **Monthly Summary:** Provides a summary of income and expenses for a specific month.
- **Category Breakdown:** Analyzes spending/income distribution by category for a specific month.
- **Custom Date Range Reports:** Retrieves transactions within a custom date range.

---

## API Endpoints

### Report Generation
1. **Monthly Summary**
   - **Endpoint:** `GET /api/report/monthly-summary`
   - **Description:** Generates a summary of transactions for a given user, year, and month.

   - **Query Parameters:**
     - `userId`: User's ID.
     - `year`: Year of the report.
     - `month`: Month of the report.
   - **Response:** Total amounts grouped by transaction type.

2. **Category Breakdown**
   - **Endpoint:** `GET /api/report/category-breakdown`
   - **Description:** Retrieves total amounts grouped by category for a user, year, and month.
   - **Query Parameters:**
     - `userId`: User's ID.
     - `year`: Year of the report.
     - `month`: Month of the report.
   - **Response:** Total amounts grouped by category.

3. **Custom Date Range Report**
   - **Endpoint:** `GET /api/report/custom-date-range`
   - **Description:** Fetches transactions for a user within a specified date range.
   - **Query Parameters:**
     - `userId`: User's ID.
     - `startDate`: Start date for the report.
     - `endDate`: End date for the report.
   - **Response:** List of transactions within the date range.

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
   cd ExpenseTracker/ReportService
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
