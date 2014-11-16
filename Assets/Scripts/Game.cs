using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	// Use this for initialization
	public List<List<GameObject>> playerObjects = new List<List<GameObject>>{};
	public GameObject Cylinder;
	public int me;
	public static string server = null;
	
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
				for(int j = 0; j<5; j++){
					string name = "Cylinder";
					string guid = Menu.connectionList[i].guid;
					Vector3 location = new Vector3(i*1, 1, j*2);
					networkView.RPC("SpawnObject",RPCMode.All,i,guid,name,location);
				}
			}
			break;
		}

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
				GUI.Label(new Rect(100,(150+offset),100,25),Menu.connectionList[i].username);
				GUI.Label(new Rect(200,(150+offset),100,25),(Menu.connectionList[i].ready?"Ready!":"Not Ready!"));
				//button to kick
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
	void SpawnObject(int i, string guid, string name, Vector3 location, NetworkMessageInfo info){
		//if(info.sender.guid == server){
		//TODO : Fuck unity rpcs
			if(!(Menu.connectionList[i].guid == guid)){
				i = Menu.connectionList.FindIndex(x => x.guid == guid);
			}
			while(i>=playerObjects.Count){
				playerObjects.Add(new List<GameObject>());
			}
			playerObjects[i].Add((GameObject)Instantiate(Resources.Load("Cylinder")));
			playerObjects[i][playerObjects[i].Count-1].transform.position = location;
		//}
	}
	
	[RPC]
	void AddConnectionList(string guid, string username, NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				Menu.connectionList.Add(new Menu.NConn(username,guid));
			}
		}
	}
}
