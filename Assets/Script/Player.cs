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

//���״̬

public class Player : MonoBehaviour
{
    // Start is called before the first frame update


    //������˼·��Player����������״̬����VS�ˣ���VS����������VS����
   

    //Ŀǰ��Ϸ״̬
   
    public PlayWin playWin;


   

    //��ǰ�Ǻ��������廹�ǰ���������
    BoardManage.BoardPieceColor PlayerPieceColor = BoardManage.BoardPieceColor.Black;

    //������Ϣ
    BoardManage board;
    //����Ԥ����Ϣ
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

        //ͨ��ѡ���趨
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
        


        //����������ж�
        //��ǰ����VS��
        if (Time.timeScale == 0)
            return;

        whitecnt.text = "" + board.GetComponent<BoardManage>().CntWhite;
        blackcnt.text = "" + board.GetComponent<BoardManage>().CntBlack;
        if (gameData.state==GameData.GameState.PVP)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
               

                //�������

                //��������ɹ� �ı�����������û�д������ӳɹ���˵�����Ϸ�����ǰ�����������
                if (board.CreatePiece(PlayerPieceColor, Input.mousePosition))
                {
                    //�����������
                    SwapPlayer();
                    
                    //���������ָö������ӿ��£��ٴν��н�����顣
                    //������˶����ӿ��£�˵����ֽ���
                    if (!preBoard.FindAva_CreatePiece(PlayerPieceColor))
                    {
                        SwapPlayer();
                        PlayerIsContinue(PlayerPieceColor);
                    }
                }
                else
                    Debug.Log("��ǰ���ּ�������");

                


            }
        }


        //��ǰ����VS����
        else if(gameData.state == GameData.GameState.PVA)
        {
            //�����ǰ�ǻ�����������
            if (AIplayer.pieceColor == PlayerPieceColor)
            {
                //Debug.Log("����������");

                //���һ�����µ�λ��
                preBoard.ClearavaPut();

                if (AIplayer.AICreatePiece(board))
                {
                    SwapPlayer();
                    
                    //���һ������ܷ�����ɹ�
                    if(!preBoard.FindAva_CreatePiece(PlayerPieceColor))
                    {
                        Debug.Log("��Ҳ����£����ɻ����˼��һ��");
                        SwapPlayer();
                        AIIsCotinue(PlayerPieceColor);
                    }
                }
                else
                {
                    Debug.Log("AI�����ʧ�ܣ����״��������˵��Ҫ��Bug��");
                }
                
            }
            //�����ǰ����������� ������
            else
            {
                
                //ֻ��������˲��з�Ӧ
                if (Input.GetMouseButtonDown(0))
                {
                   
                        //Debug.Log("�������");
                    if (board.CreatePiece(PlayerPieceColor,Input.mousePosition))
                    {
                        SwapPlayer();

                        //���AIû�п����µ���
                        if(!preBoard.FindAva(PlayerPieceColor))
                        {
                            SwapPlayer();
                            PlayerIsContinue(PlayerPieceColor);
                        }
                    }
                }
            }


        }




        //����VS����
        else
        {
            if (AIplayer.AICreatePiece(board))
            {
                //if (PlayerPieceColor == BoardManage.BoardPieceColor.Black)
                //    Debug.Log("������������");
                //else
                //    Debug.Log("������������");
                SwapPlayer();
                AIplayer.SetAIPieceColor(PlayerPieceColor);

            }
            //����²����壬��������·���ߣ��ж�����Ϸ����������·����
            else
            {
                SwapPlayer();
                AIIsCotinue(PlayerPieceColor);
            }


        }




        //�����ʱ�Ѿ�����ʤ����
       //�����˻����˶�����ͨ�õ�



            //�����Ϸ״̬
            //��װΪ����
        if (playWin != PlayWin.NoWin)
        {
            Invoke("OutFailPanel", 0.5f);


        }



    }


    public void PlayerIsContinue(BoardManage.BoardPieceColor boardPieceColor)
    {
        //�����Ϸ�ǿ��Լ�����
       if( preBoard.FindAva_CreatePiece(boardPieceColor))
        {
            //������ʾ
            menu.GetComponent<MenuPanel>().PopTips();
            Debug.Log("��·�����˸�磬���˿�~");
            
        }
       else
        {
            //��ֵ�״̬�����仯
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





    //AI���ж�
    public void AIIsCotinue(BoardManage.BoardPieceColor boardPieceColor)
    {
        if(preBoard.FindAva(boardPieceColor))
        {
            //������ʾ
            menu.GetComponent<MenuPanel>().PopTips();

            Debug.Log("��·�����˸�磬���˿�~");
        }
        else
        {
            //��ֵ�״̬�����仯
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
            Debug.Log("��Ӯ��~~~������û��~~");
        }
        else if(playWin==PlayWin.BlackWin)
        {
            winfailpanel.GetComponent<WinFailPanel>().whitewin.SetActive(false);
            winfailpanel.GetComponent<WinFailPanel>().drawwin.SetActive(false);
            Debug.Log("��~~~~~~���ǿ~~");
        }
        else 
        {
            winfailpanel.GetComponent<WinFailPanel>().blackwin.SetActive(false);
            winfailpanel.GetComponent<WinFailPanel>().whitewin.SetActive(false);
            Debug.Log("ƽ�ֿ�~~~~~~~");
        }
    }
}
