using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace gunggme
{
    public class LoginManager : MonoBehaviour
    {
        [SerializeField] private GameObject _loginFailedText;
        [SerializeField] private GameObject _loging;
        [SerializeField] private PolicyUI _policyUI;
        [SerializeField] private GameObject _maintenceUI;
        private string resultCode;

        private Coroutine _coroutine;

        private void Update()
        {
          Backend.AsyncPoll();
        }

        public void Login_Google()
        {
          if (_maintenceUI.activeSelf) return;
          if (_loging.activeSelf) return;
          _loging.SetActive(true);
          
          PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
          
        }

        public void MoveScene()
        {
          if (_maintenceUI.activeSelf) return;
          if (_loging.activeSelf) return;
            _loging.SetActive(true);
            StartCoroutine(SaveManager.Instance.LoadData("InGame"));
        }


      void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
          GetAccessCode();
          Debug.Log("로그인 성공");
          // Continue with Play Games Services
        } else {
          LoginFailed();
          PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
      }

      public void LoginFailed()
      {
        if (_coroutine != null)
        {
          StopCoroutine(_coroutine);
        }
        if(_policyUI.gameObject.activeSelf) _policyUI.gameObject.SetActive(false);
        _loging.SetActive(false);
        _loginFailedText.gameObject.SetActive(true);
      }

      public void GetAccessCode()
      {
        PlayGamesPlatform.Instance.RequestServerSideAccess(
          /* forceRefreshToken= */ false,
          code => {
            Debug.Log("구글 인증 코드 : " + code);

            Backend.BMember.GetGPGS2AccessToken(code, googleCallback =>
            {
              Debug.Log("GetGPGS2AccessToken 함수 호출 결과 " + googleCallback);

              string accessToken = "";

              if (googleCallback.IsSuccess())
              {
                accessToken = googleCallback.GetReturnValuetoJSON()["access_token"].ToString();
              }

              Backend.BMember.AuthorizeFederation(accessToken, FederationType.GPGS2, callback =>
              {
                Debug.Log("뒤끝 로그인 성공했습니다. " + callback);
                try
                {
                  Backend.BMember.RefreshTheBackendToken();
                  _loging.SetActive(true);
                  if (callback.GetStatusCode() == "200")
                  {
                    try
                    { 
                      StartCoroutine(SaveManager.Instance.LoadData("InGame"));
                    }
                    catch (Exception e)
                    {
                      Debug.LogError(e);
                      Debug.Log("데이터가 없음");
                      _policyUI.OpenUI();
                    }
                    return;
                  }

                  if (callback.IsMaintenanceError())
                  {
                    LoginFailed();
                    _maintenceUI.SetActive(true);
                  }

                  if (callback.GetStatusCode() == "201")
                  {
                    _policyUI.OpenUI();
                    SaveManager.Instance.InitData_NonMove();
                    return;
                  }

                  if (callback.GetStatusCode() == "401")
                  {
                    _policyUI.OpenUI();
                  }
                }
                catch (Exception e)
                {
                  Debug.LogError(e);
                }
              });
            });
          });
      }

      IEnumerator GetUserInfo()
      {

        bool isHaveName = false;
        SaveManager.Instance.GetUserInfoTest(() =>
        {
          _policyUI.OpenUI();
          isHaveName = true;
          return;
        });
        yield return new WaitUntil(() => isHaveName);
        
      }
          
      }
}
