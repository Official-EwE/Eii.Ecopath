#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ZedGraph

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog that implements the Ecosim Estimate Vulnerabilities form.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEstimateVs
        Implements IUIElement

        Friend Enum eEstimationTypes As Integer
            BoBu = 0
            BuBo
            FMaxM
            FMaxBoBu
        End Enum

#Region " Private vars "

        ''' <summary>UI context to use.</summary>
        Private m_uic As cUIContext = Nothing
        Private m_zgh As cZedGraphHelper = Nothing

        Private m_estimationmethod As eEstimationTypes = eEstimationTypes.BoBu

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.UIContext = uic
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph)

            Me.m_grid.SelectedGroupIndex = 1

            AddHandler Me.m_grid.OnSelectedVulnerabilitiesChanged, AddressOf OnSelectedVulnerabilitiesChanged

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            RemoveHandler Me.m_grid.OnSelectedVulnerabilitiesChanged, AddressOf OnSelectedVulnerabilitiesChanged

            Me.m_zgh.Detach()
            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Overrides

#Region " IUIElement implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the UI context for this dialog
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
                Me.m_grid.UIContext = value
            End Set
        End Property

#End Region ' IUIElement implementation

#Region " Events "

        Private Sub m_rbB0Bu_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbBoBu.CheckedChanged
            If (Me.m_rbBoBu.Checked) Then Me.EstimationMethod = eEstimationTypes.BoBu
        End Sub

        Private Sub m_rbBuB0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbBuBo.CheckedChanged
            If (Me.m_rbBuBo.Checked) Then Me.EstimationMethod = eEstimationTypes.BuBo
        End Sub

        Private Sub m_rbFMaxM_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbFMaxM.CheckedChanged
            If (Me.m_rbFMaxM.Checked) Then Me.EstimationMethod = eEstimationTypes.FMaxM
        End Sub

        Private Sub m_rbPredMort_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbPredMort.CheckedChanged
            If (Me.m_rbPredMort.Checked) Then Me.EstimationMethod = eEstimationTypes.FMaxBoBu
        End Sub

        Private Sub OnGroupSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            Me.UpdatePlot()
        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click
            Me.Close()
        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            If Me.Apply() Then
                Me.Close()
            End If
        End Sub

        Private Sub OnSelectedVulnerabilitiesChanged(ByVal grid As gridEstimateVs)
            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internals "

        Friend Property EstimationMethod() As eEstimationTypes
            Get
                Return Me.m_estimationmethod
            End Get
            Private Set(ByVal value As eEstimationTypes)
                Me.m_estimationmethod = value
                Me.UpdatePlot()
            End Set
        End Property

        Friend Property SelectedGroupIndex() As Integer
            Get
                Return Me.m_grid.SelectedGroupIndex
            End Get
            Set(ByVal value As Integer)
                Me.m_grid.SelectedGroupIndex = value
            End Set
        End Property

        Private Function Apply() As Boolean
            Me.m_grid.ApplySelectedVulnerabilities()
            Return True
        End Function

        Private Sub UpdateControls()

            Dim bHasSelectedVulnerabilities As Boolean = Me.m_grid.HasSelectedVulnerabilities()

            Me.m_rbBoBu.Checked = (Me.EstimationMethod = eEstimationTypes.BoBu)
            Me.m_rbBuBo.Checked = (Me.EstimationMethod = eEstimationTypes.BuBo)
            Me.m_rbFMaxM.Checked = (Me.EstimationMethod = eEstimationTypes.FMaxM)
            Me.m_rbPredMort.Checked = (Me.EstimationMethod = eEstimationTypes.FMaxBoBu)
            Me.m_btnOK.Enabled = Me.m_grid.HasSelectedVulnerabilities

        End Sub

#Region " Plot "

        Private Sub UpdatePlot()

            ' ToDo: localize this method

            If (Me.SelectedGroupIndex <= 0) Then Return

            Dim pred As cEcoPathGroupInput = Me.m_uic.Core.EcoPathGroupInputs(Me.SelectedGroupIndex)
            Dim strTitle As String = pred.Name
            Dim strXAxis As String = ""
            Dim strYAxis As String = ""

            Select Case Me.EstimationMethod
                Case eEstimationTypes.BoBu
                    strXAxis = "Carrying capacity / Ecopath biomass"
                    strYAxis = "Vulnerability"

                Case eEstimationTypes.BuBo
                    strXAxis = "Ecopath biomass / carrying capacity"
                    strYAxis = "Vulnerability"

                Case eEstimationTypes.FMaxM
                    strXAxis = "Max F / M"
                    strYAxis = "Vulnerability"

                Case eEstimationTypes.FMaxBoBu
                    strXAxis = "Ecopath biomass / carrying capacity"
                    strYAxis = "Predation mort. (rel.)"

            End Select
            Me.m_zgh.ConfigurePane(strTitle, strXAxis, strYAxis, True)

            Me.PlotGroup(Me.SelectedGroupIndex)

        End Sub

        Private Sub PlotGroup(ByVal iGroup As Integer)

            Dim gp As GraphPane = Me.m_zgh.GetPane(1)

            Dim B As Single
            Dim j As Integer
            Dim i As Integer
            Dim Vant As Single
            Dim PlotVal(2, 10000) As Single
            Dim XVal(10000) As Single
            Dim bIsLogScale As Boolean = False

            Select Case Me.EstimationMethod

                Case eEstimationTypes.BoBu
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            XVal(j) = B
                            PlotVal(i, j) = Vant
                        Next
                    Next

                Case eEstimationTypes.BuBo
                    bIsLogScale = True
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            XVal(j) = 1 / B
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            PlotVal(i, j) = Vant
                        Next
                    Next

                Case eEstimationTypes.FMaxM
                    Dim sYMax As Single = 0
                    For i = 0 To 1
                        For j = 10 To 1000
                            B = CSng(j / 100)
                            Vant = Me.m_uic.Core.CalcEcosimVulFMax(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            XVal(j) = B
                            PlotVal(i, j) = Vant
                            sYMax = Math.Max(sYMax, Vant)
                        Next
                    Next
                    bIsLogScale = (sYMax > 100)

                Case eEstimationTypes.FMaxBoBu
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            XVal(j) = 1 / B
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            PlotVal(i, j) = 1 / Vant
                        Next
                    Next
            End Select

            gp.CurveList.Clear()
            gp.CurveList.Add(Me.MakePlotLine(XVal, PlotVal, True))
            gp.CurveList.Add(Me.MakePlotLine(XVal, PlotVal, False))
            gp.XAxis.Type = AxisType.Linear
            gp.YAxis.Type = DirectCast(IIf(bIsLogScale, AxisType.Log, AxisType.Linear), AxisType)
            gp.YAxis.Scale.IsUseTenPower = bIsLogScale

            gp.XAxis.Scale.MinGrace = 0.0#
            gp.XAxis.Scale.MinAuto = True
            gp.XAxis.Scale.MaxAuto = True
            gp.XAxis.Scale.MaxGrace = 0.0#

            gp.YAxis.Scale.MinAuto = True
            gp.YAxis.Scale.MaxAuto = True
            gp.YAxis.Scale.MinGrace = 0.0#
            gp.YAxis.Scale.MaxGrace = 0.0#

            gp.AxisChange()
            Me.m_zgh.Redraw()

        End Sub

        ''' <summary>
        ''' Generate a line to plot from estimated values.
        ''' </summary>
        ''' <param name="XVal"></param>
        ''' <param name="PlotVal"></param>
        ''' <param name="bFTimeOn"></param>
        ''' <returns></returns>
        Private Function MakePlotLine(ByVal XVal() As Single, _
                                     ByVal PlotVal(,) As Single, _
                                     ByVal bFTimeOn As Boolean) As LineItem

            Dim li As LineItem = Nothing
            Dim iIndex As Integer = 0

            If bFTimeOn = False Then
                li = New LineItem("Without foraging time adjust (FT)")
                li.Line.Color = Color.Blue
                iIndex = 0
            Else
                li = New LineItem("With foraging time adjust")
                li.Line.Color = Color.Red
                iIndex = 1
            End If

            li.Symbol.IsVisible = False

            Select Case Me.EstimationMethod
                Case eEstimationTypes.BoBu, eEstimationTypes.BuBo, eEstimationTypes.FMaxBoBu
                    For j As Integer = 100 To 10000
                        li.AddPoint(XVal(j), PlotVal(iIndex, j))
                        If XVal(j) = 0 Then Exit For
                    Next j
                Case eEstimationTypes.FMaxM
                    For j As Integer = 11 To 1000
                        If PlotVal(iIndex, j) > PlotVal(iIndex, j + 1) Then
                            li.AddPoint(XVal(j), PlotVal(iIndex, j))
                        End If
                    Next
            End Select

            Return li

        End Function

#End Region ' Plot

#Region " Sim calc helpers "


#End Region ' Sim calc helpers

#End Region ' Internals

    End Class

End Namespace ' Ecosim
