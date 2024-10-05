using UnityEngine;

namespace Game.Util
{
    public static class DiceUtil
    {
        public static int D(int sides)
        {
            return Random.Range(1, sides + 1);
        }
        
        public static int D6()
        {
            return D(6);
        }
        
        public static int D8()
        {
            return D(8);
        }
        
        public static int D10()
        {
            return D(10);
        }
        
        public static int D12()
        {
            return D(12);
        }
        
        public static int D20()
        {
            return D(20);
        }
        
        public static int D100()
        {
            return D(100);
        }
        
        public static CardSuit DCardSuit()
        {
            return (CardSuit) D(4);
        }
    }
}