using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Source of this script : https://www.youtube.com/watch?v=KZuqEyxYZCc
/// Dreamlo website : http://dreamlo.com/lb/E7rCqJXFYUiqskUJNPNQPApN8VQfgahEqFY_mxDbUAFg
/// This script handles the highscore functionality
/// </summary>
public class HighScores : MonoBehaviour
{
    const string privateCode = "E7rCqJXFYUiqskUJNPNQPApN8VQfgahEqFY_mxDbUAFg";
    const string publicCode = "60c817348f40bb114c3f7078";
    const string webURL = "http://dreamlo.com/lb/";

    public Highscore[] highscoresList;

    private void Awake()
    {
        
    }

    public void AddNewHighScore(string username, int score) 
    {
        StartCoroutine(UploadNewHighScore(username, score));
    }

    IEnumerator UploadNewHighScore(string username, int score) 
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if(string.IsNullOrEmpty(www.error)) 
        {
            print("Upload Successful");
        }
        else 
        {
            print("Error uploading: " + www.error);
        }
    }

    public void DownloadHighscores() 
    {
        StartCoroutine("DownloadHighScoresFromDatabase");
    }

    IEnumerator DownloadHighScoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores(string textStream) 
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];
        for(int i = 0; i <entries.Length; i ++) 
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
            print(highscoresList[i].username + ": " + highscoresList[i].score);
        }
    }
}

public struct Highscore 
{
    public string username;
    public int score;

    public Highscore(string _username, int _score) 
    {
        username = _username;
        score = _score;
    }
}
