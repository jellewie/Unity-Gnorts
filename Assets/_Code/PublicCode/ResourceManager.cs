using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicCode {
    public static class JelleWho {
        public static float HeighSpeedIncrease = 0.01f;         //The height has X of speed increase per block

        public static float RotateSpeed = 5;                    //The speed of rotating with the scroll wheel
        public static int MoveSpeedMouse = 10;                  //Speed of dragging the map with the mouse
        public static int MoveIfThisCloseToTheSides = 10;       //If the mouse is this close to the edge
        public static int MoveEdgeScrollSpeed = 5;              //The speed of the screen when moving with the screen edges
        public static int MinCameraHeight = 1;                  //The min height the camera needs to be
        public static int MaxCameraHeight = 64;                 //The max heigh the camera can go
        public static int MaxMoveHorizontalOnMap = 128;         //The max distance the camera can be moved away from the center
        public static int ZoomScrollWheelSpeed = 2;             //The speed of the ScrollWheel when zooming in and out
    }
}