using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayingCardDTOLib;
using PlayingCardsJSON;
using System.Threading;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerConnection conn = new ServerConnection("127.0.0.1", 2048);
            try{


            conn.Connect();

            conn.sendObject(new PlayerInfo(DateTime.Now.ToString("hh:mmtt")));


            while (true)
            {
                PlayingCard p = conn.Request(new GameAction(GameActionType.CardRequest)) as PlayingCard;
                Console.WriteLine(p.getAbbrev());
                Thread.Sleep(300);
            }

            conn.sendObject(new GameAction(GameActionType.Quit));
            Console.WriteLine("quit");
            Console.ReadLine();
            }catch (Exception ex)
            {
                conn?.sendObject(new GameAction(GameActionType.Quit));
                Console.WriteLine(ex.ToString());

            }
        }
    }
}
