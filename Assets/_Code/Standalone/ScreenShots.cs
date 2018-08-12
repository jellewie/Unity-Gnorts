using UnityEngine;
using System.IO;
//Credits to @toddmoore https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
//
// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi


public class ScreenShots : MonoBehaviour
{
    public GameObject[] Objects0;                                                               //The gameobjects to tame screenshots from   name = ICON0_#
    public GameObject[] Objects1;                                                               //The gameobjects to tame screenshots from   name = ICON1_#
    public GameObject[] Objects2;                                                               //The gameobjects to tame screenshots from   name = ICON2_#
    public GameObject[] Objects3;                                                               //The gameobjects to tame screenshots from   name = ICON3_#
    public GameObject[] Objects4;                                                               //The gameobjects to tame screenshots from   name = ICON4_#
    public GameObject[] Objects5;                                                               //The gameobjects to tame screenshots from   name = ICON5_#
    public GameObject[][] Objects;
    byte TotalSubMenus = 6;                                                                     //Dont change this unless you know what you are doing. The code needs more Objects# manually being added
    public int Size = 256;                                                                      //The size of the imgage
    public string folder = "/_textures/Images/UI/Menu/1/sub";                                   //output folder path (Datapath + this)

    // configure with raw, jpg, png, or ppm (simple raw format)
    public enum Format {PNG};
    public Format format = Format.PNG;

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    private void Start()
    {
        folder = Application.dataPath + folder + "/";
        System.IO.Directory.CreateDirectory(folder);

        
        Objects = new GameObject[TotalSubMenus][];                                              //Make the Array (with arrrays of menu items) TotalSubMenus long

        Objects[0] = new GameObject[Objects0.Length];                                           //Make the Array with menu items ItemsInSubMenus long
        Objects[0] = new GameObject[Objects1.Length];                                           //^
        Objects[0] = new GameObject[Objects2.Length];                                           //^
        Objects[0] = new GameObject[Objects3.Length];                                           //^
        Objects[0] = new GameObject[Objects4.Length];                                           //^
        Objects[0] = new GameObject[Objects5.Length];                                           //^

        Objects[0] = Objects0;                                                                  //Fill the array
        Objects[1] = Objects1;                                                                  //^
        Objects[2] = Objects2;                                                                  //^
        Objects[3] = Objects3;                                                                  //^
        Objects[4] = Objects4;                                                                  //^
        Objects[5] = Objects5;                                                                  //^
    }
    public void _StartCreatingIcons()
    {
        //Destroy(InHand);
        //InHand = Instantiate(Prefab, new Vector3(0, -100, 0), Quaternion.identity);           //Create a building (we dont need to set it's position, will do later in this loop
        //InHand.transform.rotation = PreviousRotation;                                         //Restore the rotation
        //InHand.transform.SetParent(FolderBuildings);                                          //Sort the building in the right folder


        rect = new Rect(0, 0, Size, Size);                                                      // creates off-screen render texture that can rendered into
        renderTexture = new RenderTexture(Size, Size, 16);
        screenShot = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
        for (int j = 0; j < Objects.Length; j++)                                                 //For all given gameobjects
        {
            for (int i = 0; i < Objects[j].Length; i++)                                            //For all given gameobjects
            {
                Debug.Log(j + ":" + i);
                if (Objects[j][i])
                {
                    GameObject CurrentGameObject = Instantiate(Objects[j][i], new Vector3(0, 0, 0), Quaternion.identity);  //Place the GameObject
                    float SIZEX = Objects[j][i].GetComponent<BoxCollider>().size.x;
                    float SIZEY = Objects[j][i].GetComponent<BoxCollider>().size.y;
                    float BiggestSize = Objects[j][i].GetComponent<BoxCollider>().size.z;
                    if (SIZEX < SIZEY)
                    {
                        if (SIZEY > BiggestSize)
                        {
                            BiggestSize = SIZEY;
                        }
                    }
                    else
                    {
                        if (SIZEX > BiggestSize)
                        {
                            BiggestSize = SIZEX;
                        }
                    }
                    CurrentGameObject.transform.position = new Vector3(0, -SIZEY / 2, 0);           //Move them a bit down so there in the middle
                    Camera.main.transform.localPosition = new Vector3(0, 0, -BiggestSize);          //Change the camera zoom factor
                    TakeScreenShot(folder + "ICON" + j + "_" + i + "." + format.ToString().ToLower());
                    CurrentGameObject.SetActive(false);                                             //Cleanup - Hide GameObject 
                }
                else
                {
                    Debug.Log("No object found at; " + j + ":" + i);
                }
            }
        }




        Destroy(renderTexture);                                                                 //Cleanup - Remove the renderTexture again
        renderTexture = null;                                                                   //Cleanup - Just in case
        screenShot = null;                                                                      //Cleanup - Just in case
    }
    void TakeScreenShot(string filename)
    {
        Camera.main.targetTexture = renderTexture;                      //Set the renderTexture output reffrence to the camera
        Camera.main.Render();                                           //manually force a render

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);

        // reset active camera texture and render texture
        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        // pull in our file header/data bytes for the specified image format (has to be done from main thread)
        byte[] fileHeader = null;
        byte[] fileData = null;
        fileData = screenShot.EncodeToPNG();

        // create new thread to save the image to file (only operation that can be done in background)
        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            var f = System.IO.File.Create(filename);
            if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
            f.Write(fileData, 0, fileData.Length);
            f.Close();
            //Debug.Log(string.Format("Wrote screenshot {0} of size {1} to {2}", filename, fileData.Length, folder));
        }).Start();
    }
}