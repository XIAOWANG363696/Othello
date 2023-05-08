using JetBrains.Annotations;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


 
public class AIMove : MonoBehaviour
{
    public const double INF_MAX = 99999999;
    public const double INF_MIN = -99999999;
    public const double INF_DRAW = 77777777;
    public const double INF_NO = 66666666;
    
    //权重分配
    const double weightOfBoard = 0.30;
    const double weightOfMove = 0.7;

    const int valueOfMovePiece = 3;
    const int Depth = 6;
    const int StableDepth = 40;
    const int valueOfStablePiece = 10;
    public BoardManage.BoardPieceColor AIpieceType;
   
    //棋盘信息
    public BoardManage board;

    //AI信息
  

    //棋盘信息转换为一维数组
    BoardManage.BoardPieceColor[] boardPieceColor = new BoardManage.BoardPieceColor[BoardManage.boardLength * BoardManage.boardLength];

    //位置权指标
    double[] valueOfBoard = new double[] {100, -5, 10,  5,  5, 10, -5,100,
                                           -5,-50,  1,  1,  1,  1,-50, -5,
                                           10,  1,  3,  2,  2,  3,  1, 10,
                                            5,  1,  2,  1,  1,  2,  1,  5,
                                            5,  1,  2,  1,  1,  2,  1,  5,
                                           10,  1,  3,  2,  2,  3,  1, 10,
                                           -5,-50,  1,  1,  1,  1,-50, -5,
                                          100, -5, 10,  5,  5, 10, -5,100};

    //double[] valueOfBoard = new double[] { 90, -60, 10, 10, 10, 10, -60,90,
    //                                       -60,-80, 5,  5,  5,  5,-80, -60,
    //                                       10,  5,  1,  1,  1,  1,  5, 10,
    //                                       10,  5,  1,  1,  1,  1,  5, 10,
    //                                       10,  5,  1,  1,  1,  1,  5, 10,
    //                                       10,  5,  1,  1,  1,  1,  5, 10,
    //                                      -60,-80,  5,  5,  5,  5,-80, -60,
    //                                       90,-60, 10, 10, 10, 10,-60, 90};

    //位置遍历按照这个顺序来
    int[] disOfValueBoard = new int[] {0,   7, 56, 63,  2,  5, 16, 40,
                                       23, 47, 58, 61,  3,  4, 24, 32,
                                       59, 60, 31, 39, 18, 21, 42, 45,
                                       26, 34, 43, 44, 29, 37, 19, 20,
                                       10, 11, 12, 13, 17, 25, 33, 41,
                                       50, 51, 52, 53, 22, 30, 38, 46,
                                       27, 28, 35, 36,  1,  6, 48, 55,
                                       8,  15, 57, 62,  9, 14, 49, 54};
    // Start is called before the first frame update

    int[] disOfStableBoard = new int[] { 0, 1, 2, 3, 4, 5, 6, 7,
                                        };

    int[] CheckStableBoard;
    //HashSet<int> stableSet = new HashSet<int>();
    //在这局内的黑色稳定子
    HashSet<int> blackStable = new HashSet<int>();
    HashSet<int> whiteStable = new HashSet<int>();

    //四个角落的坐标
    int[] MaxPos = new int[] { 0, 7, 56, 63 };



    //置换表

    public TransPositionTabe transPositionTabe;
    void Start()
    {
        transPositionTabe = new TransPositionTabe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int CreatePiece(BoardManage board)
    {   
        int cntBlack, cntWhite, pos;
        
        this.board = board;
        TransBoard(board);
        
        cntBlack = board.CntBlack;
        cntWhite = board.CntWhite;

        pos = FindPos(AIpieceType, boardPieceColor, cntBlack, cntWhite);
        //没有位置可下，直接返回负一
        return pos;

    }


    void TransBoard(BoardManage board)
    {
        for(int i = 0;i<BoardManage.boardLength;i++)
        {
            for(int j = 0;j<BoardManage.boardLength;j++)
            {
                //转换为一维数组
                boardPieceColor[i * BoardManage.boardLength + j] = board.BoardPiece[i, j];
            }
        }    
    }




    //FindPos返回一个一维数组的位置值，通过函数转换成二元组返回
    public int FindPos( BoardManage.BoardPieceColor AIpieceType, BoardManage.BoardPieceColor[] boardPieceColor,int cntBlackPiece,int cntWhitePiece)
    {
        
        //初始化
        //黑棋初值定为最小，找寻最大值
        double valueMax = AIpieceType==BoardManage.BoardPieceColor.Black?INF_MIN:INF_MAX;
        int valuePos = -1;//记录位置为0

        //棋盘状况
        BoardManage.BoardPieceColor[] curBoard = new BoardManage.BoardPieceColor[boardPieceColor.Length];
        Array.Copy(boardPieceColor, curBoard, boardPieceColor.Length);


        int curCntBlackPiece = cntBlackPiece;
        int curCntWhitePiece = cntWhitePiece;

        //寻找可以下棋的位置放入其中，进行遍历
        //HashSet<int> canMovePos = new HashSet<int>();
        List<int> canMovePos = new List<int>();

        //每次进来先计算一下哈希值
        transPositionTabe.CalculateHashKey(curBoard);

        


        //寻找可下棋位置的算法
        //放入canMovePos数组中
        FindCanMoveSet(ref canMovePos, curBoard, AIpieceType);
       


        //对canMovePos进行四个角落的检查
        //如果可以，直接下
        for (int i = 0;i<MaxPos.Length;i++)
        {
            
            if (canMovePos.Contains(MaxPos[i]))
            {
                return MaxPos[i];//直接返回该位置
            }
        }


        //此处遍历每个位置
        foreach(int i in canMovePos)
        {
            double curValue;

            //统计可翻转棋子的列表，用来统计置换表的
            List<int> canChangePiece = new List<int>();
            //放进去后对棋盘进行翻转,同时统计目前黑白子个数
            FlipBoard(i, AIpieceType, ref curBoard,ref curCntWhitePiece,ref curCntBlackPiece,ref canChangePiece);
            
           
            curBoard[i] = AIpieceType;

            if (AIpieceType == BoardManage.BoardPieceColor.Black)
                curCntBlackPiece++;
            else if (AIpieceType == BoardManage.BoardPieceColor.White)
                curCntWhitePiece++;

            

            //如果是最后一步棋,且能赢，不需要进入循环,直接返回该值
            
            if ((curCntWhitePiece + curCntBlackPiece) == BoardManage.boardLength * BoardManage.boardLength)
            {
                double tempValue = Winner(curCntBlackPiece, curCntWhitePiece);
                if (IsWin(AIpieceType, tempValue))
                    return i;
            }


            //更新哈希值
            
            transPositionTabe.Hash_MakeMove(canChangePiece, i, AIpieceType);

            //判断该哈希值是否存在,如果存在不用进行搜索
            double HashValue = transPositionTabe.LookUpHashTable(curCntWhitePiece + curCntBlackPiece, AIpieceType);
            if (HashValue != INF_NO)
            {
                //Debug.Log("wow Hash居然存在诶"+curCntBlackPiece+curCntWhitePiece);
                curValue = HashValue;
            }
            else
                curValue = DFSFind(0,SwapPiece(AIpieceType),curBoard,curCntBlackPiece,curCntWhitePiece,valueMax);

            //-----------------这一块发现问题所在了------------------
            //查看------------是否是这里的问题你-----------------
            //Debug.Log("此时是 " + AIpieceType + "返回的值为" + curValue);
            //极大极小比较
            if (curValue>=valueMax&&AIpieceType==BoardManage.BoardPieceColor.Black)
            {
                valueMax = curValue;
                valuePos = i;//当前位置
            }
            else if(curValue <= valueMax && AIpieceType == BoardManage.BoardPieceColor.White)
            {
                valueMax = curValue;
                valuePos = i;//当前位置
            }

           

            //棋盘复原,棋子个数复原
            Array.Copy(boardPieceColor, curBoard, boardPieceColor.Length);
            curCntBlackPiece = cntBlackPiece;
            curCntWhitePiece = cntWhitePiece;

            //哈希值复原
            transPositionTabe.Hash_UnMakeMove(canChangePiece, i, AIpieceType);
           
        }


        //Debug.Log("最终此时是 " + AIpieceType + "返回的值为" + valueMax);
        return valuePos;

    }




    //DFSFind返回评分，不需要知道位置
    //
    double DFSFind(int depth,BoardManage.BoardPieceColor curPieceType, BoardManage.BoardPieceColor[] boardPieceColors,int cntBlackPiece,int cntWhitePiece,double lastValueMax)
    {
        //初始化
        //黑棋初值定为最小，找寻最大值
        double valueMax = curPieceType == BoardManage.BoardPieceColor.Black ? INF_MIN : INF_MAX;
        int valuePos = -1;//记录位置为0


        //棋盘状况
        BoardManage.BoardPieceColor[] curBoard = new BoardManage.BoardPieceColor[boardPieceColors.Length];
        Array.Copy(boardPieceColors, curBoard, boardPieceColors.LongLength);

        BoardManage.BoardPieceColor nextPieceType = SwapPiece(curPieceType);

        //寻找可以下棋的位置放入其中，进行遍历
        List<int> canMovePos = new List<int>();

        //黑子白子的状况
        int curCntBlackPiece = cntBlackPiece;
        int curCntWhitePiece = cntWhitePiece;

        //对手的可活动子
        int canMovePiece;
      
        
       
    
        //-------------------------------------递归结束部分------------------------
        //三个最底层的返回条件 1最底层 2决出结果 3四个角落可以选择
        if (depth==Depth)
        {
            //---------这里进行修改---------
            canMovePiece = FindCanMoveCnt(curBoard,curPieceType);
            double theValue = GetValue(curBoard, canMovePiece, curPieceType,curCntBlackPiece,curCntWhitePiece);
           
            
            //叶子结点的值放进哈希表里
            transPositionTabe.EnterHashTable(curPieceType, theValue, curCntBlackPiece + curCntWhitePiece);
           
            
            return theValue;
        }
        
        //决出胜负了，直接返回最大最小估值
        if((cntBlackPiece+cntWhitePiece)==(BoardManage.boardLength* BoardManage.boardLength))
        {
            return Winner(curCntBlackPiece, curCntWhitePiece);
        }


        //寻找下棋位置
        FindCanMoveSet(ref canMovePos, curBoard, curPieceType);

        //对canMovePos进行四个角落的检查
        
        for (int i = 0; i < MaxPos.Length; i++)
        {
            
            if (canMovePos.Contains(MaxPos[i]))
            {
                if (curPieceType == BoardManage.BoardPieceColor.Black)
                {
                    
                    return INF_MAX;
                }
                else if (curPieceType == BoardManage.BoardPieceColor.White)
                {
                    return INF_MIN;
                }
            }
        }
        //--------------------------------递归结束部分----------------------------


        //如果以上条件都没有，遍历depth层数
        foreach(int i in canMovePos)
        {
           
            double curValue;



    

            List<int> canChangePiece = new List<int>();
            //翻转棋盘
            FlipBoard(i, curPieceType, ref curBoard, ref curCntWhitePiece, ref curCntBlackPiece,ref canChangePiece);
            
            curBoard[i] = curPieceType;

            if(curPieceType == BoardManage.BoardPieceColor.White)
            {
                curCntWhitePiece++;
            }
            else if(curPieceType == BoardManage.BoardPieceColor.Black)
            {
                curCntBlackPiece++;
            }

            //翻转棋盘后更新哈希值,检查该值是否存在
            transPositionTabe.Hash_MakeMove(canChangePiece,i, curPieceType);

            

            double HashValue = transPositionTabe.LookUpHashTable(curCntBlackPiece + curCntWhitePiece, curPieceType);
            if (HashValue != INF_NO)
            {
                curValue = HashValue;
               // Debug.Log("哇 存在诶！" + curCntBlackPiece +"  "+ curCntWhitePiece);
            }
            else
            {
                curValue = DFSFind(depth + 1, nextPieceType, curBoard, curCntBlackPiece, curCntWhitePiece, valueMax);
                if(curValue!=INF_MAX&&curValue!=INF_MIN)
                    transPositionTabe.EnterHashTable(curPieceType, curValue, curCntBlackPiece + curCntWhitePiece);
            }

            //极大极小比较

            //是否修改---------------------------------是否修改------------------------------------
            //如果已经搜索到游戏结束且获胜，直接退出即可，不需要再进行估值函数比较
            //if (IsWin(curPieceType,curValue))
            //{
                
            //}
            //有待思考一下-----------------------------有待思考一下---------------------------------------
            
            //哈希值复原
            transPositionTabe.Hash_UnMakeMove(canChangePiece, i, curPieceType);




          



            if (curPieceType == BoardManage.BoardPieceColor.Black && curValue > valueMax)
            {
               
                //进行αβ剪枝
                if(curValue>lastValueMax)
                {
                    //---剪枝补充-----------
                    return curValue;
                }
                valueMax = curValue;
                valuePos = i;//当前位置
                
            }
            else if (curPieceType == BoardManage.BoardPieceColor.White&&curValue<valueMax)
            {
               
                //进行αβ剪枝
                if(curValue<lastValueMax)
                {
                    //----剪枝补充--------
                    return curValue;
                }
                valueMax = curValue;
                valuePos = i;
            }

            //棋盘复原
            Array.Copy(boardPieceColors, curBoard, boardPieceColors.LongLength);
            curCntBlackPiece = cntBlackPiece;
            curCntWhitePiece = cntWhitePiece;

            
        }


        return valueMax;
    }



    //交换棋子
    BoardManage.BoardPieceColor SwapPiece( BoardManage.BoardPieceColor boardPieceColor)
    {
        if (boardPieceColor == BoardManage.BoardPieceColor.Black)
            return BoardManage.BoardPieceColor.White;
        else if (boardPieceColor == BoardManage.BoardPieceColor.White)
            return BoardManage.BoardPieceColor.Black;
        else
            return BoardManage.BoardPieceColor.NULL;
    }


    bool IsWin(BoardManage.BoardPieceColor pieceType,double curValue)
    {
        if (pieceType == BoardManage.BoardPieceColor.Black && curValue == INF_MAX)
            return true;
        else if (pieceType == BoardManage.BoardPieceColor.White && curValue == INF_MIN)
            return true;
        return false;

    }
    
    //如果有赢家了返回应有的估值
    double Winner(int cntBlackPiece,int cntWhitePiece)
    {
        //返回平局信息
        if (cntWhitePiece == cntBlackPiece)
            return INF_DRAW;
        //白的赢了
        if (cntWhitePiece > cntBlackPiece)
            return INF_MIN;
        //黑的赢了
        else
            return INF_MAX;
        
    }
    //寻找能够落子的位置且添加到canMovePos集合中
    void FindCanMoveSet(ref List<int> canMovePos, BoardManage.BoardPieceColor[] curBoard,BoardManage.BoardPieceColor pieceType)
    {
        for(int i = 0;i<BoardManage.boardLength*BoardManage.boardLength;i++)
        {
            if (curBoard[disOfValueBoard[i]] != BoardManage.BoardPieceColor.NULL) continue;
            if (CanMove(disOfValueBoard[i], pieceType,curBoard))
                canMovePos.Add(disOfValueBoard[i]);
        }
    }

    //寻找活动子个数
    int FindCanMoveCnt(BoardManage.BoardPieceColor[] curBoard, BoardManage.BoardPieceColor pieceType)
    {
        int cnt = 0;
        for (int i = 0; i < BoardManage.boardLength * BoardManage.boardLength; i++)
        {
            if (curBoard[disOfValueBoard[i]] != BoardManage.BoardPieceColor.NULL) continue;
            if (CanMove(disOfValueBoard[i], pieceType, curBoard))
                cnt++;
        }
        return cnt;
    }

    bool CanMove(int pos,BoardManage.BoardPieceColor pieceType, BoardManage.BoardPieceColor[] curBoard)
    {
        //寻找行列
        int row,col;
        int[,] dis = new int[8, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        col = pos % BoardManage.boardLength;
        row = pos/ BoardManage.boardLength;
       
        for (int i = 0; i < BoardManage.boardLength; i++)
        { 
            int cnt = 0;
            int flag = 0;
            for (int j = 1; j < BoardManage.boardLength; j++)
            {
                int now_row = row + j * dis[i, 0];
                int now_col = col + j * dis[i, 1];
                //超出边界
                if (now_row >= BoardManage.boardLength || now_col >= BoardManage.boardLength || now_col < 0 || now_row < 0) break;
                //空白
                else if (curBoard[now_row*BoardManage.boardLength+now_col] == BoardManage.BoardPieceColor.NULL) break;
                //同色
                else if (curBoard[now_row * BoardManage.boardLength + now_col] == pieceType)
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



    //下棋以后进行棋子的翻转，改变目前的棋盘
    //此时传进来的curBoard是会进行参数改变的
    void FlipBoard(int pos, BoardManage.BoardPieceColor pieceType, ref BoardManage.BoardPieceColor[] curBoard,ref int cntWhite,ref int cntBlack,ref List<int> canChangePiece)
    {
        int row, col;
        int[,] dis = new int[8, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        col = pos % BoardManage.boardLength;
        row = pos / BoardManage.boardLength;
        HashSet<int> canMove = new HashSet<int>();
        for (int i = 0; i < BoardManage.boardLength; i++)
        {
            
            int flag = 0;
            canMove.Clear();
            for (int j = 1; j < BoardManage.boardLength; j++)
            {
                int now_row = row + j * dis[i, 0];
                int now_col = col + j * dis[i, 1];
                //超出边界
                if (now_row >= BoardManage.boardLength || now_col >= BoardManage.boardLength || now_col < 0 || now_row < 0) break;
                //空白
                else if (curBoard[now_row * BoardManage.boardLength + now_col] == BoardManage.BoardPieceColor.NULL) break;
                //同色
                else if (curBoard[now_row * BoardManage.boardLength + now_col] == pieceType)
                {
                    flag = 1;
                    break;
                }
                //不同色的时候加到集合里
                canMove.Add(now_row * BoardManage.boardLength + now_col);


            }
            if (flag == 1 && canMove.Count>0)
            {
                canChangePiece = canChangePiece.Union(canMove).ToList();
                foreach(int k in canMove)
                {
                    curBoard[k] = pieceType;//状态翻转
                }
            }
            //如果是当前状态是黑子
            if(pieceType==BoardManage.BoardPieceColor.Black)
            {
                cntBlack += canMove.Count;
                cntWhite -=canMove.Count;
            }
            else if(pieceType == BoardManage.BoardPieceColor.White)
            {
                cntBlack -= canMove.Count;
                cntWhite += canMove.Count;
            }


        }
    }
    
    


    //根据当前的局势来获得评估函数
    //棋盘局势 当前下棋者 活动子个数
    double GetValue(BoardManage.BoardPieceColor[] curBoard,int MovePieceCnt,BoardManage.BoardPieceColor pieceColor,int cntBlackPiece,int cntWhitePiece)
    {
        double value = 0;
        for(int i = 0;i<BoardManage.boardLength*BoardManage.boardLength;i++)
        {
            //空的跳过
            if (curBoard[i] == BoardManage.BoardPieceColor.NULL)
                continue;
            //如果是黑子+
            if (curBoard[i] == BoardManage.BoardPieceColor.Black)
                value += valueOfBoard[i];
            //如果是白子-
            else if (curBoard[i] == BoardManage.BoardPieceColor.White)
                value -= valueOfBoard[i];
        }
        value *= weightOfBoard;
        //如果当前是白子下完，说明接下来是黑子的活动子
        if(pieceColor==BoardManage.BoardPieceColor.White)
            value -= MovePieceCnt  * weightOfMove*valueOfMovePiece;
        //如果是黑子下完，说明接下来是白子的活动子判断
        else if(pieceColor==BoardManage.BoardPieceColor.Black)
            value += MovePieceCnt* weightOfMove*valueOfMovePiece;

        //如果大于该层数，进行稳定子的判定
        //if((cntBlackPiece+cntWhitePiece)>=StableDepth)
        //{
        //    int blackStablePiece = StablePiece(BoardManage.BoardPieceColor.Black, curBoard);
        //    int whiteStablePiece = StablePiece(BoardManage.BoardPieceColor.White, curBoard);
        //    value += (blackStablePiece - whiteStablePiece)*valueOfStablePiece*weightOfBoard;
        //}
        return value;  
        
    }

    //稳定子使用递归，搜寻时间比较长，应该放在棋局后面的部分
    int  StablePiece(BoardManage.BoardPieceColor pieceColor, BoardManage.BoardPieceColor[] curboar)
    {   
        //每次大搜寻前先清空
        
        //初始化表格
        InitCheckStableBoard();


        int stableCnt = 0;

        for(int i = 0;i<disOfStableBoard.Length; i++)
        {
            if (IsStablePeice(disOfStableBoard[i], pieceColor, curboar))
            {
                Debug.Log("此时是" + pieceColor + " 位置是" + disOfStableBoard[i] / 8 + " " + disOfStableBoard[i] % 8 + " 的稳定子");
                

                AddtheStablePiece(pieceColor, disOfStableBoard[i]);
               
                stableCnt++;
            }
        }
        if(stableCnt!=0)
            Debug.Log("最终有" + stableCnt + "稳定子");
        
        
        return stableCnt;
    }

    
    bool  IsStablePeice(int pos,BoardManage.BoardPieceColor pieceColor, BoardManage.BoardPieceColor[] curboar)
    {
        //先检查是否是四个子，注意颜色要相同
        CheckStableBoard[pos] = 1;
       
        foreach (int i in MaxPos)
        {
            if (i == pos && curboar[i]==pieceColor)
                return true;
        }
        int row, col;
        row = pos / 8;
        col = pos % 8;
        int stableCnt = 0;
        
          int[,] dis = new int[8, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        //八个方向只要满足四个方向是稳定子即可
        for(int i = 0;i<8;i++)
        {
          
            int new_row = row + dis[i, 0];
            int new_col = col + dis[i, 1];
            int new_pos = new_row* 8 + new_col;
            //如果不同颜色舍


            if (IsOver(new_row, new_col))
                stableCnt++;

            else if (curboar[new_pos] != pieceColor)
                continue;
            //如果已经搜寻过，跳过
            else if (CheckStableBoard[new_pos] == 1)
                continue;
            else if (IsOnStablePieceSet(pieceColor,new_row))
                stableCnt++;
            else if (IsStablePeice(new_pos, pieceColor, curboar))
                stableCnt++;

            if (stableCnt >= 4)
            {
                //当前位置是稳定子
               
                AddtheStablePiece(pieceColor, pos);
                return true;
            }
        }
        return false;
    }

    bool IsOver(int row, int col)
    {
        if (row > 7 || row < 0 || col > 7 || col < 0)
            return true;
        return false;
    }

    void InitCheckStableBoard()
    {
        CheckStableBoard = new int[64];
        for(int i = 0;i<64;i++)
        {
            CheckStableBoard[i] = 0;
        }
    }

    void AddtheStablePiece(BoardManage.BoardPieceColor pieceColor,int pos)
    {
        if(pieceColor==BoardManage.BoardPieceColor.Black)
        {
            blackStable.Add(pos);

        }
        else
        {
            whiteStable.Add(pos);
        }
    }


    bool IsOnStablePieceSet(BoardManage.BoardPieceColor pieceColor, int pos)
    {
        if(pieceColor==BoardManage.BoardPieceColor.Black)
            return blackStable.Contains(pos);
        else
            return whiteStable.Contains(pos);

    }
}