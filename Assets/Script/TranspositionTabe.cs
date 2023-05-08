using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//哈希表中元素的结构
public struct HASHITEM
{
    public long checksum;//64位校验码
    public int cnt_depth;//棋子数+层数
    public double value;//估值
};





//置换表
public class TransPositionTabe 
{
    //构造函数 说实话我也不知道要做什么,我猜应该是初始化哈希表
     public TransPositionTabe() {

        InitHashKey();
    }
    //析构函数
     ~TransPositionTabe() { }

    
    //计算棋盘的哈希值
    //初始化棋盘可以算一次
    public void CalculateHashKey(BoardManage.BoardPieceColor[] board)
    {
        
        BoardManage.BoardPieceColor pieceColor;
        nHashKey32 = 0;
        nHashKey64 = 0;
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0;j<8;j++)
            { 
                pieceColor = board[i*8+j];
                int k;
                if (pieceColor == BoardManage.BoardPieceColor.Black)
                    k = 0;
                else if (pieceColor == BoardManage.BoardPieceColor.White)
                    k = 1;
                else
                    k = 2;
                if (pieceColor != BoardManage.BoardPieceColor.NULL)
                {
                    nHashKey32 = nHashKey32 ^ HashKey32[k, i, j];
                    nHashKey64 = nHashKey64 ^ HashKey64[k, i, j];
                }

            }
        }
    }

    //增量还原哈希值这两个我在翻转Flip中去实现
    public void Hash_MakeMove(List<int> canMovePos, int pos,BoardManage.BoardPieceColor pieceColor)
    {
        int k, x, y;
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            k = 1;
        else 
            k = 0;
        //减去原来有的颜色，再变成现在有的
        //白翻黑的

        //先抹去白色
        //再加上黑色
        foreach(int i in canMovePos)
        {
            x = i / 8;
            y = i % 8;
            //减去原有颜色
            nHashKey32 = nHashKey32 ^ HashKey32[k, x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[k, x, y];

            //变成现有颜色
            nHashKey32 = nHashKey32 ^ HashKey32[SwapK(k), x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[SwapK(k), x, y];

            
        }

        //加上全新pos
        x = pos/ 8;
        y = pos % 8;

        //能够下的本来就是空的 加上就好
        nHashKey32 = nHashKey32 ^ HashKey32[SwapK(k), x, y];
        nHashKey64 = nHashKey64 ^ HashKey64[SwapK(k), x, y];

       

    }

  

    public void Hash_UnMakeMove(List<int> canMovePos,int pos, BoardManage.BoardPieceColor pieceColor)
    {

        int k, x, y;
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            k = 0;
        else
            k = 1;
        
        foreach (int i in canMovePos)
        {
            x = i / 8;
            y = i % 8;
            //删掉所有翻转的
            nHashKey32 = nHashKey32 ^ HashKey32[k, x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[k, x, y];



            //复原
            nHashKey32 = nHashKey32 ^ HashKey32[SwapK(k), x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[SwapK(k), x, y];


        }

        //减去pos添加的
        x = pos / 8;
        y = pos % 8;

        //恢复没下的状态
        nHashKey32 = nHashKey32 ^ HashKey32[k, x, y];
        nHashKey64 = nHashKey64 ^ HashKey64[k, x, y];

        
    } 
    
    int  SwapK(int k)
    {
        if (k == 1)
            return 0;
        else
            return 1;
    }

    //查询哈希表中当前节点数据，是否存在，存在则返回估值
    public double LookUpHashTable(int depth,BoardManage.BoardPieceColor pieceColor)
    {
        int x;
        HASHITEM[] hashItem;//哈希的引用
        x = nHashKey32 & 0xFFFFF;
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            hashItem = ptt0;
        else
            hashItem = ptt1;
        if (hashItem[x].cnt_depth >= depth && hashItem[x].checksum==nHashKey64)
        {
            return hashItem[x].value;
        }

        return AIMove.INF_NO;
    }

    //将当前值存入哈希表中
    public void EnterHashTable(BoardManage.BoardPieceColor pieceColor, double value, int depth)
    {
        
        int x;
        HASHITEM[] hashItem;
        
        x = nHashKey32 & 0xFFFFF;//计算二十位哈希地址
                                 
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            hashItem = ptt0;
        else
            hashItem = ptt1;
        hashItem[x].cnt_depth = depth;
        hashItem[x].checksum = nHashKey64;
        hashItem[x].value = value;
    }


    //产生64位随机数
    public long Rand64()
    {

        return Random.Range(1, 65537) ^ ((long)Random.Range(1, 65537) << 15) ^ ((long)Random.Range(1, 65537) << 30) ^ ((long)Random.Range(1, 65537) << 45) ^ ((long)Random.Range(1, 65537) << 60);

    }

    //产生32位随机数
    public int Rand32()
    {
        return Random.Range(1, 65537) ^ (Random.Range(1, 65537) << 15) ^ (Random.Range(1, 65537) << 30);
    }
    //初始化随机数组 创建哈希表
    

    //用来生成32位哈希值
    public int[,,] HashKey32 = new int[2, 8, 8];//三个状态 黑白空 棋盘64个位置


    //用来生成64位哈希值
    public long[,,] HashKey64 = new long[2, 8, 8];

    //置换表头
   public HASHITEM[] ptt0; //存放极大值的节点数据
   public HASHITEM[] ptt1;//极小值的节点数据


   public  int nHashKey32;//当前32位哈希值
   public long nHashKey64;//当前64位哈希值？

   public void InitHashKey() 
    {

        int i, j, k;
        //填充随机数组
        for(i = 0;i<2;i++)
        {
            for(j = 0;j<8;j++)
            {
                for(k = 0;k<8;k++)
                {

                    HashKey32[i, j, k] = Rand32();
                    //Debug.Log("随机数看看是不是正确的" + HashKey32[i, j, k]);
                    HashKey64[i, j, k] = Rand64();
                }
            }
        }

        ptt0 = new HASHITEM[1024 * 1024];
        ptt1 = new HASHITEM[1024 * 1024];

    }


}
