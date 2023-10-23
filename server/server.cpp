#include "server.h"
#include <fstream>
#include <iostream>
#include <filesystem>

//network_controller *control
server::server()
{
  spreadsheets = {};
  
  
/*
  //Load up spreadsheets here
  std::ifstream file("savedSpreadsheets.coco");
  std::vector<std::string> savedSheets;
  std::string s;
  while (getline(file, s))
  {
    savedSheets.push_back(s);
  }
  try
  { 
    for (std::string filename : savedSheets)
    {
      createSpreadsheet(filename);
    }
  }
  catch(int e)
  {
  } 
*/
}

server::server(const server&)
{
  
}

//Destroys the server
server::~server()
{
  free(controller);
}

///Initializes the network controller then runs the server
void server::start()
{
  boost::asio::io_service io;
  controller = new network_controller(io);
  controller->setServer(this);
  controller->start();
  std::string path = "saves/";
  for (const auto & entry : std::filesystem::directory_iterator(path))
  {
	  std::string name = entry.path();
	  name = name.substr(6, name.length() - 11);
	  createSpreadsheet(name);
  }
      
  io.run();
  std::cout << "Server Running" << std::endl;
}

void server::close()
{
std::cout << "server closing" << std::endl; 
  sheetChange.lock();
//  std::map<std::string, spreadsheet>::iterator itr;
//  std::ofstream file ("savedSpreadsheets.coco");
//  for(itr = spreadsheets.begin(); itr != spreadsheets.end(); ++itr)
//    {
//      std::cout << itr->first << std::endl;
//      std::cout << "inside shutdown loop" << std::endl;
//      itr->second.shutdown(itr->first);
//      itr++;
//    }
  //file.close();
  sheetChange.unlock();
  controller->serverClose();
}

///Returns a vector of all of the spreadsheets in the server
std::vector<std::string> server::getSpreadsheets()
{
  sheetChange.lock();
  std::vector<std::string> sheets;
  std::map<std::string, spreadsheet>::iterator itr;
  for(itr = spreadsheets.begin(); itr != spreadsheets.end(); ++itr)
    {
      sheets.push_back(itr->first);
    }
  sheetChange.unlock();
  return sheets;
}

///Sets the spreadsheet for the client and the spreadsheet
void server::setClient(std::string sheetName, con_handler::pointer socket)
{
    sheetChange.lock();
    spreadsheets[sheetName].addClient(socket);
    sheetChange.unlock();
    socket->setSpreadsheet(sheetName);
}

void server::clientDisconnected(con_handler::pointer socket)
{
  sheetChange.lock();
  spreadsheets[socket->getSheetName()].removeClient(socket);
  sheetChange.unlock();
}

///Creates a spreadsheet in the server
void server::createSpreadsheet(std::string sheetName)
{
    sheetName.erase(remove_if(sheetName.begin(), sheetName.end(), isspace), sheetName.end());
    sheetChange.lock();
    spreadsheet sheet(sheetName);
	sheet.load();
    spreadsheets.insert(std::pair<std::string, spreadsheet>(sheetName, sheet));
    sheetChange.unlock();
}

void server::handleEdit(std::string sheet, std::string info, con_handler::pointer socket)
{
  sheetChange.lock();
  std::cout << "The name of this sheet is: " << sheet << std::endl;
  spreadsheets[sheet].receivePacket(sheet, info, socket);
  sheetChange.unlock();
}