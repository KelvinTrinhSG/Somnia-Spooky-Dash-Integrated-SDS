public static class GameConfig
{
    // Địa chỉ contract ERC721
    public static readonly string NFTTokenGateContractAddress = "0x83791B8a4D271f1cBF0187891d50C07e701A724f";

    // Địa chỉ contract ERC721 khác (nếu khác với TokenGate)
    public static readonly string NFTVIPContractAddress = "0x618F7065c02043eFbdbdC3790cd88535bA0200e2";

    // Địa chỉ contract ERC20 - GEM Token
    public static readonly string TokenGemContractAddress = "0x9D74c5F7D9cA9767a0a41D44d6772244978B3B24";

    public static readonly string backendUrlLeaderboard = "https://somnia-leaderboard-backend.onrender.com/updateScore";

    public static readonly string backendUrlGetTopPlayers = "https://somnia-leaderboard-backend.onrender.com/getTopPlayers";

    public static readonly string backendUrlGetRank = "https://somnia-leaderboard-backend.onrender.com/getRank/";

    public static readonly string backendUrlSomniaSDSpublish = "https://sds-leaderboard.onrender.com/api/publish";

    public static readonly string backendUrlSomniaSDSget = "https://sds-leaderboard.onrender.com/api/data";
}