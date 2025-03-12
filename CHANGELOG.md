# Changelog

All notable changes to the GMOO .NET SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-03-11

### Added
- Initial release of the GMOO .NET SDK
- Core `Client` implementation for API communication
- Support for model operations
  - Creating models
  - Retrieving models
- Project management functionality
  - Creating projects
  - Configuring input parameters with various types
- Trial and experiment workflow
  - Loading output cases
  - Setting optimization objectives
- Inverse optimization capabilities
  - Suggesting optimal inputs
  - Loading experimental outputs
- Support for various objective types
  - Exact
  - Percent
  - Value
  - Less than/greater than variants
  - Minimize/maximize
- Input type handling
  - Boolean
  - Categorical
  - Float
  - Integer
- Robust error handling with specific exception types
  - InvalidArgumentException
  - InvalidRequestException
  - NetworkConnectionException
- Automatic retry mechanisms with exponential backoff
- XML documentation for all public APIs
- Strong type safety throughout the library

### Notes
- Requires .NET 6.0 or higher
- Compatible with .NET 6.0, .NET 7.0, and .NET 8.0+ applications
- Designed for easy integration with ASP.NET Core and other .NET applications