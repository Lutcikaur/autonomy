using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;


public class Menu : MonoBehaviour {

	static Menu _instance;

	public Texture backgroundTexture;

	//Set up for tutorial slide show
	/*public Texture[] slides;
	public int currentSlide; */ 

	public string IP = "127.0.0.1";
	public int Port = 25001;
	public string stringPort = "25001";

	public int ready = 0;

	public class NConn
	{
		public string username { get; set; }
		public string guid { get; set; }
		public bool ready = false;

		//Other properties, methods, events...
		public NConn(string uname){
			username = uname;
		}
		public NConn(string uname, string g){
			username = uname;
			guid = g;
		}
		public NConn(){
		}
	};

	public NConn me;

	public static List<NConn> connectionList = new List<NConn>{};
	public static string server = null;
	
	public string username = "";
	public string type = "none";

	void OnGUI() {
		int i = 0;



		//GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), backgroundTexture); 

		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:
			// Should make the background images, doesn't at all. 
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), backgroundTexture); 
			if(type == "none"){
				if(GUI.Button(new Rect(100,100,100,25),"Start Client")){
					username = "";
					type = "client";
				}
				if(GUI.Button(new Rect(100,125,100,25),"Start Server")){
					connectionList.Clear();
					ready = 0;
					Network.InitializeServer(10,Port);
					server = Network.player.guid;
				}
				if(GUI.Button(new Rect(100,150,100,25), "Tutorial")){
					Application.LoadLevel("Tutorial"); 
				}
				if(GUI.Button (new Rect(100,175,100,25), "Glossary")){
					Application.LoadLevel ("Glossary");
				}
			} else if(type == "client"){
				GUI.Label(new Rect(100,125,100,25),"Username:");
				username = GUI.TextArea(new Rect(200,125,100,25),username);
				username = Regex.Replace(username, @"\t|\n|\r", "");
				GUI.Label(new Rect(100,150,100,25),"IP Address:");
				IP = GUI.TextArea(new Rect(200,150,100,25),IP);
				GUI.Label(new Rect(100,175,100,25),"Port:");
				stringPort = GUI.TextArea(new Rect(200,175,100,25),stringPort);

				if(GUI.Button(new Rect(150,225,100,25),"Connect")){
					me = new NConn(username);
					connectionList.Clear();
					ready = 0;
					server = null;
					int pt;
					if(stringPort.Equals("")){
						Network.Connect(IP,Port);
					} else if(int.TryParse(stringPort,out pt)){
						Network.Connect(IP,pt);
					}
				}
				if(GUI.Button(new Rect(150,250,100,25),"Back")){
					type = "none";
				}
			}
			break;
		case NetworkPeerType.Client:
			for(i=0;i<connectionList.Count;i++){
				int offset = i*25;
				GUI.Label(new Rect(100,(150+offset),100,25),connectionList[i].username);
				GUI.Label(new Rect(200,(150+offset),100,25),(connectionList[i].ready?"Ready!":"Not Ready!"));
				//button to kick
			}
			if(GUI.Button(new Rect(100,150+(i*25),100,25),(me.ready?"Ready!":"Not Ready!"))){
				me.ready = !me.ready;
				networkView.RPC("Ready",RPCMode.Server,me.ready, me.guid);
			}
			if(GUI.Button(new Rect(100,175+(i*25),100,25),"Disconnect")){
				Network.Disconnect(250);	
			}
			break;
		case NetworkPeerType.Server:
			GUI.Label(new Rect(100,100,100,25),"Server");
			GUI.Label(new Rect(100,125,100,25),"Connections: " + Network.connections.Length + "/" + connectionList.Count);
			GUI.Label(new Rect(250,125,100,25),"Ready: " + ready + "/" + connectionList.Count);
			for(i=0;i<Network.connections.Length;i++){
				int offset = i*50;
				GUI.Label(new Rect(100,(150+offset),150,25),Network.connections[i].guid);
				GUI.Label(new Rect(100,(150+offset+25),100,25),Network.connections[i].externalIP);
				GUI.Label(new Rect(200,(150+offset+25),100,25),connectionList[i].username);
				GUI.Label(new Rect(300,(150+offset+25),100,25),(connectionList[i].ready?"Ready":"NotReady"));

				//button to kick
			}
			if(GUI.Button(new Rect(100,150+(i*50),100,25),"Shutdown")){
				Network.Disconnect(250);	
			}
			break;
		}
	}
	/*
	verify onplayerconnected broadcast, onplayerdisconnected broadcast,
	playerconnected and playerdisconnected, check rpc buffer.

	might need to send NetworkPlayer,username isntead of NConn
	*/

	void awake(){
		DontDestroyOnLoad(this);
	}

	void OnConnectedToServer(){
		//called on client when client connects
		networkView.RPC("Initialize",RPCMode.Server,username);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info){
		//called on client when client disconnects, and on server when connection has disconnected.
		Debug.Log("Disconnected from server: " + info);
	}
	
	void OnPlayerConnected(NetworkPlayer _player){
		//called on server when NetworkPlayer player connects
		//add to list
		NConn p = new NConn();
		p.guid = _player.guid;
		connectionList.Add(p);
		//broadcast client addition.
	}

	void OnPlayerDisconnected(NetworkPlayer _player){
		//called on server when NetworkPlayer player disconnects
		int c = connectionList.Count;
		string p;
		for(int i = 0; i<c; i++){
			if(_player.guid == connectionList[i].guid){
				p = connectionList[i].guid;
				if(connectionList[i].ready){
					ready--;
				}
				connectionList.RemoveAt(i);
				//broadcast client removal.
				Network.RemoveRPCs(_player);
				networkView.RPC("PlayerDisconnected",RPCMode.Others,p);
				break;
			}
		}
	}

	[RPC]
	void Initialize(string _input, NetworkMessageInfo info) {
		//client calls to update username on server side.
		if(Network.isServer){
			int c = connectionList.Count;
			for(int i = 0; i<c; i++){
				if(info.sender.guid == connectionList[i].guid){
					connectionList[i].username = _input;
					networkView.RPC("Initialize",info.sender,info.sender.guid);
					networkView.RPC("PlayerConnected",RPCMode.Others,connectionList[i].username,connectionList[i].guid,connectionList[i].ready);
					return;
				}
			}
		} else if(Network.isClient){
			if(server == null || server == info.sender.guid){
				server = info.sender.guid;
				me.guid = _input;
				networkView.RPC("RequestUsers",RPCMode.Server);
			}
		}
	}

	[RPC]
	void RequestUsers(NetworkMessageInfo info) {
		//client calls to update username on server side.
		if(Network.isServer){
			int c = connectionList.Count;
			for(int i = 0; i<c; i++){
				networkView.RPC("PlayerConnected",info.sender,connectionList[i].username,connectionList[i].guid,connectionList[i].ready);
			}
		}
	}

	[RPC]
	void PlayerConnected(string _username, string _guid, bool _rdy, NetworkMessageInfo info){
		if(Network.isClient){
			if(info.sender.guid == server){
				for(int i = 0; i<connectionList.Count; i++){
					if(connectionList[i].guid == _guid){
						return;
					}
				}
				NConn p = new NConn(_username,_guid);
				p.ready = _rdy;
				connectionList.Add(p);
			}
		}
	}

	[RPC]
	void PlayerDisconnected(string _n, NetworkMessageInfo info){
		if(Network.isClient){
			if(info.sender.guid == server){
				int c = connectionList.Count;
				for(int i = 0; i<c; i++){
					if(_n == connectionList[i].guid){
						connectionList.RemoveAt(i);
						return;
					}
				}
			}
		}
	}

	[RPC]
	void Ready(bool _rdy, string _guid, NetworkMessageInfo info) {
		//do i need to search?
		if(Network.isClient){
			if(info.sender.guid == server){
				int c = connectionList.Count;
				for(int i = 0; i<c; i++){
					if(_guid == connectionList[i].guid){
						connectionList[i].ready = _rdy;
						break;
					}
				}
			}
		}
		if(Network.isServer){
			if(info.sender.guid == _guid){
				int c = connectionList.Count;
				for(int i = 0; i<c; i++){
					if(_guid == connectionList[i].guid){
						connectionList[i].ready = _rdy;
						break;
					}
				}
				networkView.RPC("Ready",RPCMode.Others,_rdy,_guid);
				if(_rdy){
					ready++;
					if(ready >= 2 && ready == connectionList.Count){
						networkView.RPC("LoadMap",RPCMode.All);
					}
				} else {
					ready--;
				}
			}
		}
	}

	[RPC]
	void LoadMap(NetworkMessageInfo info){
		Debug.Log(info.sender.guid + " " + server);
		//if(info.sender.guid == server){
		//TODO : fuck unity rpcs again.
			if(Network.isClient){
				Debug.Log("Load Map" + info);
			} else if (Network.isServer){
				Debug.Log("Load Map" + info);
			}
			Application.LoadLevel(1);
		//}
	}
}
