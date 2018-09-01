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

    void OnEnable()                                                                 //Runned before start and every time the parrent is enabled
    {
        InitaliseKeybindings();
        InitaliseBoolSettings();
        InitaliseBuildingCost();
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


    private SettingsBool[] SettingsBoolArray;                                       //SettingsBool Array to store the data in
    private void InitaliseBoolSettings()                                            //Init the bool settings (set default values)
    {
        bool[] DefSetting = new bool[JelleWho.BoolSettingsLength];                              //Gets all default settings out of the int (every bit of the INT is now returned as BOOL[#])
        int X = 1;                                                                              //Set the first bit to read to be bit 1
        for (int i = 0; i < DefSetting.Length; i++)                                             //For each item
        {
            if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & X) == X)    //Read the bit
                DefSetting[i] = true;                                                           //Set this one as true
            else
                DefSetting[i] = false;                                                          //Set this one as false
            X *= 2;                                                                             //Select the next bit
        }
        int TotalSettingsBool = JelleWho.BoolSettingsLength;                                    //The total amount of entries, change accordingly in the "ResourceManager"
        SettingsBoolArray = new SettingsBool[TotalSettingsBool];                                //Create a new array with the proper length
        SettingsBoolArray[0] = new SettingsBool("EdgeScroll",   DefSetting[0], "Turn mouse on edge scroll on/off");  //Add some data
        SettingsBoolArray[1] = new SettingsBool("Option",       DefSetting[1], "Description");
    }
    public void SetSetting(int Position, bool SetTo)                                //set a single setting
    {
        SettingsBoolArray[Position].Stat = SetTo;
        if (SetTo)
            PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) | 0x02));  //Set bit TRUE
        else
            PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & ~0x02)); //Set bit FALSE
        Debug.LogWarning("BoolSettings SetSetting || Set " + Position + "to be " + SetTo);
    }
    public SettingsBool GetSetting(int Position)                                    //Returns the current setting (name, stat, desc)
    {
        return new SettingsBool(SettingsBoolArray[Position].Name, SettingsBoolArray[Position].Stat, SettingsBoolArray[Position].Desc);
    }
    public SettingsBool[] GetBoolSettings()                                         //Returns the full array
    {
        return SettingsBoolArray;                                                               //Just return the array as currently is
    }
    public void ResetBoolSettings()                                                 //Reset all settings
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


    private Building[] BuildingCostArray;
    private void InitaliseBuildingCost()                                            //Init the bool settings (set default values)
    {
        int ArrayLength = 32 + 1;
        BuildingCostArray = new Building[ArrayLength];                                          //Create a new array with the proper length
                                                                   //Wood Stone Iron Money 1Free
        BuildingCostArray[0] = new Building("Bridge",                   0,  10, 0,  0,  false); //Add some data
        BuildingCostArray[1] = new Building("Moat",                     0,  0,  0,  2,  false);
        BuildingCostArray[2] = new Building("Mangonel_Tower",           0,  0,  0,  100,false);
        BuildingCostArray[3] = new Building("Balista_Tower",            0,  0,  0,  100,false);
        BuildingCostArray[4] = new Building("Town_Square",              10, 10, 0,  0,  false);
        BuildingCostArray[5] = new Building("Fire_Pit",                 0,  0,  0,  10, false);
        BuildingCostArray[6] = new Building("Trap_Pit",                 5,  0,  0,  0,  false);
        BuildingCostArray[7] = new Building("Castle",                   0,  0,  0,  0,  false);

        BuildingCostArray[8] = new Building("Armory",                   10, 5,  0,  0,  false);
        BuildingCostArray[9] = new Building("Barracks",                 5,  15, 0,  0,  false);
        BuildingCostArray[10] = new Building("Swords_Maker",            0,  0,  0,  150,false);
        BuildingCostArray[11] = new Building("Bow_Maker",               0,  0,  0,  150,false);
        BuildingCostArray[12] = new Building("Spear_Maker",             0,  0,  0,  150,false);
        BuildingCostArray[13] = new Building("Leather_Jacket_Maker",    0,  0,  0,  150,false);
        BuildingCostArray[14] = new Building("Blacksmith_Armor",        0,  0,  0,  150,false);
        BuildingCostArray[15] = new Building("Blacksmith_Tools",        0,  0,  0,  150,false);

        BuildingCostArray[16] = new Building("Wooden_Wall",             1,  0,  0,  0,  false);
        BuildingCostArray[17] = new Building("Wooden_Wall_Spiked",      1,  0,  0,  0,  false);
        BuildingCostArray[18] = new Building("Wooden_Gate",             15, 0,  0,  0,  false);
        BuildingCostArray[19] = new Building("Wooden_Tower",            20, 0,  0,  0,  false);
        BuildingCostArray[20] = new Building("Wooden_Stair",            5,  0,  0,  0,  false);
        BuildingCostArray[21] = new Building("Stone_Wall",              0,  1,  0,  0,  false);
        BuildingCostArray[22] = new Building("Stone_Wall_Spiked",       0,  1,  0,  0,  false);
        BuildingCostArray[23] = new Building("Stone_Gate",              0,  30, 0,  0,  false);
        BuildingCostArray[24] = new Building("Stone_Tower",             0,  40, 0,  0,  false);
        BuildingCostArray[25] = new Building("Stone_Stair",             0,  5,  0,  0,  false);

        BuildingCostArray[26] = new Building("Stockpile",               0,  5,  0,  0,  true );
        BuildingCostArray[27] = new Building("Lumberjack_Hut",          10, 0,  0,  0,  true );
        BuildingCostArray[28] = new Building("Stone_Quarry",            20, 0,  0,  0,  false);
        BuildingCostArray[29] = new Building("Iron_Mine",               20, 0,  0,  0,  false);
        BuildingCostArray[30] = new Building("Ox_Transport",            3,  0,  0,  5,  false);
        BuildingCostArray[31] = new Building("Repair_Building",         10, 5,  0,  0,  false);
        BuildingCostArray[32] = new Building("ddddd",                   0,  0,  0,  0,  false);
    }
    public Building GetInfo(String ItemName)
    {
        for (int i = 0; i < BuildingCostArray.Length; i++)                                      //Do for all objects in the array
        {
            if (BuildingCostArray[i].Name == ItemName)                                          //If this object has the name we want
            {
                return BuildingCostArray[i];                                                    //Return this object
            }
        }
        Debug.LogError("InputManager::GetInfo -- No object named: " + ItemName + " within the BuildingCostArray");  //Log error, object not found
        return new Building("N/A", 255, 255, 255, 255, false);                                  //Just give something 'random' back
    }
}
