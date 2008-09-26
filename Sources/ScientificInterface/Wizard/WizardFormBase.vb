'==============================================================================
'
' $Log: WizardFormBase.vb,v $
' Revision 1.1  2008/09/26 07:32:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2007/10/08 02:11:37  jeroens
' * Restyled
'
' Revision 1.5  2007/06/04 02:55:48  jeroens
' * Cancel button by default disabled on last page
'
' Revision 1.4  2007/05/09 04:13:51  jeroens
' - Simplified
' + Added wizard button state control
'
'==============================================================================

Option Strict On

Namespace Wizard

    ''' <summary>
    ''' Base class that implements the minimum Wizard Form behavior
    ''' </summary>
    Public Class WizardFormBase

        Private m_iCurrentPage As Integer
        Private m_alControlsInPage As ArrayList

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()
        End Sub

#Region " Page management "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the current page in the wizard
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CurrentPage() As Integer
            Get
                Return m_iCurrentPage
            End Get
            Set(ByVal iPageIndex As Integer)

                ' Set current page
                m_iCurrentPage = iPageIndex

                ' Update wizard
                If m_iCurrentPage >= 0 Then
                    ' Set tab text
                    Text = tcMain.TabPages(iPageIndex).Text

                    ' Set visibility state of controls for each page
                    Dim iPage As Integer = 0
                    For Each oControls As ArrayList In m_alControlsInPage
                        For Each oControl As Control In oControls
                            oControl.Parent = Me
                            If iPage = iPageIndex Then
                                oControl.Show()
                            Else
                                oControl.Hide()
                            End If
                        Next
                        iPage += 1
                    Next

                    ActivatePage(iPageIndex)
                    UpdateWizardButtons()
                End If

            End Set
        End Property

        Protected Overridable Sub ActivatePage(ByVal iPageIndex As Integer)

        End Sub

#End Region ' Page management

#Region " Wizard buttons updating "

        Protected Sub UpdateWizardButtons()

            Me.btnBack.Enabled = OnUpdateBackButton()
            Me.btnNext.Enabled = OnUpdateNextButton()
            Me.btnCancel.Enabled = OnUpdateCancelButton()
            Me.btnFinish.Enabled = OnUpdateFinishButton()

            Me.Refresh()

        End Sub

        Protected Overridable Function OnUpdateBackButton() As Boolean
            ' Default implementation: cannot go back past first page (duh)
            Return (Me.CurrentPage > 0)
        End Function

        Protected Overridable Function OnUpdateNextButton() As Boolean
            ' Default implementation: cannot step past last page (oh wow)
            Return (Me.CurrentPage < Me.tcMain.TabCount - 1)
        End Function

        Protected Overridable Function OnUpdateCancelButton() As Boolean
            ' Default implementation: cannot cancel on last page
            Return (Me.CurrentPage < Me.tcMain.TabCount - 1)
        End Function

        Protected Overridable Function OnUpdateFinishButton() As Boolean
            ' Default implementation: can always finish
            Return True
        End Function

#End Region ' Wizard buttons updating

#Region " Events "

        Private Sub WizardFormBase_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If tcMain.TabCount > 0 Then
                InitializePages()
            End If
        End Sub

        Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
            If ValidatePage(m_iCurrentPage) Then
                OnPageIndexChanged(ForwardOffset(m_iCurrentPage))
            End If
        End Sub

        Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
            OnPageIndexChanged(PreviousOffset(m_iCurrentPage))
        End Sub

        Private Sub btnFinish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFinish.Click
            DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Public Overloads Function ShowDialog(ByVal poOwner As System.Windows.Forms.IWin32Window, ByVal iPageIndex As Integer) As DialogResult
            m_iCurrentPage = iPageIndex
            Return MyBase.ShowDialog(poOwner)
        End Function

#End Region ' Events

#Region " Internal admin "

        Protected Overridable Sub OnPageIndexChanged(ByVal iPageIndex As Integer)
            Me.CurrentPage = iPageIndex
        End Sub

        Protected Sub InitializePages()

            If (tcMain.TabCount > 0) Then

                tcMain.Parent = Nothing

                tcMain.Scale(New SizeF(1, CSng((tcMain.ClientSize.Height + tcMain.GetTabRect(0).Height)) / tcMain.ClientSize.Height))

                m_alControlsInPage = New ArrayList(tcMain.TabPages.Count)

                For Each oTabPage As TabPage In tcMain.TabPages
                    Dim oControls As ArrayList = New ArrayList(oTabPage.Controls.Count)
                    For Each oControl As Control In oTabPage.Controls
                        oControls.Add(oControl)
                    Next
                    m_alControlsInPage.Add(oControls)
                Next
                OnPageIndexChanged(CurrentPage)
            End If

        End Sub

        Public Overridable Function ForwardOffset(ByVal iPageIndex As Integer) As Integer
            Return iPageIndex + 1
        End Function

        Public Overridable Function PreviousOffset(ByVal iPageIndex As Integer) As Integer
            Return iPageIndex - 1
        End Function

        Protected Overridable Function ValidatePage(ByVal iPageIndex As Integer) As Boolean
            Return True
        End Function

#End Region ' Internal admin

    End Class
End Namespace
