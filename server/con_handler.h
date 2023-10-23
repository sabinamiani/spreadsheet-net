
#ifndef CON_HANDLER_H
#define CON_HANDLER_H

#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <string>

using namespace boost::asio;
using ip::tcp;

class con_handler : public boost::enable_shared_from_this<con_handler>
{
 public:
  //Length used to modify size of buffers
  enum { max_length = 1024 };

 private:
  //Socket that holds our client connection
  tcp::socket sock;
  //ID of client
  std::string username;
  //Data buffer
  char *data;
  //Name of the spreadsheet
  std::string sheetName;

  int id;


 public:
  //Type definition, for shared pointers of this object
  typedef boost::shared_ptr<con_handler> pointer;
  con_handler(boost::asio::io_service& io);
  ~con_handler();

  static pointer create(boost::asio::io_service& io_service);
  tcp::socket& socket();
  char *getData();
  void setUsername(std::string name);
  std::string getUsername();
  void setSpreadsheet(std::string name);
  std::string getSheetName();
  boost::shared_ptr<con_handler> sharedCon();
  void clearData();
  int getID();
  void setID(int num);

  void handle_write(const boost::system::error_code& err, size_t bytes_transferred);
};

#endif
