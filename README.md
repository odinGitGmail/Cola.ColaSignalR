### [Cola.ColaSignalR](https://github.com/odinGitGmail/Cola.ColaSignalR)

[![Version](https://flat.badgen.net/nuget/v/Cola.ColaSignalR?label=version)](https://github.com/odinGitGmail/Cola.ColaSignalR) [![download](https://flat.badgen.net/nuget/dt/Cola.ColaSignalR)](https://www.nuget.org/packages/Cola.ColaSignalR) [![commit](https://flat.badgen.net/github/last-commit/odinGitGmail/Cola.ColaSignalR)](https://flat.badgen.net/github/last-commit/odinGitGmail/Cola.ColaSignalR) [![Blog](https://flat.badgen.net/static/blog/odinsam.com)](https://odinsam.com)


#### 配置
```json
{
  
}
```

#### 注入
```csharp
builder.Services.AddColaSignalRHub();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    //添加/注册对应的Hub类,并添加路由
    //请注意：在不同的.Net Core版本中，MapHub方法也可能在app中
    //如果找不到MapHub请两个地方都试一试
    endpoints.MapHub<ColaSignalR>("/api/cola-signalr");
});
public class CommonService : ServiceBase, ICommonService
{
    private readonly IHubContext<ColaSignalR> _colaSignalRHub;
    public CommonService(IHubContext<ColaSignalR> colaSignalRHub)
    {
        _hubContext = colaSignalRHub;
    }
    
    /// <summary>
    /// SignalRSendMessage
    /// </summary>
    /// <param name="message">message</param>
    /// <returns>Task</returns>
    public Task SignalRSendMessage(string message)
    {
        await _colaSignalRHub.Clients.All.SendAsync("SendMessageAsync", message);
        return Task.CompletedTask;
    }
}
```

```js
//初始化客户端 官网 https://docs.microsoft.com/zh-cn/aspnet/core/signalr/javascript-client?view=aspnetcore-5.0&tabs=visual-studio
//创建链接并指定到在StartUp中配置的Chathub的路由，
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/api/cola-signalr")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//start开始连接，
async function start() {
    try {
        //会在后台触发连接Hub的Task OnConnectedAsync()方法，
        await connection.start();
        //如果想要在链接处获取身份信息，即使用/获取自带的UserID/Group对象什么的,
        //需要使用ASP.NET CORE 官方的authorize, 和Identity的相关内容，如果使用的是自研的登陆和认证内容，则无法使用以上的user对象
        //请手动发送empno/username等信息
        userId = connection.connectionId;
        console.log(`${connection.connectionId} - SignalR Connected.`);
        console.log();

    } catch (err) {
        console.log(err);
        //异步发起重连
        setTimeout(start, 5000);
    }
};

//重连机制
connection.onclose(async () => {
    await start();
});

//ReceiveMessageAsync为监听的数据标识
connection.on("ReceiveMessageAsync", function (res) {
    var result = JSON.parse(res);
    //UI显示监听的数据
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${result.data}`;
});

//ReceivePrivateMessageAsync为私发监听的数据标识
connection.on("ReceivePrivateMessageAsync", function (res) {
    var result = JSON.parse(res);
    //UI显示监听的数据
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${result.data}`;
});



//发起连接
start();

//按钮事件 普通发送
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var sendMessage = `${user} say ${message}`
    // SendMessage2为自定义方法ChatHub.cs 定义的相同
    connection.invoke("SendMessageToOtherUserAsync", JSON.stringify({
        data : sendMessage
    })).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

//按钮事件  加入组
document.getElementById("sendButton2").addEventListener("click", function (event) {
    var group = document.getElementById("groupInput").value;
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var sendMessage = `${user} say ${message}`
    // SendMessage2为自定义方法ChatHub.cs 定义的相同
    connection.invoke("AddToGroupAsync", group).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

//按钮事件  组内发送
document.getElementById("sendButton3").addEventListener("click", function (event) {
    var group = document.getElementById("groupInput").value;
    var message = document.getElementById("messageInput").value;
    var user = document.getElementById("userInput").value;
    var sendMessage = `${user} say ${message}`
    // SendMessage2为自定义方法ChatHub.cs 定义的相同
    connection.invoke("SendMessageToGroupAsync", group, JSON.stringify({
        data : sendMessage
    })).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});
```

#### 已实现接口
```csharp
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
```

