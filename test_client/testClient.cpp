#include <iostream>
#include <vector>
#include <string>
#include <stdio.h>
#include <stdlib.h>
#include "json/json2/single_include/nlohmann/json.hpp"
#include <boost/asio.hpp>

using namespace boost::asio;
using ip::tcp;

void test1 (tcp::socket *sock, boost::system::error_code error);
void test2 (tcp::socket *sock, boost::system::error_code error);

int main(int argc, char *argv[])
{
  boost::asio::io_service io;
  int testnum;
  try
    {
      testnum = atoi(argv[0]);
    }
  catch(int e)
    {
      std::cout << "Num of tests go here" << std::endl;
    }

std::string ip = argv[1];

  tcp::socket sock(io);
  sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), 1100));
  boost::system::error_code error;

  if(testnum == 1)
    {
      test1(&sock, error);
    }
else if(testnum == 2)
    {
      test2(&sock, error);
    }

}

void test1 (tcp::socket *sock, boost::system::error_code error)
{
  std::string username = "tester";
  boost::asio::write(sock, boost::asio::buffer(username), error);
  if(error)
    {
      std::cout << "Error has occurred, could not send username" << std::endl;
    }
  boost::asio::streambuf buffer;
  boost::asio::read(sock, buffer, boost::asio::transfer_all(), error);
  if(error && error != boost::asio::error::eof)
    {
      std::cout << "Receive Failed" << std::endl;
    }
  else
    {
      const char* data = boost::asio::buffer_cast<const char*>(buffer.data());
      std::cout << data << std::endl;
    }
  std::string spreadsheetName = "Hello";
  boost::asio::write(sock, boost::asio::buffer(spreadsheetName), error);
}

void test2 (tcp::socket *sock, boost::system::error_code error)
{
  std::string username = "tester";
  boost::asio::write(sock, boost::asio::buffer(username), error);
  if(error)
    {
      std::cout << "Error has occurred, could not send username" << std::endl;
    }
  boost::asio::streambuf buffer;
  boost::asio::read(sock, buffer, boost::asio::transfer_all(), error);
  if(error && error != boost::asio::error::eof)
    {
      std::cout << "Receive Failed" << std::endl;
    }
  else
    {
      const char* data = boost::asio::buffer_cast<const char*>(buffer.data());
      std::cout << data << std::endl;
    }
  std::string spreadsheetName = "Hello";
  boost::asio::write(sock, boost::asio::buffer(spreadsheetName), error);

  boost::asio::write(sock, boost::asio::buffer("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\":\"2\"}"), error);

  boost::asio::streambuf buffer2;
  boost::asio::read(sock, buffer2, boost::asio::transfer_all(), error);

  try
  {
      nlohmann::json jObject = nlohmann::json::parse(buffer2.data());
      if(jObject["messageType"] == "cellUpdated" && jObject["cellName"] == "A1" && jObject["contents"] == "2")
          std::cout << "Pass" << std::endl;
      else
          std::cout << "Fail" << std::endl;
  }
  catch (int e)
  {
    std::cout << "Fail" << std::endl;
  }

}
