using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static BoardManage;

public class BoardManage : MonoBehaviour
{
    public enum BoardPieceColor
    {
        Black,
        White,
        NULL,
        NULLPoint,
    }

    //����Դ���������ÿ�����ӵĿ��
    public Vector3 zeroPointPosition;
    public float cellWidth;  

    //�洢�������� ��������
    public int CntBlack, CntWhite;
    //�洢ȫ�����ӵ����꣬�����ж������Ƿ�����
    public HashSet<Tuple<int, int>> AllPiece = new HashSet<Tuple<int, int>>();
    //�洢Ŀǰ��Ѱ���ӵ����꣬���ж���Щ�����ܹ���ת
    public HashSet<Tuple<int, int>> NowPiece = new HashSet<Tuple<int, int>>();
    //���ڼ�¼ÿ��ÿ����Ѱ������
    HashSet<Tuple<int, int>> temp = new HashSet<Tuple<int, int>>();



    //���̳���
    public const int boardLength = 8;

    //ÿ��λ�������̵�״̬
    public BoardPieceColor[,] BoardPiece = new BoardPieceColor[boardLength, boardLength];
    public GameObject[,]  boardGameObject = new GameObject[boardLength, boardLength];

    //׼�����ӵ�ģ��
    public GameObject black_Piece;
    public GameObject white_Piece;
    public GameObject ava_Piece;
    public GameObject ava2_Piece;
    public GameObject ava3_Piece;
    
    public Sprite black_Renderer;
    public Sprite white_Renderer;

    
    public PreBoard preBoard;

    // Start is called before the first frame update
    //���̳�ʼ��
    public GameObject PosPoint;
    void Start()
    {
        //�����̽���һ����ʼ��

        Init();
      
    }

    // Update is called once per frame
    void Init()
    {
        for (int i = 0; i < BoardPiece.GetLength(0); i++)
        {
            for (int j = 0; j < BoardPiece.GetLength(1); j++)
            {
                BoardPiece[i, j] = BoardPieceColor.NULL;
            }
        }


        CreatePiece(3, 4, BoardPieceColor.White);
        CreatePiece(4, 3, BoardPieceColor.White);
        CreatePiece(3, 3, BoardPieceColor.Black);
        CreatePiece(4, 4, BoardPieceColor.Black);

        //��ȡͼƬ
        black_Renderer = black_Piece.GetComponent<SpriteRenderer>().sprite;
        white_Renderer = white_Piece.GetComponent<SpriteRenderer>().sprite;

        //��ȡ����Ԥ�жϽű�
       

        preBoard.FindAva_CreatePiece(BoardManage.BoardPieceColor.Black);

         


    }
    private void Awake()
    {
         preBoard = GetComponent<PreBoard>();
    }

    void Update()
    {
        
    }
   

    //�߽���
    public bool Check_Pos(int row,int col)
    {
        if (row >= boardLength || col >= boardLength || row < 0 || col < 0)
        {
             Debug.Log("�����߽磬���ܷ�");
            return false;
           
        }
        if (BoardPiece[row, col] != BoardPieceColor.NULL)
        {
            Debug.Log("�Ѿ����ڣ����ܷ�");
            return false;
            
        }
        return true;
    }

    //�����õ������ܷ����ٷ�תһ��
    // ���ü�⣬���flag= 0,��Pre�����ж�ÿ�������Ƿ��ܷ��õ�
   //ͬʱ����Ҫ��ת�ļ���ALLpiece��


    public bool Check_LegAddAllPiece(int row,int col,BoardPieceColor boardPieceColor)
    {
        AllPiece.Clear();
        int[,] dis= new int[8, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        for(int i = 0;i<8;i++)
        {
            int flag = 0;
            for(int j = 1;j<8;j++)
            {
                int now_row = row + j * dis[i, 0];
                int now_col = col + j * dis[i, 1];
                //�����߽�
                if (now_row >= boardLength || now_col >= boardLength || now_col < 0 || now_row < 0) break;
                //�հ�
                else if (BoardPiece[now_row, now_col] == BoardPieceColor.NULL) break;
                //ͬɫ
                else if (BoardPiece[now_row, now_col] == boardPieceColor)
                {
                    flag = 1;
                    break;
                }
                temp.Add(new Tuple<int, int>(now_row, now_col));
                
            }
            if(flag==1&&temp.Count!=0)
            {
                AllPiece.UnionWith(temp);//�����Ԫ�ؿ��Ժϲ� ���кϲ�
               
            } 
            temp.Clear();
        }
        if (AllPiece.Count == 0)
        {
            //Debug.Log("���λ�ò��У��²����壬����");
            return false;
        }
        return true;
    }

    public bool Check_CanPut(int row, int col ,BoardPieceColor boardPieceColor)
    {
       
        int[,] dis = new int[8, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        for (int i = 0; i < 8; i++)
        {
            int flag = 0;
            int cnt = 0;
            for (int j = 1; j < 8; j++)
            {
                int now_row = row + j * dis[i, 0];
                int now_col = col + j * dis[i, 1];
                //�����߽�
                if (now_row >= boardLength || now_col >= boardLength || now_col < 0 || now_row < 0) break;
                //�հ�
                else if (BoardPiece[now_row, now_col] == BoardPieceColor.NULL) break;
                //ͬɫ
                else if (BoardPiece[now_row, now_col] == boardPieceColor)
                {
                    flag = 1;
                    break;
                }
                cnt++;
                

            }
            if (flag == 1&&cnt>0)
            {
                return true;

            }
           
        }
        return false;
    }


    //���������Ƿ�ɹ�
    public bool CreatePiece(BoardPieceColor boardPieceColor,Vector3 mousePosition)
    {
        
        ///��������Ҫ�任λ��-------------------
        //AllPiece.Clear();
        ///��������Ҫ�任λ��-------------------
        ///�о����Բ��û��ˡ�����Ŀǰû�����⡣��



        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);//����Ļ����ת��Ϊ��ǰ��������
        Vector3 offsetPos = mousePos - zeroPointPosition;//�õ�����λ��������ԭ��ľ���
        int row = (int)Mathf.Floor(offsetPos.x / cellWidth);
        int col = (int)Mathf.Floor(offsetPos.y / cellWidth);
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
       
        //�Ϸ��Լ��
        if (Check_Pos(row, col)&&preBoard.IsAva(row,col)&&Check_LegAddAllPiece(row,col,boardPieceColor))
        {
            FlipPiece(boardPieceColor);//��ת����
            
            //���������������������
            preBoard.ClearavaPut();
            AddPiece(row, col, boardPieceColor, piecePos);//��ӵ��������

            AddPosPoint(row, col);
            //Debug.Log("�������λ��" + row + " " + col);

            return true;
        }
        return false;


    }
       
   //ֻ�ڳ�ʼ����ʱ��ʹ�õ��������λ����ӣ�����Ҫ���м���뷭ת
    public void CreatePiece(int row,int col, BoardPieceColor boardPieceColor)
    {
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
        if (Check_Pos(row, col))
        {

            AddPiece(row, col, boardPieceColor, piecePos);
        }

    }

    //ֻ��ʹ��AI������ʱ�������ӵ���ӣ�ͬʱ��ת������Ҫ���м��
    public void CreatePiece(int pos,BoardPieceColor AIPieceColor)
    {
        int row = pos / boardLength;
        int col = pos % boardLength;
        //Debug.Log("AI�µ�λ��" + row + " " + col);
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
        Check_LegAddAllPiece(row, col, AIPieceColor);    
        FlipPiece(AIPieceColor);//��ת����
        AddPiece(row, col, AIPieceColor, piecePos);//��ӵ��������
        AddPosPoint(row, col);

        
    }

    //ָʾ����λ�õ�
    //������
 

    public void AddPosPoint(int row,int col)
    {
        DestroyAddPosPoint();
        var newPiece = Instantiate(ava3_Piece);
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
        newPiece.transform.SetParent(GameObject.Find("board").transform, false);
        newPiece.transform.position = piecePos;
        PosPoint = newPiece;
    }
    public void DestroyAddPosPoint()
    {
        if(PosPoint!=null)
        {
            Destroy(PosPoint);
        }
    }
    //�������
    public void AddPiece(int row,int col, BoardPieceColor boardPieceColor, Vector3 piecePos)
    {
        if (boardPieceColor == BoardPieceColor.Black)
        {
            BoardPiece[row, col] = BoardPieceColor.Black;
            var newPiece = Instantiate(black_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            boardGameObject[row,col] = newPiece;
            //pieceColor = PieceColor.White;
            //Debug.Log("��ǰ���λ��" + row + col);
            CntBlack++;
        }
        else if(boardPieceColor==BoardPieceColor.White)
        {
            BoardPiece[row, col] = BoardPieceColor.White;
            var newPiece = Instantiate(white_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            boardGameObject[row, col] = newPiece;
            //pieceColor = PieceColor.Black;
            //Debug.Log("��ǰλ��" + row + col);
            CntWhite++;
        }
        else if(boardPieceColor==BoardPieceColor.NULL)
        {
           
            var newPiece = Instantiate(ava_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            boardGameObject[row, col] = newPiece;
            //pieceColor = PieceColor.Black;
            //Debug.Log("�����Ŀ������ӣ���ǰλ��" + row + col);
            
        }
        else
        {
            var newPiece = Instantiate(ava2_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            Destroy(boardGameObject[row, col]);//��ԭ�����õ�������滻
            boardGameObject[row, col] = newPiece;
        }
    }

    public void FlipPiece(BoardPieceColor boardPieceColor)
    {
        Sprite sprite;
        //Ϊ��ת�����趨��ɫ
        if (boardPieceColor == BoardPieceColor.Black)
        {
            sprite = black_Renderer;
            //��������
            CntBlack += AllPiece.Count;
            CntWhite -= AllPiece.Count;
        }
        else
        {
            sprite = white_Renderer;
            CntBlack -= AllPiece.Count;
            CntWhite += AllPiece.Count;
        }


        //��AllPiece�е����ӽ��з�ת
        foreach(Tuple<int,int>  i in AllPiece)
        {
            boardGameObject[i.Item1, i.Item2].GetComponent<SpriteRenderer>().sprite = sprite;//�޸�������ɫ
            BoardPiece[i.Item1, i.Item2] = boardPieceColor;//�޸Ĵ洢��ɫ����
            
        }

    }

    //�����Ƿ�����
    public bool IsFull()
    {
        if (CntWhite + CntBlack == 64)
            return true;
        return false;
    }


   }
