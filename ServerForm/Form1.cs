using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayingCardDTOLib;
using PlayingCardsJSON;
using System.Threading;
using System.Diagnostics.Tracing;

namespace ServerForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        CardTcpListener listener;
        Thread BackgroundThread;
        MyListView listView;
        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(Screen.PrimaryScreen.Bounds.Width -200, Screen.PrimaryScreen.Bounds.Height -150);
            listener = new CardTcpListener();
            listener.ClientChangeHandler += Lv_ClientChanged;
            BackgroundThread = new Thread(listener.Run);

            listView = new MyListView(listener.workers);
            listView.Location = new Point(10, 10);
            listView.Size = new Size(900, Size.Height - listView.Location.Y);
            listView.View = View.List;
            
            Controls.Add(listView);

            GetImages();

            BackgroundThread.Start();
        }

        private void GetImages()
        {
            Deck deck = new Deck();
            ImageList list = new ImageList();
            list.ImageSize = new Size(70, 104);
            for(int i = 0; i < deck.cards.Count; i++)
            {
                string key = deck.cards[i].getAbbrev();
                Image img = ImageFactory.GetImage($"./cardimages/front/{key}.png");
                list.Images.Add(key, img);
            }
            listView.LargeImageList = list;
            listView.SmallImageList = list;
            listView.StateImageList = list;
        }

        private void Lv_ClientChanged(object sender, EventArgs e)
        {
            listView.BeginInvoke(new Action(() =>
            {
                ClientWorker C = sender as ClientWorker;
                List<ClientWorker> copy = new List<ClientWorker>(C.workers);
                if (C == null) return;
                listView.Items.Clear();
                foreach(ClientWorker Client in copy)
                {
                    if(Client.lastCardSent == null)
                    {
                        listView.Items.Add(Client.player.Name);

                    }
                    else
                    {
                        listView.Items.Add(Client.player.Name, Client.lastCardSent.getAbbrev());
                    }
                }
            }));
        }
    }

    internal class MyListView : ListView
    {
        public List<ClientWorker> Workers { get; set; }
        public MyListView(List<ClientWorker> workers)
        {
            Workers = workers;
        }

    }
}
