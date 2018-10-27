using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        _gateClose = transform.Find("GateClose").gameObject;
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
        _gateOpen.SetActive(!state);
        _gateClose.SetActive(state);
        _closed = state;
    }

    /// <summary>
    /// Shows the menu belonging to the gate.
    /// </summary>
    public void OpenMenu()
    {
        // Set the button text to the current state.
        var button = _menu.GetComponentInChildren<Button>();
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