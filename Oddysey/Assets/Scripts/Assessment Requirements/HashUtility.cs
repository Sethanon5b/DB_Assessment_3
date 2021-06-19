using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Hashing script will be used to   
/// </summary>
public class HashUtility : MonoBehaviour
{
    public int HashString(string text) 
    {
        unchecked 
        {
            int hash = 12;
            foreach (char c in text) 
            {
                hash = hash * 78 + c;
            }
            return hash;
        }
    }
}
