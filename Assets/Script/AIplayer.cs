using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIplayer : MonoBehaviour
{
    public BoardManage.BoardPieceColor pieceColor;//AIִ����ɫ
    
    public AIMove AIMove;
    //��ȡ������Ϣ
    public BoardManage board;
    // Start is called before the first frame update

    private void Awake()
    {
        //��ȡ�ƶ����
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
            //�������ϴ������Ӳ��ҷ���

        }
        return true;
    }


    //����ʾ�õ�
    public bool AICreatePoint(BoardManage board,BoardManage.BoardPieceColor pieceColor)
    {
        BoardManage.BoardPieceColor AIpiece = this.pieceColor;//��¼һ�µ�ǰ��AI��ɫ ���Ḵԭ
        SetAIPieceColor(pieceColor);
        int pos = AIMove.CreatePiece(board);
        //���껹ԭ
        SetAIPieceColor(AIpiece);
        if (pos==-1)
        {
            return false;
        }
        else
        {
            //����һ��ָ������ɫ
            board.CreatePiece(pos/8,pos%8, BoardManage.BoardPieceColor.NULLPoint);
        }
        return true;

    }
}
