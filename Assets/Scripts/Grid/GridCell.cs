using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridCell : MonoBehaviour
{
    public Image normalImage;
    public Image hoverImage;
    public Image activeImage;
    public List<Sprite> normalImages;

    public bool Selected { get; set; }
    public int CellIndex { get; set; }
    public bool CellOccupied { get; set; }

    /// <summary>
    /// Initialize Cell as not selected and unoccupied
    /// </summary>
    private void Start()
    {
        Selected = false;
        CellOccupied = false;
    }

    /// <summary>
    /// Return cells usage state
    /// </summary>
    /// <returns></returns>
    public bool CanCellBeUsed()
    {
        return hoverImage.gameObject.activeSelf;
    }

    /// <summary>
    /// Activate the Cell, make it selected and occupied
    /// </summary>
    public void ActivateCell()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        CellOccupied = true;
    }

    /// <summary>
    /// Change the Image of the cell, (useful for designing various styles of patterns)
    /// </summary>
    public void SetImage()
    {
        normalImage.sprite = normalImages[0];
    }

    /// <summary>
    /// User is Hovering Over the Cell
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CellOccupied)
        {
            Selected = true;
            hoverImage.gameObject.SetActive(true);
        }
        else if(collision.GetComponent<BlockCell>() != null)
        {
            collision.GetComponent<BlockCell>().SetOccupied();
        }
    }

    /// <summary>
    /// User has dropped block over the cell
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
        
        if (!CellOccupied)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<BlockCell>() != null)
        {
            collision.GetComponent<BlockCell>().SetOccupied();
        }
    }

    /// <summary>
    /// Block has been removed from over the cell
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CellOccupied)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false); 
        }
        else if (collision.GetComponent<BlockCell>() != null)
        {
            collision.GetComponent<BlockCell>().UnsetOccupied();
        }
    }

    /// <summary>
    /// Place the Cell on the Board
    /// </summary>
    public void PlaceBlockOnBoard()
    {
        ActivateCell();
    }

    /// <summary>
    /// Deactivate the GridCell
    /// </summary>
    public void DeactivateCell()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(activeImage.gameObject.transform.DOScale(0.0f, 0.4f));
        seq.AppendCallback(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.gameObject.transform.localScale = Vector3.one;
        });
    }

    /// <summary>
    /// Clear the occupied cell by making it unselected and unoccupied.
    /// </summary>
    public void ClearOccupied()
    {
        Selected = false;
        CellOccupied = false;
    }
}