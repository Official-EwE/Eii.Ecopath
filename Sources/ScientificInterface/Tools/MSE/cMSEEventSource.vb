
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region


''' <summary>
''' Private worker class for the MSE forms to handle complex core messages and fire an event in response
''' </summary>
''' <remarks>This act as a filter for core messages  </remarks>
Friend Class cMSEEventSource
    Implements IDisposable

    Private m_dtReflevels As New Dictionary(Of eVarNameFlags, onChanged)
    Private m_dtDataChanged As New Dictionary(Of eMessageType, onChanged)

    Private Delegate Sub onChanged()

    Public Event onRefLevelsChanged()
    Public Event onRunCompleted()

    Public Sub New()
        m_dtReflevels.Add(eVarNameFlags.MSERefBioLower, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSERefBioUpper, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSEBBase, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSEBLim, AddressOf Me.fireOnRefLevelsChanged)

        m_dtReflevels.Add(eVarNameFlags.MSERefFleetCatchLower, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSERefFleetCatchUpper, AddressOf Me.fireOnRefLevelsChanged)

        m_dtReflevels.Add(eVarNameFlags.MSERefFleetEffortLower, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSERefFleetEffortUpper, AddressOf Me.fireOnRefLevelsChanged)

        m_dtReflevels.Add(eVarNameFlags.MSERefGroupCatchLower, AddressOf Me.fireOnRefLevelsChanged)
        m_dtReflevels.Add(eVarNameFlags.MSERefGroupCatchUpper, AddressOf Me.fireOnRefLevelsChanged)

        m_dtDataChanged.Add(eMessageType.MSERunCompleted, AddressOf Me.fireOnRunCompleted)

    End Sub

    ''' <summary>
    ''' Route core messages to an Event
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Public Sub HandleCoreMessage(ByVal msg As cMessage)
        Try
            'Reference levels
            If msg.DataType = eDataTypes.MSEGroupInput Then
                For Each var As cVariableStatus In msg.Variables
                    If m_dtReflevels.ContainsKey(var.VarName) Then
                        Try
                            m_dtReflevels.Item(var.VarName).Invoke()
                        Catch ex As Exception
                            Debug.Assert(False, Me.ToString & ".onRefLevelsChanged() Exception: " & ex.Message)
                        End Try
                        'only fire the event once
                        Return
                    End If
                Next
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onRefLevelsChanged() Exception: " & ex.Message)
        End Try

        Try
            If Me.m_dtDataChanged.ContainsKey(msg.Type) Then
                m_dtDataChanged.Item(msg.Type).Invoke()
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onStatsDataChanged() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub fireOnRefLevelsChanged()
        Try
            RaiseEvent onRefLevelsChanged()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onRefLevelsChanged() Exception: " & ex.Message)
        End Try
    End Sub


    Private Sub fireOnRunCompleted()
        Try
            RaiseEvent onRunCompleted()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onStatsDataChanged() Exception: " & ex.Message)
        End Try
    End Sub

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Try

                    For Each del As onChanged In Me.m_dtReflevels.Values
                        del = Nothing
                    Next
                    Me.m_dtReflevels.Clear()

                    For Each del As onChanged In Me.m_dtDataChanged.Values
                        del = Nothing
                    Next
                    Me.m_dtDataChanged.Clear()
                Catch ex As Exception

                End Try

                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
