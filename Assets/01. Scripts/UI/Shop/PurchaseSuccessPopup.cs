using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    /// <summary>
    /// 결제 성공 팝업 UI
    /// 획득한 다이아몬드 수량을 표시합니다.
    /// </summary>
    public class PurchaseSuccessPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _diamondAmountText;
        [SerializeField] private Image _diamondIcon;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _autoCloseDelay = 3f;
        [SerializeField] private bool _autoClose = true;

        private Coroutine _autoCloseCoroutine;

        private void Awake()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(Close);
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 팝업 표시
        /// </summary>
        /// <param name="diamondAmount">획득한 다이아몬드 수량</param>
        public void Show(int diamondAmount)
        {
            gameObject.SetActive(true);

            if (_titleText != null)
            {
                _titleText.text = "결제 완료!";
            }

            if (_diamondAmountText != null)
            {
                _diamondAmountText.text = $"+{diamondAmount:N0} 다이아";
            }

            // 페이드 인 애니메이션
            if (_canvasGroup != null)
            {
                StartCoroutine(FadeIn());
            }

            // 자동 닫기
            if (_autoClose && _autoCloseDelay > 0)
            {
                if (_autoCloseCoroutine != null)
                {
                    StopCoroutine(_autoCloseCoroutine);
                }
                _autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
            }

            Debug.Log($"[PurchaseSuccessPopup] 표시: {diamondAmount} 다이아 획득");
        }

        private IEnumerator FadeIn()
        {
            _canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeInDuration);
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }

        private IEnumerator AutoCloseCoroutine()
        {
            yield return new WaitForSeconds(_autoCloseDelay);
            Close();
        }

        /// <summary>
        /// 팝업 닫기
        /// </summary>
        public void Close()
        {
            if (_autoCloseCoroutine != null)
            {
                StopCoroutine(_autoCloseCoroutine);
                _autoCloseCoroutine = null;
            }

            gameObject.SetActive(false);
        }
    }
}
