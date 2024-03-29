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
        _loginManager.LoginFailed();
        Backend.BMember.Logout();
        
        //즉시 탈퇴
    }
}
