using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Google; // 💡 구글 플러그인을 쓰기 위해 꼭 추가해야 해!

public class LoginManager : MonoBehaviour
{
    private FirebaseAuth auth;

    // 💡 아까 파이어베이스 콘솔에서 복사한 웹 클라이언트 ID를 여기에 붙여넣어 줘!
    public string webClientId = "248541481899-phol3f4kdljtlvu0prqi9a7q9ij9fheo.apps.googleusercontent.com";

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        // 💡 구글 로그인 플러그인 초기 세팅
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true, // 파이어베이스 인증을 위해 토큰(Token)을 꼭 달라고 요청해야 해
            WebClientId = webClientId
        };
    }

// ---------------------------------------------------
    // 1. 게스트 로그인 (바로 작동 가능!)
    // ---------------------------------------------------
    public async void OnClickGuestLogin()
    {
        Debug.Log("게스트 로그인 시도 중...");
        try
        {
            // 파이어베이스 익명 로그인 함수 호출
            AuthResult result = await auth.SignInAnonymouslyAsync();
            FirebaseUser user = result.User;
            Debug.Log($"게스트 로그인 성공! 환영합니다, UID: {user.UserId}");

            // 로그인 성공 처리 (타이틀 매니저한테 성공했다고 알려줌)
            FindObjectOfType<TitleManager>().OnLoginSuccess();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게스트 로그인 에러: {e.Message}");
        }
    }

    // ---------------------------------------------------
    // 2. 구글 로그인 진짜 로직!
    // ---------------------------------------------------
    public async void OnClickGoogleLogin()
    {
        Debug.Log("구글 로그인 시도 중...");
        try
        {
            // 1. 핸드폰에 구글 로그인 창 띄우고 결과(토큰) 받아오기
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            Debug.Log("구글 계정 선택 완료! 토큰 발급 성공.");

            // 2. 받아온 구글 토큰을 파이어베이스가 이해할 수 있는 티켓(Credential)으로 변환
            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

            // 3. 파이어베이스에 진짜 로그인 요청!
            LoginWithFirebaseCredential(credential);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"구글 로그인 창 띄우기 실패: {e.Message}");
            // 유저가 로그인 창을 닫았거나, 인터넷이 끊겼을 때 여기로 옴
        }
    }

    // (애플 로그인은 일단 냅두기)
    public void OnClickAppleLogin() { }

    // ---------------------------------------------------
    // 🌟 파이어베이스 최종 로그인 공통 함수
    // ---------------------------------------------------
    private async void LoginWithFirebaseCredential(Credential credential)
    {
        try
        {
            // 변수 이름을 result에서 user로 바꿔서 안 헷갈리게 정리!
            FirebaseUser user = await auth.SignInWithCredentialAsync(credential);
            
            // 💡 핵심 변경점: user.User.DisplayName이 아니라 그냥 user.DisplayName과 user.UserId를 사용해!
            Debug.Log($"소셜 로그인 최종 성공! 유저 이름: {user.DisplayName}, UID: {user.UserId}");
            
            FindObjectOfType<TitleManager>().OnLoginSuccess();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파이어베이스 로그인 실패: {e.Message}");
        }
    }
}