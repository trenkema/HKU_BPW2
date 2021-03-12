using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FSM : MonoBehaviour
{
    // Game Panels
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject noSavePanel;
    public GameObject gameOverScreen;
    public GameObject gameDetailPanel;
    public GameObject InGameHUD;
    public GameObject inventoryPanel;
    public GameObject gameWinPanel;

    // Game State
    public enum StateEnum { MainMenu, Game, PauseMenu, GameOver, GameWin }
    public StateEnum state { get; private set; }

    // Player Scripts
    public GameObject player;
    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerManager playerManager;

    // Save Scripts
    //public SaveSystem saveSystem;

    // Loading Screen
    public GameObject loadingScreen;
    AsyncOperation loadingOperation;
    public Slider progressBar;
    public TextMeshProUGUI percentLoaded;

    // Parameter Variables
    public float detailFadeTime;

    // References
    public Animator gameOverAnimator;

    private void Awake()
    {
        GameManager.Instance.onPlayerSpawned += assignPlayer;
    }

    private void Start()
    {
        state = (StateEnum)System.Enum.Parse(typeof(StateEnum), PlayerPrefs.GetString("GameFlow", "MainMenu"));
    }

    private void assignPlayer()
    {
        player = GameManager.Instance.playerObject;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerLook = player.GetComponent<PlayerLook>();
        playerManager = player.GetComponent<PlayerManager>();
    }

    private void CheckState()
    {
        switch (state)
        {
            case StateEnum.MainMenu: MainMenuState(); break;
            case StateEnum.Game: GameState(); break;
            case StateEnum.PauseMenu: PauseMenuState(); break;
            case StateEnum.GameOver: GameOverState(); break;
            case StateEnum.GameWin: GameWinState(); break;
        }
    }

    void Update()
    {
        CheckState();
    }

    private void MainMenuState()
    {
    }

    private void GameState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Open PauseMenu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerMovement.enabled = false;
            playerLook.enabled = false;
            playerManager.enabled = false;
            state = StateEnum.PauseMenu;
            PlayerPrefs.SetString("GameFlow", state.ToString());
            pauseMenu.SetActive(true);
            gameWinPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            EnterGameOver();
        }
    }

    private void PauseMenuState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Change to GameState
        if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu.activeSelf == false)
        {
            EnterGameState();
        }
    }

    private void GameOverState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverAnimator.SetBool("GameOver", true);
    }

    public void EnterGameOver()
    {
        GameManager.Instance.inventory.DeleteInventory();
        player.GetComponent<Collider>().enabled = false;
        playerMovement.enabled = false;
        playerLook.enabled = false;
        playerManager.enabled = false;
        state = StateEnum.GameOver;
        pauseMenu.SetActive(false);
        InGameHUD.SetActive(false);
        playerManager.HUD.SetActive(false);
        inventoryPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        PlayerPrefs.SetString("GameFlow", state.ToString());
        gameOverScreen.SetActive(true);
    }

    // Button Click Functions
    public void EnterGameState()
    {
        state = StateEnum.Game;
        PlayerPrefs.SetString("GameFlow", state.ToString());
        playerMovement.enabled = true;
        playerLook.enabled = true;
        playerManager.enabled = true;
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void GameWinState()
    {
        state = StateEnum.GameWin;
        PlayerPrefs.SetString("GameFlow", state.ToString());
        playerMovement.enabled = false;
        playerLook.enabled = false;
        playerManager.enabled = false;
        gameWinPanel.SetActive(true);
    }

    // Resume a Saved Game
    public void ContinueGame(string Scene)
    {
        //bool doesExist;
        //doesExist = saveSystem.PathExists(Scene);
        //if (doesExist == true)
        //{
        //    Debug.Log("Exists");
        //    startLoading(Scene);
        //    EnterGameState();
        //}
        //else if (doesExist == false)
        //{
        //    noSavePanel.SetActive(true);
        //}
    }

    // Quit the Game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Load Level Of Choice
    public void LoadScene(string Scene)
    {
        SceneManager.LoadScene(Scene);
    }

    // Start Next Scene Loading Async
    public void startLoading(string Scene)
    {
        StartCoroutine(LoadSceneAsync(Scene));
    }

    // Load Scene While Showing Loading Screen
    IEnumerator LoadSceneAsync(string Scene)
    {
        loadingScreen.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(Scene);

        while (!loadingOperation.isDone)
        {
            OnLoadLevelProgressUpdate(loadingOperation.progress);
            yield return null;
        }
    }

    // When Loading Progress Made, Show it
    private void OnLoadLevelProgressUpdate(float progress)
    {
        progressBar.value = Mathf.Clamp01(progress / 0.9f);
        percentLoaded.text = Mathf.Round((progress / 0.9f) * 100) + "%";
    }
}
