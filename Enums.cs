namespace GMOO.SDK.Enums
{
    /// <summary>
    /// Specifies the type of objective for optimization.
    /// </summary>
    public enum ObjectiveType
    {
        /// <summary>
        /// Exact match objective.
        /// </summary>
        Exact,

        /// <summary>
        /// Percentage-based objective.
        /// </summary>
        Percent,

        /// <summary>
        /// Value-based objective.
        /// </summary>
        Value,

        /// <summary>
        /// Less than objective.
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal objective.
        /// </summary>
        LessThanEqual,

        /// <summary>
        /// Greater than objective.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal objective.
        /// </summary>
        GreaterThanEqual,

        /// <summary>
        /// Minimize objective.
        /// </summary>
        Minimize,

        /// <summary>
        /// Maximize objective.
        /// </summary>
        Maximize
    }

    /// <summary>
    /// Specifies the type of input parameter for optimization.
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// Boolean input type (true/false).
        /// </summary>
        Boolean,

        /// <summary>
        /// Categorical input type (one of predefined values).
        /// </summary>
        Category,

        /// <summary>
        /// Floating-point input type.
        /// </summary>
        Float,

        /// <summary>
        /// Integer input type.
        /// </summary>
        Integer
    }

    /// <summary>
    /// Specifies the type of webhook event from the GMOO API.
    /// </summary>
    public enum EventName
    {
        /// <summary>
        /// Event triggered when a project is created.
        /// </summary>
        ProjectCreated,

        /// <summary>
        /// Event triggered when an inverse is suggested.
        /// </summary>
        InverseSuggested
    }

    /// <summary>
    /// Specifies the reason why a trial stopped.
    /// </summary>
    public enum StopReason
    {
        /// <summary>
        /// Trial is still running or being evaluated.
        /// </summary>
        Running = 0,

        /// <summary>
        /// Trial satisfied to an optimal input and output.
        /// </summary>
        Satisfied = 1,

        /// <summary>
        /// Trial stopped due to duplicate suggested inputs.
        /// </summary>
        Stopped = 2,

        /// <summary>
        /// Trial exhausted all attempts to converge.
        /// </summary>
        Exhausted = 3
    }
}