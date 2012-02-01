using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq.Expressions;


namespace SymbolicAlgebra
{
#if SILVERLIGHT
    public partial class SymbolicVariable
#else
    public partial class SymbolicVariable : ICloneable
#endif
    {
        private class SymbolicExpressionOperator
        {
            public string Operation;
            public SymbolicExpressionOperator Next;

            public SymbolicVariable SymbolicExpression;
        }

        private class DynamicExpressionOperator
        {
            public string Operation;
            public DynamicExpressionOperator Next;

            public Expression DynamicExpression;
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
            SymbolicExpressionOperator Root = new SymbolicExpressionOperator();
            SymbolicExpressionOperator ep = Root;

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

                            TokenBuilder = new StringBuilder();

                            ep.Operation = expression[ix].ToString();
                            ep.Next = new SymbolicExpressionOperator();
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
            TokenBuilder = null;


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
                SymbolicExpressionOperator eop = Root;

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

        private static SymbolicVariable ArithExpression(SymbolicExpressionOperator eop, out short skip)
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


        private static Expression ArithExpression(DynamicExpressionOperator eop, out short skip)
        {

            Expression left = eop.DynamicExpression;
            string op = eop.Operation;
            Expression right = eop.Next.DynamicExpression;

            skip = 1;


            if (op == "^") return Expression.Power(left, right);
            if (op == "*") return Expression.Multiply(left, right);
            if (op == "/") return Expression.Divide(left, right);
            if (op == "+") return Expression.Add(left, right);
            if (op == "-") return Expression.Subtract(left, right);


            throw new NotSupportedException("Not Supported Operator '" + op + "'");
        }

        /// <summary>
        /// Returns an expression that can be compiled into a function to be called dynamically.
        /// </summary>
        /// <returns></returns>
        private Expression ParseDynamicExpression(out Dictionary<string, ParameterExpression> parameters, string expression =  null)
        {
            if (string.IsNullOrEmpty(expression))
            {
                // this is the final text to be parsed.
                expression = this.ToString();
            }

            parameters = new Dictionary<string, ParameterExpression>();



            char[] separators = { '^', '*', '/', '+', '-', '(' };
            char[] seps = { '^', '*', '/', '+', '-' };


            expression = expression.Replace(" ", "");

            // simple parsing 
            // obeys the rules of priorities

            // Priorities
            //    ^  Power
            //    *  multiplication
            //    /  division
            //    +  Addition
            //    -  Subtraction


            // Tokenization is done by separating with operators
            DynamicExpressionOperator Root = new DynamicExpressionOperator();
            DynamicExpressionOperator ep = Root;

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
                                Dictionary<string, ParameterExpression> subParameters;
                                ep.DynamicExpression = ParseDynamicExpression(out subParameters, TokenBuilder.ToString());
                                foreach (var p in subParameters) parameters.Add(p.Key, p.Value);
                                Inner = false;
                            }
                            else
                            {
                                double constant;
                                if (double.TryParse(TokenBuilder.ToString(), out constant))
                                {
                                    ep.DynamicExpression = Expression.Constant(constant, typeof(double));
                                }
                                else
                                {
                                    var pname = TokenBuilder.ToString();
                                    ParameterExpression pe;
                                    if (parameters.TryGetValue(pname, out pe))
                                    {
                                        ep.DynamicExpression = pe;
                                    }
                                    else
                                    {
                                        pe = Expression.Parameter(typeof(double), pname);

                                        ep.DynamicExpression = pe;

                                        parameters.Add(pname, pe);
                                    }
                                }
                            }

                            TokenBuilder = new StringBuilder();

                            ep.Operation = expression[ix].ToString();
                            ep.Next = new DynamicExpressionOperator();
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
                Dictionary<string, ParameterExpression> subParameters;
                ep.DynamicExpression = ParseDynamicExpression(out subParameters, TokenBuilder.ToString());
                foreach (var p in subParameters) parameters.Add(p.Key, p.Value);
                Inner = false;
            }
            else
            {
                double constant;
                if (double.TryParse(TokenBuilder.ToString(), out constant))
                {
                    ep.DynamicExpression = Expression.Constant(constant, typeof(double));
                }
                else
                {
                    var pname = TokenBuilder.ToString();
                    ParameterExpression pe;
                    if (parameters.TryGetValue(pname, out pe))
                    {
                        ep.DynamicExpression = pe;
                    }
                    else
                    {
                        pe = Expression.Parameter(typeof(double), pname);

                        ep.DynamicExpression = pe;

                        parameters.Add(pname, pe);
                    }
                }
            }
            TokenBuilder = null;


            string[] Group = { "^"    /* Power for normal product '*' */
                             };


            string[] Group1 = { "*"   /* normal multiplication */, 
                                "/"   /* normal division */, 
                                "%"   /* modulus */ };


            string[] Group2 = { "+", "-" };

            /// Operator Groups Ordered by Priorities.
            string[][] OperatorGroups = { Group, Group1, Group2 };

            foreach (var opg in OperatorGroups)
            {
                DynamicExpressionOperator eop = Root;

                //Pass for '[op]' and merge it  but from top to child :)  {forward)
                while (eop.Next != null)
                {
                    //if the operator in node found in the opg (current operator group) then execute the logic

                    if (opg.Count(c => c.Equals(eop.Operation, StringComparison.OrdinalIgnoreCase)) > 0)
                    {
                        short skip;
                        eop.DynamicExpression = ArithExpression(eop, out skip);

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

            return Root.DynamicExpression;

        }


        private Expression DynamicBody;
        private Dictionary<string, ParameterExpression> DynamicParameters;
        private LambdaExpression Lambda;
        private Delegate FunctionDelegate;


        /// <summary>
        /// Execute the expression and give the result back.
        /// </summary>
        /// <param name="parameters">Array of tuples of parameter name and value in double</param>
        /// <returns></returns>
        public double Execute(params Tuple<string, double>[] parameters)
        {
            
            var pcount = this.InvolvedSymbols.Length;
            if (parameters.Length != pcount) throw new SymbolicException("Number of arguments is not correct");

            if (DynamicBody == null)
            {
                DynamicBody = ParseDynamicExpression(out DynamicParameters);
                Lambda = Expression.Lambda(DynamicBody, DynamicParameters.Values);
                FunctionDelegate = Lambda.Compile();
            }

            double[] FinalParams = new double[DynamicParameters.Count];

            // map parameters to the discovered ones.
            for (int ix = 0; ix < DynamicParameters.Count; ix++ )
            {
                // take the parameter value from passed parameters by the dynamicparameter name.
                string key = DynamicParameters.Keys.ElementAt(ix);
                FinalParams[ix] = parameters.First(x => x.Item1 == key).Item2;
            }

            if (pcount == 0) return ((Func<double>)FunctionDelegate)();

            if (pcount == 1) return ((Func<double, double>)
                FunctionDelegate)(FinalParams[0]);

            if (pcount == 2) return ((Func<double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1]);

            if (pcount == 3) return ((Func<double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2]);

            if (pcount == 4) return ((Func<double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3]);

            if (pcount == 5) return ((Func<double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4]);

            if (pcount == 6) return ((Func<double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5]);

            if (pcount == 7) return ((Func<double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6]);

            if (pcount == 8) return ((Func<double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7]);

            if (pcount == 9) return ((Func<double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7], FinalParams[8]);

            if (pcount == 10) return ((Func<double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7], FinalParams[8], FinalParams[9]);

            if (pcount == 11) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7], FinalParams[8], FinalParams[9], FinalParams[10]);

            if (pcount == 12) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7], FinalParams[8], FinalParams[9], FinalParams[10], FinalParams[11]);

            if (pcount == 13) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(FinalParams[0], FinalParams[1], FinalParams[2], FinalParams[3], FinalParams[4], FinalParams[5], FinalParams[6], FinalParams[7], FinalParams[8], FinalParams[9], FinalParams[10], FinalParams[11], FinalParams[12]);



            throw new Exception("What is that call ???!!");
        }


        /// <summary>
        /// Simplified execute in case of expression contains one parameter only.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public double Execute(double parameter)
        {
            return Execute(new Tuple<string, double>(InvolvedSymbols[0], parameter));
        }
    }
}