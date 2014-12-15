//------------------------------//
//  HexWorld.cs                 //
//  Written by Alucard Jay      //
//  2014/9/5                    //
//------------------------------//


using UnityEngine;
using System.Collections;

public class HexWorld : MonoBehaviour 
{
	public HexChunk hexChunkPrefab;
	public Terrain terrain;

	public Game game;
	public Vector2 worldSize = new Vector2( 100, 100 );
	public int chunkSize = 50;
	
	public float tileRadius = 2f; // size of each tile in the grid
	public float hexRadius = 1.95f; // size of hexagon in the grid (smaller to have space between each hexagon)

	public float offsetY = 0.01f; // Y offset for tile, combat z-fighting with terrain (or use overlay shader)
	
	
	public HexData[,] hexWorldData;
	private HexChunk[,] hexChunks;

	private bool isInitialized = false;


	//	-------------------------------------------------------  Persistent Functions
	
	public class HexData{
		public int x = -1;
		public int y = -1;
		public int hexColor = 0;
		public float height = 0f;
		public string unit = null;
		public GameObject unitObject = null;
		public Vector2 center = Vector2.zero;
	};

	void Awake() 
	{
		Initialize();
	}


	void Start() 
	{
		isInitialized = false;
		Debug.Log ("Run");
		StartCoroutine( "GenerateHexWorld" );
	}
	
	
	//	-------------------------------------------------------  Other Functions
	
	
	void Initialize() 
	{
		if ( !hexChunkPrefab )
			Debug.LogError( gameObject.name + " : NO hexChunkPrefab ASSIGNED IN THE INSPECTOR" );

		if ( !terrain )
			Debug.LogError( gameObject.name + " : NO terrain ASSIGNED IN THE INSPECTOR" );

		// check hexRadius is not greater than tileRadius (stops overlapping hexagons)
		if ( hexRadius > tileRadius )
			hexRadius = tileRadius;
		
		// check chunk size doesn't exceed max allowed vertices
		if ( chunkSize > 50 )
			chunkSize = 50;
		
		// check worldSize values are integers
		worldSize.x = Mathf.RoundToInt( worldSize.x );
		worldSize.y = Mathf.RoundToInt( worldSize.y );

		// create a data array to store the texture index value of each hexagon
		hexWorldData = new HexData[(int)worldSize.x,(int)worldSize.y];
		float xoffset = 0.138f;
		float xdiameter = Mathf.Sqrt(3f);
		float xoffsetodd = (xdiameter/2f)+xoffset;
		float yoffset = 1f;
		float ydiameter = 1.5f;
		for(int i = 0; i < (int)worldSize.x; i++){
			for(int j = 0; j < (int)worldSize.y; j++){
				hexWorldData[i,j] = new HexData();
				if(j%2 == 1)
					hexWorldData[i,j].center.x = (xdiameter*i)+xdiameter+xoffset;
				else 
					hexWorldData[i,j].center.x = (xdiameter*i)+xoffsetodd;
				hexWorldData[i,j].center.y = (ydiameter*j)+yoffset;
				hexWorldData[i,j].x = i;
				hexWorldData[i,j].y = j;
				float num = terrain.SampleHeight(new Vector3(hexWorldData[i,j].center.x,0,hexWorldData[i,j].center.y));
				hexWorldData[i,j].height = Mathf.Round(num * 2) / 2;
			}
		}
	}
	

	IEnumerator GenerateHexWorld()
	{
		int chunksX = Mathf.FloorToInt( worldSize.x / (float)chunkSize );
		int chunksZ = Mathf.FloorToInt( worldSize.y / (float)chunkSize );

		hexChunks = new HexChunk[ chunksX, chunksZ ];

		Vector3 chunkPos;
		HexChunk hexChunk;
		
		for ( int z = 0; z < chunksZ; z ++ ) 
		{
			for ( int x = 0; x < chunksX; x ++ ) 
			{
				chunkPos.x = (float)x * (float)chunkSize * Mathf.Cos( 30f * Mathf.Deg2Rad ) * tileRadius;
				chunkPos.y = 0;
				chunkPos.z = (float)z * (float)chunkSize * 1.5f * Mathf.Sin( 30f * Mathf.Deg2Rad ) * tileRadius;

				hexChunk = (HexChunk)Instantiate( hexChunkPrefab, chunkPos, Quaternion.identity );

				hexChunk.gameObject.name = hexChunkPrefab.name + "_" + x.ToString() + "_" + z.ToString();
				hexChunk.transform.parent = transform;

				hexChunk.chunkSize = chunkSize;
				hexChunk.tileRadius = tileRadius;
				hexChunk.hexRadius = hexRadius;
				hexChunk.offsetY = offsetY;

				hexChunk.hexWorld = this;

				hexChunks[ x, z ] = hexChunk;

				// stagger to avoid bottleneck
				yield return new WaitForEndOfFrame();
			}
		}

		isInitialized = true;
		
		// debug for testing
		if ( Application.isEditor )
			Debug.Log( "HexWorld Mesh Generation Complete" );
	}
	
	
	//	-------------------------------------------------------  UV Modifying Functions
	

	public Vector2 findHex (Vector3 hitPoint){
		if ( !isInitialized )
			return -Vector2.one;
		
		
		float offsety = hitPoint.z;
		float offsetx = hitPoint.x-0.138f;
		
		float magicbase = Mathf.Sqrt (3f);
		float magic = magicbase*0.5f;
		float zone = (offsetx)/magic; //normalized, 0->1 as you cross through the x axis
		float happy = zone%2;
		float tempx = 0;
		float zonez = offsety%3f;
		
		// x==x && y==z
		
		int hexx = -10;
		int hexy = -10;
		
		if(zonez >= 0f && zonez <= 0.5f){
			if(happy<1){
				tempx = 0.5f*(2-happy)-0.5f;
			} else {
				tempx = 0.5f*happy-0.5f;
			}
			if(tempx < zonez){
				//inside
				hexx = Mathf.FloorToInt(offsetx/magicbase);
				hexy = Mathf.FloorToInt(offsety/1.5f);
			} else {
				//outside
				hexx = Mathf.FloorToInt((offsetx-magic)/magicbase);
				hexy = Mathf.FloorToInt(offsety/1.5f)-1;
			}
		} else if (zonez > 0.5f && zonez <= 1.5f){
			hexx = Mathf.FloorToInt(offsetx/magicbase);
			hexy = Mathf.FloorToInt(offsety/1.5f);
		} else if (zonez > 1.5f && zonez <= 2f){
			happy=(zone+1)%2;
			if(happy<1){
				tempx = 0.5f*(2-happy)-0.5f;
				
			} else {
				tempx = 0.5f*happy-0.5f;
			}
			if(tempx < zonez-1.5f){
				//inside
				hexx = Mathf.FloorToInt((offsetx-magic)/magicbase);
				hexy = Mathf.FloorToInt(offsety/1.5f);
			} else {
				//outside
				hexx = Mathf.FloorToInt(offsetx/magicbase);
				hexy = Mathf.FloorToInt(offsety/1.5f)-1;
			}
		} else if (zonez > 2f && zonez <= 3f){
			hexx = Mathf.FloorToInt((offsetx-magic)/magicbase);
			hexy = Mathf.FloorToInt(offsety/1.5f);
		}

		if(hexx <0 || hexy <0 || hexx >= worldSize.x || hexy >= worldSize.y)
			return -Vector2.one;

		// debug for testing
		int cX = Mathf.FloorToInt( hexx / chunkSize );
		int cY = Mathf.FloorToInt( hexy / chunkSize );
		
		
		// where is it relative to the chunk?
		
		int rX = hexx - ( chunkSize * cX );
		int rY = hexy - ( chunkSize * cY );
		if ( Application.isEditor ) {
			//Debug.Log( "world pos " + hexx + " " + hexy + " : chunk " + cX + " " + cY + " : chunk pos " + rX + " " + rY );
		}

		return new Vector2(hexx,hexy);
	}

	public Vector2 SetHexUVs( Vector2 hex, int i ) 
	{
		// check if initialized

		int hexx = Mathf.FloorToInt(hex.x);
		int hexy = Mathf.FloorToInt(hex.y);

		if(hexx <0 || hexy <0 || hexx >= worldSize.x || hexy >= worldSize.y)
			return -Vector2.one;

		// what chunk is it in?

		int cX = Mathf.FloorToInt( hexx / chunkSize );
		int cY = Mathf.FloorToInt( hexy / chunkSize );


		// where is it relative to the chunk?

		int rX = hexx - ( chunkSize * cX );
		int rY = hexy - ( chunkSize * cY );
		
		// check if in range 


		// if the value for i is -1, set the hexagon back to the last texture index value assigned
		if ( i == -1 )
		{
			i = hexWorldData[hexx,hexy].hexColor;

		}


		// update hexWorldData with new value if not value for highlighted
		if ( i != 1 )
		{
			hexWorldData[hexx,hexy].hexColor = i;
		}


		// tell the chunk to update UVs for selected tile
		HexChunk currChunk = hexChunks[ cX, cY ];
		currChunk.SetHexUVs( rX, rY, i );
		
		// return the data coordinates of this hexagon
		return new Vector2( hex.x, hex.y );
	}
}