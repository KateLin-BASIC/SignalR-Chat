Imports Microsoft.AspNet.SignalR
Imports Microsoft.Owin
Imports Owin

<Assembly: OwinStartup(GetType(SignalRChat.Startup))>
Namespace SignalRChat
    Public Class Startup
        Public Sub Configuration(ByVal app As IAppBuilder)
            app.MapSignalR()
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(100)
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(9)
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(3)
        End Sub
    End Class
End Namespace