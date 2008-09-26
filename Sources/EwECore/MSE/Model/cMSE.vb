'==============================================================================
'
' $Log: cMSE.vb,v $
' Revision 1.1  2008/09/26 07:30:27  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.15  2008/08/12 16:23:55  joeb
' Replaced PlotOn with SearchMode = MSE
'
' Revision 1.14  2008/08/11 21:10:59  joeb
' Changes for Bug Fix 459 Added Search Modes
'
' Revision 1.13  2008/06/25 17:38:43  joeb
' Fix bug 491 Initialization of Ecosim overwriting fishing mort with default
'
' Revision 1.12  2008/05/27 18:44:45  joeb
' Removed ToDo's
'
' Revision 1.11  2008/05/26 18:06:09  joeb
' Test
'
' Revision 1.10  2008/05/05 16:20:50  joeb
' Added population of Output object
'
' Revision 1.9  2008/05/01 20:34:32  joeb
' BioRisk and moved summary variables to datastructures
'
' Revision 1.8  2008/04/29 19:31:31  joeb
' Added clearing of BaseYearCost to Run() this will have to change again
'
' Revision 1.7  2008/04/28 17:59:32  joeb
' Model initialization
'
' Revision 1.6  2008/04/24 14:50:56  joeb
' Added mean results code
'

Imports EwECore
Imports EwECore.Ecosim

Namespace MSE

    Public Enum eCallBackTypes
        Started
        Stopped
        IterationCompleted
        IterationStarted
    End Enum

    Public Delegate Sub MSECallBackDelegate(ByVal CallBackType As eCallBackTypes)

    'ToDo_jb MSE How does the Fishing Policy Search integrate with the MSE
    'ToDo_jb MSE Make sure all public vars need to be public
    'ToDo_jb MSE sort out the initialization

    ''' <summary>
    ''' Management Strategy Evaluation
    ''' </summary>
    ''' <remarks>This was the Closed Loop Simulation in EwE5</remarks>
    Public Class cMSE

        Private m_data As cMSEDataStructures
        Private m_Ecosim As Ecosim.cEcoSimModel
        Private m_Search As cSearchDatastructures
        Private m_esData As cEcosimDatastructures

        Private m_epdata As cEcopathDataStructures

        Private m_nTrials As Integer

        Private m_CallbackDelegate As MSECallBackDelegate

        Private BestTime() As Single
        Private EcoValueBase As Single, ManValueBase As Single
        Private TotValBase As Single, EmployBase As Single

        Public Sub Init(ByRef MSEData As cMSEDataStructures, ByRef Ecosim As Ecosim.cEcoSimModel, ByRef SearchData As cSearchDatastructures, ByVal EcopathData As cEcopathDataStructures)

            Me.m_data = MSEData
            Me.m_Ecosim = Ecosim
            Me.m_Search = SearchData

            Me.m_esData = m_Ecosim.m_Data
            Me.m_epdata = EcopathData

        End Sub

        Public Sub Connect(ByRef CallbackDelegate As MSECallBackDelegate)
            m_CallbackDelegate = CallbackDelegate
        End Sub


        Public Sub InitForRun()

            Try
                Dim iflt As Integer
                ReDim BestTime(m_epdata.NumGroups)

                For iflt = 1 To m_epdata.NumFleet
                    'save qgrowth parameter so as not to interfere with value fitting simulations
                    Me.m_data.QGrowUsed(iflt) = m_data.Qgrow(iflt)
                Next

                'init RstockPred from GstockPred
                'GstockPred could have been altered by an interface
                For iflt = 1 To Me.m_epdata.NumLiving
                    Me.m_data.RstockPred(iflt) = (1 - Me.m_data.GstockPred(iflt)) * Me.m_esData.StartBiomass(iflt)
                Next

                'initialize Ecosim
                m_esData.dimResults()
                m_Ecosim.Init(False)

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".InitForRun() Error:" & ex.Message, ex)
            End Try

        End Sub

        Public Sub Run()

            Dim tmpTotval, tmpEmpval, tmpManval, tmpEcoval As Single
            Dim itr As Integer

            Try

                CallBack(eCallBackTypes.Started)

                'init the MSE data
                Me.InitForRun()

                'set up the search data

                'turn the evaluator on for the trials
                'this will vary Effort (Ecosim.Fgear) and Catability (Ecosim.Qyear)
                'via MSE.YearTimeStep() and MSE.AccessFs
                Me.m_Search.SearchMode = eSearchModes.MSE
                Me.m_Search.initForRun(Me.m_epdata, Me.m_esData)
                Me.m_Search.setMinSearchBlocks() 'set number of search blocks to one and dim FblockCodes()
                If Me.m_Search.BaseYear = 0 Then Me.m_Search.BaseYear = 1
                Me.m_Search.setBaseYearEffort(Me.m_esData)

                'sets MeanEmploy, MeanVal, MeanManVal, MeanEcoVal, MeanTotalValue
                Me.SetBaseValues()

                'runs Ecosim and gets the base values
                Me.setBestTotalValue()

                For itr = 1 To m_data.NTrials

                    m_data.CurrentIteration = itr
                    Me.CallBack(eCallBackTypes.IterationStarted)

                    'Set MSE data back to initial values for a new run
                    m_data.InitForTrial()

                    m_esData.dimResults()
                    m_Ecosim.RunModelValue(m_esData.NumYears, tmpTotval, tmpEmpval, tmpManval, tmpEcoval, Nothing, 0)

                    Me.SumValues(tmpTotval, tmpEmpval, tmpManval, tmpEcoval)

                    Me.CallBack(eCallBackTypes.IterationCompleted)

                Next

                Me.getMeanValues(itr)

                CallBack(eCallBackTypes.Stopped)

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".Run() Error: " & ex.Message)
            End Try

        End Sub

        Private Sub getMeanValues(ByVal NTrials As Integer)

            Me.m_data.MeanEmploy = Me.m_data.MeanEmploy / NTrials
            Me.m_data.MeanVal = Me.m_data.MeanVal / NTrials
            Me.m_data.MeanManVal = Me.m_data.MeanManVal / NTrials
            Me.m_data.MeanEcoVal = Me.m_data.MeanEcoVal / NTrials
            Me.m_data.MeanTotalValue = Me.m_data.MeanTotalValue / NTrials

        End Sub


        ''' <summary>
        ''' Sum results of Model run into Mean values
        ''' </summary>
        ''' <remarks>Once the trials have been finished the mean will be caculated from the sums in getMeanValues() (e.g. MeanEmploy) </remarks>
        Private Sub SumValues(ByVal TotalValue As Double, ByVal EmployValue As Double, ByVal ManValue As Double, ByVal EcoValue As Double)

            m_data.MeanEmploy += EmployValue
            m_data.MeanVal += TotalValue
            m_data.MeanManVal += ManValue
            m_data.MeanEcoVal += EcoValue
            m_data.MeanTotalValue = m_data.MeanTotalValue + m_Search.ValWeight(1) * TotalValue / TotValBase + _
                    m_Search.ValWeight(2) * EmployValue / EmployBase + _
                    m_Search.ValWeight(3) * ManValue / ManValueBase + _
                    m_Search.ValWeight(4) * EcoValue / EcoValueBase

        End Sub

        Private Sub setBestTotalValue()

            Try

                m_Ecosim.RunModelValue(m_esData.NumYears, m_data.BaseTotalVal, m_data.BaseTotalVal, m_data.BaseTotalVal, m_data.BaseEcoVal, Nothing, 0)

                Me.m_data.BestTotalValue = Me.m_Search.ValWeight(1) * m_data.BaseTotalVal / TotValBase + _
                                 Me.m_Search.ValWeight(2) * m_data.BaseTotalVal / EmployBase + _
                                 Me.m_Search.ValWeight(3) * m_data.BaseTotalVal / ManValueBase + _
                                 Me.m_Search.ValWeight(4) * m_data.BaseEcoVal / EcoValueBase

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("MSE.setBestTotalValue() Error: " & ex.Message, ex)
            End Try


        End Sub


        Private Sub SetBaseValues()
            Dim i As Integer, j As Integer, Cval As Single

            'RunModelValue TotalTime, TotValBase, EmployBase, EcoValueBase, lnF(), N, False
            EcoValueBase = 0
            ManValueBase = 0
            TotValBase = 0
            EmployBase = 0

            For i = 1 To m_epdata.NumLiving
                EcoValueBase = EcoValueBase + Me.m_Search.BGoalValue(i)
                ManValueBase = ManValueBase + Me.m_Search.MGoalValue(i)
            Next

            EcoValueBase = EcoValueBase * Me.m_esData.NumYears
            ManValueBase = ManValueBase * Me.m_esData.NumYears
            If ManValueBase = 0 Then ManValueBase = 1 'to avoid division with zero

            For i = 1 To m_epdata.NumLiving
                For j = 1 To m_epdata.NumFleet
                    Cval = m_esData.StartBiomass(i) * Me.m_esData.relQ(j, i) * m_epdata.Market(j, i)
                    TotValBase = TotValBase + Cval  '.5 here assumes cost likely 80% of income
                    EmployBase = EmployBase + Cval * Me.m_Search.Jobs(j)
                Next
            Next

            If Me.m_Search.DiscountFactor > 0 Then
                TotValBase = Math.Abs(TotValBase) / Me.m_Search.DiscountFactor
                EmployBase = Math.Abs(EmployBase) / Me.m_Search.DiscountFactor
            End If

            ManValueBase = Math.Abs(ManValueBase)
            EcoValueBase = Math.Abs(EcoValueBase)

            If TotValBase < 1.0E-20 Then TotValBase = 1.0E-20
            If EmployBase < 1.0E-20 Then EmployBase = 1.0E-20
            If ManValueBase < 1.0E-20 Then ManValueBase = 1.0E-20
            If EcoValueBase < 1.0E-20 Then EcoValueBase = 1.0E-20

        End Sub

        Private Sub CallBack(ByVal CallBackType As eCallBackTypes)
            Try
                System.Console.WriteLine("MSE Callback = " & CallBackType.ToString)
                m_CallbackDelegate(CallBackType)
            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        ''' <summary>
        ''' Count the number of time the Biomass is outside the lower or upper risk boundry
        ''' </summary>
        ''' <param name="Biomass"></param>
        ''' <remarks>The biomass risk count can only be one per trial</remarks>
        Friend Sub AccessBioRisk(ByVal Biomass() As Single)

            Try

                For i As Integer = 1 To m_epdata.NumLiving

                    If m_data.BioR0(i) = False And Biomass(i) < m_data.BioRiskValue(i, 0) * Me.m_esData.StartBiomass(i) Then
                        m_data.BioRiskCount(i, 0) = m_data.BioRiskCount(i, 0) + 1
                        m_data.BioR0(i) = True
                    End If

                    If m_data.BioR1(i) = False And Biomass(i) > m_data.BioRiskValue(i, 1) * Me.m_esData.StartBiomass(i) Then
                        m_data.BioRiskCount(i, 1) = m_data.BioRiskCount(i, 1) + 1
                        m_data.BioR1(i) = True
                    End If

                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".AccessBioRisk() Error: " & ex.Message, ex)
            End Try

        End Sub


        ''' <summary>
        ''' Set Fgear() and QYear() for a management strategy evaluation
        ''' </summary>
        ''' <param name="Fgear">Fishing Effort</param>
        ''' <param name="QYear">Catability growth per year</param>
        ''' <param name="iYear"></param>
        ''' <remarks>Called from Ecosim during a management strategy evaluation</remarks>
        Friend Sub VaryEffortCatchability(ByRef Fgear() As Single, ByRef QYear() As Single, ByVal iYear As Integer)

            Try

                For i As Integer = 1 To Me.m_epdata.NumFleet
                    If iYear > 1 Then
                        QYear(i) = QYear(i) * (1 + Me.m_data.QGrowUsed(i) * Rnd())
                        If m_data.Fwc(i, 1) > 0 Then Fgear(i) = Fgear(i) * m_data.Fwc(i, 0) / m_data.Fwc(i, 1)
                        If Fgear(i) < 1.0E-20 Then Fgear(i) = 1.0E-20
                    End If

                    'ToDo_jb RunModelValue implement sstest.Normal()
                    If iYear = 1 Then Fgear(i) = CSng(Fgear(i) * (1 + Me.Normal * Math.Sqrt(m_data.VarQest(i))))

                Next i

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".YearTimeStep() Error: " & ex.Message, ex)
            End Try


        End Sub


        Friend Sub AssessFs(ByVal Fgear() As Single, ByVal Bbar() As Single)
            'does assessment at end of simulated year in runmodelvalue if ploton=true,
            'returns fwc(i,1)=updated relative catchability for gear i to be used next year
            'for relative effort setting
            'uses: relative fishing efforts this year (Fgear(i)),end-year biomass bb(j)
            'wftot(i)=total fishing importance weight for gear i (computed at start of simulation)
            'yearly catch catchyear(i,j).  Note Fwc(i,0) has ecopath base weighted fishing impacts
            Dim i As Integer, j As Integer, Fwt As Single, Fest(,) As Single, Best() As Single
            Dim Fpred As Single, Bp As Single

            Try

                ReDim Fest(m_epdata.NumFleet, m_epdata.NumLiving), Best(m_epdata.NumLiving)

                'first estimate fishing rates actually achieved by gear and species, Fest(i,j)
                Select Case m_data.AssessMethod

                    Case 0 'biomasses and catch known exactly

                        For i = 1 To m_epdata.NumFleet
                            For j = 1 To m_epdata.NumLiving
                                If Bbar(j) > 0 Then Fest(i, j) = Me.m_Search.CatchYear(i, j) / Bbar(j) Else Fest(i, j) = 0
                            Next
                        Next

                    Case 1 ' Fs from biomass estimates by pool

                        For j = 1 To m_epdata.NumLiving
                            Best(j) = Math.Exp(Normal2() * m_data.CVbiomEst(j)) * Me.m_esData.StartBiomass(j) * (Bbar(j) / Me.m_esData.StartBiomass(j)) ^ m_data.AssessPower
                            If BestTime(j) > 0 Then  'have previous biomass estimate for this run
                                Bp = m_data.GstockPred(j) * BestTime(j) + m_data.RstockPred(j)
                                BestTime(j) = Bp + m_data.KalmanGain(j) * (Best(j) - Bp)
                            Else
                                BestTime(j) = Best(j)
                            End If

                            Best(j) = BestTime(j)

                            For i = 1 To m_epdata.NumFleet
                                If Best(j) > 0 Then Fest(i, j) = Me.m_Search.CatchYear(i, j) / Best(j) Else Fest(i, j) = 0
                            Next i

                        Next j

                    Case 2 ' Fs from direct exploitation method (eg tagging)

                        For i = 1 To m_epdata.NumFleet
                            For j = 1 To m_epdata.NumLiving
                                Fest(i, j) = (m_Search.CatchYear(i, j) / Bbar(j)) * Math.Exp(Normal2() * m_data.CVFest(j))
                            Next
                        Next

                    Case Else
                        Debug.Assert(False, "Assessment Method index set incorrectly")
                        '  MsgBox("Assessment Method index set incorrectly")
                End Select

                'then update relative catchability estimates by gear
                For i = 1 To m_epdata.NumFleet
                    Fwt = 0
                    'If Fgear(i) = 0 Then Fgear(i) = 0.0000000001
                    For j = 1 To m_epdata.NumLiving
                        Fwt = Fwt + Fest(i, j) * m_data.Fweight(i, j)
                    Next
                    Fpred = m_data.Fwc(i, 1) * (1 + m_data.Qgrow(i) / 2)
                    If Fgear(i) > 0 And Fwt > 0 Then
                        m_data.Fwc(i, 1) = Fpred + m_data.KalGainQ(i) * (Fwt / (m_data.Wftot(i) * Fgear(i)) - Fpred)
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".AssessFs() Error: " & ex.Message, ex)
            End Try


        End Sub


        Private Function Normal2() As Single
            Dim R As Single
            'R = -6
            'For i = 1 To 12
            '    R = R + Rnd
            'Next
            R = 2 * Rnd() - 1
            Normal2 = CSng(Math.Log((1 + R) / (1 - R)) / 1.82)

        End Function

        Private Function Normal() As Single
            Dim V1 As Single, V2 As Single
            Do
                V1 = Rnd()
                V2 = Rnd()
            Loop Until V1 > 0
            Normal = CSng(Math.Sqrt(-2 * Math.Log(V1)) * Math.Cos(2 * 3.14159 * V2))
        End Function

    End Class


End Namespace
