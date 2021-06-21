using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class HashingScript 
{
    public static string Hash(string data) 
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(data);
        SHA256Managed mySha256 = new SHA256Managed();

        byte[] hashValue = mySha256.ComputeHash(textToBytes);
    }

    private static string GetHexStringFromHash(byte[] hash) 
    {
        string hexString = string.Empty;

        foreach (byte b in hash)
            hexString += b.ToString("x2");

        return hexString;
    }
}
