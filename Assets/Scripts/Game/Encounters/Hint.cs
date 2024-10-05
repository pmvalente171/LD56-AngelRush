using System;
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
    }
}