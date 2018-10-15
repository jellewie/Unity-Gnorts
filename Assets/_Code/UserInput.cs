using UnityEngine;
using PublicCode;
using UnityEngine.EventSystems;                                                         //Used to: Check if hovering over UI while building,
using System;
using UnityEngine.UI;                                                                   //We need this to interact with the UI
using System.Collections;                                                               //we need this to use IEnumerator (coroutines)
/*
Written by JelleWho
*/
public class UserInput : MonoBehaviour
{
    public InputManager CodeInputManager;                                               //The GameObject with the InputManager code on it
    public GameObject CodeResourceManager;                                              //The GameObject with the ResourceManager code on it
    public GameObject CodeSaveLoad; 
    public GameObject FolderMenu;                                                       //The folder to enable on MenuOpen
    public GameObject FolderInfo;
    public GameObject FolderTrading;
    public GameObject FolderGraph;
    public GameObject FolderUI;                                                         //The folder to hide on HideUI
    public GameObject FolderSubMenu;                                                    //The folder to close when done building
    public Transform FolderBuildings;                                                   //The folder where all the buildings should be put in
    public GameObject FolderBuildingPopUp;                                              //The folder with the pop-up stuff in it
    public GameObject TextMessage;
    private Byte LowerObjectBy = 0;                                                     //Howmuch the gameobject should be higher (this is used for walls as example)
    public Texture2D MouseDeconstruct;                                                  //The mouse icon of the deconstruct tool
    public Byte ThisPlayerID;
    Quaternion PreviousRotation;
    public float InvalidBuildTimeThreshold = 0.2f;                                      //How long before we show a message about not being able to build

    public bool IsDragging { get; set; }                                                //Are we dragging something?

    private bool IsOutOfFocus = false;
    private bool GamePaused = false;
    private bool StopCameraControls = false;
    private GameObject InHand;                                                          //When placing down this is stuffed with the object
    private bool DeconstructToolEquiped;                                                //If the DeconstructTool is Equiped
    private float deltaTime = 0.0f;                                                     //The time between this and the last frame
    private float buildKeyDownTime;                                                     //How long the build key has been down
    public float Speed;                                                                 //Speed multiblecation for controls (zoom out slowdown)

    private void Start()                                                                //Triggered on start
    {
    }
    private void Update()                                                               //Triggered before frame update
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;                               //Calculate time elapsed since last frame
    }
    private void LateUpdate()                                                           //Triggered after frame update
    {
        if (!IsOutOfFocus)                                                                      //If the game isn't paused
        {
            AlwaysControls();                                                                   //Controls that always need to be executed (like the ESC button)
            if (!StopCameraControls)                                                            //If we are on a place where we want to control the camera
            {
                ExecuteInputs();                                                                //Check if we need to move the camera                                       
            }
        }
        if (GamePaused)                                                                         //If the game is paused
        {
            //Game is paused (Stop tick count and such)
        }
    }
    void OnApplicationFocus(bool hasFocus)                                              //Triggered when the game is in focus
    {
        IsOutOfFocus = !hasFocus;                                                               //Set game to be in focus
    }
    void OnApplicationPause(bool pauseStatus)                                           //Triggered when the game is out focus
    {
        IsOutOfFocus = pauseStatus;                                                             //Set game to be out of focus
    }

    private void ExecuteInputs()                                                        //Triggered in LateUpdate (unless the game is out of focus, or camera controls are disabled) this controlls the camera movement
    {
        if (!EventSystem.current.IsPointerOverGameObject())                                     //If mouse is not over an UI element
        {
            if (InHand)                                                                         //If we have something in our hands
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                    //Set a Ray from the cursor + lookation
                RaycastHit hit;                                                                 //Create a output variable
                if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Terrain")))  //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over on the Terrain layer only)
                {
                    InHand.transform.position = new Vector3(                                    //Move the block to the mouse position
                        Mathf.Round(hit.point.x),                                               //the rounded X mouse position
                        Mathf.Round(hit.point.y),                                               //the rounded Y mouse position
                        Mathf.Round(hit.point.z));                                              //the rounded Z mouse position

                    byte Special = CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName).BuildSpecial; //Save this for easy use
                    //ERROR LINE ABOVE: Building is missing the 'BuildingOption' code, please attach it to the object
                    if (Special == 1)                                                           //If this building can move up and down
                    {
                        if (LowerObjectBy > InHand.GetComponent<Collider>().bounds.size.y - 2)  //If this building is off the ground
                            LowerObjectBy = System.Convert.ToByte(InHand.GetComponent<Collider>().bounds.size.y - 2); //set HigherObject to max height of this object
                        InHand.transform.position -= new Vector3(0, LowerObjectBy, 0);          //Move the wall to it's set hight
                    }
                    else if (Special == 2)                                                      //If this building is a stair
                    {
                        Vector3 OneForward = new Vector3(                                       //A point 0.5 blocks away from the heigest part of the stair
                            InHand.transform.position.x + (InHand.transform.forward.x),         //InHand position + forward
                            InHand.transform.position.y + InHand.GetComponent<Collider>().bounds.size.y + 0.6f, //Height of the stair + a bit
                            InHand.transform.position.z + (InHand.transform.forward.z)          //InHand position + forward
                            );
                        //Debug.DrawRay(OneForward, -transform.up * InHand.GetComponent<Collider>().bounds.size.y, Color.red);   //Just a debug line 
                        if (Physics.Raycast(OneForward, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building")))//Do a raycast from OneForward towards the ground, and mesaure the length to a building
                        {
                            if (CodeInputManager.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).BuildSpecial == 2 //if the object hit is a stair
                            && Mathf.RoundToInt(Mathf.Abs(hit.transform.eulerAngles.y - InHand.transform.eulerAngles.y)) == 180) //And the stair is in the oposide direction
                                InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair down, so the top surface would match
                            else
                            {
                                string N = hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName;
                                if (N == "Wooden_Wall" || N == "Wooden_Stair" || N == "Stone_Wall" || N == "Stone_Stair")
                                    InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                                else
                                    InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                            }
                        }
                        else
                        {
                            Vector3 OneBackwards = new Vector3(                                 //A point 0.5 blocks away from the highest part of the stair
                                InHand.transform.position.x - (InHand.transform.forward.x),     //InHand position + forward
                                InHand.transform.position.y + InHand.GetComponent<Collider>().bounds.size.y - 0.4f, //Height of the stair + a bit
                                InHand.transform.position.z - (InHand.transform.forward.z)      //InHand position + forward
                                );
                            //Debug.DrawRay(OneBackwards, -transform.up * (InHand.GetComponent<Collider>().bounds.size.y), Color.red); //Just a debug line 
                            if (Physics.Raycast(OneBackwards, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building"))) //Do a raycast from OneBackwards towards the ground, and mesaure the length to a building
                            {
                                if (CodeInputManager.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).BuildSpecial == 2 //if the object hit is a stair
                                && Mathf.RoundToInt(Mathf.Abs(hit.transform.eulerAngles.y - InHand.transform.eulerAngles.y)) != 180) //And the stair is not in the oposide direction
                                    InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair up, the stair is going up  
                                else
                                {
                                    string N = hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName;
                                    if (N == "Wooden_Wall" || N == "Wooden_Stair" || N == "Stone_Wall" || N == "Stone_Stair")
                                        InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                                    else
                                        InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                                }
                               
                            }
                            else
                                InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                        }
                    }
                    else if (Special == 3)                                                      //If this building is a Fire_Basket
                    {
                        Debug.DrawRay(InHand.transform.position - transform.up, transform.up * 5, Color.red); //Just a debug line 
                        if (Physics.Raycast(InHand.transform.position - transform.up * 5, transform.up, out hit, 5, 1 << LayerMask.NameToLayer("Building")))//Do a raycast from the ground upwards (This would teturn the wall if there)
                        {
                            string N = hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName;//Get obstructing building name
                            if (N == "Stone_Wall" || N == "Stone_Gate" || N == "Stone_Tower")   //If it's a stone structure
                            {
                                InHand.transform.position = new Vector3(                        //Place it on top of the wall
                                    InHand.transform.position.x,                                //Keep X position the same
                                    hit.collider.bounds.size.y + hit.transform.position.y,      //Is the colider hight + object height offset (So on top of the collider)
                                    InHand.transform.position.z                                 //Keep Z position the same
                                );
                            }
                        }
                    }
                }
                if (CodeInputManager.GetButtonDownOnce(19))                                     //If we want to cancel the build
                    Destroy(InHand);                                                            //Destoy the building
                else if (CodeInputManager.GetButtonDown(18) && !IsDragging)                     //If we need to build the object here
                {
                    buildKeyDownTime += Time.deltaTime;                                         //Increase the build key timer
                    Build(InHand);                                                              //Try to place the building
                }
                else if (CodeInputManager.GetButtonDownOnce(10))                                //If we want to rotate the building
                {
                    if (CodeInputManager.GetButtonDown(20))                                     //If we want to rotate the other way
                        InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y - 90, 0); //Rotate it 90 degrees counter clock wise
                    else
                        InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y + 90, 0); //Rotate it 90 degrees clock wise
                    PreviousRotation = InHand.transform.rotation;                               //Save the rotation
                }
            }
            else if (DeconstructToolEquiped)                                                    //If the Deconstruct tool is aquiped
            {
                if (CodeInputManager.GetButtonDown(18))                                         //If we want to Deconstruct this building
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over                                      
                    {
                        if (CodeInputManager.GetButtonDownOnce(18))                             //If the button is pressed for the first time
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                        else if (CodeInputManager.GetButtonDown(20))                            //If the continue button is pressed
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                    }
                }
                else if (CodeInputManager.GetButtonDownOnce(19))                                //If we want to cancel Removing buildings
                {
                    DeconstructToolEquiped = false;                                             //Stop the DeconstructTool being equiped
                    SetCursor(null);                                                            //Reset cursor icon, so it isn't the Deconstruct Tool
                }
            }
            else
            {
                if (CodeInputManager.GetButtonDownOnce(18))                                     //If the button is pressed for the first time
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray 
                    {
                        _HideMenus();                                                           //Hide the Menu's
                        FolderBuildingPopUp.SetActive(true);                                    //Show BuildingPopUp
                        FolderBuildingPopUp.GetComponent<BuildingPopUp>().SelectBuilding(       //Open Pop-up window
                            hit.collider.gameObject,                                            //Send the gameobject that we have clicked on
                            CodeInputManager.GetInfo(hit.collider.GetComponent<BuildingOption>().BuildingName).ClickSpecial, //And it's special stats
                            ThisPlayerID
                            );
                        Debug.Log("You've clicked on " + hit.collider.name);
                    }
                }
            }
        }

        if (CodeInputManager.GetButtonUp(18))                                                   //Check if the build key was released
            buildKeyDownTime = 0;                                                               //Reset the timer

        if (CodeInputManager.GetButtonDownOnce(21) && LowerObjectBy > 0)                        //If we want to higher the object && the object is lower than the max heigth
            LowerObjectBy--;                                                                    //higher the object
        else if (CodeInputManager.GetButtonDownOnce(22))                                        //If we want to lower the object
            LowerObjectBy++;                                                                    //lower the object
        if (CodeInputManager.GetButtonDownOnce(19))                                             //If we right click to cancel
            _HideMenus();                                                                       //Hide the Menu's
        if (CodeInputManager.GetButtonDownOnce(9))                                              //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Goggle the UI
        {                                                                                       //Camera stuff
            Speed = (JelleWho.SpeedC * Camera.main.transform.position.y + JelleWho.SpeedD) * deltaTime; //The height has X of speed increase per block (times the time elapsed since last frame)
            Vector2 input = new Vector2(0f, 0f);                                                //Create a new (emnthy) movement change vector
            float Xr = Camera.main.transform.eulerAngles.x;                                     //Get main camera rotation
            float Yr = Camera.main.transform.eulerAngles.y;                                     //^
            if (CodeInputManager.GetButtonDown(6))                                              //If the given key has been pressed
            {
                Yr -= JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x = JelleWho.MoveSpeedKeyboard;                                           //Also move camera to the left
            }
            if (CodeInputManager.GetButtonDown(7))                                              //If the given key has been pressed
            {
                Yr += JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x -= JelleWho.MoveSpeedKeyboard;                                          //Also move camera to the right
            }
            if (CodeInputManager.GetButtonDown(1))                                              //If the given key has been pressed
            {
                Xr -= Input.GetAxis("Mouse Y") * JelleWho.RotateSpeedMouse * deltaTime;         //Get the mouse movement
                Yr += Input.GetAxis("Mouse X") * JelleWho.RotateSpeedMouse * deltaTime;         //^
            }
            if (CodeInputManager.GetButtonDown(2))                                              //Keyboard move left
                input.x -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(4))                                              //Keyboard move right
                input.x += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(5))                                              //Keyboard move up
                input.y += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(3))                                              //Keyboard move down
                input.y -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(0))                                              //If the Drag button is presse
            {
                input.x -= Input.GetAxis("Mouse X") * JelleWho.MoveSpeedMouse;                  //Calculate howmuch we need to move in the axes 
                input.y -= Input.GetAxis("Mouse Y") * JelleWho.MoveSpeedMouse;                  //^
            }
            else if (input == new Vector2(0f, 0f))                                              //If camera doesn't need to move yet
            {
                if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & 0x01) != 0x01)//If EdgeScroll setting is on
                {
                    float xpos = Input.mousePosition.x;                                         //Save mouse position
                    float ypos = Input.mousePosition.y;                                         //^        
                    if (xpos >= 0 && xpos < JelleWho.MoveIfThisCloseToTheSides)                 //Edge scroll left
                        input.x -= JelleWho.MoveEdgeScrollSpeed;                                //
                    else if (xpos <= Screen.width && xpos > Screen.width - JelleWho.MoveIfThisCloseToTheSides)//Edge scroll right
                        input.x += JelleWho.MoveEdgeScrollSpeed;                                //
                    if (ypos >= 0 && ypos < JelleWho.MoveIfThisCloseToTheSides)                 //Edge scroll down
                        input.y -= JelleWho.MoveEdgeScrollSpeed;                                //
                    else if (ypos <= Screen.height && ypos > Screen.height - JelleWho.MoveIfThisCloseToTheSides)//Edge scrolll up
                        input.y += JelleWho.MoveEdgeScrollSpeed;                                //
                }
            }
            if (Mathf.Abs(input.y) > Mathf.Epsilon)                                             //Movement up/down relative to the screen
            {
                Vector3 horDir = Camera.main.transform.forward;                                 //Get the camera position
                horDir.y = 0f;                                                                  //Set Up/Down movement to 0, so we ignore that direction
                horDir.Normalize();                                                             //If there is a value make it 1
                Vector3 newCameraPos = Camera.main.transform.position + horDir * Speed * input.y; //Get the new camera position
                Camera.main.transform.position = newCameraPos;                                  //Set camera position
            }
            if (Mathf.Abs(input.x) > Mathf.Epsilon)                                             //Movement left/right relative to the screen
            {
                Vector3 horDir = Camera.main.transform.right;                                   //Get the camera position 
                horDir.y = 0f;                                                                  //Set Up/Down movement to 0, so we ignore that direction
                horDir.Normalize();                                                             //If there is a value make it 1
                Vector3 newCameraPos = Camera.main.transform.position + horDir * Speed * input.x; //Get the new camera position
                Camera.main.transform.position = newCameraPos;                                  //Set camera position
            }
            float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");                       //Get the scrollwheel location
            if (ScrollWheelChange != 0 && !EventSystem.current.IsPointerOverGameObject())       //If the scrollwheel has changed (and zoom is enabled)
            {
                Vector3 cameraForward = Camera.main.transform.forward;                          //Get the angle (so forward is relative to the camera and not to North of the world)
                Vector3 newCameraPos = cameraForward * JelleWho.ZoomScrollWheelSpeed * ScrollWheelChange * Speed; //Create anc calculate new position of the camera
                Camera.main.transform.position += newCameraPos;                                 //Set camera position
            }
            Vector3 C = Camera.main.transform.position;                                         //Get setted camera camera position
            Camera.main.transform.position = new Vector3(                                       //Limit movement
                Mathf.Clamp(C.x, -JelleWho.MaxMoveHorizontalOnMap, JelleWho.MaxMoveHorizontalOnMap),//Clamp X horizontal movement
                Mathf.Clamp(C.y, JelleWho.MinCameraHeight, JelleWho.MaxCameraHeight),           //Clamp Y vertical movement
                Mathf.Clamp(C.z, -JelleWho.MaxMoveHorizontalOnMap, JelleWho.MaxMoveHorizontalOnMap));//Clamp Z 
            Camera.main.transform.eulerAngles = new Vector2(Mathf.Clamp(Xr, 0, 89.99f), Yr);    //Clamp Up Down looking angle 
        }                                                                                       //Camera stuff
    }
    /// <summary>
    /// Indicate the end of a dragging operation.
    /// </summary>
    internal void StopDragging()                                                                
    {
        IsDragging = false;                                                                     //Stop dragging
        if (InHand != null)                                                                     //If we don't have anything in our hand
        {
            Build(InHand);                                                                      //Drop a building if we have one
            Destroy(InHand);                                                                    //Empty our hand
        }
    }
    /// <summary>
    /// Indicate the start of a dragging operation.
    /// </summary>
    internal void StartDragging()
    {
        IsDragging = true;                                                                      //Indicate that we're dragging
    }
    /// <summary>
    /// Try to place a building.
    /// </summary>
    /// <param name="prefab">The GameObject to place</param>
    public void Build(GameObject prefab)
    {
        prefab.layer = 0;                                                                       //Set to default Layer
        InHand inHand = prefab.GetComponent<InHand>();                                          //Get a reference to the in-hand component
        if (inHand == null || !inHand.validPlacement || inHand.CheckCollision())                //Check if everything is clear
        {
            if (buildKeyDownTime > InvalidBuildTimeThreshold)                                   //Have we been trying to build here long?
                ShowMessage("Can't build here");                                                //Give player more feedback
            return;                                                                             //Don't do anything else
        }
        string Pay = CanWePayFor(prefab);                                                       //Create a new string, will return what we are missing if we can't build
        if (Pay == "Done")                                                                      //If we do have enough to build this building
        {
            GameObject building = Instantiate(prefab);                                          //Clone the prefab as a new building
            building.layer = LayerMask.NameToLayer("Building");                                 //Set this to be in the building layer (so we can't build on this anymore)
            building.transform.SetParent(FolderBuildings);                                      //Sort the building in the right folder
            building.GetComponent<BuildingOption>().StartTimer();                               //Start the 'Used' after timer if this object
            building.GetComponent<BuildingOption>().OwnerID = ThisPlayerID;                     //Set the current player to be the owner of this building
            Destroy(building.GetComponent<InHand>());                                           //Remove the in-hand component
        }
        else
        {
            ShowMessage("Not enough " + Pay + " to build that");                                //Give the user the warning message
        }
    }
    public void CameraControls(bool SetTo)                                              //With this buttons can change the camera mode
    {
        StopCameraControls = !SetTo;                                                            //Set the camera mode to what ever there is given (CameraControls FALSE = Stop camera)
    }
    private void AlwaysControls()                                                       //Triggered in LateUpdate (unless the game is out of focus)
    {
        if (CodeInputManager.GetButtonDownOnce(8))                                              //If the Open/Close menu button is pressed
        {
            StopCameraControls = !FolderMenu.activeSelf;                                        //Flag that the camera controls should be active or not
            FolderMenu.SetActive(StopCameraControls);                                           //Set the menu's visibility
        }
        if (CodeInputManager.GetButtonDownOnce(17))                                             //If the Open/Close menu button is pressed
            GamePaused = true;
    }
    public void _PlaceInHand(GameObject Prefab)                                         //Triggered by menu, with the object to build as prefab, this will hook in to the mouse cursor
    {
        Destroy(InHand);                                                                        //Destroy the current held item (If any)
        Prefab.transform.position = new Vector3(0, -100, 0);                                    //Hide new building
        PlaceInHand(Prefab);                                                                    //Place the new building on our hand
    }
    private void PlaceInHand(GameObject Prefab)                                         //With the object to build as prefab, this will hook in to the mouse cursor
    {
        _DeconstructTool(false);                                                                //Make sure the DeconstructTool is NOT Equiped
        InHand = Instantiate(Prefab);                                                           //Create a new building and put it in our hands (coord will be set later)
        InHand.transform.rotation = PreviousRotation;                                           //Restore the rotation
        InHand.layer = 0;                                                                       //Set to default Layer
        if (InHand.GetComponent<InHand>() == null)                                              //Check for in-hand functionality
            InHand.AddComponent<InHand>();                                                      //Give the object in-hand functionality
    }
    public void _DeconstructTool(bool Equiped)                                          //Triggered by menu, Equipe the Deconstruct tool
    {
        if (DeconstructToolEquiped != Equiped)                                                  //If the tool status is not up to date, and need to be changed
        {
            DeconstructToolEquiped = Equiped;                                                   //Set the given state
            if (Equiped)                                                                        //If DeconstructTool is still active
            {
                Destroy(InHand);                                                                //Destoy the building (This will cancel 'NowBuilding' if it's ative)
                SetCursor(MouseDeconstruct);                                                    //Set the mouse cursor to be the Deconstruct Tool
            }
            else
                SetCursor(null);                                                                //Reset cursor icon, so it isn't the Deconstruct Tool
        }
    }
    public void _HideMenus()                                                            //This will hide the full sub menu
    {
        FolderBuildingPopUp.SetActive(false);                                                   //Hide BuildingPopUp
        foreach (Transform child in FolderSubMenu.transform)                                    //Do for each SubMenu
        {
            child.gameObject.SetActive(false);                                                  //Hide the SubMenu
        }
    }
    Vector3 PolarToCartesian(Vector2 polar, Vector3 Offset)                             //Offset=(Left, Up, Forward)
    {
        var rotation = Quaternion.Euler(polar.x, polar.y, 0);                                   //Convert it
        return rotation * Offset;                                                               //Return the Vector 3 of the target point
    }
    public void ShowMessage(string Message)                                             //If we need to show the player a message
    {
        TextMessage.GetComponentInChildren<Text>().text = Message;                              //Give the user the message
        TextMessage.SetActive(true);                                                            //Show the message (This objects auto hides)
        StartCoroutine(ShowMessageAttention());
    }
    public void DeconstructBuilding(GameObject TheBuilding)                             //This code will give stuff back and deconstruct the building
    {
        //If this line gives an error. Check if 'TheBuilding' had the code 'BuildingOption' attached to it. Also check that only this parrent is on the 'Building' layer 
        Building BuildingInfo = CodeInputManager.GetInfo(TheBuilding.GetComponent<BuildingOption>().BuildingName);  //Get the buildings info (like cost etc)
        if (TheBuilding.GetComponent<BuildingOption>().Used)                            //If the building is not brand new
        {
            CodeResourceManager.GetComponent<ResourceManager>().ChangeWood (BuildingInfo.Cost_Wood  * JelleWho.DeconstructUsed); //Return some percentage
            CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(BuildingInfo.Cost_Stone * JelleWho.DeconstructUsed); //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeIron (BuildingInfo.Cost_Iron  * JelleWho.DeconstructUsed); //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeGold (BuildingInfo.Cost_Gold  * JelleWho.DeconstructUsed); //^
        }
        else
        {
            CodeResourceManager.GetComponent<ResourceManager>().ChangeWood (BuildingInfo.Cost_Wood  * JelleWho.DeconstructUnused); //Return some percentage
            CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(BuildingInfo.Cost_Stone * JelleWho.DeconstructUnused); //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeIron (BuildingInfo.Cost_Iron  * JelleWho.DeconstructUnused); //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeGold (BuildingInfo.Cost_Gold  * JelleWho.DeconstructUnused); //^
        }
        TheBuilding.GetComponent<BuildingOption>()._Destroy();                           //Destroy the building
    }
    private string CanWePayFor(GameObject TheBuilding)                                  //Checks if we can pay for a building, and pays if posible. else it will return what we don't have enough off
    {
        Building BuildingInfo = CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName);
        //Debug.Log("Type=" + BuildingInfo.Name +" Cost_Wood=" + BuildingInfo.Cost_Wood +" Cost_Stone=" + BuildingInfo.Cost_Stone +" Cost_Iron=" + BuildingInfo.Cost_Iron +" Cost_Gold=" + BuildingInfo.Cost_Gold);
        if (CodeResourceManager.GetComponent<ResourceManager>().Wood >= BuildingInfo.Cost_Wood)             //If we have enough Wood
        {
            if (CodeResourceManager.GetComponent<ResourceManager>().Stone >= BuildingInfo.Cost_Stone)       //If we have enough Stone
            {
                if (CodeResourceManager.GetComponent<ResourceManager>().Iron >= BuildingInfo.Cost_Iron)     //If we have enough Iron
                {
                    if (CodeResourceManager.GetComponent<ResourceManager>().Gold >= BuildingInfo.Cost_Gold) //If we have enough Gold
                    {
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeWood (-BuildingInfo.Cost_Wood);  //Remove the cost from the wood the player has
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(-BuildingInfo.Cost_Stone); //^
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeIron (-BuildingInfo.Cost_Iron);  //^
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeGold (-BuildingInfo.Cost_Gold);  //^
                        return "Done";                                                          //Return with; the payed = Done command
                    }
                    else
                        return "Gold";                                                          //Return with; We dont have enough of this
                }
                else
                    return "Iron";                                                              //Return with; We dont have enough of this
            }
            else
                return "Stone";                                                                 //Return with; We dont have enough of this
        }
        else
            return "Wood";                                                                      //Return with; We dont have enough of this
    }
    private void SetCursor(Texture2D NewCursorIcon)                                     //Call this to change the mouse icon (Use 'NULL' to reset to normal)
    {
        //Make sure to set the texture type of the image to Cursor!
        Cursor.SetCursor(NewCursorIcon, Vector2.zero, CursorMode.Auto);                         //Set the image as icon
    }
    public void _LoadFromFile(String TheFile)                                           //Call this to call the LoadFile handler
    {
        CodeSaveLoad.GetComponent<SaveLoad>().LoadFromFile(TheFile);                            //Call the handler
    }
    public void _SaveToFile(String TheFile)                                             //Call this to call the SaveFile handler
    {
        CodeSaveLoad.GetComponent<SaveLoad>().SaveToFile(TheFile);                              //Call the handler
    }
    private IEnumerator ShowMessageAttention()
    {
        float StartTime = Time.time;
        float CurrentTime = StartTime;
        while (StartTime + 0.05f > CurrentTime)
        {
            CurrentTime = Time.time;
            if (TextMessage.GetComponent<Image>().color == Color.white)
                TextMessage.GetComponent<Image>().color = new Color(100, 0, 0, 100);
            else
                TextMessage.GetComponent<Image>().color = Color.white;
            yield return null;
        }
        TextMessage.GetComponent<Image>().color = Color.white;
    }




    public InputField SaveLoadTXT;

    public void _TESTLoad2(InputField TheFile)
    {
        if (TheFile.text == "")
        {
            _LoadFromFile("TestFile.txt");
            Debug.LogWarning("No file name given, taking 'TestFile.txt'");
        }
        else
            _LoadFromFile(TheFile.text);
    }
    public void _TESTSave2(InputField TheFile)
    {
        if (TheFile.text == "")
        {
            _SaveToFile("TestFile.txt");
            Debug.LogWarning("No file name given, taking 'TestFile.txt'");
        }
        else
            _SaveToFile(TheFile.text);
    }



    public void _Opti()
    {
        StaticBatchingUtility.Combine(FolderBuildings.gameObject);
    }
    public void _TempSetResourceManager()
    {
        CodeResourceManager.GetComponent<ResourceManager>().Set(999999, 999999, 999999, 999999);
    }   
}