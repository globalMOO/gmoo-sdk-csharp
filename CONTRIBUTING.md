# Contributing to GMOO.SDK

Thank you for your interest in contributing to the GMOO .NET SDK! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

By participating in this project, you agree to uphold our Code of Conduct, which expects all contributors to:

- Be respectful and inclusive of differing viewpoints and experiences
- Give and gracefully accept constructive feedback
- Focus on what is best for the community and users
- Show empathy towards other community members

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider
- Git

### Setting Up for Development

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```
   git clone https://github.com/YOUR-USERNAME/gmoo-sdk-csharp.git
   ```
3. Add the original repository as an upstream remote:
   ```
   git remote add upstream https://github.com/globalMOO/gmoo-sdk-csharp.git
   ```
4. Set up your development environment:
   ```
   cd gmoo-sdk-csharp
   dotnet restore
   ```

## Making Changes

1. Create a new branch for your feature or bugfix:
   ```
   git checkout -b feature/your-feature-name
   ```
   or
   ```
   git checkout -b fix/issue-number
   ```

2. Make your changes, adhering to the coding conventions below
3. Add tests that verify your changes
4. Run the existing tests to ensure you haven't broken anything:
   ```
   dotnet test
   ```
5. Update documentation as needed

## Coding Conventions

- Follow the [.NET Coding Style Guide](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md)
- Use nullable reference types and make proper use of null-checking
- Use XML documentation comments for all public APIs
- Maintain existing code formatting (4 spaces for indentation)
- Maintain high code quality with StyleCop and/or FxCop analyzers
- Keep methods focused and small when possible
- Prioritize readability over clever code

## Pull Request Process

1. Update your branch with the latest changes from upstream:
   ```
   git pull upstream main
   ```
2. Resolve any merge conflicts
3. Push your branch to your fork:
   ```
   git push origin feature/your-feature-name
   ```
4. Submit a pull request to the `main` branch of the original repository
5. Ensure the PR description clearly describes the problem and solution, referencing any relevant issues
6. Wait for maintainers to review your PR and address any feedback

## Testing Guidelines

- Write unit tests for all new functionality
- Ensure all tests pass before submitting PRs
- Mock external dependencies in tests
- Test edge cases and error scenarios

## Documentation Guidelines

- Update the README.md if your changes impact the installation or usage
- Add XML documentation comments to all public APIs
- Consider updating the sample code if relevant to your changes
- If you add new features, ensure they're documented

## Version Management

- We follow [Semantic Versioning](https://semver.org/)
- Update the CHANGELOG.md file for significant changes

## Release Process

The maintainers will handle the release process which typically includes:

1. Finalizing the CHANGELOG.md
2. Updating version numbers
3. Building and publishing the NuGet package
4. Creating GitHub releases

## License

By contributing to this project, you agree that your contributions will be licensed under the project's [MIT License](LICENSE).

## Questions?

If you have questions about contributing, please:

- Open an issue on GitHub
- Join our Discord community (link in README)
- Contact us at support@globalmoo.com

Thank you for your contributions!
