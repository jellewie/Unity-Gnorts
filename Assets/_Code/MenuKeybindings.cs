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

    InputManager inputManager;
    public GameObject PrefabKeyBindItem;
    public GameObject keyList;
    public GameObject TextPressAKey;                                                        //Text to show when a user is doing a keybind
    string buttonToRebind = null;
    Dictionary<string, Text> buttonToLabel;

    private const string MsgAlreadyAssigned =
        "The key <color=orange>{0}</color> is already assigned to the action <color=orange>{1}</color>. Are you sure you want to rebind it to <color=orange>{2}</color>?";

    // Use this for initialization
    void Start () 
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        // Create one "Key List Item" per button in inputManager
        buttonToLabel = new Dictionary<string, Text>();             
        LoadList();                                                                             //Put all the entries in the list
    }

    void LoadList()
    {
        foreach (Transform child in keyList.transform)                                          //For each entry in the list
            GameObject.Destroy(child.gameObject);                                               //Remove the entry
        KeyBinding[] keyBindingArray = inputManager.GetAllKeys();
        for (int i = 0; i < keyBindingArray.Length; i++)
        {
            string bn;
            bn = keyBindingArray[i].Name;

            GameObject go = (GameObject)Instantiate(PrefabKeyBindItem);
            go.transform.SetParent(keyList.transform);
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("Button Name").GetComponent<Text>();
            buttonNameText.text = bn;

            Text keyNameText = go.transform.Find("Button/Key Name").GetComponent<Text>();
            keyNameText.text = System.Convert.ToString(keyBindingArray[i].KeyCode);
            // Mark missing key bindings with red.
            if (keyBindingArray[i].KeyCode == KeyCode.None)
            {
                buttonNameText.color = Color.red;
                keyNameText.text = string.Empty;
            }

            buttonToLabel[bn] = keyNameText;

            Button keyBindButton = go.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener(() => { StartRebindFor(bn); });
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(buttonToRebind != null)
        {
            TextPressAKey.SetActive(true);
            if (Input.anyKeyDown)                                                            //If a key has been pressed
            {
                var pressedKey = InputManager.GetPressedKey();
                // Check if that key is already used.
                string alreadyBound = inputManager.GetButtonForKey(pressedKey);
                if (string.IsNullOrEmpty(alreadyBound))
                {
                    // The key is still free so assign it and reload the ui.
                    inputManager.SetButtonForKey(buttonToRebind, pressedKey);
                    LoadList();
                }
                else if (alreadyBound != buttonToRebind)
                {
                    // Create a modal dialog with a text and two buttons.
                    var text = string.Format(MsgAlreadyAssigned, pressedKey.ToString(), alreadyBound, buttonToRebind);
                    // The yes button will perform the binding and reload the ui.
                    var buttonName = buttonToRebind;
                    var yes = new ModalDialogButton
                    {
                        Label = "Yes", Action = () =>
                        {
                            inputManager.SetButtonForKey(buttonName, pressedKey);
                            LoadList();
                        }
                    };
                    // The no button will do nothing.
                    var no = new ModalDialogButton { Label = "No" };

                    // Show the created dialog.
                    ModalDialog.Instance().ShowDialog(text, new[] {yes, no});
                }
                TextPressAKey.SetActive(false);
                buttonToRebind = null;
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
