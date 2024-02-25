using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Cola.ColaSignalR;

public class ColaSignalR : Hub<IColaSignalRHub>
{
    public override Task OnConnectedAsync()
    {
        Clients.All.ReceiveMessageAsync($"{Context.ConnectionId} - signalR Connected");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Clients.All.ReceiveMessageAsync($"{Context.ConnectionId} - signalR Disconnected");
        return base.OnDisconnectedAsync(exception);
    }

    #region group
    
    /// <summary>
    /// 加入指定组
    /// </summary>
    /// <param name="userConnectionId">userConnectionId</param>
    /// <param name="groupName">groupName</param>
    public async Task<string> UserConnectionIdAddToGroupAsync(string userConnectionId, string groupName)
    {
        await Groups.AddToGroupAsync(userConnectionId, groupName);
        return userConnectionId;
    }

    /// <summary>
    /// 加入指定组
    /// </summary>
    /// <param name="groupName">groupName</param>
    public async Task<string> AddToGroupAsync(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        return Context.ConnectionId;
    }
    
    /// <summary>
    /// 退出指定组
    /// </summary>
    /// <param name="groupName">groupName</param>
    public async Task<string> UserConnectionIdRemoveFromGroupAsync(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        return Context.ConnectionId;
    }
    
    /// <summary>
    /// 退出指定组
    /// </summary>
    /// <param name="userConnectionId">userConnectionId</param>
    /// <param name="groupName">groupName</param>
    public async Task<string> RemoveFromGroupAsync(string userConnectionId, string groupName)
    {
        await Groups.RemoveFromGroupAsync(userConnectionId, groupName);
        return userConnectionId;
    }

    #endregion
    
    /// <summary>
    /// 向指定群组发送信息
    /// </summary>
    /// <param name="groupName">组名</param>
    /// <param name="message">信息内容</param>  
    public async Task SendMessageToGroupAsync(string groupName, object message)
    {
        await Clients.Group(groupName).ReceiveMessageAsync(message);
    }

    

    /// <summary>
    /// SignalR推送信息给指定客户端接
    /// </summary>
    /// <param name="userId">userId</param>
    /// <param name="message">信息内容</param>
    public async Task SendPrivateMessageAsync(string userId, object message)
    {
        await Clients.Client(userId).SendPrivateMessageAsync(message);
    }
    
    /// <summary>
    /// SignalR推送信息给所有客户端
    /// </summary>
    /// <param name="message">信息内容</param>
    public async Task SendMessageAsync(object message)
    {
        await Clients.All.ReceiveMessageAsync(message);
    }
    
    /// <summary>
    /// SignalR推送信息给所有客户端
    /// </summary>
    /// <param name="message">信息内容</param>
    public async Task SendMessageToOtherUserAsync(object message)
    {
        await Clients.Others.ReceiveMessageAsync(message);
    }
}