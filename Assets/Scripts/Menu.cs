using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Menu : MonoBehaviour {

	static Menu _instance;

	public string IP = "127.0.0.1";
	public int Port = 25001;

	public int ready = 0;

	class NConn
	{
		public string username { get; set; }
		public string guid { get; set; }
		public bool ready = false;

		//Other properties, methods, events...
		public NConn(string uname){
			username = uname;
		}
		public NConn(){
		}

	};

	NConn me;

	List<NConn> connectionList = new List<NConn>{};

	protected string username = "";
	protected string type = "none";

	void OnGUI() {
		int i = 0;
		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:
			if(type == "none"){
				if(GUI.Button(new Rect(100,100,100,25),"Start Client")){
					type = "client";
				}
				if(GUI.Button(new Rect(100,125,100,25),"Start Server")){
					connectionList.Clear();
					ready = 0;
					Network.InitializeServer(10,Port);
				}
			} else if(type == "client"){
				username = GUI.TextArea(new Rect(100,125,110,25),username);
				
				if(GUI.Button(new Rect(100,150,110,25),"Connect")){
					me = new NConn(username);
					connectionList.Clear();
					ready = 0;
					Network.Connect(IP,Port);
				}
				//TODO: add backbutton
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
				networkView.RPC("Ready",RPCMode.All,me.ready, me.guid);
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
	void OnConnectedToServer(){
		//called on client when client connects
		networkView.RPC("Initialize",RPCMode.Server,username);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info){
		//called on client when client disconnects, and on server when connection has disconnected.
		Debug.Log("Disconnected from server: " + info);
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		//called on server when NetworkPlayer player connects
		//add to list
		NConn p = new NConn();
		p.guid = player.guid;
		connectionList.Add(p);
		//broadcast client addition.
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		//called on server when NetworkPlayer player disconnects
		int c = connectionList.Count;
		string p;
		for(int i = 0; i<c; i++){
			if(player.guid == connectionList[i].guid){
				p = connectionList[i].guid;
				if(connectionList[i].ready){
					ready--;
				}
				connectionList.RemoveAt(i);
				//broadcast client removal.
				Network.RemoveRPCs(player);
				networkView.RPC("PlayerDisconnected",RPCMode.All,p);
				break;
			}
		}
	}

	[RPC]
	void Initialize(string input, NetworkMessageInfo info) {
		//client calls to update username on server side.
		if(Network.isServer){
			int c = connectionList.Count;
			for(int i = 0; i<c; i++){
				if(info.sender.guid == connectionList[i].guid){
					connectionList[i].username = input;
					networkView.RPC("Initialize",info.sender,info.sender.guid);
					networkView.RPC("PlayerConnected",RPCMode.Others,connectionList[i].username,connectionList[i].guid);
					return;
				}
			}
		} else if(Network.isClient){
			me.guid = input;
			networkView.RPC("RequestUsers",RPCMode.Server);
		}
	}

	[RPC]
	void RequestUsers(NetworkMessageInfo info) {
		//client calls to update username on server side.
		if(Network.isServer){
			int c = connectionList.Count;
			for(int i = 0; i<c; i++){
				networkView.RPC("PlayerConnected",info.sender,connectionList[i].username,connectionList[i].guid);
			}
		}
	}

	[RPC]
	void PlayerConnected(string username, string guid, NetworkMessageInfo info){
		if(Network.isClient){
			for(int i = 0; i<connectionList.Count; i++){
				if(connectionList[i].guid == guid){
					return;
				}
			}
			NConn p = new NConn(username);
			p.guid = guid;
			connectionList.Add(p);
		}
	}

	[RPC]
	void PlayerDisconnected(string n, NetworkMessageInfo info){
		if(Network.isClient){
			int c = connectionList.Count;
			for(int i = 0; i<c; i++){
				if(n == connectionList[i].guid){
					connectionList.RemoveAt(i);
					return;
				}
			}
		}
	}

	[RPC]
	void Ready(bool rdy, string guid, NetworkMessageInfo info) {
		//do i need to search?
		int c = connectionList.Count;
		for(int i = 0; i<c; i++){
			if(guid == connectionList[i].guid){
				connectionList[i].ready = rdy;
				break;
			}
		}
		if(Network.isServer){
			if(rdy){
				ready++;
				if(ready > 2 && ready > connectionList.Count){
					networkView.RPC("LoadMap",RPCMode.All);
				}
			} else {
				ready--;
			}
		}
	}

	[RPC]
	void LoadMap(){
		if(Network.isClient){

		} else if (Network.isServer){

		}
	}
}
