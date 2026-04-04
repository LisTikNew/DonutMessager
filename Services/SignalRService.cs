using Microsoft.AspNetCore.SignalR.Client;

namespace DonutMessenger.Client.Services;

public class SignalRService
{
    public HubConnection Connection { get; }

    public SignalRService(string serverUrl)
    {
        Connection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/chatHub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        if (Connection.State == HubConnectionState.Disconnected)
            await Connection.StartAsync();
    }

    public async Task JoinChat(int chatId)
    {
        await Connection.InvokeAsync("JoinChat", chatId);
    }

    public async Task LeaveChat(int chatId)
    {
        await Connection.InvokeAsync("LeaveChat", chatId);
    }

    public async Task SendMessage(int chatId, int senderId, string text)
    {
        await Connection.InvokeAsync("SendMessage", chatId, senderId, text);
    }
}