using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Board
    {
        public Transform hidenCard;
        public List<Transform> cards = new();
        public List<Transform> gnacks = new();
        
    }
}