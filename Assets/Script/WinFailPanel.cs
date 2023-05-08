using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinFailPanel : MonoBehaviour
{
    public static WinFailPanel instance;//����ģʽ�ľ�̬�ֶ�
    public GameObject player;
    public GameObject whitewin;
    public GameObject blackwin;
    public GameObject drawwin;

    public GameObject background;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;//ע���������
        this.gameObject.SetActive(false);//���ص�ǰ��Ϸ����
        this.transform.Find("again").GetComponent<Button>().onClick.AddListener(BeginGame);//ע�����
        this.transform.Find("return").GetComponent<Button>().onClick.AddListener(ReturnMenu);//ע�����
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


    //���¼��ظþ���Ϸ
    public void BeginGame()
    {

        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }


    //���ز˵�
    public void ReturnMenu()
    {
        GameObject.Destroy(GameObject.Find("GameData"));
        SceneManager.LoadScene("Start");
        Time.timeScale = 1;
    }

   
}
