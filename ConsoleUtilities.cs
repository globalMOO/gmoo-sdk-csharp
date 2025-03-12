using System;

namespace GMOO.SDK
{
    /// <summary>
    /// Utility methods for console output formatting.
    /// </summary>
    public static class ConsoleUtilities
    {
        private const string CheckMark = "+";  // Simple plus
        private const string XMark = "x";      // Simple x
        private const string InText = "in";    // Plain text

        /// <summary>
        /// Prints the satisfaction status of an objective with checkmark/x and detail.
        /// </summary>
        /// <param name="objectiveNum">The objective number.</param>
        /// <param name="satisfied">Whether the objective is satisfied.</param>
        /// <param name="detail">The detail message.</param>
        public static void PrintSatisfactionStatus(int objectiveNum, bool satisfied, string detail)
        {
            string symbol = satisfied ? $"({CheckMark})" : $"({XMark})";
            ConsoleColor color = satisfied ? ConsoleColor.Green : ConsoleColor.Red;

            Console.Write($"  Objective {objectiveNum}: ");

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ForegroundColor = originalColor;

            // Simple string replacement for the detail text
            string detailText = detail.Replace(" within ", " in ").Replace(" in ", $" {InText} ");
            Console.WriteLine($" - {detailText}");
        }

        /// <summary>
        /// Prints a section header with consistent styling.
        /// </summary>
        /// <param name="title">The section title.</param>
        public static void PrintSectionHeader(string title)
        {
            Console.WriteLine();

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(title);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a list of values with consistent formatting.
        /// </summary>
        /// <param name="label">The label for the values.</param>
        /// <param name="values">The values to print.</param>
        /// <param name="precision">The number of decimal places to show.</param>
        public static void PrintValues(string label, double[] values, int precision = 4)
        {
            string[] formattedValues = Array.ConvertAll(values, x => x.ToString($"F{precision}"));
            Console.WriteLine($"  {label}: [{string.Join(", ", formattedValues)}]");
        }

        /// <summary>
        /// Prints an info message in cyan.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void PrintInfo(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a success message in green.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void PrintSuccess(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints an error message in red.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void PrintError(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a warning message in yellow.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void PrintWarning(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }
}