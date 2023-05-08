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

    //自设源点的坐标与每个格子的宽度
    public Vector3 zeroPointPosition;
    public float cellWidth;  

    //存储黑子数量 白子数量
    public int CntBlack, CntWhite;
    //存储全部棋子的坐标，用来判断棋盘是否满了
    public HashSet<Tuple<int, int>> AllPiece = new HashSet<Tuple<int, int>>();
    //存储目前搜寻棋子的坐标，来判断哪些棋子能够翻转
    public HashSet<Tuple<int, int>> NowPiece = new HashSet<Tuple<int, int>>();
    //短期记录每行每列搜寻的棋子
    HashSet<Tuple<int, int>> temp = new HashSet<Tuple<int, int>>();



    //棋盘长宽
    public const int boardLength = 8;

    //每个位置上棋盘的状态
    public BoardPieceColor[,] BoardPiece = new BoardPieceColor[boardLength, boardLength];
    public GameObject[,]  boardGameObject = new GameObject[boardLength, boardLength];

    //准备棋子的模板
    public GameObject black_Piece;
    public GameObject white_Piece;
    public GameObject ava_Piece;
    public GameObject ava2_Piece;
    public GameObject ava3_Piece;
    
    public Sprite black_Renderer;
    public Sprite white_Renderer;

    
    public PreBoard preBoard;

    // Start is called before the first frame update
    //棋盘初始化
    public GameObject PosPoint;
    void Start()
    {
        //对棋盘进行一个初始化

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

        //获取图片
        black_Renderer = black_Piece.GetComponent<SpriteRenderer>().sprite;
        white_Renderer = white_Piece.GetComponent<SpriteRenderer>().sprite;

        //获取棋盘预判断脚本
       

        preBoard.FindAva_CreatePiece(BoardManage.BoardPieceColor.Black);

         


    }
    private void Awake()
    {
         preBoard = GetComponent<PreBoard>();
    }

    void Update()
    {
        
    }
   

    //边界检测
    public bool Check_Pos(int row,int col)
    {
        if (row >= boardLength || col >= boardLength || row < 0 || col < 0)
        {
             Debug.Log("超出边界，不能放");
            return false;
           
        }
        if (BoardPiece[row, col] != BoardPieceColor.NULL)
        {
            Debug.Log("已经存在，不能放");
            return false;
            
        }
        return true;
    }

    //检测放置的棋子能否至少翻转一个
    // 公用检测，如果flag= 0,是Pre用来判断每个棋子是否能放置的
   //同时把需要翻转的加入ALLpiece中


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
                //超出边界
                if (now_row >= boardLength || now_col >= boardLength || now_col < 0 || now_row < 0) break;
                //空白
                else if (BoardPiece[now_row, now_col] == BoardPieceColor.NULL) break;
                //同色
                else if (BoardPiece[now_row, now_col] == boardPieceColor)
                {
                    flag = 1;
                    break;
                }
                temp.Add(new Tuple<int, int>(now_row, now_col));
                
            }
            if(flag==1&&temp.Count!=0)
            {
                AllPiece.UnionWith(temp);//如果有元素可以合并 进行合并
               
            } 
            temp.Clear();
        }
        if (AllPiece.Count == 0)
        {
            //Debug.Log("这个位置不行，下不了棋，家人");
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
                //超出边界
                if (now_row >= boardLength || now_col >= boardLength || now_col < 0 || now_row < 0) break;
                //空白
                else if (BoardPiece[now_row, now_col] == BoardPieceColor.NULL) break;
                //同色
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


    //创建棋子是否成功
    public bool CreatePiece(BoardPieceColor boardPieceColor,Vector3 mousePosition)
    {
        
        ///这个清除需要变换位置-------------------
        //AllPiece.Clear();
        ///这个清除需要变换位置-------------------
        ///感觉可以不用换了。。。目前没出问题。。



        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);//把屏幕坐标转换为当前世界坐标
        Vector3 offsetPos = mousePos - zeroPointPosition;//得到鼠标的位置与设置原点的距离
        int row = (int)Mathf.Floor(offsetPos.x / cellWidth);
        int col = (int)Mathf.Floor(offsetPos.y / cellWidth);
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
       
        //合法性检查
        if (Check_Pos(row, col)&&preBoard.IsAva(row,col)&&Check_LegAddAllPiece(row,col,boardPieceColor))
        {
            FlipPiece(boardPieceColor);//翻转棋子
            
            //清空棋盘上其他待下棋子
            preBoard.ClearavaPut();
            AddPiece(row, col, boardPieceColor, piecePos);//添加点击的棋子

            AddPosPoint(row, col);
            //Debug.Log("玩家下棋位置" + row + " " + col);

            return true;
        }
        return false;


    }
       
   //只在初始创建时候使用的与可下棋位置添加，不需要进行检测与翻转
    public void CreatePiece(int row,int col, BoardPieceColor boardPieceColor)
    {
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
        if (Check_Pos(row, col))
        {

            AddPiece(row, col, boardPieceColor, piecePos);
        }

    }

    //只有使用AI机器人时进行棋子的添加，同时翻转，不需要进行检测
    public void CreatePiece(int pos,BoardPieceColor AIPieceColor)
    {
        int row = pos / boardLength;
        int col = pos % boardLength;
        //Debug.Log("AI下的位置" + row + " " + col);
        Vector3 piecePos = new Vector3(row * cellWidth + cellWidth / 2, col * cellWidth + cellWidth / 2, zeroPointPosition.z) + zeroPointPosition;
        Check_LegAddAllPiece(row, col, AIPieceColor);    
        FlipPiece(AIPieceColor);//翻转棋子
        AddPiece(row, col, AIPieceColor, piecePos);//添加点击的棋子
        AddPosPoint(row, col);

        
    }

    //指示下棋位置的
    //俩重载
 

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
    //添加棋子
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
            //Debug.Log("当前添加位置" + row + col);
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
            //Debug.Log("当前位置" + row + col);
            CntWhite++;
        }
        else if(boardPieceColor==BoardPieceColor.NULL)
        {
           
            var newPiece = Instantiate(ava_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            boardGameObject[row, col] = newPiece;
            //pieceColor = PieceColor.Black;
            //Debug.Log("创建的可下棋子，当前位置" + row + col);
            
        }
        else
        {
            var newPiece = Instantiate(ava2_Piece);
            newPiece.transform.SetParent(GameObject.Find("board").transform, false);
            newPiece.transform.position = piecePos;
            Destroy(boardGameObject[row, col]);//将原来放置的清除再替换
            boardGameObject[row, col] = newPiece;
        }
    }

    public void FlipPiece(BoardPieceColor boardPieceColor)
    {
        Sprite sprite;
        //为翻转重新设定颜色
        if (boardPieceColor == BoardPieceColor.Black)
        {
            sprite = black_Renderer;
            //计数棋子
            CntBlack += AllPiece.Count;
            CntWhite -= AllPiece.Count;
        }
        else
        {
            sprite = white_Renderer;
            CntBlack -= AllPiece.Count;
            CntWhite += AllPiece.Count;
        }


        //将AllPiece中的棋子进行翻转
        foreach(Tuple<int,int>  i in AllPiece)
        {
            boardGameObject[i.Item1, i.Item2].GetComponent<SpriteRenderer>().sprite = sprite;//修改棋子颜色
            BoardPiece[i.Item1, i.Item2] = boardPieceColor;//修改存储颜色类型
            
        }

    }

    //棋盘是否满了
    public bool IsFull()
    {
        if (CntWhite + CntBlack == 64)
            return true;
        return false;
    }


   }
