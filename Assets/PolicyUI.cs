using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using gunggme;

public class PolicyUI : MonoBehaviour
{
    private LoginManager _loginManager;

    private void Awake()
    {
        _loginManager = FindObjectOfType<LoginManager>();

        // LoginManager를 찾지 못한 경우 예외 처리
        if (_loginManager == null)
        {
            Debug.LogError("LoginManager를 찾을 수 없습니다.");
        }
    }

    public void OpenUI()
    {
        gameObject.SetActive(true);
    }

    public void Accept()
    {
        SaveManager.Instance.InitData();
    }

    public void DeAccept()
    {
        
        if (_loginManager != null)
        {
            _loginManager.LoginFailed();
        }
        else
        {
            Debug.LogWarning("_loginManager가 null입니다.");
        }

        Backend.BMember.Logout();

        //즉시 탈퇴
    }
}
