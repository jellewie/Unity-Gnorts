using System.Collections.Generic;                                                               //We need this for 'List<string>'
using UnityEngine;
using UnityEngine.UI;                                                                           //We need this to interact with the UI

public class BuildingPopUp : MonoBehaviour {

    public List<GameObject> SelectedBuildings = new List<GameObject>();
    byte SelectedBuildingSpecial;                                                               //The special tag of the building
    public Dropdown DropDownMenu;                                                               //This needs to be set to the Dropdown menu itzelf

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
            FolderGate = transform.Find("Gate").gameObject;
            //Other references
        }
    }
    public void SelectBuilding(GameObject Building, byte ClickSpecial, byte ThePlayerID)        //If a building is being selected
    {
        if (ThePlayerID == Building.GetComponent<BuildingOption>().OwnerID)                     //If the building is placed by this user
        {
            if (SelectedBuildingSpecial == 0)                                                   //If this building has a special
            {
                gameObject.SetActive(false);                                                    //This object doesn't have a special drop down menu, so hide it
                if (ClickSpecial > 0)
                    SelectedBuildingSpecial = ClickSpecial;                                     //Remember the ClickSpecial of this 
            }
            if (SelectedBuildingSpecial == ClickSpecial)
            {
                SelectedBuildings.Add(Building);                                                //Add this building to our list

                bool Multi = false;
                if (SelectedBuildings.Count > 1)
                    Multi = true;
                if (SelectedBuildingSpecial == 1)
                {
                    if (Multi)
                        FolderGate.GetComponentInChildren<Text>().text = "Multible Gates";
                    else
                        FolderGate.GetComponentInChildren<Text>().text = SelectedBuildings[0].GetComponent<BuildingOption>().BuildingName;
                    FolderGate.SetActive(true);
                    ChangeOption(SelectedBuildings[0], SelectedBuildingSpecial, true, 255, Multi);
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
            GameObject GateOpen = Building.transform.Find("GateOpen").gameObject;               //Get the gate
            if (ToOption < 255)                                                                 //If we need to go to an option
            {
                GameObject GateClose = Building.transform.Find("GateClose").gameObject;         //Get the gate
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
                gameObject.transform.Find("Gate/Open").gameObject.GetComponent<Button>().interactable = true;
                gameObject.transform.Find("Gate/Close").gameObject.GetComponent<Button>().interactable = true;
                if (!Multible)
                {
                    if (ToOption == 0)
                        gameObject.transform.Find("Gate/Open").gameObject.GetComponent<Button>().interactable = false;
                    else
                        gameObject.transform.Find("Gate/Close").gameObject.GetComponent<Button>().interactable = false;
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
