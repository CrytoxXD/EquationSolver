using System.Collections.Generic;

namespace EquationSolver.Interfaces
{
    /// <summary>
    /// Class used for saving information
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the value of the token
        /// The Value is a string representation of the symbol found
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the kind of the token.
        /// The kind represents what the symbol is (i.e a Number or a calculation operation or ...)
        /// </summary>
        public TokenKind Kind { get; set; }
        /// <summary>
        /// Gets or sets the underlying tokens
        /// </summary>
        public IList<Token> Tokens { get; set; }
    }

    /// <summary>
    /// Enum containing the different types of tokens
    /// </summary>
    public enum TokenKind
    {
        ParenOpen,          // Opening Parentheses ("(")
        ParenClose,         // Closing Parentheses (")")

        Division,           // Division Symbol ("/")
        Multiplication,     // Multiplication Symbol ("*")
        Subtraction,        // Subtraction Symbol ("-")
        Addition,           // Addition Symbol ("+")
        Pow,                // PowerOf Symbol ("^")

        Number,             // Numbers (i.e. 1, 5, -3, 3.5, ...)

        Equation            // Full Equations (i.e 3+5, ...)
    }
}
