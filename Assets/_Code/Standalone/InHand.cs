﻿using UnityEngine;
using PublicCode;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class InHand : MonoBehaviour
{
    public Color InvalidPlacement = new Color(1f, 0f, 0f, 0.5f);                                //The color to apply when placement is invalid.
    private BoxCollider boxCollider;
    private int nonGroundLayerMask;
    public bool validPlacement = true;
    private Renderer[] renderers;
    public List<GameObject> RemoveObjectsHit = new List<GameObject>();
    
    public string ErrorMSG = "";
    /// <summary>
    /// Check if there is something colliding.</summary>
    /// <returns>Collision indicator</returns>
    public bool CheckCollision()                                                        //Check if there is something colliding.
    {
        // Get all the colliders within a box slightly smaller than our collider, except those in the ground layer.
        Collider[] Hits = Physics.OverlapBox(
            boxCollider.bounds.center,                                                          //From the center
            boxCollider.size / 2 - new Vector3(0.01f, 0.01f, 0.01f),                            //Size from the center to the edge
            transform.rotation,
            nonGroundLayerMask);

        if (Hits.Length == 1)                                                       //an ugly workaround cant find the reason why the boxcolider hits
            Hits = RemoveElementArray.removeAtIndexColliderArray(Hits, 0);          //still this is the only implementation that keeps the placement colision code intact                      

        ErrorMSG = "";                                                                          //reset the error message
        RemoveObjectsHit = new List<GameObject>();                                              //Make sure the list is emthy
        int InvalidObjectsHitAmount = Hits.Length;                                              //Get the total amount of objects in the way
        BuildType InhandType = BuildingData.GetInfo(gameObject.GetComponent<BuildingOption>().BuildingName).BuildType; //Get this Buildings name
        if(InhandType == BuildType.ReplaceWall || InhandType == BuildType.Wall || InhandType == BuildType.SpikedWall)             //If it's a wall
        {
            for (int i = 0; i < Hits.Length; i++)                                               //For each item hit
            {
                BuildType type = BuildingData.GetInfo(Hits[i].GetComponent<BuildingOption>().BuildingName).BuildType; //Get it's type
                if (type == BuildType.Wall || type == BuildType.SpikedWall)                     //If it's a (Spiked) wall
                    if (Hits[i].GetComponent<BuildingOption>().BuildingName != gameObject.GetComponent<BuildingOption>().BuildingName)    //If the object isn't this one
                        RemoveObjectsHit.Add(Hits[i].gameObject);                               //Store it in the list so we would be able to remove it upon placement
                else
                    i = Hits.Length;                                                            //Stop the loop code, we have found something invalid. No point in going on
            }
            InvalidObjectsHitAmount -= RemoveObjectsHit.Count;                                  //Calculate the invalid objects in the way of a placement
        }
        else if (InhandType == BuildType.FireBasket)                                            //If it's a fire basket
        {
            //Debug.DrawRay(gameObject.transform.position + transform.up, -transform.up, Color.red); //Just a debug line 
            if (!Physics.Raycast(gameObject.transform.position + transform.up, -transform.up, 1, 1 << LayerMask.NameToLayer("Building"))) //If there is no structure (and thus its not on a stone structure)
            {
                ErrorMSG = "That can only be build on Stone_Walls";
                InvalidObjectsHitAmount++;                                                      //Flag this place as invalid
            }
        }


        else if (InvalidObjectsHitAmount == 0)
        {
            if (InhandType == BuildType.Bridge)
            {
                ErrorMSG = "That can only be attached to a Stone_Gate";
                InvalidObjectsHitAmount++;                                                      //Flag this place as invalid by default
                RaycastHit hit;                                                                 //Create a output variable
                if (   Physics.Raycast(gameObject.transform.position + transform.up + transform.forward * System.Convert.ToByte(gameObject.GetComponent<Collider>().bounds.size.z) / 2,  transform.forward, out hit, 1, 1 << LayerMask.NameToLayer("Building")) 
                    || Physics.Raycast(gameObject.transform.position + transform.up - transform.forward * System.Convert.ToByte(gameObject.GetComponent<Collider>().bounds.size.z) / 2, -transform.forward, out hit, 1, 1 << LayerMask.NameToLayer("Building"))
                    || Physics.Raycast(gameObject.transform.position + transform.up + transform.right   * System.Convert.ToByte(gameObject.GetComponent<Collider>().bounds.size.z) / 2,  transform.right  , out hit, 1, 1 << LayerMask.NameToLayer("Building"))
                    || Physics.Raycast(gameObject.transform.position + transform.up - transform.right   * System.Convert.ToByte(gameObject.GetComponent<Collider>().bounds.size.z) / 2, -transform.right  , out hit, 1, 1 << LayerMask.NameToLayer("Building")))
                {
                    if (hit.collider.gameObject.GetComponent<BuildingOption>().BuildingName == "Stone_Gate")//If the object next ot it is a Stone_Gate
                    {
                        if (Mathf.Abs(hit.collider.transform.eulerAngles.y - 180) == 90)        //If the object is rotated 90 degrees either way
                        {
                            if (hit.collider.bounds.center.z == gameObject.GetComponent<Collider>().bounds.center.z)//If it does align with the Gate
                            {
                                InvalidObjectsHitAmount--;                                      //Flag this place as valid
                                if (hit.collider.bounds.center.x > gameObject.GetComponent<Collider>().bounds.center.x)
                                    gameObject.transform.rotation = Quaternion.Euler(0, 90, 0); //Rotate it to face the gate   
                                else
                                    gameObject.transform.rotation = Quaternion.Euler(0, 270, 0);//Rotate it to face the gate
                            }
                        }
                        else
                        {
                            if (hit.collider.bounds.center.x == gameObject.GetComponent<Collider>().bounds.center.x)//If it does align with the Gate
                            {
                                InvalidObjectsHitAmount--;                                      //Flag this place as valid
                                if (hit.collider.bounds.center.z > gameObject.GetComponent<Collider>().bounds.center.z)
                                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);  //Rotate it to face the gate   
                                else
                                    gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);//Rotate it to face the gate
                            }
                        }
                    }
                }
            }
        }
        return InvalidObjectsHitAmount > 0;                                                     //Return true if there's at least 1 collider.
    }

    private void Start()                                                                //Triggered on start
    {
        // Store references to things we need now rather than get or calulcate them each time they're needed.
        nonGroundLayerMask = ~((1 << 0) | (1 << 9));                               //A mask to ignore 0:Default and 9:Terrain
        boxCollider = gameObject.GetComponent<BoxCollider>();                      //
        renderers = gameObject.GetComponentsInChildren<Renderer>();                //
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
    private void OnDestroy()                                                            //If the Gamobject InHand gets destroyed (and it's either placed down or canceled)
    {
        //The renderer references are already gone, so get them again temporarily.
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        ShowAsNormal();
        renderers = null;
    }

}
