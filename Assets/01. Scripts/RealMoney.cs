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

        [Header("결제 성공 팝업")]
        [SerializeField] private PurchaseSuccessPopup _purchaseSuccessPopup;

        private void Awake()
        {
            _slotUI = GetComponent<ShopItemSlotUI>();

#if UNITY_ANDROID
            // Awake에서 이벤트 구독 (GameObject가 비활성화되어도 구독 유지)
            if (OneStorePurchaseManager.Instance != null)
            {
                OneStorePurchaseManager.Instance.OnPurchaseComplete += HandlePurchaseComplete;
                OneStorePurchaseManager.Instance.OnPurchaseFail += HandlePurchaseFailed;
                Debug.Log("[RealMoney] 이벤트 구독 완료");
            }
            else
            {
                Debug.LogWarning("[RealMoney] OneStorePurchaseManager가 아직 없습니다. Start에서 재시도합니다.");
            }
#endif
        }

        private void Start()
        {
#if UNITY_ANDROID
            // Awake에서 실패했을 경우 재시도
            if (OneStorePurchaseManager.Instance != null)
            {
                // 중복 구독 방지
                OneStorePurchaseManager.Instance.OnPurchaseComplete -= HandlePurchaseComplete;
                OneStorePurchaseManager.Instance.OnPurchaseFail -= HandlePurchaseFailed;

                OneStorePurchaseManager.Instance.OnPurchaseComplete += HandlePurchaseComplete;
                OneStorePurchaseManager.Instance.OnPurchaseFail += HandlePurchaseFailed;
                Debug.Log("[RealMoney] 이벤트 구독 완료 (Start)");
            }
#endif
        }

        private void OnDestroy()
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
        /// 구매 버튼 클릭 시 호출 (ShopItemSlotUI에서)
        /// </summary>
        public void OnPurchaseButtonClick()
        {
            if (_slotUI != null)
            {
                _currentShopDiamond = _slotUI._shopItem as ShopDiamond;
            }

            if (_currentShopDiamond == null)
            {
                Debug.LogError("[RealMoney] ShopDiamond가 null입니다.");
                return;
            }

            RequestPurchase();
        }

        /// <summary>
        /// ShopItemPannel에서 직접 호출 시 사용
        /// </summary>
        public void Purchase(ShopItem shopItem)
        {
            _currentShopDiamond = shopItem as ShopDiamond;
            if (_currentShopDiamond == null)
            {
                Debug.LogError("[RealMoney] ShopDiamond가 아닙니다.");
                return;
            }

            RequestPurchase();
        }

        private void RequestPurchase()
        {
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
            // OneStorePurchaseManager에서 직접 보상을 지급하므로 여기서는 처리 불필요
            // 이 콜백은 추가 로직이 필요할 때 사용
            if (_currentShopDiamond != null && purchase.ProductId == _currentShopDiamond.Diamond_ID)
            {
                Debug.Log($"[RealMoney] 구매 완료 확인: {_currentShopDiamond.Diamonds} 다이아몬드");
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
