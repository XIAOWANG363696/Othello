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
    
    //Ȩ�ط���
    const double weightOfBoard = 0.30;
    const double weightOfMove = 0.7;

    const int valueOfMovePiece = 3;
    const int Depth = 6;
    const int StableDepth = 40;
    const int valueOfStablePiece = 10;
    public BoardManage.BoardPieceColor AIpieceType;
   
    //������Ϣ
    public BoardManage board;

    //AI��Ϣ
  

    //������Ϣת��Ϊһά����
    BoardManage.BoardPieceColor[] boardPieceColor = new BoardManage.BoardPieceColor[BoardManage.boardLength * BoardManage.boardLength];

    //λ��Ȩָ��
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

    //λ�ñ����������˳����
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
    //������ڵĺ�ɫ�ȶ���
    HashSet<int> blackStable = new HashSet<int>();
    HashSet<int> whiteStable = new HashSet<int>();

    //�ĸ����������
    int[] MaxPos = new int[] { 0, 7, 56, 63 };



    //�û���

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
        //û��λ�ÿ��£�ֱ�ӷ��ظ�һ
        return pos;

    }


    void TransBoard(BoardManage board)
    {
        for(int i = 0;i<BoardManage.boardLength;i++)
        {
            for(int j = 0;j<BoardManage.boardLength;j++)
            {
                //ת��Ϊһά����
                boardPieceColor[i * BoardManage.boardLength + j] = board.BoardPiece[i, j];
            }
        }    
    }




    //FindPos����һ��һά�����λ��ֵ��ͨ������ת���ɶ�Ԫ�鷵��
    public int FindPos( BoardManage.BoardPieceColor AIpieceType, BoardManage.BoardPieceColor[] boardPieceColor,int cntBlackPiece,int cntWhitePiece)
    {
        
        //��ʼ��
        //�����ֵ��Ϊ��С����Ѱ���ֵ
        double valueMax = AIpieceType==BoardManage.BoardPieceColor.Black?INF_MIN:INF_MAX;
        int valuePos = -1;//��¼λ��Ϊ0

        //����״��
        BoardManage.BoardPieceColor[] curBoard = new BoardManage.BoardPieceColor[boardPieceColor.Length];
        Array.Copy(boardPieceColor, curBoard, boardPieceColor.Length);


        int curCntBlackPiece = cntBlackPiece;
        int curCntWhitePiece = cntWhitePiece;

        //Ѱ�ҿ��������λ�÷������У����б���
        //HashSet<int> canMovePos = new HashSet<int>();
        List<int> canMovePos = new List<int>();

        //ÿ�ν����ȼ���һ�¹�ϣֵ
        transPositionTabe.CalculateHashKey(curBoard);

        


        //Ѱ�ҿ�����λ�õ��㷨
        //����canMovePos������
        FindCanMoveSet(ref canMovePos, curBoard, AIpieceType);
       


        //��canMovePos�����ĸ�����ļ��
        //������ԣ�ֱ����
        for (int i = 0;i<MaxPos.Length;i++)
        {
            
            if (canMovePos.Contains(MaxPos[i]))
            {
                return MaxPos[i];//ֱ�ӷ��ظ�λ��
            }
        }


        //�˴�����ÿ��λ��
        foreach(int i in canMovePos)
        {
            double curValue;

            //ͳ�ƿɷ�ת���ӵ��б�����ͳ���û����
            List<int> canChangePiece = new List<int>();
            //�Ž�ȥ������̽��з�ת,ͬʱͳ��Ŀǰ�ڰ��Ӹ���
            FlipBoard(i, AIpieceType, ref curBoard,ref curCntWhitePiece,ref curCntBlackPiece,ref canChangePiece);
            
           
            curBoard[i] = AIpieceType;

            if (AIpieceType == BoardManage.BoardPieceColor.Black)
                curCntBlackPiece++;
            else if (AIpieceType == BoardManage.BoardPieceColor.White)
                curCntWhitePiece++;

            

            //��������һ����,����Ӯ������Ҫ����ѭ��,ֱ�ӷ��ظ�ֵ
            
            if ((curCntWhitePiece + curCntBlackPiece) == BoardManage.boardLength * BoardManage.boardLength)
            {
                double tempValue = Winner(curCntBlackPiece, curCntWhitePiece);
                if (IsWin(AIpieceType, tempValue))
                    return i;
            }


            //���¹�ϣֵ
            
            transPositionTabe.Hash_MakeMove(canChangePiece, i, AIpieceType);

            //�жϸù�ϣֵ�Ƿ����,������ڲ��ý�������
            double HashValue = transPositionTabe.LookUpHashTable(curCntWhitePiece + curCntBlackPiece, AIpieceType);
            if (HashValue != INF_NO)
            {
                //Debug.Log("wow Hash��Ȼ������"+curCntBlackPiece+curCntWhitePiece);
                curValue = HashValue;
            }
            else
                curValue = DFSFind(0,SwapPiece(AIpieceType),curBoard,curCntBlackPiece,curCntWhitePiece,valueMax);

            //-----------------��һ�鷢������������------------------
            //�鿴------------�Ƿ��������������-----------------
            //Debug.Log("��ʱ�� " + AIpieceType + "���ص�ֵΪ" + curValue);
            //����С�Ƚ�
            if (curValue>=valueMax&&AIpieceType==BoardManage.BoardPieceColor.Black)
            {
                valueMax = curValue;
                valuePos = i;//��ǰλ��
            }
            else if(curValue <= valueMax && AIpieceType == BoardManage.BoardPieceColor.White)
            {
                valueMax = curValue;
                valuePos = i;//��ǰλ��
            }

           

            //���̸�ԭ,���Ӹ�����ԭ
            Array.Copy(boardPieceColor, curBoard, boardPieceColor.Length);
            curCntBlackPiece = cntBlackPiece;
            curCntWhitePiece = cntWhitePiece;

            //��ϣֵ��ԭ
            transPositionTabe.Hash_UnMakeMove(canChangePiece, i, AIpieceType);
           
        }


        //Debug.Log("���մ�ʱ�� " + AIpieceType + "���ص�ֵΪ" + valueMax);
        return valuePos;

    }




    //DFSFind�������֣�����Ҫ֪��λ��
    //
    double DFSFind(int depth,BoardManage.BoardPieceColor curPieceType, BoardManage.BoardPieceColor[] boardPieceColors,int cntBlackPiece,int cntWhitePiece,double lastValueMax)
    {
        //��ʼ��
        //�����ֵ��Ϊ��С����Ѱ���ֵ
        double valueMax = curPieceType == BoardManage.BoardPieceColor.Black ? INF_MIN : INF_MAX;
        int valuePos = -1;//��¼λ��Ϊ0


        //����״��
        BoardManage.BoardPieceColor[] curBoard = new BoardManage.BoardPieceColor[boardPieceColors.Length];
        Array.Copy(boardPieceColors, curBoard, boardPieceColors.LongLength);

        BoardManage.BoardPieceColor nextPieceType = SwapPiece(curPieceType);

        //Ѱ�ҿ��������λ�÷������У����б���
        List<int> canMovePos = new List<int>();

        //���Ӱ��ӵ�״��
        int curCntBlackPiece = cntBlackPiece;
        int curCntWhitePiece = cntWhitePiece;

        //���ֵĿɻ��
        int canMovePiece;
      
        
       
    
        //-------------------------------------�ݹ��������------------------------
        //������ײ�ķ������� 1��ײ� 2������� 3�ĸ��������ѡ��
        if (depth==Depth)
        {
            //---------��������޸�---------
            canMovePiece = FindCanMoveCnt(curBoard,curPieceType);
            double theValue = GetValue(curBoard, canMovePiece, curPieceType,curCntBlackPiece,curCntWhitePiece);
           
            
            //Ҷ�ӽ���ֵ�Ž���ϣ����
            transPositionTabe.EnterHashTable(curPieceType, theValue, curCntBlackPiece + curCntWhitePiece);
           
            
            return theValue;
        }
        
        //����ʤ���ˣ�ֱ�ӷ��������С��ֵ
        if((cntBlackPiece+cntWhitePiece)==(BoardManage.boardLength* BoardManage.boardLength))
        {
            return Winner(curCntBlackPiece, curCntWhitePiece);
        }


        //Ѱ������λ��
        FindCanMoveSet(ref canMovePos, curBoard, curPieceType);

        //��canMovePos�����ĸ�����ļ��
        
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
        //--------------------------------�ݹ��������----------------------------


        //�������������û�У�����depth����
        foreach(int i in canMovePos)
        {
           
            double curValue;



    

            List<int> canChangePiece = new List<int>();
            //��ת����
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

            //��ת���̺���¹�ϣֵ,����ֵ�Ƿ����
            transPositionTabe.Hash_MakeMove(canChangePiece,i, curPieceType);

            

            double HashValue = transPositionTabe.LookUpHashTable(curCntBlackPiece + curCntWhitePiece, curPieceType);
            if (HashValue != INF_NO)
            {
                curValue = HashValue;
               // Debug.Log("�� ��������" + curCntBlackPiece +"  "+ curCntWhitePiece);
            }
            else
            {
                curValue = DFSFind(depth + 1, nextPieceType, curBoard, curCntBlackPiece, curCntWhitePiece, valueMax);
                if(curValue!=INF_MAX&&curValue!=INF_MIN)
                    transPositionTabe.EnterHashTable(curPieceType, curValue, curCntBlackPiece + curCntWhitePiece);
            }

            //����С�Ƚ�

            //�Ƿ��޸�---------------------------------�Ƿ��޸�------------------------------------
            //����Ѿ���������Ϸ�����һ�ʤ��ֱ���˳����ɣ�����Ҫ�ٽ��й�ֵ�����Ƚ�
            //if (IsWin(curPieceType,curValue))
            //{
                
            //}
            //�д�˼��һ��-----------------------------�д�˼��һ��---------------------------------------
            
            //��ϣֵ��ԭ
            transPositionTabe.Hash_UnMakeMove(canChangePiece, i, curPieceType);




          



            if (curPieceType == BoardManage.BoardPieceColor.Black && curValue > valueMax)
            {
               
                //���Ц��¼�֦
                if(curValue>lastValueMax)
                {
                    //---��֦����-----------
                    return curValue;
                }
                valueMax = curValue;
                valuePos = i;//��ǰλ��
                
            }
            else if (curPieceType == BoardManage.BoardPieceColor.White&&curValue<valueMax)
            {
               
                //���Ц��¼�֦
                if(curValue<lastValueMax)
                {
                    //----��֦����--------
                    return curValue;
                }
                valueMax = curValue;
                valuePos = i;
            }

            //���̸�ԭ
            Array.Copy(boardPieceColors, curBoard, boardPieceColors.LongLength);
            curCntBlackPiece = cntBlackPiece;
            curCntWhitePiece = cntWhitePiece;

            
        }


        return valueMax;
    }



    //��������
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
    
    //�����Ӯ���˷���Ӧ�еĹ�ֵ
    double Winner(int cntBlackPiece,int cntWhitePiece)
    {
        //����ƽ����Ϣ
        if (cntWhitePiece == cntBlackPiece)
            return INF_DRAW;
        //�׵�Ӯ��
        if (cntWhitePiece > cntBlackPiece)
            return INF_MIN;
        //�ڵ�Ӯ��
        else
            return INF_MAX;
        
    }
    //Ѱ���ܹ����ӵ�λ������ӵ�canMovePos������
    void FindCanMoveSet(ref List<int> canMovePos, BoardManage.BoardPieceColor[] curBoard,BoardManage.BoardPieceColor pieceType)
    {
        for(int i = 0;i<BoardManage.boardLength*BoardManage.boardLength;i++)
        {
            if (curBoard[disOfValueBoard[i]] != BoardManage.BoardPieceColor.NULL) continue;
            if (CanMove(disOfValueBoard[i], pieceType,curBoard))
                canMovePos.Add(disOfValueBoard[i]);
        }
    }

    //Ѱ�һ�Ӹ���
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
        //Ѱ������
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
                //�����߽�
                if (now_row >= BoardManage.boardLength || now_col >= BoardManage.boardLength || now_col < 0 || now_row < 0) break;
                //�հ�
                else if (curBoard[now_row*BoardManage.boardLength+now_col] == BoardManage.BoardPieceColor.NULL) break;
                //ͬɫ
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



    //�����Ժ�������ӵķ�ת���ı�Ŀǰ������
    //��ʱ��������curBoard�ǻ���в����ı��
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
                //�����߽�
                if (now_row >= BoardManage.boardLength || now_col >= BoardManage.boardLength || now_col < 0 || now_row < 0) break;
                //�հ�
                else if (curBoard[now_row * BoardManage.boardLength + now_col] == BoardManage.BoardPieceColor.NULL) break;
                //ͬɫ
                else if (curBoard[now_row * BoardManage.boardLength + now_col] == pieceType)
                {
                    flag = 1;
                    break;
                }
                //��ͬɫ��ʱ��ӵ�������
                canMove.Add(now_row * BoardManage.boardLength + now_col);


            }
            if (flag == 1 && canMove.Count>0)
            {
                canChangePiece = canChangePiece.Union(canMove).ToList();
                foreach(int k in canMove)
                {
                    curBoard[k] = pieceType;//״̬��ת
                }
            }
            //����ǵ�ǰ״̬�Ǻ���
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
    
    


    //���ݵ�ǰ�ľ����������������
    //���̾��� ��ǰ������ ��Ӹ���
    double GetValue(BoardManage.BoardPieceColor[] curBoard,int MovePieceCnt,BoardManage.BoardPieceColor pieceColor,int cntBlackPiece,int cntWhitePiece)
    {
        double value = 0;
        for(int i = 0;i<BoardManage.boardLength*BoardManage.boardLength;i++)
        {
            //�յ�����
            if (curBoard[i] == BoardManage.BoardPieceColor.NULL)
                continue;
            //����Ǻ���+
            if (curBoard[i] == BoardManage.BoardPieceColor.Black)
                value += valueOfBoard[i];
            //����ǰ���-
            else if (curBoard[i] == BoardManage.BoardPieceColor.White)
                value -= valueOfBoard[i];
        }
        value *= weightOfBoard;
        //�����ǰ�ǰ������꣬˵���������Ǻ��ӵĻ��
        if(pieceColor==BoardManage.BoardPieceColor.White)
            value -= MovePieceCnt  * weightOfMove*valueOfMovePiece;
        //����Ǻ������꣬˵���������ǰ��ӵĻ���ж�
        else if(pieceColor==BoardManage.BoardPieceColor.Black)
            value += MovePieceCnt* weightOfMove*valueOfMovePiece;

        //������ڸò����������ȶ��ӵ��ж�
        //if((cntBlackPiece+cntWhitePiece)>=StableDepth)
        //{
        //    int blackStablePiece = StablePiece(BoardManage.BoardPieceColor.Black, curBoard);
        //    int whiteStablePiece = StablePiece(BoardManage.BoardPieceColor.White, curBoard);
        //    value += (blackStablePiece - whiteStablePiece)*valueOfStablePiece*weightOfBoard;
        //}
        return value;  
        
    }

    //�ȶ���ʹ�õݹ飬��Ѱʱ��Ƚϳ���Ӧ�÷�����ֺ���Ĳ���
    int  StablePiece(BoardManage.BoardPieceColor pieceColor, BoardManage.BoardPieceColor[] curboar)
    {   
        //ÿ�δ���Ѱǰ�����
        
        //��ʼ�����
        InitCheckStableBoard();


        int stableCnt = 0;

        for(int i = 0;i<disOfStableBoard.Length; i++)
        {
            if (IsStablePeice(disOfStableBoard[i], pieceColor, curboar))
            {
                Debug.Log("��ʱ��" + pieceColor + " λ����" + disOfStableBoard[i] / 8 + " " + disOfStableBoard[i] % 8 + " ���ȶ���");
                

                AddtheStablePiece(pieceColor, disOfStableBoard[i]);
               
                stableCnt++;
            }
        }
        if(stableCnt!=0)
            Debug.Log("������" + stableCnt + "�ȶ���");
        
        
        return stableCnt;
    }

    
    bool  IsStablePeice(int pos,BoardManage.BoardPieceColor pieceColor, BoardManage.BoardPieceColor[] curboar)
    {
        //�ȼ���Ƿ����ĸ��ӣ�ע����ɫҪ��ͬ
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
        //�˸�����ֻҪ�����ĸ��������ȶ��Ӽ���
        for(int i = 0;i<8;i++)
        {
          
            int new_row = row + dis[i, 0];
            int new_col = col + dis[i, 1];
            int new_pos = new_row* 8 + new_col;
            //�����ͬ��ɫ��


            if (IsOver(new_row, new_col))
                stableCnt++;

            else if (curboar[new_pos] != pieceColor)
                continue;
            //����Ѿ���Ѱ��������
            else if (CheckStableBoard[new_pos] == 1)
                continue;
            else if (IsOnStablePieceSet(pieceColor,new_row))
                stableCnt++;
            else if (IsStablePeice(new_pos, pieceColor, curboar))
                stableCnt++;

            if (stableCnt >= 4)
            {
                //��ǰλ�����ȶ���
               
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