using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//��ϣ����Ԫ�صĽṹ
public struct HASHITEM
{
    public long checksum;//64λУ����
    public int cnt_depth;//������+����
    public double value;//��ֵ
};





//�û���
public class TransPositionTabe 
{
    //���캯�� ˵ʵ����Ҳ��֪��Ҫ��ʲô,�Ҳ�Ӧ���ǳ�ʼ����ϣ��
     public TransPositionTabe() {

        InitHashKey();
    }
    //��������
     ~TransPositionTabe() { }

    
    //�������̵Ĺ�ϣֵ
    //��ʼ�����̿�����һ��
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

    //������ԭ��ϣֵ���������ڷ�תFlip��ȥʵ��
    public void Hash_MakeMove(List<int> canMovePos, int pos,BoardManage.BoardPieceColor pieceColor)
    {
        int k, x, y;
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            k = 1;
        else 
            k = 0;
        //��ȥԭ���е���ɫ���ٱ�������е�
        //�׷��ڵ�

        //��Ĩȥ��ɫ
        //�ټ��Ϻ�ɫ
        foreach(int i in canMovePos)
        {
            x = i / 8;
            y = i % 8;
            //��ȥԭ����ɫ
            nHashKey32 = nHashKey32 ^ HashKey32[k, x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[k, x, y];

            //���������ɫ
            nHashKey32 = nHashKey32 ^ HashKey32[SwapK(k), x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[SwapK(k), x, y];

            
        }

        //����ȫ��pos
        x = pos/ 8;
        y = pos % 8;

        //�ܹ��µı������ǿյ� ���Ͼͺ�
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
            //ɾ�����з�ת��
            nHashKey32 = nHashKey32 ^ HashKey32[k, x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[k, x, y];



            //��ԭ
            nHashKey32 = nHashKey32 ^ HashKey32[SwapK(k), x, y];
            nHashKey64 = nHashKey64 ^ HashKey64[SwapK(k), x, y];


        }

        //��ȥpos��ӵ�
        x = pos / 8;
        y = pos % 8;

        //�ָ�û�µ�״̬
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

    //��ѯ��ϣ���е�ǰ�ڵ����ݣ��Ƿ���ڣ������򷵻ع�ֵ
    public double LookUpHashTable(int depth,BoardManage.BoardPieceColor pieceColor)
    {
        int x;
        HASHITEM[] hashItem;//��ϣ������
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

    //����ǰֵ�����ϣ����
    public void EnterHashTable(BoardManage.BoardPieceColor pieceColor, double value, int depth)
    {
        
        int x;
        HASHITEM[] hashItem;
        
        x = nHashKey32 & 0xFFFFF;//�����ʮλ��ϣ��ַ
                                 
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            hashItem = ptt0;
        else
            hashItem = ptt1;
        hashItem[x].cnt_depth = depth;
        hashItem[x].checksum = nHashKey64;
        hashItem[x].value = value;
    }


    //����64λ�����
    public long Rand64()
    {

        return Random.Range(1, 65537) ^ ((long)Random.Range(1, 65537) << 15) ^ ((long)Random.Range(1, 65537) << 30) ^ ((long)Random.Range(1, 65537) << 45) ^ ((long)Random.Range(1, 65537) << 60);

    }

    //����32λ�����
    public int Rand32()
    {
        return Random.Range(1, 65537) ^ (Random.Range(1, 65537) << 15) ^ (Random.Range(1, 65537) << 30);
    }
    //��ʼ��������� ������ϣ��
    

    //��������32λ��ϣֵ
    public int[,,] HashKey32 = new int[2, 8, 8];//����״̬ �ڰ׿� ����64��λ��


    //��������64λ��ϣֵ
    public long[,,] HashKey64 = new long[2, 8, 8];

    //�û���ͷ
   public HASHITEM[] ptt0; //��ż���ֵ�Ľڵ�����
   public HASHITEM[] ptt1;//��Сֵ�Ľڵ�����


   public  int nHashKey32;//��ǰ32λ��ϣֵ
   public long nHashKey64;//��ǰ64λ��ϣֵ��

   public void InitHashKey() 
    {

        int i, j, k;
        //����������
        for(i = 0;i<2;i++)
        {
            for(j = 0;j<8;j++)
            {
                for(k = 0;k<8;k++)
                {

                    HashKey32[i, j, k] = Rand32();
                    //Debug.Log("����������ǲ�����ȷ��" + HashKey32[i, j, k]);
                    HashKey64[i, j, k] = Rand64();
                }
            }
        }

        ptt0 = new HASHITEM[1024 * 1024];
        ptt1 = new HASHITEM[1024 * 1024];

    }


}
