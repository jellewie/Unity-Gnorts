using UnityEngine;
using System;                                                                           //We need this to convert to bytes
/*  The format buildup of the Save/Load string is as follows
        Each new Line (Lines end at "\r") is a new building that consisting of: (each seperated by ",")
            BuildingName,       Or rather the type name
            Coordinate x,
            Coordinate y,
            Coordinate z,
            Active,             If this building is NOT in sleep mode
            SelectedOption,     Which option is selected (like for Gates)
            Health,             The current health level

        'FlagAsUsedAfterSeconds' and 'Used' are ignored, all buildings are flagged as Used to conserve some data and make things easier
        'MaxHealth' is grabbed from the <BuildingOption>
*/
public class SaveLoad : MonoBehaviour {
    public InputManager CodeInputManager;                                               //The GameObject with the InputManager code on it
    public Transform FolderBuildings;                                                   //The folder where all the buildings should be put in
    public GameObject FolderBuildingPopUp;                                              //The folder with the pop-up stuff in it
    public GameObject[] Objects;                                                        //The array with all the PreFabs in it (doesn't need to be in order)

    public bool LoadFromFile(string FileLocation)                                       //Call this to load from a file, with FileLocation as the location
    {
        return false;                                                                           //Return false, File could not be loaded
    }
    public bool LoadFromSring(string LevelData)                                         //Call this to load a string, With LevelData as the string of data
    {
        foreach (Transform child in FolderBuildings)                                            //For each building
            Destroy(child.gameObject);                                                          //Remove it from this scene
        if (LevelData == "")                                                                    //If the LevelData string is emthy
        {
            Debug.LogWarning("SaveLoad:LoadFromSring - No data has been given");
            return false;                                                                       //Return False; no scene has been loaded
        }
        string[] SplitBlocks = LevelData.Split(new string[] { "\r\n" }, StringSplitOptions.None); //Split at line end (building)
        for (int B = 0; B < SplitBlocks.Length; B++)                                            //Do for each line (building)
        {
            string[] SplitBlockData = SplitBlocks[B].Split(new string[] { "," }, StringSplitOptions.None); //Split at each Data section
            if (SplitBlockData.Length < 8)                                                      //If we dont have enought data to place a building
            {
                Debug.LogWarning("SaveLoad:LoadFromSring - Not enough data to build object at line " + (B + 1) + " I'm Skipping it...");
            }
            else
            {
                string BuildingTypeName = SplitBlockData[0];                                    //Get the building type
                
                for (int i = 0; i < Objects.Length; i++)                                        //Do for all Known buildings
                {
                    if (BuildingTypeName == Objects[i].name)                                    //If this building is the one we need to place
                    {
                        int DataX = System.Convert.ToInt32(SplitBlockData[1]);                  //Get the X coords of this new building
                        int DataY = System.Convert.ToInt32(SplitBlockData[2]);                  //Get the Y coords of this new building
                        int DataZ = System.Convert.ToInt32(SplitBlockData[3]);                  //Get the Z coords of this new building
                        int DataRotation = System.Convert.ToInt32(SplitBlockData[4]);           //Get the rotation of the new building
                        bool DataActive = System.Convert.ToBoolean(SplitBlockData[5]);          //Get if the building should be active or asleep
                        byte DataSelectedOption = System.Convert.ToByte(SplitBlockData[6]);     //Get the current selected mode (Like for the gate if it's open or closed)
                        byte DataHealth = System.Convert.ToByte(SplitBlockData[7]);             //Get the amount of health it should have
                        var a = Instantiate(Objects[i], new Vector3(DataX, DataY, DataZ), Quaternion.Euler(0, DataRotation, 0)); //Create object, place and select it
                        a.transform.SetParent(FolderBuildings);                                 //Sort the object in to the Blocks folder
                        a.GetComponent<BuildingOption>().SetStats(DataActive, DataSelectedOption, DataHealth);//Set the BuildingOption



                        byte SelectedBuildingSpecial = CodeInputManager.GetInfo(a.GetComponent<BuildingOption>().BuildingName).ClickSpecial; //And it's special stats
                        if (SelectedBuildingSpecial > 0)
                        {
                            FolderBuildingPopUp.GetComponent<BuildingPopUp>().ChangeOption(a, SelectedBuildingSpecial, false, DataSelectedOption);
                        }
                        

                        /*
                         * Make a new code sheet that class that changes the building special stats, that is going to be called from this load option. and from the building pop-up window 
                         * Bla Bla Bla Bla Bla Bla just fix it
                         */




                        i = Objects.Length;                                                     //Stop the loop we have found the building
                    }
                    else if (i + 1 == Objects.Length)                                           //If we have come to the end of the list without finding it
                    {
                        Debug.LogWarning("SaveLoad:LoadFromSring - I don't know the building '" + BuildingTypeName + "' I'm Skipping it...");
                    }
                }
            }
        }
        FolderBuildingPopUp.SetActive(false);
        return true;                                                                            //Return true, Scene has been loaden
    }
    public bool SaveToFile(string FileLocation)
    {
        

        return false;                                                                           //Return false, File could not be saved
    }
    public string SaveToSring(string LevelData)
    {
        String ReturnData = "";                                                                 //Create a string to put the data in
        foreach (Transform child in FolderBuildings)                                            //For each building that is in this scene
        {
            string BuildingSaveInfo = System.Convert.ToString(child.GetComponent<BuildingOption>().BuildingName) //Get the BuildingName
                 + "," + child.transform.position.x                                             //Get the X coords of this building
                 + "," + child.transform.position.y                                             //Get the Y coords of this building
                 + "," + child.transform.position.z                                             //Get the Z coords of this building
                 + "," + child.eulerAngles.y                                                    //Get the rotation of the building
                 + "," + child.GetComponent<BuildingOption>().Active                            //Get if the building is active or asleep
                 + "," + child.GetComponent<BuildingOption>().SelectedOption                    //Get the current selected mode (Like for the gate if it's open or closed)
                 + "," + child.GetComponent<BuildingOption>().Health;                           //Get the amount of health it has
            if (ReturnData != "")                                                               //If if this is the first entry
                ReturnData += "\r\n";                                                           //Put a enter after this current data
            ReturnData += BuildingSaveInfo;                                                     //add this data
        }
        return ReturnData;                                                                      //Return the whole string
    }
}
