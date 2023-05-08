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

    //这是预测下一个玩家是否还能行走用的
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
    

    //这是玩家提示用的
    //找到所有可行的解放进ava中
    //同时在屏幕上显示出来

    public bool FindAva_CreatePiece(BoardManage.BoardPieceColor boardPieceColor)
    {
        //搜寻前先清空
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
                    //添加精灵物体
                    //Debug.Log("添加了一个可下的位置"+i+j);
                }
            }
        }
        if(avaPut.Count==0)
            return false;//没有可以放的棋子
        return true;//有可以放的棋子
    }

    //将所有AVA进行清空
    public void ClearavaPut()
    {
        //删除精灵物体
        foreach(Tuple<int,int> i in avaPut)
        {
            //删除显示
            GameObject.Destroy(boardManage.boardGameObject[i.Item1, i.Item2]);
            //Debug.Log("摧毁了"+i.Item1+i.Item2);
            
        }
        avaPut.Clear();
    }

    //判断是否在可行中
    public bool IsAva(int i,int j)
    {
        if(avaPut.Contains(new Tuple<int, int>(i,j)))
            return true;
        return false;
    }





}
