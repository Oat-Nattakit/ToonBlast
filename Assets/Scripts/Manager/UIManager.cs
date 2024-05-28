using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _panelStart;
    [SerializeField] private Button _btnStart;
    public Button btnStart { get => this._btnStart; }

    [SerializeField] private TextMeshProUGUI _textScore = null;

    [Header("Board Setting Space")]
    [SerializeField] private Slider _widthSetting;
    public int WidthSetting { get => (int)this._widthSetting.value; }
    [SerializeField] private Slider _hightSetting;
    public int HightSetting { get => (int)this._hightSetting.value; }
    [SerializeField] private TextMeshProUGUI _currentSetting = null;


    public void InitUI()
    {
        this._panelStart.SetActive(true);
        this._textScore.text = "Score : 0";
        this._OnSliderValueChange();
        this._UpdatetextBoardSize();
    }

    public void HidePanelStart(bool isHide)
    {
        this._panelStart.SetActive(!isHide);
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

}
