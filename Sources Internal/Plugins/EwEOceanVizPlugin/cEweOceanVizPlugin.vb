#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cEwEOceanVizPlugin
    Implements IMenuItemPlugin
    Implements IUIContextPlugin

    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return "OceanViz"
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndOceanViz"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "OceanViz"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' nop
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Private m_ui As frmOceanViz = Nothing

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return Not Me.m_ui.IsDisposed
    End Function

    Private Function GetUI() As frmOceanViz
        If Not Me.HasUI Then
            Me.m_ui = New frmOceanViz()
            Me.m_ui.UIContext = Me.m_uic
        End If
        Return Me.m_ui
    End Function


End Class
