using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicCode {
    public static class ResourceManager {
        public static float ScrollSpeed { get { return 25.0f; } }       // 
        public static float RotateSpeed { get { return 100.0f; } }      //
        public static float RotateAmount { get { return 10.0f; } }      //

        public static float MoveSpeedMouse { get { return 15.0f; } }    //
        public static int MoveIfThisCloseToTheSides { get { return 5; } }    //If the mouse is this close to the edge
        public static float MoveEdgeScrollSpeed { get { return 15.0f; } }    //

        public static float MinCameraHeight { get { return 1.0f; } }   //
        public static float MaxCameraHeight { get { return 100.0f; } }   //
    }
}