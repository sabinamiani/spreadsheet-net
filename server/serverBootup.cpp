#include <iostream>
#include <csignal>
#include "server.h"

using namespace boost::asio;

server serv = server();

void signalHandle(int signum)
{
  printf("handle signal called \n");

  serv.close();
  std::cout << "server closed" << std::endl;
  exit(signum);
}

int main()
{
  //serv = server();
  signal(SIGINT, signalHandle);
  serv.start();

  return 0;
}
