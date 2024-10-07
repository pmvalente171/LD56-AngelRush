using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    public class EncounterUserDialog : MonoBehaviour
    {
        public TMP_Text userName;
        public TMP_Text userDialogue;
        public Image suitImage;
        
        [Space] public SuitIconMapping suitIconMapping;
        
        public void SetValue(string name, string dialogue, CardSuit suit)
        {
            userName.text = name;
            userDialogue.text = dialogue;
        }
    }
}