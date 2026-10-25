Imports System.Data
Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Threading.Tasks

Namespace Database

    Public Module DataReaderDiff
        
          Public Class cWebSocketHelper

            Private Const ServerPrefix As String = "http://localhost:5123/ws/"
     
            ''' <summary>
            ''' How long to hold outgoing broadcasts before actually sending them, giving
            ''' clients (which reconnect roughly every 3s) a chance to establish their
            ''' WebSocket connection first. The countdown starts on the first broadcast,
            ''' not at server startup - see <see cref="GetStartupGraceTask"/>.
            ''' </summary>
            Private Const StartupGracePeriodMs As Integer = 5000

            ' Singleton instance (lazy initialization)
            Private Shared _instance As cWebSocketHelper = Nothing
            Private Shared ReadOnly _instanceLock As New Object()
            Public Shared ReadOnly Property Instance As cWebSocketHelper
                Get
                    If _instance Is Nothing Then
                        SyncLock _instanceLock
                            If _instance Is Nothing Then
                                _instance = New cWebSocketHelper()
                            End If
                        End SyncLock
                    End If
                    Return _instance
                End Get
            End Property

            Private _listener As HttpListener
            Private _clients As New ConcurrentDictionary(Of WebSocket, Byte)()
            Private _listenCts As CancellationTokenSource
            Private _ready As New ManualResetEventSlim(False)
            Private _sendLock As New SemaphoreSlim(1, 1)
            Private _startupGraceTask As Task = Nothing
            Private ReadOnly _startupGraceGate As New Object()

            ' Private constructor — only called via Instance property
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
                Instance._ready.Wait()
                Task.Run(Sub() Instance.BroadcastInternalAsync(args).GetAwaiter().GetResult())
            End Sub

            Private Async Function BroadcastInternalAsync(args() As Object) As Task
                Await GetStartupGraceTask()

                Await _sendLock.WaitAsync()
                Try
                    Dim json As String = Text.Json.JsonSerializer.Serialize(args)
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

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Returns a Task that completes once the startup grace period has elapsed.
            ''' The countdown starts on first use - i.e. when the first message is
            ''' actually broadcast, not at server construction - so if nothing is
            ''' broadcast for a while after startup, that first message isn't delayed
            ''' for no reason.
            ''' </summary>
            ''' <remarks>
            ''' Every call, whether it's the first or the hundredth, whether it arrives
            ''' before or after the deadline, awaits the SAME underlying Task instance.
            ''' Only the first caller actually creates the Task.Delay; every other
            ''' caller just awaits that same instance, so there is exactly one
            ''' countdown ever running, not one per message. Once that Task has
            ''' completed, awaiting it again resolves immediately, so calls made
            ''' after the grace period has elapsed incur no delay at all.
            ''' </remarks>
            ''' -----------------------------------------------------------------------
            Private Function GetStartupGraceTask() As Task
                If _startupGraceTask Is Nothing Then
                    SyncLock _startupGraceGate
                        If _startupGraceTask Is Nothing Then
                            _startupGraceTask = Task.Delay(StartupGracePeriodMs)
                        End If
                    End SyncLock
                End If
                Return _startupGraceTask
            End Function
     
            ' Gracefully stop the server and close all clients
            Public Shared Async Function StopServerAsync() As Task
                Await Instance.StopInternalAsync()
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

            ' Returns True if the singleton instance has been created and the server is running
            Public Shared Function IsRunning() As Boolean
                Return _instance IsNot Nothing AndAlso _instance._listener IsNot Nothing AndAlso _instance._listener.IsListening
            End Function

        End Class
        
        Public Class RowDiff
            Public Property ColumnName As String
            Public Property ValueA As Object
            Public Property ValueB As Object
            Public Overrides Function ToString() As String
                Return $"{ColumnName}: [{ValueA}] -> [{ValueB}]"
            End Function
        End Class

        Private _tableNameCache As New Dictionary(Of IDataReader, String)()
        Private _columnMapCache As New Dictionary(Of IDataReader, Dictionary(Of String, Integer))()

        Public Sub BroadcastDiffs(readerA As IDataReader, readerB As IDataReader, diffs As List(Of RowDiff), rowCount As Integer)
            Dim tableName = GetTableName(readerB)
            If diffs.Any() Then
                cWebSocketHelper.BroadcastMessage("table", tableName, "rowCount", rowCount, "rowDiffs", diffs.ToArray())
            Else
                cWebSocketHelper.BroadcastMessage("table", tableName, "rowCount", rowCount)
            End If
        End Sub

        Public Function CompareCurrentRow(readerA As IDataReader, readerB As IDataReader) As List(Of RowDiff)
            Dim diffs As New List(Of RowDiff)()
            Dim colsA = GetColumnMap(readerA)
            Dim colsB = GetColumnMap(readerB)

            For Each col In colsA.Keys
                If Not colsB.ContainsKey(col) Then Continue For
                Dim valA = readerA(colsA(col))
                Dim valB = readerB(colsB(col))
                If Not ObjectsEqual(valA, valB) Then
                    diffs.Add(New RowDiff With {
                        .ColumnName = col,
                        .ValueA = valA,
                        .ValueB = valB
                    })
                End If
            Next
            Return diffs
        End Function

        Private Function GetColumnMap(reader As IDataReader) As Dictionary(Of String, Integer)
            If _columnMapCache.ContainsKey(reader) Then Return _columnMapCache(reader)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To reader.FieldCount - 1
                map(reader.GetName(i).Trim()) = i
            Next
            _columnMapCache(reader) = map
            Return map
        End Function

        Public Function GetTableName(reader As IDataReader) As String
            If _tableNameCache.ContainsKey(reader) Then Return _tableNameCache(reader)
            Dim name = "Unknown"
            Try
                Dim schema = reader.GetSchemaTable()
                If schema IsNot Nothing AndAlso schema.Rows.Count > 0 Then
                    Dim val = schema.Rows(0)("BaseTableName")
                    If val IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(val.ToString()) Then
                        name = val.ToString()
                    End If
                End If
            Catch
            End Try
            _tableNameCache(reader) = name
            Return name
        End Function

        Private Const Tolerance As Double = 0.001 ' eg. 18765.7676 vs 18765.768

        Private Function ObjectsEqual(a As Object, b As Object) As Boolean
            If a Is DBNull.Value AndAlso b Is DBNull.Value Then Return True
            If a Is DBNull.Value OrElse b Is DBNull.Value Then Return False

            If TypeOf a Is Double OrElse TypeOf a Is Single OrElse
               TypeOf a Is Decimal OrElse TypeOf b Is Double OrElse
               TypeOf b Is Single OrElse TypeOf b Is Decimal Then
                Return Math.Abs(Convert.ToDouble(a) - Convert.ToDouble(b)) <= Tolerance
            End If
            Return a.Equals(b) OrElse a.ToString() = b.ToString()
        End Function

    End Module

End Namespace