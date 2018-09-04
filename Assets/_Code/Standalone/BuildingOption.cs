using UnityEngine;
using System.Collections;                                                               //We need this for 'StartCoroutine'

public class BuildingOption : MonoBehaviour {
    public string BuildingName;                                                                 //The name of the building (used for the lookup)
    public byte FlagAsUsedAfterSeconds;                                                         //Flag the object as used after this amount of seconds (0 to ignore) This will effect the deconstruct return
    public byte MaxHealth = 100;                                                                //The maximum (and start) health of the building

    public bool Used;                                                                           //If this object is used (if not you can get more for removing it)
    public bool Active;                                                                         //Flag false when building, and when sleeping
    public byte SelectedOption;                                                                 //Which option is selected (if this option has options, examples Lumberjack(ox), castle(tax), ox(trapsport), etc
    public byte Health;                                                                         //The current health of th building

    public void StartTimer()                                                            //This code will start the 'Used' after x seconds timer
    {
        Health = MaxHealth;                                                                     //Set the current health level to max
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
    public void _RemoveHealth(byte RemoveAmount)
    {
        Health -= RemoveAmount;
        if (Health == 0)
        {
            _Destroy();
        }
    }
    public void _Destroy()
    {
        //Here could be some code that moved NPC's down and remove a bit from there health  
        Destroy(this.gameObject);
    }
}
