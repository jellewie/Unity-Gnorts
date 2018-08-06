using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MenuSettings : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        // Create one "Key List Item" per button in inputManager
        buttonToLabel = new Dictionary<string, Text>();
        LoadList();                                             //Put all the entries in the list
    }


    void LoadList()
    {
        foreach (Transform child in keyList.transform)                                          //For each entry in the list
        {
            GameObject.Destroy(child.gameObject);                                               //Remove the entry
        }
        string[] buttonName = inputManager.GetButtonNames();                                    //Gets all button names and plot it in a array
        for (int i = 0; i < buttonName.Length; i++)                                             //For each button name
        {
            string bn;
            bn = buttonName[i];

            GameObject go = (GameObject)Instantiate(keyItemPrefab);
            go.transform.SetParent(keyList.transform);
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("Button Name").GetComponent<Text>();
            buttonNameText.text = bn;


            //TODO set the state of the toggle

            //Text keyNameText = go.transform.Find("Toggle").GetComponent<Text>();
            //keyNameText.text = inputManager.GetKeyNameForButton(bn);
            //buttonToLabel[bn] = keyNameText;

            Button keyBindButton = go.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener(() => { StartRebindFor(bn); });
        }
    }
    InputManager inputManager;
    public GameObject keyItemPrefab;
    public GameObject keyList;

    string buttonToRebind = null;
    Dictionary<string, Text> buttonToLabel;

    // Update is called once per frame
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
                        buttonToLabel[buttonToRebind].text = kc.ToString();
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
        inputManager.ResetAllShotKeys();
        LoadList();
    }
}
