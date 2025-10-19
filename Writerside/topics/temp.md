### Introducing Ploch.Common and Ploch.Data: .NET Libraries on GitHub

I'm excited to share my .NET libraries, **Ploch.Common** and **Ploch.Data**, both available on GitHub! These libraries are designed to enhance your development experience by providing utility classes, extension methods, and robust data handling capabilities.

#### Ploch.Common

**Repository:** [Ploch.Common](https://github.com/mrploch/ploch-common/)

Ploch.Common is a collection of utility classes and extension methods aimed at simplifying both common and uncommon tasks in .NET development. Whether you need handy helpers or more specialized functionality, Ploch.Common has you covered.

- **Key Features:**
  - A rich set of utility classes for various tasks.
  - Extension methods that enhance existing .NET types.
  - Comprehensive documentation and examples to help you get started quickly.

For more detailed insights, check out the README files:
- [General README](https://github.com/mrploch/ploch-common/blob/master/src/Common/README.md)
- [Collections README](https://github.com/mrploch/ploch-common/blob/master/src/Common/Collections/README.md)

You can also explore numerous examples through the unit tests available in the repository: [Unit Tests](https://github.com/mrploch/ploch-common/tree/master/src/Common.Tests).

#### Ploch.Data

**Repository:** [Ploch.Data](https://github.com/mrploch/ploch-data)

Ploch.Data is tailored for developers working with data in .NET. It provides a structured approach to data management through a common data model and a powerful generic repository.

- **Key Features:**
  - **Common Data Model:** Standardizes entities with interfaces for properties like `Id`, `Title`, `Children`, and `Parent`, making it ideal for representing categories or hierarchical structures in databases.
  - **Generic Repository:** This flagship feature includes battle-tested CRUD operations and supports the implementation of the Command Query Responsibility Segregation (CQRS) pattern through various repository types:
    - `IReadRepository` and `IReadRepositoryAsync` for read-only operations.
    - `IReadWriteRepository` and `IReadWriteRepositoryAsync` for both read and write operations.
  - **Unit of Work Pattern:** Facilitates managing transactions across multiple repositories.
  - **Integration Testing Library:** Simplifies writing integration tests with an SQLite in-memory database, supporting all types of queries, including complex scenarios.

For more details, visit the documentation sites:
- [Ploch.Data Documentation](https://github.ploch.dev/ploch-data/index.html)
- [Ploch.Data API Documentation](https://github.ploch.dev/ploch-data/api/index.html)

### Conclusion

Both Ploch.Common and Ploch.Data are designed to empower .NET developers by providing robust tools that streamline development and enhance code quality. I encourage you to explore these repositories, utilize the documentation, and contribute to the projects. Your feedback and contributions are always welcome!

Feel free to star the repositories if you find them helpful! Happy coding!