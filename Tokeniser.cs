using System.Collections.Generic;
using System.IO;
using System;
using EquationSolver.Interfaces;
using System.Linq;

namespace EquationSolver
{
    /// <summary>
    /// Class used for creating a AST (abstract syntax tree) from an string
    /// Uses the shunting yard algorithm for parsing
    /// </summary>
    public class Tokeniser
    {
        /// <summary>
        /// Resolve an given equation and return it as an AST
        /// </summary>
        /// <param name="equation">String representation of an equation</param>
        /// <returns>An AST</returns>
        public static Token ResolveEquation(string equation)
        {
            return ResolveOperationPrecendences(ResolveParens(ParseTokens(equation.Replace(" ", ""))));
        }

        /// <summary>
        /// Converts the entered equation into an linear list containing each part of the equation
        /// </summary>
        /// <param name="equation">A string containing the equation</param>
        /// <returns>A linear list resembling each part of the equation</returns>
        /// <exception cref="InvalidOperationException">Occures if an unsupported character is detected</exception>
        private static IList<Token> ParseTokens(string equation)
        {
            var res = new List<Token>();

            var reader = new StringReader(equation);
            var peeked = reader.Peek();
            var lastToken = new Token();

            // Read the entire string as long we don't receive "-1" which means that no character is left in the string
            while (peeked != -1)
            {
                // Check which character which just peeked
                switch (peeked)
                {
                    case '(':

                        // Handle implicit multiplication. i.e something like this "2(1+2)" gets read as "2*(1+2)"
                        if (res.Count > 0)
                        {
                            lastToken = res.Last();
                            if (lastToken.Kind == TokenKind.Number || lastToken.Kind == TokenKind.ParenClose)
                                res.Add(new Token { Value = "*", Kind = TokenKind.Multiplication });
                        }

                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.ParenOpen });
                        break;

                    case ')':
                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.ParenClose });
                        break;

                    case '/':
                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.Division });
                        break;

                    case '*':
                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.Multiplication });
                        break;

                    case '-':
                        if(res.Count > 0)
                            lastToken = res.Last();

                        // Handle negative numbers. 
                        if (lastToken.Kind != TokenKind.Number && lastToken.Kind != TokenKind.ParenClose)
                        {
                            reader.Read();
                            res.Add(new Token { Value = $"-{ParseNumber(reader)}", Kind = TokenKind.Number });
                        }
                        else
                            res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.Subtraction });
                        break;

                    case '+':
                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.Addition });
                        break;

                    case '^':
                        res.Add(new Token { Value = ((char)reader.Read()).ToString(), Kind = TokenKind.Pow });
                        break;
                    
                    // Looking here for either decimal point symbol just ignore different culture input
                    case ',':
                    case '.':
                        reader.Read();
                        var numberBefore = res.Last().Value;
                        var numberAfter = ParseNumber(reader);
                        res.RemoveAt(res.Count - 1);

                        res.Add(new Token { Value = $"{numberBefore},{numberAfter}", Kind = TokenKind.Number });
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        res.Add(new Token { Value = ParseNumber(reader), Kind = TokenKind.Number });
                        break;

                    // If we didn't handle it, throw an exception
                    default:
                        throw new InvalidOperationException($"Unknown character {(char)peeked}");
                }

                // Update the peeked symbol for each iteration
                peeked = reader.Peek();
            }

            return res;
        }

        /// <summary>
        /// Add read numbers to the returning string to parse bigger numbers
        /// </summary>
        /// <param name="reader">The current string reader object</param>
        /// <returns>A string representation of a number</returns>
        private static string ParseNumber(StringReader reader)
        {
            var nr = string.Empty;

            while (reader.Peek() - '0' >= 0 && reader.Peek() - '0' <= 9)
                nr += (char)reader.Read();

            return nr;
        }

        /// <summary>
        /// Resolve all parentheses inside the token list
        /// </summary>
        /// <param name="tokens">A parsed list of tokens</param>
        /// <returns>A list of tokens with resolved parentheses</returns>
        private static IList<Token> ResolveParens(IList<Token> tokens)
        {
            // Convert parens to equations
            var res = new List<Token>();

            for (var i = 0; i < tokens.Count;)
            {
                if (tokens[i].Kind == TokenKind.ParenOpen)
                {
                    res.Add(ResolveParen(tokens, i, out var l));
                    i += l;
                }
                else
                {
                    res.Add(tokens[i]);
                    i++;
                }
            }

            return res;
        }

        /// <summary>
        /// Resolve one parentheses
        /// </summary>
        /// <param name="tokens">tokens to be resolved</param>
        /// <param name="index">current index</param>
        /// <param name="length">outputed length of the equation</param>
        /// <returns></returns>
        private static Token ResolveParen(IList<Token> tokens, int index, out int length)
        {
            length = 2;
            var res = new Token { Kind = TokenKind.Equation, Tokens = new List<Token>() };

            for (var i = index; i < tokens.Count;)
            {
                if (i == index)
                {
                    i++;

                    continue;
                }

                if (tokens[i].Kind == TokenKind.ParenOpen)
                {
                    res.Tokens.Add(ResolveParen(tokens, i, out var l));
                    i += l;

                    continue;
                }

                if (tokens[i].Kind == TokenKind.ParenClose)
                {
                    length = i - index + 1;

                    break;
                }

                res.Tokens.Add(tokens[i]);
                i++;
            }

            return res;
        }

        // 2-dimensional array declaring the order of operation
        private static string[][] _precendence = {
            new[] { "^" },
            new[] {"*","/"},
            new[] {"+","-"}
        };

        /// <summary>
        /// Resolve the operation order
        /// </summary>
        /// <param name="tokens">A list of tokens</param>
        /// <returns>A single token that contains the right order of operation of tokens</returns>
        private static Token ResolveOperationPrecendences(IList<Token> tokens)
        {
            // Resolve most upper layer of equations
            var res = new Token { Kind = TokenKind.Equation, Tokens = tokens };
            ResolveOperationPrecendence(res);

            return res;
        }

        /// <summary>
        /// Resolve the order of operation
        /// </summary>
        /// <param name="equation">An equation token</param>
        private static void ResolveOperationPrecendence(Token equation)
        {
            foreach (var equ in equation.Tokens.Where(x => x.Kind == TokenKind.Equation))
                ResolveOperationPrecendence(equ);

            // If equation contains only up to 3 tokens, then we only have one operation (single number, or single mathematical operation)
            // Doesn't need to be resolved further
            if (equation.Tokens.Count <= 3)
                return;

            foreach (var precedence in _precendence)
            {
                for (var i = 1; i < equation.Tokens.Count; i += 2)
                {
                    if (precedence.Contains(equation.Tokens[i].Value))
                    {
                        var equTokens = equation.Tokens.Skip(i - 1).Take(3).ToList();

                        equation.Tokens.RemoveAt(i - 1);
                        equation.Tokens.RemoveAt(i - 1);
                        equation.Tokens.RemoveAt(i - 1);

                        equation.Tokens.Insert(i - 1, new Token { Kind = TokenKind.Equation, Tokens = equTokens });

                        i -= 2;
                    }
                }
            }
        }
    }
}
