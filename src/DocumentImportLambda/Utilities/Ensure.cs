using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DocumentImportLambda.Utilities
{
    [SuppressMessage("Style", "IDE0280:Use 'nameof'", Justification = "I'm pretty sure this breaks the functionality")]
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the specified value is greater than zero.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is greater than zero.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is less than or equal to zero.</exception>

        public static int GreaterThanZero(int value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{paramName} must be greater than zero.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified value is greater than zero.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is greater than zero.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is less than or equal to zero.</exception>

        public static long GreaterThanZero(long value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{paramName} must be greater than zero.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified DateTime value is not equal to DateTime.MinValue.
        /// </summary>
        /// <param name="value">The DateTime value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not equal to DateTime.MinValue.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is equal to DateTime.MinValue.</exception>

        public static DateTime NotDefault(DateTime value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentException($"{paramName} cannot be the default value (DateTime.MinValue).", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified integer value is not equal to zero.
        /// </summary>
        /// <param name="value">The integer value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not equal to zero.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is equal to zero.</exception>

        public static int NotDefault(int value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value == 0)
            {
                throw new ArgumentException($"{paramName} cannot be zero.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified Guid value is not equal to Guid.Empty.
        /// </summary>
        /// <param name="value">The Guid value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not equal to Guid.Empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is equal to Guid.Empty.</exception>

        public static Guid NotDefault(Guid value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{paramName} cannot be the default value (Guid.Empty).", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified long value is not equal to zero.
        /// </summary>
        /// <param name="value">The long value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not equal to zero.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is equal to zero.</exception>

        public static long NotDefault(long value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (value == 0L)
            {
                throw new ArgumentException($"{paramName} cannot be zero.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified reference type value is not null.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>

        public static T NotNull<T>(T? value, [CallerArgumentExpression("value")] string? paramName = null) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified nullable value type is not null.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>

        public static T NotNull<T>(T? value, [CallerArgumentExpression("value")] string? paramName = null) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
            }

            return value.Value;
        }

        /// <summary>
        /// Ensures that the specified array is not null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original array if it is not null or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the array is null or empty.</exception>
        public static T[] NotNullOrEmpty<T>(T[] array, [CallerArgumentExpression("array")] string? paramName = null)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
            }

            return array;
        }

        /// <summary>
        /// Ensures that the specified string is not null or empty.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original string if it is not null or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the string is null or empty.</exception>

        public static string NotNullOrEmpty(string? value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures that the specified string is not null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
        /// <returns>The original string if it is not null, empty, or consists only of white-space characters.</returns>
        /// <exception cref="ArgumentException">Thrown when the string is null, empty, or consists only of white-space characters.</exception>

        public static string NotNullOrWhiteSpace(string? value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} cannot be null or whitespace.", paramName);
            }

            return value;
        }
    }
}