using UnityEngine;
using PublicCode;
using UnityEngine.EventSystems;                                                         //Used to: Check if hovering over UI while building,
using System;
using UnityEngine.UI;                                                                   //We need this to interact with the UI
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

    Quaternion PreviousRotation;

    public bool IsDragging { get; set; }                                                //Are we dragging something?

    private bool IsOutOfFocus = false;
    private bool GamePaused = false;
    private bool StopCameraControls = false;
    private GameObject InHand;                                                          //When placing down this is stuffed with the object
    private bool DeconstructToolEquiped;                                                //If the DeconstructTool is Equiped
    private float deltaTime = 0.0f;                                                     //The time between this and the last frame
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
                    if (Special == 2)                                                           //If this building is a stair
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
                                InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                        }
                        else
                        {
                            Vector3 OneBackwards = new Vector3(                                 //A point 0.5 blocks away from the heigest part of the stair
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
                                    InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                            }
                            else
                                InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                        }
                    }
                    else if (Special == 1)                                                      //If this building can move up and down
                    {
                        if (LowerObjectBy > InHand.GetComponent<Collider>().bounds.size.y - 2)  //If this building is off the ground
                            LowerObjectBy = System.Convert.ToByte(InHand.GetComponent<Collider>().bounds.size.y - 2); //set HigherObject to max height of this object
                        InHand.transform.position -= new Vector3(0, LowerObjectBy, 0);          //Move the wall to it's set hight
                    }
                }
                if (CodeInputManager.GetButtonDownOnce("Cancel build"))                         //If we want to cancel the build
                    Destroy(InHand);                                                            //Destoy the building
                else if (CodeInputManager.GetButtonDown("Build") && !IsDragging)                //If we need to build the object here
                {
                    Build(InHand);
                }
                else if (CodeInputManager.GetButtonDownOnce("Rotate building"))                 //If we want to rotate the building
                {
                    if (CodeInputManager.GetButtonDown("Alternative"))                          //If we want to rotate the other way
                        InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y - 90, 0);    //Rotate it 90 degrees counter clock wise
                    else
                        InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y + 90, 0);    //Rotate it 90 degrees clock wise
                    PreviousRotation = InHand.transform.rotation;                               //Save the rotation
                }
            }
            else if (DeconstructToolEquiped)                                                    //If the Deconstruct tool is aquiped
            {
                if (CodeInputManager.GetButtonDown("Build"))                                    //If we want to Deconstruct this building
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over                                      
                    {
                        if (CodeInputManager.GetButtonDownOnce("Build"))                        //If the button is pressed for the first time
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                        else if (CodeInputManager.GetButtonDown("Alternative"))                 //If the continue button is pressed
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                    }
                }
                else if (CodeInputManager.GetButtonDownOnce("Cancel build"))                    //If we want to cancel Removing buildings
                {
                    DeconstructToolEquiped = false;                                             //Stop the DeconstructTool being equiped
                    SetCursor(null);                                                            //Reset cursor icon, so it isn't the Deconstruct Tool
                }
            }
            else
            {
                if (CodeInputManager.GetButtonDownOnce("Build"))                                //If the button is pressed for the first time
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray 
                    {
                        _HideMenus();                                                           //Hide the Menu's
                        FolderBuildingPopUp.SetActive(true);                                    //Show BuildingPopUp
                        FolderBuildingPopUp.GetComponent<BuildingPopUp>().SelectBuilding(       //Open Pop-up window
                        hit.collider.gameObject,                                                //Send the gameobject that we have clicked on
                        CodeInputManager.GetInfo(hit.collider.GetComponent<BuildingOption>().BuildingName).ClickSpecial); //And it's special stats
                        Debug.Log("You've clicked on " + hit.collider.name);
                    }
                }
            }
        }

        if (CodeInputManager.GetButtonDownOnce("Walls higher") && LowerObjectBy > 0)            //If we want to higher the object && the object is lower than the max heigth
            LowerObjectBy--;                                                                    //higher the object
        else if (CodeInputManager.GetButtonDownOnce("Walls lower"))                             //If we want to lower the object
            LowerObjectBy++;                                                                    //lower the object
        if (CodeInputManager.GetButtonDownOnce("Cancel build"))                                 //If we right click to cancel
            _HideMenus();                                                                       //Hide the Menu's
        if (CodeInputManager.GetButtonDownOnce("Toggle UI"))                                    //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Goggle the UI

        {                                                                                       //Camera stuff
            Speed = (JelleWho.SpeedC * Camera.main.transform.position.y + JelleWho.SpeedD) * deltaTime; //The height has X of speed increase per block (times the time elapsed since last frame)
            Vector2 input = new Vector2(0f, 0f);                                                //Create a new (emnthy) movement change vector
            float Xr = Camera.main.transform.eulerAngles.x;                                     //Get main camera rotation
            float Yr = Camera.main.transform.eulerAngles.y;                                     //^
            if (CodeInputManager.GetButtonDown("Rotate left"))                                  //If the given key has been pressed
            {
                Yr -= JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x = JelleWho.MoveSpeedKeyboard;                                           //Also move camera to the left
            }
            if (CodeInputManager.GetButtonDown("Rotate right"))                                 //If the given key has been pressed
            {
                Yr += JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x -= JelleWho.MoveSpeedKeyboard;                                          //Also move camera to the right
            }
            if (CodeInputManager.GetButtonDown("Rotate"))                                       //If the given key has been pressed
            {
                Xr -= Input.GetAxis("Mouse Y") * JelleWho.RotateSpeedMouse * deltaTime;         //Get the mouse movement
                Yr += Input.GetAxis("Mouse X") * JelleWho.RotateSpeedMouse * deltaTime;         //^
            }
            if (CodeInputManager.GetButtonDown("Left"))                                         //Keyboard move left
                input.x -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown("Right"))                                        //Keyboard move right
                input.x += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown("Up"))                                           //Keyboard move up
                input.y += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown("Down"))                                         //Keyboard move down
                input.y -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown("Drag"))                                         //If the Drag button is presse
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
        IsDragging = false;                                                                     //Stop drgging
        if (InHand != null)
            Build(InHand);                                                                      //Drop a building if we have one
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
    /// <param name="building">The GameObject to place</param>
    public void Build(GameObject building)
    {
        building.layer = 0;                                                                     //Set to default Layer
        RaycastHit[] Hit = Physics.BoxCastAll(                                                  //Cast a ray to see if there is already a building where we are hovering over
            building.GetComponent<Collider>().bounds.center,                                    //The center of the block
            (building.GetComponent<BoxCollider>().size / 2.1f) - new Vector3(0.5f, 0, 0.5f),    //Size of center to side of the block (minus a bit to make sure we dont touch the next block)
            -transform.up,                                                                      //Do the ray downwards (in to the ground basicly to check only it's own position)
            building.GetComponent<Collider>().transform.rotation,                               //The orientation in Quaternion (Always in steps of 90 degrees)
            0.5f,                                                                               //Dont go much depth, the building should be inside this block
            1 << LayerMask.NameToLayer("Building"));                                            //Only try to find buildings
        if (Hit.Length > 0)                                                                     //If there a building already there
        {
            //Debug.Log("Can't build on top " + Hit[0].transform.name);
            /*TODO FIXME 
            If this is hit for more than <1 sec> than show a message that we can't build there
             */
        }
        else
        {
            string Pay = CanWePayFor(building);                                                 //Create a new string, will return what we are missing if we can't build
            if (Pay == "Done")                                                                  //If we do have enough to build this building
            {
                building.layer = LayerMask.NameToLayer("Building");                             //Set this to be in the building layer (so we can't build on this anymore)
                building.GetComponent<BuildingOption>().StartTimer();                           //Start the 'Used' after timer if this object
                PlaceInHand(building);                                                          //Put a new building on our hands, and leave this one be (this one is now placed down)
            }
            else
                ShowMessage("Not enough " + Pay + " to build that");                            //Give the user the warning message
        }
    }

    public void CameraControls(bool SetTo)                                              //With this buttons can change the camera mode
    {
        StopCameraControls = !SetTo;                                                            //Set the camera mode to what ever there is given (CameraControls FALSE = Stop camera)
    }
    private void AlwaysControls()                                                       //Triggered in LateUpdate (unless the game is out of focus)
    {
        if (CodeInputManager.GetButtonDownOnce("Menu"))                                         //If the Open/Close menu button is pressed
        {
            StopCameraControls = !FolderMenu.activeSelf;                                        //Flag that the camera controls should be active or not
            FolderMenu.SetActive(StopCameraControls);                                           //Set the menu's visibility
        }
        if (CodeInputManager.GetButtonDownOnce("Pause"))                                        //If the Open/Close menu button is pressed
            GamePaused = true;
    }
    public void _PlaceInHand(GameObject Prefab)                                         //Triggered by menu, with the object to build as prefab, this will hook in to the mouse cursor
    {
        Destroy(InHand);                                                                        //Destroy the current held item (If any)
        PlaceInHand(Prefab);                                                                    //Place the new building on our hand
    }
    private void PlaceInHand(GameObject Prefab)                                         //With the object to build as prefab, this will hook in to the mouse cursor
    {
        _DeconstructTool(false);                                                                //Make sure the DeconstructTool is NOT Equiped
        InHand = Instantiate(Prefab, new Vector3(0, -100, 0), Quaternion.identity);             //Create a new building and put it in our hands (coord will be set later)
        InHand.transform.rotation = PreviousRotation;                                           //Restore the rotation
        InHand.transform.SetParent(FolderBuildings);                                            //Sort the building in the right folder
        InHand.layer = 0;                                                                       //Set to default Layer
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
    private void ShowMessage(string Message)                                            //If we need to show the player a message
    {
        TextMessage.GetComponentInChildren<Text>().text = Message;                              //Give the user the message
        TextMessage.SetActive(true);                                                            //Show the message (This objects auto hides)
    }
    public void DeconstructBuilding(GameObject TheBuilding)                             //This code will give stuff back and deconstruct the building
    {
        //If this line gives an error. Check if 'TheBuilding' had the code 'BuildingOption' attached to it. Also check that only this parrent is on the 'Building' layer 
        Building BuildingInfo = CodeInputManager.GetInfo(TheBuilding.GetComponent<BuildingOption>().BuildingName);  //Get the buildings info (like cost etc)
        if (TheBuilding.GetComponent<BuildingOption>().Used)                            //If the building is not brand new
        {
            CodeResourceManager.GetComponent<ResourceManager>().ChangeWood (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Wood  * JelleWho.DeconstructUsed)));  //Return some percentage
            CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Stone * JelleWho.DeconstructUsed)));  //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeIron (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Iron  * JelleWho.DeconstructUsed)));  //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeMoney(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Money * JelleWho.DeconstructUsed)));  //^
        }
        else
        {
            CodeResourceManager.GetComponent<ResourceManager>().ChangeWood (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Wood  * JelleWho.DeconstructUnused)));  //Return some percentage
            CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Stone * JelleWho.DeconstructUnused)));  //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeIron (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Iron  * JelleWho.DeconstructUnused)));  //^
            CodeResourceManager.GetComponent<ResourceManager>().ChangeMoney(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Money * JelleWho.DeconstructUnused)));  //^
        }
        TheBuilding.GetComponent<BuildingOption>()._Destroy();                           //Destroy the building
    }
    private string CanWePayFor(GameObject TheBuilding)                                  //Checks if we can pay for a building, and pays if posible. else it will return what we don't have enough off
    {
        Building BuildingInfo = CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName);
        //Debug.Log("Type=" + BuildingInfo.Name +" Cost_Wood=" + BuildingInfo.Cost_Wood +" Cost_Stone=" + BuildingInfo.Cost_Stone +" Cost_Iron=" + BuildingInfo.Cost_Iron +" Cost_Money=" + BuildingInfo.Cost_Money);
        if (CodeResourceManager.GetComponent<ResourceManager>().Wood >= BuildingInfo.Cost_Wood)             //If we have enough Wood
        {
            if (CodeResourceManager.GetComponent<ResourceManager>().Stone >= BuildingInfo.Cost_Stone)       //If we have enough Stone
            {
                if (CodeResourceManager.GetComponent<ResourceManager>().Iron >= BuildingInfo.Cost_Iron)     //If we have enough Iron
                {
                    if (CodeResourceManager.GetComponent<ResourceManager>().Money >= BuildingInfo.Cost_Money)//If we have enough Money
                    {
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeWood(-BuildingInfo.Cost_Wood);    //Remove the cost from the wood the player has
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeStone(-BuildingInfo.Cost_Stone);  //^
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeIron(-BuildingInfo.Cost_Iron);    //^
                        CodeResourceManager.GetComponent<ResourceManager>().ChangeMoney(-BuildingInfo.Cost_Money);  //^
                        return "Done";                                                          //Return with; the payed = Done command
                    }
                    else
                        return "Money";                                                         //Return with; We dont have enough of this
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
    public void _LoadFromString(String Data)                                            //Call this to call the LoadData handler
    {
        CodeSaveLoad.GetComponent<SaveLoad>().LoadFromSring(Data);                              //Call the handler
    }
    public void _SaveToString(String Data)                                              //Call this to call the SaveData handler
    {
        SaveLoadTXT.text = CodeSaveLoad.GetComponent<SaveLoad>().SaveToSring(Data);             //Call the handler
    }





    

    public InputField SaveLoadTXT;
    public void _TESTLoad()
    {
        _LoadFromString(SaveLoadTXT.text);
    }
    public void _TESTSave()
    {
        _SaveToString(SaveLoadTXT.text);
    }



    public void _Opti()
    {
        StaticBatchingUtility.Combine(FolderBuildings.gameObject);
    }
    public void _TempSetResourceManager()
    {
        CodeResourceManager.GetComponent<ResourceManager>().Set(999999, 999999, 999999, 999999);
    }


    


    //https://answers.unity.com/questions/546045/how-can-i-access-a-bool-for-a-specific-gameobject.html
    //public class MyNewClass
    //{
    //    public bool aBool;
    //}
    //public BuildingSettings[] myNewClassArray;
    //public void _Test()
    //{
    //    foreach (BuildingSettings thisClass in myNewClassArray)
    //    {
    //        thisClass.aBool = !thisClass.aBool;
    //        Debug.Log(thisClass.aBool);
    //    }
    //}
}

/*  public GameObject Tempblock;
    void Temp()
    {
        Vector3 OffsetXYZ = Camera.main.transform.position;                                     //The Starting position
        Vector2 LookingAt = Camera.main.transform.eulerAngles;                                  //The angles we are looking at
        Vector3 Range = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));     //The range where we need to go relative to the angles
        Tempblock.transform.position = OffsetXYZ + PolarToCartesian(LookingAt, Range);          //Calculate and move
    }*/
