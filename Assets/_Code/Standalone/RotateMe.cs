using UnityEngine;

public class RotateMe : MonoBehaviour {

    private Transform Object;
    public int Speed = 1;

    // Use this for initialization
    void Start () {
        Object = this.transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Object.transform.rotation = Quaternion.Euler(0, Object.transform.eulerAngles.y + Speed, 0);    //Rotate it 90 degrees clock wise
    }
}
