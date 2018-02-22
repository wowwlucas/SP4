﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EnemyStrategy{
	AGGRESSIVE,
	RANDOM,
	DEFENSIVE,
	STRATEGIC
};

public class AI : MonoBehaviour {
	[SerializeField]
	Color HoverColor;

	// Reference to the Node's Components
	private Renderer rend;

	[SerializeField]
	public EnemyStrategy Personality;

	Vector3 TargetMovement;

	private UnitVariables Stats;

	// AI Unit's ID
	private int ID;
	private static int AICount = 0;
	private float counter = 0.0f;

	Nodes currNode;
	Nodes nextNode;
	private Queue<Nodes> m_path = new Queue<Nodes>();

	Nodes EnemyTarget;

	private bool Path_Set;
	private bool isAttacking;

	// Reference to the UnitManager's instance
	private TurnManager turnManager;

	// Reference to GridSystem's Instance
	private GridSystem GridRef;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();

		//Gathers the name from the Unit Variable
		Stats = this.gameObject.GetComponent<UnitVariables> ();
		//Gathers the stats from the Json File
		Stats.Copy(UnitDatabase.Instance.FetchUnitByName (Stats.Name));

		// Set AI Unit's ID
		ID = AICount;
		++AICount;

		// Set the AI starting Position & occupies the current node
		currNode = GridSystem.Instance.GetNode(Random.Range(0,GridSystem.Instance.GetRows()),Random.Range(0,GridSystem.Instance.GetColumn()));
		//currNode = GridSystem.Instance.GetNode(0,0);
		currNode.SetOccupied (this.gameObject);
		this.transform.position = currNode.transform.position;
		TargetMovement = this.transform.position;

		nextNode = currNode;

		turnManager = TurnManager.Instance;
		GridRef = GridSystem.Instance;

		Path_Set = false;
		isAttacking = false;
	}

	// Run only when Mouse click on the unit
	void OnMouseDown()
	{
		// if unit can attack
		if (PlayerManager.Instance.GetAbleToAttack ())
		{
			// Get information from Units class
			Players selectedUnitClass = PlayerManager.Instance.GetSelectedUnit ().GetComponent<Players> ();

			if ((selectedUnitClass.GetCurrNode ().GetXIndex () + 1 == currNode.GetXIndex () 
				&& selectedUnitClass.GetCurrNode ().GetZIndex () == currNode.GetZIndex ()) ||
				(selectedUnitClass.GetCurrNode ().GetXIndex () - 1 == currNode.GetXIndex () 
					&& selectedUnitClass.GetCurrNode ().GetZIndex () ==  currNode.GetZIndex ()) ||
				(selectedUnitClass.GetCurrNode ().GetZIndex () + 1 ==  currNode.GetZIndex () 
					&& selectedUnitClass.GetCurrNode ().GetXIndex () == currNode.GetXIndex ()) ||
				(selectedUnitClass.GetCurrNode ().GetZIndex () - 1 ==  currNode.GetZIndex ()
					&& selectedUnitClass.GetCurrNode ().GetXIndex () == currNode.GetXIndex ()))
			{
				int damageDeal = PlayerManager.Instance.CalculateDamage (selectedUnitClass.gameObject, this.gameObject);

				Stats.HP -= damageDeal;
				if (Stats.HP <= 0)
				{
					Destroy (this);
					SceneManager.LoadScene ("SceneCleared");
				}
				PlayerManager.Instance.SetAbleToAttack (false);
				// End the turn of the player node
				selectedUnitClass.TurnEnd ();
				// Change unit back to no unit
				PlayerManager.Instance.ChangeUnit (null);

			}
		}
	}

	// Run only when Mouse cursor move into the node collision box
	// Visual feedback for player, show that he/she can clicked on these nodes
	void OnMouseEnter()
	{
		// if unit can attack
		if (PlayerManager.Instance.GetAbleToAttack ())
		{
			// Get information from Units class
			Players selectedUnitClass = PlayerManager.Instance.GetSelectedUnit ().GetComponent<Players> ();
			Nodes unitCurrNode = selectedUnitClass.GetCurrNode ();

			// Limits move range to one grid from the player current node
//			if(unitCurrNode.GetXIndex () + 1 == this.X && unitCurrNode.GetZIndex () == this.Z)
//					Debug.Log ("Right Node Occupied.");
//			if(unitCurrNode.GetXIndex () - 1 == this.X && unitCurrNode.GetZIndex () == this.Z)
//					Debug.Log ("Left Node Occupied.");
//			if(unitCurrNode.GetZIndex () + 1 == this.Z && unitCurrNode.GetXIndex () == this.X)
//					Debug.Log ("Up Node Occupied.");
//			if(unitCurrNode.GetZIndex () - 1 == this.Z && unitCurrNode.GetXIndex () == this.X)
//					Debug.Log ("Down Node Occupied.");

			if ((selectedUnitClass.GetCurrNode ().GetXIndex () + 1 == currNode.GetXIndex ()
			    && selectedUnitClass.GetCurrNode ().GetZIndex () == currNode.GetZIndex ()) ||
			    (selectedUnitClass.GetCurrNode ().GetXIndex () - 1 == currNode.GetXIndex ()
			    && selectedUnitClass.GetCurrNode ().GetZIndex () == currNode.GetZIndex ()) ||
			    (selectedUnitClass.GetCurrNode ().GetZIndex () + 1 == currNode.GetZIndex ()
			    && selectedUnitClass.GetCurrNode ().GetXIndex () == currNode.GetXIndex ()) ||
			    (selectedUnitClass.GetCurrNode ().GetZIndex () - 1 == currNode.GetZIndex ()
			    && selectedUnitClass.GetCurrNode ().GetXIndex () == currNode.GetXIndex ()))
			{
				// Change Visibility of Node to opague
				rend.material.color = HoverColor;
			}
		}
	}

	// Run only when Mouse cursor move out of the node collision box
	void OnMouseExit()
	{
		rend.material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<UnitVariables> ().Copy (Stats);
		this.gameObject.GetComponent<UnitVariables> ().UpdateHealthBar ();

		if (turnManager.GetCurrUnit() == null || turnManager.GetCurrUnit().GetID() != this.gameObject.GetComponent<AI>().GetID())
		{
			return;
		}

		if (Stats.AP != 0) {
			// print (this.gameObject.name + " " + Stats.AP);

			if ((this.transform.position - TargetMovement).magnitude < 0.1f) {
				currNode.SetSelectable (false);
				switch (Personality) {
				case(EnemyStrategy.AGGRESSIVE):
					AggressiveAction ();
					break;
				case(EnemyStrategy.DEFENSIVE):
					break;
				case(EnemyStrategy.RANDOM):
					RandomAction ();
					break;
				case(EnemyStrategy.STRATEGIC):
					break;
				}

				TargetMovement = currNode.transform.position;
			}
		} else {
			if ((this.transform.position - TargetMovement).magnitude < 0.1f) {
				this.transform.position = TargetMovement;
				currNode.SetSelectable (false);
			}
			TurnEnd ();
		}

		this.transform.position += (TargetMovement - this.transform.position).normalized * 0.05f;
	}

	void AggressiveAction()
	{
		if (EnemyTarget == null) {

			Nodes Temp = null;

			for (int x = 0; x < GridRef.GetRows (); ++x) {
				for (int z = 0; z < GridRef.GetColumn (); ++z) {
					if (GridRef.GetNode (x, z).GetOccupied () != null && GridRef.GetNode (x, z).GetOccupied ().tag == "PlayerUnit") {
						if (Temp != null) { // Temp Magnitude is more than current grid magnitude, change Temp altogether
							if ((Temp.GetOccupied ().transform.position - currNode.transform.position).magnitude > (GridRef.GetNode (x, z).GetOccupied ().transform.position - currNode.transform.position).magnitude) {
								Temp = GridRef.GetNode (x, z);
							}
						} else { // Sets the first reference of Temp
							Temp = GridRef.GetNode (x, z);
						}
					}
				}
			}

			EnemyTarget = Temp;
		} else {
			if (!Path_Set) {
				SetPath (currNode, EnemyTarget);
				m_path.Dequeue ();
				Path_Set = true;
			} else {
				if (!isAttacking) {
					Nodes TempMove = currNode;
					if (m_path.Count != 0) {
						TempMove = m_path.Dequeue ();
					}
					if (TempMove != EnemyTarget) {
						currNode = TempMove;
					} else {
						isAttacking = true;
						EnemyTarget.GetOccupied ().GetComponent<UnitVariables> ().HP--;
					}
				} else {
					print (Stats.AP + "vs" + EnemyTarget.GetOccupied ().GetComponent<UnitVariables> ().HP);
					EnemyTarget.GetOccupied ().GetComponent<UnitVariables> ().HP--;
				}
				this.Stats.AP--;
			}
		}
	}

	void DefensiveAction()
	{

	}

	void RandomAction()
	{
		
	}

	void StrategicAction()
	{
		
	}

	// Init for the start of each Unit's turn
	public void TurnStart()
	{
		// Reset nextNode to null
		nextNode = null;
		// Set Camera position to be the Unit's position
		Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
	}

	// The reset for the end of each Unit's turn
	public void TurnEnd()
	{
		// If nextNode is not null, update currNode to be nextNode
		// And null nextNode
		if (nextNode)
		{
			currNode = nextNode;
			nextNode = null;
		}
	}

	public void SetPath(Nodes m_start, Nodes m_end)
	{
		m_path.Enqueue (m_start);
		Nodes Temp = m_start;
		int AP_Ref = Stats.AP;

		while (!m_path.Contains (m_end) && AP_Ref > 0) {
			float UpMag = (Temp.transform.position - m_end.transform.position).magnitude,
			DownMag = (Temp.transform.position - m_end.transform.position).magnitude,
			LeftMag = (Temp.transform.position - m_end.transform.position).magnitude,
			RightMag = (Temp.transform.position - m_end.transform.position).magnitude,
			Closest;

			if (Temp.GetZIndex () != GridRef.GetColumn () - 1) {
				UpMag = (GridRef.GetNode(Temp.GetXIndex(), Temp.GetZIndex() + 1).transform.position - m_end.transform.position).magnitude;
			}
			if (Temp.GetZIndex () != 0) {
				DownMag = (GridRef.GetNode(Temp.GetXIndex(), Temp.GetZIndex() - 1).transform.position - m_end.transform.position).magnitude;
			}
			if (Temp.GetXIndex () != GridRef.GetRows () - 1) {
				LeftMag = (GridRef.GetNode(Temp.GetXIndex() + 1, Temp.GetZIndex()).transform.position - m_end.transform.position).magnitude;
			}
			if (Temp.GetXIndex () != 0) {
				RightMag = (GridRef.GetNode(Temp.GetXIndex() - 1, Temp.GetZIndex()).transform.position - m_end.transform.position).magnitude;
			}
			Closest = Mathf.Min (UpMag, DownMag, LeftMag, RightMag);

			if (Closest == UpMag) {
				Temp = GridRef.GetNode (Temp.GetXIndex (), Temp.GetZIndex () + 1);
			} else if (Closest == DownMag) {
				Temp = GridRef.GetNode (Temp.GetXIndex (), Temp.GetZIndex () - 1);
			} else if (Closest == LeftMag) {
				Temp = GridRef.GetNode (Temp.GetXIndex () + 1, Temp.GetZIndex ());
			} else if (Closest == RightMag) {
				Temp = GridRef.GetNode (Temp.GetXIndex () - 1, Temp.GetZIndex ());
			}

			Temp.SetSelectable (true);
			m_path.Enqueue (Temp);
			AP_Ref--;
		}
	}

	// Get & Set Unit's ID
	public int GetID() {return ID;}
	public void SetID(int _id) {ID = _id;}

	// Get Unit's Stats
	public UnitVariables GetStats() {return Stats;}
}
