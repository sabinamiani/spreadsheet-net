using FormulaEvaluator;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FormulaEvaluatorTesting
{
    class Program
    {
        static void Main(string[] args)
        {

            
            //A short string of addition
            int solToParen = 7;
            int evalParen = Evaluator.Evaluate("3+4", LookupSimple);
            if (solToParen == evalParen)
            {
                Console.Out.WriteLine("Test addition passed");
            }
            else
            {
                Console.WriteLine("Failed no paren, gave value of " + evalParen);
            }

            //A short string of subtraction
            int solToParen2 = -1;
            int evalParen2 = Evaluator.Evaluate("3-4", LookupSimple);
            if (solToParen2 == evalParen2)
            {
                Console.Out.WriteLine("Test subtraction passed");
            }
            else
            {
                Console.WriteLine("Failed no paren, gave value of " + evalParen2);
            }

            //A short string of multiplication
            int solToParen3 = 12;
            int evalParen3 = Evaluator.Evaluate("3*4", LookupSimple);
            if (solToParen3 == evalParen3)
            {
                Console.Out.WriteLine("Test multiplication passed");
            }
            else
            {
                Console.WriteLine("Failed no paren, gave value of " + evalParen3);
            }

            //A short string of division
            int solToParen4 = 2;
            int evalParen4 = Evaluator.Evaluate("6/3", LookupSimple);
            if (solToParen4 == evalParen4)
            {
                Console.Out.WriteLine("Test division passed");
            }
            else
            {
                Console.WriteLine("Failed no paren, gave value of " + evalParen4);
            }

            //A short string to test on
            String testShort = "(2+4)/2";
            int solToShort = 3;
            int evalShort = Evaluator.Evaluate(testShort, LookupSimple);
            if (solToShort == evalShort)
            {
                Console.Out.WriteLine("Test short string passed");
            }
            else
            {
                Console.Out.WriteLine("Test short String failed, gave value of " + evalShort);
            }

            //A mid sized string to test on
            String testMid = "(10+5)*2+3*(4+4)";
            int solToMid = 54;
            int evalMid = Evaluator.Evaluate(testMid, LookupSimple);
            if (solToMid == evalMid)
            {
                Console.WriteLine("Test mid string passed");
            }
            else
            {
                Console.WriteLine("test mid failed with " + evalMid);
            }


            //A long string to test on
            String testLong = "4/(6+3)*7-5*4*3+(4-3)";
            int solToLong = -59;
            int evalLong = Evaluator.Evaluate(testLong, LookupSimple);
            if (solToLong == evalLong)
            {
                Console.WriteLine("Test long string passed");
            }
            else
            {
                Console.WriteLine("Test long failed with " + evalLong);
            }

            //A string with whitespace to test on in the middle
            String testWhiteMiddle = "(4 + 3) * 3";
            int solToWhMid = 21;
            int evalWhMid = Evaluator.Evaluate(testWhiteMiddle, LookupSimple);
            if (solToWhMid == evalWhMid)
            {
                Console.WriteLine("Test with whitespace in middle passed");
            }
            else
            {
                Console.WriteLine("Whitespace middle failed, gave value of " + evalWhMid);
            }
            //A string with leading and tailing white space
            String testEndWhtSpc = " (4+4)-3 ";
            int solToEndWhtSpc = 5;
            int evalEndWhtSpc = Evaluator.Evaluate(testEndWhtSpc, LookupSimple);
            if (solToEndWhtSpc == evalEndWhtSpc)
            {
                Console.WriteLine("Test with leading and tailing white space passed");
            }
            else
            {
                Console.WriteLine("lead and tail white space test failed with " + evalEndWhtSpc);
            }


            //A string with a variable to test on
            int solToVar = 7;
            int evalVar = Evaluator.Evaluate("3+a4", LookupSimple);
            if (solToVar == evalVar)
            {
                Console.Out.WriteLine("Test variable passed");
            }
            else
            {
                Console.WriteLine("Failed variable, gave value of " + evalVar);
            }

            //A string with a multiplication or division at the start
            try { Evaluator.Evaluate("/6 + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("Division at start exception worked");
            }

            //A string with division by 0
            try { Evaluator.Evaluate("4/0 + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("Division by zero exception worked");
            }

            //A string with less than 2 values in the value stack with a + or -
            try { Evaluator.Evaluate("5--6 + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("multiple minus exception worked");
            }

            //A string with a right parenthesis but no left parenthesis
            try { Evaluator.Evaluate("4+3) + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("right with no left parenthesis exception worked");
            }

            //A string with a right parenthesis containing fewer than 2 values trying to pop
            try { Evaluator.Evaluate("(+3) + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("less than two value parenthesis exception worked");
            }

            //A string with an unrelated phrase
            try { Evaluator.Evaluate("$6 + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("unknown symbol exception worked");
            }

            //multiple values left in the stack
            try { Evaluator.Evaluate("3 3 + 3", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("multiple values in stack at end exception worked");
            }

            //not enough values left in the stack at very end
            try { Evaluator.Evaluate("3 + 3 +", LookupSimple); }
            catch (ArgumentException e)
            {
                Console.WriteLine("not enough values at end of stack worked");
            }

        }

        public static int LookupSimple(String var)
        {
            if(var.Equals("a4"))
            {
                return 4;
            }

            else
            {
                return 0;
            }
        }
    }
}
