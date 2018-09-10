using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                                                                               //We need this to interact with the UI

public class BuildingPopUp : MonoBehaviour {

    public GameObject SelectedBuilding;                                                         //The selected building
    byte SelectedBuildingSpecial;                                                               //The special tag of the building
    public Dropdown DropDownMenu;                                                               //This needs to be set to the Dropdown menu itzelf
    

    void Start()                                                                        //Triggered on star
    {
        DropDownMenu.onValueChanged.AddListener(delegate {InputDropdown(DropDownMenu);});       //Create a listner for this Dropdown menu
    }
    public void SelectBuilding(GameObject Building, byte ClickSpecial)                  //If a building is being selected
    {
        SelectedBuildingSpecial = ClickSpecial;                                                 //Remember the ClickSpecial of this building
        SelectedBuilding = Building;                                                            //Remember this building
        DropDownMenu.ClearOptions();                                                            //Clear the old options of the Dropdown menu
        if (SelectedBuildingSpecial == 1)                                                       //If this is a Gate GameObject
        {                                                                                       
            DropDownMenu.AddOptions(new List<string> { "Open", "close" });                      //Add the options to the dropdown menu
            GameObject Gate = SelectedBuilding.transform.Find("Gate").gameObject;               //Get the gate
            if (Gate.activeSelf)                                                                //If the gate is active
                DropDownMenu.value = 1;                                                         //Set the value to 1 (Open)
            else                                                                                
                DropDownMenu.value = 0;                                                         //Set the value to 0 (Close)
            SelectedBuilding.GetComponent<BuildingOption>().SelectedOption = System.Convert.ToByte(DropDownMenu.value);
        }                                                                                       
        else                                                                                    
            this.gameObject.SetActive(false);                                                   //This object doesn't have a dropdown menu, so hide 
    }
    void InputDropdown(Dropdown change)                                                 //Triggered when the dropdown menu changes
    {
        if (SelectedBuildingSpecial == 1)                                                       //If this is a Gate
        {
            GameObject Gate = SelectedBuilding.transform.Find("Gate").gameObject;               //Get the Gate GameObject
            if (change.value == 0)                                                              //If value has been changed to 0 (Close Gate)
                Gate.SetActive(false);                                                          //Hide Gate
            else
                Gate.SetActive(true);                                                           //Show Gate
            SelectedBuilding.GetComponent<BuildingOption>().SelectedOption = System.Convert.ToByte(DropDownMenu.value);
        }
    }
}
