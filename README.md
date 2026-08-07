# 🛒 E-Commerce Website

A simple e-commerce website built with **ASP.NET MVC** and **Microsoft SQL Server**, focused primarily on CRUD operations, API-based actions, and traditional server-side web development.

The project intentionally uses an **old-school 2010s-style website design**, giving it a nostalgic look and feel while implementing the core functionality of a basic e-commerce platform.

## 📌 About the Project

This project was created to practice working with **MVC architecture, API controllers, SQL queries, and database-driven CRUD operations**.

Most of the application's actions are handled through **API Controllers**, which communicate with the SQL Server database to perform operations such as creating, reading, updating, and deleting data.

In short, this project can be considered a **CRUD-based e-commerce application** with a traditional web interface.

## ✨ Features

* 🛍️ Browse products
* 🔎 View product details
* ➕ Add products
* ✏️ Update products
* 🗑️ Delete products
* 📦 Manage product information
* 👤 User-related functionality
* 🛒 Basic e-commerce functionality
* 🔌 API Controller-based operations
* 🗄️ Microsoft SQL Server database
* 🔄 CRUD operations using SQL queries
* 🖥️ Traditional 2010s-inspired UI

## 🏗️ Architecture

The application follows the **MVC (Model-View-Controller)** architecture.

```text
┌──────────────────────┐
│        Views         │
│   User Interface     │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│     Controllers      │
│   MVC + API          │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│       Models         │
│   Application Data   │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│    Microsoft SQL     │
│       Server         │
└──────────────────────┘
```

The **API Controllers** are responsible for handling many of the application's operations and interacting with the database through SQL queries.

## 🛠️ Technologies Used

* **C#**
* **ASP.NET MVC**
* **API Controllers**
* **Microsoft SQL Server**
* **SQL**
* **HTML / CSS**
* **JavaScript**

## 🗃️ Database

The project uses **Microsoft SQL Server** as its relational database.

The database is used to store and manage the application's e-commerce data, with SQL queries being used for the primary CRUD operations.

Typical operations include:

```text
CREATE  → Add new records
READ    → Retrieve records
UPDATE  → Modify existing records
DELETE  → Remove records
```

## 🎨 Design

One of the intentional aspects of this project is its visual style.

Instead of following modern minimalist e-commerce designs, the website uses an **old-school 2010s web aesthetic**. The goal was to recreate the look and feel of traditional e-commerce websites from that era.

## 🚀 Getting Started

### Prerequisites

Make sure you have the following installed:

* Visual Studio
* .NET / ASP.NET development environment
* Microsoft SQL Server
* SQL Server Management Studio (SSMS)

### Installation

1. Clone the repository:

```bash
git clone https://github.com/your-username/your-repository.git
```

2. Open the project in **Visual Studio**.

3. Configure the SQL Server connection string in the project's configuration.

4. Create/import the required database and tables using the provided SQL scripts.

5. Build the project.

6. Run the application through Visual Studio.

## 📚 Purpose of the Project

This project was mainly developed as a learning project to gain practical experience with:

* MVC architecture
* CRUD operations
* API Controllers
* SQL queries
* Relational databases
* Database-driven web applications
* Connecting an ASP.NET application to SQL Server
* Building a traditional e-commerce workflow

## ⚠️ Project Scope

This is a **simple/educational e-commerce project** rather than a production-ready online store.

The main focus is on understanding the relationship between the **MVC application, API Controllers, SQL queries, and the database**, while implementing common CRUD-based e-commerce functionality.

## 📄 License

This project is available for educational and personal use.
