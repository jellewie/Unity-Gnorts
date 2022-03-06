using UnityEngine;

public class AssignedBuilding : MonoBehaviour
{
    public GameObject Building;
    public int offsetX;
    public int offsetZ;

    public void Start()
    {
        offsetX = 10;
        offsetZ = 10;
    }

    public void MoveCameraToBulding()
    {
        var buildingPos = Building.transform.position;
        var camPos = Camera.main.transform.position;
        camPos = new Vector3(buildingPos.x - offsetX, 16, buildingPos.z - offsetZ);
        Camera.main.transform.position = camPos;
    }
}
