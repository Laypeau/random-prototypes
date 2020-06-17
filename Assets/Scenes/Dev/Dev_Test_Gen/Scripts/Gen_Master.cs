using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CustomLibrary;

public class Gen_Master : MonoBehaviour
{
	public Transform focus;
	public GameObject prefab_Tile;
	[Range(1, 5)] public int radius = 2;
	private Vector2Int[] activeTiles;
	private ArrayList existentTiles = new ArrayList();
	private Vector2 cellPos;

	[Header("Tile Settings")]
	public float tileSize;
	public int tileSegments;
	public float noiseScale;
	public float noiseHeight;

	void Awake()
	{
		if (focus == null || prefab_Tile == null)
			throw new UnityException("Focus/tile not set");
			
		prefab_Tile.GetComponent<Gen_Tile>().length = Vector2.one * tileSize;
		prefab_Tile.GetComponent<Gen_Tile>().segments = Vector2Int.one * tileSegments;
		prefab_Tile.GetComponent<Gen_Tile>().noiseScale = noiseScale;
		prefab_Tile.GetComponent<Gen_Tile>().noiseHeight = noiseHeight;

		activeTiles = new Vector2Int[radius * radius * 4];
	}

	void Start()
	{
		cellPos = General.Vector2Round(new Vector2(focus.position.x, focus.position.z), tileSize);
		Init();
	}

	void Update()
	{
		var currentRoundedPos = General.Vector2Round(new Vector2(focus.position.x, focus.position.z), tileSize);
		if (currentRoundedPos != cellPos) //rounded pos crosses cell
		{
			cellPos = General.Vector2Round(new Vector2(focus.position.x, focus.position.z), tileSize);
			Recalculate();
		}
	}

	void Init()
	{
		//Create all

		activeTiles = new Vector2Int[radius * radius * 4];
		int i = 0;
		for (int z = -radius; z < radius; z++)
		{
			for (int x = -radius; x < radius; x++)
			{
				activeTiles[i] = new Vector2Int((int)(Mathf.Floor((focus.position.x / tileSize) + 0.5f) + x), (int)(Mathf.Floor((focus.position.x / tileSize) + 0.5f) + z)); //could potentially cause problems casting as an int
				existentTiles.Add(activeTiles[i]);
				CreateTile(activeTiles[i].ToString(), focus.position + new Vector3(tileSize * activeTiles[i].x, 0f, tileSize * activeTiles[i].y));
				i++;
			}
		}
	}

	void Recalculate()
	{
		//Recalculate activetiles based on focus pos
		activeTiles = new Vector2Int[radius * radius * 4];
		int i = 0;
		for(int z = -radius; z < radius; z++)
		{
			for (int x = -radius; x < radius; x++) 
			{
				activeTiles[i] = new Vector2Int((int)(Mathf.Floor((focus.position.x / tileSize) + 0.5f) + x), (int)(Mathf.Floor((focus.position.z / tileSize) + 0.5f) + z));
				i++;
			}
		}

		// Chack if any existing tiles should be reactivated/deactivated
		foreach(Vector2Int existentTile in existentTiles)
		{
			if(activeTiles.Contains(existentTile))
				transform.Find(existentTile.ToString()).gameObject.SetActive(true);
			else
				transform.Find(existentTile.ToString()).gameObject.SetActive(false);
		}

		// Create a new tile if it doesnt already exist
		foreach(Vector2Int activeTile in activeTiles)
		{
			if (existentTiles.Contains(activeTile) == false)
			{
				CreateTile(activeTile.ToString(), new Vector3(tileSize * activeTile.x, 0f, tileSize * activeTile.y));
				existentTiles.Add(activeTile);
			}
		}
	}

	private void CreateTile(string name, Vector3 position)
	{
		GameObject tile = Instantiate(prefab_Tile);
		tile.transform.parent = transform;
		tile.name = name;
		tile.transform.position = position;

		tile.GetComponent<Gen_Tile>().Remesh();
	}
}
