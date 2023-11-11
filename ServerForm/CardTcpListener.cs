using PlayingCardDTOLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerForm
{
    public class CardTcpListener
    {

        private TcpListener server;
        private MyDeck deck;
        public EventHandler ClientChangeHandler;
        public void Run()
        {
            deck = new MyDeck();
            deck.Shuffle();
            try
            {
                int port = 2048;
                IPAddress address = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(address, port);

                server.Start();

                Console.WriteLine($"Successfuly started server at: {address.ToString()}:{port}");
                ClientCheck(ref deck);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public List<ClientWorker> workers;
        public List<Thread> threads;
        private void ClientCheck(ref MyDeck deck)
        {
            bool loop = true;
            List<Thread> threads = new List<Thread>();
            List<ClientWorker> workers = new List<ClientWorker>();
            while (loop)
            {
                ClientWorker wo = new ClientWorker(server.AcceptTcpClient(), ref workers, ref deck, ClientChangeHandler);
                workers.Add(wo);
                threads.Add(new Thread(wo.Run));
                threads[threads.Count - 1].Start();
            }
        }
    }
}
