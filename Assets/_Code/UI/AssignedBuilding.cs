using UnityEngine;

public class AssignedBuilding : MonoBehaviour
{
    public GameObject Building;
    public int offsetX=9;
    public int offsetZ=12;
    
    public void MoveCameraToBulding()
    {
        var buildingPos = Building.transform.position;
        var camPos = Camera.main.transform.position;
        camPos = new Vector3(buildingPos.x - offsetX, 16, buildingPos.z - offsetZ);
        Camera.main.transform.position = camPos;
    }
}
