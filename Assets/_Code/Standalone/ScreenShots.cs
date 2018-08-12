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
    public GameObject[] ObjectsToMakeIconsOff;          //The gameobjects to tame screenshots from
    public int Size = 256;                              //The size of the imgage
    public string folder = "/_textures/Images/UI/Menu/1/sub";               //output folder path (Datapath + this)



    // optional game object to hide during screenshots (usually your scene canvas hud)
    public GameObject hideGameObject;

    // optimize for many screenshots will not destroy any objects so future screenshots will be fast
    public bool optimizeForManyScreenshots = true;

    // configure with raw, jpg, png, or ppm (simple raw format)
    public enum Format { RAW, JPG, PNG, PPM };
    public Format format = Format.PNG;

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    private void Start()
    {
        folder = Application.dataPath + folder + "/";
        System.IO.Directory.CreateDirectory(folder);
    }
    public void _StartCreatingIcons()
    {
        for (int i = 0; i < ObjectsToMakeIconsOff.Length; i++)          //For all given gameobjects
        {
            ObjectsToMakeIconsOff[i].SetActive(false);                  //Hide gameobject 
        }
        rect = new Rect(0, 0, Size, Size);                              // creates off-screen render texture that can rendered into
        renderTexture = new RenderTexture(Size, Size, 16);
        screenShot = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
        for (int i = 0; i < ObjectsToMakeIconsOff.Length; i++)          //For all given gameobjects
        {
            if (i > 0)                                                  //If this isnt't the first gameobject
            {
                ObjectsToMakeIconsOff[i - 1].SetActive(false);          //Hide previous gameobject 
            }
            float SIZEX = ObjectsToMakeIconsOff[i].GetComponent<BoxCollider>().size.x;
            float SIZEY = ObjectsToMakeIconsOff[i].GetComponent<BoxCollider>().size.y;
            float BiggestSize = ObjectsToMakeIconsOff[i].GetComponent<BoxCollider>().size.z;
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

            ObjectsToMakeIconsOff[i].transform.position = new Vector3(0, -SIZEY / 2, 0);    //Move them a bit down so there in the middle

            //Debug.Log(-BiggestSize + "  " + Camera.main.transform.position);
            Camera.main.transform.localPosition = new Vector3(0, 0, -BiggestSize);      //Change the camera zoom factor


            ObjectsToMakeIconsOff[i].SetActive(true);                   //Show gameobject 
            TakeScreenShot(folder + "ICON" + i + "." + format.ToString().ToLower());
        }
        Destroy(renderTexture);                                         //Cleanup - Remove the renderTexture again
        renderTexture = null;                                           //Cleanup - Just in case
        screenShot = null;                                              //Cleanup - Just in case
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
            Debug.Log(string.Format("Wrote screenshot {0} of size {1} to {2}", filename, fileData.Length, folder));
        }).Start();
    }
}