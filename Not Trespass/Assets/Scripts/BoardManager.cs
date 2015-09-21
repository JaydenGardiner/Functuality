﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoardManager : MonoBehaviour {

    //Prefab for instantiating piece, set in scene
    public GameObject PiecePrefab;

    //this allows me to set the tiles up in the scene and access them from code
    public GameObject[] tiles;

 
    /* In row column order where the rows go downwards
     * 5 6 7 8 9  r=0
     * 0 1 2 3 4  r=1
     * - - - - -  r=2
     * - - - - -  r=3
     * 0 1 2 3 4  r=4
     * 5 6 7 8 9  r=5
     */
    //Use this for 2d array
    public Tile[,] Tiles2D;

    void Awake()
    {
        //Add tile script to all tiles.  This is done in awake to ensure that by Start(), all tiles have Tile script
        foreach (GameObject t in tiles)
        {
            t.AddComponent<Tile>();
        }
    }

	// Use this for initialization
	void Start () {
        
        //Create 2d arrays of positions and game objects and instantiate pieces
        Tiles2D = new Tile[6, 5];
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                //Convert 1d tile array from scene to 2d representation.
                //Assumes tiles are ordered in scene, probably not best implementation but w.e
                GameObject tile = tiles[(i * 5) + j];
                Tiles2D[i, j] = tile.gameObject.GetComponent<Tile>();

                //Instantiate pieces on tiles.
                if (i == 0 || i == 1 || i == 5 || i == 4)
                {
                    //Instantiate piece on center of tile
                    Vector3 center = tile.gameObject.GetComponentInChildren<MeshRenderer>().bounds.center;
                    Object n_Obj = GameObject.Instantiate(PiecePrefab, center, Quaternion.identity);
                    GameObject n_GameObj = (GameObject)n_Obj;
                    //needs completion to center piece on square b/c unity instantiates corner of piece on center of tile
                    n_GameObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    n_GameObj.transform.Rotate(new Vector3(90, 0, 0));
                    n_GameObj.transform.Translate(0, 0, -2);
                    n_GameObj.AddComponent<MeshCollider>();
                    //Set tile's piece
                    Tiles2D[i, j].Piece = n_GameObj.GetComponent<PieceMovement>();
                    //Instantiate virtual board location
                    Tiles2D[i, j].I = i;
                    Tiles2D[i, j].J = j;
                    Debug.Log("this: " + center.ToString() + ", tile: " + Tiles2D[i, j].Location);
                }
                else
                {
                    Tiles2D[i, j].Piece = null;
                }
                
            }
        }

	}
	
	// Update is called once per frame
	void Update () {

	}
}
