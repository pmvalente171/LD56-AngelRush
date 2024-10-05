using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Encounters
{
    [Serializable]
    public class Hint
    {
        [TextArea] 
        public string hintText;
        
        [TextArea]
        public string hintDialogue;
        
        public int hintPriority;
        
        [Space]
        public List<string> context;
    }
}