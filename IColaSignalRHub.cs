using Cola.Core.Models;

namespace Cola.ColaSignalR;
/// <summary>
/// IColaSignalRHub
/// </summary>
public interface IColaSignalRHub
{
    /// <summary>
    /// ReceiveMessage - SignalR推送信息给所有客户端
    /// </summary>
    /// <param name="message">信息内容</param>
    /// <returns>Task</returns>
    Task ReceiveMessageAsync(object message);
    
    /// <summary>
    /// ReceiveMessage - SignalR推送信息给所有客户端
    /// </summary>
    /// <param name="message">信息内容</param>
    /// <returns>Task</returns>
    Task SendPrivateMessageAsync(object message);
    
}