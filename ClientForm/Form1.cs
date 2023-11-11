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

namespace ClientForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn?.sendObject(new GameAction(GameActionType.Quit));
            conn.Close();
        }

        Button ButtonConnect;
        Button ButtonGetCard;
        Button ButtonDisconnect;
        Button ButtonSetName;

        PictureBox CardBox;
        TextBox PlayerNameBox;

        GroupBox ClientBox;

        ServerConnection conn;

        ImageList Images;

        private string GetTime => DateTime.Now.ToString("hh:mm:sstt");
        
        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(Screen.PrimaryScreen.Bounds.Width - 200, Screen.PrimaryScreen.Bounds.Height - 200);
            int left = 10, top = 10, gap = 10, bh = 30, bw = 90;

            ButtonConnect = GenerateButton(left, top + (bh + gap) * 0, bw, bh, "Connect", ConnectClicked);
            ButtonConnect.Enabled = true;
            ButtonDisconnect = GenerateButton(left,top + (bh + gap) * 1, bw, bh, "Disconnect", DisconnectClicked);
            ButtonDisconnect.Enabled = false;


            ClientBox = new GroupBox();
            ClientBox.Location = new Point(left, top + (bh + gap) * 2);
            ClientBox.Size = new Size(Size.Height - ClientBox.Location.Y, Size.Width);
            ClientBox.Enabled = false;

            Label tempLabel = new Label();
            tempLabel.Text = "Player Name";
            tempLabel.Location = new Point(left, top + (bh + gap) * 0);
            PlayerNameBox = new TextBox();
            PlayerNameBox.Location = new Point(left, top + (bh + gap) * 1);
            PlayerNameBox.Size = new Size(bw *4, bh);
            PlayerNameBox.Text = $"Player - {GetTime}";
            ButtonSetName = GenerateButton(left, top + (bh + gap) * 2, bw, bh, "Set Name", SetNameClicked);

            CardBox = new PictureBox();
            CardBox.Size = new Size(70, 104);
            CardBox.Location = new Point(left, top + (bh + gap) * 3);
            CardBox.Paint += CardBox_Paint;

            ButtonGetCard = GenerateButton(left + CardBox.Width + gap, top + (bh + gap) * 3, bw, bh, "Get Card", GetCardClicked);

            Images = new ImageList();
            Images.ImageSize = new Size(70, 104);
            GetImages();


            conn = new ServerConnection("127.0.0.1", 2048);

            ClientBox.Controls.AddRange(new Control[] { ButtonSetName, ButtonGetCard, tempLabel, PlayerNameBox, CardBox});
            Controls.AddRange(new Control[] { ButtonConnect, ButtonDisconnect, ClientBox });
        }

        private void GetImages()
        {
            Deck deck = new Deck();
            foreach(PlayingCard c in deck.cards)
            {
                Image img = ImageFactory.GetImage($"./cardimages/front/{c.getAbbrev()}.png");
                Images.Images.Add(c.getAbbrev(),img);
            }
        }

        private void CardBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PictureBox p = sender as PictureBox;
            if (p == null) return;
            PlayingCard card = p.Tag as PlayingCard;
            if (card != null && Images.Images.ContainsKey(card.getAbbrev())) p.Image = Images.Images[card.getAbbrev()];
            if (p.Image == null) return;
            g.DrawImage(p.Image, 0,0,p.Width,p.Height);
        }

        private void GetCardClicked(object sender, EventArgs e)
        {
            CardBox.Tag = conn.Request(new GameAction(GameActionType.CardRequest)) as PlayingCard;
            CardBox.Refresh();
        }

        private void SetNameClicked(object sender, EventArgs e)
        {
            if (PlayerNameBox.Text == PlayerNameBox.Tag as string && PlayerNameBox == null) return;
            conn?.sendObject(new PlayerInfo(PlayerNameBox.Text));
        }

        private void DisconnectClicked(object sender, EventArgs e)
        {
            ClientBox.Enabled = true;
            ButtonConnect.Enabled = true;
            ButtonDisconnect.Enabled = false;
            conn?.sendObject(new GameAction(GameActionType.Quit));
            conn?.Close();
        }

        private void ConnectClicked(object sender, EventArgs e)
        {
            ClientBox.Enabled = true;
            ButtonConnect.Enabled = false;
            ButtonDisconnect.Enabled = true;
            conn?.Connect();
            conn?.sendObject(new PlayerInfo(VerifiedName()));

        }

        private string VerifiedName()
        {
            string st = PlayerNameBox.Tag as string;
            if (st == null) return PlayerNameBox.Text;
            return st;
        }

        private Button GenerateButton(int x, int y, int w, int h, string text, EventHandler handler)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Click += handler;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
            return btn;
        }
    }
}
