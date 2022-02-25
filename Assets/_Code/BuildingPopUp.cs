using System.Collections.Generic;                                                               //We need this for 'List<string>'
using UnityEngine;
using UnityEngine.UI;                                                                           //We need this to interact with the UI

public class BuildingPopUp : MonoBehaviour {

    public List<GameObject> SelectedBuildings = new List<GameObject>();
    byte SelectedBuildingSpecial;                                                               //The special tag of the building
    public Dropdown DropDownMenu;                                                               //This needs to be set to the Dropdown menu itzelf
    public GameObject DisplayGameObjectInformation;

    GameObject FolderGate;

    void OnDisable()
    {
        SelectedBuildings = new List<GameObject>();                                             //Make sure the list is emthy
        SelectedBuildingSpecial = 0;                                                            //Reset
        FolderGate.SetActive(false);
    }
    private void OnEnable()
    {
        if (FolderGate == null)                                                                 //If no reffrence is yet set
        {
            FolderGate = transform.Find("Gate").gameObject;                                     //Set the reference to the Gate folder
            //Other references
        }
    }
    public void SelectBuilding(GameObject Building, byte ClickSpecial, byte ThePlayerID)        //If a building is being selected
    {
        if (ThePlayerID == Building.GetComponent<BuildingOption>().OwnerID)                     //If the building is placed by this user
        {
            if (SelectedBuildingSpecial == 0)                                                   //If this building has a special
            {
                if (ClickSpecial == 0)                                                          //If this building has no ClickSpecial
                    gameObject.SetActive(false);                                                //This object doesn't have a pop-up menu, so hide it
                else
                    SelectedBuildingSpecial = ClickSpecial;                                     //Save the selected buildin(s) special tag
            }
            if (SelectedBuildingSpecial == ClickSpecial)                                        //If this building has the same tag as stored (for multiple selection)
            {
                SelectedBuildings.Add(Building);                                                //Add this building to our list
                bool Multiple = false;                                                          //Default to no multiple selection
                if (SelectedBuildings.Count > 1)                                                //If we have selected more than 1
                    Multiple = true;                                                            //Flag we have multiple buildings
                if (SelectedBuildingSpecial == 1)                                               //If this is a Gate
                {
                    if (Multiple)                                                               //If we have multiple buildings
                        FolderGate.GetComponentInChildren<Text>().text = "Multiple Gates";      //Change the text to reflect that
                    else
                        FolderGate.GetComponentInChildren<Text>().text = Building.GetComponent<BuildingOption>().BuildingName; //Set the building name in the pop-up window 
                    FolderGate.SetActive(true);                                                 //Show the gate folder
                    ChangeOption(SelectedBuildings[0], SelectedBuildingSpecial, true, 255, Multiple); //Load the settings 
                }
            }
        }
        else
        {
            GameObject.Find("UserInput").GetComponent<UserInput>().ShowMessage("You do not own this building");
            gameObject.SetActive(false);                                                        //Hide BuildingPopUp
        }
    }
    public void ChangeOption(int ChangeTo)
    {
        for (int i = 0; i < SelectedBuildings.Count; i++)
        {
            ChangeOption(SelectedBuildings[i], SelectedBuildingSpecial, true, System.Convert.ToByte(ChangeTo), false);
        }
    }
    public void ChangeOption(GameObject Building, byte ClickSpecial, bool PopUp, byte ToOption, bool Multible) //When ToOption 255 = Dont know the option.
    {
        if (ClickSpecial == 1)                                                                  //If it's a Gate
        {
            GameObject GateOpen = Building.transform.Find("Mesh").gameObject.transform.Find("GateOpen").gameObject;  //Get the gate... // Lazy fix after the prefab changed
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                GameObject GateClose = Building.transform.Find("Mesh").gameObject.transform.Find("GateClose").gameObject;   //Get the gate... // Lazy fix after the prefab changed
                if (ToOption == 0)                                                              //If value has been changed to 0 (Close Gate)
                {
                    GateOpen.SetActive(true);                                                   //Hide the closed gate
                    GateClose.SetActive(false);                                                 //Show the open gate
                }
                else
                {
                    GateOpen.SetActive(false);                                                  //Show the closed gate
                    GateClose.SetActive(true);                                                  //Hide the open gate
                }
            }
            else
            {
                if (GateOpen.activeSelf)                                                        //If the gate is active
                    ToOption = 0;                                                               //Set the value to 1 (Open)
                else
                    ToOption = 1;                                                               //Set the value to 0 (Closed)
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                gameObject.transform.Find("Gate/Open") .gameObject.GetComponent<Button>().interactable = true; //By default enable the button
                gameObject.transform.Find("Gate/Close").gameObject.GetComponent<Button>().interactable = true; //By default enable the button
                if (!Multible)                                                                  //If we have multible buildings
                {
                    if (ToOption == 0)                                                          //If the Gate should be in stage '0'
                        gameObject.transform.Find("Gate/Open") .gameObject.GetComponent<Button>().interactable = false; //Disable this button, gate already in this stage
                    else
                        gameObject.transform.Find("Gate/Close").gameObject.GetComponent<Button>().interactable = false; //Disable this button, gate already in this stage
                }
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;                  //Update the Building with this info
        }
        else if (ClickSpecial == 2)                                                             //If it's a Keep
        {
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                //If option has been given, and we need to do something custom (Like; gate is placed, open gate)
            }
            else
            {
                ToOption = Building.GetComponent<BuildingOption>().SelectedOption;
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> {"+8 tax -8 happiness", "+4 tax -4 happiness", "+2 tax -2 happiness", "no tax +1 happiness", "-2 tax +2 happiness", "-2 tax +2 happiness", "-4 tax +4 happiness", "-8 tax +8 happiness"}); //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;          //Update the Building with this info
        }
        else if (ClickSpecial == 3)                                                              //If it's a Ox_Transport
        {
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                //If option has been given, and we need to do something custom (Like; gate is placed, open gate)
            }
            else
            {
                ToOption = Building.GetComponent<BuildingOption>().SelectedOption;
                if(ToOption == 254)
                {
                    //More code, see issue #61
                }
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> {"Wood", "Stone", "Iron"});            //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;                  //Update the Building with this info
        }
        else if (ClickSpecial == 4)                                                             //If it's a Lumberjack_Hut || Stone_Quarry || Iron_Mine
        {
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                //If option has been given, and we need to do something custom (Like; gate is placed, open gate)
            }
            else
            {
                ToOption = Building.GetComponent<BuildingOption>().SelectedOption;
                if (ToOption == 254)
                {
                    //More code, see issue #62
                }
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> {"Ox tether", "Move by itself"});      //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;                  //Update the Building with this info
        }
        else if (ClickSpecial == 5)                                                             //If it's a Granary
        {
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                //If option has been given, and we need to do something custom (Like; gate is placed, open gate)
            }
            else
            {
                //Unknow status, read the building state (like 'ToOption = Gate is open')
                ToOption = SelectedBuildings[0].GetComponent<BuildingOption>().SelectedOption;
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> { "No food (-8 happiness)", "Half rations (-4 happiness)", "Normal", "Extra rations (+4 happiness)", "Double rations (+8 happiness)" });           //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;                  //Update the Building with this info
        }
        else if (ClickSpecial == 255)                                                           //If it's a XXXX
        {
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                //If option has been given, and we need to do something custom (Like; gate is placed, open gate)
            }
            else
            {
                //Unknow status, read the building state (like 'ToOption = Gate is open')
                //ToOption = SelectedBuilding.GetComponent<BuildingOption>().SelectedOption;
            }
            if (PopUp)                                                                          //If we need a PopUp window
            {
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> { "Option 1", "Option 2" });           //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
            }
            Building.GetComponent<BuildingOption>().SelectedOption = ToOption;                  //Update the Building with this info
        }
    }
}
