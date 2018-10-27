using System.Linq;
using UnityEngine;

/// <summary>
/// A menu that sticks to the gameObject it belongs to.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class StickyMenu : MonoBehaviour
{
    /// <summary>
    /// The panel showing the menu.
    /// </summary>
    private GameObject _panel;

    /// <summary>
    /// A reference to the input manager so we can detect cancel clicks.
    /// </summary>
    private InputManager _inputManager;

    /// <summary>
    /// The offset between the game object and the menu panel. 
    /// </summary>
    private Vector3 _menuOffset;

    protected virtual void Awake()
    {
        // Get some of the components we need.
        _inputManager = FindObjectOfType<InputManager>();
        _panel = GetFirstChild(gameObject);
    }

    private void OnEnable()
    {
        // Move the menu panel to the point where the mouse clicked.
        _panel.transform.position = Input.mousePosition;
        // Remember the offset between that point and the gameObject's position.
        // We do this so that the menu appears where you click and then stays in the same position relative to the
        // gameObject. 
        _menuOffset = _panel.transform.position - Camera.main.WorldToScreenPoint(transform.parent.position);
    }

    private void Update()
    {
        // Move the menu panel to the location of the gameObject, keeping it in roughly the same place as where ot was
        // activated.
        _panel.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position) + _menuOffset;
        // Close the menu when the user cancel-clicks.
        if (_inputManager.GetButtonDownOnce(ButtonId.CancelBuild))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Helper method to get the first child gameObject that has a RectTransform.
    /// This should be the Panel for the menu. 
    /// </summary>
    /// <param name="parent">The gameObject to search in</param>
    /// <returns>The first child found</returns>
    private static GameObject GetFirstChild(GameObject parent)
    {
        return (from transform in parent.GetComponentsInChildren<RectTransform>()
            where transform.gameObject != parent
            select transform.gameObject).FirstOrDefault();
    }
}