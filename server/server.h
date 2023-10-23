
#ifndef SERVER_H
#define SERVER_H

#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <iostream>
#include <vector>
#include <string>
#include <map>
#include <memory>
#include <mutex>
#include "spreadsheet.h"
#include "con_handler.h"
#include "network_controller.h"


using namespace boost::asio;
using ip::tcp;

class network_controller;

class server
{
  
 private:
  ///List of all spreadsheets in the server
  std::map<std::string, spreadsheet> spreadsheets;
  std::mutex sheetChange;

  ///network_controller that manages all networking functions
  network_controller *controller;
 
  void start_accept();
  
 public:
  server();
  ~server();
  // server(network_controller *control);
  server(const server&);

  void start();
  void close();
  
  std::vector<std::string> getSpreadsheets();
  void setClient(std::string sheetName, con_handler::pointer socket);
  void createSpreadsheet(std::string sheetName);
  void handleEdit(std::string sheet, std::string info, con_handler::pointer socket);
  void clientDisconnected(con_handler::pointer socket);
  
};

#endif
