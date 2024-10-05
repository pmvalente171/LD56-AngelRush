using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CardDeck
    {
        public Queue<Card> deck = new ();

        public void Shuffle(int deckSize=60)
        {
            deck.Clear();
            for (int i = 0; i < deckSize; i++)
            {
                deck.Enqueue(new Card());
            }
        }
        
        public Card Draw() => deck.Dequeue();
        
        public void Add(Card card) => deck.Enqueue(card);
        
        public void ExtendDeck(int deckSize=60)
        {
            for (int i = 0; i < deckSize; i++)
            {
                Add(new Card());
            }
        }
        
        public int Count() => deck.Count;
        
    }
}