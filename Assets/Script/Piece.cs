using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
    {
        Black,White,
    }
public class NewBehaviourScript : MonoBehaviour
{
    //当前棋子行列数
    public int row;
    public int col;


    //当前棋子的颜色
    public PieceColor color = PieceColor.Black;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
