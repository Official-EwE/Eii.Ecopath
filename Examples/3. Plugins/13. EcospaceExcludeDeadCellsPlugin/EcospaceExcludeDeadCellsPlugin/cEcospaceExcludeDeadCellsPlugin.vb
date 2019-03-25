' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcospaceExcludeDeadCellsPlugin
    Implements IMenuItemPlugin

    Private m_core As cCore = Nothing

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
            Return My.Resources.PLUGIN_DESCRIPTION
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndEcospaceExcludeDeadCells"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.PLUGIN_DISPLAYTEXT
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return My.Resources.PLUGIN_DESCRIPTION
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE development team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements IGUIPlugin.OnControlClick

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim nrows As Integer = bm.InRow
        Dim ncols As Integer = bm.InCol
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim excl As cEcospaceLayerExclusion = bm.LayerExclusion
        Dim isolatedcells As New List(Of Point)

        ' Make assessment of the entire grid depth cells
        For r As Integer = 1 To nrows
            For c As Integer = 1 To ncols
                If (depth.IsWaterCell(r, c)) Then
                    If (excl.IsIncludedCell(r, c)) Then
                        Dim bIsDead As Boolean = True
                        For dr As Integer = -1 To 1 Step 2
                            If (bm.IsValidCellPosition(r + dr, c)) Then
                                bIsDead = bIsDead And depth.IsLandCell(r + dr, c)
                            End If
                        Next dr
                        For dc As Integer = -1 To 1 Step 2
                            If (bm.IsValidCellPosition(r, c + dc)) Then
                                bIsDead = bIsDead And depth.IsLandCell(r, c + dc)
                            End If
                        Next dc
                        ' Is still dead?
                        If (bIsDead) Then
                            ' #Yes: flag cell. Mind reversal; row = y, col = x
                            isolatedcells.Add(New Point(c, r))
                        End If
                    End If
                End If
            Next c
        Next r

        ' Nothing to do? Abort
        If (isolatedcells.Count = 0) Then Return

        ' Ask user for permission to modify model input data
        Dim msg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.PROMPT_UPDATE, isolatedcells.Count),
                                        eCoreComponentType.EcoSpace, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
        msg.Reply = eMessageReply.NO
        Me.m_core.Messages.SendMessage(msg)

        ' Abort if permission denied
        If (msg.Reply <> eMessageReply.YES) Then Return

        ' Go ahead!
        For Each pt As Point In isolatedcells
            ' Mind reversal! row = y, col = x
            excl.IsExcludedCell(pt.Y, pt.X) = True
        Next
        depth.Invalidate()
        Me.m_core.onChanged(excl)

    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = CType(core, cCore)
    End Sub

End Class
