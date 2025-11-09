using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// 🧩 Cấu trúc phản hồi JSON từ backend
[System.Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string player;
    public string score;
    public string playTime;
}

[System.Serializable]
public class LeaderboardResponse
{
    public int totalPlayers;
    public List<LeaderboardEntry> leaderboard;
}

public class LeaderboardFetcher1 : MonoBehaviour
{
    private string API_URL = GameConfig.backendUrlSomniaSDSget;
    [Header("ScrollView Content (Parent of rows)")]
    public Transform content; // assign Content object here

    public Text playerHighestScoreValue;

    private void Start()
    {
        FetchLeaderboard();
    }

    // ✅ Gọi hàm này để lấy leaderboard
    public void FetchLeaderboard()
    {
        StartCoroutine(GetLeaderboardCoroutine());
    }

    private IEnumerator GetLeaderboardCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            Debug.Log("📡 Fetching leaderboard from: " + API_URL);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Response: " + request.downloadHandler.text);

                // Parse JSON
                LeaderboardResponse data = JsonUtility.FromJson<LeaderboardResponse>(
                    FixJson(request.downloadHandler.text)
                );

                if (data != null && data.leaderboard != null)
                {
                    UpdateUI(data.leaderboard);

                    Debug.Log("🎯 Total players: " + data.totalPlayers);
                    foreach (var entry in data.leaderboard)
                    {
                        Debug.Log($"🏆 Rank {entry.rank} | {entry.player} | Score: {entry.score} | PlayTime: {entry.playTime}s");
                        // ✅ So sánh với ví hiện tại của người chơi
                        if (!string.IsNullOrEmpty(PlayerDataManager.Instance.walletAddress))
                        {
                            // So sánh không phân biệt hoa thường
                            if (entry.player.Equals(PlayerDataManager.Instance.walletAddress, System.StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.Log($"💎 Your current rank is: {entry.rank}");
                                playerHighestScoreValue.text = entry.rank.ToString();
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ Empty or invalid leaderboard data.");
                }
            }
            else
            {
                Debug.LogError("❌ Error fetching leaderboard: " + request.error);
            }
        }
    }

    private void UpdateUI(List<LeaderboardEntry> leaderboard)
    {
        // Giới hạn hiển thị tối đa 10 dòng
        int count = Mathf.Min(leaderboard.Count, 10);

        for (int i = 0; i < 10; i++)
        {
            // Tìm hàng tương ứng (01Row, 02Row, …)
            Transform row = content.Find((i + 1).ToString("D2") + "Row");
            if (row == null) continue;

            // Lấy các TextMeshPro trong hàng
            TMP_Text noText = row.Find("No").GetComponent<TMP_Text>();
            TMP_Text walletText = row.Find("WalletAddress").GetComponent<TMP_Text>();
            TMP_Text scoreText = row.Find("Apple").GetComponent<TMP_Text>();
            TMP_Text playTimeText = row.Find("PlayTime").GetComponent<TMP_Text>();

            if (i < count)
            {
                var entry = leaderboard[i];
                noText.text = entry.rank.ToString();
                walletText.text = entry.player;
                scoreText.text = entry.score;
                playTimeText.text = entry.playTime;
            }
            else
            {
                // Nếu không có dữ liệu cho hàng này → clear text
                noText.text = "-";
                walletText.text = "-";
                scoreText.text = "-";
                playTimeText.text = "-";
            }
        }

        Debug.Log("🏁 Leaderboard UI updated!");
    }

    // ⚙️ JsonUtility không parse được root array nên ta cần sửa JSON nếu cần
    private string FixJson(string json)
    {
        // Nếu JSON bắt đầu bằng [, thêm object wrapper để JsonUtility đọc được
        if (json.StartsWith("["))
            json = "{\"leaderboard\":" + json + "}";
        return json;
    }
}
