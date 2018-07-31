using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PublicCode;

public class UserInput : MonoBehaviour{
    public Player _player;                                              //Reffrence to the player
    public GameObject block;

    private void Start(){                                               //On start
        _player = transform.root.GetComponent<Player>();                    //Fill the player reference
    }

    private void Update(){                                              //Update before frame
        if (_player.human){                                                 //If the input is the User
            MoveCamera();                                                   //Check if we need to move the camera
            RotateCamera();                                                 //Check if we need to rotate the camera
        }
    }

    private void MoveCamera(){
        float xpos = Input.mousePosition.x;                                 //Save mouse position
        float ypos = Input.mousePosition.y;                                 //^
        Vector3 movement = new Vector3(0, 0, 0);

        if (Input.GetMouseButton(2)){                                       //detect rotation amount
            movement.x -= Input.GetAxis("Mouse X") * ResourceManager.MoveSpeedMouse;
            movement.z -= Input.GetAxis("Mouse Y") * ResourceManager.MoveSpeedMouse;
        } else {
            //Edge scroll
            if (xpos >= 0 && xpos < ResourceManager.MoveIfThisCloseToTheSides){ //horizontal camera movement
                movement.x -= ResourceManager.MoveEdgeScrollSpeed;          //Scroll a bit
            } else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.MoveIfThisCloseToTheSides){
                movement.x += ResourceManager.MoveEdgeScrollSpeed;          //Scroll a bit
            }
            if (ypos >= 0 && ypos < ResourceManager.MoveIfThisCloseToTheSides){ //vertical camera movement
                movement.z -= ResourceManager.MoveEdgeScrollSpeed;          //Scroll a bit
            } else if (ypos <= Screen.height && ypos > Screen.height - ResourceManager.MoveIfThisCloseToTheSides){
                movement.z += ResourceManager.MoveEdgeScrollSpeed;          //Scroll a bit
            }
        }
        movement = Camera.main.transform.TransformDirection(movement);      //make sure movement is in the direction the camera is pointing (Y is overwritten)
        movement.y = ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel"); //away from ground movement




        //TEMP TRYING TO MAKE SCROLL ZOOM BETER
        float PosX = Camera.main.transform.rotation.x;          //Get up and down
        float PosY = Camera.main.transform.rotation.y;          //Get left to right (Scale -1 to 1)

        float Xo = Camera.main.transform.position.x;
        float Yo = Camera.main.transform.position.y;
        float Zo = Camera.main.transform.position.z;

        Debug.Log("A" + PosX + " B" + PosY);
        int R = 5;
        float X = R * Mathf.Sin(PosY * Mathf.PI) * Mathf.Cos(PosX * Mathf.PI);
        float Y = R * Mathf.Sin(PosY * Mathf.PI) * Mathf.Sin(PosX * Mathf.PI);
        float Z = R * Mathf.Cos(PosY * Mathf.PI);
        //Debug.Log("X" + X + " Y" + Y + " Z" + Z);
        block.transform.localPosition = new Vector3(Xo + X, Yo + Y, Zo + Z);





        if (PosX >= 0){                                         //If we are goinging to the right
            //Debug.Log("R" + PosX);
        } else {
            //Debug.Log("L" + PosY);
        }
        









        Vector3 origin = Camera.main.transform.position;                    //calculate desired camera position based on received input
        Vector3 destination = origin;

        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        //limit away from gorudn movement to be between a minimum and maximum distance
        if (destination.y > ResourceManager.MaxCameraHeight){
            destination.y = ResourceManager.MaxCameraHeight;
        } else if (destination.y < ResourceManager.MinCameraHeight) {
            destination.y = ResourceManager.MinCameraHeight;
        }

        //if a change in position is detected perform the necessary update to the camera position
        if (destination != origin){
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
        }
    }

    private void RotateCamera(){
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;
        if (Input.GetMouseButton(1)){                               //detect rotation amount
            destination.x -= Input.GetAxis("Mouse Y") * ResourceManager.RotateAmount;
            destination.y += Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
        }
        if (destination != origin){                                 //if a change in the rotation is detected perform the necessary update
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
        }
    }
}
