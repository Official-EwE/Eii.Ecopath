#Region " Imports "

Option Strict On
Imports System.Windows.Forms

#End Region ' Imports

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Tab control behaving like a wizard, hiding its tab buttons.
    ''' </summary>
    ''' <remarks>
    ''' <para>The initial version of this class assumes that tab pages will not be
    ''' added and removed once the control is deployed. It might work; these 
    ''' dynamics simply have not been tried out.</para>
    ''' </remarks>
    ''' ===========================================================================
    Public Class ucWizardTabControl
        Inherits TabControl

#Region " Privates "

#Region " Helper class "

        ''' =======================================================================
        ''' <summary>
        ''' Helper class for sorting a collection of controls by tab index.
        ''' </summary>
        ''' =======================================================================
        Private Class cControlComparer
            Implements IComparer(Of Control)

            Public Function Compare(ByVal x As Control, ByVal y As Control) As Integer _
                Implements System.Collections.Generic.IComparer(Of Control).Compare
                Return x.TabIndex.CompareTo(y.TabIndex)
            End Function

        End Class

#End Region ' Helper class

        ''' <summary>List of currently visible controls</summary>
        Private m_lControls As New List(Of Control)
        ''' <summary>Current tab page that is 'visible'</summary>
        Private m_iCurrentPage As Integer = -1
        ''' <summary>The former parent of the tab control at creation time</summary>
        Private m_custodian As Control = Nothing
        ''' <summary>Scale used to resize the tab page area.</summary>
        Private m_szScale As SizeF = Nothing

#End Region ' Privates

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a new ucWizardTabControl
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Disposal - overridden to restore the intial parental arrangement and
        ''' control size. The designer is grateful for this, and looks much better 
        ''' in the will.
        ''' </summary>
        ''' <param name="bDisposing">*agk*</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)

            ' Restore parent, control scale
            If (Me.m_custodian IsNot Nothing) Then
                Me.Scale(New SizeF(1 / Me.m_szScale.Width, 1 / Me.m_szScale.Height))
                Me.Parent = Me.m_custodian
                Me.m_custodian = Nothing
            End If

            MyBase.Dispose(bDisposing)

        End Sub

#End Region ' Construction / destruction 

#Region " Overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' .NET tab control has been created - engage wizard mode.
        ''' </summary>
        ''' <param name="e"></param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnHandleCreated(ByVal e As System.EventArgs)

            MyBase.OnHandleCreated(e)

            ' Only engage wizard mode in run time mode
            If (Me.DesignMode = False) Then

                ' Faintly remember parent before breaking all ties
                Me.m_custodian = Me.Parent
                ' Detach from original parent to hide the tab buttons and the tab control.
                ' This unfortunately also hides the pages and their content. This,
                ' however, is dealt with in SyncWizardPage
                Me.Parent = Nothing

                ' Scale up to let content occupy full tab control window area
                Select Case Me.Alignment

                    Case TabAlignment.Top, TabAlignment.Bottom
                        Dim iHeight As Integer = Me.ClientSize.Height
                        ' Prevent div by 0
                        If iHeight = 0 Then iHeight = 1
                        Me.m_szScale = New SizeF(1, CSng(iHeight + Me.GetTabRect(0).Height) / iHeight)

                    Case TabAlignment.Left, TabAlignment.Right
                        Dim iWidth As Integer = Me.ClientSize.Width
                        ' Prevent div by 0
                        If iWidth = 0 Then iWidth = 1
                        Me.m_szScale = New SizeF(CSng((iWidth + Me.GetTabRect(0).Width) / iWidth), 1)

                End Select

                ' Rescale control to take advantage of full client area now that the 
                ' tab page bar is no longer visible.
                Me.Scale(Me.m_szScale)

                ' Show first page
                If Me.TabCount > 0 Then
                    ' Events will not be pumping yet
                    Me.SelectedIndex = 0
                    ' Switch to first page
                    Me.SyncWizardPage()
                End If

            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Selected tab index has changed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As System.EventArgs)

            MyBase.OnSelectedIndexChanged(e)

            If (Me.DesignMode = False) Then
                ' Do the wizard thing
                Me.SyncWizardPage()
            End If

        End Sub

#End Region ' Overrides

#Region " Implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Synchronize the visibility of pages with the tab control selected tab
        ''' index.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub SyncWizardPage()

            ' Sanity check
            Debug.Assert(Me.DesignMode = False, "Can only invoke this method in runtime mode")

            ' Optimizations
            If (Me.m_iCurrentPage = Me.SelectedIndex) Then Return

            ' Clear up previous visible page
            If (Me.m_iCurrentPage <> -1) Then
                Me.ShowPageControls(Me.TabPages(Me.m_iCurrentPage), False)
            End If

            ' Update to new page
            Me.m_iCurrentPage = Me.SelectedIndex

            ' Show new visible page
            If (Me.m_iCurrentPage <> -1) Then
                Me.ShowPageControls(Me.TabPages(Me.m_iCurrentPage), True)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Show the controls in a page in wizard mode: the tab control and pages
        ''' are not visible. Instead, controls are manually transferred back and 
        ''' forth between placeholder tab pages and our 'custodian'.
        ''' </summary>
        ''' <param name="page">The page to show or hide.</param>
        ''' <param name="bShow">Flag stating whether the page is shown or hidden.</param>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub ShowPageControls(ByVal page As TabPage, ByVal bShow As Boolean)

            ' Sanity check
            If (Me.DesignMode = True) Then Return

            ' Hold layout while transferring controls
            Me.m_custodian.SuspendLayout()

            If bShow Then
                ' Transfer content of page to m_lControls
                For i As Integer = 0 To page.Controls.Count - 1
                    Me.m_lControls.Add(page.Controls(i))
                Next
                Me.m_lControls.Sort(New cControlComparer())

                For i As Integer = 0 To Me.m_lControls.Count - 1
                    Me.m_lControls(i).Parent = Me.m_custodian
                    Me.m_lControls(i).Show()
                Next i
            Else
                ' Transfer content of m_lControls to page
                For i As Integer = 0 To Me.m_lControls.Count - 1
                    Me.m_lControls(i).Parent = page
                    Me.m_lControls(i).Hide()
                Next i
                Me.m_lControls.Clear()

            End If

            ' Resume layout after transferring controls
            Me.m_custodian.ResumeLayout()

        End Sub

#End Region ' Implementation

    End Class

End Namespace
