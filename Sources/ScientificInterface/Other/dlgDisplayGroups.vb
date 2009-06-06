'==============================================================================
'
' $Log: dlgDisplayGroups.vb,v $
' Revision 1.5  2009/06/06 01:45:18  jeroens
' Added option to suppress groups, total options
'
' Revision 1.4  2009/05/28 12:37:18  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.3  2008/12/15 15:54:30  jeroens
' no message
'
' Revision 1.2  2008/11/27 03:10:42  jeroens
' Group visible flags maintained by style guide, no longer by AppLauncher
'
' Revision 1.1  2008/09/26 07:32:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/04/07 02:31:13  jeroens
' Cleaning up resources
'
' Revision 1.1  2008/02/13 16:43:10  jeroens
' Moved, made generic to app
'
' Revision 1.5  2007/08/07 03:00:39  jeroens
' + Added header
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    Public Class dlgDisplayGroups

        Private m_core As cCore = Nothing
        Private m_sg As cStyleGuide = Nothing
        Private m_bShowGroups As Boolean = True
        Private m_bShowTotals As Boolean = False

        Public Sub New(ByVal bShowGroups As Boolean, ByVal bShowTotals As Boolean)

            Me.InitializeComponent()
            Me.m_core = cCore.GetInstance()
            Me.m_sg = cStyleGuide.GetInstance()
            Me.m_bShowGroups = bShowGroups
            Me.m_bShowTotals = bShowTotals

        End Sub

        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim group As cEcoPathGroupInput = Nothing

            Me.m_clbGroups.Items.Clear()

            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    group = m_core.EcoPathGroupInputs(iGroup)
                    Me.m_clbGroups.Items.Add(group.Name, Me.m_sg.GroupVisible(iGroup))
                Next
            End If

            If Me.m_bShowTotals Then
                Me.m_clbGroups.Items.Add(My.Resources.HEADER_TOTALCATCH, Me.m_sg.TotalCatchVisible)
                Me.m_clbGroups.Items.Add(My.Resources.HEADER_TOTALLENGTH, Me.m_sg.TotalValueVisible)
            End If

        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            Dim iIndex As Integer = 0

            Me.m_sg.SuspendEvents()

            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    Me.m_sg.GroupVisible(iGroup) = Me.m_clbGroups.GetItemChecked(iGroup - 1)
                Next
                iIndex += Me.m_core.nGroups
            End If

            If Me.m_bShowTotals Then
                Me.m_sg.TotalCatchVisible = Me.m_clbGroups.GetItemChecked(iIndex)
                Me.m_sg.TotalValueVisible = Me.m_clbGroups.GetItemChecked(iIndex + 1)
            End If

            Me.m_sg.ResumeEvents()

            ' And done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub m_btnAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnAll.Click
            ' Check all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, True)
            Next
            Me.m_clbGroups.ResumeLayout()
        End Sub

        Private Sub m_btnNone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnNone.Click
            ' Uncheck all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, False)
            Next
            Me.m_clbGroups.ResumeLayout()
        End Sub

        Private Sub m_btnDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnDefault.Click
            ' Check all groups, uncheck summary items
            Me.m_clbGroups.SuspendLayout()

            Dim iIndex As Integer = 0

            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    Me.m_clbGroups.SetItemChecked(iGroup - 1, True)
                Next
                iIndex = Me.m_core.nGroups
            End If

            If Me.m_bShowTotals Then
                For iItem As Integer = iIndex To Me.m_clbGroups.Items.Count - 1
                    Me.m_clbGroups.SetItemChecked(iItem, False)
                Next
            End If

            Me.m_clbGroups.ResumeLayout()
        End Sub
    End Class

End Namespace
