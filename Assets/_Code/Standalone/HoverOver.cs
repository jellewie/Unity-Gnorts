using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PublicCode;

public class HoverOver : MonoBehaviour, 
IPointerDownHandler,
IPointerEnterHandler,
IPointerExitHandler,
IBeginDragHandler, 
IDragHandler, 
IEndDragHandler
{
    public GameObject Prefab;                                                                   //The preab that needs to be placed when clicked on     Leave emthy to not place anything
    public string Direction = "L";                                                              //The direction of the text                             Can be; Left Righ Up Down
    public string HoverText;                                                                    //The text to show

    private GameObject ParentHoverOver;                                                         //The parrent
    private GameObject OBJHoverOver;                                                            //The child we are going to move and enable/disable (We can't to the parent or we will lose track of the parent)
    private UserInput CodeUserInput;                                                            //The UserInput code
    private Text TextBox;                                                                       //The child textbox
    private bool MouseIsOver;                                                                   //A bool that keeps track if the mouse is over this object
    private Vector2 HorzontalOffzet;                                                            //Offset direction
    private void Start()                                                                //Run once on startup
    {
        ParentHoverOver = GameObject.Find("HoverOver");                                         //Get the Code HoverOver
        CodeUserInput = GameObject.Find("UserInput").GetComponent<UserInput>();                 //Get the Code UserInput
        OBJHoverOver = ParentHoverOver.transform.GetChild(0).gameObject;                        //Get the object we are going to move and enable/disable
        Image_RectTransform = OBJHoverOver.GetComponent<RectTransform>();                       //Set the RectTransform reference (Needed for size measurement        
        TextBox = OBJHoverOver.GetComponentInChildren<Text>();                                  //Set the Textbox reference (This text will be changed)
        if(HoverText == "" && Prefab != null)                                                   //If no HoverText is given, and there is a Prefab
            HoverText = Prefab.name;                                                            //Use the Prefab name 
    }
    private void MoveToCursos()                                                         //Move the box to the mouse
    {
        OBJHoverOver.transform.localPosition = new Vector2(                                     //Let the box follow the mouse around
            Input.mousePosition.x - Screen.width / 2 + HorzontalOffzet.x,                       //X position of box
            Input.mousePosition.y - Screen.height / 2 - 10 + HorzontalOffzet.y                  //Y position of box
            );
    }
    private RectTransform Image_RectTransform;                                          //The parrent image
    public void OnPointerEnter(PointerEventData evd)                                    //Run each time the mouse enters this object
    {
        MouseIsOver = true;                                                                     //Flag that the mouse is over this object
        TextBox.text = (                                                                        //Set the text of the textbox
                (
                    HoverText.Substring(0, 1).ToUpper() +                                       //Make sure the first letter is UPPER CASE
                    HoverText.Substring(1, HoverText.Length - 1).ToLower()                      //Make sure the others are lower case
                ).Replace("_", " ")                                                             //Replace all underlines by spaces
            );
        TextBox.fontSize = 25;
        if (Prefab != null)                                                                     //If a Prefab is set
        {
            BuildingInfo BuildingInfo = BuildingData.GetInfo(Prefab.name);                //Get the building info
            if (BuildingInfo.Name != "N/A")                                                     //If we have not encountered an error
            {
                if (BuildingInfo.Cost.Wood > 0)                                                 //If this building needs wood
                {
                    TextBox.text += "\n" + BuildingInfo.Cost.Wood + " Wood";                    //Show howmuch wood it needs to be build
                }
                if (BuildingInfo.Cost.Stone > 0)                                                //If this building needs Stone
                {
                    TextBox.text += "\n" + BuildingInfo.Cost.Stone + " Stone";                  //Show howmuch Stone it needs to be build
                }
                if (BuildingInfo.Cost.Gold > 0)                                                 //If this building needs Stone
                {
                    TextBox.text += "\n" + BuildingInfo.Cost.Gold + " Gold";                    //Show howmuch Gold it needs to be build
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
    public void OnPointerExit(PointerEventData evd)                                     //Run each time the mouse leaves this object
    {
        MouseIsOver = false;                                                                    //Flag that the mouse is NOT over this object
        OBJHoverOver.SetActive(false);                                                          //Disable the box (We dont need it anymore)
    }
    public void OnPointerDown(PointerEventData evd)                                    //Run each time the mouse clicks on this object
    {
        if (Prefab != null)                                                                     //If a Prefab is set
            CodeUserInput._PlaceInHand(Prefab);                                                 //Put the Prefab in our hands
    }
    private void Update()                                                               //Run each frame
    {
        if (MouseIsOver)                                                                        //If the mouse is over this object
            MoveToCursos();                                                                     //Move the HoverOver box to the mouse
    }

    public void OnBeginDrag(PointerEventData eventData)                                 //Run before a drag is started
    {
        CodeUserInput.StartDragging();
    }

    public void OnDrag(PointerEventData eventData)                                      //Run when dragging and the cursor is moved
    {
        // Empty implementation of IDragHandler.
        // Needed for OnBeginDrag and OnEndDrag to function.
    }

    public void OnEndDrag(PointerEventData eventData)                                   //Run when a drag is ended
    {
        CodeUserInput.StopDragging();
    }
}