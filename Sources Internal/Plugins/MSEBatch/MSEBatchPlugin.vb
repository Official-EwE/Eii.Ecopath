
Imports EwECore.MSEBatchManager
Imports EwEPlugin
Imports EwEUtils.Core

Public Class MSEBatchPlugin
    Implements IMSEBatch
    Implements IGUIPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.Data.IDatabasePlugin

    Private m_manager As EwECore.MSEBatchManager.cMSEBatchManager
    Private m_frmBatch As frmMSEBatch

    Public Sub MSEBatchInitialized(ByVal MSEBatchManager As Object, ByVal MSEBatchManagerDataStrucures As Object) Implements EwEPlugin.IMSEBatch.MSEBatchInitialized
        m_manager = DirectCast(MSEBatchManager, EwECore.MSEBatchManager.cMSEBatchManager)
    End Sub

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Institute for the Oceans and Fisheries, Institute for Ocean Conservation Science, School of Marine and Atmospheric Sciences, Stony Brook University"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Run the MSE module in batch mode from a command file."
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            'the "z" is so the plugin node sorts to the bottom of the branch
            Return "zMSE Batch"
        End Get
    End Property

    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "MSE Batch"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "MSE Batch"
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        Try

            If Not Me.HasInterface Then
                Me.m_frmBatch = New frmMSEBatch(Me.m_manager)
            End If
            frmPlugin = Me.m_frmBatch

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".OnControlClick() Exception: " & ex.Message)
        End Try

    End Sub

    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTools|ndMSE|ndMSEOutput"
        End Get
    End Property

    Public Sub Close() Implements EwEPlugin.Data.IDatabasePlugin.Close
        If Me.HasInterface Then
            Me.m_frmBatch.Close()
            Me.m_frmBatch.Dispose()
        End If
        Me.m_frmBatch = Nothing
    End Sub

    Private Function HasInterface() As Boolean
        If Me.m_frmBatch Is Nothing Then Return False
        If Me.m_frmBatch.IsDisposed Then Return False
        Return True
    End Function


    Public Function IsModified() As Boolean Implements EwEPlugin.Data.IDatabasePlugin.IsModified
        'not interested
    End Function

    Public Function Open(ByVal strName As String) As Boolean Implements EwEPlugin.Data.IDatabasePlugin.Open
        'not interested
    End Function
End Class
