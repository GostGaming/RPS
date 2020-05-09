using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    GameObject[] pauseObjects;
    public Text handCount;
    public Text handCountbg;
    public Text turnCount;
    public Text turnCountbg;
    public ResourceManager resourceManager;
    public string currentScene;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "MainScene") {
            Time.timeScale = 1;
            pauseObjects = GameObject.FindGameObjectsWithTag("PauseUI");
            hidePaused();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentScene == "MainScene") {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                pauseSwitch();
            }
            handCount.text = "Hand Count: " + resourceManager.Hands;
            handCountbg.text = "Hand Count: " + resourceManager.Hands;
            turnCount.text = "Turn Count: " + resourceManager.turns;
            turnCountbg.text = "Turn Count: " + resourceManager.turns;
        }
    }
    // switch pause state
    public void pauseSwitch() {
        if (Time.timeScale == 1) {
            Time.timeScale = 0;
            showPaused();
        } else if (Time.timeScale == 0) {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    // Shows pause text and buttons, hides healthbar   
    private void showPaused() {
        foreach(GameObject obj in pauseObjects) {
            obj.SetActive(true);
        }
    }
    // hide pause text and buttons, shows healthbar
    private void hidePaused() {
        foreach(GameObject obj in pauseObjects) {
            obj.SetActive(false);
        }
    }    
    
    public void LoadScene() {
        if(currentScene == "MainScene") {
            SceneManager.LoadScene("MainMenu");
        }
        else SceneManager.LoadScene("MainScene");
    }

    public void ExitButton() {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void RestartLevel() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
