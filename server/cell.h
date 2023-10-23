#ifndef CELL_H
#define CELL_H

#include <string>

class cell
{
   public:
    cell(std::string content, std::string name);

    //returns content of cell
    std::string getContent();

    std::string getName();
    void setName(std::string name);

    //sets contents of cell
    void setContent(std::string content);

    //get content of previous cell
    std::string getPrevContent();

    //sets contents of previous cell
    void setPrevContent(std::string content);

   private:
    //the actual content of the cell
    std::string content;
    std::string name;
    std::string prevContent;
};


#endif
