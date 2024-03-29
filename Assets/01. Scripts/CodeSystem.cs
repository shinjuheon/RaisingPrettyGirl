using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class CodeSystem : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        private TMP_Text _placeHolder;

        private List<Dictionary<string, object>> _codeData;

        public List<string> UsedCode;

        private GoodsManager _goodsManager;

        [Header("텍스트")] public GameObject SuccessText;
        public GameObject FailText;


        private void Awake()
        {
            
            _codeData = CSVReader.Read("CodeFile");
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();

            _placeHolder = _inputField.placeholder.GetComponent<TMP_Text>();

        }

        private void Start()
        {
            UsedCode = SaveManager.Instance.StageCoupon.UseCoupon;
            ChgPlaceHolder();
        }

        public void OnEndEdit()
        {
            _inputField.text = "";
            ChgPlaceHolder();
        }
        
        public void OnSubmit()
        {
            Debug.Log(_inputField.text);

            int idx = _codeData.FindIndex(i => i["Code"].ToString() == _inputField.text);
            if (idx != -1)
            {
                if (UsedCode.Contains(_codeData[idx]["Code"].ToString()))
                {
                    FailCode();
                    return;
                }
                SuccessCode();
                Debug.Log("쿠폰 등록 완료!");
                _placeHolder.text = "쿠폰 등록 완료!";
                
                UsedCode.Add(_codeData[idx]["Code"].ToString());
                Resources.Load<CouponItem>("Coupon/" + _codeData[idx]["Code"].ToString()).UseCoupon();
#if !UNITY_EDITOR
                SaveManager.Instance.UpdateData();
#endif
            }
            else
            {
                FailCode();
                Debug.Log("쿠폰 등록 실패!");
                _placeHolder.text = "존재하지 않는 쿠폰입니다.";
            }
            _inputField.text = "";
            
            Invoke(nameof(ChgPlaceHolder), 2.5f);
        }

        private void ChgPlaceHolder()
        {
            _placeHolder.text = "쿠폰 입력...";
        }

        public void FailCode()
        {
            if (!FailText.activeSelf)
            {
                FailText.SetActive(true);
            }
        }

        public void SuccessCode()
        {
            if (!SuccessText.activeSelf)
            {
                SuccessText.SetActive(true);
            }
        }
    }
}
