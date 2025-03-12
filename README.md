# GMOO.SDK

Official .NET SDK for the globalMOO API, providing a clean and intuitive interface for mathematical optimization operations.

[![NuGet version](https://img.shields.io/nuget/v/GMOO.SDK.svg)](https://www.nuget.org/packages/GMOO.SDK/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Prerequisites

- .NET 6.0 or later
- A globalMOO account with valid API credentials

## Getting Started

### 1. Create an Account
Create a new account on the [globalMOO API dashboard](https://app.globalmoo.com) to obtain your API credentials.

### 2. Install the SDK
Add the SDK to your project using NuGet:

```bash
dotnet add package GMOO.SDK
```

Or via the Package Manager Console:

```powershell
Install-Package GMOO.SDK
```

### 3. Configure Credentials
Set the `GMOO_API_KEY` and `GMOO_API_URI` environment variables or provide them directly when creating the client.

## Quick Start

<details>
  <summary><strong>Initialize the Client</strong></summary>

```csharp
using GMOO.SDK;

// Load credentials from environment variables or provide directly
string apiKey = Environment.GetEnvironmentVariable("GMOO_API_KEY");
string baseUri = Environment.GetEnvironmentVariable("GMOO_API_URI");

using var client = new Client(apiKey, baseUri);
Console.WriteLine("Successfully initialized globalMOO client");
```
</details>

<details>
  <summary><strong>Step 1: Create a Model</strong></summary>

```csharp
// Create a model to represent your optimization problem
var model = await client.CreateModelAsync(
    name: "Linear Function Example",
    description: "Basic example demonstrating optimization with globalMOO"
);
Console.WriteLine($"Created model with ID: {model.Id}");
```
</details>

<details>
  <summary><strong>Step 2: Create a Project</strong></summary>

```csharp
// Configure the project parameters including input constraints
var project = await client.CreateProjectAsync(
    modelId: model.Id,
    name: "README Example Project",
    inputCount: 2,
    minimums: new List<double> { 0.0, 0.0 },
    maximums: new List<double> { 10.0, 10.0 },
    inputTypes: new List<string> { "float", "float" },
    categories: new List<string>()
);
Console.WriteLine($"Created project with ID: {project.Id}");
```
</details>

<details>
  <summary><strong>Step 3: Get Input Cases</strong></summary>

```csharp
// Get the generated input cases for evaluation
var inputCases = project.InputCases;
Console.WriteLine($"Received {inputCases.Count} input cases");
```
</details>

<details>
  <summary><strong>Step 4: Compute Outputs</strong></summary>

```csharp
// Define the function that represents your system or process
/// <summary>
/// Simple 2-input, 3-output linear function for demonstration.
/// 
/// This function represents a "black box" system that we want to optimize.
/// In real applications, this could be a complex simulation, physical experiment,
/// or any process where we can control inputs and measure outputs.
/// </summary>
/// <param name="inputs">A list containing two float values [x, y]</param>
/// <returns>Three output values representing different linear combinations of inputs</returns>
private static List<double> LinearFunction(List<double> inputs)
{
    double x = inputs[0];
    double y = inputs[1];
    return new List<double>
    {
        x + y,          // Output 1: simple sum
        2 * x + y,      // Output 2: weighted sum
        x + 2 * y       // Output 3: different weighted sum
    };
}

// Compute outputs for each input case
var outputCases = new List<List<double>>();
foreach (var singleCase in inputCases)
{
    outputCases.Add(LinearFunction(singleCase));
}
Console.WriteLine($"Computed {outputCases.Count} output cases");
```
</details>

<details>
  <summary><strong>Step 5: Create a Trial</strong></summary>

```csharp
// Submit the output cases to create a trial
var trial = await client.LoadOutputCasesAsync(
    projectId: project.Id,
    outputCount: 3,
    outputCases: outputCases
);
Console.WriteLine($"Successfully created trial with ID: {trial.Id}");
```
</details>

<details>
  <summary><strong>Step 6: Set Optimization Objectives</strong></summary>

```csharp
// Define the optimization objectives
var targetValues = new List<double> { 2.0, 3.0, 3.0 };
var objectiveTypes = Enumerable.Repeat(ObjectiveType.Percent, 3).ToList();
var minimumBounds = new List<double> { -1.0, -1.0, -1.0 };
var maximumBounds = new List<double> { 1.0, 1.0, 1.0 };

// Initialize the inverse optimization with objectives
var objective = await client.LoadObjectivesAsync(
    trialId: trial.Id,
    objectives: targetValues,
    objectiveTypes: objectiveTypes,
    initialInput: inputCases[0],
    initialOutput: outputCases[0],
    minimumBounds: minimumBounds,
    maximumBounds: maximumBounds,
    desiredL1Norm: 0.0
);
Console.WriteLine("Initialized inverse optimization");
```
</details>

<details>
  <summary><strong>Step 7: Run Inverse Optimization</strong></summary>

```csharp
// Run the optimization iteration loop
const int maxIterations = 10;
List<double> nextOutput = null;
Inverse inverse = null;

for (int iteration = 0; iteration < maxIterations; iteration++)
{
    // Get a suggested input from the optimizer
    inverse = await client.SuggestInverseAsync(objectiveId: objective.Id);
    
    // Evaluate the suggested input with your function
    nextOutput = LinearFunction(inverse.Input);
    
    // Submit the results back to the optimizer
    inverse = await client.LoadInverseOutputAsync(inverseId: inverse.Id, output: nextOutput);

    // Check if optimization should stop
    if (inverse.ShouldStop())
    {
        Console.WriteLine("Optimization stopped");
        break;
    }
}
```
</details>

## Workflow Diagram

```
┌────────────┐     ┌────────────┐     ┌────────────┐     ┌────────────┐
│  Create    │     │  Create    │     │  Compute   │     │  Set       │
│  Model     ├────►│  Project   ├────►│  Outputs   ├────►│  Objectives│
└────────────┘     └────────────┘     └────────────┘     └────────────┘
                                                               │
                                                               ▼
                                                         ┌────────────┐
                                                         │  Run       │
                                                         │  Optimizer │
                                                         └────────────┘
```

## Complete Example

Below is a complete, runnable example that demonstrates the full workflow:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMOO.SDK;

namespace GMOOExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Initialize client
                string apiKey = Environment.GetEnvironmentVariable("GMOO_API_KEY");
                string baseUri = Environment.GetEnvironmentVariable("GMOO_API_URI");
                using var client = new Client(apiKey, baseUri);
                Console.WriteLine("Successfully initialized globalMOO client");

                // Create a model
                var model = await client.CreateModelAsync(
                    name: "Linear Function Example",
                    description: "Basic example demonstrating optimization with globalMOO"
                );
                Console.WriteLine($"Created model with ID: {model.Id}");

                // Create a project
                var project = await client.CreateProjectAsync(
                    modelId: model.Id,
                    name: "README Example Project",
                    inputCount: 2,
                    minimums: new List<double> { 0.0, 0.0 },
                    maximums: new List<double> { 10.0, 10.0 },
                    inputTypes: new List<string> { "float", "float" },
                    categories: new List<string>()
                );
                Console.WriteLine($"Created project with ID: {project.Id}");

                // Get input cases
                var inputCases = project.InputCases;
                Console.WriteLine($"Received {inputCases.Count} input cases");

                // Compute outputs
                var outputCases = new List<List<double>>();
                foreach (var singleCase in inputCases)
                {
                    outputCases.Add(LinearFunction(singleCase));
                }
                Console.WriteLine($"Computed {outputCases.Count} output cases");

                // Create a trial
                var trial = await client.LoadOutputCasesAsync(
                    projectId: project.Id,
                    outputCount: 3,
                    outputCases: outputCases
                );
                Console.WriteLine($"Successfully created trial with ID: {trial.Id}");

                // Set optimization objectives
                var targetValues = new List<double> { 2.0, 3.0, 3.0 };
                var objectiveTypes = Enumerable.Repeat(ObjectiveType.Percent, 3).ToList();
                var minimumBounds = new List<double> { -1.0, -1.0, -1.0 };
                var maximumBounds = new List<double> { 1.0, 1.0, 1.0 };

                var objective = await client.LoadObjectivesAsync(
                    trialId: trial.Id,
                    objectives: targetValues,
                    objectiveTypes: objectiveTypes,
                    initialInput: inputCases[0],
                    initialOutput: outputCases[0],
                    minimumBounds: minimumBounds,
                    maximumBounds: maximumBounds,
                    desiredL1Norm: 0.0
                );
                Console.WriteLine("Initialized inverse optimization");

                // Run inverse optimization
                const int maxIterations = 10;
                List<double> nextOutput = null;
                Inverse inverse = null;

                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    inverse = await client.SuggestInverseAsync(objectiveId: objective.Id);
                    nextOutput = LinearFunction(inverse.Input);
                    inverse = await client.LoadInverseOutputAsync(inverseId: inverse.Id, output: nextOutput);

                    Console.WriteLine($"Iteration {iteration + 1}: Input = [{string.Join(", ", inverse.Input)}], " +
                                      $"Output = [{string.Join(", ", nextOutput)}]");

                    if (inverse.ShouldStop())
                    {
                        Console.WriteLine("Optimization converged successfully");
                        break;
                    }
                }

                Console.WriteLine("Optimization complete!");
                Console.WriteLine($"Best input found: [{string.Join(", ", inverse.Input)}]");
                Console.WriteLine($"Corresponding output: [{string.Join(", ", nextOutput)}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Simple 2-input, 3-output linear function for demonstration.
        /// 
        /// This function represents a "black box" system that we want to optimize.
        /// In real applications, this could be a complex simulation, physical experiment,
        /// or any process where we can control inputs and measure outputs.
        /// </summary>
        /// <param name="inputs">A list containing two float values [x, y]</param>
        /// <returns>Three output values representing different linear combinations of inputs</returns>
        private static List<double> LinearFunction(List<double> inputs)
        {
            double x = inputs[0];
            double y = inputs[1];
            return new List<double>
            {
                x + y,          // Output 1: simple sum
                2 * x + y,      // Output 2: weighted sum
                x + 2 * y       // Output 3: different weighted sum
            };
        }
    }
}
```

## Features

- Full support for the globalMOO API
- Strong typing for all API operations
- Automatic serialization/deserialization
- Robust error handling
- Configurable retry policies
- Comprehensive documentation

## Environment Setup

Set environment variables for simpler credential management:

```
GMOO_API_KEY=your-api-key
GMOO_API_URI=https://app.globalmoo.com/api/
```

Then initialize the client:

```csharp
var client = new Client();
```

The SDK will automatically check for these environment variables if you don't provide them explicitly.

## Common Scenarios

### Scenario 1: Parameter Tuning
Use inverse optimization to find optimal parameter values for your system:

```csharp
// Configure objectives for parameter tuning
var targetValues = new List<double> { desiredOutput1, desiredOutput2, desiredOutput3 };
var objectiveTypes = new List<ObjectiveType> { 
    ObjectiveType.Minimize, 
    ObjectiveType.Target, 
    ObjectiveType.Maximize 
};
```

### Scenario 2: Simultaneous and Weight-Free Multi-objective Optimization
Balance multiple competing objectives without traditional weighting schemes:

```csharp
// Configure multiple objectives with different target types
var targetValues = new List<double> { 10.0, 5.0, 3.0 };
var objectiveTypes = new List<ObjectiveType> { 
    ObjectiveType.Maximize,
    ObjectiveType.Target, 
    ObjectiveType.Minimize 
};

// Note: No weights required - our algorithm automatically balances objectives
var objective = await client.LoadObjectivesAsync(
    trialId: trial.Id,
    objectives: targetValues,
    objectiveTypes: objectiveTypes,
    initialInput: inputCases[0],
    initialOutput: outputCases[0],
    // Additional constraints for the optimization process
    minimumBounds: minimumBounds,
    maximumBounds: maximumBounds
);
```

## Error Handling

The SDK provides specific exception types to help you diagnose issues:

```csharp
try
{
    var client = new Client();
    var model = await client.CreateModelAsync("Test Model");
}
catch (InvalidArgumentException ex)
{
    // Handle validation errors in your arguments
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (InvalidRequestException ex)
{
    // Handle API response errors
    Console.WriteLine($"API error: {ex.Message}");
    foreach (var error in ex.GetErrors())
    {
        Console.WriteLine($"  {error["property"]}: {error["message"]}");
    }
}
catch (AuthenticationException ex)
{
    // Handle authentication failures
    Console.WriteLine($"Authentication failed: {ex.Message}");
    Console.WriteLine("Check your API key and ensure it's still valid");
}
catch (RateLimitException ex)
{
    // Handle rate limiting
    Console.WriteLine($"Rate limit exceeded: {ex.Message}");
    Console.WriteLine($"Retry after: {ex.RetryAfter} seconds");
}
catch (Exception ex)
{
    // Catch-all for unexpected errors
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Version Compatibility

| SDK Version | globalMOO API Version | .NET Version |
|-------------|------------------------|--------------|
| 1.0.x       | v1                     | 6.0+         |

## Documentation

For detailed documentation, see:
- [API Reference](https://globalmoo.gitbook.io/api-reference/)
- [Usage Guides](https://globalmoo.gitbook.io/guides/)
- [Tutorials](https://globalmoo.gitbook.io/tutorials/)

## Support

- **Issues and Bugs**: Report issues on our [GitHub repository](https://github.com/globalMOO/gmoo-sdk-csharp)
- **Community Support**: Join our [Discord community](https://discord.gg/globalmoo)
- **Email Support**: Contact support@globalmoo.com for direct assistance

## More Examples

Additional examples can be found in the [gmoo-sdk-suite](https://github.com/globalMOO/gmoo-sdk-suite).

## License

This project is licensed under the [MIT License](LICENSE).

## Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.