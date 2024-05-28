using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _panelStart;

    [Header("Panel Button")]
    [SerializeField] private GameObject _panelButton;
    [SerializeField] private Button _btnStart;
    public Button btnStart { get => this._btnStart; }   
    [SerializeField] private Button _btnSetting;
    public Button btnSetting { get => this._btnSetting; }

    [Header("Board Setting Space")]
    [SerializeField] private GameObject _panelSetting = null;    
    [SerializeField] private Button _exitbtnSetting;
    public Button btnExitSetting { get => this._exitbtnSetting; }
    [SerializeField] private Slider _widthSetting;
    public int WidthSetting { get => (int)this._widthSetting.value; }
    [SerializeField] private Slider _hightSetting;
    public int HightSetting { get => (int)this._hightSetting.value; }
    [SerializeField] private TextMeshProUGUI _currentSetting = null;

    [Header("Game UI")]
    [SerializeField] private GameObject _panelGamePlay;
    [SerializeField] private TextMeshProUGUI _textScore = null;
    [SerializeField] private Button _btnExit;
    public Button btnExit { get => this._btnExit; }    

    public void InitUI()
    {
        this._panelStart.SetActive(true);
        this._textScore.text = "Score : 0";
        this._OnSliderValueChange();
        this._UpdatetextBoardSize();
    }

    public void showPanelGameplay(bool isShow)
    {
        this._panelStart.SetActive(!isShow);
        this._panelGamePlay.SetActive(isShow);
    }

    public void UpdateScore(int score)
    {
        this._textScore.text = "Score : " + score.ToString();
    }

    private void _OnSliderValueChange()
    {
        this._widthSetting.onValueChanged.AddListener((value)=> { this._UpdatetextBoardSize();});
        this._hightSetting.onValueChanged.AddListener((value)=> { this._UpdatetextBoardSize();});
    }

    private void _UpdatetextBoardSize()
    {        
        this._currentSetting.text = "Board Size : " + this._widthSetting.value + " : " + this._hightSetting.value;
    }

    public void ClickSetting(bool isShow)
    {
        this._panelButton.SetActive(!isShow);
        this._panelSetting.SetActive(isShow);
        //this.pa
    }

}
