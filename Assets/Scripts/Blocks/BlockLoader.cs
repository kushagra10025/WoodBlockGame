using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLoader : MonoBehaviour
{
    public List<BlockIdentifier> blockIdentifiers = new List<BlockIdentifier>();
    public List<Block> blocks = new List<Block>();

    private void Start()
    {
        DrawRandomBlocks();
    }

    /// <summary>
    /// Subscribe to Request New Shape Event
    /// </summary>
    private void OnEnable()
    {
        GameEvents.RequestNewBlock += RequestNewRandomBlock;
    }

    /// <summary>
    /// Unsubscribe to Request New Shape Event
    /// </summary>
    private void OnDisable()
    {
        GameEvents.RequestNewBlock -= RequestNewRandomBlock;
    }

    /// <summary>
    /// Request New Random Block 
    /// </summary>
    private void RequestNewRandomBlock()
    {
        /*foreach(Block block in blocks)
        {
            int blockIndex = Random.Range(0, blockIdentifiers.Count);
            block.RequestNewBlock(blockIdentifiers[blockIndex]);
        }*/
        DrawRandomBlocks();
    }

    /// <summary>
    /// Draw Random Blocks
    /// </summary>
    public void DrawRandomBlocks()
    {
        foreach (Block block in blocks)
        {
            int blockIndex = Random.Range(0, blockIdentifiers.Count);
            block.RequestNewBlock(blockIdentifiers[blockIndex]);
        }
    }

    /// <summary>
    /// Get the Currently Selected Block
    /// Block is selected if it is not on Start Position and is any cell is active
    /// </summary>
    /// <returns></returns>
    public Block GetCurrentSelectedBlock()
    {
        foreach(var block in blocks)
        {
            if(!block.IsOnStartPosition() && block.IsAnyOfBlockCellActive())
            {
                return block;
            }
        }

        Debug.LogError("No Block Selected!");
        return null;
    }
}
