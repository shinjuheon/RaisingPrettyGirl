using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using Google.Play.AppUpdate;
using Google.Play.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace gunggme
{
    public class TitleSceneLoginScript : MonoBehaviour
    {
        public GameObject _loginObj;

        private LoginManager _loginManager;

        [SerializeField] private GameObject _maintenceUI;

        private void Awake()
        {
            _loginManager = GetComponent<LoginManager>();

            if (_loginManager == null)
            {
                Debug.LogWarning("LoginManager 컴포넌트를 찾지 못했습니다.");
            }
        }

        private void Start()
        {
            SoundManager.Instance.PlaySound(SoundType.BackgroundMusic, 0);
#if UNITY_ANDROID
            StartCoroutine(CheckForUpdate());
#endif
        }

        private void Update()
        {
            if (!_maintenceUI.activeSelf && _loginObj != null && !_loginObj.activeSelf && Input.anyKeyDown)
            {
#if !UNITY_EDITOR
                var bro = Backend.BMember.LoginWithTheBackendToken();
                if (bro.IsSuccess())
                {
                    Debug.Log("토큰이 존재함");
                    _loginManager.MoveScene();
                }
                else
                {
                    Debug.Log("토큰이 존재하지 않음 ");
                    _loginObj.SetActive(true);
                }
#endif
#if UNITY_EDITOR
                SceneManager.LoadScene("InGame");
#endif
            }
        }

#if UNITY_ANDROID
        IEnumerator CheckForUpdate()
        {
            yield return new WaitForSeconds(0.5f);
    
            AppUpdateManager appUpdateManager = new AppUpdateManager();

            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

            yield return appUpdateInfoOperation; //업데이트 가능한지 체크 중

            if (appUpdateInfoOperation.IsSuccessful)
            {
                var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

                if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                {
                    var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                    var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult,appUpdateOptions);
                
                    while(!startUpdateRequest.IsDone)
                    {
                        if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                        {
                            Debug.Log("업데이트 다운로드 진행중");

                        }
                        else if(startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                        {
                            Debug.Log("다운로드가 완료");
                        }

                        yield return null;
                    }

                    var result = appUpdateManager.CompleteUpdate();

                    while(!result.IsDone)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    yield return (int)startUpdateRequest.Status;
                }
                else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
                {
                    Debug.Log("업데이트가 없습니다");
                }
            }
            else
            {
                Debug.Log("업데이트 에러");
            }
        }
#endif
    }
}
