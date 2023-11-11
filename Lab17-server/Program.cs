using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using PlayingCardsJSON;
using System.IO;
using PlayingCardDTOLib;
using System.Threading;

namespace Lab17_server
{
    public class CardTcpListener
    {

        static private TcpListener server;
        static private Deck deck;
        public static void Main()
        {
            deck = new Deck();
            deck.Shuffle();
            try
            {
                int port = 2048;
                IPAddress address = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(address, port);

                server.Start();

                Console.WriteLine($"Successfuly started server at: {address.ToString()}:{port}");
                ClientCheck(ref deck);
            }catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static private void ClientCheck(ref Deck deck)
        {
            bool loop = true;
            List<Thread> threads = new List<Thread>();
            List<ClientWorker> workers = new List<ClientWorker>();
            while (loop)
            {
                ClientWorker wo = new ClientWorker(server.AcceptTcpClient(), ref workers, ref deck);
                workers.Add(wo);
                threads.Add(new Thread(wo.Run));
                threads[threads.Count - 1].Start();
            }
        }
    }

}
