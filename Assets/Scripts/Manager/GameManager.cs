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

        this._initBtnStart();
    }

    private void _initBtnStart()
    {
        var startBtn = this._uiManager.btnStart;
        startBtn.onClick.AddListener(() =>
        {
            this._uiManager.HidePanelStart(true);
            this._gameplay.Init();
        });
    }
}
