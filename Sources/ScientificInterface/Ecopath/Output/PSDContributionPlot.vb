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

        Private m_zgh As cZedGraphHelper = Nothing

#End Region ' Variables

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Debug.Assert(Me.UIContext IsNot Nothing)

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.zgcZedGraphCntl)

            Me.m_lbGroups.Attach(Me.UIContext)
            Me.m_lbGroups.SelectedIndex = 0

        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_lbGroups.SelectedIndexChanged
            AddCurves(CreatePane(My.Resources.CAPTION_PSD_GROUP_CONTRIB, My.Resources.HEADER_BODYWEIGHT_LOGg, _
                     My.Resources.HEADER_BIOMASS_LOGg))

            'highlight group contribution in the histogram
            UpdatePlot()
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
            Me.m_lbGroups.Detach()
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

            Dim psd As cPSDParameters = Me.Core.ParticleSizeDistributionParameters
            Dim gp As GraphPane = Me.m_zgh.ConfigurePane(strTitle, strXAxisTitle, strYAxisTitle, False)

            gp.XAxis.Scale.Min = Int(Math.Log10(psd.FirstWeightClass))
            gp.XAxis.Scale.Max = Math.Round(Math.Log10(psd.FirstWeightClass * 2 ^ (Me.Core.nWeightClasses - 1)) + 0.4, 0, MidpointRounding.AwayFromZero)
            gp.YAxis.Scale.Min = 0

        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)

            Dim psd As cPSDParameters = Me.Core.ParticleSizeDistributionParameters
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing
            Dim sSystemPSD(Me.Core.nWeightClasses) As Single
            Dim curveSelected As BarItem = Nothing
            Dim iSelectedGrpNum As Integer = 1

            InitLists(resultLists, Me.Core.nLivingGroups) '3)

            'Find the system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            For igroup As Integer = 1 To Me.Core.nLivingGroups
                'No need to check if group is selected. Generate the result list even for the not selected group. It will have zero Y values
                'If IsGroupSelected(igroup) Then
                grpOutput = Me.Core.EcoPathGroupOutputs(igroup)
                For iWtClass As Integer = 1 To Me.Core.nWeightClasses
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
            For iGroup As Integer = 1 To Me.Core.nLivingGroups
                If Me.Core.EcoPathGroupOutputs(iGroup).Name = m_lbGroups.Items(m_lbGroups.SelectedIndex).ToString() Then
                    iSelectedGrpNum = iGroup
                    Exit For
                End If
            Next

            For iGroup As Integer = 1 To Me.Core.nLivingGroups
                If iGroup = iSelectedGrpNum Then
                    curveSelected = AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), Me.StyleGuide.GroupColor(Me.Core, iGroup - 1), Color.Gray)
                Else
                    AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), Me.StyleGuide.GroupColor(Me.Core, iGroup - 1), Color.Gray)
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
            For iGroup As Integer = 1 To Me.Core.nLivingGroups
                If Me.m_lbGroups.GroupIndex(iGroup) > -1 Then
                    grpOutput = Me.Core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To Me.Core.nWeightClasses
                        sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                    Next
                End If
            Next
        End Sub

#End Region ' Helper methods

    End Class

End Namespace