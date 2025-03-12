using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GMOO.SDK.ResponseTypes;
using GMOO.SDK.Enums;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GMOO.SDK
{
    /// <summary>
    /// Client for interacting with the GMOO API.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly bool _disposeHttpClient;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="baseUri">The base URI of the API.</param>
        /// <param name="httpClient">Optional HTTP client to use. If not provided, a new one will be created.</param>
        /// <param name="validateTls">Whether to validate TLS certificates.</param>
        public Client(
            string apiKey = null,
            string baseUri = null,
            HttpClient httpClient = null,
            bool validateTls = true)
        {
            // Try to get values from parameters first, then fall back to environment variables
            string finalApiKey = apiKey ?? Environment.GetEnvironmentVariable("GMOO_API_KEY");
            string finalBaseUri = baseUri ?? Environment.GetEnvironmentVariable("GMOO_API_URI");

            // Validate API key
            if (string.IsNullOrEmpty(finalApiKey))
                throw new ArgumentException(
                    "API key cannot be empty. Please provide it as a parameter or set the GMOO_API_KEY environment variable.",
                    nameof(apiKey));

            // Validate base URI
            if (string.IsNullOrEmpty(finalBaseUri))
                throw new ArgumentException(
                    "Base URI cannot be empty. Please provide it as a parameter or set the GMOO_API_URI environment variable.",
                    nameof(baseUri));

            // Validate URI format
            if (!Uri.TryCreate(finalBaseUri, UriKind.Absolute, out var uri))
                throw new ArgumentException("Base URI must be a valid absolute URI.", nameof(baseUri));

            // Ensure TLS validation for official domains
            if (!validateTls && uri.Host.ToLowerInvariant().Contains("globalmoo.com"))
                throw new ArgumentException(
                    "TLS validation must be enabled when using the official GMOO domain.",
                    nameof(validateTls));

            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            // Create or configure HttpClient
            if (httpClient == null)
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = validateTls ? null : HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = uri,
                    Timeout = TimeSpan.FromSeconds(30)
                };
                _disposeHttpClient = true;
            }
            else
            {
                _httpClient = httpClient;
                _disposeHttpClient = false;
            }

            // Set default headers
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", finalApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Model Operations

        /// <summary>
        /// Gets all models.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A collection of models.</returns>
        public async Task<IEnumerable<Model>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync<IEnumerable<Model>>("models", cancellationToken);
        }

        /// <summary>
        /// Creates a new model.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="description">Optional model description.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created model.</returns>
        public async Task<Model> CreateModelAsync(
            string name,
            string description = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Model name cannot be empty.", nameof(name));

            var request = new
            {
                name,
                description
            };

            return await PostAsync<Model>("models", request, cancellationToken);
        }

        #endregion

        #region Project Operations

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="modelId">ID of the model to create the project under.</param>
        /// <param name="name">Project name (must be at least 4 characters).</param>
        /// <param name="inputCount">Number of input variables.</param>
        /// <param name="minimums">Minimum values for each input.</param>
        /// <param name="maximums">Maximum values for each input.</param>
        /// <param name="inputTypes">Types for each input variable.</param>
        /// <param name="categories">Categories for categorical inputs.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created project.</returns>
        public async Task<Project> CreateProjectAsync(
            int modelId,
            string name,
            int inputCount,
            List<double> minimums,
            List<double> maximums,
            List<string> inputTypes,
            List<string> categories = null,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (modelId <= 0)
                throw new ArgumentOutOfRangeException(nameof(modelId), "Model ID must be greater than zero.");

            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 4)
                throw new ArgumentException("Project name must be at least 4 characters long.", nameof(name));

            if (inputCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Input count must be greater than zero.");

            if (minimums == null)
                throw new ArgumentNullException(nameof(minimums));

            if (maximums == null)
                throw new ArgumentNullException(nameof(maximums));

            if (inputTypes == null)
                throw new ArgumentNullException(nameof(inputTypes));

            if (minimums.Count != inputCount)
                throw new ArgumentException($"Length of minimums list ({minimums.Count}) does not match input count ({inputCount}).", nameof(minimums));

            if (maximums.Count != inputCount)
                throw new ArgumentException($"Length of maximums list ({maximums.Count}) does not match input count ({inputCount}).", nameof(maximums));

            if (inputTypes.Count != inputCount)
                throw new ArgumentException($"Length of input types list ({inputTypes.Count}) does not match input count ({inputCount}).", nameof(inputTypes));

            // Validate input types
            foreach (var inputType in inputTypes)
            {
                if (!IsValidInputType(inputType))
                {
                    throw new ArgumentException(
                        $"Invalid input type: {inputType}. Valid types are: boolean, category, float, integer",
                        nameof(inputTypes));
                }
            }

            // Categories validation
            var categoriesList = categories ?? new List<string>();
            foreach (var category in categoriesList)
            {
                if (string.IsNullOrEmpty(category))
                    throw new ArgumentException("Categories cannot contain null or empty strings.", nameof(categories));
            }

            var request = new
            {
                name,
                inputCount,
                minimums,
                maximums,
                inputTypes,
                categories = categoriesList
            };

            return await PostAsync<Project>($"models/{modelId}/projects", request, cancellationToken);
        }

        /// <summary>
        /// Loads output cases for a project.
        /// </summary>
        /// <param name="projectId">Project ID.</param>
        /// <param name="outputCount">Number of output variables.</param>
        /// <param name="outputCases">List of output cases.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created trial.</returns>
        public async Task<Trial> LoadOutputCasesAsync(
            int projectId,
            int outputCount,
            List<List<double>> outputCases,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (projectId <= 0)
                throw new ArgumentOutOfRangeException(nameof(projectId), "Project ID must be greater than zero.");

            if (outputCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(outputCount), "Output count must be greater than zero.");

            if (outputCases == null)
                throw new ArgumentNullException(nameof(outputCases));

            foreach (var outputCase in outputCases)
            {
                if (outputCase == null)
                    throw new ArgumentException("Output cases cannot contain null entries.", nameof(outputCases));

                if (outputCase.Count != outputCount)
                    throw new ArgumentException(
                        $"All output cases must have length {outputCount}.", nameof(outputCases));
            }

            var request = new
            {
                outputCount,
                outputCases
            };

            return await PostAsync<Trial>($"projects/{projectId}/output-cases", request, cancellationToken);
        }

        #endregion

        #region Trial Operations

        /// <summary>
        /// Gets a specific trial.
        /// </summary>
        /// <param name="trialId">The trial ID to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The requested trial.</returns>
        public async Task<Trial> GetTrialAsync(int trialId, CancellationToken cancellationToken = default)
        {
            if (trialId <= 0)
                throw new ArgumentOutOfRangeException(nameof(trialId), "Trial ID must be greater than zero.");

            return await GetAsync<Trial>($"trials/{trialId}", cancellationToken);
        }

        /// <summary>
        /// Loads objectives for a trial.
        /// </summary>
        /// <param name="trialId">Trial ID.</param>
        /// <param name="objectives">Objective values.</param>
        /// <param name="objectiveTypes">Objective types.</param>
        /// <param name="initialInput">Initial input values.</param>
        /// <param name="initialOutput">Initial output values.</param>
        /// <param name="desiredL1Norm">Desired L1 norm (default: 0.0).</param>
        /// <param name="minimumBounds">Minimum bounds for objectives.</param>
        /// <param name="maximumBounds">Maximum bounds for objectives.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created objective.</returns>
        public async Task<Objective> LoadObjectivesAsync(
            int trialId,
            List<double> objectives,
            List<ObjectiveType> objectiveTypes,
            List<double> initialInput,
            List<double> initialOutput,
            double? desiredL1Norm = null,
            List<double> minimumBounds = null,
            List<double> maximumBounds = null,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (trialId <= 0)
                throw new ArgumentOutOfRangeException(nameof(trialId), "Trial ID must be greater than zero.");

            if (objectives == null)
                throw new ArgumentNullException(nameof(objectives));

            if (objectiveTypes == null)
                throw new ArgumentNullException(nameof(objectiveTypes));

            if (initialInput == null)
                throw new ArgumentNullException(nameof(initialInput));

            if (initialOutput == null)
                throw new ArgumentNullException(nameof(initialOutput));

            if (objectives.Count != objectiveTypes.Count)
                throw new ArgumentException("Number of objectives must match number of objective types.");

            // Convert objective types to strings
            var objectiveTypeStrings = new List<string>();
            foreach (var objectiveType in objectiveTypes)
            {
                objectiveTypeStrings.Add(ConvertObjectiveTypeToString(objectiveType));
            }

            // Set default L1 norm if not provided
            var l1Norm = desiredL1Norm ?? 0.0;

            // Set default bounds if needed
            List<double> minBounds = minimumBounds;
            List<double> maxBounds = maximumBounds;

            if (objectiveTypes.Count > 0 && objectiveTypes[0] == ObjectiveType.Exact)
            {
                minBounds ??= new List<double>(new double[objectives.Count]);
                maxBounds ??= new List<double>(new double[objectives.Count]);
            }

            var request = new
            {
                desiredL1Norm = l1Norm,
                objectives,
                objectiveTypes = objectiveTypeStrings,
                initialInput,
                initialOutput,
                minimumBounds = minBounds,
                maximumBounds = maxBounds
            };

            return await PostAsync<Objective>($"trials/{trialId}/objectives", request, cancellationToken);
        }

        #endregion

        #region Inverse Operations

        /// <summary>
        /// Suggests the next inverse optimization step.
        /// </summary>
        /// <param name="objectiveId">Objective ID.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The suggested inverse.</returns>
        public async Task<Inverse> SuggestInverseAsync(int objectiveId, CancellationToken cancellationToken = default)
        {
            if (objectiveId <= 0)
                throw new ArgumentOutOfRangeException(nameof(objectiveId), "Objective ID must be greater than zero.");

            return await PostAsync<Inverse>($"objectives/{objectiveId}/suggest-inverse", new { }, cancellationToken);
        }

        /// <summary>
        /// Loads output for an inverse optimization step.
        /// </summary>
        /// <param name="inverseId">Inverse ID.</param>
        /// <param name="output">Output values.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The updated inverse.</returns>
        public async Task<Inverse> LoadInverseOutputAsync(
            int inverseId,
            List<double> output,
            CancellationToken cancellationToken = default)
        {
            if (inverseId <= 0)
                throw new ArgumentOutOfRangeException(nameof(inverseId), "Inverse ID must be greater than zero.");

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (output.Count == 0)
                throw new ArgumentException("Output list cannot be empty.", nameof(output));

            var request = new
            {
                output
            };

            return await PostAsync<Inverse>($"inverses/{inverseId}/load-output", request, cancellationToken);
        }

        #endregion

        #region Account Operations

        /// <summary>
        /// Registers a new account.
        /// </summary>
        /// <param name="company">Company name.</param>
        /// <param name="name">User's name.</param>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <param name="timeZone">User's time zone.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created account.</returns>
        public async Task<Account> RegisterAccountAsync(
            string company,
            string name,
            string email,
            string password,
            string timeZone,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("Company name cannot be empty.", nameof(company));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            if (string.IsNullOrWhiteSpace(timeZone))
                throw new ArgumentException("Time zone cannot be empty.", nameof(timeZone));

            var request = new
            {
                company,
                name,
                email,
                password,
                timeZone
            };

            return await PostAsync<Account>("accounts/register", request, cancellationToken);
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Parses a webhook event payload from the GMOO API.
        /// </summary>
        /// <param name="payload">JSON payload from the webhook.</param>
        /// <returns>The parsed event.</returns>
        public Event ParseWebhookEvent(string payload)
        {
            try
            {
                // Parse the JSON to verify basic structure
                var eventData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payload, _jsonOptions);

                if (!eventData.ContainsKey("id") || !eventData.ContainsKey("name"))
                {
                    throw new ArgumentException("The payload does not appear to be a valid event.", nameof(payload));
                }

                if (eventData["name"].ValueKind != JsonValueKind.String)
                {
                    throw new ArgumentException("The 'name' property must be a string.", nameof(payload));
                }

                // Deserialize the full event
                return JsonSerializer.Deserialize<Event>(payload, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Failed to parse webhook payload as valid JSON.", nameof(payload), ex);
            }
        }

        #endregion

        #region Helper Methods

        private async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken);
        }

        private async Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<T>(HttpMethod.Post, endpoint, data, cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object data, CancellationToken cancellationToken)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            Exception lastError = null;

            while (true)
            {
                try
                {
                    return await ExecuteRequestAsync<T>(method, endpoint, data, cancellationToken);
                }
                catch (HttpRequestException ex) when (ShouldRetry(ex))
                {
                    lastError = ex;
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        throw new HttpRequestException("Maximum number of retries reached.", lastError);
                    }

                    int waitTime = Math.Min(4 * (int)Math.Pow(2, retryCount - 1), 10) * 1000; // Exponential backoff
                    await Task.Delay(waitTime, cancellationToken);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private async Task<T> ExecuteRequestAsync<T>(HttpMethod method, string endpoint, object data, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, endpoint);

            if (data != null && method != HttpMethod.Get)
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        private bool ShouldRetry(HttpRequestException ex)
        {
            // Retry on server errors and rate limiting
            return !ex.StatusCode.HasValue ||
                   (int)ex.StatusCode.Value >= 500 ||
                   (int)ex.StatusCode.Value == 429;
        }

        private bool IsValidInputType(string inputType)
        {
            var validTypes = new[] { "boolean", "category", "float", "integer" };
            return !string.IsNullOrEmpty(inputType) &&
                   Array.IndexOf(validTypes, inputType.ToLowerInvariant()) >= 0;
        }

        private string ConvertObjectiveTypeToString(ObjectiveType type)
        {
            return type switch
            {
                ObjectiveType.Exact => "exact",
                ObjectiveType.Percent => "percent",
                ObjectiveType.Value => "value",
                ObjectiveType.LessThan => "lessthan",
                ObjectiveType.LessThanEqual => "lessthan_equal",
                ObjectiveType.GreaterThan => "greaterthan",
                ObjectiveType.GreaterThanEqual => "greaterthan_equal",
                ObjectiveType.Minimize => "minimize",
                ObjectiveType.Maximize => "maximize",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown objective type.")
            };
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes resources used by the client.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources used by the client.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _disposeHttpClient)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}