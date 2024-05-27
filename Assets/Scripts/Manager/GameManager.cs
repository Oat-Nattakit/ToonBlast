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

    [Header("Board Space")]
    public int BoardWidth = 8;
    public int BoardHight = 8;

    [Header("Symbol size")]
    public int spacingX = 0;
    public int spacingY = 0;

    [Header("Target Spawn Special")]
    public int LimitBomb = 0;
    public int LimitDisco = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this._uiManager.InitUI();

        this._InitBtnStart();
    }

    private void _InitBtnStart()
    {
        var startBtn = this._uiManager.btnStart;
        startBtn.onClick.AddListener(() =>
        {
            this._uiManager.HidePanelStart(true);
            this._gameplay.Init();
        });
    }
}
