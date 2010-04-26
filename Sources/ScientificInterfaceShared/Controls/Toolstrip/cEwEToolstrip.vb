#Region " Imports "

Option Strict On
Imports EwEUtils.Utilities

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
            ' Do base class thing
            MyBase.OnLayout(e)

            ' Release lock
            Me.m_bInUpdate = False

        End Sub

#End Region ' Overrides

#Region " Internals "

        Private Sub ShowHideRepeatingSeparators()

            Dim tsi As ToolStripItem = Nothing
            Dim bAllInvisibleControls As Boolean = True

            Me.SuspendLayout()
            ' For all toolbar items
            For i As Integer = 0 To Me.Items.Count - 1
                ' Get item
                tsi = Me.Items(i)
                ' Is a separator?
                If (TypeOf tsi Is ToolStripSeparator) Then
                    ' #Yes: ok, show it
                    tsi.Visible = (bAllInvisibleControls = False)
                    bAllInvisibleControls = True
                Else
                    ' #No: regular control. Is this control visible?
                    If tsi.Visible = True Then
                        ' #Yes: forget last separator since it separates a visible control
                        bAllInvisibleControls = False
                    End If
                End If
            Next
            Me.ResumeLayout()

        End Sub

#End Region ' Internals

    End Class

End Namespace
