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

namespace ServerForm
{
    public class ClientWorker : Worker
    {
        public event EventHandler changeEvent;
        public TcpClient client { get; set; }

        public PlayerInfo player { get; set; }

        private PlayingCard _lastCardSent;

        public PlayingCard lastCardSent { get { return _lastCardSent; } }

        public List<ClientWorker> workers;

        private MyDeck deck;
        public MyDeck Deck { get { return deck; } }

        private PlayerList GetPlayerList => new PlayerList(workers.ConvertAll((item) => item.player.Name));

        public ClientWorker(TcpClient client, ref List<ClientWorker> workers, ref MyDeck deck, EventHandler handler)
        {
            this.client = client;
            this.workers = workers;
            this.player = new PlayerInfo();
            this.deck = deck;
            changeEvent = handler;


            changeEvent?.Invoke(this, EventArgs.Empty);
            //Console.WriteLine($"Connection: {client.Client}");
        }

        public override void Update()
        {
            using (NetworkStream stream = client.GetStream())
            {
                BinaryReader br = new BinaryReader(stream);
                BinaryWriter bw = new BinaryWriter(stream);
                LinkStatus loop = LinkStatus.open;
                while (loop == LinkStatus.open)
                {
                    loop = Action(stream, br, bw);//runs the server queries for the specific client
                }
                br.Close();
                bw.Close();
            }
            client.Close();
            completed = true;
        }

        private LinkStatus Action(NetworkStream stream, BinaryReader br, BinaryWriter bw)
        {
            object obj = JSONFactory.Deserialize(br.ReadString());//gets the object the client sent

            //checks what object it was and runs method for corusponding type
            if (obj is GameAction) return GameActionSent((obj as GameAction).MyAction, br, bw);
            if (obj is PlayerInfo) return PlayerInfoSent((PlayerInfo)obj);

            return LinkStatus.closed;
        }

        private LinkStatus PlayerInfoSent(PlayerInfo player)
        {
            this.player = player;
            //Console.WriteLine($"Name Set:{client.Client} -> {player.Name}");
            changeEvent?.Invoke(this, EventArgs.Empty);
            return LinkStatus.open;
        }

        private LinkStatus GameActionSent(GameActionType action, BinaryReader br, BinaryWriter bw)
        {
            if (action == GameActionType.Quit)
            {
                //Console.WriteLine($"Disconnected: {player.Name}");
                workers.Remove(this);
                changeEvent?.Invoke(this, EventArgs.Empty);
                return LinkStatus.closed;
            }
            if (action == GameActionType.NoAction)
            {
                //Console.WriteLine($"No Action: {player.Name}");
                return LinkStatus.open;
            }

            if (action == GameActionType.CardRequest)
            {
                
                //Console.WriteLine($"Card sent to: {player.Name}");
                PlayingCard card = deck.GetTopCardBuffered();
                _lastCardSent = card;
                bw.Write(JSONFactory.Serialize(card));
                changeEvent?.Invoke(this, EventArgs.Empty);
                return LinkStatus.open;
            }
            if (action == GameActionType.GetPlayerList)
            {
                //Console.WriteLine($"PlayerList sent to: {player.Name}");
                bw.Write(JSONFactory.Serialize(GetPlayerList));
                return LinkStatus.open;
            }

            return LinkStatus.open;



        }
    }
    public enum LinkStatus
    {
        open,
        closed
    }
}
