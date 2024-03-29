using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurVersionText : MonoBehaviour
{
    private TMP_Text _verText;
    
    void Awake()
    {
        _verText = GetComponent<TMP_Text>();
        _verText.text = $"{Application.version}v";
    }
}
