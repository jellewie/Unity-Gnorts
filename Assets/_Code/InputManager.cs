using UnityEngine;
using System;
using PublicCode;
/*
    Written by JelleWho
 */

public struct ButtonId
{
    public const int Drag = 0;
    public const int Rotate = 1;
    public const int Left = 2;
    public const int Down = 3;
    public const int Right = 4;
    public const int Up = 5;
    public const int RotateLeft = 6;
    public const int RotateRight = 7;
    public const int Menu = 8;
    public const int ToggleUi = 9;
    public const int RotateBuilding = 10;
    public const int Trading = 11;
    public const int Stockpile = 12;
    public const int Granary = 13;
    public const int Church = 14;
    public const int Barracks = 15;
    public const int Castle = 16;
    public const int Pause = 17;
    public const int Build = 18;
    public const int CancelBuild = 19;
    public const int Alternative = 20;
    public const int WallsHigher = 21;
    public const int WallsLower = 22;
};

public class InputManager : MonoBehaviour
{
    void OnEnable()                                                                     //Runned before start and every time the parrent is enabled
    {
        InitaliseKeybindings(false);                                                            //Initalise all the keybindings (and dont reset to default)
        InitaliseBoolSettings();                                                                //Initalise all the settings with bools
    }

    private Keys[] KeysArray;                                                           //KeysArray Array to store the data in
    private void InitaliseKeybindings(bool ToDefault)                                   //Init the bool settings (set default values)
    {
        int ArrayLength = 22 + 1;                                                               //The total amount of entries, change accordingly (+1 since we have a '0' entry too)
        KeysArray = new Keys[ArrayLength];                                                      //Create a new array with the proper length

        KeysArray[ButtonId.Drag]           = new Keys("Drag",             KeyCode.Mouse1,         "Drag the camera"); //Add some data
        KeysArray[ButtonId.Rotate]         = new Keys("Rotate",           KeyCode.Mouse2,         "Rotatate the camera");
        KeysArray[ButtonId.Left]           = new Keys("Left",             KeyCode.A,              "Move camera left");
        KeysArray[ButtonId.Down]           = new Keys("Down",             KeyCode.S,              "Move camera backwards");
        KeysArray[ButtonId.Right]          = new Keys("Right",            KeyCode.D,              "Move camera right");
        KeysArray[ButtonId.Up]             = new Keys("Up",               KeyCode.W,              "Move camera forward");
        KeysArray[ButtonId.RotateLeft]     = new Keys("Rotate left",      KeyCode.E,              "Rotatate the camera a bit left");
        KeysArray[ButtonId.RotateRight]    = new Keys("Rotate right",     KeyCode.Q,              "Rotatate the camera a bit right");
        KeysArray[ButtonId.Menu]           = new Keys("Menu",             KeyCode.Escape,         "Open / close the menu");
        KeysArray[ButtonId.ToggleUi]       = new Keys("Toggle UI",        KeyCode.Tab,            "Make the UI hidden/viseble");
        KeysArray[ButtonId.RotateBuilding] = new Keys("Rotate building",  KeyCode.R,              "");
        KeysArray[ButtonId.Trading]        = new Keys("Trading",          KeyCode.T,              "");
        KeysArray[ButtonId.Stockpile]      = new Keys("Stockpile",        KeyCode.F,              "");
        KeysArray[ButtonId.Granary]        = new Keys("Granary",          KeyCode.G,              "");
        KeysArray[ButtonId.Church]         = new Keys("Church",           KeyCode.H,              "");
        KeysArray[ButtonId.Barracks]       = new Keys("Barracs",          KeyCode.B,              "");
        KeysArray[ButtonId.Castle]         = new Keys("Castle",           KeyCode.C,              "");
        KeysArray[ButtonId.Pause]          = new Keys("Pause",            KeyCode.P,              "");
        KeysArray[ButtonId.Build]          = new Keys("Build",            KeyCode.Mouse0,         "Place the building");
        KeysArray[ButtonId.CancelBuild]    = new Keys("Cancel build",     KeyCode.Mouse1,         "Cancel building");
        KeysArray[ButtonId.Alternative]    = new Keys("Alternative",      KeyCode.LeftShift,      "Continue building & Inverse build rotation");
        KeysArray[ButtonId.WallsHigher]    = new Keys("Walls higher",     KeyCode.KeypadPlus,     "Make the walls higher");
        KeysArray[ButtonId.WallsLower]     = new Keys("Walls lower",      KeyCode.KeypadMinus,    "Make the walls lower");

        if (!ToDefault)                                                                         //If we need to load Playerdata instead of default settings
        {
            for (int i = 0; i < KeysArray.Length; i++)                                          //For each entry in the array
                KeysArray[i].Key_ = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeysArray[i].Name, System.Convert.ToString(KeysArray[i].Key_)));   //Set the key to the Users bind key, or leave it
        }
    }
    public bool GetButtonDown(int ButtonID)                                             //Checks if the button has been pressed
    {
        return Input.GetKey(KeysArray[ButtonID].Key_);                                          //Return the button state
    }
    public bool GetButtonDownOnce(int ButtonID)                                         //Checks if the button is pressed (and flag it as processed)
    {
        return Input.GetKeyDown(KeysArray[ButtonID].Key_);                                     //Return the button state
    }
    /// <summary>
    /// Check if a button has been released this frame.
    /// </summary>
    /// <param name="ButtonID">ID of the button to check</param>
    /// <returns>True during the frame the user releases the button.</returns>
    public bool GetButtonUp(int ButtonID)
    {
        return Input.GetKeyUp(KeysArray[ButtonID].Key_);                                       //Check if key was released
    }
    //public bool GetButtonDown(string buttonName)                                        //Checks if the button has been pressed
    //{
    //    for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
    //    {
    //        if (KeysArray[i].Name == buttonName)                                                //If this is the button we are looking for
    //            return Input.GetKey(KeysArray[i].Key_);                                         //Return the button state
    //    }
    //    Debug.LogError("InputManager::GetButtonDown -- No button named: '" + buttonName + "'"); //Show an error
    //    return false;
    //}
    //public bool GetButtonDownOnce(string buttonName)                                    //Checks if the button is pressed (and flag it as processed)
    //{
    //    for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
    //    {
    //        if (KeysArray[i].Name == buttonName)                                                //If this is the button we are looking for
    //            return Input.GetKeyDown(KeysArray[i].Key_);                                     //Return the button state
    //    }
    //    Debug.LogError("InputManager::GetButtonDown -- No button named: '" + buttonName + "'"); //Show an error
    //    return false;                                                                           //Return False (Could not find button, so it's defenetly not pressed)
    //}
    public bool SetButtonForKey(String buttonName, KeyCode keyCode)                     //Set a keybinding
    {
        bool UsedBefore = false;                                                                //Create a flag with if this key is already mapped for something
        for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
        {
            if (KeysArray[i].Key_ == keyCode)                                                   //If we alread have set a usecase for this key
                UsedBefore = true;                                                              //Flag that we have used this key already
            if (KeysArray[i].Name == buttonName)                                                //If this is the button we are looking for
            {
                for (int j = i + 1; j < KeysArray.Length; j++)                                  //Continue with the loop
                {
                    if (KeysArray[j].Key_ == keyCode)                                           //If we alread have set a usecase for this key
                        UsedBefore = true;                                                      //Flag that we have used this key already
                }
                KeysArray[i].Key_ = keyCode;                                                    //Set the KeyCode, so it will be used in the shortcut
                PlayerPrefs.SetString(KeysArray[i].Name, System.Convert.ToString(keyCode));     //Save the button to Playerprefs
                return UsedBefore;
            }
        }
        Debug.LogError("InputManager::GetButtonDown -- No button named: '" + buttonName + "'"); //Show an error      
        return false;
    }
    public Keys[] GetAllKeys()                                                          //Returns the whole array
    {
        return KeysArray;                                                                       //Return the whole array
    }
    public void ResetAllShotKeys()                                                      //Reset all Keybindings
    {
        InitaliseKeybindings(true);                                                             //Call the init and set set it to default
        for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
        {
            PlayerPrefs.SetString(KeysArray[i].Name, System.Convert.ToString(KeysArray[i].Key_)); //Save the default value to PlayerPrefs
        }
    }

    private SettingsBool[] SettingsBoolArray;                                           //SettingsBool Array to store the data in
    private void InitaliseBoolSettings()                                                //Init the bool settings (set default values)
    {
        bool[] DefSetting = new bool[JelleWho.BoolSettingsLength];                              //Gets all default settings out of the int (every bit of the INT is now returned as BOOL[#])
        int X = 1;                                                                              //Set the first bit to read to be bit 1
        for (int i = 0; i < DefSetting.Length; i++)                                             //For each entry in the array
        {
            if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & X) == X)    //Read the bit
                DefSetting[i] = true;                                                           //Set this one as true
            else
                DefSetting[i] = false;                                                          //Set this one as false
            X *= 2;                                                                             //Select the next bit
        }
        int TotalSettingsBool = JelleWho.BoolSettingsLength;                                    //The total amount of entries, change accordingly in the "PublicCode"
        SettingsBoolArray = new SettingsBool[TotalSettingsBool];                                //Create a new array with the proper length
        SettingsBoolArray[0] = new SettingsBool("EdgeScroll",   DefSetting[0], "Turn mouse on edge scroll on/off");  //Add some data
        SettingsBoolArray[1] = new SettingsBool("Option",       DefSetting[1], "Description");
    }
    public void SetSetting(int Position, bool SetTo)                                    //set a single setting
    {
        SettingsBoolArray[Position].Stat = SetTo;                                               //Update our array with this new info
        if (SetTo)
            PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) | 0x02));  //Set bit TRUE
        else
            PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & ~0x02)); //Set bit FALSE
        Debug.Log("BoolSettings SetSetting || Set " + Position + "to be " + SetTo);
    }
    public SettingsBool GetSetting(int Position)                                        //Returns the current setting (name, stat, desc)
    {
        return new SettingsBool(SettingsBoolArray[Position].Name, SettingsBoolArray[Position].Stat, SettingsBoolArray[Position].Desc);
    }
    public SettingsBool[] GetBoolSettings()                                             //Returns the full array
    {
        return SettingsBoolArray;                                                               //Just return the array as currently is
    }
    public void ResetBoolSettings()                                                     //Reset all settings
    {
        int Hex = 1;                                                                            //Set the first bit to read to be bit 1
        for (int i = 0; i < SettingsBoolArray.Length; i++)                                      //For each item
        {
            if ((JelleWho.BoolSettingsDefault & Hex) == Hex)                                    //Read the bit
            {
                SettingsBoolArray[i].Stat = true;                                               //Set this one as true
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) | Hex));  //Set bit TRUE
                Debug.LogWarning("BoolSettings ResetBoolSettings || Set Hex" + Hex + "to be TRUE");
            }
            else
            {
                SettingsBoolArray[i].Stat = false;                                              //Set this one as false
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & ~Hex)); //Set bit FALSE
                Debug.LogWarning("BoolSettings ResetBoolSettings || Set Hex" + Hex + "to be FALSE");
            }
            Hex *= 2;                                                                           //Select the next bit
        }
    }

}
