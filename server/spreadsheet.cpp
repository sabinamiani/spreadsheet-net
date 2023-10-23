#include "spreadsheet.h"
#include "cell.h"
#include <iostream>
#include <exception>
#include <utility> 
#include <regex>
#include <fstream>
#include <iterator>


spreadsheet::spreadsheet ()
{
  spreadsheetName = "Test";
  sheet = {};
}

spreadsheet::spreadsheet(const spreadsheet& sheetCopy)
{
  // blank on purpose -- used for mutex 
  *this = sheetCopy;
}

spreadsheet::spreadsheet(std::string pathname)
{
  // initializing variables 
  spreadsheetName = pathname;
  sheet = {};
}

std::string spreadsheet::getCellContents(std::string cellName)
{
  if(sheet.find(cellName) != sheet.end())
  {
    return sheet.find(cellName)->second.getContent();
  } 
  else
  {
    return "";
  }
}

std::string spreadsheet::getCellPrevContents(std::string cellName)
{
  if(sheet.find(cellName) != sheet.end())
  {
    return sheet.find(cellName)->second.getPrevContent();
  }
  else
  {
    return "";
  }
}

std::vector<std::string> spreadsheet::getNamesOfAllNonemptyCells()
{
  std::vector<std::string> cells;

  for(auto key : sheet)
  {
    cells.push_back(key.first);
  }
  return cells;
}

void spreadsheet::addUndoChange (cell cellToAdd)
{
  undoStack.push(cellToAdd);
}

void spreadsheet::save(std::string fileName)
{
  std::vector<std::string> cells = getNamesOfAllNonemptyCells();

  std::vector<nlohmann::json> packets;
  for(int i = 0; i < cells.size(); i++)
  {
    nlohmann::json jobj;
    jobj["key"] = "setCell";
    jobj["cellName"] = cells[i];
    jobj["cellContent"] = getCellContents(cells[i]);
    jobj["cellPrevContent"] = getCellPrevContents(cells[i]);
    packets.push_back(jobj);
  }

  std::string fileToName = "saves/" + fileName + ".coco";
  std::ofstream file (fileToName); //"~/savedSpreadsheets/" + 
  
  for(int i = 0; i < packets.size(); i++)
  {
    std::string jsonString = packets[i].dump();
    file << jsonString << std::endl;
  }

  std::stack<cell> copyUndo = undoStack;

  while(copyUndo.size() != 0)
  {
     cell c = copyUndo.top();

     nlohmann::json jobj;
     jobj["key"] = "undo";
     jobj["cellName"] = c.getName();
     jobj["cellContent"] = c.getContent();
     jobj["cellPrevContent"] = c.getPrevContent();
     file << jobj.dump() << std::endl;

     copyUndo.pop();
  }

  file.close();
}

void spreadsheet::load()
{
    std::ifstream file("saves/" + spreadsheetName + ".coco");
    std::vector<std::string> packets;
    std::string s;

    while (getline(file, s))
    {
        packets.push_back(s);
    }

    for(int i = packets.size() - 1; i >= 0; i--)
    {
        nlohmann::json jObject = nlohmann::json::parse(packets[i]);
        cell c(jObject["cellContent"], jObject["cellName"]);
        c.setPrevContent(jObject["cellPrevContent"]);

        if(jObject["key"] == "undo")
        {
            addUndoChange(c);
        }
        else if (jObject["key"] == "setCell")
        {
            std::pair<std::string, cell> p (jObject["cellName"], c);
            sheet.insert(p);
        }
    }
}

void spreadsheet::receivePacket(std::string name, std::string packet, con_handler::pointer client)
{
    std::cout << packet << std::endl;

    nlohmann::json jObject = nlohmann::json::parse(packet);
    std::string key = jObject["requestType"];

    if(key == "editCell")
    {
      setCellContents(packet, client); 
    }

    else if(key == "revertCell")
    {
        try
        {
            processRevert(jObject["cellName"], client);
        }
        catch (int e)
        {
            //send error to client
            nlohmann::json errorJson;
            errorJson["messageType"] = "requestError";
            errorJson["cellName"] = jObject["cellName"];
            errorJson["message"] = "Cannot revert, cell no previous value";
            client->socket().async_write_some(boost::asio::buffer(errorJson.dump(), 1024), boost::bind(&spreadsheet::sendCallback, this));
        }
    }
    else if(key == "selectCell")
    {
        processSelection(jObject["cellName"], client);
    }
    else if(key == "undo")
    {
      undoChange(client);
    }
  
    save(name);
}

void spreadsheet::processSelection (std::string cellName, con_handler::pointer client)
{
  // send following to client 
  // {messageType: "cellSelected", cellName: “<cell name>”, selector: <ID # of selector>, selectorName: “<name of selector>”}
  
  nlohmann::json jobj;
  jobj["messageType"] = "cellSelected";
  jobj["cellName"] = cellName;
  jobj["selector"] = client->getID();
  jobj["selectorName"] = client->getUsername();

  std::string packet = jobj.dump();

  for(int i = 0; i < sockets.size(); i++)
  {
    sockets[i]->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));
  }

}

void spreadsheet::undoChange (con_handler::pointer client)
{
  if(undoStack.size() <=0)
  {
      //send error back to client
      nlohmann::json jobj;
      jobj["messageType"] = "requestError";
      jobj["cellName"] = "Spreadsheet";
      jobj["message"] = "Invalid undo. Nothing left to undo";

      std::string packet = jobj.dump();

      client->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));
      return;
  }
  cell change = undoStack.top();

  //set cell previous val to current value
  auto fkey = sheet.find(change.getName());
  if(sheet.size() != 0 && fkey != sheet.end())
  {
    //set previous value in cell for Revert
    fkey->second.setPrevContent(change.getPrevContent());
    fkey->second.setContent(change.getContent());

    // if text is empty, remove cell from sheet
    if(change.getContent() == "")
    {
      sheet.erase(change.getName());
    }
  }
  else
  {
    std::pair<std::string, cell> p(change.getName(), change);
    sheet.insert(p);
  }
  sendCellUpdate(change);
  undoStack.pop();
}

void spreadsheet::processRevert(std::string cellName, con_handler::pointer client)
{
    auto key = sheet.find(cellName);
    if(sheet.size() != 0 && key != sheet.end())
    {
        //Cell is in sheet
        cell c(key->second.getContent(), key->second.getName());
        c.setPrevContent(key->second.getPrevContent());
        if(c.getContent() != c.getPrevContent())
        {
            addUndoChange (c);
            key->second.setContent(key->second.getPrevContent());

            sendCellUpdate(key->second);
        }
        else
        {
            //send error to client
            nlohmann::json jobj;
            jobj["messageType"] = "requestError";
            jobj["cellName"] = cellName;
            jobj["message"] = "Cannot revert cell no previous value";

            std::string packet = jobj.dump();

            client->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));
        }
    }
    //else cell is not in sheet do nothing
}

std::vector<std::string> spreadsheet::getVariables(const std::string value)
{
  std::vector<std::string> vars;

  // parse and read strings between =,+,-,/,*,(,)
  std::regex delims ("[a-zA-Z]+[0-9]+|[0-9]+[a-zA-Z]+|[^\\=\\+\\-\\(\\)\\*\\.\\/0-9]");
  std::regex_iterator<std::string::const_iterator> itr (value.begin(), value.end(), delims);
  std::regex_iterator<std::string::const_iterator> end;

  while (itr != end)
  {
    vars.push_back(itr->str());
    itr++;
  }

  // return list of variables 
  return vars;
}

bool spreadsheet::evaluateFormula(std::string formula)
{
  //  validate formula 
  //  mathematical operations -- +,-,*,/,(,), - done
  //  contains no invalid tokens - done
  //  if variables are used, check for validity - maybe done
  //  no operator at end of expression - done
  //  must have at least one double or variable - done
  //  same left and right # parenthesis - done
  //  negative signs are valid  - done
  
  // if not a formula return true 
  if(formula.length() == 0 || formula.at(0) != '=') return true;

  formula = formula.substr(1);

  //remove whitespace in the formula string
  formula.erase(std::remove_if(formula.begin(), formula.end(), ::isspace), formula.end());

  bool num_or_var = false;

  
  std::vector<std::string> varsInForm = getVariables(formula);
  std::regex validityCheck ("^[a-zA-Z]+[0-9]+$");
  for(std::string v : varsInForm)
  {
      if(v != "")
      {
	  if(!std::regex_match(v, validityCheck))
	  {
	     return false;
	  }
	  num_or_var = true;
      }
  }
  
  std::string prev = "";
  int numParen = 0;
  
  //Not 100% sure that this regex works
  std::regex e ("[a-zA-Z0-9\\*\\/\\+\\-\\(\\)\\.]");
  std::regex symbols("[\\*\\/\\+\\-]");
  std::regex numbers("[0-9]*(\\.[0-9]+)?");
  std::regex decimal("[\\.]");

  //go through formula, and add items to the vector
  for(int i = 0; i < formula.length(); i++)
    {
      //TODO: figure out how to find and insert variables and numbers (longer than one)
      if(formula.substr(i, 1) == "(")
	{
	  numParen++;
	  prev = formula.substr(i, 1);
	}
      else if(formula.substr(i,1) == ")")
	{
	  numParen--;
	  prev = formula.substr(i, 1);
	}
      //Check to see if value is not alphanumeric or one of the known operators, throw error if it isn't
      else if(!std::regex_match(formula.substr(i,1),e))
	{
	  return false;
	}
      //Check to see if there are repeated operators
      else if(std::regex_match(formula.substr(i,1),symbols) && std::regex_match(prev,symbols))
	{
	  if(formula.substr(i,1) != "-")
	    {
	      return false;
	    }
	  else
	    {
	      prev = formula.substr(i, 1);
	    }
	}
      //Check if the string is a number
      else if(std::regex_match(formula.substr(i,1),numbers) || (std::regex_match(formula.substr(i,1),decimal) && std::regex_match(prev,numbers)) 
	|| (std::regex_match(prev,decimal) && std::regex_match(formula.substr(i,1),numbers)))
	{
	  num_or_var = true;
	  prev = formula.substr(i, 1);
	}
      //check to see if previous was one of the operators, if it was and the current substring 
      //is an operator, give error.
      else
	{
	  prev = formula.substr(i, 1);
	}
    }
  
  if(std::regex_match(formula.substr(formula.length() - 1,1),symbols) || formula.substr(formula.length() - 1,1) == "(")
    {
      return false;
    }
  if(numParen != 0)
    {
      return false;
    }
  if(!num_or_var)
    {
      return false;
    } 
  return true;
}

void spreadsheet::sendCellUpdate(cell cellToSend)
{
    nlohmann::json jobj;
    jobj["messageType"] = "cellUpdated";
    jobj["cellName"] = cellToSend.getName();
    jobj["contents"] = cellToSend.getContent();

    std::string packet = jobj.dump();

    for(int i = 0; i < sockets.size(); i++)
    {
        sockets[i]->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));
    }
}

void spreadsheet::setCellContents(std::string packet, con_handler::pointer client)
{
    //deserialize json
    nlohmann::json jObject = nlohmann::json::parse(packet);

    std::string cellName = jObject["cellName"];
    std::string text = jObject["contents"];

    setCellContents(cellName, text, client);
}

void spreadsheet::setCellContents(std::string cellName, std::string text, con_handler::pointer client)
{
  // check text is valid 
  // any string without "=" is fine
  // strings beginning with "=" need to be evaluated as a valid formula

  if(evaluateFormula(text) && dependencyCheck(cellName, text))
  {
         // if name exists in sheet, update value
         // else create new cell
         // update dependencies

         auto fkey = sheet.find(cellName);
         if(sheet.size() != 0 && fkey != sheet.end())
         {
           std::string oldContent = fkey->second.getContent();
           cell copy(oldContent, cellName);
           copy.setPrevContent(fkey->second.getPrevContent());
           addUndoChange(copy);

           //set previous value in cell for Revert
           fkey->second.setPrevContent(oldContent);
           fkey->second.setContent(text);

           // if text is empty, remove cell from sheet
           if(text == "")
           {
            sheet.erase(cellName);
           }
         }
         else
         {
           cell undoCell("", cellName);
           addUndoChange(undoCell);

           cell c(text, cellName);
           std::pair<std::string, cell> p (cellName, c);
           sheet.insert(p);
         }
    
         // send packet to user
         cell cellSendGood(text, cellName);
         sendCellUpdate(cellSendGood);
  }
  else
  {

    cell sendCell(text, cellName);
    sendRequestError(sendCell, client);
  }

}

void spreadsheet::sendRequestError (cell cell, con_handler::pointer client)
{
      nlohmann::json jobj;
      jobj["messageType"] = "requestError";
      jobj["cellName"] = cell.getName();
      jobj["message"] = "There was an error with the formula, edit did not go through";
    
      std::string packet = jobj.dump();
	    
      client->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));
}

void spreadsheet::addClient (con_handler::pointer connection)
{
    std::vector<std::string> allCells;
    allCells = getNamesOfAllNonemptyCells();

    //send cell contents
    for(int i = 0; i < allCells.size(); i++)
    {
        auto fkey = sheet.find(allCells[i]);
        std::string content = fkey->second.getContent();

        nlohmann::json jobj;
        jobj["messageType"] = "cellUpdated";
        jobj["cellName"] = allCells[i];
        jobj["contents"] = content;

        std::string packet = jobj.dump();  
        packet += "\n";   
        connection->socket().async_write_some(boost::asio::buffer(packet, 1024), boost::bind(&spreadsheet::sendCallback, this));   
    }

    //socketsChange.lock();
    sockets.push_back(connection);
    //socketsChange.unlock();

    //then send user id
    connection->socket().async_write_some(boost::asio::buffer(connection->getID() + "\n\n", 1024), boost::bind(&spreadsheet::sendCallback, this));
}

void spreadsheet::removeClient(con_handler::pointer connection)
{
  //socketsChange.lock();
    for(int i = 0; i < sockets.size(); i++)
    {
      if(sockets[i]->getUsername() == connection->getUsername())
      {
	sockets.erase(sockets.begin() + i);
      }
    }
    //socketsChange.unlock();
}

void spreadsheet::sendCallback ()
{

}

std::vector<std::string> spreadsheet::getDirectDependents(std::string start, std::string cellName)
{
  // if name has formula contents, return variables 
  // else, cell has no dependents 
  
  auto fkey = sheet.find(cellName);
  if(cellName == start)
  {
	return getVariables(startContents);
  }
  else if(sheet.size() != 0 && fkey != sheet.end() && fkey->second.getContent().at(0) == '=')
  {
    std::vector<std::string> dependents = getVariables(fkey->second.getContent());
    return dependents;
  }
  else 
  {
    return std::vector<std::string>();
  }
}

bool spreadsheet::getCellsToRecalculate (std::unordered_set<std::string> nameList)
{
  std::forward_list<std::string> changed;
  std::unordered_set<std::string> visited;
  bool returnError = true;

  for (std::string name : nameList)
  {
    auto fkey = visited.find(name);
    //sheet.size() != 0 &&  maybe????
    if (fkey == visited.end())
    {
      returnError = visit(name, name, visited, changed);
    }
  }

  return returnError;
}

bool spreadsheet::getCellsToRecalculate (std::string cellName)
{
  std::unordered_set<std::string> cellRecalc ( {cellName} );

  return getCellsToRecalculate(cellRecalc);
}

bool spreadsheet::visit (std::string start, std::string cellName, std::unordered_set<std::string> visited, std::forward_list<std::string> changed)
{
	bool returnBool = true;
  visited.insert(cellName);

std::vector<std::string> dependents = getDirectDependents(start, cellName);

  for (std::string n : dependents)
  {
    if (n == start)
    {
	  return false;
    }
    
    auto fkey = visited.find(n);
    //sheet.size() != 0 && maybe ????
    if (fkey == visited.end())
    {
      returnBool = visit (start, n, visited, changed);
    }
  }

  changed.push_front(cellName);
  return returnBool;

}

bool spreadsheet::dependencyCheck (std::string cellName, std::string text)
{
  //Checks to see if cell content is a formula

    if (text.length() > 0 && text.at(0) == '=')
    {
	  startContents = text;
	  
           std::vector<std::string> oldDependees = dGraph.getDependees(cellName);  
           std::vector<std::string> form = getVariables(text);
           std::unordered_set<std::string> formVars (form.begin(), form.end());
	  
	  if(form.size() == 0)
	  {
		  return true;
	  }
           auto fkey = formVars.find(cellName);
           if (formVars.size() != 0 && fkey != formVars.end())
           {
	     //throw a circular dependency exception
	     //sheetsChange.unlock();
	     return false;
           }
	
	   if(!getCellsToRecalculate(cellName))
	   {
	      //sheetsChange.unlock();
	      return false;
	   } 
	   else
            {
	      dGraph.replaceDependees(cellName, getVariables(text));
            }
    }

    //Clears old dependees after change
    else
    {
      std::vector<std::string> list;
      dGraph.replaceDependees(cellName, list);
    }

    return true;
}

void spreadsheet::shutdown (std::string name)
{
  std::cout << "shutdown called" << std::endl;
  std::cout << "spreadsheet size is " << getNamesOfAllNonemptyCells().size() << std::endl;
  save(name);

  for(int i = 0; i < sockets.size(); i++)
  {
    removeClient(sockets[i]);
  }
}
