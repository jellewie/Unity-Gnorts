using UnityEngine;

//Please check the next url to see a basic implementation of the singleton pattern
//https://blog.studica.com/how-to-create-a-singleton-in-unity-3d
public class Manager : MonoBehaviour
{
    public static Manager instance = null;
    public bool isSelected = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isSelected)
            Camera.main.GetComponent<Outliner>().enabled = false;        // disable the outliner camera to stop the outlining
        else
            Camera.main.GetComponent<Outliner>().enabled = true;
    }
}
