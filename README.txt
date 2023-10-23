Spreadsheet Client-Server Spreadsheet

-- README -- 

Team COCOPRO

░█████╗░░█████╗░░█████╗░░█████╗░██████╗░██████╗░░█████╗░
██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗
██║░░╚═╝██║░░██║██║░░╚═╝██║░░██║██████╔╝██████╔╝██║░░██║
██║░░██╗██║░░██║██║░░██╗██║░░██║██╔═══╝░██╔══██╗██║░░██║
╚█████╔╝╚█████╔╝╚█████╔╝╚█████╔╝██║░░░░░██║░░██║╚█████╔╝
░╚════╝░░╚════╝░░╚════╝░░╚════╝░╚═╝░░░░░╚═╝░░╚═╝░╚════╝░

Authors
	Dylan Habersetzer
	Sabina Miani
	Erickson Nguyen
	Dylan Quach
	Tyler Wood

Compilation
	With the command line, run the following prompts:
	
	to start the server: 
unzip server.zip
cd server
docker image build --tag server_cocopro .
docker run --mount type=bind,source=somefolderofmine,target=/spreadsheets -p 1100:1100 server_cocopro /spreadsheet_server

	to run the test client: 
unzip tester.zip
cd tester
docker image build --tag tester_cocopro .
docker run tester_cocopro <test number> <ip address>:<port>

Server Shutdown
	In order to gracefully exit and shutdown the spreadsheet server, the key phrase control-C needs to be entered into the terminal after the server has been started successfully. 

