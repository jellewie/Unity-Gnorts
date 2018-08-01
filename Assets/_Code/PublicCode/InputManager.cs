using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour
{
    Dictionary<string, KeyCode> buttonKeys;

    void OnEnable()
    {
        buttonKeys = new Dictionary<string, KeyCode>();

        // TODO:  Consider reading these from a user preferences file
        buttonKeys["Drag"] = KeyCode.Space;         
        buttonKeys["Rotate"] = KeyCode.Mouse2;

        buttonKeys["Right"] = KeyCode.D;
        buttonKeys["Left"] = KeyCode.A;
        buttonKeys["Up"] = KeyCode.W;
        buttonKeys["Down"] = KeyCode.S;
       
    }
    void Start()                        // Use this for initialization
    {
    }
    void Update()                       // Update is called once per frame
    {
    }
    public bool GetButtonDown(string buttonName)
    {
        // TODO: Check to see if the game is supposed to be paused
        //  Or maybe if you're in a different input mode (like a window
        //  is open, or if the player is typing in a text box)

        if (buttonKeys.ContainsKey(buttonName) == false)                                        //If the button is not defined
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);    //Show an error
            return false;                                                                       //Return false, since the non existing button isn't pressed
        }
        return Input.GetKey(buttonKeys[buttonName]);                                            //Return the button state
    }
    public string[] GetButtonNames()
    {
        return buttonKeys.Keys.ToArray();
    }
    public string GetKeyNameForButton(string buttonName)
    {
        if (buttonKeys.ContainsKey(buttonName) == false)
        {
            Debug.LogError("InputManager::GetKeyNameForButton -- No button named: " + buttonName);
            return "N/A";
        }

        return buttonKeys[buttonName].ToString();
    }
    public void SetButtonForKey(string buttonName, KeyCode keyCode)
    {
        buttonKeys[buttonName] = keyCode;
    }
}
