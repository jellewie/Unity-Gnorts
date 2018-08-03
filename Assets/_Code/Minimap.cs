using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
    public Transform FollowCamera;

	void LateUpdate () {
        FollowCamera.position = new Vector3(Camera.main.transform.position.x, 100, Camera.main.transform.position.z);
        FollowCamera.rotation = Quaternion.Euler(90f, Camera.main.transform.eulerAngles.y, 0f);
    }
}
