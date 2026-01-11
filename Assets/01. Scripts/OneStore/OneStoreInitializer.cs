#if UNITY_ANDROID || UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    /// <summary>
    /// 원스토어 SDK 초기화 헬퍼
    /// 게임 시작 시 상품 정보를 미리 로드합니다.
    /// </summary>
    public class OneStoreInitializer : MonoBehaviour
    {
        [Header("상품 ID 목록")]
        [Tooltip("원스토어 개발자센터에 등록된 상품 ID")]
        [SerializeField] private List<string> _productIds = new List<string>();

        [Header("설정")]
        [SerializeField] private bool _loadOnStart = true;
        [SerializeField] private bool _queryPurchasesOnStart = true;

        private void Start()
        {
            if (OneStorePurchaseManager.Instance != null)
            {
                // 이미 초기화되었으면 바로 로드
                if (OneStorePurchaseManager.Instance.IsInitialized)
                {
                    OnOneStoreInitialized();
                }
                else
                {
                    // 초기화 완료 대기
                    OneStorePurchaseManager.Instance.OnInitialized += OnOneStoreInitialized;
                }
            }
            else
            {
                Debug.LogError("[OneStoreInitializer] OneStorePurchaseManager가 없습니다.");
            }
        }

        private void OnOneStoreInitialized()
        {
            Debug.Log("[OneStoreInitializer] OneStore 초기화 완료됨");

            if (_loadOnStart && _productIds.Count > 0)
            {
                LoadProducts();
            }

            if (_queryPurchasesOnStart)
            {
                QueryPendingPurchases();
            }
        }

        private void OnDestroy()
        {
            if (OneStorePurchaseManager.Instance != null)
            {
                OneStorePurchaseManager.Instance.OnInitialized -= OnOneStoreInitialized;
            }
        }

        /// <summary>
        /// 상품 정보 로드
        /// </summary>
        public void LoadProducts()
        {
            if (OneStorePurchaseManager.Instance == null)
            {
                Debug.LogError("[OneStoreInitializer] OneStorePurchaseManager가 없습니다.");
                return;
            }

            if (_productIds.Count == 0)
            {
                Debug.LogWarning("[OneStoreInitializer] 로드할 상품 ID가 없습니다.");
                return;
            }

            Debug.Log($"[OneStoreInitializer] 상품 정보 로드 시작: {_productIds.Count}개");
            OneStorePurchaseManager.Instance.LoadProducts(_productIds);
        }

        /// <summary>
        /// 미처리 구매 내역 확인
        /// </summary>
        public void QueryPendingPurchases()
        {
            if (OneStorePurchaseManager.Instance == null)
            {
                Debug.LogError("[OneStoreInitializer] OneStorePurchaseManager가 없습니다.");
                return;
            }

            Debug.Log("[OneStoreInitializer] 미처리 구매 내역 확인");
            OneStorePurchaseManager.Instance.QueryPurchases();
        }

        /// <summary>
        /// 상품 ID 추가 (런타임)
        /// </summary>
        public void AddProductId(string productId)
        {
            if (!_productIds.Contains(productId))
            {
                _productIds.Add(productId);
            }
        }

        /// <summary>
        /// 상품 ID 목록 설정 (런타임)
        /// </summary>
        public void SetProductIds(List<string> productIds)
        {
            _productIds = new List<string>(productIds);
        }
    }
}

#endif
