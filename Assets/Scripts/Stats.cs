using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stats : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public bool canMove;
	public bool canAttack;

	public Texture2D image;
	public string name;
	public int techLevel;

	public int spawnHeight;

	public int moveSpeed;
	public int attackRange;
	public int maximumHealth;
	public int currentHealth;
	public int damage;

	public List<Vector2> moveList;

	//turn stuff
	public bool hasMoved;
	public bool hasAttacked;


	// Update is called once per frame
	void Update () {
	
	}
}
