using System.Collections.Generic;
using Game.Encounters;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "GameEncounter", menuName = "Game/GameEncounter", order = 0)]
    public class Encounter : ScriptableObject
    {
        public Card hiddenCard;
        public List<Hint> hints;
        
        [Space] 
        [Header("Encounter Description")]
        public string encounterName;
        public string personName;
        [TextArea] public string encounterDescription;
        
        [Space]
        public string encounterInitialDialogue;
        public string encounterSuccessDialogue;
        public string encounterFailureDialogue;
        
        [Space]
        public List<string> stressDialogue;
        
        public Queue<Hint> getHints()
        {
            hints.Sort((a, b) => a.hintPriority.CompareTo(b.hintPriority));
            Queue<Hint> hintQueue = new(hints);
            return hintQueue;
        }
        
    }
}