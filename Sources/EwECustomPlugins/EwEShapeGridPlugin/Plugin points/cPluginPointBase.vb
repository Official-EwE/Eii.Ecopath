#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public MustInherit Class cPluginPointBase
    Implements IUIContextPlugin
    Implements IDisposedPlugin
    Implements INavigationTreeItemPlugin

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmShapes = Nothing
    Private m_core As cCore = Nothing

#Region " Plug-in methods "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IUIContextPlugin.UIContext"/>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(ByVal uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IDisposedPlugin.Dispose"/>
    ''' -----------------------------------------------------------------------
    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose
        If (Me.HasForm) Then
            Me.m_ui.Close()
            Me.m_ui.Dispose()
        End If
        Me.m_ui = Nothing
        Me.m_uic = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        If Not Me.HasForm Then
            Me.m_ui = New frmShapes(Me.GridType)
            Me.m_ui.UIContext = Me.m_uic
            Me.m_ui.Text = Me.ControlText
            AddHandler Me.m_ui.FormClosed, AddressOf OnFormClosed
        End If
        frmPlugin = Me.m_ui
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek, UBC Fisheries Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.ControlText"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlTooltipText As String _
            Implements EwEPlugin.IGUIPlugin.ControlTooltipText

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="INavigationTreeItemPlugin.NavigationTreeItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NavigationTreeItemLocation As String _
            Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation

    Friend MustOverride Function GridType() As Type

#End Region ' Plug-in methods

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Protected Function HasForm() As Boolean
        If Me.m_ui Is Nothing Then Return False
        Return Not Me.m_ui.IsDisposed
    End Function

    Private Sub OnFormClosed(ByVal sender As Object, ByVal arg As EventArgs)
        RemoveHandler Me.m_ui.FormClosed, AddressOf OnFormClosed
        Me.m_ui.Dispose()
        Me.m_ui = Nothing
    End Sub
#End Region ' Internals

End Class
