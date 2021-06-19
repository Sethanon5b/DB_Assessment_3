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
        GUIContent c = new GUIContent("play", null, "Test tooltip");
        GUIContent d = new GUIContent("highscore", null, "Test tooltip");
        GUIContent e = new GUIContent("quit", null, "Test tooltip");
        GUIContent f = new GUIContent("oddysey", null, "Test tooltip");

        // Generates the title of the game
        GUI.Box(new Rect(100, 150, 200, 75), f, titleStyle);

        // This button will take the player to the game scene
        if (GUI.Button(new Rect(100, 300, 200, 75), c, style)) 
        {
            SceneManager.LoadScene("Level_Selection");
        }
        // This button will take the player to the highscore screen
        if (GUI.Button(new Rect(100, 450, 200, 75), d, style))
        {
            SceneManager.LoadScene("Highscore_Scene");
        }
        // This button will terminate the game
        if (GUI.Button(new Rect(100, 600, 200, 75), e, style))
        {
            Application.Quit();
        }
    }
}
