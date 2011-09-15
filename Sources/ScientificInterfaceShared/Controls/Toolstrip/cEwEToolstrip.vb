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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Layout handler, overridden to update the state of separators.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnLayout(ByVal e As LayoutEventArgs)

            ' Already updating? Abort
            If Me.m_bInUpdate Then Return

            ' Set lock
            Me.m_bInUpdate = True
            ' Update separators
            Me.ShowHideRepeatingSeparators()
            ' Set default display properties
            Me.GripStyle = ToolStripGripStyle.Hidden
            Me.RenderMode = ToolStripRenderMode.System
            ' Do base class thing
            MyBase.OnLayout(e)

            ' Release lock
            Me.m_bInUpdate = False

        End Sub

#End Region ' Overrides

#Region " Internals "

        Private Sub ShowHideRepeatingSeparators()

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
                    ' #Yes: show this separator only if it separates visible controls
                    If (iNumVisibleControl > 0) Then
                        ' Show separator
                        tsi.Visible = True
                        ' Remember this visible separator
                        iLastVisibleSeparator = i
                    Else
                        tsi.Visible = False
                    End If

                    ' Reset visible control count
                    iNumVisibleControl = 0
                Else
                    ' #No: count number of visible regular controls
                    If (tsi.Visible) Then iNumVisibleControl += 1
                End If
            Next

            ' Fishished without visible controls since the last visible separator?
            If (iNumVisibleControl = 0 And iLastVisibleSeparator >= 0) Then
                ' #Yep: hide the last separator
                Me.Items(iLastVisibleSeparator).Visible = False
            End If

            Me.ResumeLayout()

        End Sub

#End Region ' Internals

    End Class

End Namespace
