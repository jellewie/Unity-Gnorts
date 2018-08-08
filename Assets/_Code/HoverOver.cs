using UnityEngine;
//using System.Collections;
using UnityEngine.UI;


public class HoverOver : MonoBehaviour
{
    public Text TextBox;
    public bool RightOfMouse;
    bool Enabled;

    void Update()
    {
        Debug.Log("update");
    }

    void LateUpdate()
    {
        Debug.Log("Lateupdate");
        if (Enabled)
        {
            TextBox.transform.localPosition = new Vector2(
                 Input.mousePosition.x - Screen.width / 2,
                 Input.mousePosition.y - Screen.height / 2 - 10
                );
        }
    }

    public void _ShowHoverText(string text)
    {
        Debug.Log("======================================='" + text + "'");
        TextBox.text = text;
        if (text == "")
        {
            Enabled = false;
        }
        else
        {
            Enabled = true;
            if (text.Substring(0, 1) == "R")
            {
                Debug.Log("Right");
                TextBox.alignment = TextAnchor.MiddleLeft; ;
                TextBox.text = text.Substring(1, text.Length -1);
            }
            else if (text.Substring(0, 1) == "L")
            {
                Debug.Log("Left");
                TextBox.alignment = TextAnchor.MiddleRight;
                TextBox.text = text.Substring(1, text.Length - 1);
            }
            else
            {
                TextBox.text = text;//
                Debug.LogError("No direction of HoverText given, using the last know direction");
            }
        }
    }
}