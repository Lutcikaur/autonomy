using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	// Use this for initialization
	public HexWorld hexWorld;
	public TerrainRaycaster terrainCaster;
	public List<List<GameObject>> playerObjects = new List<List<GameObject>>{};
	//public GameObject Cylinder;
	public int me = -1;
	public List<List<string>> depotList = new List<List<string>>{};
	public static string server = null;
	public int turn = 0;

	//map bounds inclusive
	public int xLowerBound = 80;
	public int xUpperBound = 99;
	public int yLowerBound = 0;
	public int yUpperBound = 25;


	public int ToggleTemp=0;
	//public bool handToggleTempBool=false;
	public bool[] EnlargeBool = new bool[7]; //Bool[0] = Hand Enlarging, Bool[1-6] = Depot Enlarging

	public int handToggleTemp=0;
	public bool handToggleTempBool=false;

	public Texture artex;
	Texture2D img;
	Texture2D depotBack;


	void Start () {
		depotBack=Resources.Load("depotWindow") as Texture2D;
		img = Resources.Load("Deck_2") as Texture2D;

		for(int i=0; i<7; i++){
			EnlargeBool[i]=false;
		}

		server = Menu.server;
		Debug.Log("Here");
		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:

			break;
		case NetworkPeerType.Client:
			break;
		case NetworkPeerType.Server:
			//We have to null then build up the connection list to clients first.
			networkView.RPC("Clear",RPCMode.OthersBuffered);
			for(int i = 0; i < Menu.connectionList.Count; i++){
				networkView.RPC("AddConnectionList",RPCMode.OthersBuffered,Menu.connectionList[i].guid,Menu.connectionList[i].username);
			}
			networkView.RPC("Initialize",RPCMode.OthersBuffered);
			//some test stuff.
			for(int i = 0; i < Menu.connectionList.Count; i++){
				playerObjects.Add(new List<GameObject>());
				depotList.Add (new List<string>());
				for(int j = 0; j<5; j++){
					string name = "Vtol";
					string guid = Menu.connectionList[i].guid;
					Vector3 location = new Vector3(i+86,1,j+9);
					networkView.RPC("SpawnObject",RPCMode.All,i,guid,name,location);
				}
			}
			break;
		}

	}

	public Vector2[] getNeighbor(int x, int y)
	{
		Vector2[] neighborList = new Vector2[6];
		int numberOfNeighbors = 0;
		bool odd = false;

		if(x > xUpperBound || x < xLowerBound || y > yUpperBound || y < yUpperBound){
			return new Vector2[0];
		}

		if ((y % 2) > 0) {
			odd = true;
		}

		//adds x neighbors
		if(hexWorld.hexWorldData[x-1,y].inBounds){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y].x,hexWorld.hexWorldData[x-1,y].y);
			numberOfNeighbors++;
		}

		if(hexWorld.hexWorldData[x+1,y].inBounds){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y].x,hexWorld.hexWorldData[x+1,y].y);
			numberOfNeighbors++;
		}
		
		//adds y neighbors
		if(hexWorld.hexWorldData[x,y-1].inBounds){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y-1].x,hexWorld.hexWorldData[x,y-1].y);
			numberOfNeighbors++;
		}

		if(hexWorld.hexWorldData[x,y+1].inBounds){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y+1].x,hexWorld.hexWorldData[x,y+1].y);
			numberOfNeighbors++;
		}
		
		//gets y right neighbors for odd
		if(odd == true) {
			Debug.Log ("If");
			if(hexWorld.hexWorldData[x+1,y-1].inBounds){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y-1].x,hexWorld.hexWorldData[x+1,y-1].y);
				numberOfNeighbors++;
			}
			
			if(hexWorld.hexWorldData[x+1,y+1].inBounds){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y+1].x,hexWorld.hexWorldData[x+1,y+1].y);
				numberOfNeighbors++;
			}
		} else {
			//Debug.Log ("Else");
			//gets y left neighbors for even
			if(hexWorld.hexWorldData[x-1,y+1].inBounds){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y+1].x,hexWorld.hexWorldData[x-1,y+1].y);
				numberOfNeighbors++;
			}

			if(hexWorld.hexWorldData[x-1,y-1].inBounds){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y-1].x,hexWorld.hexWorldData[x-1,y-1].y);
				numberOfNeighbors++;
			}
		}
		
		for(int i=0; i<numberOfNeighbors; i++)
		{
			hexWorld.SetHexUVs(neighborList[i],2);
		}
		return neighborList;
	}

	/*
	deck RandomizeDeck(deck d){
		int i,j;
		int iMin;
		var r=new Random();
		for (i=0; i<deckSize; i++){
			d[i].randomVal=r.Next(1000);
		}
		deck TempDeck=new deck;

		// a[0] to a[n-1] is the array to sort 

		// advance the position through the entire array 
		//   (could do j < n-1 because single element is also min element) 
		for (j = 0; j < deckSize; j++) {
			// find the min element in the unsorted a[j .. n-1] 
			
			// assume the min is the first element 
			iMin = j;
			// test against elements after j to find the smallest 
			for ( i = j+1; i <= deckSize; i++) {
				// if this element is less, then it is the new minimum   
				if (d[i].randomVal < d[iMin].randomVal) {
					// found new minimum; remember its index 
					iMin = i;
				}
			}
			
			if(iMin != j) {
				//swaps d[j]& d[iMin];
				TempDeck[0].index=d[j].index;
				TempDeck[0].randomVal=d[j].randomVal;
				d[j].index=d[iMin].index;
				d[j].randomVal=d[iMin].randomVal;
				d[iMin].index=TempDeck[0];
				d[iMin].randomVal=TempDeck[0];
			}
			
		}



		return(d);
	}

	*/

	//pathfinds from the starting location to the ending location 
	//IF IT RETURNS NULL, THERE IS NO PATH
	public List<Vector2> pathfind(Vector2 start, Vector2 end){

		//openList is for yet checked hexes that may work
		List<PFList> openList = new List<PFList>{};
		//closedlist is for already checked hexes
		List<PFList> closedList = new List<PFList>{};

		//stick the starting hex in the openlist to start
		openList.Add(new PFList(start,0,0,null));

		bool done = false;
		bool fail = false;

		//dont stop till we are done
		while(done == false){
			//count runs the whole list
			int count = openList.Count;
			//temp holds the best FScore to choose the best openList hex,  Low is better
			int temp = 100000000;
			//the chosen hex in openlist.  If it stays at -1 there is no path and it kicks out
			int choose = -1;
			PFList currentHex = null;

			//finds the best FScore in openList
			for(int i=0; i<count; i++){
				if (openList[i].getF() < temp){
					temp = openList[i].getF();
					choose = i;
				}
			}

			if(choose != -1){
				currentHex = openList[choose];
			}
			else{
				done = true;
				fail = true;
			}

			//hex is checked. move from open to closed
			openList.RemoveAt(choose);

			if(currentHex != null && currentHex.getHex().x == end.x && currentHex.getHex().y == end.y){

				done = true;
			}

			closedList.Add(currentHex);

			//get neighbors
			Vector2[] neighborList = new Vector2[6];


			//PLZ WORK O HELP ME GAWD
			neighborList = getNeighbor((int)currentHex.getHex().x, (int)currentHex.getHex().y);

			for(int i=0; i<6 || done == true; i++){
				//check if a neighbor is out of bounds or occupied and close them if they are
				if(hexWorld.hexWorldData[(int)neighborList[i].x,(int)neighborList[i].y].inBounds == false || hexWorld.hexWorldData[(int)neighborList[i].x,(int)neighborList[i].y].unitObject != null){

					closedList.Add(new PFList(neighborList[i]));

				}

				//if a neighbor is not in the closed list or the open list and can be reached height wise
				if(closedList.FindIndex(PFList => PFList.getHex() == neighborList[i]) == -1){
					if(hexWorld.hexWorldData[(int)neighborList[i].x,(int)neighborList[i].y].height - hexWorld.hexWorldData[(int)currentHex.getHex().x, (int)currentHex.getHex().y].height >= -0.5 || hexWorld.hexWorldData[(int)neighborList[i].x,(int)neighborList[i].y].height - hexWorld.hexWorldData[(int)currentHex.getHex().x, (int)currentHex.getHex().y].height <= 0.5){
						if(openList.FindIndex(PFList => PFList.getHex() == neighborList[i]) == -1){

							int temph;

							//you can count its h based on how far it is from the goal
							if(currentHex.getHex().x > end.x){
								temph = (int)currentHex.getHex().x - (int)end.x;
							}
							else{
								temph = (int)end.x - (int)currentHex.getHex().x;
							}

							if(currentHex.getHex().y > end.y){
								temph = (int)currentHex.getHex().y - (int)end.y;
							}
							else{
								temph = (int)end.y - (int)currentHex.getHex().y;
							}

							//and add to the openList
							openList.Add(new PFList(neighborList[i], currentHex.getG() + 10, temph * 10, currentHex));
						}
					}//if it is height allowed but already in the openList, check if this currentHex is a better parent FScore wise
					else {
						int tempindex = openList.FindIndex(PFList => PFList.getHex() == neighborList[i]);
						if(currentHex.getG()+10 < openList[tempindex].getG()){
							openList[tempindex].resetParent(currentHex, currentHex.getG() + 10);
						}
					}
				}
			}
		}
		if(fail == true){
			return null;
		}


		//int pLength = closedList[closedList.Count - 1].getG() / 10;
		List<Vector2> path = new List<Vector2>{};
		PFList current;
		current = closedList[closedList.Count - 1];
		path.Add(current.getHex());
		current = current.getParent();

		while(current.getParent() != null){
			path.Insert(0, current.getHex());
			current = current.getParent();
		}

		return path;
	}

	void OnGUI() {
		int i = 0;
		float x=Screen.width;
		float y=Screen.height;
		int numInDepot;

		switch(Network.peerType){
		default:
			//later on just uncomment things
		//case NetworkPeerType.Disconnected:
		//	break;
		//case NetworkPeerType.Client:
			for(i=0;i<Menu.connectionList.Count;i++){
				int offset = i*25;
				GUI.Label(new Rect(10,(10+offset),100,25),Menu.connectionList[i].username);
			}
			GUI.DrawTexture (new Rect(0, y-(x*.22f), x*.2f, x*.22f), depotBack); //Depot Back Splash
			
			//This switch is the GUI for objects in the Depot
			numInDepot = 6;
			switch (numInDepot){ //REPLACE 6 with numInDepot
			case 0:
				Debug.Log("nothing in Depot");
				break;
			case 1:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				
				break;
			case 2:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f), img);
				break;
			case 3:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.14f, y-(x*.185f), x*.06f, x*.09f), img);
				break;
			case 4:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.14f, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(0, y-(x*.09f), x*.06f, x*.09f), img);
				break;
			case 5:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.14f, y-(x*.185f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(0, y-(x*.09f), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.09f), x*.06f, x*.09f), img);		
				break;
			case 6:
				GUI.DrawTexture (new Rect(0, y-(x*.185f), x*.06f, x*.09f), img);
				if(GUI.Button (new Rect(0, y-(x*.185f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[1]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f), img);
				if(GUI.Button (new Rect(x*.07f, y-(x*.185f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[2]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				GUI.DrawTexture (new Rect(x*.14f, y-(x*.185f), x*.06f, x*.09f), img);
				if(GUI.Button (new Rect(x*.14f, y-(x*.185f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[3]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				GUI.DrawTexture (new Rect(0, y-(x*.09f), x*.06f, x*.09f), img);
				if(GUI.Button (new Rect(0, y-(x*.09f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[4]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				GUI.DrawTexture (new Rect(x*.07f, y-(x*.09f), x*.06f, x*.09f), img);
				if(GUI.Button (new Rect(x*.07f, y-(x*.09f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[5]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				GUI.DrawTexture (new Rect(x*.14f, y-(x*.09f), x*.06f, x*.09f), img);	
				if(GUI.Button (new Rect(x*.14f, y-(x*.09f), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[6]=true;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<7; j++){
							EnlargeBool[j]=false;
						}
						ToggleTemp--;
					}
				}
				break;
			}
			GUI.Box (new Rect(x-(x*.3f), y-(y*.25f), x*.15f, y*.25f), "Unit Details & abilities goes here?");
			//GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), img);
			if(GUI.Button (new Rect(x*.2f, y-(y*.25f), x*.5f, y*.25f), "Hand of Cards Goes Here?"))
			{
				if(ToggleTemp ==0){
					EnlargeBool[0]=true;
					ToggleTemp++;
				} else if (ToggleTemp==1){
					EnlargeBool[0]=false;
					ToggleTemp--;
				}
			}
			if(EnlargeBool[0]){
				//GUI.Box (new Rect(x*.2f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");
				GUI.DrawTexture(new Rect(x*.2f, y*.1f, (y*.65f)/1.5f, y*.65f), img);
			}
			
			//GUI.Box (new Rect(x*.1f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");
			if(GUI.Button(new Rect(x-100, y-40, 80, 20), (me == turn?"Pass Turn":"Waiting"))) {
				networkView.RPC ("RequestTurnSwitch",RPCMode.Server);
			}
			break;
		case NetworkPeerType.Server:
			GUI.Label(new Rect(100,100,100,25),"Server");
			GUI.Label(new Rect(100,125,100,25),"Connections: " + Network.connections.Length );
			for(i=0;i<Network.connections.Length;i++){
				int offset = i*50;
				GUI.Label(new Rect(100,(150+offset),150,25),Network.connections[i].guid);
				GUI.Label(new Rect(100,(150+offset+25),100,25),Network.connections[i].externalIP);
				GUI.Label(new Rect(200,(150+offset+25),100,25),Menu.connectionList[i].username);
				GUI.Label(new Rect(300,(150+offset+25),100,25),(Menu.connectionList[i].ready?"Ready":"NotReady"));
				
				//button to kick
			}
			if(GUI.Button(new Rect(100,150+(i*50),100,25),"Shutdown")){
				Application.LoadLevel(0);
				Network.Disconnect(250);	
			}
			break;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}

	float hexDistance2(Vector2 start, Vector2 dest){
		if (start.x == dest.x)
			return Mathf.Abs(dest.y - start.y);
		else if (start.y == dest.y)
			return Mathf.Abs(dest.x - start.x);
		else {
			float dx = Mathf.Abs(dest.x - start.x);
			float dy = Mathf.Abs(dest.y - start.y);
			if (start.y < dest.y) {
				Debug.Log(dx + " a " + dy + " " + Mathf.Ceil(dx / 2.0f));
				return dx + dy - Mathf.Ceil(dx / 2.0f);
			}
			else {
				Debug.Log(dx + " " + dy + " " + Mathf.Floor(dx / 2.0f));
				return dx + dy - Mathf.Floor(dx / 2.0f);
			}
		}
	}

	float hexDistance(Vector2 p1, Vector2 p2){
		float x1 = p1.x;
		float y1 = p1.y;
		float x2 = p2.x;
		float y2 = p2.y;
		float du = x2-x1;
		float dv = (y2 + Mathf.FloorToInt(x2/2f)) - (y1 + Mathf.FloorToInt(x1/2f));
		if((du >= 0 && dv >= 0) || (du < 0 && dv < 0))
			return Mathf.Max(Mathf.Abs(du), Mathf.Abs(dv));
		else 
			return (Mathf.Abs(du) + Mathf.Abs(dv));
	}


	void OnDisconnectedFromServer(NetworkDisconnection info){
		//called on client when client disconnects, and on server when connection has disconnected.
		Application.LoadLevel(0);
		Debug.Log("Disconnected from server: " + info);
	}

	public void moveUnit(Vector2 _selected, Vector2 _point){
		if(turn == me){
			if(playerObjects[me].Contains(hexWorld.hexWorldData[(int)_selected.x,(int)_selected.y].unitObject)){
				Vector3 selected = new Vector3(_selected.x,1,_selected.y);
				Vector3 point = new Vector3(_point.x,1,_point.y);
				//theres no server checking here. dont send 'me' later.
				networkView.RPC("NetworkMove",RPCMode.All,selected,point);
			}
		}
	}

	public void attackUnit(Vector2 _selected, Vector2 _point){
		if(turn == me){
			if(!playerObjects[me].Contains(hexWorld.hexWorldData[(int)_point.x,(int)_point.y].unitObject) && playerObjects[me].Contains(hexWorld.hexWorldData[(int)_selected.x,(int)_selected.y].unitObject)){
				// CALL RPCS
				Vector3 selected = new Vector3(_selected.x,1,_selected.y);
				Vector3 point = new Vector3(_point.x,1,_point.y);
				Stats _selectedUnit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.y].unitObject.GetComponent<Stats>();
				Debug.Log(hexDistance(_selected,_point) + " " + _selectedUnit.attackRange);
				if(hexDistance(_selected,_point) <= _selectedUnit.attackRange){
					networkView.RPC("NetworkAttack",RPCMode.All,selected,point);
				}
			}
		}
	}

	[RPC]
	void Clear(NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				Menu.connectionList.Clear();
			}
		}
	}

	[RPC]
	void Initialize(NetworkMessageInfo info) {
		if(Network.isServer){
			me=-1;
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				me = Menu.connectionList.FindIndex(x => x.guid == Network.player.guid);
			}
		}
	}
	
	[RPC]
	void SpawnObject(int _i, string _guid, string _name, Vector3 _location, NetworkMessageInfo info){
		//if(info.sender.guid == server){
		//TODO : Fuck unity rpcs
		if(!(Menu.connectionList[_i].guid == _guid)){
			_i = Menu.connectionList.FindIndex(x => x.guid == _guid);
		}
		while(_i>=playerObjects.Count){
			playerObjects.Add(new List<GameObject>());
		}
		playerObjects[_i].Add((GameObject)Instantiate(Resources.Load(_name)));
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unit = _name;
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unitObject = playerObjects[_i][playerObjects[_i].Count-1];
		Vector2 center = hexWorld.hexWorldData[(int)_location.x,(int)_location.z].center;
		Vector3 finalloc = new Vector3(center.x,1,center.y);
		Debug.Log (center.x + " " + center.y + " " + finalloc.x + " " + finalloc.z);
		playerObjects[_i][playerObjects[_i].Count-1].transform.position = finalloc;
		//playerObjects[_i][playerObjects[_i].Count-1].renderer.materials[0].color = Color.red;
		MeshRenderer[] a = playerObjects[_i][playerObjects[_i].Count-1].GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mesh in a){
			switch(_i){
			default:
				mesh.material.color = Color.gray;
				break;
			case 0:
				mesh.material.color = Color.cyan;
				break;
			case 1:
				mesh.material.color = Color.magenta;
				break;
			}
		}
		//}
	}

	[RPC]
	void NetworkMove(Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//are you here to fix it sending 'me' aka _i? Have it hunt for sender.guid over all connected guids.
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.transform.position = new Vector3 (hexWorld.hexWorldData[(int)_point.x,(int)_point.z].center.x, 1 , hexWorld.hexWorldData[(int)_point.z,(int)_point.z].center.y);
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject = null;
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit = null;
	}

	[RPC]
	void NetworkAttack(Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//Not secure. Just like networkmove. Have it check to see if the sender is correct.
		Stats _selectedUnit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject.GetComponent<Stats>();
		Stats _targetUnit = hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.GetComponent<Stats>();
		//if distance between them is <= _selectedUnit.attackRange
		_targetUnit.currentHealth -= _selectedUnit.damage;
		if(_targetUnit.currentHealth <= 0){
			Destroy(hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject);
			int targetx = -1 ,targety = -1;
			for(int i = 0; i < playerObjects.Count; i++){
				playerObjects[i].Remove(hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject);
			}
			/*
			 	for (int j = 0; j < playerObjects[i].Count; j++){
					if(playerObjects[i][j] == hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject){
						targetx = i;
						targety = j;
						break;
					}
				}
			*/
			hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject = null;
				//openList.FindIndex(PFList => PFList.getHex() == neighborList[i]);
		}
	}
	
	[RPC]
	void AddConnectionList(string _guid, string _username, NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				Menu.connectionList.Add(new Menu.NConn(_username,_guid));
			}
		}
	}

	[RPC]
	void RequestTurnSwitch(NetworkMessageInfo info){
		if(Network.isServer){
			int c = Menu.connectionList.Count;
			for(int i = 0; i<c; i++){
				if(info.sender.guid == Menu.connectionList[i].guid){
					if(turn == i){;
						int nturn = turn+1==c?0:turn+1;
						turn = nturn;
						Debug.Log (nturn + " " + turn + " " + c + " " + me);
						networkView.RPC("SwitchTurn",RPCMode.All,nturn);
					}
				}
			}
		}
	}

	[RPC]
	void SwitchTurn(int _newTurn, NetworkMessageInfo info){
		if(info.sender.guid == server)
			turn = _newTurn;
	}

	public class PFList{
		int fScore;
		int gScore;
		int hScore;
		Vector2 hex;
		PFList parent;

		public PFList(Vector2 start, int G, int H, PFList p){
			hex = start;
			parent = p;
			fScore = G + H;
			gScore = G;
			hScore = H;
		}

		public PFList(Vector2 start){
			hex = start;
		}

		public int getF(){
			return fScore;
		}

		public int getG(){
			return gScore;
		}

		public int getH(){
			return hScore;
		}

		public PFList getParent(){
			return parent;
		}

		public Vector2 getHex(){
			return hex;
		}

		public void resetParent(PFList newP, int newG){
			parent = newP;
			gScore = newG;
			fScore = gScore + hScore ;
		}
	};


}
