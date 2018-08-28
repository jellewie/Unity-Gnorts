using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PublicCode;
using UnityEngine.EventSystems;                                                         //Used to: Check if hovering over UI while building,
/*
    Written by JelleWho
 */
public class UserInput : MonoBehaviour
{
    public InputManager inputManager;
    public GameObject FolderMenu;                                                       //The folder to enable on MenuOpen
    public GameObject FolderInfo;
    public GameObject FolderTrading;
    public GameObject FolderGraph;
    public GameObject FolderUI;                                                         //The folder to hide on HideUI
    public GameObject FolderSubMenu;                                                    //The folder to close when done building
    public Transform FolderBuildings;                                                   //The folder where all the buildings should be put in

    Quaternion PreviousRotation;

    bool IsOutOfFocus = false;
    bool GamePaused = false;
    bool StopCameraControls = false;
    private GameObject InHand;                                                          //When placing down this is stuffed with the object
    bool RemoveToolEquiped;

    private void Start()                                                                //Triggered on start
    {
    }
    private void Update()                                                               //Triggered before frame update
    {  
    }
    private void LateUpdate()                                                           //Triggered after frame update
    {
        if (!IsOutOfFocus)                                                                      //If the game isn't paused
        {
            AlwaysControls();                                                                   //Controls that always need to be executed (like the ESC button)
            if (!StopCameraControls)                                                            //If we are somewhere where we dont want to control the camera
            {
                ExecuteInputs();                                                                //Check if we need to move the camera                                       
            }
        }
        if (GamePaused)
        {
            //some IA stuff here
        }
    }
    public void CameraControls(bool SetTo)                                              //With this buttons can change the camera mode
    {
        StopCameraControls = !SetTo;                                                            //Set the camera mode to what ever there is given (CameraControls FALSE = Stop camera)
    }
    void OnApplicationFocus(bool hasFocus)                                              //Triggered when the game is in focus
    {
        IsOutOfFocus = !hasFocus;                                                               //Set game to be in focus
    }
    void OnApplicationPause(bool pauseStatus)                                           //Triggered when the game is out focus
    {
        IsOutOfFocus = pauseStatus;                                                             //Set game to be out of focus
    }
    private void AlwaysControls()                                                       //Triggered in LateUpdate (unless the game is out of focus)
    {
        if (inputManager.GetButtonDownOnce("Menu"))                                             //If the Open/Close menu button is pressed
        {
            StopCameraControls = !FolderMenu.activeSelf;                                        //Flag that the camera controls should be active or not
            FolderMenu.SetActive(StopCameraControls);                                           //Set the menu's visibility
        }
        if (inputManager.GetButtonDownOnce("Pause"))                                            //If the Open/Close menu button is pressed
            GamePaused = true;
    }
    public void _PlaceInHand(GameObject Prefab)                                         //Triggered by menu, with the object to build as prefab, this will hook in to the mouse cursor
    {
        Destroy(InHand);                                                                        //Destroy the current held item (If any)
        PlaceInHand(Prefab);                                                                    //Place the new building on our hand
    }
    private void PlaceInHand(GameObject Prefab)                                         //With the object to build as prefab, this will hook in to the mouse cursor
    {
        InHand = Instantiate(Prefab, new Vector3(0, -100, 0), Quaternion.identity);             //Create a building (we dont need to set it's position, will do later in this loop
        InHand.transform.rotation = PreviousRotation;                                           //Restore the rotation
        InHand.transform.SetParent(FolderBuildings);                                            //Sort the building in the right folder
        InHand.layer = 0;                                                                       //Set to default Layer
    }
    public void _RemoveTool(bool Equiped)                                               //Triggered by menu, Equipe the remove tool
    {
        RemoveToolEquiped = Equiped;                                                            //Set the given state
    }
    public void _HideSubMenu()                                                          //This will hide the full sub menu
    {
        foreach (Transform child in FolderSubMenu.transform)                                    //Do for each SubMenu
        {
            child.gameObject.SetActive(false);                                                  //Hide the SubMenu
        }
    }
    private void ExecuteInputs()                                                                //Triggered in LateUpdate (unless the game is out of focus, or camera controls are disabled) this controlls the camera movement
    {
        if (InHand)                                                                             //If we have something in our hands
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                        //Set a Ray from the cursor + lookation
            RaycastHit hit;                                                                     //Create a output variable
            if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Terrain")))      //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over on the Terrain layer only)
                InHand.transform.position = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z)); //Move the block to the mouse position
            InHand.layer = 0;                                                                   //Set to default Layer
            RaycastHit[] Hit = Physics.BoxCastAll(                                              //Cast a ray to see if there is already a building where we are hovering over
                InHand.GetComponent<Collider>().bounds.center,                                      //The center of the block
                (InHand.GetComponent<BoxCollider>().size / 2.1f) - new Vector3(0.5f, 0, 0.5f),      //Size of center to side of the block (minus a bit to make sure we dont touch the next block)
                -transform.up,                                                                      //Do the ray downwards (in to the ground basicly to check only it's own position)
                InHand.GetComponent<Collider>().transform.rotation,                                 //The orientation in Quaternion (Always in steps of 90 degrees)
                0f,                                                                                 //Dont go any depth, the building should be inside this block
                1 << LayerMask.NameToLayer("Building"));                                            //Only try to find buildings

            if (inputManager.GetButtonDownOnce("Cancel build"))                                 //If we want to cancel the build
            {
                PreviousRotation = InHand.transform.rotation;                                   //Save the rotation
                Destroy(InHand);                                                                //Destoy the building
            }
            else if (inputManager.GetButtonDown("Build"))                                       //If we need to build the object here
            {
                if (Hit.Length > 0)                                                             //If there a building already there
                {
                    /*TODO FIXME 
                    If this is hit for more than <1 sec> than show a message that we can't build there
                     */
                }
                else
                {
                    if (!EventSystem.current.IsPointerOverGameObject())                         //If mouse is not over an UI element
                    {
                        InHand.layer = LayerMask.NameToLayer("Building");                       //Set this to be in the building layer (so we can't build on this anymore)
                        PlaceInHand(InHand);                                                        //Put a new building on our hands, and leave this one be (this one is now placed down)
                    }
                }
            }
            else if (inputManager.GetButtonDownOnce("Rotate building"))                         //If we want to rotate the building
            {
                if (inputManager.GetButtonDown("Alternative"))                                  //If we want to rotate the other way
                    InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y - 90, 0);    //Rotate it 90 degrees counter clock wise
                else
                    InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y + 90, 0);    //Rotate it 90 degrees clock wise
                PreviousRotation = InHand.transform.rotation;                                   //Save the rotation
            }
        }
        else if (RemoveToolEquiped)                                                             //If the remove tool is aquiped
        {
            if (inputManager.GetButtonDown("Build"))                                            //If we want to Remove this building
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                    //Set a Ray from the cursor + lookation
                RaycastHit hit;                                                                 //Create a output variable
                if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over                                      
                {
                    if (inputManager.GetButtonDownOnce("Build"))                                //If the button is pressed for the first time
                        Destroy(hit.transform.gameObject);                                      //Remove the selected building
                    else if(inputManager.GetButtonDown("Alternative"))                          //If the continue button is pressed
                        Destroy(hit.transform.gameObject);                                      //Remove the selected building
                }
            }
            else if (inputManager.GetButtonDownOnce("Cancel build"))                            //If we want to cancel Removing buildings
                RemoveToolEquiped = false;                                                      //Stop the RemoveTool being equiped
        }
        if (inputManager.GetButtonDownOnce("Cancel build"))                                     //If we right click to cancel
            _HideSubMenu();                                                                     //Hide the sub menu
        if (inputManager.GetButtonDownOnce("Toggle UI"))                                        //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Goggle the UI
        float Speed = Camera.main.transform.position.y * JelleWho.HeighSpeedIncrease;           //The height has X of speed increase per block
        Vector2 input = new Vector2(0f, 0f);                                                    //Create a new (emnthy) movement change vector
        if (inputManager.GetButtonDown("Left"))                                                 //Keyboard scroll left
            input.x -= 1f * JelleWho.MoveSpeedKeyboard;
        if (inputManager.GetButtonDown("Right"))                                                //Keyboard scroll right
            input.x += 1f * JelleWho.MoveSpeedKeyboard;
        if (inputManager.GetButtonDown("Up"))                                                   //Keyboard scroll up
            input.y += 1f * JelleWho.MoveSpeedKeyboard;
        if (inputManager.GetButtonDown("Down"))                                                 //Keyboard scroll down
            input.y -= 1f * JelleWho.MoveSpeedKeyboard;
        if (inputManager.GetButtonDown("Drag"))                                                 //If the Drag button is presse
        {
            input.x -= Input.GetAxis("Mouse X") * JelleWho.MoveSpeedMouse * Speed;              //Calculate howmuch we need to move in the axes 
            input.y -= Input.GetAxis("Mouse Y") * JelleWho.MoveSpeedMouse * Speed;              //^
        } else if (input == new Vector2(0f, 0f))                                                //If camera doesn't need to move yet
        {
            if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & 0x01) != 0x01)//If EdgeScroll setting is on
            {
                float xpos = Input.mousePosition.x;                                             //Save mouse position
                float ypos = Input.mousePosition.y;                                             //^        
                if (xpos >= 0 && xpos < JelleWho.MoveIfThisCloseToTheSides)                     //Edge scroll left
                    input.x -= 1f * JelleWho.MoveEdgeScrollSpeed;                               //
                else if (xpos <= Screen.width && xpos > Screen.width - JelleWho.MoveIfThisCloseToTheSides)//Edge scroll right
                    input.x += 1f * JelleWho.MoveEdgeScrollSpeed;                               //
                if (ypos >= 0 && ypos < JelleWho.MoveIfThisCloseToTheSides)                     //Edge scroll down
                    input.y -= 1f * JelleWho.MoveEdgeScrollSpeed;                               //
                else if (ypos <= Screen.height && ypos > Screen.height - JelleWho.MoveIfThisCloseToTheSides)//Edge scrolll up
                    input.y += 1f * JelleWho.MoveEdgeScrollSpeed;                               //
            }
        }
        if (Mathf.Abs(input.y) > Mathf.Epsilon)                                                 //Movement up/down relative to the screen
        {
            Vector3 horDir = Camera.main.transform.forward;                                     //Get the camera position
            horDir.y = 0f;                                                                      //Set Up/Down movement to 0, so we ignore that direction
            horDir.Normalize();                                                                 //If there is a value make it 1
            Vector3 newCameraPos = Camera.main.transform.position + horDir * Speed * input.y;   //Get the new camera position
            Camera.main.transform.position = newCameraPos;                                      //Set camera position
        }
        if (Mathf.Abs(input.x) > Mathf.Epsilon)                                                 //Movement left/right relative to the screen
        {
            Vector3 horDir = Camera.main.transform.right;                                       //Get the camera position 
            horDir.y = 0f;                                                                      //Set Up/Down movement to 0, so we ignore that direction
            horDir.Normalize();                                                                 //If there is a value make it 1
            Vector3 newCameraPos = Camera.main.transform.position + horDir * Speed * input.x;   //Get the new camera position
            Camera.main.transform.position = newCameraPos;                                      //Set camera position
        }
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");                           //Get the scrollwheel location
        if (ScrollWheelChange != 0 && EnableZoom)                                               //If the scrollwheel has changed (and zoom is enabled)
        {
            Vector3 newCameraPos = Camera.main.transform.position;
            Vector3 cameraForward = Camera.main.transform.forward;
            newCameraPos += cameraForward * JelleWho.ZoomScrollWheelSpeed * ScrollWheelChange;
            newCameraPos = new Vector3(
                newCameraPos.x,
                newCameraPos.y,
                newCameraPos.z);
            Camera.main.transform.position = newCameraPos;
        }
        float Xr = Camera.main.transform.eulerAngles.x;                                         //Get main camera rotation
        float Yr = Camera.main.transform.eulerAngles.y;                                         
        if (inputManager.GetButtonDown("Rotate left"))
            Yr -= JelleWho.RotateSpeedKeyboard;                                                 //Get the mouse movement
        if (inputManager.GetButtonDown("Rotate right"))
            Yr += JelleWho.RotateSpeedKeyboard;                                                 //Get the mouse movement
        if (inputManager.GetButtonDown("Rotate"))                                               //If the rotate button is pressed
        {
            Xr -= Input.GetAxis("Mouse Y") * JelleWho.RotateSpeedMouse;                         //Get the mouse movement
            Yr += Input.GetAxis("Mouse X") * JelleWho.RotateSpeedMouse;                         //^
        }
        Vector3 C = Camera.main.transform.position;                                             //Get setted camera camera position
        Camera.main.transform.position = new Vector3(                                           //Limit movement
            Mathf.Clamp(C.x, -JelleWho.MaxMoveHorizontalOnMap, JelleWho.MaxMoveHorizontalOnMap),//Clamp X horizontal movement
            Mathf.Clamp(C.y, JelleWho.MinCameraHeight, JelleWho.MaxCameraHeight),               //Clamp Y vertical movement
            Mathf.Clamp(C.z, -JelleWho.MaxMoveHorizontalOnMap, JelleWho.MaxMoveHorizontalOnMap));//Clamp Z 
        Camera.main.transform.eulerAngles = new Vector2(                                        //Limit camera look angles
            Mathf.Clamp(Xr,0 ,89.99f), Yr);                                                     //Clamp Up down looking angle 
    }
    Vector3 PolarToCartesian(Vector2 polar, Vector3 Offset)                             //Offset=(Left, Up, Forward)
    {
        var rotation = Quaternion.Euler(polar.x, polar.y, 0);                                   //Convert it
        return rotation * Offset;                                                               //Return the Vector 3 of the target point
    }
    private bool EnableZoom = true;                                                             //If Zoom is enabled
    public void Zoom(bool Enabled)                                                      //Enable or disable zoom
    {
        EnableZoom = Enabled;                                                                   //Set the right state
    }
}

/*  public GameObject Tempblock;
    void Temp()
    {
        Vector3 OffsetXYZ = Camera.main.transform.position;                                     //The Starting position
        Vector2 LookingAt = Camera.main.transform.eulerAngles;                                  //The angles we are looking at
        Vector3 Range = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));     //The range where we need to go relative to the angles
        Tempblock.transform.position = OffsetXYZ + PolarToCartesian(LookingAt, Range);          //Calculate and move
    }*/
