using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Encounters
{
    [Serializable]
    public class Hint
    {
        public string hintName;
        [TextArea] 
        public string hintText;
        
        [Space] [TextArea]
        public string hintDialogue;
        public string hintEnemyName;
        public Sprite hintEnemyImage;
        
        [Space]
        public int hintPriority;
        public List<string> context;
    }
}