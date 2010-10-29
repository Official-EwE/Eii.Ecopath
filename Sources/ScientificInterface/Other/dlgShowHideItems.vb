#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog class, implements the generic show/hide items interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgShowHideItems

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_bShowGroups As Boolean = True
        Private m_bShowTotals As Boolean = False

#End Region ' Private variables

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new dialog.
        ''' </summary>
        ''' <param name="uic">The UI context to connect to.</param>
        ''' <param name="bShowGroups">Flag stating whether all groups should be shown.</param>
        ''' <param name="bShowTotals">Flag stating whether totals should be shown.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal bShowGroups As Boolean, _
                       ByVal bShowTotals As Boolean)

            Me.InitializeComponent()

            ' Sanity check
            Debug.Assert(uic IsNot Nothing)

            Me.m_uic = uic
            Me.m_bShowGroups = bShowGroups
            Me.m_bShowTotals = bShowTotals

        End Sub

#End Region ' Constructor

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim group As cEcoPathGroupInput = Nothing
            Dim fleet As cFleetInput = Nothing

            Me.m_clbGroups.Items.Clear()
            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                    group = Me.m_uic.Core.EcoPathGroupInputs(iGroup)
                    Me.m_clbGroups.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, iGroup, group.Name), _
                                             Me.m_uic.StyleGuide.GroupVisible(iGroup))
                Next
            End If

            If Me.m_bShowTotals Then
                Me.m_clbGroups.Items.Add(SharedResources.HEADER_TOTALCATCH, Me.m_uic.StyleGuide.TotalCatchVisible)
                Me.m_clbGroups.Items.Add(SharedResources.HEADER_TOTALLENGTH, Me.m_uic.StyleGuide.TotalValueVisible)
            End If

            Me.m_clbFleets.Items.Clear()
            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                fleet = Me.m_uic.Core.FleetInputs(iFleet)
                Me.m_clbFleets.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, iFleet, fleet.Name), _
                                         Me.m_uic.StyleGuide.FleetVisible(iFleet))
            Next

        End Sub

#End Region ' Form overrides

#Region " Events "

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            Dim iIndex As Integer = 0

            Me.m_uic.StyleGuide.SuspendEvents()

            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                    Me.m_uic.StyleGuide.GroupVisible(iGroup) = Me.m_clbGroups.GetItemChecked(iGroup - 1)
                Next
                iIndex += Me.m_uic.Core.nGroups
            End If

            If Me.m_bShowTotals Then
                Me.m_uic.StyleGuide.TotalCatchVisible = Me.m_clbGroups.GetItemChecked(iIndex)
                Me.m_uic.StyleGuide.TotalValueVisible = Me.m_clbGroups.GetItemChecked(iIndex + 1)
            End If

            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                Me.m_uic.StyleGuide.FleetVisible(iFleet) = Me.m_clbFleets.GetItemChecked(iFleet - 1)
            Next

            Me.m_uic.StyleGuide.ResumeEvents()
            Me.m_uic.StyleGuide.ItemVisibilityChanged()

            ' And done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnSelectAllGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAllGroups.Click

            ' Check all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, True)
            Next
            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Sub OnSelectNoneGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNoneGroups.Click

            ' Uncheck all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, False)
            Next
            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Sub OnSelectDefaultGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnDefaultGroups.Click

            ' Check all groups, uncheck summary items
            Me.m_clbGroups.SuspendLayout()

            Dim iIndex As Integer = 0

            If Me.m_bShowGroups Then
                For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                    Me.m_clbGroups.SetItemChecked(iGroup - 1, True)
                Next
                iIndex = Me.m_uic.Core.nGroups
            End If

            If Me.m_bShowTotals Then
                For iItem As Integer = iIndex To Me.m_clbGroups.Items.Count - 1
                    Me.m_clbGroups.SetItemChecked(iItem, False)
                Next
            End If

            Me.m_clbGroups.ResumeLayout()

        End Sub


        Private Sub OnSelectAllFleets(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAllFleets.Click

            ' Check all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, True)
            Next
            Me.m_clbFleets.ResumeLayout()

        End Sub

        Private Sub OnSelectNoneFleets(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNoneFleets.Click

            ' Uncheck all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, False)
            Next
            Me.m_clbFleets.ResumeLayout()

        End Sub

#End Region ' Events

    End Class

End Namespace
