'==============================================================================
'
' $Log: dlgSelectTSPlots.vb,v $
' Revision 1.2  2008/12/15 15:54:30  jeroens
' no message
'
' Revision 1.1  2008/02/12 23:06:56  jeroens
' Revised and debugged
'
' Revision 1.4  2007/09/24 17:57:54  sherman
' Added header log
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    Public Class dlgSelectAllFitsPlots

        Private m_lplots As List(Of ShowAllFitsPlotData)

        Public Sub New(ByVal lplots As List(Of ShowAllFitsPlotData))

            InitializeComponent()
            Me.m_lplots = lplots

        End Sub

        Private Sub SelectTSPlots_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim plot As ShowAllFitsPlotData = Nothing
            Dim ts As cTimeSeries = Nothing

            clbAllPlots.Items.Clear()

            For i As Integer = 0 To Me.m_lplots.Count - 1
                plot = Me.m_lplots(i)
                ts = plot.TimeSeries
                clbAllPlots.Items.Add(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, ts.Index, ts.Name), _
                    plot.Selected)
            Next

        End Sub

        Private Sub btnCheckAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCheckAll.Click

            For i As Integer = 0 To clbAllPlots.Items.Count - 1
                clbAllPlots.SetItemChecked(i, True)
            Next

        End Sub

        Private Sub btnUnCheckAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnCheckAll.Click

            For i As Integer = 0 To clbAllPlots.Items.Count - 1
                clbAllPlots.SetItemChecked(i, False)
            Next

        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            Dim plot As ShowAllFitsPlotData = Nothing

            For i As Integer = 0 To Me.m_lplots.Count - 1
                plot = Me.m_lplots(i)
                plot.Selected = Me.clbAllPlots.GetItemChecked(i)
            Next

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

    End Class

End Namespace
