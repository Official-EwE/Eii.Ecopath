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
            Me.m_zgh.Attach(Me.m_uic.Core, Me.m_graph)

            ' ToDo: Select first group with Fish1 > 0
            Me.m_grid.SelectedGroupIndex = 1

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            MyBase.OnFormClosed(e)
            Me.m_zgh.Detach()

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
            Handles m_rbB0Bu.CheckedChanged
            If (Me.m_rbB0Bu.Checked) Then Me.EstimationMethod = eEstimationTypes.BoBu
        End Sub

        Private Sub m_rbBuB0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbBuB0.CheckedChanged
            If (Me.m_rbBuB0.Checked) Then Me.EstimationMethod = eEstimationTypes.BuBo
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
            Handles m_btnOK.Click
            Me.Close()
        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click
            If Me.Apply() Then
                Me.Close()
            End If
        End Sub

#End Region ' Events

#Region " Internals "

        Friend Property EstimationMethod() As eEstimationTypes
            Get
                Return Me.m_estimationmethod
            End Get
            Private Set(ByVal value As eEstimationTypes)
                Me.m_estimationmethod = value
                Me.UpdateGrid()
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
            Return True
        End Function

        Private Sub UpdateControls()
            Me.m_rbB0Bu.Checked = (Me.EstimationMethod = eEstimationTypes.BoBu)
            Me.m_rbBuB0.Checked = (Me.EstimationMethod = eEstimationTypes.BuBo)
            Me.m_rbFMaxM.Checked = (Me.EstimationMethod = eEstimationTypes.FMaxM)
            Me.m_rbPredMort.Checked = (Me.EstimationMethod = eEstimationTypes.FMaxBoBu)
        End Sub

        Private Sub UpdateGrid()
            ' ToDo: implement this
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
            Dim bIsLogScale As Boolean = False
            Dim dStepSize As Double

            Dim Vant As Single
            Dim PlotVal(2, 10000) As Single
            Dim XVal(10000) As Single
            Dim sYMax As Single = Single.MinValue

            ' ToDo: fix log scale confusion

            Select Case Me.EstimationMethod

                Case eEstimationTypes.BoBu
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            XVal(j) = B
                            PlotVal(i, j) = Vant
                            sYMax = Math.Max(sYMax, Vant)
                        Next
                    Next
                    If sYMax > 0 Then
                        i = CInt(CLng(Math.Log(sYMax) / Math.Log(10)) \ 1 - 1)
                        dStepSize = 10 ^ i
                        If dStepSize <= 0 Then dStepSize = 1
                    End If

                Case eEstimationTypes.BuBo
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            XVal(j) = 1 / B
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            'for Becopath / bunfished plot then display log10 of vulnerability
                            PlotVal(i, j) = CSng(Math.Log(Vant) / Math.Log(10))
                            sYMax = Math.Max(sYMax, PlotVal(i, j))
                        Next
                    Next
                    If sYMax > 0 Then sYMax = CSng(Math.Round(sYMax + 0.5, 0))

                Case eEstimationTypes.FMaxM
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

                    If sYMax > 100 Then
                        sYMax = CSng(Math.Round(Math.Log(sYMax) / Math.Log(10) + 0.5, 0)) ' \ 1
                        bIsLogScale = True
                        For i = 0 To 1
                            For j = 1 To 1000
                                If PlotVal(i, j) > 0 Then PlotVal(i, j) = CSng(Math.Log(PlotVal(i, j)) / Math.Log(10))
                            Next
                        Next
                    Else
                        bIsLogScale = False
                    End If
                    If sYMax > 0 And bIsLogScale = False Then
                        i = CInt(CLng(Math.Log(sYMax) / Math.Log(10)) \ 1) - 1
                        dStepSize = 10 ^ i
                        If dStepSize <= 0 Then dStepSize = 1
                    End If

                Case eEstimationTypes.FMaxBoBu
                    sYMax = 1
                    For i = 0 To 1
                        For j = 100 To 10000
                            B = CSng(j / 100)
                            XVal(j) = 1 / B
                            Vant = Me.m_uic.Core.CalcEcosimVulBo(B, iGroup, i = 1)
                            If Vant < 0 Then Vant = 1
                            'for Becopath / bunfished plot then display log10 of vulnerability
                            PlotVal(i, j) = 1 / Vant
                        Next
                    Next
            End Select

            gp.CurveList.Clear()
            gp.CurveList.Add(Me.MakePlotLine(XVal, PlotVal, True))
            gp.CurveList.Add(Me.MakePlotLine(XVal, PlotVal, False))
            gp.XAxis.Type = DirectCast(IIf(bIsLogScale, AxisType.Log, AxisType.Linear), AxisType)
            gp.XAxis.Scale.IsUseTenPower = False
            gp.YAxis.Type = DirectCast(IIf(bIsLogScale, AxisType.Log, AxisType.Linear), AxisType)
            gp.YAxis.Scale.IsUseTenPower = False

            ' ToDo: set up scale values
            'gp.XAxis.Scale.Min = sXMin : gp.XAxis.Scale.Max = sXMax
            'gp.YAxis.Scale.Min = sYMin : gp.YAxis.Scale.Max = sYMax

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
                        li.AddPoint(XVal(j), PlotVal(iIndex, j))
                    Next
            End Select

            Return li

        End Function

#If 0 Then ' EwE5 plot code

            If maxX > 0 And maxY > 0 Then
                'Scale and colors and title first
                pic.DrawWidth = 1
    pic.Scale (-0.2 * maxX, maxY * 1.1)-(1.2 * maxX, -0.2 * maxY)
                pic.ForeColor = QBColor(0)
    pic.Line (0, maxY)-(0, 0)
    pic.Line (0, 0)-(maxX, 0)
                'Label x-axis
                For B = 0 To 1.01 * maxX Step 0.1 * maxX
        pic.Line (B, 0)-(B, -0.01 * maxY)
                Next
                pic.CurrentY = -0.02 * maxY
                For B = 0 To 1.01 * maxX Step 0.1 * maxX
                    pic.CurrentX = B - 0.015 * maxX
                    Select Case PlotType
        Case 0: pic.Print Format(B, "0");
        Case 1: pic.Print Format(B, "0.0");
        Case 2: pic.Print Format(B, "0");
        Case 3: pic.Print Format(B, "0.0");
                    End Select
                Next
                'Label Y-axis
                Select Case PlotType
                    Case 0
                        For i = 0 To maxY Step StepSize
            pic.Line (0, i)-(-0.015 * maxX, i)
                        Next
                        For i = 0 To maxY Step StepSize
                            pic.CurrentX = -0.1 * maxX
                            pic.CurrentY = i + StepSize / 15
                            pic.Print(i)
                        Next
                    Case 1
                        For i = 0 To maxY
            pic.Line (0, i)-(-0.015 * maxX, i)
                        Next
                        For B = 0.1 To 1.0# Step 0.1
                            For i = 1 To maxY
                                M = i + Log(B) / Log(10)
                pic.Line (0, M)-(-0.01 * maxX, M)
                            Next
                        Next
                        For B = 0 To maxY
                            pic.CurrentX = -0.1 * maxX
                            pic.CurrentY = B + 0.07
                            pic.Print(10 ^ B)
                        Next
                    Case 2
                        If LogOn Then
                            For i = 0 To maxY
                pic.Line (0, i)-(-0.015 * maxX, i)
                            Next
                            For B = 0.1 To 1.0# Step 0.1
                                For i = 1 To maxY
                                    M = i + Log(B) / Log(10)
                    pic.Line (0, M)-(-0.01 * maxX, M)
                                Next
                            Next
                            For B = 0 To maxY
                                pic.CurrentX = -0.1 * maxX
                                pic.CurrentY = B + 0.07
                                pic.Print(10 ^ B)
                            Next
                        Else
                            For i = 0 To maxY Step StepSize
                pic.Line (0, i)-(-0.015 * maxX, i)
                            Next
                            For i = 0 To maxY Step StepSize
                                pic.CurrentX = -0.1 * maxX
                                pic.CurrentY = i + StepSize / 15
                                pic.Print(i)
                            Next
                        End If
                    Case 3
                        For B = 0 To 1.01 Step 0.1
            pic.Line (0, B)-(-0.015 * maxX, B)
                        Next
                        For B = 0.1 To 1.01 Step 0.1
            pic.Line (0, B)-(-0.01 * maxX, B)
                        Next
                        For B = 0 To 1.01 Step 0.1
                            pic.CurrentX = -0.1 * maxX
                            pic.CurrentY = B + 0.01
                            pic.Print(Format(B, "0.0"))
                        Next
                End Select

                pic.CurrentX = 0.4 * maxX
                pic.CurrentY = 1.09 * maxY
                pic.Print("(" + CStr(Grp) + ") " + Specie(Grp))
                pic.CurrentX = 0.2 * maxX
                pic.CurrentY = 1.06 * maxY
                pic.ForeColor = QBColor(12)
    pic.Print "  Red: with foraging time adjust (FT);";
                pic.ForeColor = QBColor(9)
                pic.Print("   Blue: w/o FT")
                pic.CurrentX = -0.18 * maxX
                pic.CurrentY = 1.065 * maxY
                pic.ForeColor = QBColor(0)
                pic.Print(IIf(PlotType = 3, "Predation mort. (rel.)", "Vulnerability"))
                pic.CurrentY = -0.1 * maxY
                Select Case PlotType
                    Case 0
                        pic.CurrentX = 0.3 * maxX
                        pic.Print("Carrying capacity / Ecopath biomass")
                    Case 1, 3
                        pic.CurrentX = 0.3 * maxX
                        pic.Print("Ecopath biomass / carrying capacity")
                    Case 2
                        pic.CurrentX = 0.4 * maxX
                        pic.Print("Max F / M")
                End Select

                'then plot values, series by series
                pic.DrawWidth = 1.5
                For i = 0 To 1
                    PostPeak = False
                    pic.ForeColor = QBColor(9 + i * 3)
                    LastX = XVal(100)
                    LastY = PlotVal(i, 100)
                    For j = IIf(PlotType = 2, 11, 100) To IIf(PlotType = 2, 1000, 10000)
                        If PlotType = 2 Then
                If LastY > 0 And LastX < Xval(j) Then pic.Line (LastX, LastY)-(Xval(j), PlotVal(i, j))
                        Else
                pic.Line (LastX, LastY)-(Xval(j), PlotVal(i, j))
                        End If
                        If XVal(j) = 0 Then Stop
                        LastX = XVal(j)
                        LastY = PlotVal(i, j)
                    Next
                Next
            End If
#End If

#End Region ' Plot

#End Region ' Internals

    End Class

End Namespace ' Ecosim
