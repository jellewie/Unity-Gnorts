using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicCode {
    public static class JelleWho {
        public static float HeighSpeedIncrease = 0.05f;                                         //The height has X of speed increase per block
        public static int MoveIfThisCloseToTheSides = 25;                                       //If the mouse is this close to the edge
        public static float MoveEdgeScrollSpeed = 1.5f;                                         //The speed of the screen when moving with the screen edges
        public static float MoveSpeedMouse = 0.5f;                                              //The speed of dragging the map with the mouse
        public static float MoveSpeedKeyboard = 0.7f;                                           //The speed of dragging the map with the keyboard
        public static int RotateSpeedMouse = 5;                                                 //The speed of rotating with the scroll wheel
        public static int RotateSpeedKeyboard = 1;                                              //The speed of the rotation when keyboard is used for rotation
        public static int MinCameraHeight = 1;                                                  //The min height the camera needs to be
        public static int MaxCameraHeight = 64;                                                 //The max heigh the camera can go
        public static int MaxMoveHorizontalOnMap = 128;                                         //The max distance the camera can be moved away from the center
        public static int ZoomScrollWheelSpeed = 25;                                            //The speed of the ScrollWheel when zooming in and out
    
        public static int BoolSettingsDefault = 0x01;                                           //Set the default value of the Bool Settings
        public static int BoolSettingsLength = 1;                                               //How many settings we are storing

        /*
            Llayout of playerPrefs 'BoolSettings':
            Bit 1 = 0x02 = EdgeScroll (on/off)
            Bit 2 = 0x04 = 

            Read
                if ((PlayerPrefs.GetInt("BoolSettings",JelleWho.BoolSettingsDefault) & 0x02) == 0x02){}
            Set
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) | 0x02));
            Clear
                 PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & ~0x02));
            Toggle
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) ^ 0x02));
        */




        //public static bool EdgeScroll { get { if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & 0x02) == 0x02) return true; else return false; } }
    }
}