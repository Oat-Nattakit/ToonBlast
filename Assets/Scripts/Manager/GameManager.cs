using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private UIManager _uiManager;
    public UIManager uiManager { get => this._uiManager; }

    [SerializeField] private GameplayManager _gameplay;
    public GameplayManager GameplayManager { get => this._gameplay; }

    public NodePooling _nodePooling;
   

    [Header("Board Space")]
    [HideInInspector] public int BoardWidth = 8;
    [HideInInspector] public int BoardHight = 8;

    [SerializeField] private RectTransform _refScreenSize;

    [Header("Symbol size")]
    [HideInInspector] public int spacingX = 101;
    [HideInInspector] public int spacingY = 101;

    [Header("Target Spawn Special")]
    public int LimitBomb = 0;
    public int LimitDisco = 0;

    private int _currentScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this._reset();
        this._initUI();
    }

    private void _reset()
    {
        this._currentScore = 0;
        this._gameplay.ResetBoard();
        this._nodePooling.Clear();        
    }

    private void _initUI()
    {
        this._uiManager.InitUI();
        this._InitButton();
    }

    private void _InitButton()
    {
        var startBtn = this._uiManager.btnStart;
        var exitGame = this._uiManager.btnExit;
        var settingGame = this._uiManager.btnSetting;
        var exSettingGame = this._uiManager.btnExitSetting;

        startBtn.onClick.AddListener(() =>
        {
            this._uiManager.showPanelGameplay(true);
            this._initGamePlay();
        });

        exitGame.onClick.AddListener(() =>
        {
            this._uiManager.showPanelGameplay(false);
            this._reset();
        });

        settingGame.onClick.AddListener(() => {
            this._uiManager.ClickSetting(true);
        });

        exSettingGame.onClick.AddListener(() => {
            this._uiManager.ClickSetting(false);
        });
    }

    private void _initGamePlay()
    {
        this.BoardHight = this._uiManager.HightSetting;
        this.BoardWidth = this._uiManager.WidthSetting;
        this._checkScreenSize();
        this._gameplay.Init();
        this._gameplay.InitCallbackScore(this._CalculateScore);
    }

    private void _CalculateScore(int score)
    {
        int baseScore = 1;
        int scoreValue = score / 2;
        int multiply = (score / 3);
        int ScoreValue = (baseScore * scoreValue) + (multiply * baseScore);
        this._currentScore += ScoreValue;
        this._uiManager.UpdateScore(this._currentScore);
    }

    private void _checkScreenSize()
    {
        int AvaScreenWidth = (int)this._refScreenSize.rect.width;
        int AvaScreenHidth = (int)this._refScreenSize.rect.height;
       
        var findSizeX = (AvaScreenWidth / this.BoardWidth);
        var findSizeY = (AvaScreenHidth / this.BoardHight);

        bool useSizeX = this.spacingX <= findSizeX;
        bool useSizeY = this.spacingY <= findSizeY;

        if (useSizeX != true || useSizeY != true)
        {
            int size = (findSizeX <= findSizeY) ? findSizeX : findSizeY;

            this.spacingX = size;
            this.spacingY = size;
        }       
    }
}
