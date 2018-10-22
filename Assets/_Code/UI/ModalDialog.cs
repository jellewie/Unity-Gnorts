using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalDialog : MonoBehaviour
{
    public Text DialogText;
    public Button DialogButton; 
    public GameObject ModalPanel;
    public GameObject ButtonPanel;

    private static ModalDialog _modalPanel;

    public static ModalDialog Instance()
    {
        if (_modalPanel) return _modalPanel;

        _modalPanel = FindObjectOfType(typeof(ModalDialog)) as ModalDialog;
        if (!_modalPanel)
            Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");

        return _modalPanel;
    }

    /// <summary>
    /// Show a modal dialog with a text and one or more buttons.
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="buttons">An array of buttons with labels and actions</param>
    public void ShowDialog(string text, ModalDialogButton[] buttons)
    {
        ModalPanel.SetActive(true);
        DialogText.text = text;

        // Create a button for each passed in button.
        foreach (var buttonInfo in buttons)
        {
            // Create a new instance of the referenced button prefab.
            var button = Instantiate(DialogButton, ButtonPanel.transform);
            button.GetComponentInChildren<Text>().text = buttonInfo.Label;
            button.onClick.AddListener(buttonInfo.Action);
            button.onClick.AddListener(ClosePanel);    
        }
    }

    private void ClosePanel()
    {
        // Destroy all the buttons
        foreach (var button in ButtonPanel.GetComponentsInChildren<Button>())
        {
           Destroy(button.gameObject);
        }      
        // And close the modal. 
        ModalPanel.SetActive(false);
    }
}

public class ModalDialogButton
{
    public string Label;
    public UnityAction Action = () => { };
}
