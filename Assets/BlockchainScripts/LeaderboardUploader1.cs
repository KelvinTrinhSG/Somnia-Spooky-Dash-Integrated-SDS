using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LeaderboardUploader1 : MonoBehaviour
{
    // ✅ Địa chỉ API
    private string API_URL = GameConfig.backendUrlSomniaSDSpublish;

    // ✅ Gọi hàm này để gửi dữ liệu
    public void UploadScore(string playerAddress, float score, float duration)
    {
        StartCoroutine(SendData(playerAddress, score, duration));
    }

    private IEnumerator SendData(string playerAddress, float score, float duration)
    {
        // Chuyển score và duration (float) sang int
        int intScore = Mathf.RoundToInt(score);
        int intPlayTime = Mathf.RoundToInt(duration);

        // Tạo JSON thủ công
        string jsonData = $"{{\"player\":\"{playerAddress}\",\"score\":{intScore},\"playTime\":{intPlayTime}}}";

        // Gói dữ liệu thành UnityWebRequest POST
        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("🚀 Sending data to API: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Upload success! Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Upload failed: " + request.error);
            }
        }
    }
}
