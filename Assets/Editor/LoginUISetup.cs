using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class LoginUISetup : EditorWindow
{
    [MenuItem("Tools/로그인 UI 생성")]
    public static void CreateLoginUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다. 씬에 Canvas가 있어야 합니다.");
            return;
        }

        // 로그인 패널 생성
        GameObject loginPanel = CreateLoginPanel(canvas.transform);

        // 회원가입 패널 생성
        GameObject signUpPanel = CreateSignUpPanel(canvas.transform);
        signUpPanel.SetActive(false); // 기본 비활성화

        // LoginManager 연결
        ConnectToLoginManager(loginPanel, signUpPanel);

        Selection.activeGameObject = loginPanel;
        Debug.Log("로그인/회원가입 UI가 생성되었습니다!");
    }

    static GameObject CreateLoginPanel(Transform parent)
    {
        GameObject panel = new GameObject("LoginPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 320);
        rect.anchoredPosition = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        img.raycastTarget = true;

        // 전체 화면 블로커 (뒤 클릭 방지)
        GameObject blocker = new GameObject("Blocker");
        blocker.transform.SetParent(panel.transform, false);
        blocker.transform.SetAsFirstSibling();
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.offsetMin = new Vector2(-1000, -1000);
        blockerRect.offsetMax = new Vector2(1000, 1000);
        Image blockerImg = blocker.AddComponent<Image>();
        blockerImg.color = new Color(0, 0, 0, 0.3f);
        blockerImg.raycastTarget = true;

        // 타이틀
        CreateText(panel.transform, "Title", "로그인", new Vector2(0, 120), 28, Color.white);

        // ID 입력
        CreateInputField(panel.transform, "ID_InputField", "아이디 입력", new Vector2(0, 50), false);

        // 비밀번호 입력
        CreateInputField(panel.transform, "Password_InputField", "비밀번호 입력", new Vector2(0, -20), true);

        // 로그인 버튼
        CreateButton(panel.transform, "LoginButton", "로그인", new Vector2(0, -90), new Color(0.2f, 0.6f, 0.2f), 200, 45);

        // 회원가입으로 이동 버튼
        CreateButton(panel.transform, "GoToSignUpButton", "회원가입", new Vector2(0, -140), new Color(0.3f, 0.3f, 0.3f), 200, 40);

        // 에러 메시지
        GameObject errorText = CreateText(panel.transform, "LoginErrorText", "", new Vector2(0, -185), 16, Color.red);
        errorText.SetActive(false);

        return panel;
    }

    static GameObject CreateSignUpPanel(Transform parent)
    {
        GameObject panel = new GameObject("SignUpPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 400);
        rect.anchoredPosition = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        img.raycastTarget = true;

        // 전체 화면 블로커 (뒤 클릭 방지)
        GameObject blocker = new GameObject("Blocker");
        blocker.transform.SetParent(panel.transform, false);
        blocker.transform.SetAsFirstSibling();
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.offsetMin = new Vector2(-1000, -1000);
        blockerRect.offsetMax = new Vector2(1000, 1000);
        Image blockerImg = blocker.AddComponent<Image>();
        blockerImg.color = new Color(0, 0, 0, 0.5f);
        blockerImg.raycastTarget = true;

        // 타이틀
        CreateText(panel.transform, "Title", "회원가입", new Vector2(0, 160), 28, Color.white);

        // ID 입력
        CreateInputField(panel.transform, "SignUp_ID_InputField", "아이디 입력 (4자 이상)", new Vector2(0, 90), false);

        // 비밀번호 입력
        CreateInputField(panel.transform, "SignUp_Password_InputField", "비밀번호 입력 (6자 이상)", new Vector2(0, 20), true);

        // 비밀번호 확인 입력
        CreateInputField(panel.transform, "SignUp_PasswordConfirm_InputField", "비밀번호 확인", new Vector2(0, -50), true);

        // 회원가입 버튼
        CreateButton(panel.transform, "SignUpButton", "가입하기", new Vector2(0, -120), new Color(0.2f, 0.5f, 0.8f), 200, 45);

        // 뒤로가기 버튼
        CreateButton(panel.transform, "BackToLoginButton", "← 돌아가기", new Vector2(0, -170), new Color(0.3f, 0.3f, 0.3f), 200, 40);

        // 에러 메시지
        GameObject errorText = CreateText(panel.transform, "SignUpErrorText", "", new Vector2(0, -215), 16, Color.red);
        errorText.SetActive(false);

        return panel;
    }

    static void ConnectToLoginManager(GameObject loginPanel, GameObject signUpPanel)
    {
        gunggme.LoginManager loginManager = FindObjectOfType<gunggme.LoginManager>();
        if (loginManager == null)
        {
            Debug.LogWarning("LoginManager를 찾을 수 없습니다. 수동으로 연결해주세요.");
            return;
        }

        SerializedObject so = new SerializedObject(loginManager);

        // 로그인 패널 연결
        so.FindProperty("_loginPanel").objectReferenceValue = loginPanel;
        so.FindProperty("_idInputField").objectReferenceValue =
            loginPanel.transform.Find("ID_InputField")?.GetComponent<TMP_InputField>();
        so.FindProperty("_passwordInputField").objectReferenceValue =
            loginPanel.transform.Find("Password_InputField")?.GetComponent<TMP_InputField>();
        so.FindProperty("_loginErrorText").objectReferenceValue =
            loginPanel.transform.Find("LoginErrorText")?.GetComponent<TMP_Text>();

        // 회원가입 패널 연결
        so.FindProperty("_signUpPanel").objectReferenceValue = signUpPanel;
        so.FindProperty("_signUpIdInputField").objectReferenceValue =
            signUpPanel.transform.Find("SignUp_ID_InputField")?.GetComponent<TMP_InputField>();
        so.FindProperty("_signUpPasswordInputField").objectReferenceValue =
            signUpPanel.transform.Find("SignUp_Password_InputField")?.GetComponent<TMP_InputField>();
        so.FindProperty("_signUpPasswordConfirmInputField").objectReferenceValue =
            signUpPanel.transform.Find("SignUp_PasswordConfirm_InputField")?.GetComponent<TMP_InputField>();
        so.FindProperty("_signUpErrorText").objectReferenceValue =
            signUpPanel.transform.Find("SignUpErrorText")?.GetComponent<TMP_Text>();

        so.ApplyModifiedProperties();

        // 버튼 이벤트 연결
        // 로그인 버튼
        Button loginBtn = loginPanel.transform.Find("LoginButton")?.GetComponent<Button>();
        if (loginBtn != null)
            UnityEditor.Events.UnityEventTools.AddPersistentListener(loginBtn.onClick, loginManager.OnLoginButtonClicked);

        // 회원가입으로 이동 버튼
        Button goToSignUpBtn = loginPanel.transform.Find("GoToSignUpButton")?.GetComponent<Button>();
        if (goToSignUpBtn != null)
            UnityEditor.Events.UnityEventTools.AddPersistentListener(goToSignUpBtn.onClick, loginManager.OnGoToSignUpClicked);

        // 회원가입 버튼
        Button signUpBtn = signUpPanel.transform.Find("SignUpButton")?.GetComponent<Button>();
        if (signUpBtn != null)
            UnityEditor.Events.UnityEventTools.AddPersistentListener(signUpBtn.onClick, loginManager.OnSignUpButtonClicked);

        // 돌아가기 버튼
        Button backBtn = signUpPanel.transform.Find("BackToLoginButton")?.GetComponent<Button>();
        if (backBtn != null)
            UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn.onClick, loginManager.OnBackToLoginClicked);

        Debug.Log("LoginManager에 UI가 연결되었습니다!");
    }

    static GameObject CreateInputField(Transform parent, string name, string placeholder, Vector2 position, bool isPassword)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 50);
        rect.anchoredPosition = position;

        Image bg = inputObj.AddComponent<Image>();
        bg.color = Color.white;

        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

        // Text Area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        textArea.AddComponent<RectMask2D>();

        // Placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(textArea.transform, false);
        RectTransform phRect = placeholderObj.AddComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = Vector2.zero;
        phRect.offsetMax = Vector2.zero;

        TextMeshProUGUI phText = placeholderObj.AddComponent<TextMeshProUGUI>();
        phText.text = placeholder;
        phText.fontSize = 20;
        phText.color = new Color(0.5f, 0.5f, 0.5f);
        phText.alignment = TextAlignmentOptions.Left;
        phText.verticalAlignment = VerticalAlignmentOptions.Middle;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(textArea.transform, false);
        RectTransform txtRect = textObj.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.fontSize = 22;
        tmpText.color = Color.black;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;

        inputField.textViewport = textAreaRect;
        inputField.textComponent = tmpText;
        inputField.placeholder = phText;

        if (isPassword)
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }

        return inputObj;
    }

    static GameObject CreateButton(Transform parent, string name, string text, Vector2 position, Color color, float width = 120, float height = 45)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = position;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        // Hover 색상 설정
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(color.r + 0.1f, color.g + 0.1f, color.b + 0.1f);
        colors.pressedColor = new Color(color.r - 0.1f, color.g - 0.1f, color.b - 0.1f);
        btn.colors = colors;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 20;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;

        return btnObj;
    }

    static GameObject CreateText(Transform parent, string name, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(350, 40);
        rect.anchoredPosition = position;

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = color;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;

        return textObj;
    }
}
