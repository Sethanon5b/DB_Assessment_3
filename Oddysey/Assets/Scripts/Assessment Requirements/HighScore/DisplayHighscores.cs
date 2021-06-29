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
    public GameObject inputField;
    public Toggle toggle;
    public Button button;
    public Text indexResult;

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

        button.onClick.AddListener(PerformSearch);

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

    public Highscore FindUsername(string username)
    {
        Highscore highScore = new Highscore(null, 0);
        input = inputField.GetComponent<InputField>().text;

        foreach (Highscore hs in highscoreManager.linkedList)
        {
            if (hs.username == username)
            {
                highScore = hs;
            }
        }

       return highScore;
    }

    private void PerformSearch()
    {
        input = inputField.GetComponent<InputField>().text;
        
        if (!toggle.isOn)
        {
            int indexToFind;
            int.TryParse(input, out indexToFind);

            if(indexToFind != -1)
            {
                BinaryTree.BinaryTreeNode node = FindIndex(indexToFind);

                if(node.username != null /*string.Empty*/)
                {
                    indexResult.text = $"Found record in binary tree - name: {node.username}, score: {node.score}";
                }
                else
                {
                    Debug.Log("Search query returned no results.");
                }
            }
            else
            {
                Debug.Log("User tried to parse a non-int value");
            }
        }
        else
        {
            Highscore result = FindUsername(input);
            
            if(result.username != string.Empty)
            {
                Debug.Log("Result: " + result.username);
                indexResult.text = $"Found record in linked list - name: {result.username}, score: {result.score}";
            }
            else
            {
                Debug.Log("Search query returned no results.");
            }
        }
    }
    #endregion
}
