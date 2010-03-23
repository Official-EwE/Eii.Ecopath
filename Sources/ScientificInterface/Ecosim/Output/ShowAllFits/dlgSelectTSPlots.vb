#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog class, allows users to configure which plots to show in the Ecosim
    ''' Show All Fits interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgSelectAllFitsPlots

        Private m_lplots As List(Of cShowAllFitsPlotData)

        Public Sub New(ByVal lplots As List(Of cShowAllFitsPlotData))

            InitializeComponent()
            Me.m_lplots = lplots

        End Sub

        Private Sub SelectTSPlots_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim plot As cShowAllFitsPlotData = Nothing
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

            Dim plot As cShowAllFitsPlotData = Nothing

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
