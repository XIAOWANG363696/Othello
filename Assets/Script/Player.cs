using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PlayWin
    {
        BlackWin,
        WhiteWin,
        NoWin,
        DrawWin,
    }

//玩家状态

public class Player : MonoBehaviour
{
    // Start is called before the first frame update


    //待完善思路：Player自身有三个状态，人VS人，人VS机器，机器VS机器
   

    //目前游戏状态
   
    public PlayWin playWin;


   

    //当前是黑棋手下棋还是白棋手下棋
    BoardManage.BoardPieceColor PlayerPieceColor = BoardManage.BoardPieceColor.Black;

    //棋盘信息
    BoardManage board;
    //棋盘预测信息
    PreBoard preBoard;

    GameData gameData;

    
   public AIplayer AIplayer;

    public GameObject menu;

    public GameObject winfailpanel;

    public Text whitecnt;
    public Text blackcnt;
    void Start()
    {
        
        playWin = PlayWin.NoWin;

        //通过选择设定
        AIplayer.SetAIPieceColor(gameData.AIPieceColor);
        if(gameData.state==GameData.GameState.AVA)
        {
            preBoard.ClearavaPut();
        }


    }
    private void Awake()
    {
        board= GameObject.Find("board").GetComponent<BoardManage>();
        preBoard = GameObject.Find("board").GetComponent<PreBoard>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        AIplayer = GetComponent<AIplayer>();
    }
    // Update is called once per frame
    void Update()
    {
        


        //鼠标点击进行判断
        //当前是人VS人
        if (Time.timeScale == 0)
            return;

        whitecnt.text = "" + board.GetComponent<BoardManage>().CntWhite;
        blackcnt.text = "" + board.GetComponent<BoardManage>().CntBlack;
        if (gameData.state==GameData.GameState.PVP)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
               

                //添加落子

                //如果创建成功 改变下棋对象，如果没有创建棋子成功，说明不合法，当前对象继续下棋
                if (board.CreatePiece(PlayerPieceColor, Input.mousePosition))
                {
                    //交换下棋对象
                    SwapPlayer();
                    
                    //交换过后发现该对象无子可下，再次进行交换检查。
                    //如果两人都无子可下，说明棋局结束
                    if (!preBoard.FindAva_CreatePiece(PlayerPieceColor))
                    {
                        SwapPlayer();
                        PlayerIsContinue(PlayerPieceColor);
                    }
                }
                else
                    Debug.Log("当前棋手继续下棋");

                


            }
        }


        //当前是人VS机器
        else if(gameData.state == GameData.GameState.PVA)
        {
            //如果当前是机器人在下棋
            if (AIplayer.pieceColor == PlayerPieceColor)
            {
                //Debug.Log("机器人下棋");

                //清空一下能下的位置
                preBoard.ClearavaPut();

                if (AIplayer.AICreatePiece(board))
                {
                    SwapPlayer();
                    
                    //检测一下玩家能否下棋成功
                    if(!preBoard.FindAva_CreatePiece(PlayerPieceColor))
                    {
                        Debug.Log("玩家不能下！换成机器人检测一下");
                        SwapPlayer();
                        AIIsCotinue(PlayerPieceColor);
                    }
                }
                else
                {
                    Debug.Log("AI下棋大失败，这个状况出现了说明要改Bug了");
                }
                
            }
            //如果当前是玩家在下棋 检测鼠标
            else
            {
                
                //只有鼠标点击了才有反应
                if (Input.GetMouseButtonDown(0))
                {
                   
                        //Debug.Log("玩家下棋");
                    if (board.CreatePiece(PlayerPieceColor,Input.mousePosition))
                    {
                        SwapPlayer();

                        //如果AI没有可以下的棋
                        if(!preBoard.FindAva(PlayerPieceColor))
                        {
                            SwapPlayer();
                            PlayerIsContinue(PlayerPieceColor);
                        }
                    }
                }
            }


        }




        //机器VS机器
        else
        {
            if (AIplayer.AICreatePiece(board))
            {
                //if (PlayerPieceColor == BoardManage.BoardPieceColor.Black)
                //    Debug.Log("黑棋手下完了");
                //else
                //    Debug.Log("白棋手下完了");
                SwapPlayer();
                AIplayer.SetAIPieceColor(PlayerPieceColor);

            }
            //如果下不了棋，可能是无路可走，判断是游戏结束还是无路可走
            else
            {
                SwapPlayer();
                AIIsCotinue(PlayerPieceColor);
            }


        }




        //如果此时已经决出胜负！
       //机器人还是人都可以通用的



            //检测游戏状态
            //封装为函数
        if (playWin != PlayWin.NoWin)
        {
            Invoke("OutFailPanel", 0.5f);


        }



    }


    public void PlayerIsContinue(BoardManage.BoardPieceColor boardPieceColor)
    {
        //如果游戏是可以继续的
       if( preBoard.FindAva_CreatePiece(boardPieceColor))
        {
            //弹出提示
            menu.GetComponent<MenuPanel>().PopTips();
            Debug.Log("无路可走了哥哥，换人咯~");
            
        }
       else
        {
            //棋局的状态发生变化
            if(board.CntBlack>board.CntWhite)
            {
                
                playWin = PlayWin.BlackWin;

            }
            else if(board.CntBlack < board.CntWhite)
            {
                playWin = PlayWin.WhiteWin;
            }
            else 
            {
                playWin = PlayWin.DrawWin;
            }
        }
    }





    //AI的判断
    public void AIIsCotinue(BoardManage.BoardPieceColor boardPieceColor)
    {
        if(preBoard.FindAva(boardPieceColor))
        {
            //弹出提示
            menu.GetComponent<MenuPanel>().PopTips();

            Debug.Log("无路可走了哥哥，换人咯~");
        }
        else
        {
            //棋局的状态发生变化
            if (board.CntBlack > board.CntWhite)
            {

                playWin = PlayWin.BlackWin;

            }
            else if (board.CntBlack<board.CntWhite)
            {
                playWin = PlayWin.WhiteWin;
            }
            else
            {
                playWin = PlayWin.DrawWin;
            }

        }

    }



    public void SwapPlayer()
    {
        if (PlayerPieceColor == BoardManage.BoardPieceColor.Black)
            PlayerPieceColor = BoardManage.BoardPieceColor.White;
        else if (PlayerPieceColor == BoardManage.BoardPieceColor.White)
            PlayerPieceColor = BoardManage.BoardPieceColor.Black;
    }



    public BoardManage.BoardPieceColor SwapType(BoardManage.BoardPieceColor pieceColor)
    {
        if (pieceColor == BoardManage.BoardPieceColor.Black)
            return BoardManage.BoardPieceColor.White;
        else if (pieceColor == BoardManage.BoardPieceColor.White)
            return BoardManage.BoardPieceColor.Black;
        else
            return BoardManage.BoardPieceColor.NULL;
    }




    public void PointOut()
    {
        AIplayer.AICreatePoint(board, PlayerPieceColor);
    }


    public void OutFailPanel()
    {
        winfailpanel.SetActive(true);
        winfailpanel.GetComponent<WinFailPanel>().background.SetActive(true);

        if (playWin==PlayWin.WhiteWin)
        {
            winfailpanel.GetComponent<WinFailPanel>().blackwin.SetActive(false);
            winfailpanel.GetComponent<WinFailPanel>().drawwin.SetActive(false);
            Debug.Log("白赢咯~~~黑子真没用~~");
        }
        else if(playWin==PlayWin.BlackWin)
        {
            winfailpanel.GetComponent<WinFailPanel>().whitewin.SetActive(false);
            winfailpanel.GetComponent<WinFailPanel>().drawwin.SetActive(false);
            Debug.Log("黑~~~~~~你好强~~");
        }
        else 
        {
            winfailpanel.GetComponent<WinFailPanel>().blackwin.SetActive(false);
            winfailpanel.GetComponent<WinFailPanel>().whitewin.SetActive(false);
            Debug.Log("平局咯~~~~~~~");
        }
    }
}
