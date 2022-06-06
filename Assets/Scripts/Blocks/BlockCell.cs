using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCell : MonoBehaviour
{
    public Image occupiedImage;

    private void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivate the block and disable the collision to stop trigger events
    /// </summary>
    public void DeactivateBlock()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Aactivate the block and enable the collision to accept trigger events
    /// </summary>
    public void ActivateBlock()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Set occupied status of the block by Activating the occupied image
    /// </summary>
    public void SetOccupied() 
    {
        occupiedImage.gameObject.SetActive(true); 
    }

    /// <summary>
    /// Unset occupied status of the block by deactivating the occupied image
    /// </summary>
    public void UnsetOccupied() 
    { 
        occupiedImage.gameObject.SetActive(false); 
    }
}
