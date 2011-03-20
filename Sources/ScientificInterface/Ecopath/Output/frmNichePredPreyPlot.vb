#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph
Imports SAUPUtil.Misc.Colours
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

Namespace Ecopath.Output

    ''' <summary>
    ''' Implementation of the EwE5 niche pred/prey plot
    ''' </summary>
    ''' <remarks></remarks>
    Public Class frmNichePredPreyPlot

        Private m_zgh As cZedGraphHelper = Nothing
        Private m_sCutOff As Single = 0.1!
        Private m_fpCutOff As cEwEFormatProvider = Nothing

        Private Enum eColourType As Integer
            None
            ByPredator
            ByPrey
            ByOverlap
        End Enum

        Private m_colourType As eColourType = eColourType.ByPredator
        Private m_crColor As New ARGBColorRamp(New Color() {Color.White, Color.Gray, Color.Black}, New Double() {0, 0.6, 0.4})

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
            Me.m_zgh.ShowPointValue = True

            Me.m_fpCutOff = New cEwEFormatProvider(Me.UIContext, Me.m_nudCutOff, GetType(Single))
            Me.m_fpCutOff.Value = Me.m_sCutOff
            Me.m_nudCutOff.Maximum = CDec(1)
            Me.m_nudCutOff.Minimum = CDec(0.0)
            Me.m_nudCutOff.Increment = CDec(0.1)

            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If (cmd IsNot Nothing) Then
                cmd.AddControl(Me.m_btnShowHideGroups)
            End If

            Me.CoreExecutionState = eCoreExecutionState.EcopathCompleted

            Me.UpdateControls()
            Me.UpdatePlot()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            If Me.UIContext Is Nothing Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If (cmd IsNot Nothing) Then
                cmd.RemoveControl(Me.m_btnShowHideGroups)
            End If

            Me.m_fpCutOff.Release()
            Me.m_fpCutOff = Nothing

            Me.m_zgh.Detach()
            Me.m_zgh = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Protected Overrides Sub UpdateControls()
            MyBase.UpdateControls()
            Select Case Me.m_colourType
                Case eColourType.ByPredator : Me.m_rbPredator.Checked = True
                Case eColourType.ByPrey : Me.m_rbPrey.Checked = True
                Case eColourType.ByOverlap : Me.m_rbOverlap.Checked = True
            End Select
        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ByVal ct As ScientificInterfaceShared.Style.cStyleGuide.eChangeType)
            MyBase.OnStyleGuideChanged(ct)
            Me.UpdatePlot()
        End Sub

        Private Sub UpdatePlot()

            ' ToDo: globalize this method

            Dim prey As cEcoPathGroupOutput = Nothing
            Dim pred As cEcoPathGroupOutput = Nothing
            Dim ppl As PointPairList = Nothing
            Dim pane As GraphPane = Nothing
            Dim li As LineItem = Nothing
            Dim label As ZedGraph.TextObj = Nothing
            Dim strLabel As String = ""

            pane = Me.m_zgh.ConfigurePane("Niche overlap", "Predator overlap index", "Prey overlap index", False)
            pane.XAxis.Scale.Max = 1.1!
            pane.YAxis.Scale.Max = 1.1!

            ' Clear curves
            pane.CurveList.Clear()
            ' Clear text objects and other misc objects that may have been added
            pane.GraphObjList.Clear()

            For j As Integer = 1 To Me.Core.nGroups

                pred = Me.Core.EcoPathGroupOutputs(j)
                For i As Integer = 1 To Me.Core.nGroups
                    prey = Me.Core.EcoPathGroupOutputs(i)

                    If Me.StyleGuide.GroupVisible(i) And Me.StyleGuide.GroupVisible(j) Then

                        ' Avoid 0
                        If (prey.Hlap(j) > Me.CutOff) And _
                           (prey.Plap(j) > Me.CutOff) And _
                           (i > j) Then

                            ' Create a new line item for each diet
                            ppl = New PointPairList()
                            li = New LineItem(strLabel, ppl, Color.Black, SymbolType.Circle)
                            li.Line.Color = Color.Transparent
                            li.Symbol.Size = CSng(10)

                            Select Case Me.m_colourType
                                Case eColourType.None
                                    li.Symbol.Fill.IsVisible = False
                                Case eColourType.ByPredator
                                    li.Symbol.Fill = New Fill(Me.StyleGuide.GroupColor(Me.Core, pred.Index))
                                Case eColourType.ByPrey
                                    li.Symbol.Fill = New Fill(Me.StyleGuide.GroupColor(Me.Core, prey.Index))
                                Case eColourType.ByOverlap
                                    li.Symbol.Fill = New Fill(Me.m_crColor.GetColor(prey.Hlap(j) + prey.Plap(j) / 2, 1.0))
                            End Select

                            strLabel = String.Format(SharedResources.GENERIC_LABEL_INDEXED, pred.Name, prey.Name)
                            ppl.Add(prey.Hlap(j), prey.Plap(j))

                            label = New TextObj(String.Format("{0}, {1}", pred.Index, prey.Index), _
                                                prey.Hlap(j), prey.Plap(j), CoordType.AxisXYScale, AlignH.Left, AlignV.Top)
                            label.FontSpec.Border.IsVisible = False
                            label.FontSpec.Fill.IsVisible = False
                            pane.GraphObjList.Add(label)

                            pane.CurveList.Add(li)

                        End If
                    End If
                Next
            Next

            Me.m_zgh.RescaleAndRedraw()

        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)
        End Sub

        Private Sub OnColourOptionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbPredator.CheckedChanged, _
                    m_rbPrey.CheckedChanged, _
                    m_rbOverlap.CheckedChanged, _
                    m_rbNone.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return

            If Me.m_rbNone.Checked Then
                Me.m_colourType = eColourType.None
            ElseIf Me.m_rbPredator.Checked Then
                Me.m_colourType = eColourType.ByPredator
            ElseIf Me.m_rbPrey.Checked Then
                Me.m_colourType = eColourType.ByPrey
            Else
                Me.m_colourType = eColourType.ByOverlap
            End If
            Me.UpdatePlot()

        End Sub

        Private Sub OnCutOffValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudCutOff.ValueChanged
            If (Me.UIContext Is Nothing) Then Return
            Me.CutOff = CSng(Me.m_nudCutOff.Value)
        End Sub

        Protected Property CutOff() As Single
            Get
                Return Me.m_sCutOff
            End Get
            Set(ByVal value As Single)
                If (Me.m_sCutOff <> value) Then
                    Me.m_sCutOff = value
                    Me.UpdatePlot()
                End If
            End Set
        End Property
    End Class

End Namespace
