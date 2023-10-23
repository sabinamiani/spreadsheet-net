#include "cell.h"

cell::cell(std::string content, std::string name)
{
  this->content = content;
  this->name = name;
  prevContent = "";
}

std::string cell::getContent()
{
  return content;
}

void cell::setContent(std::string content)
{
  this->content = content;
}

std::string cell::getPrevContent()
{
  return prevContent;
}

void cell::setPrevContent(std::string content)
{
  this->prevContent = content;
}

std::string cell::getName()
{
    return name;
}
void cell::setName(std::string name)
{
    this->name = name;
}
