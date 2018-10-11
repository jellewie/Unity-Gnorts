//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using PublicCode;

public class Minimap : MonoBehaviour {
    public Camera FollowCam;                                                            //The camera to follow

    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);                    //A mathematical construct to represent the "floor" of the world

    void LateUpdate()                                                                   //Called after each frame after Update
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));                   //Cast a ray to the center of the camera
        float distance;                                                                         //The distance from the screen to the ground plane
        if (groundPlane.Raycast(ray, out distance))                                             //Get where the ray intersects the ground
        {
            Vector3 newPosition = Vector3.ClampMagnitude(ray.GetPoint(distance), 1.5f * JelleWho.MaxMoveHorizontalOnMap); //Clamp to a circle around the center of the world
            FollowCam.transform.position = new Vector3(newPosition.x, FollowCam.transform.position.y, newPosition.z); //Let the minimap follow where the camera is looking
        }
        FollowCam.transform.rotation = Quaternion.Euler(90f, Camera.main.transform.eulerAngles.y, 0f); //Also let the minimap follow the angle of the camera
        if (EnableZoom)                                                                         //If Zoom is enabled
        {
            float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");                       //Get the scrollwheel location
            if (ScrollWheelChange != 0)                                                         //If the scrollwheel has changed
            {
                float O = FollowCam.orthographicSize * ScrollWheelChange * JelleWho.MinimapScrollSpeed; //Set the amount to move
                FollowCam.orthographicSize = Mathf.Clamp(FollowCam.orthographicSize - O, 10, JelleWho.MaxCameraHeight * 1.5f); //Set and clamp minimap camera
            }
        }
    }
    private bool EnableZoom;                                                            //If Zoom is enabled
    public void Zoom(bool Enabled)                                                      //Enable or disable zoom
    {
        EnableZoom = Enabled;                                                                   //Set the right state
    }
}
