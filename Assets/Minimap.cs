using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
    public GameObject FollowCamera;

	void LateUpdate () {
        FollowCamera.transform.position = new Vector3(Camera.main.transform.position.x, 100, Camera.main.transform.position.z);
    }
}
