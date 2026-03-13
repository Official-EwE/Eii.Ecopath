Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports System.Collections.Concurrent

Namespace NetUtilities

    Public Class cWebSocketHelper

        Private Const ServerPrefix As String = "http://localhost:5123/ws/"

        ' Singleton instance
        Private Shared ReadOnly _instance As New cWebSocketHelper()
        Public Shared ReadOnly Property Instance As cWebSocketHelper
            Get
                Return _instance
            End Get
        End Property

        Private _listener As HttpListener
        Private _clients As New ConcurrentDictionary(Of WebSocket, Byte)()
        Private _listenCts As CancellationTokenSource
        Private _ready As New ManualResetEventSlim(False)
        Private _sendLock As New SemaphoreSlim(1, 1)

        ' Private constructor — called once when _instance is created
        Private Sub New()
            _listener = New HttpListener()
            _listener.Prefixes.Add(ServerPrefix)
            _listener.Start()
            _listenCts = New CancellationTokenSource()
            Task.Run(Sub() AcceptClientsAsync().GetAwaiter().GetResult())
            _ready.Set()

            AddHandler AppDomain.CurrentDomain.ProcessExit, Sub(sender, e)
                StopServerAsync().GetAwaiter().GetResult()
            End Sub
        End Sub

        ' Background loop to accept incoming WebSocket connections
        Private Async Function AcceptClientsAsync() As Task
            While Not _listenCts.IsCancellationRequested
                Try
                    Dim ctx = Await _listener.GetContextAsync()
                    If ctx.Request.IsWebSocketRequest Then
                        Dim wsCtx = Await ctx.AcceptWebSocketAsync(Nothing)
                        _clients.TryAdd(wsCtx.WebSocket, 0)
                    Else
                        ctx.Response.StatusCode = 400
                        ctx.Response.Close()
                    End If
                Catch ex As HttpListenerException
                    Exit While
                Catch ex As Exception
                    ' Log or ignore transient errors
                End Try
            End While
        End Function

        ' Broadcast serialised args to all connected clients (fire and forget)
        Public Shared Sub BroadcastMessage(ParamArray args() As Object)
            _instance._ready.Wait()
            Task.Run(Sub() _instance.BroadcastInternalAsync(args).GetAwaiter().GetResult())
        End Sub

        Private Async Function BroadcastInternalAsync(args() As Object) As Task
            Await _sendLock.WaitAsync()
            Try
                Dim json As String = JsonConvert.SerializeObject(args)
                Dim segment As New ArraySegment(Of Byte)(Encoding.UTF8.GetBytes(json))
                Dim dead As New List(Of WebSocket)()

                For Each kvp In _clients
                    Dim ws = kvp.Key
                    If ws.State = WebSocketState.Open Then
                        Try
                            Await ws.SendAsync(segment, WebSocketMessageType.Text, True, CancellationToken.None)
                        Catch
                            dead.Add(ws)
                        End Try
                    Else
                        dead.Add(ws)
                    End If
                Next

                For Each ws In dead
                    Dim ignored As Byte
                    _clients.TryRemove(ws, ignored)
                    ws.Dispose()
                Next
            Finally
                _sendLock.Release()
            End Try
        End Function

        ' Gracefully stop the server and close all clients
        Public Shared Async Function StopServerAsync() As Task
            Await _instance.StopInternalAsync()
        End Function

        Private Async Function StopInternalAsync() As Task
            If _listener Is Nothing Then Return
            _listenCts.Cancel()
            _listener.Stop()
            _listener.Close()
            _listener = Nothing

            For Each kvp In _clients
                Dim ws = kvp.Key
                Try
                    If ws.State = WebSocketState.Open Then
                        Await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None)
                    End If
                    ws.Dispose()
                Catch
                End Try
            Next
            _clients.Clear()
        End Function

    End Class

End Namespace