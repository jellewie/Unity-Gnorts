using UnityEngine;
//using System.Collections;
using UnityEngine.UI;


public class HoverOver : MonoBehaviour
{
    public Text TextBox;
    public GameObject Image;
    int HorzontalOffzet;
    bool Enabled;

    void Start()
    {
        //TextBox = this.gameObject.transform.GetChild(0);
    }

    void Update()
    {
        if (Enabled)
        {
            Image.transform.localPosition = new Vector2(
                 Input.mousePosition.x - Screen.width / 2 + HorzontalOffzet,
                 Input.mousePosition.y - Screen.height / 2 - 10
                );
        }
    }

    public void _ShowHoverText(string text)
    {
        RectTransform m_RectTransform = GetComponent<RectTransform>();

        TextBox.text = text;
        if (text == "")
        {
            Enabled = false;
            Image.SetActive(false);
        }
        else
        {
            Image.SetActive(true);
            Enabled = true;
            if (text.Substring(0, 1) == "R")
            {
                HorzontalOffzet = 150;
                TextBox.alignment = TextAnchor.MiddleLeft;
                TextBox.text = text.Substring(1, text.Length -1);


                m_RectTransform.anchorMin = new Vector2(0, 0.5f);
                m_RectTransform.anchorMax = new Vector2(0, 0.5f);
            }
            else if (text.Substring(0, 1) == "L")
            {
                HorzontalOffzet = -100;
                TextBox.alignment = TextAnchor.MiddleRight;
                TextBox.text = text.Substring(1, text.Length - 1);


                
                m_RectTransform.anchorMin = new Vector2(1, 0.5f);
                m_RectTransform.anchorMax = new Vector2(1, 0.5f);
            }
            else
            {
                TextBox.text = text;
                Debug.LogError("No direction of HoverText given, using the last know direction");
            }
        }
    }
}