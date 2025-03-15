public class CreateGameResponse
{
    public CreateGameResponse(string gameId, string playerName)
    {
        GameId = gameId;
        PlayerName = playerName;
    }

    public string GameId { get; set; }
    public string PlayerName { get; set; }
}