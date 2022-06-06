using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    /// <summary>
    /// Used to Update the Score and Best Score in GameOverHandler
    /// </summary>
    public static Action<int, int, bool> UpdateScores;

    /// <summary>
    /// Used to call GameOver related functions
    /// </summary>
    public static Action GameOver;

    /// <summary>
    /// AddScore Action -> Used to update current score and if condition satisfied Best Score
    /// </summary>
    public static Action<int> AddScores;

    /// <summary>
    /// Used to check if block is over a valid space and can be placed successfully.
    /// </summary>
    public static Action CheckIfBlockCanBePlaced;

    /// <summary>
    /// Used to revert back the block to start position in case of failure to place
    /// </summary>
    public static Action MoveBlockToInitialPosition;

    /// <summary>
    /// Requests New Blocks in case all the blocks have been exhausted.
    /// </summary>
    public static Action RequestNewBlock;

    /// <summary>
    /// Requests Shape to be set to inactive.
    /// </summary>
    public static Action SetBlockInactive;
}
