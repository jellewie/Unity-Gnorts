using UnityEngine;
using UnityEngine.UI;                                                               //We need this to interact with the UI

/* 
->Image With; code "HoverOver", "Content Size Fitter" Preffered, "Horizontal layout group" with your own spacing
--> Textbox
*/
public class HoverOver : MonoBehaviour
{
    private Text TextBox;                                                                       //The child textbox
    private GameObject img;                                                                     //The parrent image
    private RectTransform Image_RectTransform;                                                  //The parrent image
    public Vector2 HorzontalOffzet;                                                             //Offset direction

    void Start()                                                                    //Run on startup
    {
        img = this.gameObject;                                                                  //Set the gameObject link (Needed to enable / disable)
        Image_RectTransform = GetComponent<RectTransform>();                                    //Set the RectTransform link (Needed for size measurement        
        TextBox = this.GetComponentInChildren<Text>();                                          //Set the Textbox link (This text will be changed)
    }
    void Update()                                                                   //Update every frame (only if active)
    {
        MoveToCursos();                                                                         //Move the box to the mouse
    }
    private void MoveToCursos()                                                     //Move the box to the mouse
    {
        img.transform.localPosition = new Vector2(                                              //Let the box follow the mouse around
            Input.mousePosition.x - Screen.width / 2 + HorzontalOffzet.x,                       //X position of box
            Input.mousePosition.y - Screen.height / 2 - 10 + HorzontalOffzet.y                  //Y position of box
            );
    }
    public void _ShowHoverText(string text)                                         //If the state of the HoverText changes
    {
        if (text == "")                                                                         //If the text is emthy (then disabled hover,  we are done)
        {
            img.SetActive(false);                                                               //Disable the box (We dont need it anymore)
        }
        else
        {
            img.SetActive(false);                                                               //Temp diable it
            if (text.Substring(0, 1) == "R")                                                    //If text need to be on the Right side of the cursor
            {
                Image_RectTransform.pivot = new Vector2(0, 0.5f);                               //Set the pivit point (coords 0,0)
                HorzontalOffzet = new Vector2(10, 0);                                           //Set the offset (So we are not below the cursor)
                TextBox.text = text.Substring(1, text.Length - 1);                              //Set the text
            }
            else if (text.Substring(0, 1) == "L")                                               //If text need to be on the Left side of the cursor
            {
                Image_RectTransform.pivot = new Vector2(1, 0.5f);                               //Set the pivit point (coords 0,0)
                HorzontalOffzet = new Vector2(-10, 0);                                          //Set the offset (So we are not below the cursor)
                TextBox.text = text.Substring(1, text.Length - 1);                              //Set the text
            }
            else if (text.Substring(0, 1) == "U")                                               //If text need to be on the top (Up) side of the cursor
            {
                Image_RectTransform.pivot = new Vector2(0.5f, 0);                               //Set the pivit point (coords 0,0)
                HorzontalOffzet = new Vector2(0, 20);                                           //Set the offset (So we are not below the cursor)
                TextBox.text = text.Substring(1, text.Length - 1);                              //Set the text
            }
            else if (text.Substring(0, 1) == "D")                                               //If text need to be on the bottom (Down) side of the cursor
            {
                Image_RectTransform.pivot = new Vector2(0.5f, 1);                               //Set the pivit point (coords 0,0)
                HorzontalOffzet = new Vector2(0, -20);                                          //Set the offset (So we are not below the cursor)
                TextBox.text = text.Substring(1, text.Length - 1);                              //Set the text
            }
            else
            {
                TextBox.text = text;                                                            //Set the text
                Debug.LogError("No direction of HoverText given, using the last know direction");//Show an error
            }
            img.SetActive(true);                                                                //Enable the box
            MoveToCursos();                                                                     //Move the box to the mouse
        }
    }
}