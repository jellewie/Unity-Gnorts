using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InHand : MonoBehaviour
{
    public Color InvalidPlacement = new Color(1f, 0f, 0f, 0.5f);                                //The color to apply when placement is invalid.
    private BoxCollider boxCollider;
    private int nonGroundLayerMask;
    public bool validPlacement = true;
    private Renderer[] renderers;

    /// <summary>
    /// Check if there is something colliding.</summary>
    /// <returns>Collision indicator</returns>
    public bool CheckCollision()                                                        //Check if there is something colliding.
    {
        // Get all the colliders within a box slightly smaller than our collider, except those in the ground layer.
        Collider[] Hits = Physics.OverlapBox(
            boxCollider.bounds.center,
            boxCollider.size / 2.05f,
            transform.rotation,
            nonGroundLayerMask);
        return Hits.Length > 0;                                                                 //Return true if there's at least 1 collider.
    }

    private void Start()                                                                //Triggered on start
    {
        // Store references to things we need now rather than get or calulcate them each time they're needed.
        nonGroundLayerMask = ~(1 << LayerMask.NameToLayer("Terrain") | 1);                      //A mask to ignore 0:Default and 9:Terrain
        boxCollider = gameObject.GetComponent<BoxCollider>();                                   //
        renderers = gameObject.GetComponentsInChildren<Renderer>();                             //
    }

    private void Update()                                                               //Triggered before frame update
    {
        bool colliding = CheckCollision();                                                      //Get if we are colliding with something
        if (colliding && validPlacement)                                                        //If the current state is valid placement but we're colliding then update the state.
        {
            validPlacement = false;                                                             //Flag we are colliding
            ShowAsInvalid();                                                                    //Show we are colliding
        }
        else if (!colliding && !validPlacement)                                                 //If the current state is invalid placement but we're not colliding then update the state.
        {
            validPlacement = true;                                                              //Flag we are fine
            ShowAsNormal();                                                                     //Show we are fine
        }
    }

    /// <summary>
    /// Apply shading to mark this location invalid.
    /// </summary>
    private void ShowAsInvalid()                                                        //Apply shading to mark this location invalid.
    {
        foreach (Renderer renderer in renderers)                                                //Change all the materials for all the renderers.
        {
            foreach (Material material in renderer.materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);    //Change the shader settings to "Transparent".
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); //^
                material.EnableKeyword("_ALPHABLEND_ON");                                       //^
                material.renderQueue = 3000;                                                    //^
                material.color = InvalidPlacement;                                              //Apply the configured color and opacity.
            }
        }
    }

    /// <summary>
    /// Restore the normal look of the object.
    /// </summary>
    private void ShowAsNormal()
    {
        foreach (Renderer renderer in renderers)                                                // Change all the materials for all the renderers.
        {
            foreach (Material material in renderer.materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);         //Change the shader settings to "non Transparent". So we are normal again
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);        //^
                material.DisableKeyword("_ALPHABLEND_ON");                                      //^
                material.renderQueue = -1;                                                      //^
                material.color = Color.white;                                                   //Clear the configured color and opacity, so we are normal again
            }
        }
    }

    /// <summary>
    /// Ensure the object looks normal again when this component is destroyed.
    /// </summary>
    private void OnDestroy()                                                            //If the Gamobject InHand gets destroyed
    {
            //The renderer references are already gone, so get them again temporarily.
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            ShowAsNormal();
            renderers = null;
    }

}
