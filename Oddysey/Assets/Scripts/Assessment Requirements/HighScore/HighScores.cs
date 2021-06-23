﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Source of this script : https://www.youtube.com/watch?v=KZuqEyxYZCc
/// Dreamlo website : http://dreamlo.com/lb/E7rCqJXFYUiqskUJNPNQPApN8VQfgahEqFY_mxDbUAFg
/// This script handles the highscore functionality, including uploading/downloading data from dreamlo
/// </summary>
public class HighScores : MonoBehaviour
{
    const string privateCode = "E7rCqJXFYUiqskUJNPNQPApN8VQfgahEqFY_mxDbUAFg";
    const string publicCode = "60c817348f40bb114c3f7078";
    const string webURL = "http://dreamlo.com/lb/";

    public Highscore[] highscoresList;
    static HighScores instance;
    DisplayHighscores highscoresDisplay;

    public BinaryTree binaryTree = new BinaryTree();

    // Calls the instance of the game object, this script is attached to.
    // Gets the script component DisplayHighscores
    private void Awake()
    {
        instance = this;
        highscoresDisplay = GetComponent<DisplayHighscores>();
    }

    // Gets called by the player when they die, which then uploads their username / score to the DreamLo website. 
    public static void AddNewHighScore(string username, int score) 
    {
        instance.StartCoroutine(instance.UploadNewHighScore(username, score));
    }
 
    // Uploads the username / score values to the DreamLo website
    IEnumerator UploadNewHighScore(string username, int score) 
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if(string.IsNullOrEmpty(www.error)) 
        {
            print("Upload Successful");
            DownloadHighscores();
        }
        else 
        {
            print("Error uploading: " + www.error);
        }
    }

    // Starts the coroutine for connecting to the DreamLo website, and downloading the data
    public void DownloadHighscores() 
    {
        StartCoroutine("DownloadHighScoresFromDatabase");

    }

    // If connection is made 
    IEnumerator DownloadHighScoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            highscoresDisplay.OnHighscoresDownloaded(highscoresList);
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
            //print(highscoresList[i].username + ": " + highscoresList[i].score);
            
        }
        DuplicateHighscore(highscoresList);
    }

    // This duplicates the current scores on the highscore, into binary tree data nodes.  
    void DuplicateHighscore(Highscore[] highScores) 
    {
        binaryTree = new BinaryTree();
        for (int i = 0; i < highScores.Length; i ++) 
        {
            BinaryTree.BinaryTreeNode node = new BinaryTree.BinaryTreeNode() { index = highScores[i].score };
            binaryTree.CreateNode(node);
        }
        binaryTree.TraversePreOrder(binaryTree.root);
    }

    void CompareNodeValue(Highscore[] highscores) 
    {
        //// Doubly Linked List Implementation
        LinkedList<int> b1 = new LinkedList<int>();
        LinkedList<int> b2 = new LinkedList<int>();


    }

    // This method will sort the scores from highest to lowest via Bubble Sort
    void sortAscending() 
    {
    
    }

    // This method will sort the scores from Lowest to Highest via Shell Sort 
    void sortDescending() 
    {
    
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
