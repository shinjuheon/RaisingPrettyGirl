using System.Collections;
using System.Collections.Generic;
using gunggme;
using UnityEngine;

public class ClosePannelUI : MonoBehaviour
{
    IEnumerator GameQuit_Coroutine()
    {
        bool isSave = false;
        SaveManager.Instance.UpdateData(() => isSave = true);
        yield return new WaitUntil(() => isSave);
        
        Application.Quit();
    }
    
    public void CloseGame()
    {
        StartCoroutine(GameQuit_Coroutine());
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
