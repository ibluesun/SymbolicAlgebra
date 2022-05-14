﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public static class TextTools
    {
        /// <summary>
        /// Split the text based on coma separator on the first level not in inner parenthesis
        /// so that u,v,(8,3,f) => u  and  v   and  (8,3,f)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string[] ComaSplit(string expression)
        {
            return Split(expression, ',');
        }


        /// <summary>
        /// split the text with splitter character and ignore between parenthesis
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static string[] Split(string expression, char splitter)
        {
            int Split = 0;
            List<string> all = new List<string>();

            int i = 0;

            StringBuilder SplitedText = new StringBuilder();

            while (i < expression.Length)
            {
                if (expression[i] == '(') Split++;
                if (expression[i] == ')') Split--;

                if (Split == 0 & expression[i] == splitter)
                {
                    all.Add(SplitedText.ToString());
                    SplitedText = new StringBuilder();
                }
                else
                {
                    // add charachter
                    if (!char.IsWhiteSpace(expression[i])) SplitedText.Append(expression[i]);
                }
                i++;
            }

            all.Add(SplitedText.ToString());

            return all.ToArray();
        }

        /// <summary>
        /// Finds explicit character in the string top level that is not in parenthesis 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool TopLevelExist(string expression, char character)
        {
            int i = 0;
            int Split = 0;

            while (i < expression.Length)
            {
                if (expression[i] == '(') Split++;
                if (expression[i] == ')') Split--;

                if (Split == 0 & expression[i] == character)
                {
                    return true;
                }
                
                i++;
            }

            return false;
        }


        /// <summary>
        /// Extracts words that begin with letter from expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string[] WordsFromExpression(string expression)
        {
            char[] separators = { '^', '*', '/', '+', '-', '(', '|', '.', '<', '>', '=' };
            char[] seps = { '^', '*', '/', '+', '-', '|', '.' };
            expression = expression.Replace(" ", "");

            List<string> results = new List<string>();

            StringBuilder TokenBuilder = new StringBuilder();
            Stack<int> PLevels = new Stack<int>();
            bool Inner = false;
            bool FunctionContext = false;

            for (int ix = 0; ix < expression.Length; ix++)
            {
                if (PLevels.Count == 0)
                {
                    // include the normal parsing when we are not in parenthesis group
                    if (separators.Contains(expression[ix]))
                    {
                        if ((expression[ix] == '-' || expression[ix] == '+') && ix == 0)
                        {
                            //TokenBuilder.Append(expression[ix]);
                            // ignore the leading sign .. in this function we want the words only
                        }
                        else if (expression[ix] == '(')
                        {
                            PLevels.Push(1);
                            var bb = ix > 0 ? separators.Contains(expression[ix - 1]) : true;
                            if (!bb)
                            {
                                //the previous charachter is normal word which indicates we reached a function
                                FunctionContext = true;
                                TokenBuilder.Append(expression[ix]);
                            }
                        }
                        else if (seps.Contains(expression[ix - 1]) && (expression[ix] == '-' || expression[ix] == '+'))
                        {
                            TokenBuilder.Append(expression[ix]);
                        }
                        else
                        {
                            // tokenize   when we reach any operator  or open '(' parenthesis 
                            if (Inner)
                            {
                                results.AddRange(WordsFromExpression(TokenBuilder.ToString()));
                                Inner = false;
                            }
                            else
                            {
                                results.Add(TokenBuilder.ToString());
                            }

                            TokenBuilder = new StringBuilder();
                        }
                    }
                    else
                    {
                        TokenBuilder.Append(expression[ix]);
                    }
                }
                else
                {
                    // we are in group
                    if (expression[ix] == '(')
                    {
                        PLevels.Push(1);
                    }
                    if (expression[ix] == ')')
                    {
                        PLevels.Pop();

                        if (PLevels.Count == 0)
                        {
                            Inner = true;
                            if (FunctionContext)
                            {
                                TokenBuilder.Append(expression[ix]);
                                FunctionContext = false;
                                Inner = false;   // because i am taking the function body as a whole in this parse pass.
                                // then inner parameters of the function will be parsed again 
                            }
                        }
                        else
                        {
                            TokenBuilder.Append(expression[ix]);
                        }
                    }
                    else
                    {
                        TokenBuilder.Append(expression[ix]);
                    }
                }
            }

            // Last pass that escaped from the loop.
            if (Inner)
            {
                results.AddRange(WordsFromExpression(TokenBuilder.ToString()));
                Inner = false;
            }
            else
            {
                results.Add(TokenBuilder.ToString());

                
            }
            TokenBuilder = null;



            var ff = from z in results
                     where string.IsNullOrEmpty(z)==false && char.IsDigit(z, 0) == false
                     select z;

            return ff.ToArray();
        }


        public static string[] ExtractFunctionParameters(string expression)
        {
            var firstpass = ComaSplit(expression);
            
            List<string> final = new List<string>(firstpass.Length);

            foreach (var fp in firstpass) final.AddRange(WordsFromExpression(fp));

            return final.ToArray();


        }
    }
}
