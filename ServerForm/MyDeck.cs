using PlayingCardDTOLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerForm
{
    public class MyDeck : Deck
    {


        public MyDeck() : base() {
            bufferDeck = GenerateDeck();
            ShuffleBuffered();
        }


        public void AddDeck()
        {
            for (CardSuit cardSuit = CardSuit.Clubs; cardSuit <= CardSuit.Spades; cardSuit++)
            {
                for (CardRank cardRank = CardRank.Ace; cardRank <= CardRank.King; cardRank++)
                {
                    if (true)
                    {
                        PlayingCard item = new PlayingCard(cardRank, cardSuit, 0);
                        cards.Add(item);
                    }
                }
            }
        }


        private List<PlayingCard> GenerateDeck()
        {
            List<PlayingCard> temp = new List<PlayingCard>(); 
            for (CardSuit cardSuit = CardSuit.Clubs; cardSuit <= CardSuit.Spades; cardSuit++)
            {
                for (CardRank cardRank = CardRank.Ace; cardRank <= CardRank.King; cardRank++)
                {
                    if (true)
                    {
                        PlayingCard item = new PlayingCard(cardRank, cardSuit, 0);
                        temp.Add(item);
                    }
                }
            }
            return temp;
        }
        public void ShuffleBuffered()
        {
            for (int num = bufferDeck.Count - 1; num > 1; num--)
            {
                int index = Rnd.Next(num);
                PlayingCard value = bufferDeck[num];
                bufferDeck[num] = bufferDeck[index];
                bufferDeck[index] = value;
            }
        }

        List<PlayingCard> bufferDeck;
        bool bufferUsed = false;

        int bufferTracker = 0;
        public PlayingCard GetTopCardBuffered()
        {
            try
            {

                if (bufferUsed)
                {
                    bufferDeck = GenerateDeck();
                    ShuffleBuffered();
                    bufferUsed = false;
                }
                if (IsEmptyBufferd())
                {
                    cards = bufferDeck;
                    bufferUsed = true;
                }

                PlayingCard result = IsEmptyBufferd() ? GetBufferedCard() : cards.First((card) => card != null);
                if(result == null ) { throw new Exception(); }
                if(!IsEmpty()) cards.RemoveAt(0);
                return result;
            }
            catch (Exception e)
            {
                
                Thread.Sleep(100);
                return GetTopCardBuffered();
            }
        }

        public bool IsEmptyBufferd()
        {
            return cards.Count <= 0 || (cards.Count - cards.FindAll((card) => card == null).Count) <= 0;
        }
        private PlayingCard GetBufferedCard()
        {
            PlayingCard card = bufferDeck[0];
            bufferDeck.RemoveAt(0);
            bufferUsed = true;
            return card;

        }

    }
}
