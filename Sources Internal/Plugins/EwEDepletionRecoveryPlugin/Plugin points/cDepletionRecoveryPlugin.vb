#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands
Imports EwEPlugin
Imports System.Text
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports System.Drawing

#End Region ' Imports

Public Class cDepletionRecoveryPlugin
    Implements IGUIPlugin
    Implements IDockStatePlugin
    Implements IUIContextPlugin
    Implements IMenuItemPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_frmMain As frmMain = Nothing
    Private m_bInitOK As Boolean = False

#End Region ' Private vars

#Region " IPlugin implementation "

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek, Marta Coll"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:j.steenbeek@fisheries.ubc.ca"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Dim sb As New StringBuilder()
            sb.AppendLine("Main interface for Depletion/recovery plug-in.")
            Return sb.ToString()
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
    End Sub

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndDepletionRecovery\ndNode1"
        End Get
    End Property

#End Region ' IPlugin implementation

#Region " IGUIPlugin implementation "

    Public Sub UIContext(ByVal uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
        Me.m_bInitOK = True
    End Sub

    Public ReadOnly Property ControlImage() As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Bitmap.FromHicon(My.Resources.DepletionRecovery.Handle)
        End Get
    End Property

    Public ReadOnly Property ControlText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Depletion/recovery"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return Me.Description
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False

        Debug.Assert(m_bInitOK, "Plugin was not initialized properly.")

        'Show the server interface
        If m_bInitOK Then

            ' Test if form does not yet exist
            If Not Me.HasInterface() Then
                Me.m_frmMain = New frmMain(Me.m_uic)
                Me.m_frmMain.Text = Me.ControlText
            End If

            ' Pass form reference back to calling app
            frmPlugin = Me.m_frmMain
        End If

    End Sub

    Public ReadOnly Property MenuItemLocation() As String _
        Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' IGUIPlugin implementation

#Region " IDockStatePlugin implementation "

    Public Function DockState() As Integer Implements EwEPlugin.IDockStatePlugin.DockState
        Return WeifenLuo.WinFormsUI.Docking.DockState.Document
    End Function

#End Region ' IDockStatePlugin implementation

#Region " Internals "

    Private Function HasInterface() As Boolean
        If Me.m_frmMain Is Nothing Then Return False
        Return Not Me.m_frmMain.IsDisposed
    End Function

#End Region ' Internals

End Class
