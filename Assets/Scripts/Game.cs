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

	void Start () {
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
					Vector3 location = new Vector3(i,1,j);
					networkView.RPC("SpawnObject",RPCMode.All,i,guid,name,location);
				}
			}
			break;
		}

	}

	/*
	Vector2[] getNeighbors(int x, int y, int radi){
			
		//Initializes first neighbor, changes slightly if y coordinate is even.
		int tempX = x;
		int tempY = y + 1;
		int lastTempX = x;
		int lastTempY = y + 1;
		int tempCheckRange = 0;
		string dir = "East";
		int direction = 1;
		int workingRadi = 1;
		int radiCopy = radi;
		int totalNeighbors = 0;
		int neighborIndex = 0;
		while (radiCopy>0) {
			totalNeighbors=totalNeighbors+(6*radiCopy);
			radiCopy--;
		}
		Vector2[] neighborList = new Vector2[totalNeighbors];


		if ((y % 2) > 0) {
			tempX = x - 1;
			lastTempX=x-1;
		}


		while (workingRadi<=radi) {
			direction=1;
			for (int i=0; i<=(6*workingRadi); i++) {
				tempCheckRange = Mathf.Abs(tempX - x) + Mathf.Abs(tempY - y);
				if(tempCheckRange==workingRadi){
					if(hexWorld.hexWorldData[tempX,tempY].unit==null){
						neighborList[neighborIndex]=new Vector2(tempX,tempY);
						neighborIndex++;
					}
					lastTempX=tempX;
					lastTempY=tempY;
				} else {
					tempX=lastTempX;
					tempY=lastTempY;
					direction++;
					i--;
				}
				switch (direction) {
				case 1:
						dir = "East";
						tempX++;
						break;
				case 2:
						dir = "SE";
						if(tempY%2==0){
							tempY--;
						} else {
							tempY--;
							tempX++;
						}
						break;
				case 3:
						dir = "SW";
						if(tempY%2==0){
							tempX--;
							tempY--;
						}else{
							tempY--;
						}
						break;
				case 4:
						dir = "West";
						tempX--;
						break;
				case 5:
						dir = "NW";
						if(tempY%2==0){
							tempX--;
							tempY++;
						}else{
							tempY++;
						}
						break;
				case 6:
						dir = "NE";
						if(tempY%2==0){
							tempY++;
						} else {
							tempY++;
							tempX++;
						}
						break;
				default:
						dir = "ERROR";
						break;							
				}
						
			}
			workingRadi++;
			tempY = tempY + 2;
		}
		return (neighborList);
			
	}
*/

	public Vector2[] getNeighbor(int x, int y)
	{
		Vector2[] neighborList = new Vector2[6];
		int numberOfNeighbors = 0;
		bool odd = false;
		
		if ((y % 2) > 0) {
			odd = true;
		}
		
		Debug.Log ("START: " + x + " " + y + " " + hexWorld.hexWorldData[x,y].height);
		//adds x neighbors
		if(x-1 >= xLowerBound && x-1 <= xUpperBound && y <= yUpperBound && y >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y].x,hexWorld.hexWorldData[x-1,y].y);
			Debug.Log("A: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		if(x+1 <= xUpperBound && x+1 >= xLowerBound && y <= yUpperBound && y >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y].x,hexWorld.hexWorldData[x+1,y].y);
			Debug.Log("B: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		//adds y neighbors
		if(y-1 >= yLowerBound && x >= xLowerBound && x <= xUpperBound && y-1 <= yUpperBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y-1].x,hexWorld.hexWorldData[x,y-1].y);
			Debug.Log("C: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		if(y+1 <= yUpperBound && x >= xLowerBound && x <= xUpperBound && y+1 >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y+1].x,hexWorld.hexWorldData[x,y+1].y);
			Debug.Log("D: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		//gets y right neighbors for odd
		if(odd == true) {
			Debug.Log ("If");
			if(x+1 <= xUpperBound && y-1 >= yLowerBound && x+1 >= xLowerBound && y-1 <= yUpperBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y-1].x,hexWorld.hexWorldData[x+1,y-1].y);
				Debug.Log("E: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
			
			if(x+1 <= xUpperBound && y+1 <= yUpperBound && x+1 >= xLowerBound && y+1 >= yLowerBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y+1].x,hexWorld.hexWorldData[x+1,y+1].y);
				Debug.Log("F: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
		} else {
			Debug.Log ("Else");
			//gets y left neighbors for even
			if(x-1 <= xUpperBound && y+1 <= yUpperBound && x-1 >= xLowerBound && y+1 >= yLowerBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y+1].x,hexWorld.hexWorldData[x-1,y+1].y);
				Debug.Log("G: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
			
			if(x-1 <= xUpperBound && y-1 >= yLowerBound && x-1 >= xLowerBound && y-1 <= yUpperBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y-1].x,hexWorld.hexWorldData[x-1,y-1].y);
				Debug.Log("H: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
		}
		
		for(int i=0; i<numberOfNeighbors; i++)
		{
			hexWorld.SetHexUVs(neighborList[i],2);
		}
		return neighborList;
	}



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
			PFList currentHex = new PFList();

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
			openList.Remove(choose);

			if(currentHex.getHex().x == end.x && currentHex.getHex().y == end.y){

				done = true;
			}

			closedList.Add(currentHex);

			//get neighbors
			Vector2[] neighborList = new Vector2[6];


			//PLZ WORK O HELP ME GAWD
			neighborList = getNeighbor(currentHex.x, currentHex.y);

			for(int i=0; i<6 || done == true; i++){
				//check if a neighbor is out of bounds or occupied and close them if they are
				if(hexWorld.hexWorldData[neighborList[i].x,neighborList[i].y].inBounds == false || hexWorld.hexWorldData[neighborList[i].x,neighborList[i].y].unitObject != null){

					closedList.Add(new PFList(neighborList[i]));

				}

				//if a neighbor is not in the closed list or the open list and can be reached height wise
				if(!closedList.FindIndex(PFList => PFList.getHex() == neighborList[i]) == -1){
					if(hexWorld.hexWorldData[neighborList[i].x,neighborList[i].y].height - hexWorld.hexWorldData[currentHex.x, currentHex.y].height >= -0.5 || hexWorld.hexWorldData[neighborList[i].x,neighborList[i].y].height - hexWorld.hexWorldData[currentHex.x, currentHex.y].height <= 0.5){
						if(openList.FindIndex(PFList => PFList.getHex() == neighborList[i]) == -1){

							int temph;

							//you can count its h based on how far it is from the goal
							if(currentHex.getHex().x > end.x){
								temph = currentHex.getHex().x - end.x;
							}
							else{
								temph = end.x - currentHex.getHex().x;
							}

							if(currentHex.getHex().y > end.y){
								temph = currentHex.getHex().y - end.y;
							}
							else{
								temph = end.y - currentHex.getHex().y;
							}

							//and add to the openList
							openList.Add(new PFList(neighborList[i], currentHex.getG() + 10, temph * 10, currentHex));
						}
					}//if it is height allowed but already in the openList, check if this currentHex is a better parent FScore wise
					else {
						int tempindex = openList.FindIndex(PFList => PFList.getHex() == neighborList[i]);
						if(currentHex.getG()+10 < openList[tempindex].getG){
							openList[tempindex].resetParent(currentHex, currentHex.getG() + 10)
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
		PFList current = new PFList();
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
		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:
			break;
		case NetworkPeerType.Client:
			for(i=0;i<Menu.connectionList.Count;i++){
				int offset = i*25;
				GUI.Label(new Rect(10,(10+offset),100,25),Menu.connectionList[i].username);
				//button to kick
				//playerObjects[_i][playerObjects[_i].Count-1].transform.position = _location;
				//for(int j=0;j<depotList[i].Count;j++){
				//	GUI.Label(new Rect(100+(100*j),(10+offset),100,25),depotList[i][j]);
				//}
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

	void OnDisconnectedFromServer(NetworkDisconnection info){
		//called on client when client disconnects, and on server when connection has disconnected.
		Application.LoadLevel(0);
		Debug.Log("Disconnected from server: " + info);
	}

	public void moveUnit(Vector2 _selected, Vector2 _point){
		if(turn == me){
			Vector3 selected = new Vector3(_selected.x,1,_selected.y);
			Vector3 point = new Vector3(_point.x,1,_point.y);
			//theres no server checking here. dont send 'me' later.
			networkView.RPC("NetworkMove",RPCMode.All,me,Network.player.guid,selected,point);
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
		//}
	}

	[RPC]
	void NetworkMove(int _i, string _guid, Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//are you here to fix it sending 'me' aka _i? Have it hunt for sender.guid over all connected guids.
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.transform.position = new Vector3 (hexWorld.hexWorldData[(int)_point.x,(int)_point.z].center.x, 1 , hexWorld.hexWorldData[(int)_point.z,(int)_point.z].center.y);
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject = null;
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit = null;
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
					if(turn == i){
						networkView.RPC("SwitchTurn",RPCMode.All,(turn+1>c?0:(turn+1)));
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
