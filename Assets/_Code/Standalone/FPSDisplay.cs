using UnityEngine;
using UnityEngine.UI;                   //Required when Using UI elements.

public class FPSDisplay : MonoBehaviour
{
    public Text FPSCounter;                                                //Text that shows FPS amount
    float deltaTime = 0.0f;

    void Update()
    {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void Start()
    {
        if (PlayerPrefs.GetInt("FPSCounter", 0) == 1)                       //If FPS it set to show
        {
            InvokeRepeating("FPS", 0, 0.1f);                                //Repeatedly show FPS count (refresh every 0.01s)
        }
    }
    void FPS()
    {
    float msec = deltaTime * 1000.0f;
    float fps = 1.0f / deltaTime;
    FPSCounter.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }
}