using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public List<Text> scoreTexts;

    private void Awake()
    {
        // load the scores from the player prefs
        var scores = PlayerPrefs.GetString("scores_u", "0,0,0,0,0"); 
        var scoreList = scores.Split(',');
        
        scoreList = scoreList.Skip(1).ToArray();
        Array.Reverse(scoreList);
        
        var max = Mathf.Min(scoreTexts.Count, scoreList.Length);
        for (var i = 0; i < max; i++)
        {
            scoreTexts[i].text = scoreList[i];
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
