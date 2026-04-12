# .NET Hexagonal Architecture Templates

This repository contains a collection of high-quality, production-ready templates for building applications using **Hexagonal Architecture** (also known as Ports and Adapters) in .NET.

The goal of this project is to provide developers with different architectural "flavors" ranging from simple repository patterns to complex Domain-Driven Design (DDD) implementations with multi-tenancy support.

## 🚀 Key Features

  - **Clean Separation of Concerns:** Strict decoupling between Domain, Application, and Infrastructure layers.
  - **Independence from Frameworks:** The core business logic is not dependent on external libraries or databases.
  - **Testability:** Built with a design that facilitates Unit, Integration, and Functional testing.
  - **Ready for Production:** Includes standard configurations for Logging, Exception Handling, and Dependency Injection.

## 📂 Repository Structure

Each template is contained within its own independent solution. This allows you to copy only the folder you need without carrying unnecessary dependencies.

```text
/templates
├── Template-DDD-MultiTenant-Isolated
```

## 🛠️ How to Use

### Option 1: Manual Copy

1.  Navigate to the `templates` folder.
2.  Choose the architecture that best fits your needs.
3.  Copy the folder to your local environment.
4.  Perform a global "Find and Replace" for the project namespace to match your project name.

### Option 2: Clone the Repo

```bash
git clone https://github.com/[your-username]/[your-repo-name].git
```

## 📋 Prerequisites

  - [.NET SDK](https://dotnet.microsoft.com/download) (Current LTS)
  - [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)
  - Your preferred database engine (SQL Server, PostgreSQL, etc.)

## 🤝 Contributing

Contributions are welcome\! If you find a bug or want to suggest a new architectural pattern, please open an issue or submit a pull request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://www.google.com/search?q=LICENSE) file for details.
