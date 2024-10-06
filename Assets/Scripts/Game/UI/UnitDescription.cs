using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UnitDescription : MonoBehaviour
    {
        public TMP_Text unitName;
        public TMP_Text unitDescription;
        
        public CanvasGroup canvasGroup;
        
        private ContentSizeFitter contentSizeFitter;
        private Coroutine fadeCoroutine;
        private float f;

        private void Awake()
        {
            contentSizeFitter = GetComponent<ContentSizeFitter>();
            canvasGroup.alpha = 0f;
            f = 0f;
        }

        public void SetValue(string name, string description)
        {
            unitName.text = name;
            unitDescription.text = description;
            f = 0f;
            Show();
        }
        
        private IEnumerator FadeOut()
        {
            while (f > 0f)
            {
                f -= Time.deltaTime;
                canvasGroup.alpha = f;
                yield return null;
            }
        }
        
        public void Hide()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            
            fadeCoroutine = StartCoroutine(FadeOut());
        }
        
        public IEnumerator FadeIn()
        {
            while (f < 1f)
            {
                f += Time.deltaTime;
                canvasGroup.alpha = f;
                yield return null;
            }
        }
        
        public void Show()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            
            fadeCoroutine = StartCoroutine(FadeIn());
        }
        
        
    }
}