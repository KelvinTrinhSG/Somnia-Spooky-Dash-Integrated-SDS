using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Thirdweb;
using System;

public class LeaderboardFetcher : MonoBehaviour
{
    private string backendUrl = GameConfig.backendUrlGetTopPlayers;

    private string backendUrlGetRank = GameConfig.backendUrlGetRank;

    [Header("Table Rows (Drag 01Row → 05Row vào đây)")]
    public List<Transform> rows;

    public Text rankText;    // ô hiển thị thứ hạng

    private string walletAddress;

    [System.Serializable]
    public class LeaderboardResponse
    {
        public List<string> players;
        public List<string> scores;
    }

    void Start()
    {
        StartCoroutine(GetTopPlayers(20)); // lấy top 10
        GetPlayerRank();
    }

    /// <summary>
    /// Gọi API để lấy danh sách top N người chơi
    /// </summary>
    /// <param name="topN">Số lượng người chơi muốn lấy (mặc định 5)</param>
    public IEnumerator GetTopPlayers(int topN = 20)
    {
        string url = $"{backendUrl}?topN={topN}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log($"📡 Fetching top {topN} players...");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log($"✅ Response: {json}");

                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);

                if (response == null || response.players == null || response.players.Count == 0)
                {
                    Debug.LogWarning("⚠️ No data found");
                    yield break;
                }

                for (int i = 0; i < rows.Count; i++)
                {
                    Transform row = rows[i];
                    TMP_Text walletText = row.Find("WalletAddress").GetComponent<TMP_Text>();
                    TMP_Text scoreText = row.Find("Apple").GetComponent<TMP_Text>();

                    if (i < response.players.Count)
                    {
                        string fullAddress = response.players[i];
                        string shortAddress = fullAddress.Length > 10
                            ? fullAddress.Substring(0, 6) + "..." + fullAddress.Substring(fullAddress.Length - 4)
                            : fullAddress;

                        walletText.text = shortAddress;
                        scoreText.text = response.scores[i];
                    }
                    else
                    {
                        walletText.text = "---";
                        scoreText.text = "---";
                    }
                }
            }
            else
            {
                Debug.LogError($"❌ Failed to get top players: {www.error}\nResponse: {www.downloadHandler.text}");
            }
        }
    }

    public async void GetPlayerRank()
    {
        try
        {
            // 🟢 Bước 1: Lấy địa chỉ ví từ Thirdweb
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            //walletAddress = "0xAe909F999CE1334eD02d40f0Afb883A967B03DEA"; //For Test

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is null or empty.");

            // 🟢 Bước 3: Gửi request
            StartCoroutine(GetPlayerRank(walletAddress));
        }
        catch (Exception ex)
        {
            Debug.LogError("Get Rank error: " + ex.Message);
        }
    }

    /// <summary>
    /// Gọi API để lấy rank của người chơi theo địa chỉ ví
    /// </summary>
    /// <param name="playerAddress">Địa chỉ ví của người chơi (VD: 0x123...)</param>
    public IEnumerator GetPlayerRank(string playerAddress)
    {
        string url = backendUrlGetRank + playerAddress;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log($"📡 Fetching rank for {playerAddress}...");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Error getting rank: {www.error}");
                rankText.text = "Error";
                yield break;
            }

            string json = www.downloadHandler.text;
            Debug.Log("✅ Response: " + json);

            RankResponse response = JsonUtility.FromJson<RankResponse>(json);
            if (response != null)
            {
                // Hiển thị kết quả
                string shortAddress = playerAddress.Length > 10
                    ? playerAddress.Substring(0, 6) + "..." + playerAddress.Substring(playerAddress.Length - 4)
                    : playerAddress;

                if (rankText != null)
                    rankText.text = response.rank.ToString();
            }
            else
            {
                Debug.LogWarning("⚠️ Invalid response format");
                rankText.text = "N/A";
            }
        }
    }

    [System.Serializable]
    public class RankResponse
    {
        public string player;
        public int rank;
    }
}
