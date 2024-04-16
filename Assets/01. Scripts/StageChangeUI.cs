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

    [SerializeField]private int _changedStage;

    public GameObject stageChangeMaxInfoObj;
    public GameObject stageChangeMiniInfoObj;
    public GameObject stageChangeUI;
    private void Awake()
    {
        _stageManager = FindObjectOfType<StageManager>();
    }

    private void OnEnable()
    {
        _stageManager = FindObjectOfType<StageManager>();

        _changedStage = _stageManager.CurrentStage;
        _curStageText.text = $"{_changedStage}";

        Debug.Log(_curStageText);
    }

    //public void StageNumChange(int num)
    //{
    //    // 현재 스테이지에서 +1 더한게 서버에 저장된 최고 스테이지보다 크거나 || 현재 스테이지에서 +1 더한게 0보다 작거나 같다면
    //    //Debug.Log(_changedStage + num > SaveManager.Instance.StageCoupon.MaxStage || _changedStage + num <= 0);

    //    if (_changedStage+(num) == SaveManager.Instance.StageCoupon.MaxStage) // 이 창을 열었을때 현재스테이지 6 + 1 맥스스테이지 7 이라면
    //    {// 현재 스테이지와 최고 스테이지 값이 같다면
    //        _changedStage = SaveManager.Instance.StageCoupon.MaxStage;
    //        _curStageText.text = $"{_changedStage}";
    //    }
    //    else if (_changedStage == SaveManager.Instance.StageCoupon.MaxStage) // 현재 스테이지와 맥스 스테이지가 같다면 
    //    {
    //        StartCoroutine(WarningTextActiveTime(stageChangeWarningObj, 1.5f));
    //    }
    //    else if (_changedStage + (num) < SaveManager.Instance.StageCoupon.MaxStage) // 현재 스테이지 5 +-1 = 맥스 스테이지7가 더 크다면
    //    {

    //    }
    //    if (_changedStage + (num) > SaveManager.Instance.StageCoupon.MaxStage || _changedStage + (num) <= 0) return;
    //    _changedStage += num;
    //    _curStageText.text = $"{_changedStage}";
    //}

    public void StageChangePlus()
    {
        _changedStage++; // 5  //8
        if (_changedStage >= SaveManager.Instance.StageCoupon.MaxStage)
        {
            _changedStage = SaveManager.Instance.StageCoupon.MaxStage;
            StartCoroutine(MaxStageTextActiveTime(stageChangeMaxInfoObj, 1.5f));
        }
        _curStageText.text = $"{_changedStage}";
    }
    public void StageChangeMinus()
    {
        _changedStage--;
        if (_changedStage <= 1)
        {
            _changedStage = 1;
            StartCoroutine(MinimumStageTextActiveTime(stageChangeMiniInfoObj, 1.5f));
        }
        _curStageText.text = $"{_changedStage}";
    }

    public void ChangeStage()
    {
        _stageManager.SetStage(_changedStage, 0);
        stageChangeUI.SetActive(false);
    }

    IEnumerator MaxStageTextActiveTime(GameObject obj, float time)
    {
        obj.SetActive(true);

        yield return new WaitForSeconds(time);

        obj.SetActive(false);
    }

    IEnumerator MinimumStageTextActiveTime(GameObject obj, float time)
    {
        obj.SetActive(true);

        yield return new WaitForSeconds(time);

        obj.SetActive(false);
    }
}
