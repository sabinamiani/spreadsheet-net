// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

// (Tyler Wood)
// Version 1.3 (9/17/20)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {

        List<string> formulaList = new List<string>();

        List<string> varList = new List<string>();


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            //The previously run through string, for correct math grammar checks
            String prevString = "";

            //A counter for how many left parenthesis there are in the formula
            int leftParen = 0;
            
            //A counter for how many right parenthesis there are in the formula
            int rightParen = 0;
            
            foreach (string s in GetTokens(formula))
            {
                
                if (Double.TryParse(s, out double fInt))
                {
                    HelpNumber(fInt, prevString);
                }

                //check to see if s is a variable using the below regex expression
                else if (Regex.IsMatch(s, "^[a-zA-Z_]+[0-9a-zA-z_]*$"))
                {
                    HelpVariable(s, prevString, normalize, isValid);
                }

                
                else if (s.Equals("+") || s.Equals("-") || s.Equals("*") || s.Equals("/"))
                {
                    HelpOperator(s, prevString);
                }

                
                else if (s.Equals("("))
                {
                    HelpParenLeft(s, prevString, ref leftParen);
                }

                else if (s.Equals(")"))
                {
                    HelpParenRight(s, prevString, leftParen, ref rightParen);
                }

                else
                {
                    throw new FormulaFormatException("formula may only contain numbers, variables, parenthesis, or operators");
                }

                prevString = s;
            }

            //Covers Ending Token Rule
            if (prevString.Equals("(") || prevString.Equals("+") || prevString.Equals("-") || prevString.Equals("*") || prevString.Equals("/"))
            {
                throw new FormulaFormatException("The last token in a formula must be a number, variable, or closing parenthesis");
            }

            //Check parenthesis balance
            if (leftParen != rightParen)
            {
                throw new FormulaFormatException("There cannot be more opening than closing parenthesis in the formula");
            }

            //Covers One Token Rule
            if (formulaList.Count == 0)
            {
                throw new FormulaFormatException("There must be at least one token.");
            }
        }

        /// <summary>
        /// Helps when checking numbers, makes sure that the number is placed correctly
        /// </summary>
        /// <param name="doubleToCheck">The current string being checked then added</param>
        /// <param name="last">The previous string that was added</param>
        private void HelpNumber(double doubleToCheck, string last)
        {
            if (last.Equals("") || last.Equals("(") || last.Equals("+") || last.Equals("-") || last.Equals("*") || last.Equals("/"))
            {
                formulaList.Add("" + doubleToCheck);
            }
            else
            {
                throw new FormulaFormatException("A number may only follow an opening parenthesis or a basic operator");
            }
        }

        /// <summary>
        /// Helper method for checking if a variable is correct both in placement and in format, after normalizing the variable
        /// </summary>
        /// <param name="variable">the variable being checked</param>
        /// <param name="last">the previous string</param>
        /// <param name="normalize">the normalizer being used</param>
        /// <param name="isValid">the validater provided to the program</param>
        private void HelpVariable(string variable, string last, Func<string, string> normalize, Func<string, bool> isValid)
        {
            
            string normVar = normalize(variable);
            //check to see that the normalized variable still fits the rules
            if (!Regex.IsMatch(normVar, "^[a-zA-Z_]+[0-9a-zA-z_]*$"))
            {
                throw new FormulaFormatException("variable must be a valid variable after normalization");
            }
            //check to see if the variable is valid
            else if (!isValid(normVar))
            {
                throw new FormulaFormatException("variable must be valid");
            }
            
            else if (last.Equals("") || last.Equals("(") || last.Equals("+") || last.Equals("-") || last.Equals("*") || last.Equals("/"))
            {
                formulaList.Add(normVar);
                if (!varList.Contains(normVar))
                {
                    varList.Add(normVar);
                }

            }

            else
            {
                throw new FormulaFormatException("A variable can only follow an opening parenthesis or basic operator");
            }

        }

        /// <summary>
        /// A helper method to ensure that an operator was placed in the correct location
        /// </summary>
        /// <param name="op">The operator being checked</param>
        /// <param name="last">The previously added string</param>
        private void HelpOperator(string op, string last)
        {
            if (last.Equals("") || last.Equals("(") || last.Equals("+") || last.Equals("-") || last.Equals("*") || last.Equals("/"))
            {
                throw new FormulaFormatException("An operator can only follow a closed parenthesis, a number, or a variable");
            }
            else
            {
                formulaList.Add(op);
            }
        }

        /// <summary>
        /// Helper method to ensure that a left parenthesis was placed in the correct location, and to increase the count of the leftParen variable
        /// </summary>
        /// <param name="leftP">The left parenthesis</param>
        /// <param name="last">The previous string</param>
        /// <param name="leftParen">the counter for the number of left parenthesis in the formula</param>
        private void HelpParenLeft(string leftP, string last, ref int leftParen)
        {
            if (leftP.Equals("(") && (last.Equals("(") || last.Equals("") || last.Equals("+") || last.Equals("-") || last.Equals("*") || last.Equals("/")))
            {
                leftParen++;
                formulaList.Add(leftP);
            }
            else
            {
                throw new FormulaFormatException("Opening parenthesis may only follow another opening parenthesis or an operator");
            }


        }

        /// <summary>
        /// Helper method to ensure that the right parenthesis was placed in the correct location, and to increase the count of the rightParen variable, also checking afterwards to make sure
        /// that there is a correct balance between left and right parenthesis
        /// </summary>
        /// <param name="RightP"> The right parenthesis</param>
        /// <param name="last">The previous string</param>
        /// <param name="leftParen">The counter for the number of left parenthesis in the formula</param>
        /// <param name="rightParen">The counter for the number of right parenthesis in the formula</param>
        private void HelpParenRight(string RightP, string last, int leftParen, ref int rightParen)
        {
            if (RightP.Equals(")") && (last.Equals("(") || last.Equals("") || last.Equals("+") || last.Equals("-") || last.Equals("*") || last.Equals("/")))
            {
                throw new FormulaFormatException("Closing parenthesis may only follow another closing parenthesis, a number, or a variable, and cannot be the first token in a formula");

            }
            else
            {
                rightParen++;
                formulaList.Add(RightP);
            }
            if (rightParen > leftParen)
            {
                throw new FormulaFormatException("There cannot be more closing parenthesis than opening parenthesis in a formula");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //the stack containing the numerical values of the formula
            Stack<double> valueStack = new Stack<double>();

            //the stack containing the operators in the formula
            Stack<String> operatorStack = new Stack<String>();

            foreach (String s in formulaList)
            {
                //check to see if the token is just an int or variable
                if (double.TryParse(s, out double _) || Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"))
                {
                    double token;

                    if (Double.TryParse(s, out double parsedDouble))
                    {
                        token = parsedDouble;
                    }
                    else
                    {
                        try
                        {
                            token = lookup(s);
                        }
                        catch (ArgumentException)
                        {
                            return new FormulaError("Must give a valid variable");
                        }
                    }
                    if (AddDouble(token, valueStack, operatorStack) != null)
                    {
                        return new FormulaError("Cannot divide by zero");
                    }

                }

                else if (s.Equals("+") || s.Equals("-"))
                {
                    AddOrSubtractSign(true, s, valueStack, operatorStack);
                }


                else if (s.Equals("*") || s.Equals("/"))
                {
                    operatorStack.Push(s);
                }


                else if (s.Equals("("))
                {
                    operatorStack.Push(s);

                }


                else if (s.Equals(")"))
                {
                    if (RightParenthesis(valueStack, operatorStack) != null)
                    {
                        return new FormulaError("cannot divide by zero");
                    }
                }
            }

            //quick math to do in event that the operator stack is not empty
            if (operatorStack.Count > 0)
            {
                //do addition to finalize the number to return
                double val1 = valueStack.Pop();
                double val2 = valueStack.Pop();
                String op = operatorStack.Pop();
                if (op.Equals("+"))
                {
                    valueStack.Push(val2 + val1);
                }
                else if (op.Equals("-"))
                {
                    valueStack.Push(val2 - val1);
                }
            }

            return valueStack.Pop();



        }

        /// <summary>
        /// Helper method for Evaluate which is used when the right parenthesis are reached
        /// </summary>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static object RightParenthesis(Stack<double> vs, Stack<String> os)
        {

            AddOrSubtractSign(false, "empty", vs, os);

            //get rid of the left parenthesis or throw an exception if there isn't a left parenthesis there
            if (IsOnTop(os, "("))
            {
                os.Pop();
            }

            //use the addDouble function to check for a * or /
            if (vs.Count > 0)
            {
                
                if (AddDouble(vs.Pop(), vs, os) != null)
                {
                    return new FormulaError("Cannot divide by zero");
                }
            }
            return null;

        }

        /// <summary>
        /// Helper method that adds a double to the value stack, and also performs multiplication or division if the operator stack has a * or /
        /// </summary>
        /// <param name="doubleAdd"> The double being added</param>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static object AddDouble(double doubleAdd, Stack<double> vs, Stack<String> os)
        {
            //check to see if the operator stack has a * or / at the top
            if (IsOnTop(os, "*") || IsOnTop(os, "/"))
            {


                //divide by 0 check
                if (doubleAdd == 0 && os.Peek().Equals("/"))
                {
                    return new FormulaError("Cannot divide by zero");
                }

                //once checks are good, perform either multiplication or division
                else
                {
                    double val1 = vs.Pop();
                    String op = os.Pop();

                    if (op.Equals("*"))
                    {
                        vs.Push(val1 * doubleAdd);
                    }
                    else
                    {
                        vs.Push(val1 / doubleAdd);
                    }
                }
            }

            //push the original value of t onto the stack
            else
            {
                vs.Push(doubleAdd);
            }

            return null;
        }

        /// <summary>
        /// helper method for adding or subtracting
        /// </summary>
        /// <param name="push"> Boolean to check whether or not pushing will occur (needed because of right parenthesis check)</param>
        /// <param name="sign"> String to be added to stack, either plus or minus</param>
        /// <param name="vs"> The value stack</param>
        /// <param name="os"> The operator stack</param>
        private static void AddOrSubtractSign(bool push, String sign, Stack<double> vs, Stack<String> os)
        {
            //check to see if the operator stack contains a + or - at the top
            if (IsOnTop(os, "+") || IsOnTop(os, "-"))
            {
                //perform the addition or subtraction
                double secondVal = vs.Pop();
                double firstVal = vs.Pop();
                String topOp = os.Pop();
                if (topOp.Equals("+"))
                {
                    vs.Push(firstVal + secondVal);
                }
                else
                {
                    vs.Push(firstVal - secondVal);
                }
            }

            //as long as push is true, push the new symbol onto the operator stack
            if (push)
                os.Push(sign);
        }

        /// <summary>
        /// Helper method to check and see if the requested value is on top of the requested stack
        /// </summary>
        /// <typeparam name="T">The type of the stack and given parameter</typeparam>
        /// <param name="ts">The stack being checked</param>
        /// <param name="topStack">The given parameter to check</param>
        /// <returns></returns>
        private static bool IsOnTop<T>(Stack<T> ts, T topStack)
        {
            return (ts.Count > 0 && ts.Peek().Equals(topStack));
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            foreach (string var in varList)
            {
                yield return var;
            }
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            foreach (string s in formulaList)
            {
                returnString.Append(s.ToString());
            }
            return returnString.ToString();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }
            else if (obj is Formula)
            {
                return obj.ToString().Equals(this.ToString());

            }
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            
            if (Object.ReferenceEquals(f1, null))
            {
                if(Object.ReferenceEquals(f2, null))
                    return true;
                else
                {
                    return false;
                }
            }
            
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (!Object.ReferenceEquals(f1, null) && Object.ReferenceEquals(f2, null) || Object.ReferenceEquals(f1, null) && !Object.ReferenceEquals(f2, null))
            {
                return true;
            }
            else if (Object.ReferenceEquals(f1, null) && Object.ReferenceEquals(f2, null))
            {
                return false;
            }
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}



