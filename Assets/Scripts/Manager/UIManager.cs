using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _panelStart;
    [SerializeField] private Button _btnStart;
    public Button btnStart { get => this._btnStart; }


    public void InitUI()
    {
        this._panelStart.SetActive(true);
    }

    public void HidePanelStart(bool isHide)
    {
        this._panelStart.SetActive(!isHide);
    }

}
