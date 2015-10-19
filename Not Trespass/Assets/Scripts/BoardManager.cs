﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoardManager : MonoBehaviour {

    //Prefab for instantiating piece, set in scene
    public GameObject PiecePrefab;

    //this allows me to set the tiles up in the scene and access them from code
    public GameObject[] tiles;

    //Get these from other stuff, just initialising for now.
    public GamePlayer Player1;
    public GamePlayer Player2;

    public int Player1Secret;
    public int Player2Secret;

    //the currently selected piece
    public Piece currentPiece;

    public int CurrentTeam;


    /* In row column order where the rows go downwards
     * 5 6 7 8 9  r=0
     * 0 1 2 3 4  r=1
     * - - - - -  r=2
     * - - - - -  r=3
     * 0 1 2 3 4  r=4
     * 5 6 7 8 9  r=5
     */
    //Use this for 2d array, is in row, column order
    public Tile[,] Tiles2D;

    private bool m_ZeroWins;
    private bool m_OneWins;
    

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
        m_OneWins = false;
        m_ZeroWins = false;
        //Create 2d arrays of positions and game objects and instantiate pieces
        Tiles2D = new Tile[6, 5];
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                //Convert 1d tile array from scene to 2d representation.
                //Assumes tiles are ordered in scene, probably not best implementation but w.e
                GameObject tile = tiles[(i * 5) + j];
                tile.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<BoxCollider>();
                Tiles2D[i, j] = tile.gameObject.GetComponent<Tile>();

                //Instantiate virtual board location
                Tiles2D[i, j].I = i;
                Tiles2D[i, j].J = j;
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
                    //for collision
                    n_GameObj.tag = "piece";
                    //Set tile's piece
                    Tiles2D[i, j].Piece = n_GameObj.GetComponent<Piece>();
                    Tiles2D[i, j].Piece.Tile = Tiles2D[i, j];
                    Tiles2D[i, j].Piece.transform.Find("Piece").GetComponent<Renderer>().material.SetColor("_TintColor", Color.red);
                    

                    //Set team for each piece
                    if (i == 0 || i == 1)
                    {
                        Tiles2D[i, j].Piece.Team = 0;
                    }
                    else
                    {
                        Tiles2D[i, j].Piece.Team = 1;
                    }
                    Debug.Log("this: " + center.ToString() + ", tile: " + Tiles2D[i, j].Location);
                }
                else
                {
                    Tiles2D[i, j].Piece = null;
                }
                
            }
        }
        //random initilization, testing ideas
        Player1 = new GamePlayer(new Player("John"), 0, true);
        Player2 = new GamePlayer(new Player("Cena"), 1, false);
        CurrentTeam = 0;
        Tiles2D[0, 0].Piece.IsSecret = true;
        Tiles2D[5, 0].Piece.IsSecret = true;
	}
	
	// Update is called once per frame
	void Update () {
        foreach(Tile t in Tiles2D)
        {
            Piece p = t.Piece;
            if (p != null && p.IsSecret && (p.Team == 0) && (t.I == 0))
            {
                m_ZeroWins = true;
            }
            else if (p != null && p.IsSecret && (p.Team == 1) && (t.I == 5))
            {
                m_OneWins = true;
            }
        }
        if (m_OneWins)
        {
            //Application.Quit();
        }
        if (m_ZeroWins)
        {
            //Application.Quit();
        }
	}

    public void UpdateBoard(Piece[,] n_pieces)
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Tiles2D[i, j].Piece = n_pieces[i, j];
            }
        }
    }

    public void ChangeTurn()
    {
        if (CurrentTeam == 0)
        {
            CurrentTeam = 1;
        }
        else
        {
            CurrentTeam = 0;
        }
    }


    public void RestoreAllTiles()
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                Tiles2D[i, j].Dehighlight();
            }
        }

    }
    // Displays tiles that the current piece ca move to
    public void FindMovementOptions()
    {
        Piece p = currentPiece;
        //multiplier to keep pieces moving "forward"
        int side = 1;
        if (p.Team == 1) side = -1;
        Debug.Log("finding movement options");
        int[,] marked = new int[6, 5];
        Stack<Tile> toVisit = new Stack<Tile>();
        int i = p.Tile.I;
        int j = p.Tile.J;
        while (i >= 0 && i < 6 && (Tiles2D[i, j].Piece == null || (i == p.Tile.I && j == p.Tile.J)))
        {
            toVisit.Push(Tiles2D[i, j]);
            int k = j + 1;
            while (k < 5 && Tiles2D[i, k].Piece == null)
            {
                toVisit.Push(Tiles2D[i, k]);
                k++;
            }
            k = j - 1;
            while (k >= 0 && Tiles2D[i, k].Piece == null)
            {
                toVisit.Push(Tiles2D[i, k]);
                k--;
            }
            i += side;
        }
        Debug.Log("highlighting tiles");
        while (toVisit.Count > 0)
        {
            Tile t = toVisit.Pop();
            t.Highlight();
            j = t.J;
            i = t.I;
            if (side == -1 && i > 1 && marked[i - 2, j] == 0 && Tiles2D[i - 1, j].Piece != null && Tiles2D[i - 2, j].Piece == null) toVisit.Push(Tiles2D[i - 2, j]);
            if (side == 1 && i < 4 && marked[i + 2, j] == 0 && Tiles2D[i + 1, j].Piece != null && Tiles2D[i + 2, j].Piece == null) toVisit.Push(Tiles2D[i + 2, j]);
            if (j > 1 && marked[i, j - 2] == 0 && Tiles2D[i, j - 1].Piece != null && Tiles2D[i, j - 2].Piece == null) toVisit.Push(Tiles2D[i, j - 2]);
            if (j < 3 && marked[i, j + 2] == 0 && Tiles2D[i, j + 1].Piece != null && Tiles2D[i, j + 2].Piece == null) toVisit.Push(Tiles2D[i, j + 2]);
            marked[i, j] = 1;
        }

        p.Tile.Dehighlight();
    }
}
//just some ideas
public struct GamePlayer
{
    public Player P { get; private set; }
    public int Team { get; private set; }

    public bool IsTurn { get; private set; }

    public void ChangeTurn()
    {
        IsTurn = !IsTurn;
    }

    public GamePlayer(Player nPlayer, int team, bool isStarting)
		: this()
    {
        P = nPlayer;
        Team = team;
        IsTurn = isStarting;
    }
}

