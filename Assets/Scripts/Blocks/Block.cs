using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject BlockCellPrefab;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 1400f);

    [HideInInspector] public BlockIdentifier currentBlockIdentifier;

    public int TotalCellCount { get; set; }

    private List<GameObject> _currentBlock = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _rectTransform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _blockActive = true;

    /// <summary>
    /// Subscribe to MoveBlockToInitialPosition and SetBlockInactive events
    /// </summary>
    private void OnEnable()
    {
        GameEvents.MoveBlockToInitialPosition += MoveBlockToStartPosition;
        GameEvents.SetBlockInactive += SetBlockInactive;
    }

    /// <summary>
    /// Unsubscribe to MoveBlockToInitialPosition and SetBlockInactive events
    /// </summary>
    private void OnDisable()
    {
        GameEvents.MoveBlockToInitialPosition -= MoveBlockToStartPosition;
        GameEvents.SetBlockInactive -= SetBlockInactive;
    }

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _shapeStartScale = _rectTransform.localScale;
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = _rectTransform.localPosition;
        _blockActive = true;
    }

    /// <summary>
    /// Check if block is on start position
    /// </summary>
    /// <returns></returns>
    public bool IsOnStartPosition()
    {
        return _rectTransform.localPosition == _startPosition;
    }

    /// <summary>
    /// Check if any cell of block is active
    /// </summary>
    /// <returns></returns>
    public bool IsAnyOfBlockCellActive()
    {
        foreach(var cell in _currentBlock)
        {
            if (cell.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Deactivate Block 
    /// </summary>
    public void DeactivateBlock()
    {
        if (_blockActive)
        {
            foreach (var cell in _currentBlock)
            {
                cell?.GetComponent<BlockCell>().DeactivateBlock();
            }
        }
        _blockActive = false;
    }

    /// <summary>
    /// Set the block to be inactive so that it can't be moved
    /// </summary>
    public void SetBlockInactive()
    {
        if(!IsOnStartPosition() && IsAnyOfBlockCellActive())
        {
            foreach(var blockCell in _currentBlock)
            {
                blockCell.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Activate Block 
    /// </summary>
    public void ActivateBlock()
    {
        if (!_blockActive)
        {
            foreach (var cell in _currentBlock)
            {
                cell?.GetComponent<BlockCell>().ActivateBlock();
            }
        }
        _blockActive = true;
    }

    /// <summary>
    /// Request New Block and place it on the start position
    /// </summary>
    /// <param name="blockIdentifier"></param>
    public void RequestNewBlock(BlockIdentifier blockIdentifier)
    {
        _rectTransform.localPosition = _startPosition;
        CreateBlock(blockIdentifier);
    }

    /// <summary>
    /// Create the block from blockIdentifier Scriptable Object
    /// </summary>
    /// <param name="blockIdentifier"></param>
    private void CreateBlock(BlockIdentifier blockIdentifier)
    {
        currentBlockIdentifier = blockIdentifier;
        TotalCellCount = GetNumberOfCellsInBlock(blockIdentifier);
        while(_currentBlock.Count <= TotalCellCount)
        {
            _currentBlock.Add(GameObject.Instantiate(BlockCellPrefab, transform));
        }

        foreach(var cell in _currentBlock)
        {
            cell.gameObject.transform.position = Vector3.zero;
            cell.gameObject.SetActive(false);
        }

        var squareRect = BlockCellPrefab.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndex = 0;
        for(int row = 0; row < blockIdentifier.rows; row++)
        {
            for(var col = 0; col < blockIdentifier.columns; col++)
            {
                if (blockIdentifier.board[row].column[col])
                {
                    _currentBlock[currentIndex].SetActive(true);
                    _currentBlock[currentIndex].GetComponent<RectTransform>().localPosition = new Vector2(GetXPositionForCellBlock(blockIdentifier, col, moveDistance), GetYPositionForCellBlock(blockIdentifier, row, moveDistance));
                    currentIndex++;
                }
            }
        }
    }

    /// <summary>
    /// Get All X Cell Positions of the block from the Block Identifier
    /// </summary>
    /// <param name="shapeData"></param>
    /// <param name="column"></param>
    /// <param name="moveDistance"></param>
    /// <returns></returns>
    private float GetXPositionForCellBlock(BlockIdentifier shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (shapeData.columns > 1)
        {
            float startXPos;
            if (shapeData.columns % 2 != 0)
                startXPos = (shapeData.columns / 2) * moveDistance.x * -1;
            else
                startXPos = ((shapeData.columns / 2) - 1) * moveDistance.x * -1 - moveDistance.x / 2;
            shiftOnX = startXPos + column * moveDistance.x;

        }
        return shiftOnX;
    }

    /// <summary>
    /// Get All Y Cell Positions of the block from the Block Identifier
    /// </summary>
    /// <param name="shapeData"></param>
    /// <param name="row"></param>
    /// <param name="moveDistance"></param>
    /// <returns></returns>
    private float GetYPositionForCellBlock(BlockIdentifier shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (shapeData.rows > 1)
        {
            float startYPos;
            if (shapeData.rows % 2 != 0)
                startYPos = (shapeData.rows / 2) * moveDistance.y;
            else
                startYPos = ((shapeData.rows / 2) - 1) * moveDistance.y + moveDistance.y / 2;
            shiftOnY = startYPos - row * moveDistance.y;
        }
        return shiftOnY;
    }

    /// <summary>
    /// Get Cell Counts in Block
    /// </summary>
    /// <param name="blockIdentifier"></param>
    /// <returns></returns>
    private int GetNumberOfCellsInBlock(BlockIdentifier blockIdentifier)
    {
        int num = 0;

        foreach(var row in blockIdentifier.board)
        {
            foreach(var col in row.column)
            {
                if (col)
                {
                    num++;
                }
            }
        }

        return num;
    }

    /// <summary>
    /// Move the block to start position
    /// </summary>
    private void MoveBlockToStartPosition()
    {
        _rectTransform.transform.localPosition = _startPosition;
    }

    /// <summary>
    /// Event OnDrag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.zero;
        _rectTransform.pivot = Vector2.zero;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _rectTransform.localPosition = pos + offset;
    }

    /// <summary>
    /// Event OnBeginDrag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.localScale = shapeSelectedScale;
    }

    /// <summary>
    /// Event OnEndDrag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        _rectTransform.localScale = _shapeStartScale;
        GameEvents.CheckIfBlockCanBePlaced();
    }

}