using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Gate : MonoBehaviour, InteractableMenu
{
    /// <summary>
    /// True if the gate starts closed, false otherwise.
    /// </summary>
    public bool Closed;

    /// <summary>
    /// The internal open or closed state.
    /// </summary>
    private bool _closed;

    /// <summary>
    /// The gameObject to activate when the gate is opened.
    /// </summary>
    private GameObject _gateOpen;

    /// <summary>
    /// The gameObject to activate when the gate is closed.
    /// </summary>
    private GameObject _gateClose;

    /// <summary>
    /// A menu that sticks to the gate.
    /// </summary>
    private StickyMenu _menu;

    private void Awake()
    {
        // Find some of the components we need.
        _gateOpen = transform.Find("GateOpen").gameObject;
        if (_gateOpen == null)
            Debug.LogWarning("A gate should have a \"GateOpen\" gameObject");
        _gateClose = transform.Find("GateClose").gameObject;
        if (_gateOpen == null)
            Debug.LogWarning("A gate should have a \"GateClose\" gameObject");
        _menu = GetComponentInChildren<StickyMenu>(true);

        // Set the gate to the state specified in the prefab.
        SetClosed(Closed);
    }

    /// <summary>
    /// Open or close the gate.
    /// </summary>
    /// <param name="state">true for closed and false for open</param>
    private void SetClosed(bool state)
    {
        if (_gateOpen != null) _gateOpen.SetActive(!state);
        if (_gateClose != null) _gateClose.SetActive(state);
        _closed = state;
    }

    /// <summary>
    /// Shows the menu belonging to the gate.
    /// </summary>
    public void OpenMenu()
    {
        if (_menu == null) return;
        // Set the button text to the current state.
        var button = _menu.GetComponentInChildren<Button>();
        Assert.IsNotNull(button, "A gate menu needs a button");
        button.GetComponentInChildren<Text>().text = _closed ? "Open" : "Close";
        // Tell the button to toggle the state and the close the menu when clicked.
        button.onClick.AddListener(() =>
        {
            SetClosed(!_closed);
            button.onClick.RemoveAllListeners();
            _menu.gameObject.SetActive(false);
        });
        // Show the menu.
        _menu.gameObject.SetActive(true);
    }
}