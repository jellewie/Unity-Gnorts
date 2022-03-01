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

    private KeyBinding[] _keyBindings;                                                          //KeysArray Array to store the data in
    private void InitaliseKeybindings(bool ToDefault)                                           //Init the bool settings (set default values)
    {
        int ArrayLength = 22 + 1;                                                               //The total amount of entries, change accordingly (+1 since we have a '0' entry too)
        _keyBindings = new KeyBinding[ArrayLength];                                                      //Create a new array with the proper length

        _keyBindings[ButtonId.Drag]           = new KeyBinding("Drag",             KeyCode.Mouse1,         "Drag the camera"); //Add some data
        _keyBindings[ButtonId.Rotate]         = new KeyBinding("Rotate",           KeyCode.Mouse2,         "Rotate the camera");
        _keyBindings[ButtonId.Left]           = new KeyBinding("Left",             KeyCode.A,              "Move camera left");
        _keyBindings[ButtonId.Down]           = new KeyBinding("Down",             KeyCode.S,              "Move camera backwards");
        _keyBindings[ButtonId.Right]          = new KeyBinding("Right",            KeyCode.D,              "Move camera right");
        _keyBindings[ButtonId.Up]             = new KeyBinding("Up",               KeyCode.W,              "Move camera forward");
        _keyBindings[ButtonId.RotateLeft]     = new KeyBinding("Rotate left",      KeyCode.E,              "Rotate the camera a bit left");
        _keyBindings[ButtonId.RotateRight]    = new KeyBinding("Rotate right",     KeyCode.Q,              "Rotate the camera a bit right");
        _keyBindings[ButtonId.Menu]           = new KeyBinding("Menu",             KeyCode.Escape,         "Open / close the menu");
        _keyBindings[ButtonId.ToggleUi]       = new KeyBinding("Toggle UI",        KeyCode.Tab,            "Make the UI hidden/visible");
        _keyBindings[ButtonId.RotateBuilding] = new KeyBinding("Rotate building",  KeyCode.R,              "");
        _keyBindings[ButtonId.Trading]        = new KeyBinding("Trading",          KeyCode.T,              "");
        _keyBindings[ButtonId.Stockpile]      = new KeyBinding("Stockpile",        KeyCode.F,              "");
        _keyBindings[ButtonId.Granary]        = new KeyBinding("Granary",          KeyCode.G,              "");
        _keyBindings[ButtonId.Church]         = new KeyBinding("Church",           KeyCode.H,              "");
        _keyBindings[ButtonId.Barracks]       = new KeyBinding("Barracks",         KeyCode.B,              "");
        _keyBindings[ButtonId.Castle]         = new KeyBinding("Castle",           KeyCode.C,              "");
        _keyBindings[ButtonId.Pause]          = new KeyBinding("Pause",            KeyCode.P,              "");
        _keyBindings[ButtonId.Build]          = new KeyBinding("Build",            KeyCode.Mouse0,         "Place the building");
        _keyBindings[ButtonId.CancelBuild]    = new KeyBinding("Cancel build",     KeyCode.Mouse1,         "Cancel building");
        _keyBindings[ButtonId.Alternative]    = new KeyBinding("Alternative",      KeyCode.LeftShift,      "Continue building & Inverse build rotation");
        _keyBindings[ButtonId.WallsHigher]    = new KeyBinding("Walls higher",     KeyCode.KeypadPlus,     "Make the walls higher");
        _keyBindings[ButtonId.WallsLower]     = new KeyBinding("Walls lower",      KeyCode.KeypadMinus,    "Make the walls lower");
                                                                                
        if (!ToDefault) {                                                                       //If we need to load Playerdata instead of default settings
            for (int i = 0; i < _keyBindings.Length; i++)                                       //For each entry in the array
            {
                var key = _keyBindings[i];
                key.KeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key.Name, Convert.ToString(key.KeyCode)));   //Set the key to the Users bind key, or leave it
            }
        }
    }
    public bool GetButtonDown(int ButtonID)                                             //Checks if the button has been pressed
    {
        return Input.GetKey(_keyBindings[ButtonID].KeyCode);                                          //Return the button state
    }
    public bool GetButtonDownOnce(int ButtonID)                                         //Checks if the button is pressed (and flag it as processed)
    { 
            return Input.GetKeyDown(_keyBindings[ButtonID].KeyCode);                                     //Return the button state  
    }
    /// <summary>
    /// Check if a button has been released this frame.
    /// </summary>
    /// <param name="ButtonID">ID of the button to check</param>
    /// <returns>True during the frame the user releases the button.</returns>
    public bool GetButtonUp(int ButtonID)
    {
        return Input.GetKeyUp(_keyBindings[ButtonID].KeyCode);                                       //Check if key was released
    }

    /// <summary>
    /// Bind a key to a button.
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <param name="keyCode">The code of the key</param>
    public void SetButtonForKey(string buttonName, KeyCode keyCode)
    {
        // If the key is already bound to something else, we unbind it.
        string alreadyBound = GetButtonForKey(keyCode);
        if (!string.IsNullOrEmpty(alreadyBound))
        {
            PlayerPrefs.SetString(alreadyBound, "None");
        }
        // Save the new binding.
        PlayerPrefs.SetString(buttonName, Convert.ToString(keyCode));
        InitaliseKeybindings(false);
    }

    /// <summary>
    /// Get the name of the button assigned to a keycode.
    /// </summary>
    /// <param name="key">The keycode to check.</param>
    /// <returns>The name of the button or an empty string if not found.</returns>
    public string GetButtonForKey(KeyCode key)
    {
        foreach (var keyBinding in _keyBindings)
        {
            if (keyBinding.KeyCode == key)
            {
                return keyBinding.Name;
            }
        }

        return string.Empty;
    }
    
    public KeyBinding[] GetAllKeys()                                                            //Returns the whole array
    {
        return _keyBindings;                                                                    //Return the whole array
    }
    public void ResetAllKeyBindings()                                                      //Reset all Keybindings
    {
        InitaliseKeybindings(true);                                                             //Call the init and set set it to default
        for (int i = 0; i < _keyBindings.Length; i++)                                           //For each entry in the array
        {
            PlayerPrefs.SetString(_keyBindings[i].Name, Convert.ToString(_keyBindings[i].KeyCode)); //Save the default value to PlayerPrefs
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

    /// <summary>
    /// Find out which key is pressed.
    /// </summary>
    /// <returns>The keycode of the pressed key</returns>
    public static KeyCode GetPressedKey()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
                return key;
        }

        return KeyCode.None;
    }
}
