using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //����ģʽ�ľ�̬�ֶ�
    public enum GameState
    {
        //��Ϸ������״̬
        //�� VS ��
        //�� VS AI
        //AI VS AI
        PVP,
        PVA,
        AVA,
    }

    public BoardManage.BoardPieceColor AIPieceColor;
    public  GameState state;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        state = GameState.PVP;//Ĭ������Ϊpvp
        AIPieceColor = BoardManage.BoardPieceColor.Black;//Ĭ��Ϊ��ɫ
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
