using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonkinBoard : MonoBehaviour
{
    //Locations of all pieces
    public Piece[] pieces = new Piece[45];
    //locations of all nodes on the Board
    private Vector2[] nodes = new Vector2[45];
    //Adjacency matrix
    private bool[,] matrix = new bool[45, 45];
    //Jagged array of lines for win condition
    private int[][] lines = new int[24][];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject redPiecePrefab;

    private Vector3 boardOffset = new Vector3(-3.8f, 0, -3.8f);

    private Piece selectedPiece;
    private Piece redPiece;

    private Vector2 mouseOver;
    private int startNode;
    private int endNode;

    //Team Information
    public Piece[] whitePieces = new Piece[10];
    public Piece[] blackPieces = new Piece[10];

    //Game information
    public bool winner = false;
    public string winnerTeam;

    //Turn Information
    public bool whiteTurn = true;
    private int whiteOpeningMoves = 0; //Changed to 10 after board generation
    private int blackOpeningMoves = 0;


    private void Start()
    {
        GenerateBoard();

        GenerateNodes();

        GenerateMatrix();

        GenerateLines();

        GameObject go = Instantiate(redPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        redPiece = go.GetComponent<Piece>();
        //MovePiece(redPiece, .5f, .5f);

    }

    private void Update()
    {
        //Get mouse position
        UpdateMouseOver();

        float x = mouseOver.x;
        float y = mouseOver.y;

        if (winner)
        {
            return;
        }
            

        //If mouse clicked down
        if (Input.GetMouseButtonDown(0))
        {
            startNode = GetClosestNode(x, y);

            if (pieces[startNode] != null && pieces[startNode].IsWhite() == whiteTurn)
            {
                float redX = nodes[startNode].x;
                float redY = nodes[startNode].y;
                MovePiece(redPiece, redX, redY);
                redPiece.gameObject.SetActive(true);
            }


            //If white turn
            if (whiteTurn)
            {
                //If opening move
                if (whiteOpeningMoves > 0)
                {
                    selectedPiece = whitePieces[10 - whiteOpeningMoves];
                    if (TryMove(selectedPiece, 46, startNode))
                    {
                        whiteTurn = !whiteTurn;
                        whiteOpeningMoves--;
                        redPiece.gameObject.SetActive(false);
                    }

                }


            }
            else //Black turn
            {
                if (blackOpeningMoves > 0)
                {
                    selectedPiece = blackPieces[10 - blackOpeningMoves];
                    if (TryMove(selectedPiece, 46, startNode))
                    {
                        whiteTurn = !whiteTurn;
                        blackOpeningMoves--;
                        redPiece.gameObject.SetActive(false);
                    }
                }
            }
            
        }
            

        //When mouse released, do regular move
        if (Input.GetMouseButtonUp(0))
        {
            endNode = GetClosestNode(x, y);
            if (pieces[startNode] != null && pieces[startNode].IsWhite() == whiteTurn)
            {
                selectedPiece = pieces[startNode];
                if (TryMove(selectedPiece, startNode, endNode))
                { 
                    CheckWin();
                    whiteTurn = !whiteTurn;
                    redPiece.gameObject.SetActive(false);
                }
            }
        }
           
    }
    private void UpdateMouseOver()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (hit.point.x - boardOffset.x) / 7.6f;
            mouseOver.y = (hit.point.z - boardOffset.z) / 7.6f;
          //Debug.Log(mouseOver);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private int GetClosestNode(float x, float y)
    {
        //Input: mouse positions
        int nodeIndex = -1;
        float shortestDistance = 99999f;

        for(int i = 0; i < 45; i++)
        {
            float currentDistance = Vector2.Distance(new Vector2(x, y), nodes[i]);
            if(shortestDistance > currentDistance)
            {
                shortestDistance = currentDistance;
                nodeIndex = i;
            }
        }
        //Debug.Log("Closest Node:" + startNode);

        return nodeIndex;

    }

    private bool TryMove(Piece p, int start, int end)
    {
        if (start != 46 && (whiteOpeningMoves > 0 || blackOpeningMoves > 0))
            return false;

        if (start == 46 && pieces[end] == null)
        {
            //Opening move, only chack destination
            pieces[end] = p;
            MovePiece(selectedPiece, nodes[end].x, nodes[end].y);
            return true;
        }
        else if (pieces[end] == null && matrix[start, end] == true)
        {
            pieces[end] = p;
            pieces[start] = null;
            MovePiece(selectedPiece, nodes[end].x, nodes[end].y);
            return true;
        }

        //Debug.Log("TryMove() Failed, destination: " + endNode);
        return false;
    }

    private void GenerateBoard()
    {
        //Create pieces
        for (int i = 0; i < 20; i++)
        {

            //Generate Piece
            GeneratePiece((i>9)? 1.4f: -0.4f, (float)(i%10)/8);
        }
    }
    private void GenerateNodes()
    {
        nodes[0] = new Vector2(0.5f, 0.5f);
        nodes[1] = new Vector2(0f, 0f);
        nodes[2] = new Vector2(0.25f, 0f);
        nodes[3] = new Vector2(0.5f, 0f);
        nodes[4] = new Vector2(0.75f, 0f);
        nodes[5] = new Vector2(1f, 0f);
        nodes[6] = new Vector2(1f, 0.25f);
        nodes[7] = new Vector2(1f, 0.5f);
        nodes[8] = new Vector2(1f, 0.75f);
        nodes[9] = new Vector2(1f, 1f);
        nodes[10] = new Vector2(0.75f, 1f);
        nodes[11] = new Vector2(0.5f, 1f);
        nodes[12] = new Vector2(0.25f, 1f);
        nodes[13] = new Vector2(0f, 1f);
        nodes[14] = new Vector2(0f, 0.75f);
        nodes[15] = new Vector2(0f, 0.5f);
        nodes[16] = new Vector2(0f, 0.25f);
        nodes[17] = new Vector2(0.25f, 0.25f);
        nodes[18] = new Vector2(0.375f, 0.25f);
        nodes[19] = new Vector2(0.5f, 0.25f);
        nodes[20] = new Vector2(0.625f, 0.25f);
        nodes[21] = new Vector2(0.75f, 0.25f);
        nodes[22] = new Vector2(0.75f, 0.375f);
        nodes[23] = new Vector2(0.75f, 0.5f);
        nodes[24] = new Vector2(0.75f, 0.625f);
        nodes[25] = new Vector2(0.75f, 0.75f);
        nodes[26] = new Vector2(0.625f, 0.75f);
        nodes[27] = new Vector2(0.5f, 0.75f);
        nodes[28] = new Vector2(0.375f, 0.75f);
        nodes[29] = new Vector2(0.25f, 0.75f);
        nodes[30] = new Vector2(0.25f, 0.625f);
        nodes[31] = new Vector2(0.25f, 0.5f);
        nodes[32] = new Vector2(0.25f, 0.375f);
        nodes[33] = new Vector2(0.125f, 0.125f);
        nodes[34] = new Vector2(0.875f, 0.125f);
        nodes[35] = new Vector2(0.875f, 0.875f);
        nodes[36] = new Vector2(0.125f, 0.875f);
        nodes[37] = new Vector2(0.16667f, 0.33333f);
        nodes[38] = new Vector2(0.33333f, 0.16667f);
        nodes[39] = new Vector2(0.66667f, 0.16667f);
        nodes[40] = new Vector2(0.83333f, 0.33333f);
        nodes[41] = new Vector2(0.83333f, 0.66667f);
        nodes[42] = new Vector2(0.66667f, 0.83333f);
        nodes[43] = new Vector2(0.33333f, 0.83333f);
        nodes[44] = new Vector2(0.16667f, 0.66667f);
    }
    private void GenerateMatrix() {

        matrix[0,17] = true; matrix[0,18] = true; matrix[0,19] = true; matrix[0,20] = true;
        matrix[0,21] = true; matrix[0,22] = true; matrix[0,23] = true; matrix[0,24] = true;
        matrix[0,25] = true; matrix[0,26] = true; matrix[0,27] = true; matrix[0,28] = true;
        matrix[0,29] = true; matrix[0,30] = true; matrix[0,31] = true; matrix[0,32] = true;

        matrix[1,2] = true; matrix[1,33] = true; matrix[1,16] = true;

        matrix[2,1] = true; matrix[2,33] = true; matrix[2,38] = true; matrix[2,3] = true;

        matrix[3,2] = true; matrix[3,38] = true; matrix[3,19] = true; matrix[3,39] = true; matrix[3,4] = true;

        matrix[4,3] = true; matrix[4,39] = true; matrix[4,34] = true; matrix[4,5] = true;

        matrix[5,4] = true; matrix[5,34] = true; matrix[5,6] = true;

        matrix[6,5] = true; matrix[6,34] = true; matrix[6,40] = true; matrix[6,7] = true;

        matrix[7,6] = true; matrix[7,40] = true; matrix[7,23] = true; matrix[7,41] = true; matrix[7,8] = true;

        matrix[8,7] = true; matrix[8,41] = true; matrix[8,35] = true; matrix[8,9] = true;

        matrix[9,8] = true; matrix[9,35] = true; matrix[9,10] = true;

        matrix[10,9] = true; matrix[10,35] = true; matrix[10,42] = true; matrix[10,11] = true;

        matrix[11,10] = true; matrix[11,42] = true; matrix[11,27] = true; matrix[11,43] = true; matrix[11,12] = true;

        matrix[12,11] = true; matrix[12,43] = true; matrix[12,36] = true; matrix[12,13] = true;

        matrix[13,12] = true; matrix[13,36] = true; matrix[13,14] = true;

        matrix[14,13] = true; matrix[14,36] = true; matrix[14,44] = true; matrix[14,15] = true;

        matrix[15,14] = true; matrix[15,44] = true; matrix[15,31] = true; matrix[15,37] = true; matrix[15,16] = true;

        matrix[16,15] = true; matrix[16,37] = true; matrix[16,33] = true; matrix[16,1] = true;

        matrix[17,37] = true; matrix[17,32] = true; matrix[17,0] = true; matrix[17,18] = true; matrix[17,38] = true; matrix[17,33] = true;

        matrix[18,0] = true; matrix[18,19] = true; matrix[18,38] = true; matrix[18,17] = true;

        matrix[19,0] = true; matrix[19,20] = true; matrix[19,3] = true; matrix[19,18] = true;

        matrix[20,0] = true; matrix[20,21] = true; matrix[20,39] = true; matrix[20,19] = true;

        matrix[21,0] = true; matrix[21,22] = true; matrix[21,40] = true; matrix[21,34] = true; matrix[21,39] = true; matrix[21,20] = true;

        matrix[22,0] = true; matrix[22,23] = true; matrix[22,40] = true; matrix[22,21] = true;

        matrix[23,24] = true; matrix[23,7] = true; matrix[23,22] = true; matrix[23,0] = true;

        matrix[24,25] = true; matrix[24,41] = true; matrix[24,23] = true; matrix[24,0] = true;

        matrix[25,42] = true; matrix[25,35] = true; matrix[25,41] = true; matrix[25,24] = true; matrix[25,0] = true; matrix[25,26] = true;

        matrix[26,42] = true; matrix[26,25] = true; matrix[26,0] = true; matrix[26,27] = true;

        matrix[27,11] = true; matrix[27,26] = true; matrix[27,0] = true; matrix[27,28] = true;

        matrix[28,43] = true; matrix[28,27] = true; matrix[28,0] = true; matrix[28,29] = true;

        matrix[29,36] = true; matrix[29,43] = true; matrix[29,28] = true; matrix[29,0] = true; matrix[29,30] = true; matrix[29,44] = true;

        matrix[30,44] = true; matrix[30,29] = true; matrix[30,0] = true; matrix[30,31] = true;

        matrix[31,30] = true; matrix[31,0] = true; matrix[31,32] = true; matrix[31,15] = true;

        matrix[32,31] = true; matrix[32,0] = true; matrix[32,17] = true; matrix[32,37] = true;

        matrix[33,16] = true; matrix[33,17] = true; matrix[33,2] = true; matrix[33,1] = true;

        matrix[34,21] = true; matrix[34,6] = true; matrix[34,5] = true; matrix[34,4] = true;

        matrix[35,10] = true; matrix[35,9] = true; matrix[35,8] = true; matrix[35,25] = true;

        matrix[36,13] = true; matrix[36,12] = true; matrix[36,29] = true; matrix[36,14] = true;

        matrix[37,15] = true; matrix[37,32] = true; matrix[37,17] = true; matrix[37,16] = true;

        matrix[38,17] = true; matrix[38,18] = true; matrix[38,3] = true; matrix[38,2] = true;

        matrix[39,20] = true; matrix[39,21] = true; matrix[39,4] = true; matrix[39,3] = true;

        matrix[40,22] = true; matrix[40,7] = true; matrix[40,6] = true; matrix[40,21] = true;

        matrix[41,25] = true; matrix[41,8] = true; matrix[41,7] = true; matrix[41,24] = true;

        matrix[42,11] = true; matrix[42,10] = true; matrix[42,25] = true; matrix[42,26] = true;

        matrix[43,12] = true; matrix[43,11] = true; matrix[43,28] = true; matrix[43,29] = true;

        matrix[44,14] = true; matrix[44,29] = true; matrix[44,30] = true; matrix[44,15] = true;

    }
    private void GenerateLines()
    {
        lines[0] = new int[] {13, 12, 11, 10, 9};
        lines[1] = new int[] {9, 8, 7, 6, 5};
        lines[2] = new int[] {5, 4, 3, 2, 1};
        lines[3] = new int[] {1, 16, 15, 14, 13};
        lines[4] = new int[] {29, 28, 27, 26, 25};
        lines[5] = new int[] {25, 24, 23, 22, 21};
        lines[6] = new int[] {21, 20, 19, 18, 17};
        lines[7] = new int[] {17, 32, 31, 30, 29};
        lines[8] = new int[] {15, 44, 29, 43, 11};
        lines[9] = new int[] {11, 42, 25, 40, 7};
        lines[10] = new int[] {7, 40, 21, 39, 3};
        lines[11] = new int[] {3, 38, 17, 37, 15};
        lines[12] = new int[] {14, 36, 12};
        lines[13] = new int[] {10, 35, 8};
        lines[14] = new int[] {6, 34, 4};
        lines[15] = new int[] {2, 33, 16};
        lines[16] = new int[] {13, 36, 29, 0, 21, 34, 5};
        lines[17] = new int[] {12, 43, 28, 0, 20, 39, 4};
        lines[18] = new int[] {11, 27, 0, 19, 3};
        lines[19] = new int[] {10, 42, 26, 0, 18, 38, 2};
        lines[20] = new int[] {9, 35, 25, 0, 17, 33, 1};
        lines[21] = new int[] {8, 41, 24, 0, 32, 37, 16};
        lines[22] = new int[] {7, 23, 0, 31, 15};
        lines[23] = new int[] {6, 40, 22, 0, 30, 44, 14};

    }    
    private void GeneratePiece(float x, float y)
    {

        bool isWhite = (x >= 0) ? false : true;
        GameObject go = Instantiate((isWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        p.SetColor((isWhite) ? "White": "Black");

        if (isWhite)
        {
            whitePieces[whiteOpeningMoves] = p;
            whiteOpeningMoves++;
        }
        else
        {
            blackPieces[blackOpeningMoves] = p;
            blackOpeningMoves++;
        }

        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, float x, float y)
    {
        p.transform.position = new Vector3(x-0.5f, 0f, y-0.5f) * 7.6f;
    }

    private void CheckWin()
    {
        if (whiteOpeningMoves > 0 || blackOpeningMoves > 0)
            return;

        winner = false;
        int i, j;
        Piece p = null;

        for (i = 0; i < lines.Length; i++)
        {

            int count = 0;
            int goal = lines[i].Length;
            
            string locString = string.Join(",", lines[i]);
            //Debug.Log("Checking " + locString);
            
            for (j = 0; j < lines[i].Length; j++)
            {
                p = pieces[lines[i][j]];
                if (p != null && p.IsWhite() == whiteTurn)
                {
                    count++;

                }
                else
                {
                    //Debug.Log("break on " + lines[i][j]);
                    break;
                }
            }
            
            if(count == goal)
            {
                winner = true;
                winnerTeam = p.GetColor();
                Debug.Log(p.GetColor() + " is the Winner!");
                break;
            }

        }
    }



}
