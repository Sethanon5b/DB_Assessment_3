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
    public string input;
    public Text title;
    public InputField inputField;
    public Dropdown searchMethod;
    public Button searchButton;
    //public Button bubbleSortButton;
    //public Button shellSortButton;
    public Button resetDataButton;
    public Text indexResult;

    public Text[] highscoreText;
    HighScores highscoreManager;

    /// <summary>
    /// Initially, the list of scores will all display the message "fetching...", until it downloads the information from Dreamlo. 
    /// If it cannot get this information, it will repeat displaying the initial message.
    /// The functionality of the buttons are also called here. 
    /// </summary>
    void Start()
    {
        for(int i = 0; i < highscoreText.Length; i ++) 
        {
            highscoreText[i].text = i + 1 + ". fetching...";
        }

        highscoreManager = GetComponent<HighScores> ();

        StartCoroutine("RefreshHighscores");

        searchButton.onClick.AddListener(PerformSearch);
        resetDataButton.onClick.AddListener(ResetDataPressed);

    }

    /// <summary>
    /// When the information from Dreamlo is downloaded, it will display the two string values "username" and "score". 
    /// </summary>
    /// <param name="highscoreList"></param>
    public void OnHighscoresDownloaded(Highscore[] highscoreList)
    {
        for (int i = 0; i < highscoreText.Length; i++)
        {
            highscoreText[i].text = i + 1 + " - " + highscoreManager.highscoresList[i].username + " - " + highscoreManager.highscoresList[i].score;
        }
    }

    /// <summary>
    /// This is tied to the Onclick event for the relevant UI button. When this is clicked, 
    /// it uses the binary tree (which has been sorted according to the bubble sort function) to display the top ten scores.  
    /// </summary>
    public void BubbleSortPressed()
    {
        highscoreManager.CreateSortArrays();
        Debug.Log("Bubble sort pressed!");
        for (int i = 0; i < highscoreText.Length; i++)
        {
            Debug.Log("Current index: " + i);
            BinaryTree.BinaryTreeNode node = highscoreManager.binaryTree.Find(i + 1);
            if (node != null)
            {
                highscoreText[i].text = i + 1 + " - " + node.username + " - " + node.score;
            }
            else
            {
                Debug.Log("Node is null");
            }
        }
    }

    /// <summary>
    /// This clears the highscores present in the highscore scene. 
    /// </summary>
    private void ResetDataPressed()
    {
        for (int i = 0; i < highscoreText.Length; i++)
        {
            highscoreText[i].text = "Data Cleared";
        }
    }

    /// <summary>
    /// Every 30 seconds, it will refresh the highscores list. 
    /// </summary>
    /// <returns></returns>
    IEnumerator RefreshHighscores() 
    {
        while (true) 
        {
            highscoreManager.DownloadHighscores();
            yield return new WaitForSeconds(30);
        }
    }

    #region Comparator Functionality
    /// <summary>
    /// This function will allow the user to search through the binary tree nodes, to find highscore records which are not listed on the scene.
    /// By typing in the rank number, players will be able to find the username and score of whoever has that position. 
    /// </summary>
    public BinaryTree.BinaryTreeNode FindIndex(int index)
    {
        BinaryTree.BinaryTreeNode node = highscoreManager.binaryTree.Find(index - 1);

        return node;
    }

    /// <summary>
    /// When the user types in the correct username, this method will return the data associated with that username (i.e score)
    /// If what the user types doesn't correspond with anything in the list, it returns null.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public Highscore FindUsername(string username)
    {
        input = inputField.text;

        foreach (Highscore hs in highscoreManager.linkedList)
        {
            if (hs.username == username)
            {
                return hs;
            }
        }

       return null;
    }

    /// <summary>
    /// When the user types in the correct score, this method will return the data associated with that score (i.e username)
    /// If what the user types doesn't correspond with anything in the list, it returns null.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public BinaryTree.BinaryTreeNode FindScore(int score)
    {
        BinaryTree.BinaryTreeNode node = highscoreManager.PerformLinearSearch(score);
        
        if (node != null)
        {
            return node;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// This creates the value of the drop-down menu in the highscore scene, which allows the user to specify what type of search to perform. I.E Username, Score or Rank. 
    /// </summary>
    private void PerformSearch()
    {
        input = inputField.text;
        
        if (searchMethod.value == 0) // Search by rank
        {
            int indexToFind;
            int.TryParse(input, out indexToFind);

            if(indexToFind != -1)
            {
                BinaryTree.BinaryTreeNode node = FindIndex(indexToFind + 1);

                if(node != null)
                {
                    indexResult.text = $"Found record in binary tree - name: {node.username}, score: {node.score}";
                }
                else
                {
                    indexResult.text = "Could not find a result";
                    Debug.Log("Search query returned no results.");
                }
            }
            else
            {
                Debug.Log("User tried to parse a non-int value");
            }
        }
        else if (searchMethod.value == 1) // Search by username
        {
            Highscore result = FindUsername(input);
            
            if(result != null)
            {
                //Debug.Log("Result: " + result.username);
                indexResult.text = $"Found record in linked list - name: {result.username}, score: {result.score}";
            }
            else
            {
                indexResult.text = "Could not find a result";
                Debug.Log("Search query returned no results.");
            }
        }
        else if (searchMethod.value == 2) // Search by score
        {
            int scoreToFind;
            int.TryParse(inputField.text, out scoreToFind);

            if(scoreToFind != -1)
            {
                BinaryTree.BinaryTreeNode result = FindScore(int.Parse(inputField.text));

                if(result != null)
                {
                    indexResult.text = $"Found record in linked list - name: {result.username}, score: {result.score}";
                }
                else
                {
                    indexResult.text = "Could not find a result";
                    Debug.Log("Search query returned no results.");
                }
            }
            else
            {
                indexResult.text = "Could not find a result";
                Debug.Log("Search query returned no results.");
            }
        }
    }
    #endregion
}
