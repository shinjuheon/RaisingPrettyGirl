using System;
using System.Collections;
using System.Collections.Generic;
using gunggme;
using UnityEngine;
using UnityEngine.UI;

public class SaveBtnUI : MonoBehaviour
{
    private Button _button;

    [SerializeField] private GameObject _successText;
    [SerializeField] private GameObject _failedText;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(UpdateData);
    }

    void UpdateData()
    {
        SaveManager.Instance.UpdateData(() =>
        {
            _successText.SetActive(true);
        }, () =>
        {
            _failedText.SetActive(true);
        });
    }
}
