using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using PublicCode;
/*
    A little help from @quill18creates at https://www.youtube.com/watch?v=HkmP7raUYi0&t=
    But adapted by JelleWho
 */
public class MenuKeybindings : MonoBehaviour {

	// Use this for initialization
	void Start () 
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
        Keys[] KeysArray = inputManager.GetAllKeys();
        for (int i = 0; i < KeysArray.Length; i++)
        {
            string bn;
            bn = KeysArray[i].Name;

            GameObject go = (GameObject)Instantiate(keyItemPrefab);
            go.transform.SetParent(keyList.transform);
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("Button Name").GetComponent<Text>();
            buttonNameText.text = bn;

            Text keyNameText = go.transform.Find("Button/Key Name").GetComponent<Text>();
            keyNameText.text = KeysArray[i].Key_.ToString();
            buttonToLabel[bn] = keyNameText;

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
	void Update () {
	
        if(buttonToRebind != null)
        {
            if(Input.anyKeyDown)
            {
                // WHICH key was pressed down?!?

                // Loop through all possible keys and see if it was pressed down
                foreach(KeyCode kc in Enum.GetValues( typeof(KeyCode) ) )
                {
                    // Is this key down?
                    if(Input.GetKeyDown(kc))
                    {
                        // Yes!
                        inputManager.SetButtonForKey(buttonToRebind, kc );
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
