using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public static MenuPanel instance;//单例模式
    public  GameObject background;
    public  GameObject me;
    public GameObject tips;
    public GameObject rulepanel;
   
    void Start()
    {
        instance = this;
        background.SetActive(false);
        me.SetActive(false);
        tips.SetActive(false);
        rulepanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickMebut()
    {
        Debug.Log("你点到我了,游戏暂停");
        Time.timeScale = 0;//暂停游戏
        background.SetActive(true);
        me.SetActive(true);
    }

    public void OnClickReturn()
    {
        SceneManager.LoadScene("Start");
        GameObject.Destroy(GameObject.Find("GameData"));
        Time.timeScale = 1;//正常速度开始游戏
    }

    public void OnClickcontinue()
    {
        background.SetActive(false);
        me.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickagain()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;//正常速度开始游戏
    }
    public void PopTips()
    {
        tips.SetActive(true);
        Invoke("CloseTips", 1.5f);
    }
    public void CloseTips()
    {
        tips.SetActive(false);
    }

    public void OnclickRule()
    {
        rulepanel.SetActive(true);
        background.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnclickRuleX()
    {
        rulepanel.SetActive(false);
        background.SetActive(false);
        Time.timeScale = 1;
    }

   
}
