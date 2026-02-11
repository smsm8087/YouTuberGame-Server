using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIClient : MonoBehaviour
{
    private const string API_URL = "https://localhost:5001/api";
    private string _authToken = null;

    public static APIClient Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadToken();
    }

    private void LoadToken()
    {
        _authToken = PlayerPrefs.GetString("auth_token", null);
    }

    private void SaveToken(string token)
    {
        _authToken = token;
        PlayerPrefs.SetString("auth_token", token);
        PlayerPrefs.Save();
    }

    public void ClearToken()
    {
        _authToken = null;
        PlayerPrefs.DeleteKey("auth_token");
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(_authToken);

    // 회원가입
    public IEnumerator Register(string email, string password, string playerName, string channelName, System.Action<bool, string> callback)
    {
        var data = new { Email = email, Password = password, PlayerName = playerName, ChannelName = channelName };
        yield return Post("/auth/register", data, false, callback);
    }

    // 로그인
    public IEnumerator Login(string email, string password, System.Action<bool, string> callback)
    {
        var data = new { Email = email, Password = password };
        yield return Post("/auth/login", data, false, (success, response) =>
        {
            if (success)
            {
                var auth = JsonUtility.FromJson<AuthResponse>(response);
                SaveToken(auth.Token);
            }
            callback?.Invoke(success, response);
        });
    }

    // 플레이어 데이터 조회
    public IEnumerator GetPlayerData(System.Action<bool, string> callback)
    {
        yield return Get("/player/me", callback);
    }

    // 가챠
    public IEnumerator DrawGacha(int count, System.Action<bool, string> callback)
    {
        var data = new { Count = count };
        yield return Post("/gacha/draw", data, true, callback);
    }

    // HTTP 헬퍼
    private IEnumerator Get(string endpoint, System.Action<bool, string> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(API_URL + endpoint))
        {
            if (!string.IsNullOrEmpty(_authToken))
                req.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                callback?.Invoke(true, req.downloadHandler.text);
            else
            {
                Debug.LogError($"GET {endpoint} failed: {req.error}");
                callback?.Invoke(false, req.error);
            }
        }
    }

    private IEnumerator Post(string endpoint, object data, bool auth, System.Action<bool, string> callback)
    {
        string json = data != null ? JsonUtility.ToJson(data) : "{}";
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(API_URL + endpoint, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            if (auth && !string.IsNullOrEmpty(_authToken))
                req.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                callback?.Invoke(true, req.downloadHandler.text);
            else
            {
                Debug.LogError($"POST {endpoint} failed: {req.error}");
                callback?.Invoke(false, req.error);
            }
        }
    }
}

[Serializable]
public class AuthResponse
{
    public bool Success;
    public string Message;
    public string Token;
    public string UserId;
}
