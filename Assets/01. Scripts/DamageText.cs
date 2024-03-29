using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private RectTransform _rectTransform;

    [SerializeField] private float _upSpeed;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.anchoredPosition += Vector2.up * _upSpeed * Time.deltaTime;
    }

    public void DisableFunc()
    {
        gameObject.SetActive(false);
    }
}
