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


    public void InitUI()
    {
        this._panelStart.SetActive(true);
        this._textScore.text = "Score : 0";
    }

    public void HidePanelStart(bool isHide)
    {
        this._panelStart.SetActive(!isHide);
    }

    public void UpdateScore(int score)
    {
        this._textScore.text = "Score : " + score.ToString();
    }

}
