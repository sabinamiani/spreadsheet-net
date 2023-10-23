#include <iostream> 
#include "spreadsheet.h"
#include "DependencyGraph.h"

int main ()
{
   spreadsheet s("dep");
   //std::cout << s.dependencyCheck("A3", "=45+1") << std::endl;
   s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\":\"=45+1\"}", NULL);
   std::cout << s.getCellContents("A3") << std::endl;

   //std::cout << s.dependencyCheck("A1", "=B1") << std::endl;
   s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"=B1\"}", NULL);
   std::cout << s.getCellContents("A1") << std::endl;

   //std::cout << s.dependencyCheck("B1", "=A1") << std::endl;
   s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"B1\", \"contents\":\"=A1\"}", NULL);
   std::cout << s.getCellContents("B1") << std::endl;

   s.receivePacket("{\"requestType\": \"editCell\", \"cellName\": \"B1\", \"contents\":\"=A1\"}", NULL);
   std::cout << s.getCellContents("B1") << std::endl;

   return 0;

}