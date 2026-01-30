' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog class, allows users to configure which plots to show in the Ecosim
    ''' Show All Fits interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgSelectAllFitsPlots

        Private m_lplots As cShowAllFitsPlotData()

        Public Sub New(lplots() As cShowAllFitsPlotData)

            Me.InitializeComponent()
            Me.m_lplots = lplots

        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim plot As cShowAllFitsPlotData = Nothing
            Dim ts As cTimeSeries = Nothing

            Me.clbAllPlots.Items.Clear()

            For i As Integer = 0 To Me.m_lplots.Count - 1
                plot = Me.m_lplots(i)
                Me.clbAllPlots.Items.Add(plot, plot.Selected)
            Next

        End Sub

        Private Sub btnCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles btnCheckAll.Click

            For i As Integer = 0 To Me.clbAllPlots.Items.Count - 1
                Me.clbAllPlots.SetItemChecked(i, True)
            Next

        End Sub

        Private Sub btnUnCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles btnUnCheckAll.Click

            For i As Integer = 0 To Me.clbAllPlots.Items.Count - 1
                Me.clbAllPlots.SetItemChecked(i, False)
            Next

        End Sub

        Private Sub OK_Button_Click(sender As System.Object, e As System.EventArgs) Handles OK_Button.Click

            Dim plot As cShowAllFitsPlotData = Nothing

            For i As Integer = 0 To Me.m_lplots.Count - 1
                plot = Me.m_lplots(i)
                plot.Selected = Me.clbAllPlots.GetItemChecked(i)
            Next

            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(sender As System.Object, e As System.EventArgs) Handles Cancel_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub clbAllPlots_Format(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles clbAllPlots.Format

            If (e.ListItem IsNot Nothing) Then
                Dim plot As cShowAllFitsPlotData = DirectCast(e.ListItem, cShowAllFitsPlotData)
                Dim ts As cTimeSeries = plot.TimeSeries
                Debug.Assert(ts IsNot Nothing)
                e.Value = String.Format(SharedResources.GENERIC_LABEL_INDEXED, ts.Index, ts.Name)
            End If
        End Sub
    End Class

End Namespace
