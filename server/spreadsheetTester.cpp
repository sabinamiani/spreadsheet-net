#include "spreadsheet.h"
#include "cell.h"
#include <iostream>
#include <exception>
#include <utility> 
#include <regex>

void test7();
void testLoad();
void revertUndo();
void revertUndoHelper(std::string val1, std::string val2, int testNum, spreadsheet &s);
void getVarTest();
void simpleUndo();
void jensenTest();
void simpleRevert ();
void simpleAdd ();
void testParse();
void saveTest ();

int main ()
{
    std::cout << "Tests begin" << std::endl;
    testLoad();
    //jensenTest();
    //test7();
    //revertUndo();
    //simpleUndo();
    //simpleRevert ();

//    simpleAdd ();

  // testParse();

//saveTest ();

    return 0;
}

void saveTest ()
{
  spreadsheet s("parse");
  s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"=45+1\"}", NULL);
  s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"2\"}", NULL);
  s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A4\", \"contents\":\"3\"}", NULL);
  s.receivePacket("{\"requestType\": \"undo\"}", NULL);

  s.save();
}


void testParse()
{
  spreadsheet s("parse");
  s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"=45+1\"}", NULL);
  std::cout << s.getCellContents("A2") << std::endl;

}


void simpleAdd ()
{
    spreadsheet s("add");
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"Table\"}", NULL);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\":\"A2\"}", NULL);
    std::cout << s.getCellContents("A2") << std::endl;
    std::cout << s.getCellContents("A3") << std::endl;
}

void simpleUndo()
{
    spreadsheet s("simpleUndo");
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"1\"}", NULL);
    revertUndoHelper("1", "", 1, s);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\":\"2\"}", NULL);
    revertUndoHelper("1", "2", 2, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("1", "", 3, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("", "", 4, s);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"1\"}", NULL);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"2\"}", NULL);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("1", "", 5, s);
}

void jensenTest()
{
    spreadsheet a("jensenTest");
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"1\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"2\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"3\"}", NULL);
    a.save();

    spreadsheet b("jensenTest");
    b.load();
    b.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A2\"}", NULL);
    b.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A1\"}", NULL);

    if(b.getCellContents("A1") == "1"
            && b.getCellContents("A2") == "")
        std::cout << "Jensen test works 1/3" << std::endl;

    b.receivePacket("{\"requestType\": \"undo\"}", NULL);
    b.receivePacket("{\"requestType\": \"undo\"}", NULL);

    if(b.getCellContents("A1") == "2"
            && b.getCellContents("A2") == "3")
        std::cout << "Jensen test works 2/3" << std::endl;

    b.save();

    b.receivePacket("{\"requestType\": \"undo\"}", NULL);
    b.receivePacket("{\"requestType\": \"undo\"}", NULL);
    //a.receivePacket("{\"requestType\": \"undo\"}", NULL);

    if(b.getCellContents("A1") == "1"
            && b.getCellContents("A2") == "")
        std::cout << "Jensen test works 3/3" << std::endl;

    std::cout << b.getCellContents("A1") << " " << b.getCellContents("A2") << std::endl;
}

void simpleRevert ()
{
  spreadsheet g("simpleG");
  g.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"1\"}", NULL);
  g.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A1\"}", NULL);
  g.receivePacket("{\"requestType\": \"undo\"}", NULL);
  g.receivePacket("{\"requestType\": \"undo\"}", NULL);

  if(g.getCellContents("A1") == "")
    std::cout << "simpleG passes" << std::endl;
}


void testLoad()
{
    spreadsheet a("testLoad");
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A5\", \"contents\":\"1\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A6\", \"contents\":\"2\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A5\", \"contents\":\"3\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A7\", \"contents\":\"4\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A8\", \"contents\":\"5\"}", NULL);
    a.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A7\", \"contents\":\"6\"}", NULL);
    a.save();

    spreadsheet b("testLoad");
    b.load();
    if(b.getCellContents("A5") == "3"
            && b.getCellContents("A6") == "2"
            && b.getCellContents("A7") == "6"
            && b.getCellContents("A8") == "5")
        std::cout << "Test save and load works" << std::endl;
}

void test7()
{
    spreadsheet s("test7");

    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A5\", \"contents\":\"1\"}", NULL);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A6\", \"contents\":\"2\"}", NULL);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    if(s.getCellContents("A6") == "")
        std::cout << "TEST 7.0 PASSED" << std::endl;
    std::cout << s.getCellContents("A6") << std::endl;
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    if(s.getCellContents("A5") == "")
        std::cout << "TEST 7.5 PASSED" << std::endl;
    std::cout << s.getCellContents("A5") << std::endl;
}

void revertUndo()
{
    spreadsheet s("revertUndo");
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"Table\"}", NULL);
    revertUndoHelper("Table", "", 1, s);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\":\"A2\"}", NULL);
    revertUndoHelper("Table", "A2", 2, s);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"Text\"}", NULL);
    revertUndoHelper("Text", "A2", 3, s);
    s.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A3\"}", NULL);
    revertUndoHelper("Text", "", 4, s);
    s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A2\", \"contents\":\"Data\"}", NULL);
    revertUndoHelper("Data", "", 5, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Text", "", 6, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Text", "A2", 7, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Table", "A2", 8, s);
    s.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A2\"}", NULL);
    revertUndoHelper("", "A2", 9, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Table", "A2", 10, s);
    s.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A2\"}", NULL);
    revertUndoHelper("", "A2", 11, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Table", "A2", 12, s);
    s.receivePacket("{\"requestType\": \"revertCell\", \"cellName\": \"A2\"}", NULL);
    revertUndoHelper("", "A2", 13, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Table", "A2", 14, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("Table", "", 15, s);
    s.receivePacket("{\"requestType\": \"undo\"}", NULL);
    revertUndoHelper("", "", 16, s);
}

void revertUndoHelper(std::string val1, std::string val2, int testNum, spreadsheet &s)
{
     if(s.getCellContents("A2") == val1 && s.getCellContents("A3") == val2)
     {
         std::cout << "Row " << testNum << " passed" << std::endl;
     }
     else
     {
         std::cout << "Row " << testNum << " failed ";
         std::cout << "expected " << val1 << " and " << val2;
         std::cout << " got " << s.getCellContents("A2") << " and " << s.getCellContents("A3") << std::endl;
     }

}

void getVarTest ()
{
    /*
  spreadsheet d("d");
  std::vector<std::string> vars = d.getVariables("=d3+7y-read487/98*(testing12)");

  for (std::string s : vars)
    std::cout << s << std::endl;
    */

}
