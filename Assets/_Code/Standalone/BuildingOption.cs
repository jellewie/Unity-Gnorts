using UnityEngine;

public class BuildingOption : MonoBehaviour {
    public string BuildingName; //The name of the building (used for the lookup)
    public byte SelectedOption; //Which option is selected (if this option has options, examples Lumberjack(ox), castle(tax), ox(trapsport), etc
    public bool Used;           //If this object is used (if not you can get more for removing it)
}
