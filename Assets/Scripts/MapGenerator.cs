using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Transform StartGeneratePosition;
    public Tilemap Tilemap;
    public Tile _Tile;

    public List<Tile> GrassesTiles;
    public List<Tile> FlowesTitles;

    public List<GameObject> Stones;

    public int MapSize;

    private Tile previousTile;
    void Start()
    {
        GenerateMap();
    }
    

    private void GenerateMap()
    {
        previousTile = FlowesTitles[0];
        Tilemap = GetComponent<Tilemap>();
        for (int i = 0; i < MapSize; i++)
        {
            for (int j = 0; j < MapSize; j++)
            {
                Tile spawnTile;
                bool isBadTile = false;
                do
                {
                    spawnTile = Random.Range(1, 4)  != 3
                        ? GrassesTiles[Random.Range(0, GrassesTiles.Count)]
                        : FlowesTitles[Random.Range(0, FlowesTitles.Count)];
                    if (i < 1)
                        continue;
                    if (Tilemap.GetTile(new Vector3Int(i - 1, j, 0)) == spawnTile || previousTile == spawnTile)
                    {
                        isBadTile = true;
                    }
                    else
                    {
                        isBadTile = false;
                    }
                    
                } while (isBadTile);

                previousTile = spawnTile;

                
                Tilemap.SetTile(new Vector3Int(i, j, 0), spawnTile);


            }
        }
    }
}
