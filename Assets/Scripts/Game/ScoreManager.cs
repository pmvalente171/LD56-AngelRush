using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Game.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ComboString
    {
        public string actionText;
        public int score;
        public float time;

        public ComboString(string flippingCombo, int newScore, float newTime)
        {
            actionText = flippingCombo;
            score = newScore;
            time = newTime;
        }
    }
    
    [RequireComponent(typeof(ParticleSpawner))]
    public class ScoreManager : MonoBehaviour
    {
        public Transform boot;
        public Transform particleSpawnPosition;
        
        [Space] public bool isCounting = true;
        
        public float startingScore = 20;
        public TMP_Text scoreText;
        
        [Space] public Transform scorePanel;
        public ActionText actionTextPrefab;
        
        [HideInInspector] public float currentScore = 20;
        private Dictionary<string, ComboString> comboStrings = new ();
        
        private Vector3 bootStartPosition;
        private Vector3 bootStartScale;
        private Quaternion bootStartRotation;
        
        private Coroutine bootTween;
        private ParticleSpawner particleSpawner;

        private IEnumerator bootTweenAnimation()
        {
            float f = 0f;
            float duration = 0.1f;
            
            Vector3 finalPosition = bootStartPosition + new Vector3(0, -0.2f, 0);
            Vector3 finalScale = bootStartScale * 1.1f;

            while (f < 1f)
            {
                f += Time.deltaTime / (duration / 2);
                boot.position = Vector3.Lerp(bootStartPosition, finalPosition, Mathf.SmoothStep(0f, 1f, f));
                boot.localScale = Vector3.Lerp(bootStartScale, finalScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
            
            f = 0f;
            while (f < 1f)
            {
                f += Time.deltaTime / (duration / 2);
                boot.position = Vector3.Lerp(finalPosition, bootStartPosition, Mathf.SmoothStep(0f, 1f, f));
                boot.localScale = Vector3.Lerp(finalScale, bootStartScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
            
            boot.position = bootStartPosition;
            boot.localScale = bootStartScale;
            
            bootTween = null;
        }
        
        private void BootTween()
        {
            if (bootTween != null)
                StopCoroutine(bootTween);
            bootTween = StartCoroutine(bootTweenAnimation());
        }
        
        private void Awake()
        {
            currentScore = startingScore;
            scoreText.text = currentScore.ToString("F2");
            particleSpawner = GetComponent<ParticleSpawner>();
            
            bootStartPosition = boot.position;
            bootStartScale = boot.localScale;
            bootStartRotation = boot.rotation;
        }

        private void Update()
        {
            if (!isCounting) return;
            
            currentScore -= Time.deltaTime;
            if (Time.frameCount % 10 == 0)
            {
                // update the score every 10 frames
                scoreText.text = currentScore.ToString("F2");
            }
            
            var Keys = new List<string>(comboStrings.Keys);
            var newValues = new List<float>();
            foreach (var comboString in comboStrings.Values)
            {
                comboString.time -= Time.deltaTime;
                newValues.Add(comboString.time);
            }
            
            if (Keys.Count != newValues.Count) return;
            for (var i = 0; i < Keys.Count; i++)
            {
                comboStrings[Keys[i]] = new ComboString(comboStrings[Keys[i]].actionText, 
                    comboStrings[Keys[i]].score, newValues[i]);

                if (newValues[i] <= 0)
                {
                    AddScore(comboStrings[Keys[i]].actionText, comboStrings[Keys[i]].score, 1f);
                    comboStrings.Remove(Keys[i]);
                }
            }
        }
        
        public void StartCombo(string comboName, ComboString comboString)
        {
            comboStrings[comboName] = comboString;
        }
        
        public void VerifyCombo(string comboName)
        {
            if (comboStrings.ContainsKey(comboName))
            {
                AddScore(comboStrings[comboName].actionText, comboStrings[comboName].score, 1f);
                comboStrings.Remove(comboName);
            }
        }
        
        public void VerifyAndStartCombo(string comboName, ComboString comboString)
        {
            VerifyCombo(comboName);
            StartCombo(comboName, comboString);
        }
        
        public void AddScore(string actioText, float score, float duration)
        {
            currentScore += score;
            scoreText.text = currentScore.ToString("F2");
            BootTween();
            particleSpawner.SpawnParticle(particleSpawnPosition.position);
            
            var actionText = Instantiate(actionTextPrefab, scorePanel);
            actionText.fadeDuration = duration;
            actionText.SetText(actioText, (int) score);
        }
        
        public void SaveScore()
        {
            currentScore = Mathf.Max(0, currentScore);
            
            // get the current list of scores
            var scores = PlayerPrefs.GetString("scores_u", "0,0,0,0,0");
            var scoreList = scores.Split(',');
            
            // verify if the current score is higher than any of the previous scores
            for (var i = 1; i < scoreList.Length; i++)
            {
                if (currentScore > int.Parse(scoreList[i]))
                {
                    scoreList[i] = currentScore.ToString("F2");
                    break;
                }
            }
            
            // order the list of scores
            Array.Sort(scoreList, 1, scoreList.Length - 1);
            
            // save the scores
            PlayerPrefs.SetString("scores_u", string.Join(",", scoreList));
        }
    }
}