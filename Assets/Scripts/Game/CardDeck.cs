using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CardDeck
    {
        public Queue<CardData> deck = new ();

        public void Shuffle(int deckSize=60)
        {
            deck.Clear();
            for (int i = 0; i < deckSize; i++)
            {
                deck.Enqueue(CardData.RandomCard());
            }
        }
        
        public CardData Draw() => deck.Dequeue();
        
        public void Add(CardData cardData) => deck.Enqueue(cardData);
        
        public void ExtendDeck(int deckSize=60)
        {
            for (int i = 0; i < deckSize; i++)
            {
                Add(CardData.RandomCard());
            }
        }
        
        public int Count() => deck.Count;
        
    }
}