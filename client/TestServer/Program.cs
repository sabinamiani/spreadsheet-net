using NetworkUtil;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace TestServer
{
    class Program
    {
        private Dictionary<long, SocketState> clients;

        static void Main(string[] args)
        {
            Program server = new Program();
            server.StartServer();

            // Sleep to prevent the program from closing,
            // since all the real work is done in separate threads.
            // StartServer is non-blocking.
            Console.Read();
        }

        private void SendData(string data)
        {
            HashSet<long> disconnectedClients = new HashSet<long>();
            lock (clients)
            {
                foreach (SocketState client in clients.Values)
                {
                    if (!Networking.Send(client.TheSocket, data))
                        disconnectedClients.Add(client.ID);
                }
            }
            foreach (long id in disconnectedClients) RemoveClient(id);
        }

        /// <summary>
        /// Initialized the server's state
        /// </summary>
        public Program()
        {
            clients = new Dictionary<long, SocketState>();
        }

        /// <summary>
        /// Start accepting Tcp sockets connections from clients
        /// </summary>
        public void StartServer()
        {
            // This begins an "event loop"
            Networking.StartServer(NewClientConnected, 1100);

            Console.WriteLine("Server is running");

            while(true)
            {
                string data = Console.ReadLine();
                Console.WriteLine(data + "echo");
                SendData(data);
            }
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// when a new client connects (see line 43)
        /// </summary>
        /// <param name="state">The SocketState representing the new client</param>
        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccured)
                return;

            // Save the client state
            // Need to lock here because clients can disconnect at any time
            lock (clients)
            {
                clients[state.ID] = state;
            }

            // change the state's network action to the 
            // receive handler so we can process data when something
            // happens on the network
            state.OnNetworkAction = ReceiveMessage;

            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// when a network action occurs (see lines 68-70)
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveMessage(SocketState state)
        {
            // Remove the client if they aren't still connected
            if (state.ErrorOccured)
            {
                RemoveClient(state.ID);
                return;
            }

            ProcessMessage(state);
            state.RemoveData(0, state.GetData().Length);
            // Continue the event loop that receives messages from this client
            Networking.GetData(state);
        }

        private void ProcessMessage(SocketState state)
        {
            Console.WriteLine(state.GetData());
        }

        /// <summary>
        /// Removes a client from the clients dictionary
        /// </summary>
        /// <param name="id">The ID of the client</param>
        private void RemoveClient(long id)
        {
            Console.WriteLine("Client " + id + " disconnected");
            lock (clients)
            {
                clients.Remove(id);
            }
        }
    }
}
