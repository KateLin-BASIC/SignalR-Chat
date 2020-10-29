Imports System.IO
Imports System.Net.Http
Imports Microsoft.AspNet.SignalR

Public Class ChatHub
    Inherits Hub

    Public Sub Send(ByVal name As String, ByVal message As String)
        If Not message.Length > 100 And Not String.IsNullOrWhiteSpace(message) And Not String.IsNullOrWhiteSpace(name) And Not name.Length > 10 Then
            Dim enumerator As IDictionaryEnumerator = System.Web.HttpRuntime.Cache.GetEnumerator()

            While enumerator.MoveNext()
                If Split(enumerator.Key.ToString, "/")(2) = name Then
                    Clients.All.broadcastMessage(name, message)
                End If
            End While
        End If
    End Sub

    Public Sub Joined(ByVal name As String, ByVal des As String)
        If Not name.Length > 10 Then
            Dim strFile As String = $"{HttpRuntime.AppDomainAppPath}\Log\ConnectLog_{DateTime.Today:dd-MMM-yyyy}.txt"
            File.AppendAllText(strFile, $"{DateTime.Now} | NEW CONNECTION | {name}{vbCrLf}{des}-------------{vbCrLf}")

            Dim key = String.Concat("Users", "/", Context.ConnectionId, "/", name)

            If HttpRuntime.Cache(key) Is Nothing Then
                HttpRuntime.Cache.Add(key, True, Nothing, DateTime.Now.AddYears(1), HttpRuntime.Cache.NoSlidingExpiration, CacheItemPriority.Low, Nothing)
            End If

            Clients.All.joinMessage(name)
        End If
    End Sub

    Public Overloads Overrides Function OnDisconnected(stopCalled As Boolean) As System.Threading.Tasks.Task
        Try
            Dim enumerator As IDictionaryEnumerator = System.Web.HttpRuntime.Cache.GetEnumerator()

            While enumerator.MoveNext()
                If Split(enumerator.Key.ToString, "/")(1) = Context.ConnectionId Then
                    HttpRuntime.Cache.Remove(enumerator.Key)
                    Clients.All.exited(Split(enumerator.Key.ToString, "/")(2))
                End If
            End While
        Catch ex As Exception

        End Try

        Return MyBase.OnDisconnected(stopCalled)
    End Function

    Public Function GetAllActiveConnections() As List(Of String)
        Dim enumerator As IDictionaryEnumerator = System.Web.HttpRuntime.Cache.GetEnumerator()

        Dim str1 As String = String.Empty
        Dim list1 As List(Of String) = New List(Of String)

        While enumerator.MoveNext()
            list1.Add(Split(enumerator.Key.ToString, "/")(2))
        End While

        Return list1.ToList()
    End Function
End Class