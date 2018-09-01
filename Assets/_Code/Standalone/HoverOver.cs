using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PublicCode;

public class HoverOver : MonoBehaviour, 
IPointerClickHandler,
IPointerEnterHandler,
IPointerExitHandler,
ISelectHandler
{
    

    public GameObject Prefab;                                                                   //The preab that needs to be placed when clicked on     Leave emthy to not place anything
    public string Direction = "L";                                                              //The direction of the text                             Can be; Left Righ Up Down
    public string HoverText;                                                                    //The text to show

    private GameObject ParentHoverOver;                                                         //The parrent
    private GameObject OBJHoverOver;
    private GameObject CodeUserInput;                                                           //The GameObject with the code on it
    private GameObject CodeInputManager;

    private Text TextBox;                                                                       //The child textbox
    private bool MouseIsOver;
    private Vector2 HorzontalOffzet;                                                            //Offset direction

    private void Start()
    {
        ParentHoverOver = GameObject.Find("HoverOver");
        CodeInputManager = GameObject.Find("InputManager");
        CodeUserInput = GameObject.Find("UserInput");

        OBJHoverOver = ParentHoverOver.transform.GetChild(0).gameObject;


        Image_RectTransform = OBJHoverOver.GetComponent<RectTransform>();                          //Set the RectTransform reference (Needed for size measurement        
        TextBox = OBJHoverOver.GetComponentInChildren<Text>();                                     //Set the Textbox reference (This text will be changed)
        if(HoverText == "")
        {
            HoverText = Prefab.name;
        }
    }
    private void MoveToCursos()                                                     //Move the box to the mouse
    {
        OBJHoverOver.transform.localPosition = new Vector2(                                              //Let the box follow the mouse around
            Input.mousePosition.x - Screen.width / 2 + HorzontalOffzet.x,                       //X position of box
            Input.mousePosition.y - Screen.height / 2 - 10 + HorzontalOffzet.y                  //Y position of box
            );
    }
    private RectTransform Image_RectTransform;                                                  //The parrent image
    public void OnPointerEnter(PointerEventData evd)
    {
        MouseIsOver = true;

        TextBox.text = (
                (
                    HoverText.Substring(0, 1).ToUpper() +
                    HoverText.Substring(1, HoverText.Length - 1).ToLower()
                ).Replace("_", " ")
            );
        if (Prefab != null)
        {
            Building BuildingInfo = CodeInputManager.GetComponent<InputManager>().GetInfo(Prefab.name);
            if (BuildingInfo.Name != "N/A")
            {
                if (BuildingInfo.Cost_Wood > 0)
                {
                    TextBox.text += "\n" + BuildingInfo.Cost_Wood + " Wood";
                }
                if (BuildingInfo.Cost_Stone > 0)
                {
                    TextBox.text += "\n" + BuildingInfo.Cost_Stone + " Stone";
                }
                if (BuildingInfo.Cost_Money > 0)
                {
                    TextBox.text += "\n" + BuildingInfo.Cost_Money + " Money";
                }
            }
        }
        

        if (Direction == "R")                                                                   //If text need to be on the Right side of the cursor
        {
            Image_RectTransform.pivot = new Vector2(0, 0.5f);                                   //Set the pivit point (coords 0,0)
            HorzontalOffzet = new Vector2(10, 0);                                               //Set the offset (So we are not below the cursor)
        }
        else if (Direction == "L")                                                              //If text need to be on the Left side of the cursor
        {
            Image_RectTransform.pivot = new Vector2(1, 0.5f);                                   //Set the pivit point (coords 0,0)
            HorzontalOffzet = new Vector2(-10, 0);                                              //Set the offset (So we are not below the cursor)
        }
        else if (Direction == "U")                                                              //If text need to be on the top (Up) side of the cursor
        {
            Image_RectTransform.pivot = new Vector2(0.5f, 0);                                   //Set the pivit point (coords 0,0)
            HorzontalOffzet = new Vector2(0, 20);                                               //Set the offset (So we are not below the cursor)
        }
        else                                                                                    //If text need to be on the bottom (Down) side of the cursor
        {
            Image_RectTransform.pivot = new Vector2(0.5f, 1);                                   //Set the pivit point (coords 0,0)
            HorzontalOffzet = new Vector2(0, -20);                                              //Set the offset (So we are not below the cursor)
        }
        OBJHoverOver.SetActive(true);
        MoveToCursos();                                                                         //Move the box to the mouse
    }
    public void OnPointerExit(PointerEventData evd)
    {
        MouseIsOver = false;
        OBJHoverOver.SetActive(false);                         //Disable the box (We dont need it anymore)
    }
    public void OnPointerClick(PointerEventData evd)
    {
        if (Prefab != null)
        {
            CodeUserInput.GetComponent<UserInput>()._PlaceInHand(Prefab);
        }
    }
    public void OnSelect(BaseEventData evd)
    {
        Debug.Log("OnSelect");
    }
    private void Update()
    {
        if (MouseIsOver)
        {
            MoveToCursos();                                                                         //Move the box to the mouse
        }
    }
}