using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //单例模式的静态字段
    public enum GameState
    {
        //游戏的三种状态
        //人 VS 人
        //人 VS AI
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
        state = GameState.PVP;//默认设置为pvp
        AIPieceColor = BoardManage.BoardPieceColor.Black;//默认为黑色
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
