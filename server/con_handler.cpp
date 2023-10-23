#include "con_handler.h"

typedef boost::shared_ptr<con_handler> pointer;

//& in front of io_service?
///Constructs a data array for the con handler and initializes the socket
con_handler::con_handler(boost::asio::io_service& io): sock(io)
{
  data = static_cast<char*>(malloc(max_length * sizeof(*data)));
}

///Destructor to free the data buffer
con_handler::~con_handler()
{
  free(data);
}

/// creates the pointer of this object
pointer con_handler::create(boost::asio::io_service& io_service)
{
    return pointer(new con_handler(io_service));
}

//Returns the socket of the con handler
tcp::socket& con_handler::socket()
{
  return sock;
}

///Clears the data buffer of the con_handler
void con_handler::clearData()
{
  memset(data, 0, max_length * sizeof(data[0]));
}

///Handles writes *filler method*
void con_handler::handle_write(const boost::system::error_code& err, size_t bytes_transferred)
{
  if (!err) {
    std::cout << "Message sent from Server!"<< std::endl;
  } else {
    std::cerr << "error: " << err.message() << std::endl;
    sock.close();
  }
  
}

///Returns the data in the buffer
char *con_handler::getData()
{
  return data;
}


///Sets the userID of this client
void con_handler::setUsername(std::string name)
{
  username = name.substr(0, name.length() - 1);
}

std::string con_handler::getUsername()
{
  return username;
}


///Sets the spreadsheet of this client
void con_handler::setSpreadsheet(std::string name)
{
  sheetName = name;
}

///Returns the spreadsheet name
std::string con_handler::getSheetName()
{
  return sheetName;
}


///Returns a shared_from_this() pointer
boost::shared_ptr<con_handler> con_handler::sharedCon() 
{
  return shared_from_this();
}

int con_handler::getID()
{
  return id;
}

void con_handler::setID(int num)
{
  id = num;
}
