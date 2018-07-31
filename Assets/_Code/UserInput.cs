using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PublicCode;

public class UserInput : MonoBehaviour
{
    private Player _player;                                                                     //Reffrence to the player

    private void Start()                                                                //On start
    {
        _player = transform.root.GetComponent<Player>();                                        //Fill the player reference
    }

    private void Update()                                                               //Update before frame
    {
    }


    private void LateUpdate()                                                           //Update after frame
    {
        if (_player.human)                                                                      //If the input is the User
        {
            MoveCamera();                                                                       //Check if we need to move the camera
        }
    }


    private Vector3 MouseDragOrigin;                                                            //Mouse position when started with dragging
    private bool Dragging = false;

    private void MoveCamera()
    {
        float UD = Camera.main.transform.eulerAngles.x + 90;                                    //Get camera up and down in degrees
        float LR = -1 * (Camera.main.transform.eulerAngles.y - 90);                             //Get left to right in degrees
        float X = Camera.main.transform.position.x;                                             //Get main camera location
        float Y = Camera.main.transform.position.y;                                             //^
        float Z = Camera.main.transform.position.z;                                             //^
        float Speed = Y * JelleWho.HeighSpeedIncrease;                                          //The height has X of speed increase per block
        //Edge scroll
        float xpos = Input.mousePosition.x;                                                     //Save mouse position
        float ypos = Input.mousePosition.y;                                                     //^
        if (xpos >= 0 && xpos < JelleWho.MoveIfThisCloseToTheSides)                             //horizontal camera movement
        {
            X -= JelleWho.MoveEdgeScrollSpeed * Speed;                                          //Scroll a bit
        }
        else if (xpos <= Screen.width && xpos > Screen.width - JelleWho.MoveIfThisCloseToTheSides)
        {
            X += JelleWho.MoveEdgeScrollSpeed * Speed;                                          //Scroll a bit
        }
        if (ypos >= 0 && ypos < JelleWho.MoveIfThisCloseToTheSides)                             //vertical camera movement
        {
            Z -= JelleWho.MoveEdgeScrollSpeed * Speed;                                          //Scroll a bit
        }
        else if (ypos <= Screen.height && ypos > Screen.height - JelleWho.MoveIfThisCloseToTheSides)
        {
            Z += JelleWho.MoveEdgeScrollSpeed * Speed;                                          //Scroll a bit
        }

        if (Input.GetMouseButton(1))                                                            //detect rotation amount
        {
            int speed = 30;
            X = X - Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed;
            Z = Z - Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed;

            

            //We need to optain the direction of the screen and adjust the 2 lines above to it. it now only works in NORTH map view




        }
        Debug.Log(LR);

        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");                           //Get the scrollwheel location
        if (ScrollWheelChange != 0)                                                             //If the scrollwheel has changed
        {                          
            float R = ScrollWheelChange * JelleWho.ZoomScrollWheelSpeed * Camera.main.transform.position.y;//The radius from current camera
            UD = UD / 180 * Mathf.PI;                                                           //Convert from degrees to radians
            LR = LR / 180 * Mathf.PI;                                                           //^
            X = X + R * Mathf.Sin(UD) * Mathf.Cos(LR);                                          //Calculate new coords
            Y = Y + R * Mathf.Cos(UD);                                                          //^
            Z = Z + R * Mathf.Sin(UD) * Mathf.Sin(LR);                                          //^
        }
        if (X > JelleWho.MaxMoveHorizontalOnMap)                                                //Limit range North South(?)
        {
            X = JelleWho.MaxMoveHorizontalOnMap;                                                //^
        }
        else if (X < -1 * JelleWho.MaxMoveHorizontalOnMap)                                      //^
        {
            X = -1 * JelleWho.MaxMoveHorizontalOnMap;                                           //^
        }
        if (Y > JelleWho.MaxCameraHeight)                                                       //Limit height
        {   
            Y = JelleWho.MaxCameraHeight;                                                       //^
        }
        else if (Y < JelleWho.MinCameraHeight)                                                  //^
        {
            Y = JelleWho.MinCameraHeight;                                                       //^
        }
        if (Z > JelleWho.MaxMoveHorizontalOnMap)                                                //Limit range South West(?)
        {
            Z = JelleWho.MaxMoveHorizontalOnMap;                                                //
        }
        else if (Z < -1 * JelleWho.MaxMoveHorizontalOnMap)                                      //
        {
            Z = -1 * JelleWho.MaxMoveHorizontalOnMap;                                           //
        }
        Camera.main.transform.position = new Vector3(X, Y, Z);                                  //Move the main camera



        
        if (Input.GetMouseButton(2))                                                            //If Scroll wheel is clicked
        {
            Vector3 destination = Camera.main.transform.eulerAngles;
            destination.x -= Input.GetAxis("Mouse Y") * JelleWho.RotateSpeed;                   //Get the mouse movement
            destination.y += Input.GetAxis("Mouse X") * JelleWho.RotateSpeed;                   //^
            if (destination.x < 5)                                                              //If the camera points to HIGH
            {
                destination.x = 5;                                                              //Set camera to max HIGH level
            }
            if (destination.x > 85)                                                             //If the camera points to LOW
            {
                destination.x = 85;                                                             //Set camera to max LOW level
            }
            Camera.main.transform.eulerAngles = destination;
        }
    }
}
