using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HashTest : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
    //    Debug.Log(HashingScript.Encryption("Poop", 21));
    //}

    public string input;
    public GameObject inputField;
    public Text hashResult;

    public void StoreText() 
    {
        input = inputField.GetComponent<Text>().text;
        hashResult.GetComponent<Text>().text = HashingScript.Encryption(input, 400);
    }

}
