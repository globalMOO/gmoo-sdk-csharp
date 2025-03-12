using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using GMOO.SDK.Enums;

namespace GMOO.SDK.ResponseTypes
{
    /// <summary>
    /// Base class for all GMOO API ResponseTypes.
    /// </summary>
    public class GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ResponseType.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update timestamp.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the disabled timestamp if the ResponseType has been disabled.
        /// </summary>
        [JsonPropertyName("disabledAt")]
        public DateTime? DisabledAt { get; set; }
    }

    /// <summary>
    /// Represents a user account in the GMOO API.
    /// </summary>
    public class Account : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [JsonPropertyName("company")]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the customer ID.
        /// </summary>
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }
    }

    /// <summary>
    /// Represents an error response from the GMOO API.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the error title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the detailed validation errors, if any.
        /// </summary>
        [JsonPropertyName("errors")]
        public List<Dictionary<string, string>> Errors { get; set; } = new List<Dictionary<string, string>>();
    }

    /// <summary>
    /// Represents an event notification from the GMOO API.
    /// </summary>
    public class Event : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the event subject, if any.
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; }
    }

    /// <summary>
    /// Represents a GMOO ML ResponseType namespace.
    /// </summary>
    public class Model : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the Model name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Model description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the projects under this Model.
        /// </summary>
        [JsonPropertyName("projects")]
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    /// <summary>
    /// Represents a project in the GMOO API.
    /// </summary>
    public class Project : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets when the project was developed.
        /// </summary>
        [JsonPropertyName("developedAt")]
        public DateTime? DevelopedAt { get; set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of input variables.
        /// </summary>
        [JsonPropertyName("inputCount")]
        public int InputCount { get; set; }

        /// <summary>
        /// Gets or sets the minimum values for each input.
        /// </summary>
        [JsonPropertyName("minimums")]
        public List<double> Minimums { get; set; }

        /// <summary>
        /// Gets or sets the maximum values for each input.
        /// </summary>
        [JsonPropertyName("maximums")]
        public List<double> Maximums { get; set; }

        /// <summary>
        /// Gets or sets the types for each input variable.
        /// </summary>
        [JsonPropertyName("inputTypes")]
        public List<string> InputTypes { get; set; }

        /// <summary>
        /// Gets or sets the categories for categorical inputs.
        /// </summary>
        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the input test cases.
        /// </summary>
        [JsonPropertyName("inputCases")]
        public List<List<double>> InputCases { get; set; }

        /// <summary>
        /// Gets or sets the number of test cases.
        /// </summary>
        [JsonPropertyName("caseCount")]
        public int CaseCount { get; set; }

        /// <summary>
        /// Gets or sets the trials for this project.
        /// </summary>
        [JsonPropertyName("trials")]
        public List<Trial> Trials { get; set; } = new List<Trial>();
    }

    /// <summary>
    /// Represents a trial in the GMOO API.
    /// </summary>
    public class Trial : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the trial number.
        /// </summary>
        [JsonPropertyName("number")]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the number of output variables.
        /// </summary>
        [JsonPropertyName("outputCount")]
        public int OutputCount { get; set; }

        /// <summary>
        /// Gets or sets the output test cases.
        /// </summary>
        [JsonPropertyName("outputCases")]
        public List<List<double>> OutputCases { get; set; }

        /// <summary>
        /// Gets or sets the number of test cases.
        /// </summary>
        [JsonPropertyName("caseCount")]
        public int CaseCount { get; set; }

        /// <summary>
        /// Gets or sets the objectives for this trial.
        /// </summary>
        [JsonPropertyName("objectives")]
        public List<Objective> Objectives { get; set; } = new List<Objective>();
    }

    /// <summary>
    /// Represents an optimization objective.
    /// </summary>
    public class Objective : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the optimal inverse, if found.
        /// </summary>
        [JsonPropertyName("optimalInverse")]
        public Inverse OptimalInverse { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts made.
        /// </summary>
        [JsonPropertyName("attemptCount")]
        public int AttemptCount { get; set; }

        /// <summary>
        /// Gets or sets the reason the optimization stopped.
        /// </summary>
        [JsonPropertyName("stopReason")]
        public int StopReason { get; set; }

        /// <summary>
        /// Gets or sets the desired L1 norm.
        /// </summary>
        [JsonPropertyName("desiredL1Norm")]
        public double DesiredL1Norm { get; set; }

        /// <summary>
        /// Gets or sets the objective values.
        /// </summary>
        [JsonPropertyName("objectives")]
        public List<double> Objectives { get; set; }

        /// <summary>
        /// Gets or sets the objective types.
        /// </summary>
        [JsonPropertyName("objectiveTypes")]
        public List<string> ObjectiveTypes { get; set; }

        /// <summary>
        /// Gets or sets the minimum bounds for each objective.
        /// </summary>
        [JsonPropertyName("minimumBounds")]
        public List<double> MinimumBounds { get; set; }

        /// <summary>
        /// Gets or sets the maximum bounds for each objective.
        /// </summary>
        [JsonPropertyName("maximumBounds")]
        public List<double> MaximumBounds { get; set; }

        /// <summary>
        /// Gets or sets the inverses for this objective.
        /// </summary>
        [JsonPropertyName("inverses")]
        public List<Inverse> Inverses { get; set; } = new List<Inverse>();

        /// <summary>
        /// Gets the total number of iterations.
        /// </summary>
        public int IterationCount => Inverses.Count;

        /// <summary>
        /// Gets the last inverse if any exist.
        /// </summary>
        public Inverse LastInverse => Inverses.Count > 0 ? Inverses[Inverses.Count - 1] : null;
    }

    /// <summary>
    /// Represents an inverse optimization step.
    /// </summary>
    public class Inverse : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets when the inverse was loaded.
        /// </summary>
        [JsonPropertyName("loadedAt")]
        public DateTime? LoadedAt { get; set; }

        /// <summary>
        /// Gets or sets when the objective was satisfied.
        /// </summary>
        [JsonPropertyName("satisfiedAt")]
        public DateTime? SatisfiedAt { get; set; }

        /// <summary>
        /// Gets or sets when the optimization was stopped.
        /// </summary>
        [JsonPropertyName("stoppedAt")]
        public DateTime? StoppedAt { get; set; }

        /// <summary>
        /// Gets or sets when the optimization was exhausted.
        /// </summary>
        [JsonPropertyName("exhaustedAt")]
        public DateTime? ExhaustedAt { get; set; }

        /// <summary>
        /// Gets or sets the iteration number.
        /// </summary>
        [JsonPropertyName("iteration")]
        public int Iteration { get; set; }

        /// <summary>
        /// Gets or sets the L1 norm value.
        /// </summary>
        [JsonPropertyName("l1Norm")]
        public double L1Norm { get; set; }

        /// <summary>
        /// Gets or sets the time in nanoseconds for the suggestion.
        /// </summary>
        [JsonPropertyName("suggestTime")]
        public long SuggestTime { get; set; }

        /// <summary>
        /// Gets or sets the time in nanoseconds for the computation.
        /// </summary>
        [JsonPropertyName("computeTime")]
        public long ComputeTime { get; set; }

        /// <summary>
        /// Gets or sets the input values.
        /// </summary>
        [JsonPropertyName("input")]
        public List<double> Input { get; set; }

        /// <summary>
        /// Gets or sets the output values.
        /// </summary>
        [JsonPropertyName("output")]
        public List<double> Output { get; set; }

        /// <summary>
        /// Gets or sets the error values.
        /// </summary>
        [JsonPropertyName("errors")]
        public List<double> Errors { get; set; }

        /// <summary>
        /// Gets or sets the results for this inverse.
        /// </summary>
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }

        /// <summary>
        /// Gets the stop reason based on timestamps.
        /// </summary>
        public StopReason GetStopReason()
        {
            if (SatisfiedAt != null)
                return StopReason.Satisfied;
            else if (StoppedAt != null)
                return StopReason.Stopped;
            else if (ExhaustedAt != null)
                return StopReason.Exhausted;
            else
                return StopReason.Running;
        }

        /// <summary>
        /// Determines if the optimization should stop.
        /// </summary>
        public bool ShouldStop()
        {
            return GetStopReason() != StopReason.Running;
        }
    }

    /// <summary>
    /// Represents a result from an inverse optimization step.
    /// </summary>
    public class Result : GMOOBaseResponseType
    {
        /// <summary>
        /// Gets or sets the result number.
        /// </summary>
        [JsonPropertyName("number")]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the objective value.
        /// </summary>
        [JsonPropertyName("objective")]
        public double Objective { get; set; }

        /// <summary>
        /// Gets or sets the objective type.
        /// </summary>
        [JsonPropertyName("objectiveType")]
        public string ObjectiveType { get; set; }

        /// <summary>
        /// Gets or sets the minimum bound.
        /// </summary>
        [JsonPropertyName("minimumBound")]
        public double MinimumBound { get; set; }

        /// <summary>
        /// Gets or sets the maximum bound.
        /// </summary>
        [JsonPropertyName("maximumBound")]
        public double MaximumBound { get; set; }

        /// <summary>
        /// Gets or sets the output value.
        /// </summary>
        [JsonPropertyName("output")]
        public double Output { get; set; }

        /// <summary>
        /// Gets or sets the error value.
        /// </summary>
        [JsonPropertyName("error")]
        public double Error { get; set; }

        /// <summary>
        /// Gets or sets the detail message, if any.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the objective was satisfied.
        /// </summary>
        [JsonPropertyName("satisfied")]
        public bool Satisfied { get; set; } = true;
    }
}