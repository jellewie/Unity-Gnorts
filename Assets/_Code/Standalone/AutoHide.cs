using UnityEngine;
using System.Collections;                                                               //We need this for 'StartCoroutine'

public class AutoHide : MonoBehaviour {
    public int HideAfterSeconds;                                                                //After this amount of time hide the gameObject
    public bool HideOnBoot;                                                                     //If this gameObject should be hidden on boot

    private void Start()                                                                //Run on boot
    {
        if (HideOnBoot)                                                                         //If we need to hide this object on boot
        {
            this.gameObject.SetActive(false);                                                   //Hide this gameObject                           
        }
    }
    private void OnEnable()                                                             //Run each time the gameObject is enabled
    {
        StartCoroutine(TimeToHide(HideAfterSeconds));                                           //Start a Coroutine to trigger
    }
    IEnumerator TimeToHide(float time)                                                  //This loop will hide the object after X seconds
    {
        yield return new WaitForSeconds(time);                                                  //Only go though if we waited X seconds
        this.gameObject.SetActive(false);                                                       //Hide this gameObject
    }
}
