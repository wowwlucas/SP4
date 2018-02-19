﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour
{
	// Serializable private variable defining Node
	[SerializeField]
	Color HoverColor;

	// X and Z Index
	private int X = 0;
	private int Z = 0;

	// HoverColor's alpha value
	private float HoverAlpha;

	// Reference to the Node's Components
	private Renderer rend;

	// Reference to the UnitManager's instance
	private PlayerManager playerManager;

	void Start ()
	{
		// Code Optimising - Get Renderer Component once only
		rend = GetComponent<Renderer> ();
		HoverAlpha = HoverColor.a;
		rend.material.color = HoverColor;
		// Code Optimising - Get UnitManager instance once only
		playerManager = PlayerManager.Instance;
	}

	// Run only when Mouse click onto the unit
	void OnMouseDown()
	{
		// Check if unit is available and if unit can move
		if (playerManager.GetSelectedUnit () != null && playerManager.GetAbleToMove())
		{
			// Get information from Units class
			Units selectedUnitClass = playerManager.GetSelectedUnit ().GetComponent<Units> ();
			Nodes unitCurrNode = selectedUnitClass.GetCurrNode ();

			// Limits move range to one grid from the player current node
			if ((unitCurrNode.GetXIndex () + 1 == this.X && unitCurrNode.GetZIndex () == this.Z) ||
				(unitCurrNode.GetXIndex () - 1 == this.X && unitCurrNode.GetZIndex () == this.Z) ||
				(unitCurrNode.GetZIndex () + 1 == this.Z && unitCurrNode.GetXIndex () == this.X) ||
				(unitCurrNode.GetZIndex () - 1 == this.Z && unitCurrNode.GetXIndex () == this.X))
			{
				Debug.Log ("Node Selected.");

				// Set selected unit's target to this node's position
				if (selectedUnitClass != null)
					selectedUnitClass.SetNextNode (this);

				// Change Visibility of Node back to translucent
				HoverColor.a = HoverAlpha;
				rend.material.color = HoverColor;
			}
		}
	}

	// Run only when Mouse cursor move into the node collision box
	// Visual feedback for player, show that he/she can clicked on these nodes
	void OnMouseEnter()
	{
		// Check if unit is available and if unit can move
		if (playerManager.GetSelectedUnit () != null && playerManager.GetAbleToMove ())
		{
			Units selectedUnitClass = playerManager.GetSelectedUnit ();
			Nodes unitCurrNode = selectedUnitClass.GetCurrNode ();

			// Limits move range to one grid from the player current node
			if ((unitCurrNode.GetXIndex () + 1 == this.X && unitCurrNode.GetZIndex () == this.Z) ||
				(unitCurrNode.GetXIndex () - 1 == this.X && unitCurrNode.GetZIndex () == this.Z) ||
				(unitCurrNode.GetZIndex () + 1 == this.Z && unitCurrNode.GetXIndex () == this.X) ||
				(unitCurrNode.GetZIndex () - 1 == this.Z && unitCurrNode.GetXIndex () == this.X))
			{
				// Change Visibility of Node to opague
				HoverColor.a = 1.0f;
				rend.material.color = HoverColor;
			}
		}
	}

	// Run only when Mouse cursor move out of the node collision box
	void OnMouseExit()
	{
		// Check if unit is available and if unit can move
		if (playerManager.GetSelectedUnit () != null && playerManager.GetAbleToMove())
		{
			// Change Visibility of Node back to translucent
			HoverColor.a = HoverAlpha;
			rend.material.color = HoverColor;
		}
	}

	// Set the grid's index
	public void SetIndex(int _x, int _z)
	{
		X = _x;
		Z = _z;
	}

	// Get grid's index
	public int GetXIndex() {return X;}
	public int GetZIndex() {return Z;}
}
