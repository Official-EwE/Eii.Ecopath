' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On




Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This control class implements a toolbar for controlling a 
    ''' <see cref="ucMediationAssignments"/> control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class ucMediationAssignmentsToolbar

#Region " Variables "

        Private m_handler As cMediationShapeGUIHandler = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As cMediationShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(value As cMediationShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateControls()
            End Set
        End Property

        Public Property IsMenuVisible() As Boolean
            Get
                Return Me.m_tsMenus.Visible
            End Get
            Set(value As Boolean)
                Me.m_tsMenus.Visible = value
            End Set
        End Property

        Public Property DefineMediationLabel() As String
            Get
                Return Me.m_tsbnDefineMediatingItems.Text
            End Get
            Set(value As String)
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

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_tsbnViewAsPie.Checked = True
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.components.Dispose()
                ' Release handler
                Me.Handler = Nothing
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub OnDefineXAxis(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnDefineMediatingItems.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DefineMediation)
        End Sub

        Private Sub OnViewAsBar(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnViewAsBar.Click
            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ViewMode, Nothing, ucMediationAssignments.eViewModeTypes.Bar)
                Me.m_tsbnViewAsBar.Checked = True
                Me.m_tsbnViewAsPie.Checked = False
            End If
        End Sub

        Private Sub OnViewAsPie(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnViewAsPie.Click
            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ViewMode, Nothing, ucMediationAssignments.eViewModeTypes.Pie)
                Me.m_tsbnViewAsBar.Checked = False
                Me.m_tsbnViewAsPie.Checked = True
            End If
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Private m_bInUpdate As Boolean = False

        Private Sub UpdateControls()

            If (Me.Handler Is Nothing) Then Return

            Dim bShowViewMode As Boolean = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.ViewMode)
            Dim bEnableViewMode As Boolean = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.ViewMode)

            Me.m_tsMenus.SuspendLayout()

            Me.m_tsbnViewAsBar.Visible = bShowViewMode
            Me.m_tsbnViewAsBar.Enabled = bEnableViewMode

            Me.m_tsbnViewAsPie.Visible = bShowViewMode
            Me.m_tsbnViewAsPie.Enabled = bEnableViewMode

            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.DefineMediation, Me.m_tsbnDefineMediatingItems)

            Me.m_tsMenus.ResumeLayout(True)

        End Sub

        Private Sub UpdateCommand(cmd As cShapeGUIHandler.eShapeCommandTypes, tsi As ToolStripItem)
            If (Me.m_handler Is Nothing) Then Return
            If Me.m_handler.SupportCommand(cmd) Then
                tsi.Visible = True
                tsi.Enabled = (Me.m_handler.EnableCommand(cmd))
            Else
                tsi.Visible = False
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace