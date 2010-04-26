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
    ''' <see cref="ucBioPercent">ucBioPercent</see> control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class ucBioPercentToolbar

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

        Public WriteOnly Property IsMenuVisible() As Boolean
            Set(ByVal value As Boolean)
                tsMenus.Visible = value
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

        Private Sub SketchPadWithMenus_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Me.Load

            Me.UpdateControls()

        End Sub

        Private Sub SketchPadWithMenus_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed

            ' Release event hooks
            Me.Handler = Nothing

        End Sub

        Private Sub OnDefineXAxis(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbnDefineXAxis.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DefineXAxis)
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Private m_bInUpdate As Boolean = False

        Private Sub UpdateControls()
            If (Me.Handler Is Nothing) Then Return
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.DefineXAxis, m_tsbnDefineXAxis)
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