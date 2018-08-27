using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PublicCode;
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
    readonly int IgnoreBuildingRaycast = 1 << 9;                                                //Ignore these when placing down buildings. 8 is the mask layer (1<< 4 = ob1000 = isgnore only layer 4)

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
        IsOutOfFocus = !hasFocus;                                                                   //Set game to be in focus
    }
    void OnApplicationPause(bool pauseStatus)                                           //Triggered when the game is out focus
    {
        IsOutOfFocus = pauseStatus;                                                                 //Set game to be out of focus
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
    public void _PlaceBuilding(GameObject Prefab)                                       //Triggered by menu, with the object to build as prefab, this will hook in to the mouse cursor
    {
        Destroy(InHand);
        InHand = Instantiate(Prefab, new Vector3(0, -100, 0), Quaternion.identity);             //Create a building (we dont need to set it's position, will do later in this loop
        InHand.transform.rotation = PreviousRotation;                                           //Restore the rotation
        InHand.transform.SetParent(FolderBuildings);                                            //Sort the building in the right folder
    }
    public void _HideSubMenu()       //This will hide the full sub menu
    {
        foreach (Transform child in FolderSubMenu.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    private void ExecuteInputs()                                                                //Triggered in LateUpdate (unless the game is out of focus, or camera controls are disabled) this controlls the camera movement
    {
        if (InHand)                                                                             //If we have something in our hands
        {
            InHand.layer = 0;                                                       //Set to default Layer 

Debug.Log("Raycast");
            Quaternion Rotation = InHand.GetComponent<Collider>().transform.rotation;           //The orientation in Quaternion (Always in steps of 90 degrees)
            Vector3 Origin = InHand.GetComponent<Collider>().bounds.center;                     //The center of the block
            Vector3 Size = (InHand.GetComponent<BoxCollider>().size / 2.1f) - new Vector3(0.5f, 0, 0.5f); //Size of center to side of the block (minus a bit to make sure we dont touch the next block)
            var layerMask = 1 << LayerMask.NameToLayer("Building");                             //Only try to find buildings
            RaycastHit[] Hit = Physics.BoxCastAll(Origin, Size, -transform.up, Rotation, 0f, layerMask);    //Cast a ray to see if there is already a building where we are hovering over






            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                        //Set a Ray from the cursor + lookation
            RaycastHit hit;                                                                     //Create a output variable
            if (Physics.Raycast(ray, out hit, 512, IgnoreBuildingRaycast))                      //Send the Ray (This will return "hit" with the exact XYZ coords the mouse is over
            InHand.transform.position = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z)); //Move the block there
            if (inputManager.GetButtonDown("Build"))                                            //If we need to build the object here
            {
Debug.Log("Build" + Hit.Length);
                if (Hit.Length > 0)                                                             //If there a building already there
                {
                    Debug.Log("Can not build on top of " + Hit[0].collider.name);               //Just a debug 
                    //TODO FIXME add an in screen popup (which doesnt trigger when shift building!) 
                }
                else
                {
                    InHand.layer = LayerMask.NameToLayer("Building");                           //Set this to be in the building layer (so we can't build on this anymore)
                    PreviousRotation = InHand.transform.rotation;                               //Save the rotation
                    if (inputManager.GetButtonDown("Alternative"))                              //If we want to keep building
                    {
                        InHand = Instantiate(InHand, new Vector3(0, -100, 0), Quaternion.identity); //Create a building (we dont need to set it's position, will do later in this loop
                        InHand.transform.rotation = PreviousRotation;                           //Restore the rotation
                        InHand.transform.SetParent(FolderBuildings);                            //Sort the building in the right folder
                    }
                    else
                    {
                        InHand = null;                                                          //Clear our hand (and place the building where it now it)
                        _HideSubMenu();                                                         //Hide the sub menu
                    }
                }
            }
            else if (inputManager.GetButtonDownOnce("Cancel build"))                            //If we want to cancel the build
            {
                PreviousRotation = InHand.transform.rotation;                                   //Save the rotation
                Destroy(InHand);                                                                //Destoy the building
                _HideSubMenu();                                                                  //Hide the sub menu
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
        if (inputManager.GetButtonDownOnce("Toggle UI"))                                        //If the Toggle UI button is pressed
            FolderUI.SetActive(!FolderUI.activeSelf);                                           //Goggle the UI
        float Speed = Camera.main.transform.position.y * JelleWho.HeighSpeedIncrease;           //The height has X of speed increase per block
        Vector2 input = new Vector2(0f, 0f);
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

/*  int resWidth = 2550;
    int resHeight = 3300;
    private bool takeHiResShot = false;
    
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                                  Application.dataPath,
                                  width, height,
                                  System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
    
    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }
    
    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    
    
    
        Debug.Log("TEST");
        ScreenCapture.CaptureScreenshot("TestScreenshot.png");
    }*/



/*  public GameObject Tempblock;
    void Temp()
    {
        Vector3 OffsetXYZ = Camera.main.transform.position;                                     //The Starting position
        Vector2 LookingAt = Camera.main.transform.eulerAngles;                                  //The angles we are looking at
        Vector3 Range = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));     //The range where we need to go relative to the angles
        Tempblock.transform.position = OffsetXYZ + PolarToCartesian(LookingAt, Range);          //Calculate and move
    }*/
