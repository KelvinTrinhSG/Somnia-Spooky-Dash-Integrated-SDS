using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Thirdweb;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUploader : MonoBehaviour
    {
        private string backendUrl = GameConfig.backendUrlLeaderboard;
    private string walletAddress;
    private float amount;

    [Header("UI")]
    public Button[] actionButtons;
    public Button[] actionOtherButtons;
    public Text[] statusTexts;

    public async void SubmitHighestScore()
    {

        SetButtonsInteractable(false);
        SetStatusTexts("Submitting... Please wait");

        try
        {
            // 🟢 Bước 1: Lấy địa chỉ ví từ Thirdweb
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            //walletAddress = "0xAe909F999CE1334eD02d40f0Afb883A967B03DEA"; //For Test

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is null or empty.");
            string key = SceneManager.GetActiveScene().name + "HighScore";
            // 🟢 Bước 2: Lấy amount từ PlayerPrefs
            amount = PlayerPrefs.GetFloat(key, 0f);

            Debug.Log("PlayerPrefs.GetInt(playerPrefsKey, 0)" + amount);

            if (amount <= 0)
                throw new Exception("No HighScore to Submit!");

            // 🟢 Bước 3: Gửi request
            StartCoroutine(UploadHighScore(walletAddress, amount));
        }
        catch (Exception ex)
        {
            Debug.LogError("Claim error: " + ex.Message);
            SetStatusTexts(ex.Message);
            SetButtonsInteractable(true);
        } 
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in actionButtons)
        {
            btn.interactable = interactable;
        }
    }

    void SetOtherButtonsInteractable(bool interactable)
    {
        foreach (var btn in actionOtherButtons)
        {
            btn.interactable = interactable;
        }
    }

    void SetStatusTexts(string message)
    {
        foreach (var text in statusTexts)
        {
            text.gameObject.SetActive(true);
            text.text = message;
        }
    }

    /// <summary>
    /// Gửi điểm cao của người chơi lên backend
    /// </summary>
    /// <param name="playerAddress">Địa chỉ ví người chơi (string)</param>
    /// <param name="score">Điểm số (int)</param>
    public IEnumerator UploadHighScore(string playerAddress, float score)
        {
            // Tạo object JSON gửi đi
            var jsonData = JsonUtility.ToJson(new ScoreData
            {
                player = playerAddress,
                score = score
            });

            // Chuẩn bị request
            using (UnityWebRequest www = new UnityWebRequest(backendUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"🚀 Sending score... Player: {playerAddress}, Score: {score}");

                // Gửi request và chờ phản hồi
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✅ Score uploaded successfully: {www.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"❌ Failed to upload score: {www.error}\nResponse: {www.downloadHandler.text}");
                }
            SetStatusTexts("Highest Score Submitted");
            SetOtherButtonsInteractable(true);
        }
        }

        // Struct để convert sang JSON
        [System.Serializable]
        public class ScoreData
        {
            public string player;
            public float score;
        }
    }


