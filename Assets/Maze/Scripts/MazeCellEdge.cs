﻿using UnityEngine;
using System.Collections;

public class MazeCellEdge : MonoBehaviour {
    public MazeCell cell, otherCell;
    public MazeDirection direction;

    public virtual void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.direction = direction;
        cell.SetEdge(direction, this);
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void OnPlayerEnter() { }
    public virtual void OnPlayerExit() { }

}
