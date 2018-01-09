using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour {

    public void ToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void ToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

}
