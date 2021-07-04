using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class HashingScript 
{
    /// This script is the main functionality of the hashing script
    /// When the string is defined by Hash test, this script will assigned a value to each letter, so it will become encrypted. 
    /// Source of the script : https://www.geeksforgeeks.org/string-hashing-using-polynomial-rolling-hash-function/
    public static long HashString(String inputText)
    {
        // Declare variables
        int a = 31;
        int b = (int)(1e9 + 9);
        long powerOfA = 1;
        long hashValue = 0;

        // Loop to calculate hash value
        for (int i = 0; i < inputText.Length; i++) 
        {
            hashValue = (hashValue + (inputText[i] - 'c' + 1) * powerOfA) % b;
            powerOfA = (powerOfA * a) % b;
        }
        return hashValue;       
    }
}
