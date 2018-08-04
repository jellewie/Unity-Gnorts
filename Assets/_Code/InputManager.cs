﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class InputManager : MonoBehaviour
{
    Dictionary<string, KeyCode> buttonKeys;
    private string[] Desc;
    private string[] Key_;
    void OnEnable()                                                                             //Runned before start and every time the parrent is enabled
    {
        int TotalKeys = 17 + 1;                                                                 //The total amount of keys, change accordingly (+1 since we have a '0' entry too)
        Desc = new string[TotalKeys];
        Key_ = new string[TotalKeys];
        Desc[0] = "Drag";               Key_[0] = "Mouse1";
        Desc[1] = "Rotate";             Key_[1] = "Mouse2";
        Desc[2] = "Left";               Key_[2] = "A";
        Desc[3] = "Down";               Key_[3] = "S";
        Desc[4] = "Right";              Key_[4] = "D";
        Desc[5] = "Up";                 Key_[5] = "W";
        Desc[6] = "Rotate left";        Key_[6] = "E";
        Desc[7] = "Rotate right";       Key_[7] = "Q";
        Desc[8] = "Menu";               Key_[8] = "Escape";
        Desc[9] = "Toggle UI";          Key_[9] = "Tab";

        Desc[10] = "Rotate building";   Key_[10] = "R";
        Desc[11] = "Trading";           Key_[11] = "T";
        Desc[12] = "Stockpile";         Key_[12] = "F";
        Desc[13] = "Granary";           Key_[13] = "G";
        Desc[14] = "Church";            Key_[14] = "H";
        Desc[15] = "Barracs";           Key_[15] = "B";
        Desc[16] = "Castle";            Key_[16] = "C";
        Desc[17] = "Pause";             Key_[17] = "P";

        buttonKeys = new Dictionary<string, KeyCode>();
        for (int i = 0; i < Desc.Length; i++)                                                   //For each button name
        {
            buttonKeys[Desc[i]] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(Desc[i], Key_[i]));    //Get the key that should be connected
        }
    }
    void Start()                                                                                //Use this for initialization
    {
    }
    void Update()                                                                               //Update is called once per frame
    {
    }

    public bool GetButtonDown(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)                                        //If the button is not defined
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);    //Show an error
            return false;                                                                       //Return false, since the non existing button isn't pressed
        }
        return Input.GetKey(buttonKeys[buttonName]);                                            //Return the button state
    }
    public bool GetButtonDownOnce(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)                                        //If the button is not defined
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);    //Show an error
            return false;                                                                       //Return false, since the non existing button isn't pressed
        }
        return Input.GetKeyDown(buttonKeys[buttonName]);                                            //Return the button state
    }

    public string[] GetButtonNames()
    {
        return buttonKeys.Keys.ToArray();
    }
    public string GetKeyNameForButton(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)
        {
            Debug.LogError("InputManager::GetKeyNameForButton -- No button named: " + buttonName);
            return "N/A";
        }
        return buttonKeys[buttonName].ToString();
    }
    public void SetButtonForKey(string buttonName, KeyCode keyCode)
    {
        buttonKeys[buttonName] = keyCode;                                                       //Set the KeyCode, so it will be used in the shortkut
        PlayerPrefs.SetString(buttonName, keyCode.ToString());                                  //Save the button to Playerprefs
    }
    public void ResetAllShotKeys()
    {
        for (int i = 0; i < Desc.Length; i++)                                                   //For each button name
        {
            SetButtonForKey(Desc[i], (KeyCode)Enum.Parse(typeof(KeyCode), Key_[i]));            //reset the key
        }
    }
}