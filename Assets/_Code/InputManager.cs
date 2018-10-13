using UnityEngine;
using System;
using PublicCode;
/*
    Written by JelleWho
 */
public class InputManager : MonoBehaviour
{
    void OnEnable()                                                                     //Runned before start and every time the parrent is enabled
    {
        InitaliseKeybindings(false);                                                            //Initalise all the keybindings (and dont reset to default)
        InitaliseBoolSettings();                                                                //Initalise all the settings with bools
        InitaliseBuildingCost();                                                                //Initalise the buildingcost array lookup table
    }

    private Keys[] KeysArray;                                                           //KeysArray Array to store the data in
    private void InitaliseKeybindings(bool ToDefault)                                   //Init the bool settings (set default values)
    {
        int ArrayLength = 22 + 1;                                                               //The total amount of entries, change accordingly (+1 since we have a '0' entry too)
        KeysArray = new Keys[ArrayLength];                                                      //Create a new array with the proper length

        KeysArray[0] = new Keys("Drag",             KeyCode.Mouse1,         "Drag the camera"); //Add some data
        KeysArray[1] = new Keys("Rotate",           KeyCode.Mouse2,         "Rotatate the camera");
        KeysArray[2] = new Keys("Left",             KeyCode.A,              "Move camera left");
        KeysArray[3] = new Keys("Down",             KeyCode.S,              "Move camera backwards");
        KeysArray[4] = new Keys("Right",            KeyCode.D,              "Move camera right");
        KeysArray[5] = new Keys("Up",               KeyCode.W,              "Move camera forward");
        KeysArray[6] = new Keys("Rotate left",      KeyCode.E,              "Rotatate the camera a bit left");
        KeysArray[7] = new Keys("Rotate right",     KeyCode.Q,              "Rotatate the camera a bit right");
        KeysArray[8] = new Keys("Menu",             KeyCode.Escape,         "Open / close the menu");
        KeysArray[9] = new Keys("Toggle UI",        KeyCode.Tab,            "Make the UI hidden/viseble");
        KeysArray[10] = new Keys("Rotate building", KeyCode.R,              "");
        KeysArray[11] = new Keys("Trading",         KeyCode.T,              "");
        KeysArray[12] = new Keys("Stockpile",       KeyCode.F,              "");
        KeysArray[13] = new Keys("Granary",         KeyCode.G,              "");
        KeysArray[14] = new Keys("Church",          KeyCode.H,              "");
        KeysArray[15] = new Keys("Barracs",         KeyCode.B,              "");
        KeysArray[16] = new Keys("Castle",          KeyCode.C,              "");
        KeysArray[17] = new Keys("Pause",           KeyCode.P,              "");
        KeysArray[18] = new Keys("Build",           KeyCode.Mouse0,         "Place the building");
        KeysArray[19] = new Keys("Cancel build",    KeyCode.Mouse1,         "Cancel building");
        KeysArray[20] = new Keys("Alternative",     KeyCode.LeftShift,      "Continue building & Inverse build rotation");
        KeysArray[21] = new Keys("Walls higher",    KeyCode.KeypadPlus,     "Make the walls higher");
        KeysArray[22] = new Keys("Walls lower",     KeyCode.KeypadMinus,    "Make the walls lower");

        if (!ToDefault)                                                                         //If we need to load Playerdata instead of default settings
        {
            for (int i = 0; i < KeysArray.Length; i++)                                          //For each entry in the array
                KeysArray[i].Key_ = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeysArray[i].Name, System.Convert.ToString(KeysArray[i].Key_)));   //Set the key to the Users bind key, or leave it
        }
    }
    public bool GetButtonDown(string buttonName)                                        //Checks if the button has been pressed
    {
        for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
        {
            if (KeysArray[i].Name == buttonName)                                                //If this is the button we are looking for
                return Input.GetKey(KeysArray[i].Key_);                                         //Return the button state
        }
        Debug.LogError("InputManager::GetButtonDown -- No button named: '" + buttonName + "'"); //Show an error
        return false;
    }
    public bool GetButtonDownOnce(string buttonName)                                    //Checks if the button is pressed (and flag it as processed)
    {
        for (int i = 0; i < KeysArray.Length; i++)                                              //For each entry in the array
        {
            if (KeysArray[i].Name == buttonName)                                                //If this is the button we are looking for
                return Input.GetKeyDown(KeysArray[i].Key_);                                     //Return the button state
        }
        Debug.LogError("InputManager::GetButtonDown -- No button named: '" + buttonName + "'"); //Show an error
        return false;                                                                           //Return False (Could not find button, so it's defenetly not pressed)
    }
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

    private Building[] BuildingCostArray;                                               //BuildingCostArray Array to store the data in
    private void InitaliseBuildingCost()                                                //Init the bool settings (set default values)
    {
        int ArrayLength = 44 + 1;
        BuildingCostArray = new Building[ArrayLength];                                          //Create a new array with the proper length
                                                                   //Wood, Stone, Iron, Money, 1Free, SpecialBuild, SpecialClick, SpecialDestroy, SpecialSave?
        BuildingCostArray[0]  = new Building("Bridge",                  0,  10, 0,  0,  false,	0,  0,  0); //Add some data
        BuildingCostArray[1]  = new Building("Moat",                    0,  0,  0,  2,  false,	0,  0,  0);
        BuildingCostArray[2]  = new Building("Mangonel_Tower",          0,  0,  0,  100,false,	0,  0,  0);
        BuildingCostArray[3]  = new Building("Balista_Tower",           0,  0,  0,  100,false,	0,  0,  0);
        BuildingCostArray[4]  = new Building("Town_Square",             10, 10, 0,  0,  false,	0,  0,  0);
        BuildingCostArray[5]  = new Building("Fire_Pit",                0,  0,  0,  10, false,	0,  0,  0);
        BuildingCostArray[6]  = new Building("Trap_Pit",                5,  0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[7]  = new Building("Keep",                    0,  0,  0,  0,  false,	0,  2,  0);
                                                                                                     
        BuildingCostArray[8]  = new Building("Armory",                  10, 5,  0,	0,  false,	0,  0,  0);
        BuildingCostArray[9]  = new Building("Barracks",                5,  15, 0,  0,  false,	0,  0,  0);
        BuildingCostArray[10] = new Building("Swords_Maker",            0,  0,  0,  150,false,	0,  0,  0);
        BuildingCostArray[11] = new Building("Bow_Maker",               0,  0,  0,  150,false,	0,  0,  0);
        BuildingCostArray[12] = new Building("Spear_Maker",             0,  0,  0,  150,false,	0,  0,  0);
        BuildingCostArray[13] = new Building("Leather_Jacket_Maker",    0,  0,  0,  150,false,	0,  0,  0);
        BuildingCostArray[14] = new Building("Blacksmith_Armor",        0,  0,  0,  150,false,	0,  0,  0);
        BuildingCostArray[15] = new Building("Blacksmith_Tools",        0,  0,  0,  150,false,	0,  0,  0);
                                                                                                     
        BuildingCostArray[16] = new Building("Wooden_Wall",             1,  0,  0,  0,  false,	1,  0,  0);
        BuildingCostArray[17] = new Building("Wooden_Wall_Spiked",      1,  0,  0,  0,  false,	1,  0,  0);
        BuildingCostArray[18] = new Building("Wooden_Gate",             15, 0,  0,  0,  false,	0,  1,  0);
        BuildingCostArray[19] = new Building("Wooden_Tower",            20, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[20] = new Building("Wooden_Stair",            5,  0,  0,  0,  false,	2,  0,  0);
        BuildingCostArray[21] = new Building("Stone_Wall",              0,  1,  0,  0,  false,	1,  0,  0);
        BuildingCostArray[22] = new Building("Stone_Wall_Spiked",       0,  1,  0,  0,  false,	1,  0,  0);
        BuildingCostArray[23] = new Building("Stone_Gate",              0,  30, 0,  0,  false,	0,  1,  0);
        BuildingCostArray[25] = new Building("Stone_Stair",             0,  5,  0,  0,  false,	2,  0,  0);
        BuildingCostArray[24] = new Building("Stone_Tower",             0,  40, 0,  0,  false,	0,  0,  0);
                                                                                                     
        BuildingCostArray[26] = new Building("Stockpile",               0,  5,  0,  0,  true, 	0,  0,  0);
        BuildingCostArray[27] = new Building("Lumberjack_Hut",          10, 0,  0,  0,  true, 	0,  4,  0);
        BuildingCostArray[28] = new Building("Stone_Quarry",            20, 0,  0,  0,  false,	0,  5,  0);
        BuildingCostArray[29] = new Building("Iron_Mine",               20, 0,  0,  0,  false,	0,  6,  0);
        BuildingCostArray[30] = new Building("Ox_Transport",            3,  0,  0,  5,  false,	0,  3,  0);
        BuildingCostArray[31] = new Building("Repair_Building",         10, 5,  0,  0,  false,	0,  0,  0);
                                                                                                     
        BuildingCostArray[32] = new Building("Granary",                 25, 0,  0,  0,  true,	0,  7,  0);
        BuildingCostArray[33] = new Building("Apple_Farm",              10, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[34] = new Building("Cow_Farm",                20, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[35] = new Building("Hunter",                  10, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[36] = new Building("Wheat_Farm",              10, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[37] = new Building("Mill",                    20, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[38] = new Building("Baker",                   10, 5,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[39] = new Building("Fischer",                 10, 0,  0,  0,  false,	0,  0,  0);
                                                                                                     
        BuildingCostArray[40] = new Building("Home",                    15, 0,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[41] = new Building("Trading_House",           25, 5,  0,  0,  false,	0,  0,  0);
        BuildingCostArray[42] = new Building("Church",                  20, 40, 0,  0,  false,	0,  0,  0);
        BuildingCostArray[43] = new Building("Water_Well",              0,  10, 0,  0,  false,	0,  0,  0);
        BuildingCostArray[44] = new Building("Alchemist",               10, 10, 0,  0,  false,	0,  0,  0);

        /*
        SpecialBuild
        1 Wall
        2 Stair

        SpecialClick
        1 Gate  (gate stats: Open, Close)
    TODO:
        2 Keep  (tax rate: +8, +4, +2, 0, -2, -4, -8)
        3 Ox_Transport
        4 Lumberjack_Hut
        5 Stone_Quarry
        6 Iron_Mine
        7 Granary



        */
    }
    public Building GetInfo(String ItemName)                                            //Get building information about a building
    {
        for (int i = 0; i < BuildingCostArray.Length; i++)                                      //Do for all objects in the array
        {
            if (BuildingCostArray[i].Name == ItemName)                                          //If this object has the name we want
                return BuildingCostArray[i];                                                    //Return this object
        }
        Debug.LogError("InputManager::GetInfo -- No object named: '" + ItemName + "' in the BuildingCostArray");  //Log error, object not found
        return new Building("N/A", 255, 255, 255, 255, false, 0, 0, 0);                         //Just give something 'random' back
    }
}
