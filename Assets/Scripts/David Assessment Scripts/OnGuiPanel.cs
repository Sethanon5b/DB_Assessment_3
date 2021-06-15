using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnGuiPanel : MonoBehaviour
{
    /// <summary>
    /// When the developer mode is enabled, generated GUI buttons will appear that allows access to functionality that would normally be hidden. 
    /// </summary>
    /// 
    private int leftOffset;
    private int topOffset;
    private int buttonWidth;
    private int buttonHeight;
    private int paddingBetween;
    private void OnGUI()
    {
        GUIStyle style = GUI.skin.GetStyle("button");
        style.fontSize = 18;

        if(GameManager.instance.devModeEnabled) 
        {
            // This generates a button that gives money to the account
            if (GUI.Button(new Rect(leftOffset, topOffset, buttonWidth, buttonHeight), "Give Player Money"))
            {
                //DevGiveMoney();
            }

            if (GUI.Button(new Rect(leftOffset + buttonWidth + paddingBetween, topOffset, buttonWidth, buttonHeight), "Clear Equipment"))
            {
                //DevClearEquipment();
            }

            if (GUI.Button(new Rect(leftOffset + buttonWidth * 2 + paddingBetween, topOffset, buttonWidth, buttonHeight), "Wipe Profile"))
            {
                //WipeProfile();
            }
        }
    }

}
