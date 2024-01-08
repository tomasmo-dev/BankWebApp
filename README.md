# BankWebApp

This is a banking web application developed using C#, JavaScript, and SQL.

## Project Structure

The project is structured into several parts:

- `env/Envs.cs`: Contains the connection string for the MSSQL database.
- `database.sql`: Contains the SQL scripts for creating the necessary tables and indices in the database.

## Setup

1. Clone the repository.
2. Install the necessary dependencies.
3. Set up the MSSQL database using the provided SQL scripts.
4. Update the connection string in `env/Envs.cs` with your database details.
5. Run the application.

## Features

- User registration and login
- Bank account creation
- Transaction processing
- Transaction history
- Admin dashboard
- User dashboard

## Custom User Components

- `Navbar`: The navigation bar at the top of the page. made with View Component.
- `Footer`: The footer at the bottom of the page. made with Partial View.

## Documentation

The documentation for this project is available in the `docs` folder.
Files in the `docs` folder are generated using doxygen.

Available versions of the documentation:

- [PDF Documentation](./docs/documentation.pdf)&nbsp;&nbsp;&nbsp;&nbsp;(same as the rtf version)
- [RTF Documentation](./docs/rtf/refman.rtf)&nbsp;&nbsp;&nbsp;&nbsp;(same as the pdf version, may give warnings about being unsafe)
- [HTML Documentation](./docs/html/index.html)&nbsp;(best version, but requires a browser to view)
- Code Comments&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(in the source code)