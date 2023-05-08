using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreBoard : MonoBehaviour
{
    // Start is called before the first frame update
    BoardManage boardManage;
    public HashSet<Tuple<int, int>> avaPut = new HashSet<Tuple<int, int>>();
    void Start()
    {
        
        
    }

    private void Awake()
    {
        boardManage = GetComponent<BoardManage>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //����Ԥ����һ������Ƿ��������õ�
    public bool FindAva(BoardManage.BoardPieceColor boardPieceColor)
    {
        for (int i = 0; i < BoardManage.boardLength; i++)
        {
            for (int j = 0; j < BoardManage.boardLength; j++)
            {
                if (boardManage.BoardPiece[i, j] != BoardManage.BoardPieceColor.NULL)
                {
                    continue;
                }
                if (boardManage.Check_CanPut(i, j, boardPieceColor))
                {
                    return true;
                }
            }
        }
        return false;
    }
    

    //���������ʾ�õ�
    //�ҵ����п��еĽ�Ž�ava��
    //ͬʱ����Ļ����ʾ����

    public bool FindAva_CreatePiece(BoardManage.BoardPieceColor boardPieceColor)
    {
        //��Ѱǰ�����
        //ClearavaPut();
        for(int i = 0;i<BoardManage.boardLength;i++)
        {
            for(int j = 0;j<BoardManage.boardLength;j++)
            {
                if (boardManage.BoardPiece[i, j] != BoardManage.BoardPieceColor.NULL)
                {
                    continue;
                }
                if (boardManage.Check_LegAddAllPiece(i, j, boardPieceColor))
                {
                    avaPut.Add(new Tuple<int, int>(i, j));
                    boardManage.CreatePiece(i, j, BoardManage.BoardPieceColor.NULL);
                    //��Ӿ�������
                    //Debug.Log("�����һ�����µ�λ��"+i+j);
                }
            }
        }
        if(avaPut.Count==0)
            return false;//û�п��Էŵ�����
        return true;//�п��Էŵ�����
    }

    //������AVA�������
    public void ClearavaPut()
    {
        //ɾ����������
        foreach(Tuple<int,int> i in avaPut)
        {
            //ɾ����ʾ
            GameObject.Destroy(boardManage.boardGameObject[i.Item1, i.Item2]);
            //Debug.Log("�ݻ���"+i.Item1+i.Item2);
            
        }
        avaPut.Clear();
    }

    //�ж��Ƿ��ڿ�����
    public bool IsAva(int i,int j)
    {
        if(avaPut.Contains(new Tuple<int, int>(i,j)))
            return true;
        return false;
    }





}
