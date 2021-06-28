using System.Text;
public static class HashingScript 
{
    /// This script is the main functionality of the hashing script
    /// When the string is defined by Hash test, this script will assigned a value to each letter, so it will become encrypted. 
    // Source of the code : https://www.youtube.com/watch?v=HoEndcK6Bew
    public static string Encryption(string inputText, int key) 
    {
        StringBuilder outSB = new StringBuilder(inputText.Length);
        for(int i = 0; i < inputText.Length; i++) 
        {
            char ch = (char)(inputText[i] ^ key);
            outSB.Append(ch);
        }
        return outSB.ToString();
    }
}
