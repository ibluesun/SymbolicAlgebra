using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {

        private class ExprOp
        {
            public string Operation;
            public ExprOp Next;

            public SymbolicVariable SymbolicExpression;
        }


        /// <summary>
        /// Parse expression of variables and make SymbolicVariable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SymbolicVariable Parse(string expression)
        {
            char[] separators = {'^', '*', '/', '+', '-', '(', '|'};
            char[] seps = {'^', '*', '/', '+', '-', '|'};


            expression = expression.Replace(" ", "");

            //if (expression.StartsWith("-") ||expression.StartsWith("+")) expression = expression.Insert(1,"1*");

            // simple parsing 
            // obeys the rules of priorities

            // Priorities
            //    ^  Power
            //    *  multiplication
            //    /  division
            //    +  Addition
            //    -  Subtraction

            
            // Tokenization is done by separating with operators
            ExprOp Root = new ExprOp();
            ExprOp ep = Root;

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
                            TokenBuilder.Append(expression[ix]);
                        }
                        else if (expression[ix] == '(')
                        {
                            PLevels.Push(1);
                            var bb = ix>0?separators.Contains(expression[ix-1]):true;
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
                                ep.SymbolicExpression = Parse(TokenBuilder.ToString());
                                Inner = false;
                            }
                            else
                            {
                                ep.SymbolicExpression = new SymbolicVariable(TokenBuilder.ToString());
                            }
                            TokenBuilder.Clear();

                            ep.Operation = expression[ix].ToString();
                            ep.Next = new ExprOp();
                            ep = ep.Next;           // advance the reference to the next node to make the linked list.
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
                ep.SymbolicExpression = Parse(TokenBuilder.ToString());
                Inner = false;
            }
            else
            {
                ep.SymbolicExpression = new SymbolicVariable(TokenBuilder.ToString());
            }
            TokenBuilder.Clear();


            string[] Group = { "^"    /* Power for normal product '*' */
                             };

            string[] SymGroup = { "|" /* Derivation operator */};

            string[] Group1 = { "*"   /* normal multiplication */, 
                                "/"   /* normal division */, 
                                "%"   /* modulus */ };


            string[] Group2 = { "+", "-" };

            /// Operator Groups Ordered by Priorities.
            string[][] OperatorGroups = { Group, SymGroup, Group1, Group2};

            foreach (var opg in OperatorGroups)
            {
                ExprOp eop = Root;

                //Pass for '[op]' and merge it  but from top to child :)  {forward)
                while (eop.Next != null)
                {
                    //if the operator in node found in the opg (current operator group) then execute the logic

                    if (opg.Count(c => c.Equals(eop.Operation, StringComparison.OrdinalIgnoreCase)) > 0)
                    {
                        short skip;
                        eop.SymbolicExpression = ArithExpression(eop, out skip);

                        //drop eop.Next
                        if (eop.Next.Next != null)
                        {
                            while (skip > 0)
                            {
                                eop.Operation = eop.Next.Operation;

                                eop.Next = eop.Next.Next;

                                skip--;
                            }
                        }
                        else
                        {
                            //no more nodes exit the loop

                            eop.Next = null;      //last item were processed.
                            eop.Operation = string.Empty;
                        }
                    }
                    else
                    {
                        eop = eop.Next;
                    }
                }
            }

            return Root.SymbolicExpression;
        }

        private static SymbolicVariable ArithExpression(ExprOp eop, out short skip)
        {

            SymbolicVariable left = eop.SymbolicExpression;
            string op = eop.Operation;
            SymbolicVariable right = eop.Next.SymbolicExpression;

            skip = 1;

            if (op == "|")
            {
                int p = (int) right.SymbolPower;
                string rp = right.Symbol;

                SymbolicVariable v = left;
                while (p > 0)
                {
                    v = v.Differentiate(rp);
                    p--;
                }
                return v;
            }

            if (op == "^") return SymbolicVariable.SymbolicPower(left, right);
            if (op == "*") return SymbolicVariable.Multiply(left, right);
            if (op == "/") return SymbolicVariable.Divide(left, right);
            if (op == "+") return SymbolicVariable.Add(left, right);
            if (op == "-") return SymbolicVariable.Subtract(left, right);


            throw new NotSupportedException("Not Supported Operator '" + op + "'");
        }

    }
}