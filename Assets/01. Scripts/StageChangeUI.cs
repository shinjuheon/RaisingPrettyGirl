using System;
using System.Collections;
using System.Collections.Generic;
using gunggme;
using TMPro;
using UnityEngine;

public class StageChangeUI : MonoBehaviour
{
    [SerializeField] private StageManager _stageManager;

    [SerializeField] private TMP_Text _curStageText;

    private int _changedStage;
    
    private void Awake()
    {
        _stageManager = FindObjectOfType<StageManager>();
    }

    private void OnEnable()
    {
        _changedStage = _stageManager.CurrentStage;
        _curStageText.text = $"{_changedStage}";
    }

    public void StageNumChange(int num)
    {
        Debug.Log(_changedStage + num > SaveManager.Instance.StageCoupon.MaxStage || _changedStage + num <= 0);
        if (_changedStage + num > SaveManager.Instance.StageCoupon.MaxStage || _changedStage + num <= 0) return;
        _changedStage += num;
        _curStageText.text = $"{_changedStage}";
    }

    public void ChangeStage()
    {
        _stageManager.SetStage(_changedStage, 0);
        gameObject.SetActive(false);
    }
}
