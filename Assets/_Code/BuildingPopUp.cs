using System.Collections.Generic;                                                               //We need this for 'List<string>'
using UnityEngine;
using UnityEngine.UI;                                                                           //We need this to interact with the UI

public class BuildingPopUp : MonoBehaviour {

    public GameObject SelectedBuilding;                                                         //The selected building
    byte SelectedBuildingSpecial;                                                               //The special tag of the building
    public Dropdown DropDownMenu;                                                               //This needs to be set to the Dropdown menu itzelf

    void Start()                                                                        //Triggered on start
    {
        DropDownMenu.onValueChanged.AddListener(delegate {InputDropdown(DropDownMenu);});       //Create a listner for this Dropdown menu
    }
    public void SelectBuilding(GameObject Building, byte ClickSpecial, byte ThePlayerID)        //If a building is being selected
    {
        
        if (ThePlayerID == Building.GetComponent<BuildingOption>().OwnerID)                     //If the building is placed by this user
        {
            SelectedBuildingSpecial = ClickSpecial;                                             //Remember the ClickSpecial of this building
            SelectedBuilding = Building;                                                        //Remember this building
            DropDownMenu.ClearOptions();                                                        //Clear the old options of the Dropdown menu
            if (SelectedBuildingSpecial > 0)                                                    //If this building has a special
                ChangeOption(Building, SelectedBuildingSpecial, true, 255);                     //Do the special code
            else
                gameObject.SetActive(false);                                                    //This object doesn't have a special drop down menu, so hide it
        }
        else
        {
            GameObject.Find("UserInput").GetComponent<UserInput>().ShowMessage("You do not own this building");
            this.gameObject.SetActive(false);                                                   //Hide BuildingPopUp
        }
    }
    void InputDropdown(Dropdown change)                                                 //Triggered when the dropdown menu changes
    {
        if (SelectedBuilding != null)
        {
            ChangeOption(SelectedBuilding, SelectedBuildingSpecial, true, System.Convert.ToByte(change.value));
        }
    }
    public void ChangeOption(GameObject Building, byte ClickSpecial, bool PopUp, byte ToOption) //when ToOption 255 = Dont know the option.
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
                DropDownMenu.ClearOptions();                                                    //Clear the old options of the Dropdown menu
                DropDownMenu.AddOptions(new List<string> { "Open", "Close" });                  //Add the options to the dropdown menu
                DropDownMenu.value = ToOption;                                                  //Set the gate to the selected option
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
                ToOption = SelectedBuilding.GetComponent<BuildingOption>().SelectedOption;
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
