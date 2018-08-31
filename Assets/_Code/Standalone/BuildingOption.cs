using UnityEngine;
using System.Collections;                                                           //We need this for 'StartCoroutine'

public class BuildingOption : MonoBehaviour {
    public string BuildingName;                                                                 //The name of the building (used for the lookup)
    public byte SelectedOption;                                                                 //Which option is selected (if this option has options, examples Lumberjack(ox), castle(tax), ox(trapsport), etc
    public bool Used;                                                                           //If this object is used (if not you can get more for removing it)
    public byte FlagAsUsedAfterSeconds;

    private void Start()                                                                //Triggered on start
    {
        if (FlagAsUsedAfterSeconds > 0)                                                         //If used has been flagged
        {
            StartCoroutine(ExecuteAfterTime(FlagAsUsedAfterSeconds));                           //Start a Coroutine to trigger
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);                                          //Only go though if we waited X seconds
        Used = true;                                                                    //Set the building to be used
    }
}
