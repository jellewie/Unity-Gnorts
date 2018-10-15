using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InHand : MonoBehaviour
{
    /// <summary>
    /// The color to apply when placement is invalid.
    /// </summary>
    public Color InvalidPlacement = new Color(1f, 0f, 0f, 0.5f);

    private BoxCollider boxCollider;
    private int nonGroundLayerMask;
    public bool validPlacement = true;
    private Renderer[] renderers;

    /// <summary>
    /// Check if there is something colliding.
    /// </summary>
    /// <returns>Collision indicator</returns>
    public bool CheckCollision()
    {
        // Get all the colliders within a box slightly smaller than our collider, except those in the ground layer.
        Collider[] Hits = Physics.OverlapBox(
            boxCollider.bounds.center,
            boxCollider.size / 2.05f,
            transform.rotation,
            nonGroundLayerMask);
        // Return true if there's at least 1 collider.
        return Hits.Length > 0;
    }

    private void Start()
    {
        // Store references to things we need now rather than get or calulcate them each time they're needed.
        nonGroundLayerMask = ~(1 << LayerMask.NameToLayer("Terrain") | 1);
        boxCollider = gameObject.GetComponent<BoxCollider>();
        renderers = gameObject.GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        bool colliding = CheckCollision();
        // If the current state is valid placement but we're colliding then update the state.
        if (colliding && validPlacement)
        {
            validPlacement = false;
            ShowAsInvalid();
        }
        // If the current state is invalid placement but we're not colliding then update the state.
        else if (!colliding && !validPlacement)
        {
            validPlacement = true;
            ShowAsNormal();
        }
    }

    /// <summary>
    /// Apply shading to mark this location invalid.
    /// </summary>
    private void ShowAsInvalid()
    {
        // Change all the materials for all the renderers.
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // Change the shader settings to "Transparent".
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = 3000;
                // Apply the configured color and opacity.
                material.color = InvalidPlacement;
            }
        }
    }

    /// <summary>
    /// Restore the normal look of the object.
    /// </summary>
    private void ShowAsNormal()
    {
        // Change all the materials for all the renderers.
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.DisableKeyword("_ALPHABLEND_ON");
                material.renderQueue = -1;
                material.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Ensure the object looks normal again when this component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
            // The renderer references are already gone, so get them again temporarily.
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            ShowAsNormal();
            renderers = null;
    }

}
