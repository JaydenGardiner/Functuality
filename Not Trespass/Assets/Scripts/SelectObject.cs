﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectObject : MonoBehaviour {
    public Text m_T;

    BoardManager board;
	// Use this for initialization
	void Start () {
        board = FindObjectOfType<BoardManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // || Input.GetMouseButtonDown(0)
	    if(Input.touchCount > 0)
        {
            Touch curTouch = Input.GetTouch(0);
            switch(curTouch.phase)
            {
                case TouchPhase.Ended:
                    if (curTouch.tapCount == 1)
                    {
                        RaycastHit hit;
                        Ray worldPos = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        if (Physics.Raycast(worldPos, out hit, Mathf.Infinity))
                        {
                            board.currentPiece = hit.transform.parent.gameObject.GetComponent<Piece>();
                            board.RestoreAllTiles();
                            board.FindMovementOptions();
                            Debug.Log("hit piece");
                            Debug.Log(hit.transform.parent.gameObject.name);
                            //m_T.text = "HIT PIECE";
                        }
                    }
                    
                    break;
            }
        }
	}
}
