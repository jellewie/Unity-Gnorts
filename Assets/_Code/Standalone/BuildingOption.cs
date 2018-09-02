using UnityEngine;
using System.Collections;                                                               //We need this for 'StartCoroutine'

public class BuildingOption : MonoBehaviour {
    public string BuildingName;                                                                 //The name of the building (used for the lookup)
    public byte SelectedOption;                                                                 //Which option is selected (if this option has options, examples Lumberjack(ox), castle(tax), ox(trapsport), etc
    public bool Used;                                                                           //If this object is used (if not you can get more for removing it)
    public byte FlagAsUsedAfterSeconds;                                                         //Flag the object as used after this amount of seconds (0 to ignore) This will effect the deconstruct return
    public bool Active;                                                                         //Flag false when building, and when sleeping

    public void StartTimer()                                                            //This code will start the 'Used' after x seconds timer
    {
        if (FlagAsUsedAfterSeconds > 0)                                                         //If used has been flagged. This is called upon placement
        {
            StartCoroutine(ExecuteAfterTime(FlagAsUsedAfterSeconds));                           //Start a Coroutine to trigger
        }
    }
    IEnumerator ExecuteAfterTime(float time)                                            //This will be called if the object needs to be flagged as used after x seconds
    {
        yield return new WaitForSeconds(time);                                                  //Only go though if we waited X seconds
        Used = true;                                                                            //Set the building to be used
    }
}
