namespace FloppyBot.Chat.Twitch.Config;

public record TwitchConfiguration(
    string Username,
    string Token,
    string Channel,
    string ClientId,
    string AccessToken,
    bool DisableWhenChannelIsOffline,
    int MonitorInterval
)
{
    [Obsolete("This constructor is only present for configuration purposes and should not be used")]
    // ReSharper disable once UnusedMember.Global
    public TwitchConfiguration()
        : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true, 30) { }

    public bool HasTwitchApiCredentials =>
        !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(AccessToken);
}
