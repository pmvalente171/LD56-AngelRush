using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ActionText : MonoBehaviour
    {
        public TMP_Text actionText;
        public TMP_Text scoreModifierText;
        public float fadeDuration = 1.5f;
        
        private CanvasGroup canvasGroup;
        
        private IEnumerator Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            float f = 0;
            while (f < 1)
            {
                f += Time.deltaTime / fadeDuration;
                canvasGroup.alpha = 1 - f;
                yield return null;
            }
            
            // destroy the object 
            Destroy(gameObject);
        }
        
        public void SetText(string text, int value)
        {
            actionText.text = text;
            scoreModifierText.text = $"+{value}";
        }
        
    }
}