using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace gunggme
{
    public class NameSetManager : MonoBehaviour
    {
        [SerializeField] private string[] _speechLines;
        [SerializeField] private TMP_Text _speechUI;

        [SerializeField] private bool _isDone;

        [Header("이름 지정 ui")]
        [SerializeField] private GameObject _nameSetObj;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private GameObject _notUseableNickName;
        [SerializeField] private GameObject _duplicatedNickname;
        
        private void Start()
        {
            StartCoroutine(TypingEffectCoroutine());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isDone && !_nameSetObj.activeSelf)
                {
                    _nameSetObj.SetActive(true);
                }
            }
        }

        StringBuilder stringBuilder = new StringBuilder();

        IEnumerator TypingEffectCoroutine()
        {
            stringBuilder = new StringBuilder();
            foreach (string line in _speechLines)
            {
                yield return TypeLine(line);
                yield return new WaitForSeconds(0.5f); // Optional delay between lines
                 // Add newline after typing each line
            }

            _isDone = true;
        }

        IEnumerator TypeLine(string line)
        {
            //_speechUI.text = string.Empty;

            foreach (var t in line)
            {
                stringBuilder.Append(t);
                     _speechUI.text = stringBuilder.ToString();
                yield return new WaitForSeconds(0.04f);
            }

            stringBuilder.Append("\n");
            _speechUI.text = stringBuilder.ToString();
        }

        public void NameSet()
        {
            if (_nameInputField.text.Length > 0)
            {
                var callback = Backend.BMember.CreateNickname(_nameInputField.text);
                switch (callback.GetStatusCode())
                {
                    case "204":
                        StartCoroutine(SaveManager.Instance.LoadData("InGame"));
                        break;
                    case "409":
                        // 중복되는 닉네임
                        _duplicatedNickname.SetActive(true);
                        break;
                    default:
                        // 사용할 수 없는 닉네임
                        _notUseableNickName.SetActive(true);
                        break;
                }
            }
        }
    }
}
