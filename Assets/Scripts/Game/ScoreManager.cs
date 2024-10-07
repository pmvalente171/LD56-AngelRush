using System;
using System.Collections.Generic;
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
    
    public class ScoreManager : MonoBehaviour
    {
        public bool isCounting = true;
        
        public float startingScore = 20;
        public TMP_Text scoreText;
        
        [Space] public Transform scorePanel;
        public ActionText actionTextPrefab;
        
        [HideInInspector] public float currentScore = 20;
        private Dictionary<string, ComboString> comboStrings = new ();
        
        private void Awake()
        {
            currentScore = startingScore;
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
            
            var actionText = Instantiate(actionTextPrefab, scorePanel);
            actionText.fadeDuration = duration;
            actionText.SetText(actioText, (int) score);
        }
        
        public void SaveScore()
        {
            currentScore = Mathf.Max(0, currentScore);
            
            // get the current list of scores
            var scores = PlayerPrefs.GetString("scores", "");
            var scoreList = scores.Split(',');
            
            // add the current score to the list
            Array.Resize(ref scoreList, scoreList.Length + 1);
            scoreList[^1] = currentScore.ToString("F2");
            
            // sort the list
            Array.Sort(scoreList);
            
            // save the list
            PlayerPrefs.SetString("scores", string.Join(",", scoreList));
        }
    }
}