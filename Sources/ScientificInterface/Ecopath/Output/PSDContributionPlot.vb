#Region " Imports "
Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ZedGraph

#End Region ' Imports

Namespace Ecopath.Output

    Public Class PSDContributionPlot

#Region " Variables "

        Private m_core As cCore = Nothing
        Private m_zgh As cZedGraphHelper = Nothing

#End Region ' Variables

#Region " Constructor "
        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.m_core, Me.zgcZedGraphCntl)

            'Don't manually run! The core execution states take care of this!
            'm_core.RunPSD(IsGroupSelected)
        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Me.m_lbGroups.Populate()
            m_lbGroups.SelectedIndex = 0

        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_lbGroups.SelectedIndexChanged
            AddCurves(CreatePane(My.Resources.PSD_PLOTCAPTION_PSDCONTRIB, My.Resources.PSD_XAXISLABEL_BODYWEIGHT, _
                     My.Resources.PSD_YAXISLABEL_BIOMASS))

            'highlight group contribution in the histogram
            UpdatePlot()
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Event handlers

#Region " Helper methods "

        Private Function CreatePane(ByVal strTitle As String, ByVal strXAxisTitle As String, _
                                    ByVal strYAxisTitle As String) As GraphPane
            Dim pane As GraphPane = Me.zgcZedGraphCntl.GraphPane

            InitGraphPane(strTitle, strXAxisTitle, strYAxisTitle)
            Return pane
        End Function

        Private Sub InitGraphPane(ByVal strTitle As String, ByVal strXAxisTitle As String, ByVal strYAxisTitle As String)

            Dim psd As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim gp As GraphPane = Me.m_zgh.ConfigurePane(strTitle, strXAxisTitle, strYAxisTitle, False)

            gp.XAxis.Scale.Min = Int(Math.Log10(psd.FirstWeightClass))
            gp.XAxis.Scale.Max = Math.Round(Math.Log10(psd.FirstWeightClass * 2 ^ (m_core.nWeightClasses - 1)) + 0.4, 0, MidpointRounding.AwayFromZero)
            gp.YAxis.Scale.Min = 0

        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)

            Dim psd As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing
            Dim sSystemPSD(m_core.nWeightClasses) As Single
            Dim sgStyleGuide As cStyleGuide = cStyleGuide.GetInstance
            Dim curveSelected As BarItem = Nothing
            Dim iSelectedGrpNum As Integer = 1

            InitLists(resultLists, m_core.nLivingGroups) '3)

            'Find the system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            For igroup As Integer = 1 To m_core.nLivingGroups
                'No need to check if group is selected. Generate the result list even for the not selected group. It will have zero Y values
                'If IsGroupSelected(igroup) Then
                grpOutput = m_core.EcoPathGroupOutputs(igroup)
                For iWtClass As Integer = 1 To m_core.nWeightClasses
                    sXValue = CSng(psd.FirstWeightClass * 2 ^ (iWtClass - 1))
                    If sSystemPSD(iWtClass) * 1000000000 > 0 Then
                        'group contribution to the system PSD is Math.Log10(sSystemPSD(iWtClass) * 1000000000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass)
                        '* 1000000000 for plotting purpose
                        resultLists(igroup - 1).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 1000000000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass))
                    Else
                        resultLists(igroup - 1).Add(Math.Log10(sXValue), 0)
                    End If
                Next
                'End If
            Next

            ' Clear pane
            pane.CurveList.Clear()

            'Find the selected group number based on the selected index
            For iGroup As Integer = 1 To m_core.nLivingGroups
                If m_core.EcoPathGroupOutputs(iGroup).Name = m_lbGroups.Items(m_lbGroups.SelectedIndex).ToString() Then
                    iSelectedGrpNum = iGroup
                    Exit For
                End If
            Next

            For iGroup As Integer = 1 To m_core.nLivingGroups
                If iGroup = iSelectedGrpNum Then
                    curveSelected = AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), sgStyleGuide.GroupColor(Me.m_core, iGroup - 1), Color.Gray)
                Else
                    AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), sgStyleGuide.GroupColor(Me.m_core, iGroup - 1), Color.Gray)
                End If
            Next

            curveSelected.Bar.Border = New Border(Color.Black, 2)
            pane.BarSettings.Type = BarType.Stack
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Function AddCurveToGraphPane(ByVal pane As GraphPane, ByVal legend As String, ByVal list As PointPairList, _
                                        ByVal clrFill As Color, ByVal clrBorder As Color) As BarItem
            Dim brItem As BarItem

            brItem = pane.AddBar(legend, list, clrFill)
            brItem.Bar.Fill = New Fill(clrFill)
            brItem.Bar.Border = New Border(clrBorder, 2)

            Return brItem
        End Function

        Private Sub UpdatePlot()
            Me.zgcZedGraphCntl.AxisChange()
            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                If Me.m_lbGroups.GroupIndex(iGroup) > -1 Then
                    grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To m_core.nWeightClasses
                        sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                    Next
                End If
            Next
        End Sub

#End Region ' Helper methods

    End Class

End Namespace