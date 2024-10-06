using Game.Encounters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EncounterEnemyDialog : MonoBehaviour
    {
        public TMP_Text enemyName;
        public TMP_Text enemyDialogue;
        public Image enemyImage;
        
        public void SetHint(Hint hint)
        {
            enemyName.text = hint.hintEnemyName;
            enemyDialogue.text = hint.hintDialogue;
            enemyImage.sprite = hint.hintEnemyImage;
        }
    }
}