using UnityEditor;
using UnityEngine;

public class FitBoxCollider
{
    [MenuItem("Tools/BoxCollider/Fit to Children")]
    private static void FitToChildren()
    {
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                bool hasBounds = false;
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

                foreach (Renderer childRenderer in gameObject.GetComponentsInChildren<Renderer>())
                {
                    if (hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
                collider.center = bounds.center - gameObject.transform.position;
                collider.size = bounds.size;
            }
        }
    }
}
