using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used for creating the UI functionality of the highscore system. 
/// Source : https://www.youtube.com/watch?v=9jejKPPKomg
/// </summary>
public class DisplayHighscores : MonoBehaviour
{
    public Text[] highscoreText;
    HighScores highscoreManager;
    /// <summary>
    /// Initially, the list of scores will all display the message "fetching...", until it downloads the information from Dreamlo. 
    /// If it cannot get this information, it will repeat displaying the initial message.
    /// </summary>
    void Start()
    {
        for(int i = 0; i < highscoreText.Length; i ++) 
        {
            highscoreText[i].text = i + 1 + ". fetching...";
        }

        highscoreManager = GetComponent<HighScores> ();

        StartCoroutine("RefreshHighscores");
    }
    /// <summary>
    /// When the information from Dreamlo is downloaded, it will display the two string values "username" and "score". 
    /// </summary>
    /// <param name="highscoreList"></param>
    public void OnHighscoresDownloaded(Highscore[] highscoreList)
    {
        for (int i = 0; i < highscoreText.Length; i++)
        {
            highscoreText[i].text = i + 1 + ". ";
            if(highscoreList.Length > i) 
            {
                highscoreText[i].text += highscoreList[i].username + " - " + highscoreList[i].score;
            }
        }
    }

    IEnumerator RefreshHighscores() 
    {
        while (true) 
        {
            highscoreManager.DownloadHighscores();
            yield return new WaitForSeconds(30);
        }
    }
}
