using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;

/// <summary>
/// The project FormulaEvaluator which contains the class and method which calculate a mathematic expression
/// </summary>
namespace FormulaEvaluator
{
    /// <summary>
    /// A class containing the Evaluate method and various helper methods
    /// which takes a mathematic expression and Lookup function and performs the math needed to get a final integer answer
    /// </summary>
    public static class Evaluator
    {
        //The delegate for the Lookup function
        public delegate int Lookup(String v);


        

        /// <summary>
        /// The evaluate method, which takes an expression and Lookup and calculates and returns the correct int
        /// </summary>
        /// <param name="exp">The mathematic expression</param>
        /// <param name="variableEvaluator">The Lookup function used for variables</param>
        /// <returns>the correct integer from the expression</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {

            //The stack containing the integers that will be operated on
            Stack<int> valueStack = new Stack<int>();

            //The stack containing the operators that will be used on the integers
            Stack<String> operatorStack = new Stack<String>();

            //The string array of values from the given expression, which will be taken and put into their respective stacks 
            //(or ignored if they are only whitespace)
            String[] expToStack = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //Goes through each value in the string array, and performs the indicated action on it
            foreach (String s in expToStack)
            {
                s.Replace(" ", "");

                //check to see if the token was just whitespace, if so continue
                if (s.Length == 0 || s.Equals(" "))
                {
                    continue;
                }

                //check to see if the token is just an int or variable
                if (int.TryParse(s, out int _) || Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"))
                {
                    int token;

                    if (int.TryParse(s, out int _))
                    {
                        token = int.Parse(s);
                    }
                    else
                    {
                        token = variableEvaluator(s);
                    }

                    AddInt(token, valueStack, operatorStack);
                }

                //check to see if the token is a plus or minus sign
                else if (s.Equals("+") || s.Equals("-"))
                {
                    AddOrSubtractSign(true, s, valueStack, operatorStack);
                }

                //check to see if the token is a times or division sign
                else if (s.Equals("*") || s.Equals("/"))
                {
                    operatorStack.Push(s);
                }

                //check to see if the token is a left parenthesis
                else if (s.Equals("("))
                {
                    operatorStack.Push(s);
                }

                //check to see if the token is a right parenthesis
                else if (s.Equals(")"))
                {
                    RightParenthesis(valueStack, operatorStack);
                }

                //otherwise return that it is not any of the above
                else
                {
                    throw new ArgumentException(s + "is not a valid token");
                }


            }

            //check to see if the operator stack is not empty
            if (operatorStack.Count > 0)
            {
                //if the operator stack is too large or the value stack is not sized at two, throw an ArgumentExpression
                if (operatorStack.Count > 1 || valueStack.Count != 2)
                {
                    throw new ArgumentException("Too many operators at the end of expression");
                }

                //do addition to finalize the number to return
                int val1 = valueStack.Pop();
                int val2 = valueStack.Pop();
                String op = operatorStack.Pop();
                if (op.Equals("+"))
                {
                    valueStack.Push(val2 + val1);
                }
                else if (op.Equals("-"))
                {
                    valueStack.Push(val2 - val1);
                }
                //if last operator is not + or -, throw ArgumentException
                else
                {
                    throw new ArgumentException("final operator should be + or -");
                }
            }

            //check to see if the value stack has anything in it, throw exception otherwise
            if(valueStack.Count < 1)
            {
                throw new ArgumentException("expression must have at least one integer or variable");
            }

            return valueStack.Pop();



        }

        /// <summary>
        /// Helper method for Evaluate which is used when the right parenthesis are reached
        /// </summary>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static void RightParenthesis(Stack<int> vs, Stack<String> os)
        {

            AddOrSubtractSign(false, "empty", vs, os);

            //get rid of the left parenthesis or throw an exception if there isn't a left parenthesis there
            if (os.IsOnTop("("))
            {
                os.Pop();
            }
            else
            {
                throw new ArgumentException("Cannot have a right parenthesis without a left parenthesis");
            }

            //use the addInt function to check for a * or /
            if (vs.Count > 0)
            {
                int value1 = vs.Pop();
                AddInt(value1, vs, os);
            }
            
        }

        /// <summary>
        /// Helper method that adds an integer to the value stack, and also performs multiplication or division if the operator stack has a * or /
        /// </summary>
        /// <param name="t"> The integer being added</param>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static void AddInt(int t, Stack<int> vs, Stack<String> os)
        {
            //check to see if the operator stack has a * or / at the top
            if (os.IsOnTop("*") || os.IsOnTop("/"))
            {
                //value stack check
                if (vs.Count == 0)
                {
                    throw new ArgumentException("There is not a valid integer before the sign");
                }


                //divide by 0 check
                if (t == 0 && os.Peek().Equals("/"))
                {
                    throw new ArgumentException("Cannot divide by zero");
                }

                //once checks are good, perform either multiplication or division
                else
                {
                    int val1 = vs.Pop();
                    String op = os.Pop();

                    if (op.Equals("*"))
                    {
                        vs.Push(val1 * t);
                    }
                    else
                    {
                        vs.Push(val1 / t);
                    }
                }
            }

            //push the original value of t onto the stack
            else
            {
                vs.Push(t);
            }
        }

        /// <summary>
        /// helper method for adding or subtracting
        /// </summary>
        /// <param name="push"> Boolean to check whether or not pushing will occur (needed because of right parenthesis check)</param>
        /// <param name="s1"> String to be added to stack, either plus or minus</param>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static void AddOrSubtractSign(bool push, String s1, Stack<int> vs, Stack<String> os)
        {
            //check to see if the operator stack contains a + or - at the top
            if (os.IsOnTop("+") || os.IsOnTop("-"))
            {
                //if the value stack is too small throw exception
                if (vs.Count < 2)
                {
                    throw new ArgumentException("Not enough variables to make a calculation");
                }

                //perform the addition or subtraction
                int value1 = vs.Pop();
                int value2 = vs.Pop();
                String op1 = os.Pop();
                if (op1.Equals("+"))
                {
                    vs.Push(value2 + value1);
                }
                else
                {
                    vs.Push(value2 - value1);
                }
            }

            //as long as push is true, push the new symbol onto the operator stack
            if (push)
                os.Push(s1);
        }
    }
    /// <summary>
    /// Extension class for the Stack, which contains IsOnTop method, allowing for simplified code in the rest of PS1
    /// </summary>
    static class PS1StackExtensions
    {
        /// <summary>
        /// Checks to see if a certain value is on the top of the selected stack, without throwing an error
        /// </summary>
        /// <typeparam name="T">The given type of the stack</typeparam>
        /// <param name="ts">the stack of type T</param>
        /// <param name="topStack">the value that is being checked to see if it is on the top of the given stack</param>
        /// <returns></returns>
        public static bool IsOnTop<T>(this Stack<T> ts, T topStack)
        {
            return (ts.Count > 0 && ts.Peek().Equals(topStack));
        }
    }

}
