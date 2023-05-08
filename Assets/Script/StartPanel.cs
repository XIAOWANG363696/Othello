using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject GameData;


    public GameObject Choose;
    public GameObject background;
    void Start()
    {
        Choose.SetActive(false);
        background.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    
    public void OnClickBtu1()
    {
        //Êý¾Ý´æ´¢
        //³¡¾°ÇÐ»»
        GameData.GetComponent<GameData>().state = global::GameData.GameState.PVP;
        SceneManager.LoadScene("Game");
        background.SetActive(false);
    }
    public void OnClickBtu2()
    {
        //Êý¾Ý´æ´¢
        //³¡¾°ÇÐ»»
        Choose.SetActive(true);
        background.SetActive(true);
        GameData.GetComponent<GameData>().state = global::GameData.GameState.PVA;
        background.SetActive(true);

    }

    public void OnClickBtu3()
    {
        //Êý¾Ý´æ´¢
        //³¡¾°ÇÐ»»
        GameData.GetComponent<GameData>().state = global::GameData.GameState.AVA;
        SceneManager.LoadScene("Game");
        background.SetActive(false);
    }


    public void OnClickBtu4()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif        
    }

    public void OnClickBlack()
        
    {
        GameData.GetComponent<GameData>().AIPieceColor = BoardManage.BoardPieceColor.White;
        SceneManager.LoadScene("Game");

    }

    public void OnClickWhite()
    {
        GameData.GetComponent<GameData>().AIPieceColor = BoardManage.BoardPieceColor.Black;
        SceneManager.LoadScene("Game");
    }
}
