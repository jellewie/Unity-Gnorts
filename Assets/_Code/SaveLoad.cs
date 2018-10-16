using UnityEngine;
using System;                                                                           //We need this to convert to bytes
using System.IO;                                                                    //Required to read write files with the streamreader and streamwriter
/*  The format buildup of the Save/Load string is as follows
Each new Line (Lines end at "\r") is a new building that consisting of: (each seperated by ",")
BuildingName,       Or rather the type name
Coordinate x,
Coordinate y,
Coordinate z,
Active,             If this building is NOT in sleep mode
SelectedOption,     Which option is selected (like for Gates)
Health,             The current health level
OwnerID,            The owner of this building

'FlagAsUsedAfterSeconds' and 'Used' are ignored, all buildings are flagged as Used to conserve some data and make things easier
'MaxHealth' is grabbed from the <BuildingOption>
*/
public class SaveLoad : MonoBehaviour {
    public InputManager CodeInputManager;                                               //The GameObject with the InputManager code on it
    public Transform FolderBuildings;                                                   //The folder where all the buildings should be put in
    public GameObject FolderBuildingPopUp;                                              //The folder with the pop-up stuff in it
    public GameObject[] Objects;                                                        //The array with all the PreFabs in it (doesn't need to be in order)

    private String SaveFolderPath;                                                      //The save folder location
    private readonly int SaveVersion = 2;
    private byte CampainmapID = 0;  //TODO FIXME, THIS NEEDS TO BE CHANGED IF CAMPAIN GETS INPLEMENTED

    private void Start()
    {
        SaveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");                 //The save folder location
        if (!Directory.Exists(SaveFolderPath))                                                  //If the Save folder does NOT exist
            Directory.CreateDirectory(SaveFolderPath);                                          //Create the folder
    }
    public bool LoadFromFile(string SaveName)                                           //Call this to load from a file, with TheFile as the file name
    {
        SaveName = ValidateName(SaveName);                                                      //Check and edit the name of the given save name to be proper
        String FileFolder = Path.Combine(SaveFolderPath, SaveName);                             //The folder to put the data of the SaveGame in
        String FileBuildings = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "Buildings"); //The file location of the file with the Buildings
        String FileWorld = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "World");       //The file location of the file with the Buildings
        String FileGraph = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "Graph");       //The file location of the file with the Buildings


        if (Directory.Exists(FileFolder))                                                       //If we have a save with this name
        {
            if (File.Exists(FileBuildings))                                                //If we have a FileBuildings file
            {
                StreamReader SR = new StreamReader(FileBuildings);                              //Start writing from a file
                String TextFromTheFile = SR.ReadToEnd();                                        //Get all the text that is in this file
                SR.Close();                                                                     //Close the stream so the file isn't locked anymore
                StringToWorld(TextFromTheFile);                                                 //Create the world with this string
                Debug.Log("Loaded file from " + FileBuildings);
                return true;                                                                    //Return true, succesfull build
            }
            else
                Debug.Log("No Buildins file at " + FileBuildings);
        }
        else
            Debug.Log("No save at  " + FileFolder);
        return false;                                                                           //Return false, File could not be loaded
    }
    public bool SaveToFile(string SaveName, byte OwnerID)                               //Call this to save the world to a file, with TheFile as file name
    {
        SaveName = ValidateName(SaveName);                                                      //Check and edit the name of the given save name to be proper
        String FileFolder = Path.Combine(SaveFolderPath, SaveName);                             //The folder to put the data of the SaveGame in
        String FileBuildings = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "Buildings"); //The file location of the file with the Buildings
        String FileWorld = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "World");       //The file location of the file with the Buildings
        String FileGraph = Path.Combine(Path.Combine(SaveFolderPath, SaveName), "Graph");       //The file location of the file with the Buildings

        if (Directory.Exists(FileFolder))                                                       //If we already have a save with this name
        {
            Debug.Log("File already Exist, saving over it...");
            Directory.Delete(FileFolder, true);                                                 //Delete it, so we can put the new save down
        }
        Debug.Log("Saving to: " + FileFolder);
        Directory.CreateDirectory(FileFolder);                                                  //Create the new folder for the save

        //=== Save Buildings file
        StreamWriter SW = new StreamWriter(FileBuildings);                                      //Start writing to a file
        SW.WriteLine                                                                            //Write some settings to this file
            (SaveVersion
             + "," + OwnerID
             + "," + CampainmapID
             + "," + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "_" + DateTime.Now.Hour + ":" + DateTime.Now.Minute
             + "," + "github.com/jellewie/Unity-Gnorts"
            );
        SW.WriteLine(WorldToString());                                                          //Get and write all buildings to the file
        SW.Close();                                                                             //Close the stream so the file isn't locked anymore
        //=== Save the World data
        SW = new StreamWriter(FileWorld);                                                       //Start writing to a file
        SW.WriteLine("Not Yet Implemented");
        SW.Close();                                                                             //Close the stream so the file isn't locked anymore
        //=== Save the World data
        SW = new StreamWriter(FileGraph);                                                       //Start writing to a file
        SW.WriteLine("Not Yet Implemented");
        SW.Close();                                                                             //Close the stream so the file isn't locked anymore
        //=== end of saving
        return true;                                                                            //Return false, File could not be saved
    }
    private bool StringToWorld(string LevelData)                                        //Call this to build a world from a string
    {
        foreach (Transform child in FolderBuildings)                                            //For each building
            Destroy(child.gameObject);                                                          //Remove it from this scene
        if (LevelData == "")                                                                    //If the LevelData string is emthy
        {
            Debug.LogWarning("SaveLoad:LoadFromSring - No data has been given");
            return false;                                                                       //Return False; no save has been loaded
        }
        string[] SplitBlocks = LevelData.Split(new string[] { "\r\n" }, StringSplitOptions.None); //Split at line end (building)
        string[] SplitBlockData = SplitBlocks[0].Split(new string[] { "," }, StringSplitOptions.None); //Split at each Data section

        Debug.Log("OwnerID of the loaded game = " + SplitBlockData[1]);

        if (SplitBlockData[0] != System.Convert.ToString(SaveVersion))                          //If this Save isn't the latest version
        {
            Debug.LogWarning("SaveLoad:LoadFromSring - Saved File is out of date");
            return false;                                                                       //Return False; no save has been loaded
        }
        for (int B = 1; B < SplitBlocks.Length; B++)                                            //Do for each line (building)
        {
            SplitBlockData = SplitBlocks[B].Split(new string[] { "," }, StringSplitOptions.None); //Split at each Data section
            if (SplitBlockData.Length < 9)                                                      //If we dont have enought data to place a building
            {
                Debug.LogWarning("SaveLoad:LoadFromString - Not enough data to build object at line " + (B + 1) + " I'm Skipping it...");
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
                        byte OwnerID = System.Convert.ToByte(SplitBlockData[8]);                //Get the OwnerID

                        var a = Instantiate(Objects[i], new Vector3(DataX, DataY, DataZ), Quaternion.Euler(0, DataRotation, 0)); //Create object, place and select it
                        a.transform.SetParent(FolderBuildings);                                 //Sort the object in to the Blocks folder
                        a.GetComponent<BuildingOption>().SetStats(DataActive, DataSelectedOption, DataHealth, OwnerID);//Set the BuildingOption

                        byte SelectedBuildingSpecial = CodeInputManager.GetInfo(a.GetComponent<BuildingOption>().BuildingName).ClickSpecial; //And it's special stats
                        if (SelectedBuildingSpecial > 0)                                        //If his building has a special stats
                            FolderBuildingPopUp.GetComponent<BuildingPopUp>().ChangeOption(a, SelectedBuildingSpecial, false, DataSelectedOption); //Do the special code
                        i = Objects.Length;                                                     //Stop the loop we have found the building, and are done with it
                    }
                    else if (i + 1 == Objects.Length)                                           //If we have come to the end of the list without finding it
                    {
                        Debug.LogWarning("SaveLoad: I don't know the building '" + BuildingTypeName + "' I'm Skipping it...");
                    }
                }
            }
        }
        FolderBuildingPopUp.SetActive(false);
        return true;                                                                            //Return true, Scene has been loaden
    }
    private string WorldToString()                                                      //Call this to get the world in string form
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
                 + "," + child.GetComponent<BuildingOption>().Health                            //Get the amount of health it has
                 + "," + child.GetComponent<BuildingOption>().OwnerID;                          //Get who owns this building
            if (ReturnData != "")                                                               //If if this is the first entry
                ReturnData += "\r\n";                                                           //Put a enter after this current data
            ReturnData += BuildingSaveInfo;                                                     //Add this data
        }
        return ReturnData;                                                                      //Return the whole string
    }
    private string ValidateName(String SaveName)
    {
        SaveName = SaveName.Trim();                                                     //Remove spaces in the front and back
        SaveName = SaveName.Replace("[.]", string.Empty);                               //Make sure that it doesn't have a dot in it (So it can't be a extention)
        SaveName = SaveName.ToLower();                                                  //Make sure it's all lower case (just to make sure we dont get double names)
        return SaveName;
    }
    public void ShowSaveFolderInExplorer()
    {
        string itemPath = SaveFolderPath.Replace(@"/", @"\");                           // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
}
