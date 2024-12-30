using Duck.Http.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine;
using Duck.Http;
using Utils;
using System.Text;

public class NetworkManager : AutoSingletonMono<NetworkManager>
{
    [Serializable]
    public enum Mode
    {
        LOCAL,
        DEV,
        STAGE,
        PROD
    }

    [Serializable]
    public enum GameType
    {
        GAME_EVENT,
        GAME_CENTER
    }

    [Serializable]
    public class ServerInfo
    {
        public string apiServer = "https://dev-meta-api.defiwarrior.io";
        //public List<EncryptKey> encryptKeys;
    }

    [Serializable]
    public class EncryptKey
    {
        public GameType type;
        public string key;
        public string xorKey;
    }

    [Serializable]
    public class DictionaryServerMode : SerializableDictionary<Mode, ServerInfo>
    { }

    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    private DictionaryServerMode dictionaryServers = new DictionaryServerMode();

    [SerializeField]
    public Mode mode = Mode.DEV;

    private ServerInfo serverInfo = new ServerInfo();

    public string apiServer => serverInfo.apiServer;

    //public List<EncryptKey> encryptKeys => serverInfo.encryptKeys;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void InitEnvironment()
    {
        Debug.Log("[NetworkManager] -- Mode: " + mode.ToString());
        Debug.Log("[NetworkManager] -- Version: " + Application.version);
        if (dictionaryServers.TryGetValue(mode, out ServerInfo sInfo))
        {
            serverInfo = sInfo;
        }
    }

    public IHttpRequest HttpGet(string apiServer, string functionPath, JObject jsonObj = null, string authenPre = "Bearer {0}")
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;

        var request = Http.Get(url)
            .SetHeader("Content-Type", "application/json");
        //.SetHeader("Authorization", string.Format(authenPre, userAuthenData.token));

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error}");
        });
        return request;

    }

    public IHttpRequest HttpGetNoAuthen(string apiServer, string functionPath, JObject jsonObj = null)
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;

        var request = Http.Get(url)
            .SetHeader("Content-Type", "application/json");

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error}");
        });
        return request;
    }

    public IHttpRequest HttpPost(string apiServer, string functionPath, JObject jsonObj, string authenPre = "Bearer {0}")
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;


        var request = Http.PostJson(url, jsonObj.ToString())
             .SetHeader("Content-Type", "application/json");
        //.SetHeader("Authorization", string.Format(authenPre, userAuthenData.token));

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error} - {r.Text}");
        });

        return request;
    }

    public IHttpRequest HttpPostNoAuthen(string apiServer, string functionPath, JObject jsonObj)
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;


        if (NetworkManager.Instance.mode == Mode.DEV)
        {
            Debug.Log("NetworkManager -- HttpPost: " + url + $" -- {jsonObj}");
        }

        var request = Http.PostJson(url, jsonObj.ToString())
             .SetHeader("Content-Type", "application/json");

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error} - {r.Text}");
        });

        return request;
    }

    public IHttpRequest HttpPut(string apiServer, string functionPath, JObject jsonObj, string authenPre = "Bearer {0}")
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;

        Debug.Log("NetworkManager -- HttpPut: " + url);
        var request = Http.Put(url, jsonObj.ToString())
            .SetHeader("Content-Type", "application/json");
        //.SetHeader("Authorization", string.Format(authenPre, userAuthenData.token));

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error}");
        });

        return request;
    }

    public IHttpRequest HttpDelete(string apiServer, string functionPath, string authenPre = "Bearer {0}")
    {
        string url = new Uri(new Uri(apiServer), functionPath).AbsoluteUri;

        var request = Http.Delete(url)
             .SetHeader("Content-Type", "application/json");
        //.SetHeader("Authorization", string.Format(authenPre, userAuthenData.token));

        request.OnSuccess(r =>
        {
        })
        .OnError(r =>
        {
            Debug.LogError($"[NetworkManager] {functionPath}-- error : {r.Error}");
        });

        return request;
    }
}

[Serializable]
public class UserAuthenData
{
    public string accessToken;
    public string refreshToken;
    public long expiredAt;
    public string userId;
    public string userStatus;
    public string message;

    public void Reset()
    {
        accessToken = string.Empty;
        refreshToken = string.Empty;
        expiredAt = 0;
        userId = "";
    }
}