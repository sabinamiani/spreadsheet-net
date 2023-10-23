

#ifndef NETWORK_CONTROLLER_H
#define NETWORK_CONTROLLER_H

#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <string>
#include <sstream>
#include <iostream>
#include <vector>
#include <memory>
#include <mutex>
#include <thread>
#include <csignal>
#include "server.h"
#include "con_handler.h"


using namespace boost::asio;
using namespace boost::placeholders;
using ip::tcp;

class server;

class network_controller
{

 private:
  //Listener that accepts client connections
  tcp::acceptor listener;
  //Server that the controller pulls from
  server *serv;
  int idCounter;
  std::mutex counter;

  void handle_accept(con_handler::pointer connection, const boost::system::error_code& err);
  void start_accept();
  void clientConnected(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection);
  void spreadsheetAllocate(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection);
  void handleWrite(const boost::system::error_code& err, size_t bytes_transferred);
  void handleRecieve(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection);
  
 public:
  /*server& ser*/
  network_controller();
  ~network_controller();
  network_controller(boost::asio::io_service& io);
  void start();
  void setServer(server *ser);
  void serverClose();
};


#endif
