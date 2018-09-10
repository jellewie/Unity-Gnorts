using UnityEngine;
using UnityEngine.UI;                   //Required when Using UI elements.

public class FPSDisplay : MonoBehaviour
{
    public Text FPSCounter;                                                //Text that shows FPS amount
    public InputField Input;
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void Start()
    {
        InvokeRepeating("CalculateFPS", 0, 0.1f);                                //Repeatedly update FPS count (refresh every 0.01s)
    }
    void CalculateFPS()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        FPSCounter.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }
    public void _SetTargetFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = int.Parse(Input.text);
        Debug.Log(Application.targetFrameRate);
    }
}