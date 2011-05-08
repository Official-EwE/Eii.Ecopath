#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This control class implements a toolbar for controlling a 
    ''' <see cref="ucMediationAssignments"/> control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class ucMediationAssignmentsToolbar

#Region " Variables "

        Private m_handler As cMediationShapeGUIHandler = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As cMediationShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal value As cMediationShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateControls()
            End Set
        End Property

        Public Property IsMenuVisible() As Boolean
            Get
                Return Me.tsMenus.Visible
            End Get
            Set(ByVal value As Boolean)
                Me.tsMenus.Visible = value
            End Set
        End Property

        Public Property DefineMediationLabel() As String
            Get
                Return Me.m_tsbnDefineMediatingItems.Text
            End Get
            Set(ByVal value As String)
                Me.m_tsbnDefineMediatingItems.Text = value
            End Set
        End Property

#End Region ' Properties

#Region " Public interfaces "

        Public Overrides Sub Refresh()
            MyBase.Refresh()
            Me.UpdateControls()
        End Sub

#End Region ' Public interfaces

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_tscmbShowAs.SelectedIndex = 0
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
                ' Release handler
                Me.Handler = Nothing
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub OnDefineXAxis(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnDefineMediatingItems.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DefineMediation)
        End Sub

        Private Sub OnShowAs(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tscmbShowAs.SelectedIndexChanged
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ViewMode, Nothing, Me.m_tscmbShowAs.SelectedIndex)
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Private m_bInUpdate As Boolean = False

        Private Sub UpdateControls()
            If (Me.Handler Is Nothing) Then Return
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.DefineMediation, m_tsbnDefineMediatingItems)
        End Sub

        Private Sub UpdateCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes, ByVal tsi As ToolStripItem)
            If (Me.m_handler Is Nothing) Then Return
            If Me.m_handler.SupportCommand(cmd) Then
                tsi.Visible = True
                tsi.Enabled = (m_handler.EnableCommand(cmd))
            Else
                tsi.Visible = False
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace