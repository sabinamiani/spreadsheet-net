spreadsheet_server:	serverBootup.o network_controller.o server.o con_handler.o spreadsheet.o cell.o DependencyGraph.o
	g++ -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ serverBootup.o network_controller.o server.o con_handler.o spreadsheet.o cell.o DependencyGraph.o -lpthread -o spreadsheet_server

serverBootup.o:	serverBootup.cpp server.h spreadsheet.h con_handler.h network_controller.h
	g++ -c -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ serverBootup.cpp -lpthread

network_controller.o:	network_controller.h network_controller.cpp server.h spreadsheet.h con_handler.h
	g++ -c -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ network_controller.cpp -lpthread

server.o:	server.h server.cpp spreadsheet.h con_handler.h network_controller.h
	g++ -c -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ server.cpp -lpthread

con_handler.o:	con_handler.h con_handler.cpp
	g++ -c -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ con_handler.cpp -lpthread

spreadsheet.o:	spreadsheet.h spreadsheet.cpp cell.h DependencyGraph.h
	g++ -c -std=c++17 -I boost_1_69_0/ -L boost_1_69_0/ spreadsheet.cpp -lpthread

cell.o:	cell.h cell.cpp
	g++ -c cell.cpp

DependencyGraph.o:	DependencyGraph.h DependencyGraph.cpp
	g++ -c DependencyGraph.cpp

clean: 
	rm -f serverBootup.o network_controller.o server.o con_handler.o spreadsheet.o cell.o DependencyGraph.o server