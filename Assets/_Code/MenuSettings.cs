using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using PublicCode;
/*
    A little help from @quill18creates at https://www.youtube.com/watch?v=HkmP7raUYi0&t=
    But adapted by JelleWho
 */
public class MenuSettings : MonoBehaviour
{
    void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        // Create one "Key List Item" per button in inputManager
        buttonToLabel = new Dictionary<string, Text>();
        LoadList();                                                         //Put all the entries in the list
    }

    void LoadList()
    {
        SettingsBool[] buttonName = inputManager.GetBoolSettings();         //Gets all button names and plot it in a array
    }

    InputManager inputManager;
    public GameObject keyItemPrefab;
    public GameObject keyList;

    string buttonToRebind = null;
    Dictionary<string, Text> buttonToLabel;

    void Update()
    {
        if (buttonToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                // WHICH key was pressed down?!?

                // Loop through all possible keys and see if it was pressed down
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    // Is this key down?
                    if (Input.GetKeyDown(kc))
                    {
                        // Yes!
                        inputManager.SetButtonForKey(buttonToRebind, kc);
                        buttonToLabel[buttonToRebind].text = System.Convert.ToString(kc);
                        buttonToRebind = null;
                        break;
                    }
                }

            }
        }
    }

    void StartRebindFor(string buttonName)
    {
        buttonToRebind = buttonName;
    }
    public void ResetAllKeys()
    {
        inputManager.ResetAllKeyBindings();
        LoadList();
    }
}
