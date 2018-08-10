using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using PublicCode;
/*
    Written by JelleWho
 */
public class InputManager : MonoBehaviour
{
    Dictionary<string, KeyCode> buttonKeys;
    private string[] Name;
    private string[] Key_;
    private string[] Desc;

    void OnEnable()                                                                             //Runned before start and every time the parrent is enabled
    {
        InitaliseKeybindings();
        InitaliseBoolSettings();
    }
    private void InitaliseKeybindings()
    {
        int TotalKeys = 20 + 1;                                                                 //The total amount of entries, change accordingly (+1 since we have a '0' entry too)
        Name = new string[TotalKeys];
        Key_ = new string[TotalKeys];
        Desc = new string[TotalKeys];

        //TODO
        //Change Desc to name and add a (hover) description
        Name[0] = "Drag";               Key_[0] = "Mouse1";             Desc[0] = "Drag the camera";
        Name[1] = "Rotate";             Key_[1] = "Mouse2";             Desc[1] = "Rotatate the camera";
        Name[2] = "Left";               Key_[2] = "A";                  Desc[2] = "Move camera left";
        Name[3] = "Down";               Key_[3] = "S";                  Desc[3] = "Move camera backwards";
        Name[4] = "Right";              Key_[4] = "D";                  Desc[4] = "Move camera right";
        Name[5] = "Up";                 Key_[5] = "W";                  Desc[5] = "Move camera forward";
        Name[6] = "Rotate left";        Key_[6] = "E";                  Desc[6] = "Rotatate the camera a bit left";
        Name[7] = "Rotate right";       Key_[7] = "Q";                  Desc[7] = "Rotatate the camera a bit right";
        Name[8] = "Menu";               Key_[8] = "Escape";             Desc[8] = "Open / close the menu";
        Name[9] = "Toggle UI";          Key_[9] = "Tab";                Desc[9] = "Make the UI hidden/viseble";
        Name[10] = "Rotate building";   Key_[10] = "R";                 //Not yet done
        Name[11] = "Trading";           Key_[11] = "T";                 //Not yet done
        Name[12] = "Stockpile";         Key_[12] = "F";                 //Not yet done
        Name[13] = "Granary";           Key_[13] = "G";                 //Not yet done
        Name[14] = "Church";            Key_[14] = "H";                 //Not yet done
        Name[15] = "Barracs";           Key_[15] = "B";                 //Not yet done
        Name[16] = "Castle";            Key_[16] = "C";                 //Not yet done
        Name[17] = "Pause";             Key_[17] = "P";                 //Not yet done
        Name[18] = "Build";             Key_[18] = "Mouse0";            Desc[18] = "Place the building";
        Name[19] = "Cancel build";      Key_[19] = "Mouse1";            Desc[19] = "Cancel building";
        Name[20] = "Alternative";       Key_[20] = "LeftShift";         Desc[20] = "Continue building & Inverse build rotation";
        

        buttonKeys = new Dictionary<string, KeyCode>();
        for (int i = 0; i < Name.Length; i++)                                                   //For each button name
        {
            buttonKeys[Name[i]] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(Name[i], Key_[i]));    //Get the key that should be connected
        }
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
        for (int i = 0; i < Name.Length; i++)                                                   //For each button name
        {
            SetButtonForKey(Name[i], (KeyCode)Enum.Parse(typeof(KeyCode), Key_[i]));            //reset the key
        }
    }







    
    private SettingsBool[] SettingsBoolArray;                                                   //Create a new SettingsBool Array to store the data in
    private void InitaliseBoolSettings()                                            //Init the bool settings (set default values)
    {
        bool[] DefSetting = InitBoolSettings();                                                  //Get default values
        int TotalSettingsBool = JelleWho.BoolSettingsLength;                                    //The total amount of entries, change accordingly in the "ResourceManager"
        SettingsBoolArray = new SettingsBool[TotalSettingsBool];                                //Create a new array with the proper length
        SettingsBoolArray[0] = new SettingsBool("EdgeScroll",   DefSetting[0], "Turn mouse on edge scroll on/off");  //Add some data
        SettingsBoolArray[1] = new SettingsBool("Option",       DefSetting[1], "Description");
    }
    public SettingsBool GetSetting(int ArrayPosition)                               //Returns the current setting (name, stat, desc)
    {
        return new SettingsBool(SettingsBoolArray[ArrayPosition].Name, SettingsBoolArray[ArrayPosition].Stat, SettingsBoolArray[ArrayPosition].Desc);
    }
    public SettingsBool[] GetBoolSettings()                                         //Returns the full array
    {
        return SettingsBoolArray;
    }
    public void ResetBoolSettings()
    {
        int X = 1;
        for (int i = 0; i < SettingsBoolArray.Length; i++)                                      //For each item
        {
            if ((JelleWho.BoolSettingsDefault & X) == X)//Read the bit
                SettingsBoolArray[i].Stat = true;                                               //Set this one as true
            else
                SettingsBoolArray[i].Stat = false;                                              //Set this one as false
            X *= 2;                                                                             //Select the next bit
        }
    }
    public void SetSetting(bool SetTo)
    {

    }



    private bool[] Settings;
    private bool[] InitBoolSettings() //Gets all settings out of the int (every bit of the INT is now returned as BOOL[#])
    {
        Settings = new bool[JelleWho.BoolSettingsLength];
        int X = 1;
        for (int i = 0; i < Settings.Length; i++)                                           //For each item
        {
            if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & X) == X)//Read the bit
                Settings[i] = true;                                                         //Set this one as true
            else
                Settings[i] = false;                                                        //Set this one as false
            X *= 2;                                                                         //Select the next bit
        }
        return Settings.ToArray();                                                                    //Return the array with all the bools; (true, false) etc
    }
}
