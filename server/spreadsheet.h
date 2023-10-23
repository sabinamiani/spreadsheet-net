
#ifndef SPREADSHEET_H
#define SPREADSHEET_H

//#include <mutex>
#include <string>
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include <forward_list>
#include <stack>
#include <cctype>
#include <algorithm>
#include "cell.h"
#include "DependencyGraph.h"
#include "json/json2/single_include/nlohmann/json.hpp"
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "con_handler.h"

class spreadsheet
{

 public: 


//Checks for invalid dependencies.
  //Returns true of there are no circular dependency errors
  //Return false if there are circular dependencies.
  bool dependencyCheck (std::string name, std::string text);


  //// constructors ////

  // empty constructor used for map initializations 
  spreadsheet();
  
  //blank copy constructor for mutex use only 
  // -- requires reference when passing spreadsheet as parameter 
  spreadsheet(const spreadsheet&);

  // The default constructor for Spreadsheet, which uses the base validator, normalizer, 
  //  and gives the version name Default.
  spreadsheet(std::string path);

  //// methods ////

  // Returns the contents of the cell, unless the requested cell name is incorrect, then it will throw an 
  //  InvalidNameException. If the cell is empty, returns a blank string
  // <param name="name">The cell from which contents are being pulled</param>
  // <returns>the contents of the cell</returns>
  std::string getCellContents(std::string name);
  std::string getCellPrevContents(std::string name);

  //General recieve will parse json and call appropriate method
  void receivePacket(std::string name, std::string packet, con_handler::pointer client);

  // Adds client connection to a vector of clients held by each spreadsheet object 
  void addClient (con_handler::pointer connection);

  // Removes client connection from the vector of clients held by each spreadsheet object 
  void removeClient(con_handler::pointer connection);

  // Returns all of the cells in the spreadsheet that are not empty
  // <returns>A list of every cell in the spreadsheet that is non-empty</returns>
  std::vector<std::string> getNamesOfAllNonemptyCells();

  // Saves the file in JSON format as the spreadsheet name, throwing exceptions if there are any issues found
  void save(std::string name);

  // loads saved file of previous spreadsheets 
  void load();

  //Process of shutting down the server
  void shutdown (std::string name);
  
  // returns true if the formula is valid
  // else the formula is invalid and invalid request error is sent to client
  bool evaluateFormula(std::string formula);

 private:

  /// constructors ///
  
  

  //// variables //// 

  // each spreadsheet holds a list of current users
  std::vector<int> users;

  // dependency graph for cyclical error checking 
  DependencyGraph dGraph;

  // spreadsheet name -- must be unique 
  std::string spreadsheetName;
  
  //Mutex Lock for socket changes
  //std::mutex socketsChange;

  //Mutex Lock for cell changes
  //std::mutex sheetsChange;

  // spreadsheet being tracked 
  std::unordered_map<std::string, cell> sheet;

  //list of sockets
  std::vector<con_handler::pointer> sockets;

  //Undo stack
  std::stack<cell> undoStack;
  
  //Contents of the cell that is attempting to be set
  std::string startContents;

  //// methods ////

  

  //Sends all user connected to spreadsheet a json packet
  void sendCellUpdate(cell cellToSend);
 
  // evaluates cell value as formula for variable names 
  std::vector<std::string> getVariables(const std::string value);

  //Process undo
  void undoChange(con_handler::pointer client);

  //Process Revert
  void processRevert(std::string cellName, con_handler::pointer client);

  // process cell selection packets 
  void processSelection(std::string cellName, con_handler::pointer client);

  //call back for send -- blank on purpose 
  void sendCallback ();

  //add a change to undo stack
  void addUndoChange (cell cell);

  // Traverses from the given start through the list of changed
  bool visit (std::string start, std::string name, std::unordered_set<std::string> visited, std::forward_list<std::string> changed);


  // helper method to send error request message to an individual client 
  void sendRequestError (cell cell, con_handler::pointer client);

 protected: 

  // Sets the requested cell's contents to the number provided
  // If the name is invalid or null, throws an InvalidNameException
  void setCellContents(std::string cellName, std::string text, con_handler::pointer client);

  //same as other set cell but uses a json packet
  void setCellContents(std::string packet, con_handler::pointer client);

  // Returns a list of all of the immediate dependents of the selected cell
  // <param name="name">the name of the selected cell</param>
  // <returns>a list of all the immediate dependents of the cell</returns>
  std::vector<std::string> getDirectDependents(std::string start, std::string cellName);

  //Returns a linked list containing all the cells that need to be recalculated
  bool getCellsToRecalculate (std::string name);

  //Returns a linked list containing all the cells that need to be recalculated from the given list.
  bool getCellsToRecalculate (std::unordered_set<std::string> nameList);


};
#endif
