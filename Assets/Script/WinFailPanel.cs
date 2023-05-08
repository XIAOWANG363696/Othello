using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinFailPanel : MonoBehaviour
{
    public static WinFailPanel instance;//单例模式的静态字段
    public GameObject player;
    public GameObject whitewin;
    public GameObject blackwin;
    public GameObject drawwin;

    public GameObject background;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;//注册这个单例
        this.gameObject.SetActive(false);//隐藏当前游戏物体
        this.transform.Find("again").GetComponent<Button>().onClick.AddListener(BeginGame);//注册监听
        this.transform.Find("return").GetComponent<Button>().onClick.AddListener(ReturnMenu);//注册监听
        background.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    void Win()
    {
        this.gameObject.SetActive(true);
    }


    //重新加载该局游戏
    public void BeginGame()
    {

        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }


    //返回菜单
    public void ReturnMenu()
    {
        GameObject.Destroy(GameObject.Find("GameData"));
        SceneManager.LoadScene("Start");
        Time.timeScale = 1;
    }

   
}
