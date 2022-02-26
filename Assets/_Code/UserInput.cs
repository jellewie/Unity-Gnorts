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
    private Byte LowerObjectBy = 0;                                                     //How much the gameobject should be higher (this is used for walls as example)
    public Texture2D MouseDefault;                                                      //The default mouse icon   
    public Texture2D MouseDeconstruct;                                                  //The mouse icon of the deconstruct tool
    public Byte ThisPlayerID;
    Quaternion PreviousRotation;
    private readonly float InvalidBuildTimeThreshold = 0.5f;                                //How long before we show a message about not being able to build
    public bool IsDragging { get; set; }                                                //Are we dragging something?
    private bool IsOutOfFocus = false;
    private bool GamePaused = false;
    private bool StopCameraControls = false;
    private GameObject InHand;                                                          //When placing down this is stuffed with the object
    private bool DeconstructToolEquiped;                                                //If the DeconstructTool is Equiped
    private float deltaTime = 0.0f;                                                     //The time between this and the last frame
    private float buildKeyDownTime;                                                     //How long the build key has been down
    private bool BuildFirstTry = true;
    private float Speed;                                                                //Speed multiplication for controls (zoom out slowdown)
    private float doubleClickTime = .3f, lastClickTime;

    private void Start()                                                                //Triggered on start
    {
        SetCursor(MouseDefault);
    }

    private void Update()                                                               //Triggered before frame update
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;                               //Calculate time elapsed since last frame
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= doubleClickTime)
            {
                Debug.Log("Double click");
                Manager.instance.isDoubleClick = true;                                               //Activate double click flag 
            }             
            lastClickTime = Time.time;
        }
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
                var goName = InHand.gameObject.name;                                            //Get object in hand name
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                    //Set a Ray from the cursor + lookation
                RaycastHit hit;                                                                 //Create a output variable
                if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Terrain")))  //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over on the Terrain layer only)
                {                    
                    InHand.transform.position = new Vector3(                                    //Move the block to the mouse position
                        Mathf.Round(hit.point.x),                                               //the rounded X mouse position
                        Mathf.Round(hit.point.y),                                               //the rounded Y mouse position
                        Mathf.Round(hit.point.z)                                                //the rounded Z mouse position
                    );
                    BuildType type = BuildingData.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName).BuildType; //Save this for easy use
                    //ERROR LINE ABOVE: Building is missing the 'BuildingOption' code, please attach it to the object
                    if (type == BuildType.Wall || type == BuildType.SpikedWall)                 //If this building can move up and down
                    {
                        byte MinHeight = 2;                                                     //Set the MinHeight to be 2 blocks (from the ground)
                        if (type == BuildType.SpikedWall)                                       //If it's a spiked wall
                            MinHeight = 4;                                                      //Set the MinHeight to be 4 blocks (from the ground) (so the Spikes are 1 block above the Wall)
                        if (LowerObjectBy > InHand.GetComponent<Collider>().bounds.size.y - MinHeight) //If this building is to far in the ground
                            LowerObjectBy = System.Convert.ToByte(InHand.GetComponent<Collider>().bounds.size.y - MinHeight); //set HigherObject to be the min height of this building
                        InHand.transform.position -= new Vector3(0, LowerObjectBy, 0);          //Move the wall to it's set hight
                    }
                    else if (type == BuildType.Stair)                                           //If this building is a stair
                    {
                        Vector3 OneForward = new Vector3(                                       //A point 0.5 blocks away from the heigest part of the stair
                            InHand.transform.position.x + (InHand.transform.forward.x),         //InHand position + forward
                            InHand.transform.position.y + InHand.GetComponent<Collider>().bounds.size.y + 0.6f, //Height of the stair + a bit
                            InHand.transform.position.z + (InHand.transform.forward.z)          //InHand position + forward
                            );
                        if (Physics.Raycast(OneForward, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building")))//Do a raycast from OneForward towards the ground, and mesaure the length to a building
                        {
                            BuildType typeHit = BuildingData.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).BuildType;
                            if (typeHit == BuildType.Stair //if the object hit is a stair
                            && Mathf.RoundToInt(Mathf.Abs(hit.transform.eulerAngles.y - InHand.transform.eulerAngles.y)) == 180) //And the stair is in the oposide direction
                                InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair down, so the top surface would match
                            else
                            {
                                if (typeHit == BuildType.Stair || typeHit == BuildType.Wall)
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
                            if (Physics.Raycast(OneBackwards, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building"))) //Do a raycast from OneBackwards towards the ground, and mesaure the length to a building
                            {
                                BuildType typeHit = BuildingData.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).BuildType;
                                if (typeHit == BuildType.Stair //if the object hit is a stair
                                && Mathf.RoundToInt(Mathf.Abs(hit.transform.eulerAngles.y - InHand.transform.eulerAngles.y)) != 180) //And the stair is not in the oposide direction
                                    InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair up, the stair is going up  
                                else
                                {
                                    if (typeHit == BuildType.Stair || typeHit == BuildType.Wall)
                                        InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                                    else
                                        InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                                }
                            }
                            else
                                InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                        }
                    }
                    else if (type == BuildType.FireBasket)                                      //If this building is a Fire_Basket
                    {
                        if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over on the Terrain layer only)
                        {
                            string N = hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName;//Get obstructing building name
                            if (N == "Stone_Wall" || N == "Stone_Gate" || N == "Stone_Tower" || N == "Keep") //If it's a stone structure
                            {
                                InHand.transform.position = new Vector3(                        //Place it on top of the wall
                                    Mathf.Round(hit.point.x),                                   //Keep X position the same
                                    hit.collider.bounds.size.y + hit.transform.position.y,      //Is the colider hight + object height offset (So on top of the collider)
                                    Mathf.Round(hit.point.z)                                    //Keep Z position the same
                                );
                            }
                        }
                    }
                }
                if (CodeInputManager.GetButtonDownOnce(ButtonId.CancelBuild))                   //If we want to cancel the build
                    Destroy(InHand);                                                            //Destroy the building               
                else if(CodeInputManager.GetButtonDown(ButtonId.Build) && !IsDragging           //Walls should be placed continously 
                    &&                                                                          //Is more satisfying :)
                    IsBuildingMeantToBePlacedContinuously(goName) == true)
                {
                    buildKeyDownTime += Time.deltaTime;                                         //Increase the build key timer
                    Build(InHand);
                }                                                                              // .. Buildings should be place 1 by 1
                else if (CodeInputManager.GetButtonDownOnce(ButtonId.Build) && !IsDragging)         //If we need to build the object here
                {                                                                                
                    buildKeyDownTime += Time.deltaTime;                                         //Increase the build key timer
                    Build(InHand);                                                              //Try to place the building
                }
                else if (CodeInputManager.GetButtonDownOnce(ButtonId.RotateBuilding))           //If we want to rotate the building
                {
                    if (CodeInputManager.GetButtonDown(ButtonId.Alternative))                   //If we want to rotate the other way
                        InHand.transform.rotation *= Quaternion.Euler(0, -90, 0);               //Rotate it 90 degrees counter clock wise
                    else
                        InHand.transform.rotation *= Quaternion.Euler(0, 90, 0);                //Rotate it 90 degrees clock wise
                    PreviousRotation = InHand.transform.rotation;                               //Save the rotation
                }
            }
            else if (DeconstructToolEquiped)                                                    //If the Deconstruct tool is aquiped
            {
                if (CodeInputManager.GetButtonDown(ButtonId.Build))                               //If we want to Deconstruct this building
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over                                      
                    {
                        if (CodeInputManager.GetButtonDown(ButtonId.Build))                   //If the button is pressed for the first time
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                        else if (CodeInputManager.GetButtonDown(ButtonId.Alternative))            //If the continue button is pressed
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())                 //If mouse is not over an UI element
                                DeconstructBuilding(hit.transform.gameObject);                  //Deconstruct the selected building
                        }
                    }
                }
                else if (CodeInputManager.GetButtonDownOnce(ButtonId.CancelBuild))                //If we want to cancel Removing buildings
                {                                          
                    SetCursor(MouseDefault);                                                            //Reset cursor icon, so it isn't the Deconstruct Tool
                }
            }
            else
            {
                if (CodeInputManager.GetButtonDownOnce(ButtonId.Build))                         //If the button is pressed for the first time
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                //Set a Ray from the cursor + lookation
                    RaycastHit hit;                                                             //Create a output variable
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray 
                    {
                        if (CodeInputManager.GetButtonDown(ButtonId.Alternative))
                            HideBuildMenus();                                                   //Hide the Build Menu's
                        else
                            _HideMenus();                                                       //Hide all menu's (this will also deselect pop-up building)

                        FolderBuildingPopUp.SetActive(true);                                    //Show BuildingPopUp
                        FolderBuildingPopUp.GetComponent<BuildingPopUp>().SelectBuilding(       //Open Pop-up window
                            hit.collider.gameObject,                                            //Send the gameobject that we have clicked on
                            BuildingData.GetInfo(hit.collider.GetComponent<BuildingOption>().BuildingName).ClickSpecial, //And it's special stats
                            ThisPlayerID                                                        //And the ID of the player
                        );

                       
                        Manager.instance.isSelected = true;                                     //Activate flag for selection outline
                        if (Manager.instance.isDoubleClick)                                     //If player makes multiple selection of the same object
                        {
                            string name = hit.collider.gameObject.name;
                            GameObject buildings = GameObject.Find("Buildings");
                            foreach(Transform go in buildings.transform)
                            {
                                if(go.name == name)
                                {
                                    go.transform.GetComponent<BuildingOption>().SetSelected(true);
                                }                             
                            }
                            Debug.Log("Implement double click select all buildings of a type in scene");
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<BuildingOption>().SetSelected(true);
                        }

                        ChangeChildrenLayer(11);                                                //Changes child game object to outliner layer

                        FolderBuildingPopUp
                            .GetComponent<BuildingPopUp>()
                            .DisplayGameObjectInformation
                            .GetComponentInChildren<Image>().enabled = true;                      //Activate Image Component on UI    
                        FolderBuildingPopUp
                            .GetComponent<BuildingPopUp>()
                            .DisplayGameObjectInformation
                            .GetComponentInChildren<Image>().sprite = 
                            hit.collider.GetComponent<BuildingOption>().Sprite;                  //Set Image Component to building Sprite
                    }
                }

                if (CodeInputManager.GetButtonDown(ButtonId.CancelBuild))                        //Deselect buildings
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);               
                    RaycastHit hit;
                    
                    if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Terrain")))
                    {
                        Manager.instance.isSelected = false;
                        Manager.instance.isDoubleClick = false;
                        ChangeChildrenLayer(10);                                                //Changes child game object to building layer
                    }
                }
            }
        }
        if (CodeInputManager.GetButtonUp(ButtonId.Build))                                       //Check if the build key was released
        {
            buildKeyDownTime = 0;                                                               //Reset the timer
            BuildFirstTry = true;
        }
        if (CodeInputManager.GetButtonDownOnce(ButtonId.WallsHigher) && LowerObjectBy > 0)      //If we want to higher the object && the object is lower than the max heigth
            LowerObjectBy--;                                                                    //higher the object
        else if (CodeInputManager.GetButtonDownOnce(ButtonId.WallsLower))                       //If we want to lower the object
            LowerObjectBy++;                                                                    //lower the object
        if (CodeInputManager.GetButtonDownOnce(ButtonId.CancelBuild))                           //If we right click to cancel
            _HideMenus();                                                                       //Hide the Menu's
        if (CodeInputManager.GetButtonDownOnce(ButtonId.ToggleUi))                              //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Toggle the UI
        {                                                                                       //Camera stuff
            Speed = (JelleWho.SpeedC * Camera.main.transform.position.y + JelleWho.SpeedD) * deltaTime; //The height has X of speed increase per block (times the time elapsed since last frame)
            Vector2 input = new Vector2(0f, 0f);                                                //Create a new (emnthy) movement change vector
            float Xr = Camera.main.transform.eulerAngles.x;                                     //Get main camera rotation
            float Yr = Camera.main.transform.eulerAngles.y;                                     //^
            if (CodeInputManager.GetButtonDown(ButtonId.RotateLeft))                            //If the given key has been pressed
            {
                Yr -= JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x = JelleWho.MoveSpeedKeyboard;                                           //Also move camera to the left
            }
            if (CodeInputManager.GetButtonDown(ButtonId.RotateRight))                           //If the given key has been pressed
            {
                Yr += JelleWho.RotateSpeedKeyboard * deltaTime;                                 //Get the mouse movement
                input.x -= JelleWho.MoveSpeedKeyboard;                                          //Also move camera to the right
            }
            if (CodeInputManager.GetButtonDown(ButtonId.Rotate))                                //If the given key has been pressed
            {
                Xr -= Input.GetAxis("Mouse Y") * JelleWho.RotateSpeedMouse * deltaTime;         //Get the mouse movement
                Yr += Input.GetAxis("Mouse X") * JelleWho.RotateSpeedMouse * deltaTime;         //^
            }
            if (CodeInputManager.GetButtonDown(ButtonId.Left))                                  //Keyboard move left
                input.x -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(ButtonId.Right))                                 //Keyboard move right
                input.x += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(ButtonId.Up))                                    //Keyboard move up
                input.y += JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(ButtonId.Down))                                  //Keyboard move down
                input.y -= JelleWho.MoveSpeedKeyboard;
            if (CodeInputManager.GetButtonDown(ButtonId.Drag))                                  //If the Drag button is pressed
            {
                input.x -= Input.GetAxis("Mouse X") * JelleWho.MoveSpeedMouse;                  //Calculate how much we need to move in the axes 
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

    private bool IsBuildingMeantToBePlacedContinuously(string name)
    {
        if (name.Contains("Wall"))
            return true;
        if (name.Contains("Stair"))
            return true;
        if (name.Contains("Moat"))          //Moats should also be able to placed continuously
            return true;
        else
            return false;
    }

    private void ChangeChildrenLayer(int layer)
    {
        GameObject buildings = GameObject.Find("Buildings");
        if (layer == 10)                                            // Building layer
        {            
            foreach (Transform go in buildings.transform)
            {
                go.GetComponent<BuildingOption>().SetSelected(false);
                SetMeshChildren(layer, go);
            }
        }
        if (layer == 11)                                            // Outliner layer 
        {
            foreach (Transform go in buildings.transform)
            {
                if (go.GetComponent<BuildingOption>().GetSelected() == true)        // Changes only selected game objects that are selected to outliner layer
                {
                    go.transform.gameObject.layer = layer;
                    SetMeshChildren(layer, go);
                }                
            }
        }       
    }

    private static void SetMeshChildren(int layer, Transform go)
    {
        go.transform.gameObject.layer = layer;
        Transform[] meshChildren = go.Find("Mesh").GetComponentsInChildren<Transform>();
        foreach (Transform child in meshChildren)
        {
            child.gameObject.layer = layer;
        }
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
            if (BuildFirstTry)                                                                  //If the user is starting to trying to build 
            {
                ShowMessage("Can't build here ");                                               //Give player more feedback
                BuildFirstTry = false;                                                          //Flag that we have processed the start of this button press
            }
            if (buildKeyDownTime > InvalidBuildTimeThreshold)                                   //Have we been trying to build here long?
                ShowMessage("Can't build here ");                                               //Give player more feedback
            return;                                                                             //Don't do anything else
        }
        string Pay = CanWePayFor(prefab);                                                       //Create a new string, will return what we are missing if we can't build
        if (Pay == "Done")                                                                      //If we do have enough to build this building
        {
            BuildFirstTry = false;                                                              //Flag that we have processed the start of this button press
            buildKeyDownTime = 0;                                                               //Reset the message timer (this if for keep dragging support)
            if (inHand.RemoveObjectsHit.Count > 0)                                              //If there are objects we need to remove
            {
                for (int i = 0; i < inHand.RemoveObjectsHit.Count; i++)                         //Do for each object
                    DeconstructBuilding(inHand.RemoveObjectsHit[i]);                            //Deconstruct the building
            }
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
        if (CodeInputManager.GetButtonDownOnce(ButtonId.Menu))                                    //If the Open/Close menu button is pressed
        {
            StopCameraControls = !FolderMenu.activeSelf;                                        //Flag that the camera controls should be active or not
            FolderMenu.SetActive(StopCameraControls);                                           //Set the menu's visibility
        }
        if (CodeInputManager.GetButtonDownOnce(ButtonId.Pause))                                   //If the Open/Close menu button is pressed
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
                SetCursor(MouseDefault);
        }
    }
    public void _HideMenus()                                                            //This will hide the full sub menu
    {
        SetCursor(MouseDefault);
        FolderBuildingPopUp.SetActive(false);                                                   //Hide BuildingPopUp
        HideBuildMenus();
    }
    public void HideBuildMenus()                                                            //This will hide the full sub menu
    {
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
        //If this line gives an error. Check if 'TheBuilding' had the code 'BuildingOption' attached to it. Also check that only this parent is on the 'Building' layer 
        BuildingInfo buildingInfo = BuildingData.GetInfo(TheBuilding.GetComponent<BuildingOption>().BuildingName);  //Get the buildings info (like cost etc)
        ResourceManager resourceManager = CodeResourceManager.GetComponent<ResourceManager>();
        if (TheBuilding.GetComponent<BuildingOption>().Used)                                    //If the building is not brand new
        {
            resourceManager.ChangeWood (buildingInfo.Cost.Wood  * JelleWho.DeconstructUsed);    //Return some percentage
            resourceManager.ChangeStone(buildingInfo.Cost.Stone * JelleWho.DeconstructUsed);
            resourceManager.ChangeIron (buildingInfo.Cost.Iron  * JelleWho.DeconstructUsed);
            resourceManager.ChangeGold (buildingInfo.Cost.Gold  * JelleWho.DeconstructUsed); 
        }
        else
        {
            resourceManager.ChangeWood (buildingInfo.Cost.Wood  * JelleWho.DeconstructUnused);  //Return some percentage
            resourceManager.ChangeStone(buildingInfo.Cost.Stone * JelleWho.DeconstructUnused);
            resourceManager.ChangeIron (buildingInfo.Cost.Iron  * JelleWho.DeconstructUnused);
            resourceManager.ChangeGold (buildingInfo.Cost.Gold  * JelleWho.DeconstructUnused);
        }

        byte DestroySpecial = BuildingData.GetInfo(TheBuilding.GetComponent<BuildingOption>().BuildingName).DestroySpecial;
        if (DestroySpecial == 1)                                                                //NO 'ELSE!'. If we need to remove the posible balista
        {
            TheBuilding.layer = 0;                                                              //Set the layer of TheBuilding to be default so we wont hit it 
            Collider[] Hits = Physics.OverlapBox(                                               //Get al objects (FireBaskets) on top
                TheBuilding.GetComponent<BoxCollider>().bounds.center + transform.up,           //Center + 1 higher so we would get all FireBaskets 1 block above this object
                TheBuilding.GetComponent<BoxCollider>().size / 2.5f,                            //A bit smaller then it zelf else it might also hit it neighbors 
                transform.rotation,
                 ~(1 << LayerMask.NameToLayer("Terrain") | 1));                                 //Skip ground and the object itzelf
            for (int i = 0; i < Hits.Length; i++)                                               //For each object hit
            {
                String Name = Hits[i].gameObject.GetComponent<BuildingOption>().BuildingName;
                if (Name == "Fire_Basket" || Name == "Ballista_Tower" || Name == "Mangonel_Tower") //If it's a fire basket or balista or Mangonel
                    DeconstructBuilding(Hits[i].gameObject);                                       //Deconstruct it
            }
            DestroySpecial = 2;                                                                 //Also execute the code for DestroySpecial 3 on this object
        }
        if (DestroySpecial == 2)                                                                //NO 'ELSE!'. If we need to move NPC(s) down
        {
            Debug.Log("<Move NPC down code>");
        }
        else if (DestroySpecial > 2)
            Debug.Log("unprogrammed Destroy special found :" + DestroySpecial);


        TheBuilding.GetComponent<BuildingOption>()._Destroy();                                  //Destroy the building
    }
    private string CanWePayFor(GameObject TheBuilding)                                  //Checks if we can pay for a building, and pays if possible. else it will return what we don't have enough off
    {
        BuildingInfo buildingInfo = BuildingData.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName);
        //Debug.Log("Type=" + BuildingInfo.Name +" Cost_Wood=" + BuildingInfo.Cost_Wood +" Cost_Stone=" + BuildingInfo.Cost_Stone +" Cost_Iron=" + BuildingInfo.Cost_Iron +" Cost_Gold=" + BuildingInfo.Cost_Gold);
        ResourceManager resourceManager = CodeResourceManager.GetComponent<ResourceManager>();
        if (resourceManager.Wood >= buildingInfo.Cost.Wood)                                     //If we have enough Wood
        {
            if (resourceManager.Stone >= buildingInfo.Cost.Stone)                               //If we have enough Stone
            {
                if (resourceManager.Iron >= buildingInfo.Cost.Iron)                             //If we have enough Iron
                {
                    if (resourceManager.Gold >= buildingInfo.Cost.Gold)                         //If we have enough Gold
                    {
                        resourceManager.ChangeWood (-buildingInfo.Cost.Wood);                   //Remove the cost from the wood the player has
                        resourceManager.ChangeStone(-buildingInfo.Cost.Stone);                  //^
                        resourceManager.ChangeIron (-buildingInfo.Cost.Iron);                   //^
                        resourceManager.ChangeGold (-buildingInfo.Cost.Gold);                   //^
                        return "Done";                                                          //Return with; the payed = Done command
                    }
                    else
                        return "Gold";                                                          //Return with; We don't have enough of this
                }
                else
                    return "Iron";                                                              //Return with; We don't have enough of this
            }
            else
                return "Stone";                                                                 //Return with; We don't have enough of this
        }
        else
            return "Wood";                                                                      //Return with; We don't have enough of this
    }
    private void SetCursor(Texture2D NewCursorIcon)                                     //Call this to change the mouse icon (Use 'NULL' to reset to normal)
    {
        if (NewCursorIcon == MouseDefault)
            DeconstructToolEquiped = false;                                             //Stop the DeconstructTool being equiped
     
        //Make sure to set the texture type of the image to Cursor!
        Cursor.SetCursor(NewCursorIcon, Vector2.zero, CursorMode.Auto);                 //Set the image as icon
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

    public void _FileSave()
    {
        bool Responce = CodeSaveLoad.GetComponent<SaveLoad>()._SaveToFile(ThisPlayerID);
        if (Responce)
        {
            Debug.Log("Saved!");
        }
    }
    public void _FileLoadFromFile()
    {
        bool Responce = CodeSaveLoad.GetComponent<SaveLoad>()._LoadFromFile();
        if (Responce)
        {
            Debug.Log("Loaded!");
        }
    }
    public void _FileDeleteFile()
    {
        bool Responce = CodeSaveLoad.GetComponent<SaveLoad>()._DeleteFile();
        if (Responce)
        {
            Debug.Log("Deleted!");
        }
    }
    public void _BackToMenu()                                               //Menu button that let you go back to the main menu
    {                                                                       //^
        Application.Quit();                                                 //Since there is a lack of main menu right now, let's quit the application
    }

    public void _TEST()
    {
        CodeSaveLoad.GetComponent<SaveLoad>().LoadSaveList();
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
 