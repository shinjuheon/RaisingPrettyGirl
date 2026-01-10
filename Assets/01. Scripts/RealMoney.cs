using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using OneStore.Purchasing;
#endif

namespace gunggme
{
    /// <summary>
    /// 실제 결제 처리 클래스
    /// Unity IAP -> OneStore SDK V21 로 변경됨
    /// </summary>
    public class RealMoney : MonoBehaviour
    {
        private ShopItemSlotUI _slotUI;
        private ShopDiamond _currentShopDiamond;

        private void Awake()
        {
            _slotUI = GetComponent<ShopItemSlotUI>();
        }

        private void OnEnable()
        {
#if UNITY_ANDROID
            // 이벤트 구독
            if (OneStorePurchaseManager.Instance != null)
            {
                OneStorePurchaseManager.Instance.OnPurchaseComplete += HandlePurchaseComplete;
                OneStorePurchaseManager.Instance.OnPurchaseFail += HandlePurchaseFailed;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_ANDROID
            // 이벤트 구독 해제
            if (OneStorePurchaseManager.Instance != null)
            {
                OneStorePurchaseManager.Instance.OnPurchaseComplete -= HandlePurchaseComplete;
                OneStorePurchaseManager.Instance.OnPurchaseFail -= HandlePurchaseFailed;
            }
#endif
        }

        /// <summary>
        /// 구매 버튼 클릭 시 호출
        /// </summary>
        public void OnPurchaseButtonClick()
        {
            _currentShopDiamond = _slotUI._shopItem as ShopDiamond;
            if (_currentShopDiamond == null)
            {
                Debug.LogError("[RealMoney] ShopDiamond가 null입니다.");
                return;
            }

#if UNITY_ANDROID
            // 원스토어 구매 요청
            if (OneStorePurchaseManager.Instance != null)
            {
                OneStorePurchaseManager.Instance.Purchase(_currentShopDiamond);
            }
            else
            {
                Debug.LogError("[RealMoney] OneStorePurchaseManager가 초기화되지 않았습니다.");
            }
#else
            Debug.Log("[RealMoney] Android가 아닌 환경에서는 결제를 지원하지 않습니다.");
#endif
        }

#if UNITY_ANDROID
        /// <summary>
        /// 구매 완료 처리
        /// </summary>
        private void HandlePurchaseComplete(PurchaseData purchase)
        {
            if (_currentShopDiamond == null) return;

            if (purchase.ProductId == _currentShopDiamond.Diamond_ID)
            {
                Debug.Log($"[RealMoney] {_currentShopDiamond.Diamonds} 다이아몬드 구매 완료");
                _slotUI.Buy(); // 기존 보상 지급 로직 호출
                _currentShopDiamond = null;
            }
        }

        /// <summary>
        /// 구매 실패 처리
        /// </summary>
        private void HandlePurchaseFailed(string productId, string errorMessage)
        {
            Debug.LogError($"[RealMoney] 구매 실패: {errorMessage}");
            _currentShopDiamond = null;
            // TODO: 실패 UI 표시 (필요시 UIManager 연동)
        }
#endif

        #region Legacy Unity IAP Methods (호환성을 위해 유지, 사용하지 않음)

        /*
        // Unity IAP 콜백 - 더 이상 사용하지 않음
        public void OnPurchaseComplate(Product product)
        {
            var dast = _slotUI._shopItem as ShopDiamond;
            if (product.definition.id == dast.Diamond_ID)
            {
                Debug.Log(dast.Diamonds + " 구매 완료");
                _slotUI.Buy();
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log("구매 실패");
        }
        */

        #endregion
    }
}
