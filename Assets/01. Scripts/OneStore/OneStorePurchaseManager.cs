#if UNITY_ANDROID || UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using OneStore.Purchasing;

namespace gunggme
{
    /// <summary>
    /// 원스토어 인앱결제 관리자 (싱글톤)
    /// SDK V21 기반
    /// </summary>
    public class OneStorePurchaseManager : Singletone<OneStorePurchaseManager>, IPurchaseCallback
    {
        [Header("원스토어 설정")]
        [SerializeField] private string _licenseKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCISAbPDBKemzF1HZOQZd1cg2VH2KwN/vx1YbghYnGuNSzMdWQb6PcSWsph+Y/jvKg2n++86Fy59KO8aoziG7dDZnmhSTR4rnEdgVX7nX4qwGtTzverTZeFRkIfV2w3Tm6134LE9dMM/vv4pVsPBRv6O5Mf/91Yr9A63hp9yRw5fQIDAQAB";

        private PurchaseClientImpl _purchaseClient;
        private Dictionary<string, ProductDetail> _productCache = new Dictionary<string, ProductDetail>();
        private bool _setupFailed = false;
        private string _setupFailMessage = "";

        // 현재 구매 중인 상품 정보 (보상 지급용)
        private ShopDiamond _pendingPurchase;

        // Consume 재시도 관련
        private const int MAX_CONSUME_RETRY = 3;
        private Dictionary<string, int> _consumeRetryCount = new Dictionary<string, int>();
        private Dictionary<string, PurchaseData> _pendingConsumePurchases = new Dictionary<string, PurchaseData>();

        [Header("결제 성공 팝업")]
        [SerializeField] private PurchaseSuccessPopup _purchaseSuccessPopup;

        #region Events

        /// <summary>
        /// 구매 완료 이벤트
        /// </summary>
        public event Action<PurchaseData> OnPurchaseComplete;

        /// <summary>
        /// 구매 실패 이벤트 (productId, errorMessage)
        /// </summary>
        public event Action<string, string> OnPurchaseFail;

        /// <summary>
        /// 상품 정보 로드 완료 이벤트
        /// </summary>
        public event Action<List<ProductDetail>> OnProductsLoaded;

        /// <summary>
        /// 에러 발생 이벤트
        /// </summary>
        public event Action<string> OnError;

        #endregion

        #region Properties

        /// <summary>
        /// 초기화 완료 여부
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 마켓 구분 코드 (S2S API용)
        /// </summary>
        public string StoreCode => _purchaseClient?.StoreCode;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// 원스토어 결제 클라이언트 초기화
        /// </summary>
        private void Initialize()
        {
            if (string.IsNullOrEmpty(_licenseKey) || _licenseKey == "YOUR_LICENSE_KEY")
            {
                Debug.LogWarning("[OneStore] 라이선스 키가 설정되지 않았습니다.");
                return;
            }

            _purchaseClient = new PurchaseClientImpl(_licenseKey);
            _purchaseClient.Initialize(this);
            IsInitialized = true;
            Debug.Log("[OneStore] 초기화 완료");
        }

        /// <summary>
        /// 라이선스 키 설정 (런타임에서 설정할 경우)
        /// </summary>
        public void SetLicenseKey(string licenseKey)
        {
            _licenseKey = licenseKey;
            if (!IsInitialized)
            {
                Initialize();
            }
        }

        #endregion

        #region Product Query

        /// <summary>
        /// 관리형 상품(인앱) 정보 조회
        /// </summary>
        public void LoadProducts(List<string> productIds)
        {
            if (!CheckInitialized()) return;

            Debug.Log($"[OneStore] 상품 조회 요청: {productIds.Count}개");
            var readOnlyIds = new ReadOnlyCollection<string>(productIds);
            _purchaseClient.QueryProductDetails(readOnlyIds, ProductType.INAPP);
        }

        /// <summary>
        /// 정기결제 상품 정보 조회
        /// </summary>
        public void LoadSubscriptions(List<string> productIds)
        {
            if (!CheckInitialized()) return;

            Debug.Log($"[OneStore] 정기결제 상품 조회 요청: {productIds.Count}개");
            var readOnlyIds = new ReadOnlyCollection<string>(productIds);
            _purchaseClient.QueryProductDetails(readOnlyIds, ProductType.SUBS);
        }

        /// <summary>
        /// 캐시된 상품 정보 가져오기
        /// </summary>
        public ProductDetail GetProduct(string productId)
        {
            return _productCache.TryGetValue(productId, out var product) ? product : null;
        }

        /// <summary>
        /// 모든 캐시된 상품 정보 가져오기
        /// </summary>
        public List<ProductDetail> GetAllProducts()
        {
            return new List<ProductDetail>(_productCache.Values);
        }

        #endregion

        #region Purchase

        /// <summary>
        /// 상품 구매 요청
        /// </summary>
        public void Purchase(string productId, string developerPayload = "")
        {
            if (!CheckInitialized()) return;

            var product = GetProduct(productId);
            if (product == null)
            {
                Debug.LogError($"[OneStore] 상품을 찾을 수 없음: {productId}");
                OnPurchaseFail?.Invoke(productId, "상품 정보가 없습니다. 상품 조회를 먼저 해주세요.");
                return;
            }

            var purchaseParams = new PurchaseFlowParams.Builder()
                .SetProductId(productId)
                .SetProductType(ProductType.Get(product.type))
                .SetDeveloperPayload(developerPayload)
                .Build();

            Debug.Log($"[OneStore] 구매 요청: {productId}");
            _purchaseClient.Purchase(purchaseParams);
        }

        /// <summary>
        /// ShopDiamond 상품 구매 (기존 시스템 연동용)
        /// </summary>
        public void Purchase(ShopDiamond shopDiamond)
        {
            if (shopDiamond == null)
            {
                Debug.LogError("[OneStore] ShopDiamond가 null입니다.");
                return;
            }

            Purchase(shopDiamond.Diamond_ID, $"diamond_{shopDiamond.Diamonds}");
        }

        #endregion

        #region Purchase Processing

        /// <summary>
        /// 소비성 상품 소비 처리
        /// </summary>
        public void ConsumePurchase(PurchaseData purchase)
        {
            if (!CheckInitialized()) return;
            if (purchase == null)
            {
                Debug.LogError("[OneStore] PurchaseData가 null입니다.");
                return;
            }

            Debug.Log($"[OneStore] 소비 요청: {purchase.ProductId}");
            _purchaseClient.ConsumePurchase(purchase);
        }

        /// <summary>
        /// 비소비성 상품 확인 처리
        /// </summary>
        public void AcknowledgePurchase(PurchaseData purchase, ProductType productType = null)
        {
            if (!CheckInitialized()) return;
            if (purchase == null)
            {
                Debug.LogError("[OneStore] PurchaseData가 null입니다.");
                return;
            }

            // ProductType이 지정되지 않으면 INAPP으로 기본 설정
            var type = productType ?? ProductType.INAPP;

            if (!purchase.Acknowledged)
            {
                Debug.Log($"[OneStore] 확인 요청: {purchase.ProductId}");
                _purchaseClient.AcknowledgePurchase(purchase, type);
            }
            else
            {
                Debug.Log($"[OneStore] 이미 확인된 구매: {purchase.ProductId}");
            }
        }

        /// <summary>
        /// 미처리 구매 내역 조회
        /// </summary>
        public void QueryPurchases()
        {
            if (!CheckInitialized()) return;

            Debug.Log("[OneStore] 미처리 구매 내역 조회");
            _purchaseClient.QueryPurchases(ProductType.INAPP);
        }

        /// <summary>
        /// 정기결제 미처리 구매 내역 조회
        /// </summary>
        public void QuerySubscriptionPurchases()
        {
            if (!CheckInitialized()) return;

            Debug.Log("[OneStore] 정기결제 미처리 구매 내역 조회");
            _purchaseClient.QueryPurchases(ProductType.SUBS);
        }

        #endregion

        #region Utility

        /// <summary>
        /// 원스토어 서비스 업데이트/설치 화면 열기
        /// </summary>
        public void LaunchUpdateOrInstall()
        {
            if (_purchaseClient != null)
            {
                _purchaseClient.LaunchUpdateOrInstallFlow(result =>
                {
                    if (result.IsSuccessful())
                    {
                        Debug.Log("[OneStore] 업데이트/설치 완료");
                    }
                    else
                    {
                        Debug.LogError($"[OneStore] 업데이트/설치 실패: {result.Message}");
                    }
                });
            }
        }

        /// <summary>
        /// 정기결제 관리 화면 열기
        /// </summary>
        public void OpenSubscriptionManagement(PurchaseData purchaseData = null)
        {
            if (_purchaseClient != null)
            {
                _purchaseClient.LaunchManageSubscription(purchaseData);
            }
        }

        private bool CheckInitialized()
        {
            if (!IsInitialized)
            {
                Debug.LogError("[OneStore] 초기화되지 않았습니다.");
                OnError?.Invoke("원스토어가 초기화되지 않았습니다.");
                return false;
            }
            return true;
        }

        #endregion

        #region IPurchaseCallback Implementation

        public void OnSetupFailed(IapResult iapResult)
        {
            Debug.LogError($"[OneStore] 초기화 실패: {iapResult.Message}");
            IsInitialized = false;
            OnError?.Invoke($"초기화 실패: {iapResult.Message}");
        }

        public void OnProductDetailsSucceeded(List<ProductDetail> productDetails)
        {
            Debug.Log($"[OneStore] 상품 조회 성공: {productDetails.Count}개");

            foreach (var product in productDetails)
            {
                _productCache[product.productId] = product;
                Debug.Log($"  - {product.productId}: {product.title} ({product.price})");
            }

            OnProductsLoaded?.Invoke(productDetails);
        }

        public void OnProductDetailsFailed(IapResult iapResult)
        {
            Debug.LogError($"[OneStore] 상품 조회 실패: {iapResult.Message}");
            OnError?.Invoke($"상품 조회 실패: {iapResult.Message}");
        }

        public void OnPurchaseSucceeded(List<PurchaseData> purchases)
        {
            Debug.Log($"[OneStore] 구매 성공: {purchases.Count}개");

            foreach (var purchase in purchases)
            {
                Debug.Log($"  - {purchase.ProductId}");
                // 소비성 상품은 바로 소비 처리
                ConsumePurchase(purchase);
            }
        }

        public void OnPurchaseFailed(IapResult iapResult)
        {
            Debug.LogError($"[OneStore] 구매 실패: {iapResult.Message}");
            OnPurchaseFail?.Invoke("", iapResult.Message);
        }

        public void OnConsumeSucceeded(PurchaseData purchase)
        {
            Debug.Log($"[OneStore] 소비 완료: {purchase.ProductId}");
            OnPurchaseComplete?.Invoke(purchase);
        }

        public void OnConsumeFailed(IapResult iapResult)
        {
            Debug.LogError($"[OneStore] 소비 실패: {iapResult.Message}");
            OnError?.Invoke($"소비 실패: {iapResult.Message}");
        }

        public void OnAcknowledgeSucceeded(PurchaseData purchase, ProductType type)
        {
            Debug.Log($"[OneStore] 확인 완료: {purchase.ProductId} (Type: {type})");
            OnPurchaseComplete?.Invoke(purchase);
        }

        public void OnAcknowledgeFailed(IapResult iapResult)
        {
            Debug.LogError($"[OneStore] 확인 실패: {iapResult.Message}");
            OnError?.Invoke($"확인 실패: {iapResult.Message}");
        }

        public void OnManageRecurringProduct(IapResult iapResult, PurchaseData purchase, RecurringAction action)
        {
            if (iapResult.IsSuccessful())
            {
                Debug.Log($"[OneStore] 월정액 상태 변경 성공: {purchase.ProductId}, Action: {action}");
            }
            else
            {
                Debug.LogError($"[OneStore] 월정액 상태 변경 실패: {iapResult.Message}");
            }
        }

        public void OnNeedUpdate()
        {
            Debug.Log("[OneStore] 원스토어 서비스 업데이트가 필요합니다.");
            LaunchUpdateOrInstall();
        }

        public void OnNeedLogin()
        {
            Debug.Log("[OneStore] 로그인이 필요합니다.");
            // OneStoreAuthManager를 통해 로그인 처리
            if (OneStoreAuthManager.Instance != null)
            {
                OneStoreAuthManager.Instance.LaunchLogin();
            }
        }

        #endregion
    }
}

#endif
