#if UNITY_ANDROID || UNITY_EDITOR

using System;
using UnityEngine;
using OneStore.Auth;

namespace gunggme
{
    /// <summary>
    /// 원스토어 인증(로그인) 관리자 (싱글톤)
    /// </summary>
    public class OneStoreAuthManager : Singletone<OneStoreAuthManager>
    {
        #region Events

        /// <summary>
        /// 로그인 완료 이벤트 (success, message)
        /// </summary>
        public event Action<bool, string> OnLoginComplete;

        #endregion

        #region Properties

        /// <summary>
        /// 로그인 상태
        /// </summary>
        public bool IsLoggedIn { get; private set; }

        #endregion

        private OneStoreAuthClientImpl _authClient;

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _authClient = new OneStoreAuthClientImpl();
        }

        #endregion

        #region Public API

        /// <summary>
        /// 원스토어 로그인 화면 실행
        /// </summary>
        /// <param name="callback">로그인 결과 콜백 (optional)</param>
        public void LaunchLogin(Action<bool> callback = null)
        {
            Debug.Log("[OneStore Auth] 로그인 요청");

            _authClient.LaunchSignInFlow(result =>
            {
                IsLoggedIn = result.IsSuccessful();

                if (IsLoggedIn)
                {
                    Debug.Log("[OneStore Auth] 로그인 성공");
                }
                else
                {
                    Debug.LogError($"[OneStore Auth] 로그인 실패: {result.Message}");
                }

                OnLoginComplete?.Invoke(IsLoggedIn, result.Message);
                callback?.Invoke(IsLoggedIn);
            });
        }

        /// <summary>
        /// 로그인 후 작업 수행
        /// </summary>
        public void EnsureLoggedIn(Action onSuccess, Action<string> onFail = null)
        {
            if (IsLoggedIn)
            {
                onSuccess?.Invoke();
                return;
            }

            LaunchLogin(success =>
            {
                if (success)
                {
                    onSuccess?.Invoke();
                }
                else
                {
                    onFail?.Invoke("로그인이 필요합니다.");
                }
            });
        }

        #endregion
    }
}

#endif
