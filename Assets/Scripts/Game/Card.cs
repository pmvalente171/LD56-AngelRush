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
        
        public Card() : this(DiceUtil.D6(), DiceUtil.DCardSuit()) { }
    }
}