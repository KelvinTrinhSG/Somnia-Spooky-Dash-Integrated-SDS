using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public int vipNFT;
    public int gemToken;
    public int highestScore;
    public string walletAddress;
    public string walletBalance; // dạng string vì bal.displayValue là string
    // ✅ Thêm biến để đánh dấu đã lấy dữ liệu chain xong
    public bool isChainInitialized = false;
    public bool isGameInitialized = false;

    // ✅ Thời gian bắt đầu, kết thúc, và hiệu (tính bằng giây)
    public float startTime;
    public float endTime;
    public float duration;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    // ✅ Gọi khi bắt đầu tính thời gian
    public void StartTimer()
    {
        startTime = Time.time;
        endTime = 0f;
        duration = 0f;
        Debug.Log("⏱️ Timer started at: " + startTime);
    }

    // ✅ Gọi khi kết thúc tính thời gian
    public void StopTimer()
    {
        endTime = Time.time;
        duration = endTime - startTime;
        Debug.Log("⏹️ Timer stopped at: " + endTime + " (Duration: " + duration + "s)");
    }

    // ✅ Gọi để reset toàn bộ biến về 0
    public void ResetTimer()
    {
        startTime = 0f;
        endTime = 0f;
        duration = 0f;
        Debug.Log("🔄 Timer reset.");
    }
}