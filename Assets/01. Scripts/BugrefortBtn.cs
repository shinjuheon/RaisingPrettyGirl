using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BugrefortBtn : MonoBehaviour
{
    [SerializeField] private string _cafeURL;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick_Btn);
    }


    void OnClick_Btn()
    {
        Application.OpenURL(_cafeURL);
    }
}
