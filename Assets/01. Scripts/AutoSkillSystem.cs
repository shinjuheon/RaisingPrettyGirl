using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSkillSystem : MonoBehaviour
{
    [SerializeField] private GameObject _fill;

    private void Start()
    {
        _fill.SetActive(PlayerPrefs.GetInt("isAuto") == 1);
    }
}
