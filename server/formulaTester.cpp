#include "spreadsheet.h"
#include "cell.h"
#include <iostream>
#include <exception>
#include <utility>
#include <regex>

int main ()
{
    spreadsheet s("formula");
/*
    // should pass
    std::cout << "below should pass" << std::endl;
    std::cout << s.evaluateFormula("=23-5") << std::endl;
    std::cout << s.evaluateFormula("=23-r4") << std::endl;
    std::cout << s.evaluateFormula("=s3+(qwertyuiop1234567890)/0*7") << std::endl;
    std::cout << s.evaluateFormula("=((9)+(5)*11)") << std::endl;
    std::cout << s.evaluateFormula("=23+-5") << std::endl;
    std::cout << s.evaluateFormula("=23*-5") << std::endl;



    // should fail
    std::cout << "below should fail" << std::endl;
    std::cout << s.evaluateFormula("=43r   *    2") << std::endl;
    std::cout << s.evaluateFormula("==5") << std::endl;
    std::cout << s.evaluateFormula("=23-*5") << std::endl;
    std::cout << s.evaluateFormula("=AS25 = 5") << std::endl;
    std::cout << s.evaluateFormula("=(5))") << std::endl;
    std::cout << s.evaluateFormula("=((9") << std::endl;
    std::cout << s.evaluateFormula("=7y^2") << std::endl;
    std::cout << s.evaluateFormula("=235+") << std::endl;
*/


//    std::cout << s.evaluateFormula("=3.5") << std::endl; 
    std::cout << s.evaluateFormula("= B2 + 3.5") << std::endl; 

    return 0;
}
