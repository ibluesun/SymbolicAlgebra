using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Reflection;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
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
        /// <param name="expr"></param>
        /// <returns></returns>
        public static SymbolicVariable Parse(string expression)
        {
            

            var r = from zr in expression.Split(new string[]{":="}, StringSplitOptions.RemoveEmptyEntries)
                    where string.IsNullOrWhiteSpace(zr)==false
                    select zr.Trim();
            
            string[] rr = r.ToArray();

            string expr  = string.Empty;
            if (rr.Length == 0) return null;
            if (rr.Length > 1)
            {
                //function
                expr = rr[1];
            }
            else
                expr = rr[0];

            char[] separators = {'^', '*', '/', '+', '-', '(', '|'};
            char[] seps = {'^', '*', '/', '+', '-', '|'};


            expr = expr.Replace(" ", "");

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
            for (int ix = 0; ix < expr.Length; ix++)
            {
                if (PLevels.Count == 0)
                {
                    // include the normal parsing when we are not in parenthesis group
                    if (separators.Contains(expr[ix]))
                    {
                        if ((expr[ix] == '-' || expr[ix] == '+') && ix == 0)
                        {
                            if (expr[ix] == '-')
                            {
                                // in case of -x  for example ..this will converted into operations
                                //  of -1 * x
                                ep.SymbolicExpression = SymbolicVariable.NegativeOne;
                                ep.Operation = "*";

                                ep.Next = new SymbolicExpressionOperator();
                                ep = ep.Next;           // advance the reference to the next node to make the linked list.

                                TokenBuilder = new StringBuilder();
                            }
                            else
                            {
                                // append the normal token
                                TokenBuilder.Append(expr[ix]);
                            }
                        }
                        else if (ix > 1 
                            && char.ToUpper(expr[ix - 1]) == 'E' && char.IsDigit(expr[ix - 2])
                            && (expr[ix] == '-' || expr[ix] == '+'))
                        {
                            // case of 3e+2 4e-2  all cases with e in it.
                            TokenBuilder.Append(expr[ix]);
                        }
                        else if (expr[ix] == '(')
                        {
                            PLevels.Push(1);
                            var bb = ix > 0 ? separators.Contains(expr[ix - 1]) : true;
                            if (!bb)
                            {
                                //the previous charachter is normal word which indicates we reached a function
                                FunctionContext = true;
                                TokenBuilder.Append(expr[ix]);
                            }
                        }
                        else if (seps.Contains(expr[ix - 1]) && (expr[ix] == '-' || expr[ix] == '+'))
                        {
                            TokenBuilder.Append(expr[ix]);
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

                            ep.Operation = expr[ix].ToString();
                            ep.Next = new SymbolicExpressionOperator();
                            ep = ep.Next;           // advance the reference to the next node to make the linked list.
                        }
                    }
                    else
                    {
                        TokenBuilder.Append(expr[ix]);
                    }
                }
                else
                {
                    // we are in group
                    if (expr[ix] == '(')
                    {
                        PLevels.Push(1);
                    }
                    if (expr[ix] == ')')
                    {
                        PLevels.Pop();

                        if (PLevels.Count == 0)
                        {
                            Inner = true;
                            if (FunctionContext)
                            {
                                TokenBuilder.Append(expr[ix]);
                                FunctionContext = false;
                                Inner = false;   // because i am taking the function body as a whole in this parse pass.
                                // then inner parameters of the function will be parsed again 
                            }
                        }
                        else
                        {
                            TokenBuilder.Append(expr[ix]);    
                        }
                    }
                    else
                    {
                        TokenBuilder.Append(expr[ix]);
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

            if (rr.Length > 1)
            {
                // there was a function defintion
                var fname = rr[0];

                Functions[fname] = Root.SymbolicExpression;
                
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

            if (op == "^")
            {

                // This will be right associative operator
                // which means if more than one power appeared like this 3^2^4  then it will be processed like this 3^(2^4)

                if (eop.Next.Next != null)
                {
                    if (eop.Next.Operation == "^")
                    {
                        short iskip;
                        var powerResult = SymbolicVariable.SymbolicPower(
                            left
                            , ArithExpression(eop.Next, out iskip)
                            );

                        skip += iskip;
                        return powerResult;
                    }
                    else
                    {
                        return SymbolicVariable.SymbolicPower(left, right);
                    }
                }
                else
                {
                    return SymbolicVariable.SymbolicPower(left, right);
                }
                
            }

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


            if (op == "^")
            {
                // This will be right associative operator
                // which means if more than one power appeared like this 3^2^4  then it will be processed like this 3^(2^4)

                if (eop.Next.Next != null)
                {
                    if (eop.Next.Operation == "^")
                    {
                        short iskip;
                        var powerResult = Expression.Power(
                            left
                            , ArithExpression(eop.Next, out iskip)
                            );

                        skip += iskip;
                        return powerResult;
                    }
                    else
                    {
                        return Expression.Power(left, right);
                    }
                }
                else
                {
                    return Expression.Power(left, right);
                }
            }

            if (op == "*") return Expression.Multiply(left, right);
            if (op == "/") return Expression.Divide(left, right);
            if (op == "+") return Expression.Add(left, right);
            if (op == "-") return Expression.Subtract(left, right);

            if (op == "<") return Expression.LessThan(left, right);
            if (op == ">") return Expression.GreaterThan(left, right);
            if (op == "<=") return Expression.LessThanOrEqual(left, right);
            if (op == ">=") return Expression.GreaterThanOrEqual(left, right);
            if (op == "=") return Expression.Equal(left, right);
            if (op == "<>") return Expression.NotEqual(left, right);


            throw new NotSupportedException("Not Supported Operator '" + op + "'");
        }

        /// <summary>
        /// Returns an expression that can be compiled into a function to be called dynamically.
        /// </summary>
        /// <returns></returns>
        private Expression ParseDynamicExpression(ref Dictionary<string, ParameterExpression> discoveredParameters, string expression =  null)
        {
            if (string.IsNullOrEmpty(expression))
            {
                // this is the final text to be parsed.
                expression = this.ToString();
            }

            bool NegativeExpression = false;

            if (expression.StartsWith("-"))
            {
                NegativeExpression = true;
                expression = expression.TrimStart('-');  // remove any trailing minuses
            }


            char[] separators = { '^', '*', '/', '+', '-', '(', '<', '>', '=' };

            char[] operators = { '^', '*', '/', '+', '-' };


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

            #region method that will be reused
            Action<Dictionary<string, ParameterExpression>> redundantFunction = (parameters) =>
            {
                // Last pass that escaped from the loop.
                if (Inner)
                {
                    
                    ep.DynamicExpression = ParseDynamicExpression(ref parameters, TokenBuilder.ToString());
                    
                    Inner = false;
                }
                else
                {
                    double constant;
                    if (TokenBuilder.ToString().Equals("Infinity", StringComparison.OrdinalIgnoreCase) || TokenBuilder.ToString().Equals("inf", StringComparison.OrdinalIgnoreCase))
                    {
                        ep.DynamicExpression = Expression.Constant(double.PositiveInfinity, typeof(double));
                    }
                    else if (TokenBuilder.ToString().Equals("NaN", StringComparison.OrdinalIgnoreCase))
                    {
                        ep.DynamicExpression = Expression.Constant(double.NaN, typeof(double));
                    }
                    else if (TokenBuilder[0] == '%')
                    {
                        // constant 
                        ep.DynamicExpression = Expression.Constant(MathConstants.Constant(TokenBuilder.ToString().TrimStart('%')), typeof(double));
                    }
                    else if (double.TryParse(TokenBuilder.ToString(), out constant))
                    {
                        ep.DynamicExpression = Expression.Constant(constant, typeof(double));
                    }
                    else
                    {
                        var pname = TokenBuilder.ToString();

                        var FMatch = FunctionRegex.Match(pname);
                        // test if the parameter is a function
                        if (FMatch.Success)
                        {
                            // take the function name and search for it 
                            // if you found it take the inner parameters and parse it independently
                            var fname = FMatch.Groups["function"].Value;

                            // search for the function in the math class
                            MethodInfo targetfunction;

                            if (CoMath.AvailableFunctions.Contains(fname, StringComparer.OrdinalIgnoreCase))
                                targetfunction = typeof(CoMath).GetMethod(
                                        fname
                                        , System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                                        );
                            else if (fname.Equals("log", StringComparison.OrdinalIgnoreCase))
                                targetfunction = typeof(Math).GetMethod("Log", new Type[] { typeof(double) });
                            else if (fname.Equals("iif", StringComparison.OrdinalIgnoreCase))
                                targetfunction = typeof(CoLogic).GetMethod("IIF", new Type[] { typeof(bool), typeof(double), typeof(double) });
                            else
                                targetfunction = typeof(Math).GetMethod(
                                    fname
                                    , System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                                    );

                            if (targetfunction != null)
                            {
                                // for now take the 
                                var fps = FMatch.Groups["parameters"].Value;
                                string[] pss = TextTools.ComaSplit(fps);
                                Expression[] tfparams = new Expression[pss.Length];
                                for (int ixf = 0; ixf < pss.Length; ixf++)
                                {
                                    tfparams[ixf] = ParseDynamicExpression(ref parameters, pss[ixf]);
                                }
                                ep.DynamicExpression = Expression.Call(targetfunction, tfparams);
                            }
                            else
                            {
                                var fn = Functions.Keys.FirstOrDefault(cc => cc.StartsWith(fname));
                                if (!string.IsNullOrEmpty(fn))
                                {
                                    // this is good  because we have a body.
                                    ep.DynamicExpression = Functions[fn].ParseDynamicExpression(ref parameters);
                                }
                                else
                                {
                                    throw new SymbolicException(string.Format("The target function {0} couldn't be found", fname));
                                }
                            }
                        }
                        else
                        {
                            // this is an ordinary parameter based on our test.
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
                }
            };


            #endregion

            for (int ix = 0; ix < expression.Length; ix++)
            {
                if (PLevels.Count == 0)
                {
                    // include the normal parsing when we are not in parenthesis group
                    if (separators.Contains(expression[ix]))
                    {
                        if ((expression[ix] == '-' || expression[ix] == '+') && ix == 0)
                        {
                            // add first character if - or +  
                            TokenBuilder.Append(expression[ix]);
                        }
                        else if (ix > 1
                            && char.ToUpper(expression[ix - 1]) == 'E' && char.IsDigit(expression[ix - 2])
                            && (expression[ix] == '-' || expression[ix] == '+'))
                        {
                            // test previous charachter if E    and charachter before it to be digit and current character is - or + 
                            //    then we are in sceintific representation of number

                            // case of 3e+2 4e-2  all cases with e in it.

                            TokenBuilder.Append(expression[ix]);
                        }
                        else if (expression[ix] == '(')
                        {
                            // either we reached a function of expression group.
                            PLevels.Push(1);

                            // test for something other than separator in previous charachter
                            var OperatorBehind = ix > 0 ? separators.Contains(expression[ix - 1]) : true;

                            if (!OperatorBehind)
                            {
                                //the previous charachter is normal word which indicates we reached a function
                                FunctionContext = true;
                                TokenBuilder.Append(expression[ix]);
                            }
                        }
                        else if (operators.Contains(expression[ix - 1]) && (expression[ix] == '-' || expression[ix] == '+'))
                        {
                            // the case in which last charachter was another operator and followed by current (-) or (+)
                            TokenBuilder.Append(expression[ix]);
                        }
                        else
                        {
                            // tokenize   when we reach any operator  or open '(' parenthesis 

                            redundantFunction(discoveredParameters);

                            TokenBuilder = new StringBuilder();

                            ep.Operation = expression[ix].ToString();

                            if (expression[ix] == '<' || expression[ix] == '>')
                            {
                                // check for equal sign after this 
                                if (expression[ix + 1] == '=')
                                {
                                    ep.Operation += "=";
                                    ix++;
                                }
                            }

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

            redundantFunction(discoveredParameters);

            TokenBuilder = null;


            string[] Group = { "^"    /* Power for normal product '*' */
                             };


            string[] Group1 = { "*"   /* normal multiplication */, 
                                "/"   /* normal division */, 
                                "%"   /* modulus */ };


            string[] Group2 = { "+", "-" };

            string[] Group3 = { "<", "<=", ">", ">=" };

            string[] Group4 = { "=", "<>" };

            /// Operator Groups Ordered by Priorities.
            string[][] OperatorGroups = { Group, Group1, Group2, Group3, Group4 };

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

            Expression FinalExpression;

            if (NegativeExpression)
                FinalExpression = Expression.Multiply(Expression.Constant(-1.0, typeof(double)), Root.DynamicExpression);
            else 
                FinalExpression = Root.DynamicExpression;

            return FinalExpression;

        }


        private Expression DynamicBody;
        private Dictionary<string, ParameterExpression> DynamicParameters = new Dictionary<string,ParameterExpression>();
        private LambdaExpression Lambda;
        private Delegate FunctionDelegate;


        private void PrepareExecute()
        {

            if (DynamicBody == null)
            {
                var t0 = this[0];
                DynamicBody = this[0].ParseDynamicExpression(ref DynamicParameters);
                //if (t0.IsNegative)
                //    DynamicBody = Expression.Multiply(Expression.Constant(-1.0), DynamicBody);

                // i will parse each term alone.  // so that i have more control over the parse
                for (int tc = 1; tc < TermsCount; tc++)
                {
                    var rt = this[tc];

                    //if (rt.IsNegative)
                    //    DynamicBody = Expression.Subtract(DynamicBody, rt.ParseDynamicExpression(ref DynamicParameters));
                    //else
                        DynamicBody = Expression.Add(DynamicBody, rt.ParseDynamicExpression(ref DynamicParameters));
                }

                // sort the parameter based on alphabet.

                var finalexparms = from exp in DynamicParameters
                                   orderby exp.Key
                                   select exp.Value;

                Lambda = Expression.Lambda(DynamicBody, finalexparms.ToArray());
                FunctionDelegate = Lambda.Compile();
            }
        }

        /// <summary>
        /// Execute the expression and give the result back.
        /// </summary>
        /// <param name="parameters">Dictionary of parameters name and value in double</param>
        /// <returns></returns>
        public double Execute(Dictionary<string, double> parameters)
        {

            PrepareExecute();

            var pcount = this.InvolvedSymbols.Length;

            if (parameters.Count != pcount) throw new SymbolicException("Number of arguments is not correct");

            double[] FinalParams = (from pr in parameters 
                                   orderby pr.Key
                                   select pr.Value).ToArray();


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



        public double Execute(params Tuple<string, double>[] parameters)
        {
            Dictionary<string, double> FinalParams = new Dictionary<string, double>();
            
            foreach (var p in parameters) FinalParams.Add(p.Item1, p.Item2);

            return Execute(FinalParams);

        }


        /// <summary>
        /// Execute expression that take no arguments.
        /// </summary>
        /// <returns></returns>
        public double Execute()
        {
            PrepareExecute();
            return ((Func<double>)FunctionDelegate)();
        }

        /// <summary>
        /// Execute Function that expect you to pass arguments in alphabet argument order.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Expression Evaluation</returns>
        public double Execute(params double[] parameters)
        {
            PrepareExecute();

            var pcount = this.InvolvedSymbols.Length;
            
            if (parameters.Length < pcount) throw new SymbolicException(string.Format("Feeded parameters are less than the required parameters of the expression {0} expected where {1} feeded", pcount, parameters.Length));

            if (pcount == 0) return ((Func<double>)FunctionDelegate)();

            if (pcount == 1) return ((Func<double, double>)
                FunctionDelegate)(parameters[0]);

            if (pcount == 2) return ((Func<double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1]);

            if (pcount == 3) return ((Func<double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2]);

            if (pcount == 4) return ((Func<double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3]);

            if (pcount == 5) return ((Func<double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);

            if (pcount == 6) return ((Func<double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);

            if (pcount == 7) return ((Func<double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]);

            if (pcount == 8) return ((Func<double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]);

            if (pcount == 9) return ((Func<double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8]);

            if (pcount == 10) return ((Func<double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9]);

            if (pcount == 11) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10]);

            if (pcount == 12) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11]);

            if (pcount == 13) return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
                FunctionDelegate)(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11], parameters[12]);

            throw new Exception("What is that call ???!!");
        }

        
    }
}