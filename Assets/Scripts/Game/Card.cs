using System;
using Game.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
    public class Card
    {
        public int cardValue;
        public CardSuit cardSuit;
        
        public Card(int cardValue, CardSuit cardSuit)
        {
            this.cardValue = cardValue;
            this.cardSuit = cardSuit;
        }
        
        public Card() { }
        
        public static Card RandomCard()
        {
            return new Card(DiceUtil.D(6), DiceUtil.DCardSuit());
        }
    }
}