using System;
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
            int Split = 0;
            List<string> all = new List<string>();

            int i = 0;
            
            StringBuilder SplitedText = new StringBuilder();

            while (i < expression.Length)
            {
                if (expression[i] == '(') Split++;
                if (expression[i] == ')') Split--;

                if (Split==0 & expression[i] == ',')
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
    }
}
