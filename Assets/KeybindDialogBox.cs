using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class KeybindDialogBox : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();

        // Create one "Key List Item" per button in inputManager

        string[] buttonNames = inputManager.GetButtonNames();
        buttonToLabel = new Dictionary<string, Text>();


        //foreach(string bn in buttonNames)
        for(int i = 0; i < buttonNames.Length; i++)
        {
            string bn;
            bn = buttonNames[i];

            GameObject go = (GameObject)Instantiate(keyItemPrefab);
            go.transform.SetParent( keyList.transform );
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("Button Name").GetComponent<Text>();
            buttonNameText.text = bn;

            Text keyNameText = go.transform.Find("Button/Key Name").GetComponent<Text>();
            keyNameText.text = inputManager.GetKeyNameForButton(bn);
            buttonToLabel[bn] = keyNameText;

            Button keyBindButton = go.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener( () => { StartRebindFor(bn); } );
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
                        inputManager.SetButtonForKey( buttonToRebind, kc );
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
        Debug.Log("StartRebindFor: " + buttonName);

        buttonToRebind = buttonName;
    }
}
