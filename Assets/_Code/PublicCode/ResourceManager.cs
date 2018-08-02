using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicCode {
    public static class JelleWho {
        public static float HeighSpeedIncrease = 0.05f;                                          //The height has X of speed increase per block
        public static float MoveSpeedKeyboard = 0.7f;
        public static int RotateSpeed = 5;                                                      //The speed of rotating with the scroll wheel
        public static int MoveSpeedMouse = 1;                                                   //Speed of dragging the map with the mouse
        public static int MoveIfThisCloseToTheSides = 25;                                       //If the mouse is this close to the edge
        public static float MoveEdgeScrollSpeed = 0.4f;                                         //The speed of the screen when moving with the screen edges
        public static int MinCameraHeight = 1;                                                  //The min height the camera needs to be
        public static int MaxCameraHeight = 64;                                                 //The max heigh the camera can go
        public static int MaxMoveHorizontalOnMap = 128;                                         //The max distance the camera can be moved away from the center
        public static int ZoomScrollWheelSpeed = 25;                                            //The speed of the ScrollWheel when zooming in and out
    }
}

