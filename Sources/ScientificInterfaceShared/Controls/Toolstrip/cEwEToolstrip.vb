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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Utilities
Imports System.ComponentModel

#End Region ' Imports

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Toolstrip that automagically manages the visibile state of its separators.
    ''' </summary>
    ''' ===========================================================================
    Public Class cEwEToolstrip
        Inherits ToolStrip

#Region " Private vars "

        ''' <summary>Update lock flag.</summary>
        Private m_bInUpdate As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Private m_bIsDirty As Boolean = False

        Protected Overrides Sub OnLayoutCompleted(e As EventArgs)
            MyBase.OnLayoutCompleted(e)

            ' Set default display properties
            Me.GripStyle = ToolStripGripStyle.Hidden
            'Me.RenderMode = ToolStripRenderMode.System

            If Not Me.DesignMode And Me.IsHandleCreated Then
                Me.m_bIsDirty = True
                BeginInvoke(New MethodInvoker(AddressOf ShowHideRepeatingSeparators))
            End If

        End Sub

#End Region ' Overrides

#Region " Public bits "

        Public Function Merge(ts As ToolStrip) As Boolean

            If (ts Is Nothing) Then Return False

            Me.SuspendLayout()
            Try
                For i As Integer = ts.Items.Count - 1 To 0 Step -1
                    Me.Items.Insert(0, ts.Items(i))
                Next
                ts.Items.Clear()
            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Me.ResumeLayout()
            Return True

        End Function

#End Region ' Public bits

#Region " Internals "

        ''' <summary>
        ''' Note that this method ONLY works for left-to-right toolstrips
        ''' </summary>
        Private Sub ShowHideRepeatingSeparators()

            If Me.DesignMode Then Return
            If Not Me.m_bIsDirty Then Return

            Dim tsi As ToolStripItem = Nothing
            Dim iNumVisibleControl As Integer = 0 ' Num vis controls since last separator
            Dim iLastVisibleSeparator As Integer = -1 ' Position of last visible separator

            Me.SuspendLayout()

            ' For all toolbar items
            For i As Integer = 0 To Me.Items.Count - 1
                ' Get item
                tsi = Me.Items(i)
                ' Is a separator?
                If (TypeOf tsi Is ToolStripSeparator) Then
                    ' #Yes: show this separator only if it separates visible controls AND controls do not switch left/right alignment
                    If (iNumVisibleControl > 0) Then

                        ' Peek ahead for alignment switch
                        Dim al As Integer = CInt(tsi.Alignment)
                        Dim bShow As Boolean = True

                        For j As Integer = i + 1 To Me.Items.Count - 1
                            Dim tsiTest As ToolStripItem = Me.Items(j)
                            If (Not TypeOf tsiTest Is ToolStripSeparator) And (tsiTest.Visible) Then
                                bShow = (tsiTest.Alignment = al)
                                Exit For
                            End If
                        Next

                        ' Show separator
                        tsi.Visible = bShow
                        If (bShow) Then
                            ' Remember this visible separator
                            iLastVisibleSeparator = i
                        End If
                    Else
                        tsi.Visible = False
                    End If

                    ' Reset visible control count
                    iNumVisibleControl = 0
                Else
                    ' #No: count number of visible regular controls
                    If (tsi.Visible) Then
                        iNumVisibleControl += 1
                        iLastVisibleSeparator = 0
                    End If
                End If
            Next

            ' Fishished without visible controls since the last visible separator?
            If (iNumVisibleControl = 0 And iLastVisibleSeparator >= 0) Then
                ' #Yep: hide the last separator
                Me.Items(iLastVisibleSeparator).Visible = False
            End If

            Me.ResumeLayout()
            Me.m_bIsDirty = False

        End Sub

#End Region ' Internals

    End Class

End Namespace
