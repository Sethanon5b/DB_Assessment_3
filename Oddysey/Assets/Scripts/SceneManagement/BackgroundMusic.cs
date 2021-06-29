using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    // Source of the Script : https://answers.unity.com/questions/1260393/make-music-continue-playing-through-scenes.html
    // This script will prevent the game object it is attached to to be destroyed when the scene changes. 
    // Thus, allowing the audio source component to play the background music on loop.
    private static BackgroundMusic instance = null;
    public static BackgroundMusic Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

}
