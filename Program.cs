using EquationSolver.Interfaces;
using System;

namespace EquationSolver
{
    class Program
    {
        static void Main()
        {
            /*
            Useful links if wanting to understand how exactly the algorithm works (15.12.2022 20:41):
                - https://en.wikipedia.org/wiki/Shunting_yard_algorithm
                - https://en.wikipedia.org/wiki/Abstract_syntax_tree
            */

            string[] equations = new[]
            {
                "-1(10+5^2)((5*-2)+9-3^3)/2",       // Should result to 490
                "-1(10+5.555)((5*-2)+9-3*2.3)/0",   // Should result in zero-division error
                "-1(10+5^2)((5*-2)+9-0^0)/2"        // Should result in zero to zero power error
            };

            ResolveEquTokens(CreateEquTokens(equations));
        }

        static Token[] CreateEquTokens(string[] equations)
        {
            var res = new Token[equations.Length];

            for (var i = 0; i < equations.Length; i++)
                res[i] = Tokeniser.ResolveEquation(equations[i]);

            return res;
        }

        static void ResolveEquTokens(Token[] equTokens)
        {
            for(var i = 0; i < equTokens.Length; i++)
            {
                try
                {
                    Console.WriteLine($"Resolved index {i} of equTokens with result: {EquationSolver.Solve(equTokens[i])}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"While solving index {i} of equTokens an error was encountered: {ex.Message}");
                }
            }
        }
    }
}
