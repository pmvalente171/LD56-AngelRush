using System;
using Game.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
    public class CardData
    {
        public int cardValue;
        
        public CardData(int cardValue, CardSuit cardSuit)
        {
            this.cardValue = cardValue;
        }
        
        public CardData() { }
        
        public static CardData RandomCard(int val)
        {
            return new CardData(DiceUtil.DValue(val), DiceUtil.DCardSuit());
        }
    }
}