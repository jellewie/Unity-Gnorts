using UnityEngine;
//using System.Collections;
using UnityEngine.UI;


public class HoverOver : MonoBehaviour
{
    public Text TextBox;                                                                        //The child textbox
    public GameObject img;                                                                      //The parrent image
    public RectTransform Image_RectTransform;                                                   //The parrent image
    private sbyte HorzontalOffzet;                                                              //Offset direction
    bool Enabled;

    void Start()
    {
        img = this.gameObject;                                                                  //Set the gameObject link (Needed to enable / disable)
        img.SetActive(false);                                                                   //The text/image should be hidden by default
        Image_RectTransform = GetComponent<RectTransform>();                                    //Set the RectTransform link (Needed for size measurement        
        TextBox = this.GetComponentInChildren<Text>();                                          //Set the Textbox link (This text will be changed)
    }
    void Update()                   //Update every frame (only if active)
    {
        img.SetActive(true);
        MoveToCursor();                                                                     //Move it to the cursor
    }
    public void MoveToCursor()
    {
        /*
        The "Image_RectTransform.rect.width" isn't updated yet the first time we call this with "_ShowHoverText"
        Thats wby it's blinking once. Not sure how to fix this


         */
        img.transform.localPosition = new Vector2(
            Input.mousePosition.x - Screen.width / 2 + HorzontalOffzet * (Image_RectTransform.rect.width / 2 + 10),
            Input.mousePosition.y - Screen.height / 2 - 10
            );
    }
    public void _ShowHoverText(string text)
    {
        Debug.Log(text);
        if (text == "")
        {
            img.SetActive(false);
        }
        else
        {
            if (text.Substring(0, 1) == "R")                                                    //If text need to be on the Right side of the cursor
            {
                TextBox.alignment = TextAnchor.MiddleLeft;                                      //Set the text aligment on the Left (that is closest to the mouse)
                TextBox.text = text.Substring(1, text.Length -1);                               //Set the text
                HorzontalOffzet = 1;                                                            //Set the offset direction
                //Image_RectTransform.anchorMin = new Vector2(0, 0.5f);
                //Image_RectTransform.anchorMax = new Vector2(0, 0.5f);
            }
            else if (text.Substring(0, 1) == "L")                                               //If text need to be on the Left side of the cursor
            {
                TextBox.alignment = TextAnchor.MiddleRight;                                     //Set the text aligment on the Right (that is closest to the mouse)
                TextBox.text = text.Substring(1, text.Length - 1);                              //Set the text

                HorzontalOffzet = -1;                                                           //Set the offset direction

                Image_RectTransform.anchorMin = new Vector2(1, 0.5f);
                Image_RectTransform.anchorMax = new Vector2(1, 0.5f);
            }
            else
            {
                TextBox.text = text;
                Debug.LogError("No direction of HoverText given, using the last know direction");
            }
            img.SetActive(true);
        }
    }
}