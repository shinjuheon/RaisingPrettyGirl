using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotoMainBtn : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LogoutGame);
    }

    void LogoutGame()
    {
        Backend.BMember.Logout(callback =>
        {
            if (callback.IsSuccess())
            {
                SceneManager.LoadScene("Main");
            }
        });
    }
}
