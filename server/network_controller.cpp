#include "network_controller.h"


/*server& ser*/
///Creates a network controller and initializes the listener that accepts sockets
network_controller::network_controller(boost::asio::io_service& io) : listener(io, tcp::endpoint(tcp::v4(), 1100))
{
  idCounter = 1;
}

network_controller::~network_controller()
{
  
}

void network_controller::serverClose()
{
  std::cout << "\n" << "Server is shutting down.." << std::endl;
  listener.get_io_service().stop();
}

///Starts the acceptance thread
void network_controller::start()
{
  std::cout << "Server is running" << std::endl;
  start_accept(); 
}



///Fires when a client connects, starts the handshake after confirming the client is connected
void network_controller::handle_accept(con_handler::pointer connection, const boost::system::error_code& err)
{
  //If there are no errors, we wait for the client to send a username
  if (!err) 
    {
      connection->socket().async_read_some(
			   boost::asio::buffer(connection->getData(), 1024),
			   boost::bind(&network_controller::clientConnected,
				       this,
				       boost::asio::placeholders::error,
				       boost::asio::placeholders::bytes_transferred, 
				       connection));
    }
  else
    {
      connection->socket().close();
      std::cerr << "Error Occurred: " << err.message() << std::endl;
      return;
    }
  //Start listening for clients again
  start_accept();
}


///Listens for clients and assigns them a con_handler to represent them
void network_controller::start_accept()
{
  // socket
  con_handler::pointer connection = con_handler::create(listener.get_io_service());
  
  // asynchronous accept operation and wait for a new connection.
  listener.async_accept(connection->socket(),
    boost::bind(&network_controller::handle_accept, this, connection,
		boost::asio::placeholders::error));
}


///Fires when a client sends their username. Sets client ID and sends client list of spreadsheets
void network_controller::clientConnected(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection)
{
  //If an error occurs, we pring a message and close the socket
  if(err)
    {
      std::cerr<< "Error Occured: " << err.message() << std::endl;
      connection->socket().close();
      return;
    }

  
  //We get the data from the con_handler and set their username
  std::string username(connection->getData());
  connection->setUsername(username);
  std::cout << connection->getUsername() << " has connected" << std::endl;

  //We get a list of spreadsheets from the server and compile it into a string
  std::string spreadsheetList = "";
  std::vector<std::string> spreadsheets = serv->getSpreadsheets();
  for(int i = 0; i < spreadsheets.size(); ++i)
    {
      spreadsheetList = spreadsheetList + spreadsheets[i] + '\n';
    }
  spreadsheetList = spreadsheetList + '\n';


  //We send the string to the client
  connection->socket().async_write_some(
					boost::asio::buffer(spreadsheetList, 1024),
					boost::bind(&network_controller::handleWrite,
						    this,
						    boost::asio::placeholders::error,
						    boost::asio::placeholders::bytes_transferred));
  
  //Then we wait for the client to respond
  connection->clearData();
  connection->socket().async_read_some(
				       boost::asio::buffer(connection->getData(), 1024),
				       boost::bind(&network_controller::spreadsheetAllocate,
						   this,
						   boost::asio::placeholders::error,
						   boost::asio::placeholders::bytes_transferred,
						   connection));
  
}

//Fires when the client chooses a spreadsheet. Assigns a client to a spreadsheet, or creates one if one hasn not been made
void network_controller::spreadsheetAllocate(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection)
{
  //If an error occurs, we print a message and close the socket
  if(err)
    {
      std::cout << "Error Occurred: " << err.message() << std::endl;
      connection->socket().close();
      std::cout << connection->getUsername() << " has disconnected" << std::endl;
      return;
    }
  
  //We get the data from the client and make their choice a string
  std::string clientSheet(connection->getData());
  clientSheet = clientSheet.substr(0, clientSheet.size() - 1);
  bool sheetExists = false;
  
  //We pull the spreadsheets again and go through them to see if the client choice exists already
  std::vector<std::string> spreadsheets = serv->getSpreadsheets();
  for(int i = 0; i < spreadsheets.size(); ++i)
    {
      if(clientSheet == spreadsheets[i])
	{
	  sheetExists = true;
	  break;
	}
    }

  std::string unqID = std::to_string(idCounter);;
  counter.lock();
  connection->setID(idCounter);
  ++idCounter;
  counter.unlock();
  
  //If the sheet exists, then we add the client to the spreadsheet
  if(sheetExists)
    {
      serv->setClient(clientSheet, connection);
    }
  //Otherwise, we create a new spreadsheet and then add the client
  else 
    {
       serv->createSpreadsheet(clientSheet);
       serv->setClient(clientSheet, connection);
    }
  
  
  
  //Then we wait for the client to respond
  connection->clearData();
  connection->socket().async_read_some(
				       boost::asio::buffer(connection->getData(), 1024),
				       boost::bind(&network_controller::handleRecieve,
						   this,
						   boost::asio::placeholders::error,
						   boost::asio::placeholders::bytes_transferred,
						   connection));
	
}

void network_controller::handleRecieve(const boost::system::error_code& err, size_t bytes_transferred, con_handler::pointer connection)
{
    if(err)
    {
      std::cout << "Error Occurred: " << err.message() << std::endl;
      connection->socket().close();
      std::cout << connection->getUsername() << " has disconnected" << std::endl;
      serv->clientDisconnected(connection);
      return;
    }

    std::string editInfo(connection->getData());
    serv->handleEdit(connection->getSheetName(), editInfo, connection);

    connection->clearData();
    connection->socket().async_read_some(
					 boost::asio::buffer(connection->getData(), 1024),
					 boost::bind(&network_controller::handleRecieve,
						     this,
						     boost::asio::placeholders::error,
						     boost::asio::placeholders::bytes_transferred,
						     connection));
}

///Tst method for printing when the server sends something
void network_controller::handleWrite(const boost::system::error_code& err, size_t bytes_transferred)
{
  
}

//Sets the server that the network controller will pull from
void network_controller::setServer(server *ser)
{
  serv = ser;
}
