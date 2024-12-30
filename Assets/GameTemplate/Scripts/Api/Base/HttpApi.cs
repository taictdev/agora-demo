using Cysharp.Threading.Tasks;
using Duck.Http.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

/// <summary>
/// Represent to an API
/// </summary>
public interface IHttpApi
{
}

[Serializable]
public class BasicError : Exception, IResponseData
{
    public BasicError()
    { }

    public int code;
    public string error;
    public string message;
}

public abstract class HttpApi<T> : IHttpApi where T : class, IResponseData
{
    public enum JsonParser
    {
        JsonUtility,
        JsonNet // for complex structure ex nested list
    }

    protected abstract string ApiUrl { get; }

    protected abstract IHttpRequest GetHttpRequest();

    public string ErrorMessage { get; internal set; }

    public string ErrorEnum { get; internal set; }

    public BasicError Error { get; internal set; }

    protected JsonParser jsonParser = JsonParser.JsonNet;

    public long StatusCode { get; set; }

    public HttpApi<T> UseJsonParser(JsonParser parser)
    {
        jsonParser = parser;
        return this;
    }

    public virtual UniTask<T> Send()
    {
        var s = new UniTaskCompletionSource<T>();
        Send(t => { s.TrySetResult(t); },
            (e, msg) => s.TrySetResult(null)); //ignore exception temporarily, we handle errors on NetworkManager
        return s.Task;
    }

    public virtual UniTask<T> TryCatchSend()
    {
        var s = new UniTaskCompletionSource<T>();
        GetHttpRequest()
            .OnSuccess(r =>
            {
                this.OnSuccess(r, t =>
                {
                    s.TrySetResult(t);
                }, (error, msg) =>
                {
                    s.TrySetException(new Exception(msg));
                });
            })
            .OnError(r => this.OnError(r, (error, msg) =>
            {
                s.TrySetException(new BasicError() { error = error, code = (int)r.StatusCode, message = msg });
            }))
            .Send();

        return s.Task;
    }

    public virtual void Send(Action<T> onSuccess, Action<string, string> onError = null)
    {
        GetHttpRequest()
            .OnSuccess(r => { OnSuccess(r, onSuccess, onError); })
            .OnError(r => { OnError(r, onError); })
            .Send();
    }

    protected virtual void OnSuccess(HttpResponse r, Action<T> onSuccess, Action<string, string> onError)
    {
        StatusCode = r.StatusCode;
        T obj = null;
        try
        {
            Debug.Log(typeof(T).ToString() + " : " + r.Text);
            obj = ParseJson<T>(r.Text);
            //Debug.Log(typeof(T).ToString() + " parsed : " + JsonConvert.SerializeObject(obj));

            BasicError er = obj as BasicError;
            if (er != null && er.code != 200)
                onError?.Invoke(er.error, er.message);
            else
                onSuccess?.Invoke(obj);
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            Debug.LogError($"[HttpRequestUtils] -- error {ErrorMessage}");
            onError?.Invoke("error", ErrorMessage);
        }
    }

    protected TR ParseJson<TR>(string json) where TR : class, IResponseData
    {
        TR obj = null;
        switch (jsonParser)
        {
            case JsonParser.JsonUtility:
                obj = JsonUtility.FromJson<TR>(json);
                break;

            case JsonParser.JsonNet:
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    obj = JsonConvert.DeserializeObject<TR>(json, settings);
                    break;
                }
        }

        return obj;
    }

    protected virtual void OnError(HttpResponse r, Action<string, string> onError)
    {
        StatusCode = r.StatusCode;
        ErrorMessage = r.Error;
        if (string.IsNullOrEmpty(r.Text))
        {
            onError?.Invoke("error", ErrorMessage);
            return;
        }
        try
        {
            //  Debug.Log($"[HttpApi] HttpResponse {r.Text}");
            JObject jObject = JObject.Parse(r.Text);

            if (jObject.TryGetValue("message", out JToken msg))
                ErrorMessage = msg?.ToString();
            if (jObject.TryGetValue("error", out JToken error))
                ErrorEnum = error?.ToString();

            if (jObject.TryGetValue("errors", out JToken errorsToken))
            {
                JObject errorsObject = (JObject)errorsToken;
                JToken messageToken;
                if (errorsObject.TryGetValue("message", out messageToken))
                {
                    string errorMessage = (string)messageToken;
                    Debug.Log("Error Message: " + errorMessage);
                    ErrorMessage = errorMessage;
                }
            }
            //string errorsMessage = (string)jObject["errors"]["message"];
            //ErrorMessage = errorsMessage;
        }
        catch (Exception e)
        {
            Debug.LogError($"[HttpApi] Exception {e.Message}");
        }
        //Debug.Log($"[HttpApi] ErrorMessage {ErrorMessage}");
        onError?.Invoke(ErrorEnum, ErrorMessage);
    }
}