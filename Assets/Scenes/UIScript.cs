using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    // Start is called before the first frame update
     Button button;
    public GameObject instructionsPanel;
    public GameObject menuPanel;
    public Button instructionbutton;
    public GameObject settingPanel;
    public Button backButton;
    public Button startButton;
    public Button settingButton;
     void Start()
    {
        instructionbutton.onClick.AddListener(Instruction);
        backButton.onClick.AddListener(Menu);
        startButton.onClick.AddListener(GameStart);
        settingButton.onClick.AddListener(Settings);
    }

    // Update is called once per frame
    void Update()
    {

        if (menuPanel.activeSelf)
            backButton.gameObject.SetActive(false);
        else
            backButton.gameObject.SetActive(true);
        // menuPanel.SetActive(true);
        
    }
    public void Instruction()
    {
        instructionsPanel.SetActive(true);
        settingPanel.SetActive(false);
        menuPanel.SetActive(false);

    }
    public void Menu()
    {
        instructionsPanel.SetActive(false);
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    public void Settings()
    {
        instructionsPanel.SetActive(false);
        settingPanel.SetActive(true);
        menuPanel.SetActive(false);

    }
    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }
}
