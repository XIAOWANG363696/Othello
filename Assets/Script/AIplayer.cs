using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIplayer : MonoBehaviour
{
    public BoardManage.BoardPieceColor pieceColor;//AI执子颜色
    
    public AIMove AIMove;
    //获取棋盘信息
    public BoardManage board;
    // Start is called before the first frame update

    private void Awake()
    {
        //获取移动组件
        AIMove = GetComponent<AIMove>();
        board = GameObject.Find("board").GetComponent<BoardManage>();
    }
    void Start()
    {
       
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAIPieceColor(BoardManage.BoardPieceColor boardPieceColor)
    {
        pieceColor = boardPieceColor;
        AIMove.AIpieceType = pieceColor;
    }

    public bool AICreatePiece(BoardManage board)
    {
        int pos = AIMove.CreatePiece(board);
        if (pos == -1)
        {
            
            return false;
        }
        else
        {
            board.CreatePiece(pos, pieceColor);
            //在棋盘上创建棋子并且返回

        }
        return true;
    }


    //做提示用的
    public bool AICreatePoint(BoardManage board,BoardManage.BoardPieceColor pieceColor)
    {
        BoardManage.BoardPieceColor AIpiece = this.pieceColor;//记录一下当前的AI颜色 待会复原
        SetAIPieceColor(pieceColor);
        int pos = AIMove.CreatePiece(board);
        //算完还原
        SetAIPieceColor(AIpiece);
        if (pos==-1)
        {
            return false;
        }
        else
        {
            //创建一个指向性颜色
            board.CreatePiece(pos/8,pos%8, BoardManage.BoardPieceColor.NULLPoint);
        }
        return true;

    }
}
