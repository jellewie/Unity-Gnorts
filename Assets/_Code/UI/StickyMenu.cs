using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StickyMenu : MonoBehaviour
{
    private GameObject _panel;
    private InputManager _inputManager;
    private Vector3 _menuOffset;

    protected virtual void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _panel = GetFirstChild(gameObject);
        
    }

    private void OnEnable()
    {
        _panel.transform.position = Input.mousePosition;
        _menuOffset = _panel.transform.position - Camera.main.WorldToScreenPoint(transform.parent.position);
    }

    private void Update()
    {
        _panel.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position) + _menuOffset;
        if (_inputManager.GetButtonDownOnce(ButtonId.CancelBuild))
        {
            gameObject.SetActive(false);
        }
    }

    private static GameObject GetFirstChild(GameObject parent)
    {
        return (from transform in parent.GetComponentsInChildren<RectTransform>(true)
            where transform.gameObject != parent
            select transform.gameObject).FirstOrDefault();
    }
}