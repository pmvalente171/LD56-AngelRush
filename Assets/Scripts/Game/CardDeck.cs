using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CardDeck
    {
        public Queue<CardData> deck = new ();
        public int cardValueRange = 4;

        
        public CardData Draw() => CardData.RandomCard(cardValueRange);
        
    }
}