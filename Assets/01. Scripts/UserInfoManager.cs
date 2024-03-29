using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;

namespace gunggme
{
    public class UserInfoManager : MonoBehaviour
    {
        public UserInfoData _userinfoData = new UserInfoData();
        [SerializeField] private TMP_Text text;

        public void Start()
        {
            text.text = SaveManager.Instance.UserInfo.nickname;
        }
    }
}
