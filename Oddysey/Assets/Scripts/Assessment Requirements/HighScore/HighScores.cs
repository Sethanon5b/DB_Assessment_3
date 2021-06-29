using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

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

    public BinaryTree binaryTree;
    public LinkedList<Highscore> linkedList;

    public int[] bubbleScores;
    public int[] shellScores;

    [DllImport("SortingComparison")]
    private static extern void BubbleSort(int[] arr, int n);
    [DllImport("SortingComparison")]
    private static extern void ShellSort(int[] arr, int n);
    [DllImport("SortingComparison")]
    public static extern int LinearSearch(int[] arr, int maxIndex, int query);

    
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

    /// If connection is made, the scores are downloaded from DreamLo. 
    /// If a connection isn't made, it will print out an error. 
    
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

    /// <summary>
    /// This formats the downloaded highscores data and creates a list - sorted from the highest to lowest scores. 
    /// </summary>
    /// <param name="textStream"></param>
    void FormatHighscores(string textStream) 
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];
        for(int i = 0; i < entries.Length; i ++) 
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
        }

        // Duplicate and sort the high scores list
        CreateSortArrays();
        BubbleSort(bubbleScores, bubbleScores.Length);
        ShellSort(shellScores, shellScores.Length);

        List<Highscore> pairedBubble = PairData(bubbleScores);
        List<Highscore> pairedShell = PairData(shellScores);

        PopulateBinaryTree(pairedBubble);
        PopulateLinkedList(pairedShell);

    }

    /// <summary>
    /// This creates two highscores list arrays, one sorted by bubble sort and the other by shell sort.  
    /// </summary>
    void CreateSortArrays()
    {
        bubbleScores = new int[highscoresList.Length];
        for (int i = 0; i < bubbleScores.Length; i++)
        {
            bubbleScores[i] = highscoresList[i].score;
        }

        shellScores = new int[highscoresList.Length];
        for (int i = 0; i < shellScores.Length; i++)
        {
            shellScores[i] = highscoresList[i].score;
        }
    }

    /// <summary>
    /// This recouples the bubble sorted / shell sorted array data with the records in the highscore list.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    List<Highscore> PairData(int[] input)
    {
        List<Highscore> checkOffList = new List<Highscore>(highscoresList);
        List<Highscore> paired = new List<Highscore>();

        for (int i = 0; i < input.Length; i++)
        {
            for (int c = 0; c < checkOffList.Count; c++)
            {
                if (input[i] == checkOffList[c].score)
                {
                    paired.Add(checkOffList[c]);
                    checkOffList.Remove(checkOffList[c]);
                    break;
                }
            }
        }

        return paired;
    }

    /// <summary>
    /// This creates and populates the binary tree based on the paired data list 
    /// </summary>
    /// <param name="scores">This list is an already sorted list</param>
    void PopulateBinaryTree(List<Highscore> scores)
    {
        binaryTree = new BinaryTree();
        int index = 1;

        for (int i = scores.Count - 1; i >= 0; i--)
        {
            BinaryTree.BinaryTreeNode node = new BinaryTree.BinaryTreeNode();
            node.index = index;
            node.username = scores[i].username;
            node.score = scores[i].score;
            binaryTree.CreateNode(node);
            index++;
            Debug.Log($"Creating new node - Index: {node.index}, Username: {node.username}, Score: {node.score}");
        }
    }

    /// <summary>
    /// This creates and populates the Linked list based on the paired data list
    /// </summary>
    /// <param name="scores"></param>
    void PopulateLinkedList(List<Highscore> scores)
    {
        linkedList = new LinkedList<Highscore>();

        foreach(Highscore score in scores)
        {
            linkedList.AddFirst(score);
        }
    }

    /// <summary>
    /// This is used to find the index value in the sorted bubble score list, which then is used to find the matching index in the binary tree (but reversed, because the binary tree has already been sorted) 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public BinaryTree.BinaryTreeNode PerformLinearSearch(int value)
    {
        int index = LinearSearch(bubbleScores, bubbleScores.Length, value);

        if(index != -1)
        {
            int reversedIndex = bubbleScores.Length - index;
            BinaryTree.BinaryTreeNode node = binaryTree.Find(reversedIndex);

            if(node != null)
            {
                return node;
            }
        }
        
        return null;
    }
}

public class Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}
