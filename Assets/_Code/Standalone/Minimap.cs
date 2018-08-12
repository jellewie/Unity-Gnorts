//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using PublicCode;

public class Minimap : MonoBehaviour {
    public Transform FollowCamera;
    public Camera FollowCam;
    void LateUpdate() {
        FollowCamera.position = new Vector3(Camera.main.transform.position.x, 100, Camera.main.transform.position.z);   //Let the minimap follow the camera
        FollowCamera.rotation = Quaternion.Euler(90f, Camera.main.transform.eulerAngles.y, 0f); //Also follow the angle
        if (EnableZoom)
        {
            float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");                       //Get the scrollwheel location
            if (ScrollWheelChange != 0)                                                         //If the scrollwheel has changed
            {
                float O = FollowCam.orthographicSize * ScrollWheelChange * JelleWho.MinimapScrollSpeed; //Set the amount to move
                FollowCam.orthographicSize = Mathf.Clamp(FollowCam.orthographicSize - O, 10, JelleWho.MaxCameraHeight * 1.5f); //Set and clamp minimap camera
            }
        }
    }
    private bool EnableZoom;                                                                    //If Zoom is enabled
    public void Zoom(bool Enabled)                                                              //Enable or disable zoom
    {
        EnableZoom = Enabled;                                                                   //Set the right state
    }
}
