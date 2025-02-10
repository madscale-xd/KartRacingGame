using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneButtonManager : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    // Start is called before the first frame update

    void Start()
    {
         Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (finalTimeText != null)
        {
            finalTimeText.text = "Your time:\n"+TimerUI.raceTime;
        }
    }

    public void QuitGame()
        {
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("KartRacer");
    }

    public void LoadEnd()
    {
        SceneManager.LoadScene("EndMenu");
    }

     public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}