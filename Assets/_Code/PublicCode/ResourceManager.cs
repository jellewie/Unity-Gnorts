/*
    Written by JelleWho
 */
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
        public static int MaxCameraHeight = 128;                                                 //The max heigh the camera can go
        public static int MaxMoveHorizontalOnMap = 128;                                         //The max distance the camera can be moved away from the center
        public static int ZoomScrollWheelSpeed = 25;                                            //The speed of the ScrollWheel when zooming in and out
        public static float MinimapScrollSpeed = 1;                                             //The speed of the ScrollWheel when zooming in and out on the minimap
    
        public static int BoolSettingsDefault = 0x01;                                           //Set the default value of the Bool Settings
        public static int BoolSettingsLength = 4;                                               //How many settings we are storing

        /*
            Layout of playerPrefs 'BoolSettings' (1/0):
                Bit 1 = 0x01 = EdgeScroll (on/off)
                Bit 2 = 0x02 = 
                Bit 3 = 0x04 = 
                Bit 4 = 0x08 = 
                Bit 5 = 0x10 = 
                Bit 6 = 0x20 = 
                Bit 7 = 0x40 = 
                Bit 8 = 0x80 = 

            Read
                if ((PlayerPrefs.GetInt("BoolSettings",JelleWho.BoolSettingsDefault) & 0x02) == 0x02){}
            Set
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) | 0x02));
            Clear
                 PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & ~0x02));
            Toggle
                PlayerPrefs.SetInt("BoolSettings", (PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) ^ 0x02));
        */
    }
    public class SettingsBool                                                           //Create a new Class data type to store the bools in
    {
        //Please see the next url if you don't get this. it was really helpfull for me
        //https://unity3d.com/learn/tutorials/topics/scripting/classes
        public string Name;                                                                     //(To GET data) 1th part of the array is a string,        The name
        public bool Stat;                                                                       //(To GET data) 2nd part of the array is a is a bool,     The default state
        public string Desc;                                                                     //(To GET data) 3rd part of the array is a is a string,   The description (hoverover text)
        public SettingsBool(string name, bool default_status, string description)               //Create a way to add all data at once
        {
            this.Name = name;                                                                   //(To SET data) 1th part of the array is a string,        The name
            this.Stat = default_status;                                                         //(To SET data) 2nd part of the array is a is a bool,     The default state
            this.Desc = description;                                                            //(To SET data) 3rd part of the array is a is a string,   The description (hoverover text)
        }
    }
}

//public static bool EdgeScroll { get { if ((PlayerPrefs.GetInt("BoolSettings", JelleWho.BoolSettingsDefault) & 0x02) == 0x02) return true; else return false; } }
