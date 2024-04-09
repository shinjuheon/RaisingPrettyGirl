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
        [SerializeField] private GameObject gameQuitUI;
        [SerializeField] private GameObject blockIDUI;
        [SerializeField] private GameObject googleLoginobj;
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

#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#endif
        }

        public void MoveScene()
        {
            if (_maintenceUI.activeSelf) return;
            if (_loging.activeSelf) return;
            _loging.SetActive(true);
            StartCoroutine(SaveManager.Instance.LoadData("InGame"));
        }


        void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                GetAccessCode();
                Debug.Log("로그인 성공");
                // Continue with Play Games Services
            }
            else
            {
                LoginFailed();
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
#endif
            }
        }

        public void LoginFailed()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            if (_policyUI.gameObject.activeSelf) _policyUI.gameObject.SetActive(false);
            _loging.SetActive(false);
            // 로그인이 실패할때의 예외처리 필요.
            StartCoroutine(LoginFailAndGameQuit()); // 추가 : 로그인실패 택스트를 띄우고 2.5초뒤 게임을 종료
        }
        // TODO : 401 예외처리 (토큰만료 되었을 경우, 인증이 실패했을 경우 등등)
        public void GetAccessCode()
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.RequestServerSideAccess( // false = 액세스 토큰을 새로 고치지 않고, 만료되었을 경우에만 기존 토큰을 사용
                /* forceRefreshToken= */ false,
                code =>
                {
                    Debug.Log("구글 인증 코드 : " + code);

                    Backend.BMember.GetGPGS2AccessToken(code, googleCallback => // code = 구글인증코드, googleCallback = 구글 액세스 토큰
                    {
                        Debug.Log("GetGPGS2AccessToken 함수 호출 결과 " + googleCallback);
                        //&& googleCallback = 만료되었을 경우의 검사항목을 추가해줘야 한다.
                        if (googleCallback == null)
                        {  // 구글 액세스 토큰을 받아오지 못하는경우
                            Debug.LogError("GoogleCallback is null");
                            // Handle null case appropriately
                            StartCoroutine(LoginFailAndGameQuit()); // 추가 : 로그인실패 택스트를 띄우고 2.5초뒤 게임을 종료
                            // 현재까지의 과정에서는 구글의 인증코드만을 받았고 구글의 액세스 토큰을 얻어오지 못했기때문에,
                            // 게임을 종료해 구글 액세스 토큰을 얻는 과정을 다시 한번 실행해준다.
                            // TitleSceneLoginScript클래스의 Update()함수에서는 백엔드의 액세스 토큰을 검사하기 떄문에
                            // 게임을 종료하고 다시 실행한다면 로그인창 오브젝트가 활성화되고 다시 GetAccessCode()함수를 실행한다.
                            return;
                        }

                        string accessToken = "";
                        if (googleCallback.IsSuccess()) // 구글 액세스 토큰을 얻는 것에 성공했다면
                        {
                            var jsonResult = googleCallback.GetReturnValuetoJSON();
                            if (jsonResult != null && jsonResult.ContainsKey("access_token"))
                            {
                                accessToken = jsonResult["access_token"].ToString();
                            }
                        }

                        Backend.BMember.AuthorizeFederation(accessToken, FederationType.GPGS2, callback =>
                        { // callback = 뒤끝 백엔드의 액세스 토큰
                            Debug.Log("뒤끝 로그인 성공했습니다." + callback);
                            try
                            {
                                Backend.BMember.RefreshTheBackendToken(); // 뒤끝 토큰 갱신
                                // 로그인 이후 24시간이 지날 경우에는 Backend.BMember.RefreshTheBackendToken을 호출하면 해결하실 수 있습니다.
                                _loging.SetActive(true);

                                if (callback == null)
                                {
                                    Debug.LogError("Callback is null");
                                    // Handle null case appropriately
                                    // callback이 null일경우의 예외처리 필요
                                    StartCoroutine(ForceQuitGame());
                                    return;
                                }

                                string statusCode = callback.GetStatusCode();
                                Debug.Log("statusCodeNum = "+ statusCode);
                                if (statusCode == "200") // 기존 액세스 토큰으로 로그인한 경우
                                {
                                    try
                                    {
                                        StartCoroutine(SaveManager.Instance.LoadData("InGame"));
                                        // LoadData GetUserInfo를 통해서 유저의 정보를 가져오고 InGame씬으로 이동한다. (자동로그인)
                                    }
                                    catch (Exception e) // 예외가 발생한다면
                                    {
                                        Debug.LogError(e);
                                        Debug.Log("데이터가 없음");
                                        StartCoroutine(ForceQuitGame());
                                        //_policyUI.OpenUI();
                                    }
                                }
                                else if (statusCode == "201") // 신규 회원 가입에 성공한 경우
                                {
                                    //_policyUI.OpenUI(); // 게임 규정인듯한데 실행이 안됨 이상함 -> 정상실행이 되었을 경우 Accept -> InitData()함수 실행
                                    // 뒤끝의 유저정보를 얻어온 후 NameSet씬으로 이동 -> ID생성
                                    SaveManager.Instance.InitData_NonMove(); // 뒤끝의 유저정보를 얻어온 후 NameSet씬으로 이동 -> ID생성
                                    // 규정이 정상적으로 생긴다면  SaveManager.Instance.InitData_NonMove();이 코드는 필요없음
                                }
                                else if (statusCode == "400") // 추가 - 기기 로컬에 액세스 토큰이 존재하지 않는데 토큰 로그인 시도를 한 경우(디바이스 정보가 null일경우)
                                {
                                    Backend.BMember.Logout();
                                    googleLoginobj.SetActive(true);
                                    // 백엔드의 액세스토큰을 삭제하고 새롭게 구글로그인창으로
                                    // 구글액세스코인와 백엔드 토큰을 얻는다.
                                    //_policyUI.OpenUI();//
                                   // StartCoroutine(ForceQuitGame());
                                }
                                else if (statusCode == "401") // 다른 기기로 로그인하여 refresh_token이 만료된 경우, 
                                {
                                    Backend.BMember.Logout();
                                    googleLoginobj.SetActive(true);
                                    // 백엔드의 액세스토큰을 삭제하고 새롭게 구글로그인창으로
                                    // 구글액세스코인와 백엔드 토큰을 얻는다.

                                    //if (callback)
                                    //{
                                    // 구글 엑세스 토큰을 검사해야하나 ?
                                    //}
                                    // 다른 기기에서 로그인을 한 경우, 커스텀 로그인 혹은 페데레이션 로그인 등 직접 로그인을 통해 액세스토큰을 재발급하셔야 합니다.

                                    //_policyUI.OpenUI();
                                    //StartCoroutine(ForceQuitGame());
                                }
                                else if (statusCode == "403") // 차단당한 계정 또는 디바이스 일 경우
                                {
                                    Debug.Log("해당 계정은 차단당한 계정입니다.");
                                    StartCoroutine(BlcokIDGameQuit());
                                    // 블록ID라는 Text를 띄우고 게임종료
                                }
                                else if (callback.IsMaintenanceError())
                                {
                                    LoginFailed();
                                    _maintenceUI.SetActive(true);
                                }
                            }
                            catch (Exception e)
                            {// 뒤끝서버에 로그인하지 못하는경우
                                StartCoroutine(ForceQuitGame());
                                Debug.LogError(e);
                            }
                        });
                    });
                });
#endif
        }

        //IEnumerator GetUserInfo()
        //{
        //    bool isHaveName = false;
        //    SaveManager.Instance.GetUserInfoTest(() =>
        //    {
        //        _policyUI.OpenUI();
        //        isHaveName = true;
        //        return;
        //    });
        //    yield return new WaitUntil(() => isHaveName);
        //}

        IEnumerator ForceQuitGame()
        {
            gameQuitUI.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }

        IEnumerator LoginFailAndGameQuit()
        {
            _loginFailedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }

        IEnumerator BlcokIDGameQuit()
        {
            blockIDUI.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }
    }
    
}
