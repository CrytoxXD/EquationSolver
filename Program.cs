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

            // Should equal to 490
            var equation = "-1(10+5^2)((5*-2)+9-3^3)/2";

            var equToken = Tokeniser.ResolveEquation(equation);
            Console.WriteLine(EquationSolver.Solve(equToken));
        }
    }
}
