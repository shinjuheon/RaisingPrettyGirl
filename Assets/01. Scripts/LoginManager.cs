using System;
using System.Collections;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace gunggme
{
    public class LoginManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _loginFailedText;
        [SerializeField] private GameObject _loging;
        [SerializeField] private PolicyUI _policyUI;
        [SerializeField] private GameObject _maintenceUI;
        [SerializeField] private GameObject gameQuitUI;
        [SerializeField] private GameObject blockIDUI;

        [Header("로그인 패널")]
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_Text _loginErrorText;

        [Header("회원가입 패널")]
        [SerializeField] private GameObject _signUpPanel;
        [SerializeField] private TMP_InputField _signUpIdInputField;
        [SerializeField] private TMP_InputField _signUpPasswordInputField;
        [SerializeField] private TMP_InputField _signUpPasswordConfirmInputField;
        [SerializeField] private TMP_Text _signUpErrorText;

        private Coroutine _coroutine;

        private void Update()
        {
            Backend.AsyncPoll();
        }

        /// <summary>
        /// 로그인 버튼 클릭 시 호출
        /// </summary>
        public void OnLoginButtonClicked()
        {
            Debug.Log("[LOGIN] 로그인 버튼 클릭됨");

            if (_maintenceUI.activeSelf)
            {
                Debug.Log("[LOGIN] 점검중이라 로그인 스킵");
                return;
            }
            if (_loging.activeSelf)
            {
                Debug.Log("[LOGIN] 이미 로그인 중이라 스킵");
                return;
            }

            string id = _idInputField.text.Trim();
            string password = _passwordInputField.text;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                ShowLoginError("아이디와 비밀번호를 입력해주세요.");
                return;
            }

            if (id.Length < 4 || password.Length < 6)
            {
                ShowLoginError("아이디는 4자 이상, 비밀번호는 6자 이상이어야 합니다.");
                return;
            }

            _loging.SetActive(true);
            HideAllErrors();

            CustomLogin(id, password);
        }

        /// <summary>
        /// 회원가입 화면으로 이동 버튼
        /// </summary>
        public void OnGoToSignUpClicked()
        {
            Debug.Log("[LOGIN] 회원가입 화면으로 이동");
            _loginPanel.SetActive(false);
            _signUpPanel.SetActive(true);
            HideAllErrors();
            ClearSignUpInputs();
        }

        /// <summary>
        /// 로그인 화면으로 돌아가기 버튼
        /// </summary>
        public void OnBackToLoginClicked()
        {
            Debug.Log("[LOGIN] 로그인 화면으로 돌아가기");
            _signUpPanel.SetActive(false);
            _loginPanel.SetActive(true);
            HideAllErrors();
        }

        /// <summary>
        /// 회원가입 실행 버튼 클릭 시 호출
        /// </summary>
        public void OnSignUpButtonClicked()
        {
            Debug.Log("[LOGIN] 회원가입 버튼 클릭됨");

            if (_maintenceUI.activeSelf)
            {
                Debug.Log("[LOGIN] 점검중이라 회원가입 스킵");
                return;
            }
            if (_loging.activeSelf)
            {
                Debug.Log("[LOGIN] 이미 로딩 중이라 스킵");
                return;
            }

            string id = _signUpIdInputField.text.Trim();
            string password = _signUpPasswordInputField.text;
            string passwordConfirm = _signUpPasswordConfirmInputField.text;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                ShowSignUpError("아이디와 비밀번호를 입력해주세요.");
                return;
            }

            if (id.Length < 4)
            {
                ShowSignUpError("아이디는 4자 이상이어야 합니다.");
                return;
            }

            if (password.Length < 6)
            {
                ShowSignUpError("비밀번호는 6자 이상이어야 합니다.");
                return;
            }

            if (password != passwordConfirm)
            {
                ShowSignUpError("비밀번호가 일치하지 않습니다.");
                return;
            }

            _loging.SetActive(true);
            HideAllErrors();

            CustomSignUp(id, password);
        }

        private void ClearSignUpInputs()
        {
            if (_signUpIdInputField != null) _signUpIdInputField.text = "";
            if (_signUpPasswordInputField != null) _signUpPasswordInputField.text = "";
            if (_signUpPasswordConfirmInputField != null) _signUpPasswordConfirmInputField.text = "";
        }

        /// <summary>
        /// 뒤끝 커스텀 로그인
        /// </summary>
        private void CustomLogin(string id, string password)
        {
            Debug.Log($"[LOGIN] CustomLogin 시도 - ID: {id}");

            Backend.BMember.CustomLogin(id, password, callback =>
            {
                Debug.Log($"[LOGIN] CustomLogin 결과: {callback}");

                if (callback == null)
                {
                    Debug.LogError("[LOGIN] CustomLogin callback이 null!");
                    _loging.SetActive(false);
                    ShowLoginError("서버 연결에 실패했습니다.");
                    return;
                }

                string statusCode = callback.GetStatusCode();
                string errorCode = callback.GetErrorCode();
                string message = callback.GetMessage();
                Debug.Log($"[LOGIN] CustomLogin - StatusCode: {statusCode}, ErrorCode: {errorCode}, Message: {message}");

                if (callback.IsSuccess())
                {
                    Debug.Log("[LOGIN] 로그인 성공!");
                    OnLoginSuccess();
                }
                else
                {
                    _loging.SetActive(false);
                    HandleLoginError(statusCode, errorCode, message);
                }
            });
        }

        /// <summary>
        /// 뒤끝 커스텀 회원가입
        /// </summary>
        private void CustomSignUp(string id, string password)
        {
            Debug.Log($"[LOGIN] CustomSignUp 시도 - ID: {id}");

            Backend.BMember.CustomSignUp(id, password, callback =>
            {
                Debug.Log($"[LOGIN] CustomSignUp 결과: {callback}");

                if (callback == null)
                {
                    Debug.LogError("[LOGIN] CustomSignUp callback이 null!");
                    _loging.SetActive(false);
                    ShowSignUpError("서버 연결에 실패했습니다.");
                    return;
                }

                string statusCode = callback.GetStatusCode();
                string errorCode = callback.GetErrorCode();
                string message = callback.GetMessage();
                Debug.Log($"[LOGIN] CustomSignUp - StatusCode: {statusCode}, ErrorCode: {errorCode}, Message: {message}");

                if (callback.IsSuccess())
                {
                    Debug.Log("[LOGIN] 회원가입 성공! 초기 데이터 생성");
                    OnSignUpSuccess();
                }
                else
                {
                    _loging.SetActive(false);
                    HandleSignUpError(statusCode, errorCode, message);
                }
            });
        }

        /// <summary>
        /// 로그인 성공 처리
        /// </summary>
        private void OnLoginSuccess()
        {
            try
            {
                Backend.BMember.RefreshTheBackendToken();
                Debug.Log("[LOGIN] RefreshTheBackendToken 완료, LoadData 시작");
                StartCoroutine(SaveManager.Instance.LoadData("InGame"));
            }
            catch (Exception e)
            {
                Debug.LogError($"[LOGIN] LoadData 예외: {e.Message}\n{e.StackTrace}");
                _loging.SetActive(false);
                ShowLoginError("데이터 로드에 실패했습니다.");
            }
        }

        /// <summary>
        /// 회원가입 성공 처리
        /// </summary>
        private void OnSignUpSuccess()
        {
            try
            {
                Debug.Log("[LOGIN] 신규 유저 - InitData_NonMove 시작");
                SaveManager.Instance.InitData_NonMove();
            }
            catch (Exception e)
            {
                Debug.LogError($"[LOGIN] InitData_NonMove 예외: {e.Message}\n{e.StackTrace}");
                _loging.SetActive(false);
                ShowSignUpError("초기 데이터 생성에 실패했습니다.");
            }
        }

        /// <summary>
        /// 로그인 에러 처리
        /// </summary>
        private void HandleLoginError(string statusCode, string errorCode, string message)
        {
            Debug.LogError($"[LOGIN] 로그인 실패 - StatusCode: {statusCode}, ErrorCode: {errorCode}");

            switch (statusCode)
            {
                case "401":
                    ShowLoginError("아이디 또는 비밀번호가 잘못되었습니다.");
                    break;
                case "403":
                    Debug.LogError("[LOGIN] 403 에러 - 차단된 계정");
                    StartCoroutine(BlcokIDGameQuit());
                    break;
                case "404":
                    ShowLoginError("존재하지 않는 아이디입니다.");
                    break;
                default:
                    if (message != null && message.Contains("maintenance"))
                    {
                        _maintenceUI.SetActive(true);
                    }
                    else
                    {
                        ShowLoginError($"로그인 실패: {message}");
                    }
                    break;
            }
        }

        /// <summary>
        /// 회원가입 에러 처리
        /// </summary>
        private void HandleSignUpError(string statusCode, string errorCode, string message)
        {
            Debug.LogError($"[LOGIN] 회원가입 실패 - StatusCode: {statusCode}, ErrorCode: {errorCode}");

            switch (statusCode)
            {
                case "409":
                    ShowSignUpError("이미 존재하는 아이디입니다.");
                    break;
                case "400":
                    if (errorCode == "BadParameterException")
                    {
                        ShowSignUpError("아이디 또는 비밀번호 형식이 올바르지 않습니다.");
                    }
                    else
                    {
                        ShowSignUpError($"회원가입 실패: {message}");
                    }
                    break;
                default:
                    ShowSignUpError($"회원가입 실패: {message}");
                    break;
            }
        }

        /// <summary>
        /// 로그인 에러 메시지 표시
        /// </summary>
        private void ShowLoginError(string message)
        {
            Debug.LogWarning($"[LOGIN] 로그인 에러: {message}");
            if (_loginErrorText != null)
            {
                _loginErrorText.text = message;
                _loginErrorText.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 회원가입 에러 메시지 표시
        /// </summary>
        private void ShowSignUpError(string message)
        {
            Debug.LogWarning($"[LOGIN] 회원가입 에러: {message}");
            if (_signUpErrorText != null)
            {
                _signUpErrorText.text = message;
                _signUpErrorText.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 모든 에러 메시지 숨김
        /// </summary>
        private void HideAllErrors()
        {
            if (_loginErrorText != null)
                _loginErrorText.gameObject.SetActive(false);
            if (_signUpErrorText != null)
                _signUpErrorText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 씬 이동 (자동 로그인용)
        /// </summary>
        public void MoveScene()
        {
            Debug.Log("[LOGIN] MoveScene 호출됨");
            if (_maintenceUI.activeSelf)
            {
                Debug.Log("[LOGIN] 점검중이라 MoveScene 스킵");
                return;
            }
            if (_loging.activeSelf)
            {
                Debug.Log("[LOGIN] 이미 로딩중이라 MoveScene 스킵");
                return;
            }
            _loging.SetActive(true);
            Debug.Log("[LOGIN] LoadData 코루틴 시작");
            StartCoroutine(SaveManager.Instance.LoadData("InGame"));
        }

        public void LoginFailed()
        {
            Debug.LogWarning("[LOGIN] LoginFailed 호출됨");
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            if (_policyUI.gameObject.activeSelf) _policyUI.gameObject.SetActive(false);
            _loging.SetActive(false);
            StartCoroutine(LoginFailAndGameQuit());
        }

        public IEnumerator ForceQuitGame()
        {
            Debug.LogError("[LOGIN] ForceQuitGame - 강제 종료 시작");
            gameQuitUI.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }

        IEnumerator LoginFailAndGameQuit()
        {
            Debug.LogError("[LOGIN] LoginFailAndGameQuit - 로그인 실패로 종료");
            _loginFailedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }

        IEnumerator BlcokIDGameQuit()
        {
            Debug.LogError("[LOGIN] BlcokIDGameQuit - 차단된 계정으로 종료");
            blockIDUI.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }
    }
}
