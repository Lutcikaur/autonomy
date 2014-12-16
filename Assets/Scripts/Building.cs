using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public string name;
	public Texture2D image;
	public int owner;
	public bool notCapped;
	public int capMax;
	public int capCurrent;

	public int spawnHeight;
	public Vector2 location;

	public GameObject currentlyBuilding;
	public int buildProgress;

	// Update is called once per frame
	void Update () {
	
	}
}
