using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	
	// Use this for initialization
	public HexWorld hexWorld;
	public TerrainRaycaster terrainCaster;

	public List<List<GameObject>> playerObjects = new List<List<GameObject>>{};
	public List<GUIStyle> style = new List<GUIStyle>{};
	public List<List<GameObject>> depotList = new List<List<GameObject>>{};
	public List<GameObject> factoryList = new List<GameObject>{};
	public List<GameObject> generatorList = new List<GameObject>{};

	public List<List<GameObject>> handList = new List<List<GameObject>>{};
	public List<List<GameObject>> staticDeckList = new List<List<GameObject>>{};
	public List<List<GameObject>> currentDeckList = new List<List<GameObject>>{};


	public static string server = null;
	public int turn = 0;
	public int me = -1;

	//map bounds inclusive
	public int xLowerBound = 80;
	public int xUpperBound = 99;
	public int yLowerBound = 0;
	public int yUpperBound = 25;

	public int ToggleTemp=0;
	//public bool handToggleTempBool=false;
	public static int numThingsInteractable =21;
	bool[] EnlargeBool = new bool[numThingsInteractable]; //Bool[0-5] = Hand Enlarging, Bool[6] = Depot Enlarging

	public int handToggleTemp=0;
	public bool handToggleTempBool=false;
	
	public Texture artex;
	Texture2D img;
	Texture2D Black;
	Texture2D Red;
	Texture2D Blue;
	Texture2D Magenta;
	Texture2D Cyan;
	Texture2D depotBack;
	int cardsInHand;

	bool toFactoryBool=false;
	bool toSpawnBool=false;
	
	public bool sound=true;

	void Start () {
		Magenta=Resources.Load ("Magenta_Texture")as Texture2D;
		Cyan=Resources.Load ("Cyan_Texture")as Texture2D;
		depotList.Capacity = 3;
		factoryList.Capacity = 8;
		generatorList.Capacity = 3;
		handList.Capacity = 7;
		staticDeckList.Capacity = 30;
		currentDeckList.Capacity = 30;



		//populate factoryList somewhere;

		depotBack=Resources.Load("depotWindow") as Texture2D;
		Black=Resources.Load ("Black_Texture")as Texture2D;
		Red=Resources.Load ("Red_Texture")as Texture2D;
		Blue=Resources.Load ("Blue_Texture")as Texture2D;
		img = Resources.Load("Deck_02") as Texture2D;
		for(int i=0; i<numThingsInteractable; i++){
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
				style.Add(new GUIStyle());
				switch(i){
				default:
					style[i].normal.textColor = Color.gray;
					break;
				case 0:
					style[i].normal.textColor = Color.cyan;
					break;
				case 1:
					style[i].normal.textColor = Color.magenta;
					break;
				}
				depotList.Add(new List<GameObject>());

				//spawn shit
				networkView.RPC("SpawnNeutral",RPCMode.All,"generator",new Vector3(80,1,13));
				networkView.RPC("SpawnNeutral",RPCMode.All,"generator",new Vector3(88,1,11));
				networkView.RPC("SpawnNeutral",RPCMode.All,"generator",new Vector3(95,1,14));

				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(88,1,0));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(90,1,0));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(91,1,25));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(89,1,25));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(97,1,9));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(85,1,14));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(82,1,19));
				networkView.RPC("SpawnNeutral",RPCMode.All,"factory",new Vector3(92,1,12));

				for(int j = 0; j<5; j++){
					string name;
					if(j<3)
						name = "Vtol";
					else {
						name = "hoverArty";
					}
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
		
		if(x > xUpperBound || x < xLowerBound || y > yUpperBound || y < yLowerBound){
			Debug.Log("yep");
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
		Stats sunit=null;
		Building bunit = null;

		if (terrainCaster.selected != -Vector2.one) {
			if (hexWorld.hexWorldData [(int)terrainCaster.selected.x, (int)terrainCaster.selected.y].unitObject != null) {
					sunit = hexWorld.hexWorldData [(int)terrainCaster.selected.x, (int)terrainCaster.selected.y].unitObject.GetComponent<Stats> ();
			} else {
					sunit = null;
			}
			if (hexWorld.hexWorldData [(int)terrainCaster.selected.x, (int)terrainCaster.selected.y].building != null) {
				bunit = hexWorld.hexWorldData [(int)terrainCaster.selected.x, (int)terrainCaster.selected.y].building.GetComponent<Building> ();
			} else {
				bunit = null;
			}
		} else {
			sunit = null;
			bunit = null;
		}
		//

		switch(Network.peerType){
		default:
			//later on just uncomment things
		//case NetworkPeerType.Disconnected:
		//	break;
		//case NetworkPeerType.Client:
			for(i=0;i<playerObjects.Count;i++){
				int offset = i*25;
				GUI.Label(new Rect(10,(10+offset),100,25),Menu.connectionList[i].username,style[i]);
			}
			GUI.DrawTexture (new Rect(0, y-(x*.22f), x*.2f, x*.22f), depotBack); //Depot Back Splash
			
			//This switch is the GUI for objects in the Depot
			numInDepot = 2;

			for(int q=0; q<numInDepot; q++){
				int rofl=q/3;
				//GUI.DrawTexture (new Rect(0, y-(x*(.185f)*(rofl/2)), x*.06f, x*.09f), img);
				GUI.DrawTexture (new Rect((0+(q%3*x*.07f)), y-(x*(.0925f)*(rofl+1)), x*.06f, x*.09f), img);
				if(GUIButton.Button (new Rect((0+(q%3*x*.07f)), y-(x*(.0925f)*(rofl+1)), x*.06f, x*.09f),"")){
					if(ToggleTemp ==0){
						for(int j=0; j<numThingsInteractable; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[q]=true;
						toFactoryBool=false;
						toSpawnBool=false;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<numThingsInteractable; j++){
							EnlargeBool[j]=false;
						}
						toFactoryBool=false;
						toSpawnBool=false;
						ToggleTemp--;
					}
				}
			}

			if (sound == false)
			{
				if(GUI.Button (new Rect(x*.9f, 0 ,x*.1f, y*.05f),"Mute") ) // MyGUISkin.customStyles[1] is unselected button image
				{
					sound = true;
					AudioListener.pause = true;
					Debug.Log ("Tick On");
				}
			}
			else
			{
				if(GUI.Button (new Rect(x*.9f, 0 ,x*.1f, y*.05f),"UnMute"))  // MyGUISkin.customStyles[2] is selected button image
				{
					sound = false;
					AudioListener.pause = false; 
					Debug.Log ("Tick Off");
				}
			}


			GUI.Box (new Rect(x-(x*.25f), y-(y*.25f), x*.15f, y*.25f), "");
			if(sunit) {
				GUI.Label(new Rect(x*.75f, y*.75f, x*.15f, y*.05f),hexWorld.hexWorldData [(int)terrainCaster.selected.x, (int)terrainCaster.selected.y].unit);
				GUI.Label(new Rect(x*.75f, y*.8f, x*.15f, y*.05f),"Health: "+sunit.currentHealth+"/"+sunit.maximumHealth);
				GUI.Label(new Rect(x*.75f, y*.85f, x*.15f, y*.05f),"Movement: "+sunit.moveSpeed);
				GUI.Label(new Rect(x*.75f, y*.9f, x*.15f, y*.05f),"Range: "+sunit.attackRange);
				GUI.Label(new Rect(x*.75f, y*.95f, x*.15f, y*.05f),"Damage: "+sunit.damage);
			}
			GUI.Box (new Rect((x*.9f), (y*.75f), x*.1f, y*.15f), "");
			if(bunit) {
				GUI.Label(new Rect(x*.9f, y*.75f, x*.15f, y*.05f),bunit.name);
				GUI.Label(new Rect(x*.9f, y*.8f, x*.15f, y*.05f),"Cap: "+bunit.capCurrent+"/"+bunit.capMax);
				GUI.Label(new Rect(x*.9f, y*.85f, x*.15f, y*.05f),"Progress: "+bunit.buildProgress);
			}

			//GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), img);

			cardsInHand=7;

			for(int z=0; z<cardsInHand; z++){
				GUI.DrawTexture (new Rect(((x*.225f)+((z*2f)/3f)*(x*.10f)), y-(x*.15f), x*.1f, x*.15f), img);
				if(GUIButton.Button (new Rect(((x*.225f)+((z*2f)/3f)*(x*.10f)), y-(x*.15f), x*.1f, x*.15f), ""))
				{
					if(ToggleTemp ==0){
						for(int j=0; j<numThingsInteractable; j++){
							EnlargeBool[j]=false;
						}
						EnlargeBool[z+6]=true;
						toFactoryBool=false;
						toSpawnBool=false;
						ToggleTemp++;
					} else if (ToggleTemp==1){
						for(int j=0; j<numThingsInteractable; j++){
							EnlargeBool[j]=false;
						}
						toFactoryBool=false;
						toSpawnBool=false;
						ToggleTemp--;
					}
				}

			}

			//This Block handles the Health Bars
			GUI.DrawTexture (new Rect(x*.1f, 0, x*.375f, y*.075f), Black);
			GUI.DrawTexture (new Rect(x*.525f, 0, x*.375f, y*.075f), Black);
			int currentRedHealth=50;
			int currentBlueHealth=50;
			for(int m=200; m>(200-currentRedHealth); m--){
				GUI.DrawTexture (new Rect(((x*.11f)+(m/200f)*(x*.355f)), .01f*y, x*.00355f, y*.055f), Magenta);
			}
			for(int n=0; n<currentBlueHealth; n++){
				GUI.DrawTexture (new Rect(((x*.535f)+(n/200f)*(x*.355f)), .01f*y, x*.00355f, y*.055f), Cyan);
			}
			GUI.Box(new Rect(x*.45f, .075f*y, x*.10f, y*.1f),"");
			GUI.Label (new Rect(x*.46f,.075f*y,x*.05f, y*.05f), currentRedHealth+"/", style[1]);
			GUI.Label (new Rect(x*.46f,y*.125f,x*.05f, y*.05f), "200", style[1]);
			GUI.Label (new Rect(x*.515f,.075f*y,x*.05f, y*.05f), currentBlueHealth+"/", style[0]);
			GUI.Label (new Rect(x*.515f,y*.125f,x*.05f, y*.05f), "200", style[0]);


			//This Block displays current enlarged selection
			for(int q=0; q<numThingsInteractable; q++){
				if(EnlargeBool[q]){
					if(q<6){
						GUI.DrawTexture(new Rect(x*.4f, y*.1f, (y*.65f)/1.5f, y*.65f), img);
						if(GUIButton.Button (new Rect((x*.225f), y*.4f, x*.17f, y*.15f), "Spawn Unit")){
							for(int j=0; j<numThingsInteractable; j++){
								EnlargeBool[j]=false;
							}
							ToggleTemp--;
							toFactoryBool=false;
							toSpawnBool=true;

						}
					} else if (q>5 && q<13){
						GUI.DrawTexture(new Rect(x*.4f, y*.1f, (y*.65f)/1.5f, y*.65f), img);
						if(GUIButton.Button (new Rect((x*.225f), y*.4f, x*.17f, y*.15f), "Send to Factory")){
							for(int j=0; j<numThingsInteractable; j++){
								EnlargeBool[j]=false;
							}
							ToggleTemp--;
							toFactoryBool=true;
							toSpawnBool=false;
							
						}
					}
				}
			}

			if(toFactoryBool){

				GUI.Box (new Rect((x*.3f), y*.4f, x*.2f, y*.1f), "IM A FUCKING BOX");
				Debug.Log ("Im Drawing this box");
			}

			if(toSpawnBool){
				GUI.Box (new Rect((x*.3f), y*.4f, x*.2f, y*.1f), "IM A DIFFERENT FUCKING BOX");
			}

			
			//GUI.Box (new Rect(x*.1f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");
			if(GUIButton.Button(new Rect(x*.9f, y*.9f, x*.1f, y*.1f), (me == turn?"Pass Turn":"Waiting"))) {
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
			if(GUIButton.Button(new Rect(100,150+(i*50),100,25),"Shutdown")){
				Application.LoadLevel(0);
				Network.Disconnect(250);	
			}
			break;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}

	float hexDistance(Vector2 start, Vector2 dest){
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

	void Shuffle<T>(List<T> list) {  
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
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
				Stats _selectedUnit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.y].unitObject.GetComponent<Stats>();
				if(!_selectedUnit.canMove || _selectedUnit.hasMoved)
					return;
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
				if(!_selectedUnit.canAttack || _selectedUnit.hasAttacked)
					return;
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
	void SpawnNeutral(string _name, Vector3 _location, NetworkMessageInfo info){
		if(Network.isServer || info.sender.guid == server){
			GameObject a = (GameObject)Instantiate(Resources.Load(_name));
			if(_name == "generator"){
				generatorList.Add(a);
			} else if (_name == "factory"){
				factoryList.Add(a);
			}
			hexWorld.hexWorldData[(int)_location.x,(int)_location.z].building = a;
			Vector2 center = hexWorld.hexWorldData[(int)_location.x,(int)_location.z].center;
			Building _unit = a.GetComponent<Building>();
			_unit.location = new Vector2((int)_location.x,(int)_location.z);
			Vector3 finalloc = new Vector3(center.x,_unit.spawnHeight+hexWorld.hexWorldData[(int)_location.x,(int)_location.z].height,center.y);
			Debug.Log ("Neutral " +_name+" "+center.x + " " + center.y + " " + finalloc.x + " " + finalloc.z);
			a.transform.position = finalloc;
		}
	}

	[RPC]
	void SpawnObject(int _i, string _guid, string _name, Vector3 _location, NetworkMessageInfo info){
		//if(info.sender.guid == server){
		//TODO : Fuck unity rpcs
		if(!(Menu.connectionList[_i].guid == _guid)){
			_i = Menu.connectionList.FindIndex(x => x.guid == _guid);
		}
		playerObjects[_i].Add((GameObject)Instantiate(Resources.Load(_name)));
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unit = _name;
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unitObject = playerObjects[_i][playerObjects[_i].Count-1];
		Vector2 center = hexWorld.hexWorldData[(int)_location.x,(int)_location.z].center;
		Stats _unit = playerObjects[_i][playerObjects[_i].Count-1].GetComponent<Stats>();
		Vector3 finalloc = new Vector3(center.x,_unit.spawnHeight,center.y);
		Debug.Log (center.x + " " + center.y + " " + finalloc.x + " " + finalloc.z);
		playerObjects[_i][playerObjects[_i].Count-1].transform.position = finalloc;
		//playerObjects[_i][playerObjects[_i].Count-1].renderer.materials[0].color = Color.red;
		MeshRenderer[] a = playerObjects[_i][playerObjects[_i].Count-1].GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mesh in a){
			mesh.material.color = style[_i].normal.textColor;
		}
		//}
	}
	
	[RPC]
	void NetworkMove(Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//are you here to fix it sending 'me' aka _i? Have it hunt for sender.guid over all connected guids.
		Stats _selectedUnit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject.GetComponent<Stats>();
		if(!_selectedUnit.canMove || _selectedUnit.hasMoved)
			return;
		_selectedUnit.hasMoved = true;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.transform.position = new Vector3 (hexWorld.hexWorldData[(int)_point.x,(int)_point.z].center.x, hexWorld.hexWorldData[(int)_point.x,(int)_point.z].height+_selectedUnit.spawnHeight , hexWorld.hexWorldData[(int)_point.z,(int)_point.z].center.y);
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject = null;
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit = null;
	}

	[RPC]
	void NetworkAttack(Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//Not secure. Just like networkmove. Have it check to see if the sender is correct.
		Stats _selectedUnit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject.GetComponent<Stats>();
		Stats _targetUnit = hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.GetComponent<Stats>();
		//if distance between them is <= _selectedUnit.attackRange
		if(!_selectedUnit.canAttack || _selectedUnit.hasAttacked)
			return;
		_selectedUnit.hasAttacked = true;
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
				playerObjects.Add(new List<GameObject>());
				style.Add(new GUIStyle());
				depotList.Add(new List<GameObject>());
				switch(style.Count-1){
				default:
					style[style.Count-1].normal.textColor = Color.gray;
					break;
				case 0:
					style[style.Count-1].normal.textColor = Color.cyan;
					break;
				case 1:
					style[style.Count-1].normal.textColor = Color.magenta;
					break;
				}
				handList.Add(new List<GameObject>());
				staticDeckList.Add(new List<GameObject>());
				currentDeckList.Add(new List<GameObject>());
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
						Debug.Log (nturn + " " + turn + " " + c + " " + me);
						networkView.RPC("SwitchTurn",RPCMode.All,nturn);
					}
				}
			}
		}
	}
	
	[RPC]
	void SwitchTurn(int _newTurn, NetworkMessageInfo info){
		if(Network.isServer || info.sender.guid == server){
			//end of turn
			foreach (GameObject n in playerObjects[turn]){
				Stats _n = n.GetComponent<Stats>();
				if(_n.moveList.Count != 0){
					//move
				}
			}
			foreach (GameObject n in generatorList){
				Building _n = n.GetComponent<Building>();
				Vector2 loc = _n.location;
				GameObject _unit = hexWorld.hexWorldData[(int)loc.x,(int)loc.y].unitObject;
				if(_unit != null){
					if(playerObjects[turn].Contains(_unit)){
						if(turn != _n.owner && !_n.notCapped){
							if(_n.capCurrent>0){
								_n.capCurrent--;
							} else if(_n.capCurrent <= 0){
								_n.notCapped = true;
								_n.capCurrent++;
							}
						} else if(_n.capCurrent<_n.capMax){
							_n.capCurrent++;
						}
						if(_n.capCurrent == _n.capMax){
							_n.owner = turn;
							MeshRenderer[] a = n.GetComponentsInChildren<MeshRenderer>();
							foreach(MeshRenderer mesh in a){
								mesh.material.color = style[turn].normal.textColor;
							}
							//
						}
					}
				}
			}
			foreach (GameObject n in factoryList){
				Building _n = n.GetComponent<Building>();
				Vector2 loc = _n.location;
				GameObject _unit = hexWorld.hexWorldData[(int)loc.x,(int)loc.y].unitObject;
				if(_unit != null){
					if(playerObjects[turn].Contains(_unit)){
						if(turn != _n.owner && !_n.notCapped){
							if(_n.capCurrent>0){
								_n.capCurrent--;
							} else if(_n.capCurrent <= 0){
								_n.notCapped = true;
								_n.capCurrent++;
							}
						} else if(_n.capCurrent<_n.capMax){
							_n.capCurrent++;
						}
						if(_n.capCurrent == _n.capMax){
							_n.owner = turn;
							MeshRenderer[] a = n.GetComponentsInChildren<MeshRenderer>();
							foreach(MeshRenderer mesh in a){
								mesh.material.color = style[turn].normal.textColor;
							}
							//
						}
					}
				}
			}

			//start of turn
			turn = _newTurn;
			for(int i = 0; i < playerObjects.Count; i++){
				foreach (GameObject obj in playerObjects[i]){
					Stats unit = obj.GetComponent<Stats>();
					unit.hasMoved = false;
					unit.hasAttacked = false;
				}
			}
			//handList[turn].Add();
			
		}
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
