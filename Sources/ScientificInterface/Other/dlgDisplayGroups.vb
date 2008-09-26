'==============================================================================
'
' $Log: dlgDisplayGroups.vb,v $
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

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    Public Class dlgDisplayGroups

        Private m_Core As cCore

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Me.m_Core = cCore.GetInstance()

        End Sub

        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim group As cEcoPathGroupInput = Nothing

            Me.m_clbGroups.Items.Clear()
            For iGroup As Integer = 1 To Me.m_Core.nGroups

                group = m_Core.EcoPathGroupInputs(iGroup)
                Me.m_clbGroups.Items.Add(group.Name, appl.GroupDisplayFlags(iGroup))
            Next

            Me.m_clbGroups.Items.Add(My.Resources.HEADER_TOTALCATCH, appl.GroupDisplayFlags(Me.m_Core.nGroups + 1))
            Me.m_clbGroups.Items.Add(My.Resources.HEADER_TOTALLENGTH, appl.GroupDisplayFlags(Me.m_Core.nGroups + 2))

        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            ' Display flag array is 1-based, lb items are 0-based. Allocate 1 value more
            Dim abFlags(Me.m_clbGroups.Items.Count) As Boolean
            Dim appl As AppLauncher = AppLauncher.GetInstance()

            ' Copy item states into the array of flags to pass to AppLauncher
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                abFlags(iItem + 1) = Me.m_clbGroups.GetItemChecked(iItem)
            Next
            ' There!
            appl.GroupDisplayFlags = abFlags

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
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 3
                Me.m_clbGroups.SetItemChecked(iItem, True)
            Next
            For iItem As Integer = Me.m_clbGroups.Items.Count - 2 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, False)
            Next
            Me.m_clbGroups.ResumeLayout()
        End Sub
    End Class

End Namespace
