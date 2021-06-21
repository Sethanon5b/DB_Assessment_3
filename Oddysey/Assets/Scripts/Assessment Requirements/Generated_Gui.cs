using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This code handles the creation of the buttons on the main menu, as well as handling their functionality. 
/// Created for Assessment purposes
/// </summary>

public class Generated_Gui : MonoBehaviour
{
    public GUIStyle style;
    public GUIStyle titleStyle;

    private void OnGUI()
    {
        GUIContent c = new GUIContent("play", null, "Go to Game scene");
        GUIContent d = new GUIContent("highscore", null, "Go to Highscore scene");
        GUIContent e = new GUIContent("quit", null, "Terminate the application");
        GUIContent f = new GUIContent("oddysey", null, "Title");
        GUIContent g = new GUIContent("hashing", null, "Go to Hashing scene");

        // Generates the title of the game
        GUI.Box(new Rect(100, 150, 200, 75), f, titleStyle);

        // This button will take the player to the game scene
        if (GUI.Button(new Rect(100, 300, 200, 75), c, style)) 
        {
            SceneManager.LoadScene("Level_Selection");
        }
        // This button will take the player to the highscore scene
        if (GUI.Button(new Rect(100, 450, 200, 75), d, style))
        {
            SceneManager.LoadScene("Highscore_Scene");
        }
        // This button will take the player to the hashing scene
        if (GUI.Button(new Rect(100, 600, 200, 75), g, style))
        {
            SceneManager.LoadScene("Hashing_Scene");
        }
        // This button will terminate the game 
        if (GUI.Button(new Rect(100, 750, 200, 75), e, style))
        {
            Application.Quit();
        }
    }
}
