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
    public GameObject TextPressAKey;                                                        //Text to show when a user is doing a keybind
    string buttonToRebind = null;
    Dictionary<string, Text> buttonToLabel;
	
	// Update is called once per frame
	void Update () {
        if(buttonToRebind != null)
        {
            TextPressAKey.SetActive(true);
            if (Input.anyKeyDown)                                                            //If a key has been pressed
            {
                foreach(KeyCode kc in Enum.GetValues( typeof(KeyCode)))                     //ForEach posible key
                {
                    if(Input.GetKeyDown(kc))                                                //Check if this key is pressed down
                    {
                        if(inputManager.SetButtonForKey(buttonToRebind, kc))                //Set the key, and check if this key has been used before
                        {
                            buttonToLabel[buttonToRebind].color = new Color(1f, 0.5f, 0.16f);  //Warn user that this key has more than one function
                        }
                        else
                        {
                            buttonToLabel[buttonToRebind].color = Color.black;              //Key hasn't been used for something, so just make the color default black
                        }
                        buttonToLabel[buttonToRebind].text = kc.ToString();                 //Set the 
                        buttonToRebind = null;
                        TextPressAKey.SetActive(false);
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
