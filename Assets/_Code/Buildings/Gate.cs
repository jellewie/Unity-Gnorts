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

    private bool _closed;
    private GameObject _gateOpen;
    private GameObject _gateClose;
    private StickyMenu _menu;

    private void Awake()
    {
        _gateOpen = transform.Find("GateOpen").gameObject;
        _gateClose = transform.Find("GateClose").gameObject;
        SetClosed(Closed);
        _menu = GetComponentInChildren<StickyMenu>(true);
    }

    private void SetClosed(bool state)
    {
        _gateOpen.SetActive(!state);
        _gateClose.SetActive(state);
        _closed = state;
    }

    public void OpenMenu()
    {
        var button = _menu.GetComponentInChildren<Button>();
        button.GetComponentInChildren<Text>().text = _closed ? "Open" : "Close";
        button.onClick.AddListener(() =>
        {
            button.onClick.RemoveAllListeners();
            SetClosed(!_closed);
            _menu.gameObject.SetActive(false);
        });
        _menu.gameObject.SetActive(true);
    }
}