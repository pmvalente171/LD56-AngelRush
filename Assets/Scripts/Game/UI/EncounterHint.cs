using Game.Encounters;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class EncounterHint : MonoBehaviour
    {
        public TMP_Text hintName;
        public TMP_Text hintText;
        
        public void SetHint(Hint hint)
        {
            hintName.text = hint.hintName;
            hintText.text = hint.hintText;
        }
    }
}