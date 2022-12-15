using EquationSolver.Interfaces;

namespace EquationSolver
{
    /// <summary>
    /// Class used for resolving an AST produced by the tokeniser
    /// </summary>
    public class EquationSolver
    {
        /// <summary>
        /// Solve the tokentree recursivly
        /// </summary>
        /// <param name="equation">An equation token</param>
        /// <returns>a double representing the solved equation</returns>
        public static double Solve(Token equation)
        {
            // If the token is a number just return it
            if (equation.Kind == TokenKind.Number)
                return double.Parse(equation.Value);

            // If the is another equation resolve it first
            if (equation.Tokens.Count == 1)
                return Solve(equation.Tokens[0]);

            // Get both sides of the equation
            var nr1 = Solve(equation.Tokens[0]);
            var nr2 = Solve(equation.Tokens[2]);

            // Apply the need operation
            switch (equation.Tokens[1].Kind)
            {
                case TokenKind.Addition:
                    return nr1 + nr2;

                case TokenKind.Subtraction:
                    return nr1 - nr2;

                case TokenKind.Multiplication:
                    return nr1 * nr2;

                case TokenKind.Division:
                    return nr1 / nr2;

                case TokenKind.Pow:
                    return Pow(nr1, nr2);
            }

            return 0;
        }

        /// <summary>
        /// Method used to determine the power of an number
        /// HINT: This method isn't complete. Please rather use Math.Pow if allowed!
        /// </summary>
        /// <param name="baseNr">base</param>
        /// <param name="exponent">exponent</param>
        /// <returns>the power of the base nr</returns>
        private static double Pow(double baseNr, double exponent)
        {
            var res = baseNr;

            for (var i = 1; i < exponent; i++)
                res *= baseNr;

            return res;
        }
    }
}
