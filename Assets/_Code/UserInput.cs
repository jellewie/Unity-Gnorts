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
    public InputManager CodeInputManager;
    public GameObject CodeUserStats;                                                    //The GameObject with the code on it
    public GameObject FolderMenu;                                                       //The folder to enable on MenuOpen
    public GameObject FolderInfo;
    public GameObject FolderTrading;
    public GameObject FolderGraph;
    public GameObject FolderUI;                                                         //The folder to hide on HideUI
    public GameObject FolderSubMenu;                                                    //The folder to close when done building
    public Transform FolderBuildings;                                                   //The folder where all the buildings should be put in
    public GameObject TextMessage;
    private Byte LowerObjectBy = 0;                                                      //Howmuch the gameobject should be higher (this is used for walls as example)

    Quaternion PreviousRotation;

    bool IsOutOfFocus = false;
    bool GamePaused = false;
    bool StopCameraControls = false;
    private GameObject InHand;                                                          //When placing down this is stuffed with the object
    bool DeconstructToolEquiped;

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
        InHand = Instantiate(Prefab, new Vector3(0, -100, 0), Quaternion.identity);             //Create a new building and put it in our hands (coord will be set later)
        InHand.transform.rotation = PreviousRotation;                                           //Restore the rotation
        InHand.transform.SetParent(FolderBuildings);                                            //Sort the building in the right folder
        InHand.layer = 0;                                                                       //Set to default Layer
    }
    public void _DeconstructTool(bool Equiped)                                          //Triggered by menu, Equipe the Deconstruct tool
    {
        Destroy(InHand);                                                                        //Destoy the building
        DeconstructToolEquiped = Equiped;                                                       //Set the given state
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
            {
                InHand.transform.position = new Vector3(                                        //Move the block to the mouse position
                    Mathf.Round(hit.point.x),                                                   //the rounded X mouse position
                    Mathf.Round(hit.point.y),                                                   //the rounded Y mouse position
                    Mathf.Round(hit.point.z));                                                  //the rounded Z mouse position
               
                if (CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName).Special == "/") //If this building is a stair
                {
                    Vector3 OneForward = new Vector3(                                           //A point 0.5 blocks away from the heigest part of the stair
                        InHand.transform.position.x + (InHand.transform.forward.x),             //InHand position + forward
                        InHand.transform.position.y + InHand.GetComponent<Collider>().bounds.size.y + 0.6f, //Height of the stair + a bit
                        InHand.transform.position.z + (InHand.transform.forward.z)              //InHand position + forward
                        );
                    Debug.DrawRay(OneForward, -transform.up * InHand.GetComponent<Collider>().bounds.size.y, Color.red);   //Just a debug line 
                    if (Physics.Raycast(OneForward, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building")))//Do a raycast from OneForward towards the ground, and mesaure the length to a building
                    {
                        if (CodeInputManager.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).Special == "/" //if the object hit is a stair
                        && Mathf.RoundToInt(Mathf.Abs(hit.transform.eulerAngles.y - InHand.transform.eulerAngles.y)) == 180) //an the stair is in the oposide direction
                            InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair down, so the top surface would match
                        else
                            InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                    }
                    else
                    {
                        Vector3 OneBackwards = new Vector3(                                     //A point 0.5 blocks away from the heigest part of the stair
                            InHand.transform.position.x - (InHand.transform.forward.x),         //InHand position + forward
                            InHand.transform.position.y + InHand.GetComponent<Collider>().bounds.size.y - 0.4f, //Height of the stair + a bit
                            InHand.transform.position.z - (InHand.transform.forward.z)          //InHand position + forward
                            );
                        Debug.DrawRay(OneBackwards, -transform.up * (InHand.GetComponent<Collider>().bounds.size.y), Color.red); //Just a debug line 
                        if (Physics.Raycast(OneBackwards, -transform.up, out hit, InHand.GetComponent<Collider>().bounds.size.y, 1 << LayerMask.NameToLayer("Building"))) //Do a raycast from OneBackwards towards the ground, and mesaure the length to a building
                        {
                            if (CodeInputManager.GetInfo(hit.transform.gameObject.GetComponent<BuildingOption>().BuildingName).Special == "/") //if the object hit is a stair
                                InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance) + 1, 0); //Move the stair down, so the top surface would match
                            else
                                InHand.transform.position += new Vector3(0, -Mathf.RoundToInt(hit.distance), 0); //Move the stair down, so the top surface would match
                        }
                        else
                            InHand.transform.position += new Vector3(0, -InHand.GetComponent<Collider>().bounds.size.y + 0.5f, 0); //Move the stair down
                    }
                }
                else if (CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName).Special == "+") //If this building can move up and down
                {
                    if(LowerObjectBy > InHand.GetComponent<Collider>().bounds.size.y -2)         //If this building is off the ground
                    {
                        LowerObjectBy = System.Convert.ToByte(InHand.GetComponent<Collider>().bounds.size.y - 2); //set HigherObject to max height of this object
                    }
                    InHand.transform.position -= new Vector3(0, LowerObjectBy, 0);               //Move the wall to it's set hight
                }
            }




            if (CodeInputManager.GetButtonDownOnce("Walls higher"))                             //If we want to higher the object
            {
                if (LowerObjectBy > 0)                                                          //If the object is lower than the max heigth
                    LowerObjectBy--;                                                            //higher the object
            }
            else if (CodeInputManager.GetButtonDownOnce("Walls lower"))                         //If we want to lower the object
            {
                LowerObjectBy++;                                                                //lower the object
            }





            if (CodeInputManager.GetButtonDownOnce("Cancel build"))                             //If we want to cancel the build
            {
                Destroy(InHand);                                                                //Destoy the building
            }
            else if (CodeInputManager.GetButtonDown("Build"))                                   //If we need to build the object here
            {
                InHand.layer = 0;                                                               //Set to default Layer
                RaycastHit[] Hit = Physics.BoxCastAll(                                          //Cast a ray to see if there is already a building where we are hovering over
                    InHand.GetComponent<Collider>().bounds.center,                              //The center of the block
                    (InHand.GetComponent<BoxCollider>().size / 2.1f) - new Vector3(0.5f, 0, 0.5f),  //Size of center to side of the block (minus a bit to make sure we dont touch the next block)
                    -transform.up,                                                              //Do the ray downwards (in to the ground basicly to check only it's own position)
                    InHand.GetComponent<Collider>().transform.rotation,                         //The orientation in Quaternion (Always in steps of 90 degrees)
                    0.5f,                                                                       //Dont go much depth, the building should be inside this block
                    1 << LayerMask.NameToLayer("Building"));                                    //Only try to find buildings
                if (Hit.Length > 0)                                                             //If there a building already there
                {
                    //Debug.Log("Can't build on top " + Hit[0].transform.name);
                    /*TODO FIXME 
                    If this is hit for more than <1 sec> than show a message that we can't build there
                     */
                }
                else
                {
                    if (!EventSystem.current.IsPointerOverGameObject())                         //If mouse is not over an UI element
                    {
                        string Pay = CanWePayFor(InHand);                                       //Create a new string, will return what we are missing if we can't build
                        if (Pay == "Done")                                                      //If we do have enough to build this building
                        {
                            InHand.layer = LayerMask.NameToLayer("Building");                   //Set this to be in the building layer (so we can't build on this anymore)
                            InHand.GetComponent<BuildingOption>().StartTimer();                 //Start the 'Used' after timer if this object
                            PlaceInHand(InHand);                                                //Put a new building on our hands, and leave this one be (this one is now placed down)
                        }
                        else
                        {
                            ShowMessage("Not enough " + Pay + " to build that");                //Give the user the warning message
                        }
                    }
                }
            }
            else if (CodeInputManager.GetButtonDownOnce("Rotate building"))                     //If we want to rotate the building
            {
                if (CodeInputManager.GetButtonDown("Alternative"))                              //If we want to rotate the other way
                    InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y - 90, 0);    //Rotate it 90 degrees counter clock wise
                else
                    InHand.transform.rotation = Quaternion.Euler(0, InHand.transform.eulerAngles.y + 90, 0);    //Rotate it 90 degrees clock wise
                PreviousRotation = InHand.transform.rotation;                                   //Save the rotation
            }
        }
        else if (DeconstructToolEquiped)                                                        //If the Deconstruct tool is aquiped
        {
            if (CodeInputManager.GetButtonDown("Build"))                                        //If we want to Deconstruct this building
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                    //Set a Ray from the cursor + lookation
                RaycastHit hit;                                                                 //Create a output variable
                if (Physics.Raycast(ray, out hit, 512, 1 << LayerMask.NameToLayer("Building"))) //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over                                      
                {
                    if (CodeInputManager.GetButtonDownOnce("Build"))                            //If the button is pressed for the first time
                    {
                        if (!EventSystem.current.IsPointerOverGameObject())                     //If mouse is not over an UI element
                            DeconstructBuilding(hit.transform.gameObject);                      //Deconstruct the selected building
                    }
                    else if (CodeInputManager.GetButtonDown("Alternative"))                     //If the continue button is pressed
                    {
                        if (!EventSystem.current.IsPointerOverGameObject())                     //If mouse is not over an UI element
                            DeconstructBuilding(hit.transform.gameObject);                      //Deconstruct the selected building
                    }
                }
            }
            else if (CodeInputManager.GetButtonDownOnce("Cancel build"))                        //If we want to cancel Removing buildings
                DeconstructToolEquiped = false;                                                 //Stop the DeconstructTool being equiped
        }
        if (CodeInputManager.GetButtonDownOnce("Cancel build"))                                 //If we right click to cancel
            _HideSubMenu();                                                                     //Hide the sub menu
        if (CodeInputManager.GetButtonDownOnce("Toggle UI"))                                    //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Goggle the UI
        float Speed = Camera.main.transform.position.y * JelleWho.HeighSpeedIncrease;           //The height has X of speed increase per block
        Vector2 input = new Vector2(0f, 0f);                                                    //Create a new (emnthy) movement change vector
        if (CodeInputManager.GetButtonDown("Left"))                                             //Keyboard scroll left
            input.x -= 1f * JelleWho.MoveSpeedKeyboard;
        if (CodeInputManager.GetButtonDown("Right"))                                            //Keyboard scroll right
            input.x += 1f * JelleWho.MoveSpeedKeyboard;
        if (CodeInputManager.GetButtonDown("Up"))                                               //Keyboard scroll up
            input.y += 1f * JelleWho.MoveSpeedKeyboard;
        if (CodeInputManager.GetButtonDown("Down"))                                             //Keyboard scroll down
            input.y -= 1f * JelleWho.MoveSpeedKeyboard;
        if (CodeInputManager.GetButtonDown("Drag"))                                             //If the Drag button is presse
        {
            input.x -= Input.GetAxis("Mouse X") * JelleWho.MoveSpeedMouse * (Speed / 2);        //Calculate howmuch we need to move in the axes 
            input.y -= Input.GetAxis("Mouse Y") * JelleWho.MoveSpeedMouse * (Speed / 2);        //^
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
        if (ScrollWheelChange != 0 && !EventSystem.current.IsPointerOverGameObject())           //If the scrollwheel has changed (and zoom is enabled)
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
        float Yr = Camera.main.transform.eulerAngles.y;                                         //^
        if (CodeInputManager.GetButtonDown("Rotate left"))                                      //If the given key has been pressed
            Yr -= JelleWho.RotateSpeedKeyboard;                                                 //Get the mouse movement
        if (CodeInputManager.GetButtonDown("Rotate right"))                                     //If the given key has been pressed
            Yr += JelleWho.RotateSpeedKeyboard;                                                 //Get the mouse movement
        if (CodeInputManager.GetButtonDown("Rotate"))                                           //If the given key has been pressed
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
    private void ShowMessage(string Message)                                            //If we need to show the player a message
    {
        TextMessage.GetComponentInChildren<Text>().text = Message;                              //Give the user the message
        TextMessage.SetActive(true);                                                            //Show the message (This objects auto hides)
    }
    public void DeconstructBuilding(GameObject TheBuilding)                             //This code will give stuff back and deconstruct the building
    {
        //If this line gives an error. Check if 'TheBuilding' had the code 'BuildingOption' attached to it. Also check that only this parrent is on the 'Building' layer 
        Building BuildingInfo = CodeInputManager.GetInfo(TheBuilding.GetComponent<BuildingOption>().BuildingName);  //Get the buildings info (like cost etc)
        if (TheBuilding.GetComponent<BuildingOption>().Used)                                    //If the building is not brand new
        {
            CodeUserStats.GetComponent<UserStats>().ChangeWood (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Wood  * JelleWho.DeconstructUsed)));  //Return some percentage
            CodeUserStats.GetComponent<UserStats>().ChangeStone(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Stone * JelleWho.DeconstructUsed)));  //^
            CodeUserStats.GetComponent<UserStats>().ChangeIron (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Iron  * JelleWho.DeconstructUsed)));  //^
            CodeUserStats.GetComponent<UserStats>().ChangeMoney(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Money * JelleWho.DeconstructUsed)));  //^
        }
        else
        {
            CodeUserStats.GetComponent<UserStats>().ChangeWood (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Wood  * JelleWho.DeconstructUnused)));  //Return some percentage
            CodeUserStats.GetComponent<UserStats>().ChangeStone(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Stone * JelleWho.DeconstructUnused)));  //^
            CodeUserStats.GetComponent<UserStats>().ChangeIron (Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Iron  * JelleWho.DeconstructUnused)));  //^
            CodeUserStats.GetComponent<UserStats>().ChangeMoney(Convert.ToInt64(Mathf.Round(BuildingInfo.Cost_Money * JelleWho.DeconstructUnused)));  //^
        }
        Destroy(TheBuilding);                                                                   //Destroy the building
    }
    private string CanWePayFor(GameObject TheBuilding)                                  //Checks if we can pay for a building, and pays if posible. else it will return what we don't have enough off
    {
        Building BuildingInfo = CodeInputManager.GetInfo(InHand.GetComponent<BuildingOption>().BuildingName);
        //Debug.Log("Type=" + BuildingInfo.Name +" Cost_Wood=" + BuildingInfo.Cost_Wood +" Cost_Stone=" + BuildingInfo.Cost_Stone +" Cost_Iron=" + BuildingInfo.Cost_Iron +" Cost_Money=" + BuildingInfo.Cost_Money);
        if (CodeUserStats.GetComponent<UserStats>().Wood >= BuildingInfo.Cost_Wood)             //If we have enough Wood
        {
            if (CodeUserStats.GetComponent<UserStats>().Stone >= BuildingInfo.Cost_Stone)       //If we have enough Stone
            {
                if (CodeUserStats.GetComponent<UserStats>().Iron >= BuildingInfo.Cost_Iron)     //If we have enough Iron
                {
                    if (CodeUserStats.GetComponent<UserStats>().Money >= BuildingInfo.Cost_Money)//If we have enough Money
                    {
                        CodeUserStats.GetComponent<UserStats>().ChangeWood(-BuildingInfo.Cost_Wood);    //Remove the cost from the wood the player has
                        CodeUserStats.GetComponent<UserStats>().ChangeStone(-BuildingInfo.Cost_Stone);  //^
                        CodeUserStats.GetComponent<UserStats>().ChangeIron(-BuildingInfo.Cost_Iron);    //^
                        CodeUserStats.GetComponent<UserStats>().ChangeMoney(-BuildingInfo.Cost_Money);  //^
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
