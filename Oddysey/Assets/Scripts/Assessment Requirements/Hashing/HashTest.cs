using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HashTest : MonoBehaviour
{ 
    public String input;
    public GameObject inputField;
    public Text hashResult;
    /// <summary>
    /// Any words that the user puts into the input field, will then be encrypted via the Hashing script. 
    /// Then, the hashResult Text will gain the encryption results - allowing the user to see the results. 
    /// </summary>
    public void StoreText() 
    {
        input = inputField.GetComponent<Text>().text;
        hashResult.GetComponent<Text>().text = HashingScript.HashString(input).ToString();
    }
}
