using System;
using System.Collections;
using System.Text;
using JetBrains.Annotations;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;

public class RestClient : MonoBehaviour
{
    public IEnumerator PostJson<TRequest, TResponse>(string url, TRequest body, [CanBeNull] string bearerToken, Action<TResponse> onSuccess, Action<string> onError = null)
    {
        // serialze objek ke json
        var rawKson = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));

        // idk dude, request builder or something
        using UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(rawKson);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = 20; // maksimal 10 detik buat inferensi
        if (!string.IsNullOrEmpty(bearerToken))
        {
            req.SetRequestHeader("Authorization", $"Bearer {bearerToken}");
        }
        
        // tunggu di background sampai server reply
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                // deserialize json ke obj
                TResponse responseObj = JsonConvert.DeserializeObject<TResponse>(req.downloadHandler.text);
                onSuccess?.Invoke(responseObj);
            }
            catch (Exception e)
            {
                onError?.Invoke("JSON parse error: " + e.Message);
            }
        }
        else
        {
            onError?.Invoke(req.error + " (HTTP " + req.responseCode + ")");
        }
    }
}
