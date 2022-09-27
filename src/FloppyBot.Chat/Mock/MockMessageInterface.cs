using FloppyBot.Chat.Entities;
using FloppyBot.Chat.Entities.Identifiers;

namespace FloppyBot.Chat.Mock;

public class MockMessageInterface : IChatInterface
{
    private const string IF_NAME = "Mock";

    private readonly Stack<ChatMessage> _receivedMessages = new();
    private readonly Stack<string> _sentMessages = new();
    private readonly string _channelName;

    public MockMessageInterface(
        ChatInterfaceFeatures features = ChatInterfaceFeatures.None,
        string channelName = "MyChannel")
    {
        SupportedFeatures = features;
        MessageReceived += OnMessageReceived;
        _channelName = channelName;
    }

    public IEnumerable<ChatMessage> ReceivedMessages => _receivedMessages;
    public Stack<string> SentMessages => _sentMessages;

    public string Name => IF_NAME;
    public ChatInterfaceFeatures SupportedFeatures { get; }

    private ChannelIdentifier ChannelIdentifier => new(Name, _channelName);

    private ChatMessageIdentifier CreateNewIdentifier()
    {
        return new ChatMessageIdentifier(
            Name,
            _channelName,
            Guid.NewGuid().ToString());
    }

    public void Connect()
    {
        // but nothing happened
    }

    public void Disconnect()
    {
        // but nothing happened
    }

    public void InvokeReceivedMessage(string username, string message, PrivilegeLevel privilegeLevel)
    {
        var msg = new ChatMessage(
            CreateNewIdentifier(),
            new ChatUser(
                new ChannelIdentifier(IF_NAME, username),
                username,
                privilegeLevel),
            message);
        MessageReceived?.Invoke(msg, this);
    }

    public void SendMessage(string message)
    {
        _sentMessages.Push(message);
    }

    public void SendMessage(ChannelIdentifier _, string message)
    {
        SendMessage(message);
    }

    public event ChatMessageReceivedDelegate? MessageReceived;

    private void OnMessageReceived(ChatMessage chatMessage, IChatInterface _)
    {
        _receivedMessages.Push(chatMessage);
    }

    public void Dispose()
    {
        // but nothing happened
    }
}
