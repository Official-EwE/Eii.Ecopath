'==============================================================================
'
' $Log: EcopathMassBalance.vb,v $
' Revision 1.2  2008/10/01 17:08:57  jeroens
' Reworked CountNoOfMissing to fix issue 543
'
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.23  2008/09/24 15:53:19  jeroens
' EcopathMassBalance once again able to send messages via the core
'
' Revision 1.22  2008/09/24 00:11:03  villyc
' f limits and others
'
' Revision 1.21  2008/09/19 17:00:22  villyc
' edited error message, abort4
'
' Revision 1.20  2008/07/22 20:46:00  jeroens
' Added more suppressable messages
'
' Revision 1.19  2008/07/22 20:33:11  jeroens
' Added suppressable messages
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

Module EcopathMassBalance

    'private copy of public Properties
    Private InParameterEstimation As Integer 'currently in the parameter estimation loop 0 = false 1 = true
    Private Exit_Sub_Missing_Par As Integer 'exit parameter estimation sub (ParamEstimate(...)) because number of missing parameter > 2 


    Private m_data As cEcopathDataStructures
    '  Private mis() As Integer
    Private H() As Double
    Private Y(,) As Double
    Private P() As Double
    Private Q() As Double
    'NoBQB() Acts as a flag for B and QB 
    '1 means B(i) is missing
    '10 means QB(i) is missing
    '11 means both are missing
    Private NoBQB() As Integer
    Private AUL(,) As Single
    Private bDietsModified As Boolean

    Friend m_msgPub As cMessagePublisher = Nothing
    'Friend m_coreNotifier As cCore.CoreMessageDelegate


    Public Function EstimateParameters(ByVal EcoPathDataStructures As cEcopathDataStructures, ByVal EstimateFor As eEstimateParameterFor, ByRef Result As eStatusFlags) As Boolean

        Dim ji As Integer
        'Dim From As Integer
        Dim LoopC As Integer, Pass As Integer, SecLoop As Integer
        Dim noMissing As Integer
        Dim ExitSen As Boolean
        Dim CyclesDone As Boolean
        Dim msg As cMessage = Nothing
        Dim TimesTried As Integer

        m_data = EcoPathDataStructures

        RedimVariables()
        'Programmer: Villy Christensen
        'This is the main module for parametrization, i.e. estimation of 'missing parameter'
        'for securing mass balance
        Try

            Exit_Sub_Missing_Par = 1

Start:
            LoopC = 0

            'VC Sep 2008: the next routine doesn't do anything anymore, so why call it, Joe?
            CountMissingB_Ex()

LoopCalc:
            LoopC = LoopC + 1
            'exit strategies if the loop has executed to many times
            If LoopC > m_data.NumGroups + 2 Then

                If EstimateFor = eEstimateParameterFor.ParameterEstimation Then
                    'ToDo_jb EstimateParameters this code has never been tested!!!!
                    'Thats right kind of scary ehhhhh!
                    'FROM ParamEstimate1
                    If LoopC > m_data.NumGroups + 2 Then
                        If CyclesDone = False Then
                            FindCyclesWhenEstimatingBiomass()
                            CyclesDone = True
                            GoTo Start
                        ElseIf CheckPredatorPreyTrophicLevels() Then
                            'Check if there are crazy values with high consumption of prey with higher TL:
                            GoTo Start
                        ElseIf DoIterationsToEstimateB() <= 3 Then 'Try to do iterations
                            GoTo Start
                        Else
                            Exit_Sub_Missing_Par = 0
                            InParameterEstimation = 0
                            Result = eStatusFlags.MissingParameter
                            Return False
                        End If
                    End If

                ElseIf EstimateFor = eEstimateParameterFor.Sensitivity Then
                    Exit_Sub_Missing_Par = 0
                    InParameterEstimation = 0
                    Result = eStatusFlags.MissingParameter

                    Return False

                    'From SensitivLoop
                    'If LoopC > m_data.NumGroups + 2 Then
                    '    MsgBox("Too many loops. Quitting Parameter Estimation.")
                    '    Exit_Sub_Missing_Par = 0
                    '    InParameterEstimation = 0
                    '    Return False
                    'End If
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


                End If 'If EstimateFor = eEstimateParameterFor.ParameterEstimation Then
            End If ' If LoopC > m_data.NumGroups + 2 Then

            EstimateGE()

            If (CountNoOfMissing(m_data.mis, noMissing, EstimateFor) = False) Then
                InParameterEstimation = 0
                'jb 
                cLog.Write("Too many missing parameters. Parameter Estimation not completed successfully.")
                Result = eStatusFlags.MissingParameter
                Return False
            End If
            '040112VC: In case B is missing, BABi is entered, then estimate BA(i) again
            For ji = 1 To m_data.NumLiving
                If m_data.BA(ji) = 0 Then
                    m_data.BA(ji) = CSng(IIf(m_data.BaBi(ji) <> 0 And m_data.B(ji) > 0, m_data.BaBi(ji) * m_data.B(ji), m_data.BA(ji)))
                    'jb was m_data.BA(ji) = IIf(m_data.BaBi(ji) <> 0 And m_data.B(ji) > 0, m_data.BaBi(ji) * m_data.B(ji), m_data.BAi(ji))
                End If

            Next

            If noMissing > 0 Then              ' else No GIM

                EstimatePB(Pass)
                If Pass = 1 Then
                    GoTo LoopCalc ' NOW PB IS KNOWN
                End If

                EstimateEE(Pass)
                If Pass = 1 Then
                    GoTo LoopCalc ' Now EE is known
                End If

                EstimateB(Pass, EstimateFor, ExitSen)

                If EstimateFor = eEstimateParameterFor.ParameterEstimation And (Exit_Sub_Missing_Par = 0 Or SecLoop = 3) Then
                    'ToDo_jb EstimateParameters AGAIN this code has never been tested!!!!

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'jb
                    'Exit stratagy ParamEstimate1 
                    If Exit_Sub_Missing_Par = 0 Or SecLoop% = 3 Then
                        If CyclesDone = False Then
                            'FindCyclesWhenEstimatingBiomass(Cons())
                            FindCyclesWhenEstimatingBiomass()
                            If bDietsModified Then checkDietsSumToOne(True)
                            CyclesDone = True
                            GoTo Start
                        ElseIf CheckPredatorPreyTrophicLevels() Then
                            'Check if there are crazy values with high consumption of prey with higher TL:
                            GoTo Start
                        ElseIf DoIterationsToEstimateB() <= 3 And TimesTried <= 10 Then 'Try to do iterations
                            TimesTried = TimesTried + 1
                            GoTo Start
                        Else
                            Exit_Sub_Missing_Par = 0
                            InParameterEstimation = 0
                            Result = eStatusFlags.MissingParameter
                            Return False
                        End If
                    End If
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                End If



                'in the original code  EstimateB(Pass, from) could set "From" to -1
                'which would cause the loop to exit
                'This has been changed to set a boolean flag 'ExitSen'
                ''ExitSen' should only get set when called with EstimateFor =  eEstimateParameterFor.Sensitivity
                'see EstimateB()
                'If From = -1 Then
                If ExitSen Then
                    'GoTo NextSensL ' Couldn't est parameters
                    '    Debug.Assert(False)
                    Return False
                End If

                If Pass = 1 Then
                    GoTo Start ' Now B is known
                End If

                EstimateQBorB_1(Pass)
                If Pass = 1 Then
                    GoTo Start
                End If

                EstimateQBorB_2(Pass)
                If Pass = 1 Then
                    GoTo Start
                End If

                If EstimateFor = eEstimateParameterFor.Sensitivity And SecLoop = 3 Then
                    'jb todo implement the exit below
                    'this has not been implemented yet so just assert for now
                    '    Debug.Assert(False)
                    Result = eStatusFlags.OK
                    Return False

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'Exit strategy from SensitivLoop
                    'If SecLoop = 3 Then
                    '    'If Not ShownAlready Then
                    '    '    MsgBox "Cannot estimate all parameters." + Chr$(13) + " Results may be unreliable." + Chr$(13) + "Please check your data"
                    '    '    ShownAlready = -1
                    '    'End If
                    '    'jb this is the equivalent to exit sub
                    '    GoTo NextSensL
                    '    'Insufficient Data
                    '    'jb
                    '    'Exit Sub
                    'End If
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                End If

                'Enter Generalized Inverse Method
                Pass = 0
                For ji = 1 To m_data.NumLiving                  'GIM
                    If m_data.mis(ji) > 0 Then           'IF EQ2=0 THEN B,PB,QB,EE known
                        'If Mis(ji) > 1 Then           'IF EQ2=0 THEN B,PB,QB,EE known
                        GIM()

                        If Exit_Sub_Missing_Par = 0 Then
                            InParameterEstimation = 0
                            Result = eStatusFlags.MissingParameter
                            Return False
                        End If

                        Pass = 1
                        ji = m_data.NumLiving + 1
                        SecLoop = SecLoop + 1
                    End If       'B(2)  B(3)  EE(1)
                Next ji

                If Pass = 1 Then
                    GoTo LoopCalc
                End If

            End If           'If NoMissing>0   This was earlier called NOGIM

            'If iterate > 1 Then CalcDeviation()
            'If iterate > 1 Then Progress()
            'NextSensL:
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, "Error in EstimateParameters() " & ex.Message)
            Result = eStatusFlags.ErrorEncountered
            Return False
        End Try

        Result = eStatusFlags.OK
        Return True
    End Function


    Sub FindCyclesWhenEstimatingBiomass()
        ' Sub FindCyclesWhenEstimatingBiomass(ByVal Cons() As Single)'EwE5
        'CycDC [previously called CD] contains the proportion of the diet that is the minimum
        'amount in a cycle and should be removed to break the cycle.
        'This amount is subtracted from all flows in the cycle that contains only groups without biomasses.
        Dim APred, Comp, pred, prey As Integer
        'Dim Answer As Object
        Dim Cnt As Long
        'Dim arrow, bib
        Dim K As Integer
        Dim Diff As Single
        Dim PredTL As Single
        Dim PreyTL As Single

        Dim Pivot As Integer
        Dim path() As Integer
        Dim lastComp() As Integer
        Dim i As Integer
        Dim Level As Integer

        'jb this is in the Network Analysis plugin
        EstimateTrophicLevels(m_data.DC, m_data.TTLX)

        'AbortRun = False
        'DoWhat = "PPR"
        'frmWait.Caption = "Cycles may prevent estimation of biomasses; will break cycles by modifying diets"
        'frmWait.Frame1.Visible = False
        'frmWait.PBar.max = 1000 'm_data.Numliving
        'frmWait.ZOrder()
        'frmWait.Show() '0
        'frmWait.Refresh()
        Cnt = 0
        '   Array.Clear(Cons, 0, Cons.Length)

        ReDim path(2 * m_data.NumGroups + 2)
        ReDim lastComp(2 * m_data.NumGroups + 2)

        For Pivot = 1 To m_data.NumLiving
            If m_data.B(Pivot) > 0 Then GoTo NextPivot
            Array.Clear(path, 0, path.Length)
            '  Init1DimInteg(0, 2 * m_data.NumGroups + 2, Path())
            '  Assign1DimInteg(1, 2 * m_data.NumGroups + 1, Pivot, lastComp)
            For i = 1 To lastComp.Length - 1 : lastComp(i) = Pivot : Next

            path(Pivot - 1) = Pivot           ' Path's limits are Pivot -1 to Level 
            '*** FOR Level  = Pivot  TO m_data.Numliving
            For Level = Pivot To 2 * m_data.NumLiving
                If path(Level - 1) > 0 Then
                    pred = path(Level - 1)
                Else
                    pred = Pivot
                End If
                For Comp = lastComp(Level) To m_data.NumLiving
                    'only for groups that do not have biomass
                    If m_data.B(Comp) <= 0 And m_data.DC(pred, Comp) > 0 Then
                        prey = Comp
                        path(Level) = 0
                        CheckPath(path, Pivot, prey, Level)
                        If prey = 0 And Comp <> Pivot Then GoTo NextComp 'In Path already
                        If Pivot = Comp Then
                            path(Level) = Comp
                            '------------------------------------
                            'Next is a version of CyclePrint:
                            'CyclePrint CycDC(), Cons()
                            'arrow  = 1
                            'aa  = 0
                            'For k  = Pivot  - 1 To Level 
                            '    If Path(k ) > 0 Then
                            '        bib  = aa 
                            '        aa  = Path(k )
                            '        If arrow  = 0 Then
                            '            'mess$ = mess$ & "<---"
                            '            If CycDC(bib , aa ) < 0 Then
                            '                Cons(bib ) = 0
                            '            Else
                            '                Cons(bib ) = -DC(bib , aa )
                            '            End If
                            '        End If
                            '        'mess$ = mess$ & Cstr$(Path(k ))
                            '        arrow  = 0
                            '    End If
                            'Next k 
                            'MsgBox mess$
                            'mess$ = ""
                            'end of CyclePrint
                            '-------------------------------------
                            Cnt = Cnt + 1
                            'frmWait.Label1(0).Caption = "No of cycles: " + CStr(Cnt)
                            'UpdateWait()
                            'If AbortRun Then Exit Sub 'Have identified a cycle

                            'FindMinConsump Cons(), MinCons
                            'Find the link with the highest difference in TL
                            Diff = 100
                            For K = Pivot To m_data.NumLiving
                                If path(K) > 0 Then
                                    PreyTL = m_data.TTLX(path(K))
                                    PredTL = m_data.TTLX(path(K - 1))
                                    If PredTL - PreyTL < Diff Then
                                        Diff = PredTL - PreyTL
                                        prey = path(K)
                                        pred = path(K - 1)
                                    End If
                                    'If Cons(Path(k)) <= 0 And QB(Path(k)) > 0 Then
                                    '    If Cons(Path(k)) > MinCons Then MinCons = Cons(Path(k))
                                    'End If
                                Else
                                    Exit For
                                End If
                            Next K
                            If pred <> prey Then    'no need to break cannibalism cycles
                                'Debug.Print pred, prey, DC(pred, prey)
                                m_data.DC(pred, prey) = 0
                                bDietsModified = True
                            End If
                            path(Level) = 0
                        Else
                            path(Level) = Comp                      'Include group in Path
                            path(Level + 1) = 0
                            lastComp(Level) = Comp
                            lastComp(Level + 1) = Pivot
                            APred = 1
                            Exit For              'exit Comp  for loop when path found
                            'and continue to next Level 
                        End If
                    End If
                    APred = 0          'if program doesn't use EXIT FOR it will reset APred 
NextComp:

                    '   DoEvents()
                Next Comp
                '       If AbortRun Then Exit Sub 'Have identified a cycle
                If APred = 0 Then                   'Start backtracking
                    'For Answer = 1 To Level: Debug.Print Format(Path(Answer), "  ##");: Next
                    'Debug.Print
                    If Level > Pivot Then lastComp(Level - 1) = path(Level - 1) + 1
                    path(Level) = 0
                    Level = Level - 2
                    If Level = Pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                End If
                '      frmWait.Label1(0).Caption = "Cycles: " + CStr(Cnt) + ", Pivot: " + CStr(Pivot) + ", Level: " + CStr(Level)
            Next Level
NextPivot:
        Next Pivot
        'Unload(frmWait)
        'frmWait = Nothing
    End Sub


    Sub CheckPath(ByRef path() As Integer, ByVal Pivot As Integer, ByRef prey As Integer, ByVal level As Integer)
        Dim K As Integer

        For K = Pivot - 1 To level + 1
            If prey = path(K) Then prey = 0 : Exit For
        Next K

    End Sub


    Private Sub RedimVariables()

        'ReDim mis(m_data.NumGroups)

        Erase NoBQB
        Erase H
        Erase Y
        Erase P
        Erase Q
        Erase AUL

        ReDim NoBQB(m_data.NumGroups)
        ReDim H(m_data.NumGroups + 3)
        ReDim Y(m_data.NumGroups, m_data.NumGroups)
        ReDim P(m_data.NumGroups)
        ReDim Q(m_data.NumGroups + 10)
        ReDim AUL(m_data.NumGroups + 3, m_data.NumGroups + 3)

    End Sub

    Private Sub CountMissingB_Ex()
        Dim doub1 As Integer, doub2 As Integer, i As Integer

        doub1 = 0
        doub2 = 0
        For i = 1 To m_data.NumLiving
            If m_data.B(i) <= 0 Then doub1 = doub1 + 1
            If m_data.Ex(i) = 0 Then doub2 = doub2 + 1
        Next i
        If doub1 = m_data.NumLiving And doub2 > m_data.NumLiving - 1 Then
            'up to Mar. 94 it was Doub2 > NumGroups - 4 but no need for this ***
            'MsgBox("No biomasses -- Edit data ")
            'End
        End If

    End Sub

    Private Sub EstimateGE()
        Dim i As Integer

        For i = 1 To m_data.NumLiving
            If m_data.PB(i) < 0 And m_data.QB(i) > 0 And m_data.GE(i) > 0 Then
                m_data.PB(i) = m_data.GE(i) * m_data.QB(i)
            End If

            If m_data.QB(i) < 0 And m_data.PB(i) > 0 And m_data.GE(i) > 0 Then
                m_data.QB(i) = m_data.PB(i) / m_data.GE(i)
            End If

            If m_data.QB(i) > 0 And m_data.PB(i) >= 0 Then
                m_data.GE(i) = m_data.PB(i) / m_data.QB(i)
            End If
        Next i

    End Sub

    Private Function CountNoOfMissing(ByRef Mis() As Integer, ByRef nNoMissing As Integer, ByVal From As eEstimateParameterFor) As Boolean
        'Private Sub CountNoOfMissing(ByRef Mis() As Integer, ByRef NoMissing As Integer, ByVal From As String, ByVal chk As Integer)

        'count the number of missing parameters for each group and store the values in the argument Mis()
        'this will have to change because the basic estimator and the Sensitivity loop count the number of missing parameters differently
        Dim iMissingForGroup As Integer
        Dim i As Integer
        Static done As Boolean

        nNoMissing = 0

        For i = 1 To m_data.NumLiving
            iMissingForGroup = 0
            If m_data.B(i) <= 0 Then iMissingForGroup += 1
            If m_data.PB(i) < 0 Then iMissingForGroup += 1
            If m_data.EE(i) < 0 Then iMissingForGroup += 1

            ' If Miss >= 2 And From = "ParameterEstimate" Then
            If iMissingForGroup >= 2 And From = eEstimateParameterFor.ParameterEstimation Then
                MsgManyMissingPar(i)
                Exit_Sub_Missing_Par = 0
                cLog.Write("'CountNoOfMissing(...)' Group " & i & " missing " & iMissingForGroup.ToString & " parameter(s).")
                Return False
            End If

            If m_data.QB(i) < 0 And m_data.PP(i) < 1 Then
                iMissingForGroup = iMissingForGroup + 1
            End If

            '   If Miss >= 2 And From = "SensitivLoop" Then
            If iMissingForGroup >= 2 And From = eEstimateParameterFor.Sensitivity Then
                ' From Sensitivity routine
                ' chk = 1
                If done = False Then
                    'jb todo
                    'MsgManyMissingSens(i)
                    done = True
                End If

                Exit_Sub_Missing_Par = 0
                Return False
            End If

            Mis(i) = iMissingForGroup
            nNoMissing += iMissingForGroup
        Next i
        Return True

    End Function


    Private Sub EstimatePB(ByRef Pass As Integer)
        Dim MM2 As Double
        Dim i As Integer, j As Integer
        Dim Sum As Single

        For j = 1 To m_data.NumLiving
            Pass = 0                       'Estimate PB from other parameters
            If m_data.PB(j) < 0 And m_data.B(j) > 0 And m_data.EE(j) >= 0 Then    '1490
                MM2 = 0
                For i = 1 To m_data.NumLiving
                    If m_data.DC(i, j) > 0 Then                         '1470
                        If m_data.B(i) <= 0 Or m_data.QB(i) < 0 Then Exit For '1490
                        MM2 = MM2 + m_data.B(i) * m_data.QB(i) * m_data.DC(i, j)   'M2 is amount eaten of
                    End If                                          'group J by predators I.

                    If i = m_data.NumLiving Then
                        If (m_data.B(j) * m_data.EE(j)) <> 0 Then
                            '031220VC: Either BABi or BA is zero; Either Emigration or Emig is zero
                            Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                            Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                            Sum = Sum * m_data.B(j)
                            m_data.PB(j) = CSng((MM2 + Sum + m_data.BA(j) + m_data.Emigration(j) - m_data.Immig(j) + m_data.fCatch(j)) / (m_data.B(j) * m_data.EE(j)))
                            'Added mig above 15022000 per discussion with Kerim / Villy
                        End If

                        If m_data.PB(j) > 0 Then
                            Pass = 1
                            Exit Sub
                        Else
                            m_data.PB(j) = -9                                    'Calc production
                        End If
                    End If
                Next i
            End If
        Next j
    End Sub

    Private Sub EstimateEE(ByRef Pass As Integer)
        Dim i As Integer
        Dim j As Integer
        Dim MM2 As Double
        Dim Sum As Single
        'Estimate EE from
        For j = 1 To m_data.NumLiving                              'other parameters
            Pass = 0
            If m_data.EE(j) < 0 And m_data.B(j) > 0 And m_data.PB(j) > 0 Then
                MM2 = 0
                For i = 1 To m_data.NumLiving
                    If m_data.DC(i, j) > 0 And m_data.PP(i) < 1 Then

                        If m_data.B(i) <= 0 Or m_data.QB(i) < 0 Then
                            GoTo nextJ 'Exit For
                        End If

                        MM2 = MM2 + m_data.B(i) * m_data.QB(i) * m_data.DC(i, j)     'M2 is amount eaten of
                    End If                                         'group j by predators i
                    '031220VC Now has Emigi and BABi as rates, won't have values if Emigration and BA have.
                    '031220VC: Either BABi or BA is zero; Either Emigration or Emig is zero
                Next i
                Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                Sum = Sum * m_data.B(j)

                If m_data.B(j) * m_data.PB(j) > 0 Then
                    If m_data.StanzaGroup(j) = False Then
                        m_data.EE(j) = CSng(MM2 + Sum + m_data.Emigration(j) - m_data.Immig(j) + m_data.BA(j) + m_data.fCatch(j)) / (m_data.B(j) * m_data.PB(j))
                    Else
                        m_data.EE(j) = CSng(MM2 + Sum + m_data.Emigration(j) - m_data.Immig(j) + m_data.fCatch(j)) / (m_data.B(j) * m_data.PB(j))
                    End If
                End If

                If m_data.EE(j) >= 0 Then
                    Pass = 1
                    Exit Sub
                Else
                    m_data.EE(j) = -91
                End If
            End If 'If m_data.EE(j) < 0 And m_data.B(j) > 0 And m_data.PB(j) > 0 Then
nextJ:
        Next j
    End Sub


    Private Sub EstimateB(ByRef Pass As Integer, ByVal EstimateFor As eEstimateParameterFor, ByRef SenExit As Boolean)
        Dim i As Integer
        Dim j As Integer
        Dim Miss As Integer
        Dim PartM2 As Double
        Dim Only As Double
        Dim CancelPressed As Boolean = False
        Dim Sum As Single
        Dim msg As cFeedbackMessage = Nothing
        Dim strMessage As String = ""

        For j = 1 To m_data.NumLiving
            Pass = 0 'Estimate B
            If m_data.PB(j) > 0 And m_data.EE(j) > 0 And m_data.B(j) <= 0 And m_data.mis(j) = 1 Then
                'If Mis(j) = 0 Or Mis(j) > 1 Then goto NextJ '1680
                Miss = 0 : PartM2 = 0
                For i = 1 To m_data.NumLiving
                    If m_data.DC(i, j) > 0 Then
                        If m_data.B(i) <= 0 Or m_data.QB(i) < 0 Then
                            If i <> j Then GoTo nextJ '1680
                        Else
                            PartM2 = PartM2 + m_data.B(i) * m_data.QB(i) * m_data.DC(i, j)
                        End If
                    End If
                Next i
                If m_data.QB(j) < 0 And m_data.PP(j) < 0 Then GoTo nextJ '1680
                '031220VC: Either BABi or BA is zero; Either Emigration or Emig is zero
                Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                Only = m_data.PB(j) * m_data.EE(j) - m_data.QB(j) * m_data.DC(j, j) - Sum
                ' There may be too much cannibalism when e.g. the biomass
                ' of a group is changed and the EE is kept constant in the
                ' sens analysis. The results are not valid when this happens
                ' and they are not presented, but still the sensitivity routine
                ' is allowed to continue.
                If Only < 0 Then
                    'If From = 1 Then             ' From Parameter estimation
                    If EstimateFor = eEstimateParameterFor.ParameterEstimation Then
                        If Abort4(j) Then 'has changed data if this is true
                            'repeat estimation
                            Pass = 1    'Will make it start estimation again from scratch
                        Else
                            Exit_Sub_Missing_Par = 0
                        End If
                        Exit Sub
                        'jb changed from
                        ' ElseIf From = 2 Then         ' From Sensitivity routine
                    ElseIf EstimateFor = eEstimateParameterFor.Sensitivity Then
                        'the calling model is the Sensitivity routine 
                        'this will cause it to exit the parameter estimation
                        SenExit = True
                    End If
                End If
                If Only = 0 Then GoTo nextJ '1680
                If PartM2 < 0 Then m_data.B(j) = -99 : GoTo nextJ '1680
                ' Up to Mar 94 it was PartM2 <= 0 but this failed
                ' to catch cases with toppredators with unknown B.
                m_data.B(j) = CSng((m_data.fCatch(j) + m_data.BA(j) + m_data.Emigration(j) - m_data.Immig(j) + PartM2) / Only)
                'Added mig above 15022000 per discussion with Kerim / Villy
                If m_data.B(j) > 0 Then
                    Pass = 1
                    Exit Sub
                ElseIf Not CancelPressed Then

                    If m_data.B(j) = 0 Then
                        ' Prepare message text
                        strMessage = String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_B0_FISHERY, j, m_data.GroupName(j))
                        ' Prepare message
                        msg = New cFeedbackMessage(strMessage, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                        ' Send off
                        NotifyCore(msg)
                        ' Catch result
                        CancelPressed = (msg.Reply = 0)
                    End If

                    If m_data.B(j) < 0 Then
                        ' ToDo_JS: Change message box behaviour to run via Core messages (as above)
                        If Only < 0 Then
                            ' Prepare message text
                            strMessage = String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_PRODxEE, j, m_data.GroupName(j), Only.ToString("0.000"))
                            ' Prepare message
                            msg = New cFeedbackMessage(strMessage, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                            ' Send off
                            NotifyCore(msg)
                            ' Catch result
                            CancelPressed = (msg.Reply = 0)
                        Else
                            ' Prepare message text
                            strMessage = String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_B_FISHERIY, j, m_data.GroupName(j), m_data.B(j).ToString("0.000"))
                            ' Prepare message
                            msg = New cFeedbackMessage(strMessage, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                            ' Send off
                            NotifyCore(msg)
                            ' Catch result
                            CancelPressed = (msg.Reply = 0)
                        End If
                    End If
                    If CancelPressed = True Then Exit Sub
                    m_data.B(j) = -9
                End If
            End If
nextJ:
        Next j
    End Sub

    Private Function Abort4(ByVal j As Integer) As Boolean
        Dim str As String
        Dim str2 As String
        Dim RetVal As cFeedbackMessage.eReply
        Dim Answer As Single
        Dim msg As cFeedbackMessage = Nothing
        Static done As Boolean



        ' ToDo_JS: Localize this
        str = "Your data are not consistent. In algorithm 4 your estimate of: P/Bi * EEi - Q/Bi * DCii is negative "
        str = str & "for group " & j & ", i.e. 'cannibalism' exceeds the predation mortality."
        str = str & vbNewLine + vbNewLine
        str = str & "See the description of Algorithm 4 in Appendix 4."
        str2 = str & vbNewLine + vbNewLine
        str2 = str2 & "Do you want to have cannibalism reduced (to 20 of used production) for all groups where this problem occurs. (Note: your input data will not be changed)"
        If done = False Then
            msg = New cFeedbackMessage(str, eMessageSource.EcoPath, eMessageImportance.Maintenance, cFeedbackMessage.eReplyStyle.YES_NO_CANCEL)
            NotifyCore(msg)

            RetVal = msg.Reply

        End If
        If RetVal = cFeedbackMessage.eReply.CANCEL Then done = True
        Abort4 = False

        If RetVal = cFeedbackMessage.eReply.YES Then
            m_data.DietsModified = True
            For j = 1 To m_data.NumLiving
                If m_data.EE(j) > 0 Then
                    Answer = m_data.PB(j) * m_data.EE(j) - m_data.QB(j) * m_data.DC(j, j)
                    If Answer < 0 Then  'cannibalism exceeds utilized production
                        m_data.DC(j, j) = m_data.PB(j) * m_data.EE(j) / m_data.QB(j) / 5
                    End If
                End If
            Next
            'Now make the diets sum to 1 again:
            Abort4 = checkDietsSumToOne(True)
        Else
            ' ToDo_JS: Remove this message box, and figure out what text to display. The current message text is not overly informative.
            ' VC Sep 2008 updated this, but done is alway true?
            MsgBox(str & vbNewLine & vbNewLine & "Please edit your data.")
        End If

exitSub:
    End Function

    Private Function checkDietsSumToOne(ByVal NoQuestionsAsked As Boolean) As Boolean
        Dim pred As Integer
        Dim prey As Integer
        Dim Sum As Single
        Dim RetVal As cFeedbackMessage.eReply = cFeedbackMessage.eReply.CANCEL
        Dim briefQuestion As Boolean
        Dim tolerance As Single
        Dim msg As cFeedbackMessage = Nothing

        briefQuestion = True
        tolerance = 0.001
        checkDietsSumToOne = True
        For pred = 1 To m_data.NumLiving Step 1
            If m_data.PP(pred) < 1 Then    'a consumer
                Sum = 0
                For prey = 0 To m_data.NumGroups Step 1
                    Sum = Sum + m_data.DC(pred, prey)
                Next
                If Sum <> 0 And Math.Abs(Sum - 1) > tolerance Then
                    If NoQuestionsAsked Then    'just do it
                        RetVal = cFeedbackMessage.eReply.OK
                    ElseIf briefQuestion Then
                        '    Debug.Assert(False, "checkDietsSumToOne() needs message")
                        'ToDo_jb 
                        '    ' first time only
                        '    briefQuestion = False
                        ' ToDo_JS: globalize this
                        msg = New cFeedbackMessage(My.Resources.CoreMessages.DIETCOMP_PROMPT_SUMTOONE, _
                                eMessageSource.EcoPath, eMessageImportance.Information, cFeedbackMessage.eReplyStyle.YES_NO)
                        msg.Suppressable = True
                        NotifyCore(msg)
                        RetVal = msg.Reply

                        '    If RetVal = cFeedbackMessage.eReply.Yes Then
                        '        NoQuestionsAsked = True
                        '    Else

                        '        ' check rest individually
                        '        msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_NORMALIZE_DIET, Pred, Sum), _
                        '                 eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                        '        notifyCore(msg)
                        '        RetVal = msg.Reply

                        '    End If
                        'Else
                        '    ' dialog for each group (slow way)
                        '    msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_NORMALIZE_DIET, Pred, Sum), _
                        '             eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                        '    notifyCore(msg)
                        '    RetVal = msg.Reply

                        '    'Text = Text & vbNewLine & vbNewLine & "Raising will only affect the calculations, not change the input. "
                        '    'Text = Text & vbNewLine & "To change your input data, select 'No' below, open the"
                        '    'Text = Text & vbNewLine & "diet input form, and select the button for raising. "

                    End If

                    'Normalize the diet for pred
                    If RetVal = cFeedbackMessage.eReply.YES Then
                        For prey = 0 To m_data.NumGroups Step 1
                            m_data.DC(pred, prey) = m_data.DC(pred, prey) / Sum
                        Next
                        m_data.DietsModified = True
                    Else
                        checkDietsSumToOne = False
                        GoTo EndOfFunction  'OK to abort now as diets need editing
                    End If
                End If
            End If
        Next
EndOfFunction:
    End Function

    Private Sub EstimateQBorB_1(ByVal Pass As Integer)
        'The following is a routine made by VC in March 1994 to estimate
        'QB Or B independent of the Generalized Inverse. It works in cases
        'where for a given predator j the PB, B, EE are known for all
        'prey, and where all predation on these prey apart from that
        'caused by predator j is known

        Dim i As Integer, j As Integer, K As Integer
        Dim SumQ As Single, SumMi As Single
        Dim Sum As Single

        For j = 1 To m_data.NumLiving
            SumQ = 0
            Pass = 0                      'Estimate QB or B
            If (m_data.QB(j) > 0 And m_data.B(j) <= 0) Or (m_data.QB(j) < 0 And m_data.B(j) > 0) Then
                ' If QB(j) * B(j) < 0 Then
                ' If both are known or both are unknown it won't enter
                For i = 1 To m_data.NumLiving
                    If m_data.DC(j, i) > 0 And SumQ >= 0 Then
                        If m_data.B(i) > 0 And m_data.PB(i) > 0 And m_data.EE(i) >= 0 Then
                            '031220VC:
                            Sum = CSng(IIf(m_data.BaBi(i) <> 0 And m_data.BA(i) = 0, m_data.BaBi(i), 0))
                            Sum = Sum + CSng(IIf(m_data.Emig(i) > 0 And m_data.Emigration(i) = 0, m_data.Emig(i), 0))
                            Sum = Sum * m_data.B(i)
                            SumMi = m_data.BA(i) + Sum + m_data.Emigration(i) + m_data.Immig(i) + m_data.fCatch(i) + (1 - m_data.EE(i)) * m_data.PB(i) * m_data.B(i)
                            'Added mig above 15022000 per discussion with Kerim / Villy
                            'SumMi is used to add up all mortalities of i. If the only
                            ' lacking mortality is due to j then QB(j) or B(j) can
                            ' be estimated. The first term (above) sums export and M0.
                            For K = 1 To m_data.NumLiving
                                If K <> j Then
                                    If m_data.DC(K, i) > 0 Then
                                        'This is a predator on i
                                        If m_data.QB(K) > 0 And m_data.B(K) > 0 Then
                                            SumMi = SumMi + m_data.QB(K) * m_data.B(K) * m_data.DC(K, i)
                                            'This terms gives how much k eats of i
                                        Else
                                            SumMi = -9
                                            SumQ = -9
                                            Exit For    'for k
                                        End If
                                    End If    'End DC(k,i) > 0
                                End If       'End k <> j
                            Next K
                            If SumMi > 0 Then SumQ = SumQ + m_data.PB(i) * m_data.B(i) - SumMi
                        Else
                            SumQ = -9
                            Exit For    'for i
                        End If
                    End If
                Next i
            End If
            If SumQ > 0.0001 Then '0 Then
                If m_data.QB(j) < 0 And m_data.B(j) > 0 And SumQ > 0.0001 Then
                    m_data.QB(j) = SumQ / m_data.B(j)
                    Pass = 1
                ElseIf m_data.B(j) <= 0 And m_data.QB(j) > 0 And SumQ > 0.0001 Then
                    m_data.B(j) = SumQ / m_data.QB(j)
                    Pass = 1
                End If
                SumQ = -9
            End If
        Next j
    End Sub

    Private Sub GIM()
        Dim i As Integer
        Dim j As Integer
        Dim Estim As Integer
        Dim Pass As Integer
        Dim Total As Integer
        Dim NBQB As Integer
        'Dim jj As Integer
        Dim Kount As Integer
        Dim kountj As Integer
        Dim NN As Integer
        Dim MM As Integer
        'Dim kc As Integer
        'Dim kq As Integer
        'Dim BQBDC As Double
        'Dim PartM2 As Double
        Dim Sum As Single
        NN = 0
        MM = 0

        'added 053196 eli.
        ReDim AUL(m_data.NumGroups + 10, m_data.NumGroups + 10)
        ReDim Q(m_data.NumGroups + 10)
        ReDim m_data.LHS(m_data.NumGroups, m_data.NumGroups)


        '             Count number of unknown B's and QB's
        '             ------------------------------------
        'jb
        'set the NoBQB() flag
        '1 means B is missing
        '10 means QB and PP are missing 
        '11 means B QB and PP are all missing
        For i = 1 To m_data.NumLiving

            NoBQB(i) = 0
            If m_data.B(i) <= 0 Then
                NoBQB(i) = 1
            End If

            '040112VC Added the check for pproducers below, seems necessary when calling from frmBvary
            If m_data.QB(i) < 0 And m_data.PP(i) < 1 Then
                NoBQB(i) = NoBQB(i) + 10
            End If

        Next i

        'now count the the number of missing B QB 
        'and total missing parameters
        Total = 0 'total number of missing parameter
        NBQB = 0 'total missing  B QB 
        For i = 1 To m_data.NumLiving
            If NoBQB(i) = 11 Then NBQB = NBQB + 1
            If NoBQB(i) > 0 Then Total = Total + 1
        Next i

        Pass = 0
        If NBQB >= 1 Then
            'compute B()  and QB() 
            SolvenoBnoQB(Pass, NBQB)
            If Pass = 1 Then
                'all done
                Exit Sub
            End If
        End If

        If NBQB > 0 Then

            Dim msg As New cMessage(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_INSUFFICIENTDATA, _
                eMessageType.MassBalance_InsufficientData, eMessageSource.EcoPath, eMessageImportance.Warning)
            msg.Suppressable = True

            NotifyCore(msg)
            Exit_Sub_Missing_Par = 0
            Exit Sub
        End If

        For i = 1 To m_data.NumLiving
            If m_data.PB(i) >= 0 And m_data.EE(i) >= 0 Then
                Q(i) = m_data.fCatch(i) + m_data.BA(i) + m_data.Emigration(j) - m_data.Immig(j)
                'vc980303 This was including detritus, don't know why For j = 1 To NumGroups
                For j = 1 To m_data.NumLiving
                    AUL(i, j) = -9999
                    If NoBQB(j) = 11 Then
                        Dim strMsg As String = String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_MISSING_B_QB, m_data.GroupName(j))
                        Dim msg As New cMessage(strMsg, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Warning)
                        NotifyCore(msg)
                        Exit_Sub_Missing_Par = 0
                        Exit Sub
                    End If
                    If NoBQB(j) = 10 And i = j Then Q(i) = Q(i) - m_data.B(i) * m_data.PB(i) * m_data.EE(i)
                    If NoBQB(j) = 1 And i = j Then AUL(i, j) = (m_data.PB(i) * m_data.EE(i) - m_data.QB(j) * m_data.DC(j, i))
                    ' No B:
                    If NoBQB(j) = 1 And i <> j Then AUL(i, j) = -m_data.QB(j) * m_data.DC(j, i)
                    'No QB
                    '031220VC Emigi and BABi now included as rates, will be zero if there are flows (Emigration and BA)
                    Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                    Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                    Sum = Sum * m_data.B(j)
                    If NoBQB(j) = 10 Then AUL(i, j) = -m_data.B(j) * m_data.DC(j, i)
                    If NoBQB(j) = 0 And i <> j Then Q(i) = Q(i) + (m_data.B(j) * m_data.QB(j) * m_data.DC(j, i)) + Sum
                    If NoBQB(j) = 0 And i = j Then Q(i) = Q(i) - m_data.B(j) * (m_data.EE(i) * m_data.PB(j) - m_data.QB(j) * m_data.DC(j, i)) + Sum
                Next j
            End If
        Next i

        'GoSub 7440     'Goto generalized inverse routine
        '             Generalized inverse method
        '             --------------------------
        Kount = 0
        For i = 1 To m_data.NumLiving 'N1 modified 053196 eli.
            Total = 0
            For j = 1 To m_data.NumGroups
                If AUL(i, j) <> -9999 And AUL(i, j) <> 0 Then
                    Total = 1
                    j = m_data.NumGroups + 1
                End If
            Next j

            If m_data.fCatch(i) >= 0 And m_data.PB(i) >= 0 And Total = 1 Then    'GoTo 7620 'OR TD(i) < 0
                Kount = Kount + 1
                H(Kount) = Q(i)
                kountj = 0
                For j = 1 To m_data.NumGroups
                    If AUL(i, j) <> -9999 Then    'GoTo 7610         'EXCL PRIMARY PROD. & DETRITUS
                        kountj = kountj + 1
                        m_data.LHS(Kount, kountj) = AUL(i, j)
                    End If
                Next j
            End If
        Next i

        NN = Kount : MM = kountj
        If NN < MM Then Call ManyUnknown(m_data.NumLiving, NN, MM, NoBQB)
        If NN <> 0 And MM <> 0 Then
            'If g_in_Ranger = 0 And g_in_senseloop = 0 Then
            'frmGIM.Show
            'End If
            Geninv(NN, MM)
            'If g_in_Ranger = 0 And g_in_senseloop = 0 Then
            '        Unload frmGIM
            'End If
            Estim = 1
            'Return
        Else
            Estim = 0
            For j = 1 To m_data.NumGroups
                P(j) = 0
            Next j
        End If



        '             If parameters have been estimated
        '             ---------------------------------
        If Estim = 1 Then
            Kount = 0
            For i = 1 To m_data.NumLiving                     '*** Changed 19 jan 94
                If m_data.PB(i) >= 0 And NoBQB(i) > 0 Then
                    Kount = Kount + 1
                    If NoBQB(i) = 1 Then m_data.B(i) = CSng(P(Kount))
                    If NoBQB(i) = 10 Then m_data.QB(i) = CSng(P(Kount))
                End If
            Next i
        End If

    End Sub


    Private Sub SolvenoBnoQB(ByRef pass As Integer, ByRef NBQB As Integer)
        'Solve equation if B(i) and QB(i) are both unknown

        Dim kc As Integer, kq As Integer
        Dim i As Integer
        Dim sum As Single
        Dim BQBDC As Single
        Dim PartM2 As Single
        Dim msg As cMessage = Nothing

        'jb
        'find the first group that is missing both B QB and PP 
        'this is flaged by the NoBQB(i) = 11 see GIM()
        'kg will be this group
        For i = 1 To m_data.NumLiving
            If NoBQB(i) = 11 Then
                kq = i
                'jb todo  i = m_data.NumLiving is not doing anything
                'i is reset in the next loop
                ' i = m_data.NumLiving
                Exit For
            End If
        Next i

        'jb
        'now find the pray that is not missing any parameters for group kg
        For i = 1 To m_data.NumLiving
            If m_data.mis(i) = 0 And m_data.DC(kq, i) > 0 Then kc = i
        Next i

        If kc = 0 Or m_data.PB(kq) <= 0 Or m_data.EE(kq) <= 0 Then
            'jb changed to return to calling code
            'as was the intent of the original code
            Exit Sub
            'GoTo returnfrom_SolveBnoQB
            'Return 'GOTO ABORT
        End If

        '031220VC: rates or flows,
        sum = CSng(IIf(m_data.BaBi(kc) <> 0 And m_data.BA(kc) = 0, m_data.BaBi(kc), 0))
        sum = sum + CSng(IIf(m_data.Emig(kc) > 0 And m_data.Emigration(kc) = 0, m_data.Emig(kc), 0))
        sum = sum * m_data.B(kc)

        BQBDC = m_data.B(kc) * m_data.PB(kc) * m_data.EE(kc) - m_data.fCatch(kc) - m_data.BA(kc) - m_data.Emigration(kc) + m_data.Immig(kc) - sum
        'Added mig above 15022000 per discussion with Kerim / Villy
        PartM2 = 0
        For i = 1 To m_data.NumLiving
            If i <> kq Then
                If (m_data.DC(i, kq) > 0 Or m_data.DC(i, kc) > 0) Then   'GoTo 6830

                    If m_data.B(i) <= 0 Or m_data.QB(i) < 0 Then
                        i = m_data.NumLiving
                        'jb changed to return to calling code
                        'as was the intent of the original code
                        Exit Sub
                        'GoTo returnfrom_SolveBnoQB
                        'Return 'GoTo 6840
                    End If

                    BQBDC = BQBDC - m_data.B(i) * m_data.QB(i) * m_data.DC(i, kc)
                    PartM2 = PartM2 + m_data.B(i) * m_data.QB(i) * m_data.DC(i, kq)
                End If
            End If
        Next i

        If BQBDC < 0 Then

            Debug.Assert(False, "SolvenoBnoQB")

            'msg = New cMessage(My.Resources.INFORMATION_MISSING_PARAMETERS, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Information)
            'notifyCore(msg)

            Exit_Sub_Missing_Par = 0
            Exit Sub
        End If

        '031220VC, either use BaBi or BA and either Emigi or Emigration
        If (m_data.DC(kq, kc)) > 0 And (m_data.PB(kq) * m_data.EE(kq)) > 0 Then
            sum = CSng(IIf(m_data.BaBi(kq) <> 0 And m_data.BA(kq) = 0, m_data.BaBi(kq), 0))
            sum = sum + CSng(IIf(m_data.Emig(kq) > 0 And m_data.Emigration(kq) = 0, m_data.Emig(kq), 0))
            'Sum is the combined migration and biomass.acc instantaneous mortality rate, will only be non-zero if these are entered
            'B.PB.EE = Pred + NM.B + BAB.B + Catch, hence B = (Pred + Catch)/(PB.EE-NM-BAB)
            m_data.B(kq) = (PartM2 + m_data.BA(kq) + m_data.Emigration(kq) - m_data.Immig(kq) + m_data.fCatch(kq) + m_data.DC(kq, kq) * BQBDC / m_data.DC(kq, kc)) / (m_data.PB(kq) * m_data.EE(kq) - sum)
        End If
        'Added mig above 15022000 per discussion with Kerim / Villy

        If m_data.B(kq) > 0 And m_data.DC(kq, kc) > 0 Then
            m_data.QB(kq) = BQBDC / (m_data.B(kq) * m_data.DC(kq, kc))
        Else
            m_data.B(kq) = -9
        End If

        If m_data.B(kq) > 0 And m_data.QB(kq) >= 0 Then
            NoBQB(kq) = 0
            pass = 1
            NBQB = NBQB - 1
        End If

    End Sub

    Private Sub Geninv(ByVal NN As Integer, ByVal MM As Integer)
        Dim t As Single, i As Integer, j As Integer, L As Integer, lhsi As Integer, K As Single, d As Single
        'jb are these local?????????
        'I hope so because they are now
        Dim Z(,) As Single, W(,) As Single, Y(,) As Single

        ReDim Z(m_data.NumGroups, m_data.NumGroups)
        ReDim W(m_data.NumGroups, m_data.NumGroups)
        ReDim Y(m_data.NumGroups, m_data.NumGroups)


        'StrLong$ = " Geninv routine" & Chr$(13) & Chr$(13)
        ' LOCATE 16, 12: Print "              2*N + trace =";

        'StrLong$ = StrLong$ & "Calculating parameters using generalized inverse method" & Chr$(13)
        'StrLong$ = StrLong$ & "Optimizing until trace of matrix is near integer"
        ' NN is the number of equations
        ' MM is the number of unknowns
        For lhsi = 1 To NN
            For j = 1 To MM
                W(j, lhsi) = m_data.LHS(lhsi, j)                'W is transpose of LHS
                '    LPRINT USING " ####.####"; lhs(lhsi, j);
            Next j                            'RightHandSide of equation
            ' LPRINT H(lhsi)
        Next lhsi

        K = 0
        For i = 1 To NN
            For j = 1 To NN
                Z(i, j) = 0
                For L = 1 To MM
                    Z(i, j) = Z(i, j) + m_data.LHS(i, L) * W(L, j)
                Next L
                K = K + Math.Abs(Z(i, j))
            Next j
        Next i

        K = 1 / K
        d = 0.00001                                       'small constant
        For i = 1 To MM
            For j = 1 To NN
                Y(i, j) = K * W(i, j)                  'first approximation to inverse
            Next j
        Next i

ONE:

        For i = 1 To NN
            For j = 1 To NN
                Z(i, j) = 0
                For L = 1 To MM
                    Z(i, j) = Z(i, j) + m_data.LHS(i, L) * Y(L, j)
                Next L
            Next j
        Next i

        t = 0                                    'Trace=T
        For i = 1 To NN
            Z(i, i) = Z(i, i) - 2
            t = t + Z(i, i)
        Next i
        'LOCATE 16, 40: Print 2 * NN + T; Spc(10);

        'If g_in_Ranger = 0 And g_in_senseloop = 0 Then
        '    'frmGIM.Label1 = Format(2 * NN + T, " ###.###### ")
        '    'frmGIM.Refresh
        'End If

        For i = 1 To MM
            For j = 1 To NN
                W(i, j) = 0
                For L = 1 To NN
                    W(i, j) = W(i, j) + Y(i, L) * Z(L, j)
                Next L
            Next j
        Next i
        For i = 1 To MM
            For j = 1 To NN
                Y(i, j) = -W(i, j)
            Next j
        Next i
        If Math.Abs(t - Int(t) - 1) >= d And Math.Abs(t - Int(t)) >= d Then GoTo ONE
        'Repeat until T is an integer

        'P is the solutions to the equations
        'Y is the generalized inverse
        For i = 1 To MM
            P(i) = 0
            For j = 1 To NN
                P(i) = P(i) + Y(i, j) * H(j)
                'If Y(i, j) < 0 Then
                '
                'End If
            Next j
        Next i
        Erase Z, W   '- test for compilation
    End Sub

    Private Sub EstimateQBorB_2(ByVal Pass As Integer)
        'The following is a routine made by VC in March 1994 to estimate
        'QB Or B independent of the Generalized Inverse. It works in cases
        'where for a given prey j the PB, B, EE is known and where
        'the only unknown predation is due to one predator j
        'whose B or QB is unknown.

        Dim i As Integer, j As Integer
        Dim Cnt As Integer
        Dim LeftProd As Single
        Dim MisQ As Integer
        Dim Sum As Single
        For j = 1 To m_data.NumLiving                'j is the prey
            Pass = 0                       'Estimate QB or B
            Cnt = 0
            If m_data.B(j) > 0 And m_data.PB(j) > 0 And m_data.EE(j) > 0 Then
                '031220VC, emig and ba as rates
                Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                Sum = Sum * m_data.B(j)

                LeftProd = m_data.B(j) * m_data.PB(j) * m_data.EE(j) - m_data.fCatch(j) - Sum - m_data.BA(j) - m_data.Emigration(j) + m_data.Immig(j)
                'Added mig above 15022000 per discussion with Kerim / Villy
                For i = 1 To m_data.NumLiving             'i is the predator without Q
                    If m_data.DC(i, j) > 0 Then
                        If m_data.B(i) <= 0 Or m_data.QB(i) < 0 Then
                            Cnt = Cnt + 1
                            MisQ = i
                        Else
                            LeftProd = LeftProd - m_data.B(i) * m_data.QB(i) * m_data.DC(i, j)
                        End If
                    End If
                Next i
            End If
            If Cnt = 1 Then
                If m_data.QB(MisQ) < 0 And m_data.B(MisQ) > 0 And LeftProd > 0.0001 Then
                    m_data.QB(MisQ) = LeftProd / m_data.B(MisQ) / m_data.DC(MisQ, j)
                    Pass = 1
                ElseIf m_data.B(MisQ) < 0 And m_data.QB(MisQ) > 0 And LeftProd > 0.0001 Then
                    m_data.B(MisQ) = LeftProd / m_data.QB(MisQ) / m_data.DC(MisQ, j)
                    Pass = 1
                End If
            End If
        Next j
    End Sub

    Private Sub ManyUnknown(ByVal NumLiving As Integer, ByVal NN As Integer, ByVal MM As Integer, ByVal NoBQB() As Integer)
        'Static showGenMess As Integer
        Dim i As Integer
        Dim strMsg As String
        Dim msg As cMessage = Nothing

        'TODo_jb ManyUnknown InParameterEstimation has not been set
        If InParameterEstimation = 0 Then
            Exit Sub
        End If

        ' ToDo_JS: Localize this
        strMsg = "The generalized inverse routine is trying to estimate " & MM & " unknown "
        strMsg = strMsg & "from " & NN & " equations. The solution will not be unique. The unknown(s) are:" & vbCrLf
        For i = 1 To NumLiving
            If NoBQB(i) = 1 Then strMsg$ = strMsg$ & " B  for group " & i & vbCrLf
            If NoBQB(i) = 10 Then strMsg$ = strMsg$ & "Q/B for group " & i & vbCrLf
        Next i
        strMsg = strMsg & vbCrLf & "Check the estimated values carefully."
        msg = New cMessage(strMsg, eMessageType.TooManyMissingParameters, eMessageSource.EcoPath, eMessageImportance.Warning)
        msg.Suppressable = False
        NotifyCore(msg)

    End Sub

    Private Sub MsgManyMissingPar(ByVal i As Integer)
        Dim strMsg As String
        Dim msg As cMessage = Nothing

        Try
            ' ToDo_JS: Localize this
            strMsg = "The parameter estimation routine can work only with one of B, P/B, and EE unknown per group. "
            strMsg = strMsg & "Here, more than one of these are unknown for " & m_data.GroupName(i) & "." 'group " & i% & "."
            strMsg = strMsg & vbCrLf & vbCrLf
            strMsg = strMsg & "In addition, the Q/B may be unknown for a given predator, i.e., IF: "
            strMsg = strMsg & "B, PB, QB and EE are known for one of its prey, and IF: all groups that prey on "
            strMsg = strMsg & "these two groups have known B and QB."
            strMsg = strMsg & vbCrLf & vbCrLf
            strMsg = strMsg & "Please re-edit the input parameters."

            msg = New cMessage(strMsg, eMessageType.TooManyMissingParameters, eMessageSource.EcoPath, eMessageImportance.Warning)
            msg.Suppressable = False
            NotifyCore(msg)
            ' MsgBox(StrLong, vbCritical + vbOKOnly, "Parameter estimation failed")

        Catch ex As Exception
            cLog.Write("Error in MsgManyMissingPar(). Error: " + ex.Message())
            Debug.Assert(False)
        End Try
    End Sub

    ''' <summary>
    ''' Tell the core to send this message
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks>Wraps the delegate instance that is use to notify the core of a message</remarks>
    Private Sub NotifyCore(ByVal msg As cMessage)

        Try

            If Not m_msgPub Is Nothing Then
                'm_coreNotifier is a Delegate instance that was set by the core via the cEcoPath.CoreNotifierDelegate property
                'it will pass the message to the core where it can be processed and sent to an interface
                m_msgPub.SendMessage(msg)
            End If

        Catch ex As Exception
            cLog.Write("EcopathMassBalance.notifyCore(...) unable to call core delegate.")
        End Try

    End Sub


    Private Sub EstimateTrophicLevels(ByVal Diet(,) As Single, ByVal TLreturn() As Single)
        Dim i As Integer, j As Integer
        Dim ErrCode As Integer

        Dim TL() As Single
        ReDim TL(m_data.NumGroups)

        For i = 1 To m_data.NumGroups
            'TTLX(i) = 1
            TL(i) = 1
            For j = 1 To m_data.NumGroups
                m_data.LHS(i, j) = 0
            Next j
        Next i

        For i = 1 To m_data.NumGroups
            m_data.SumDC(i) = 0
            For j = 1 To m_data.NumGroups
                m_data.SumDC(i) = m_data.SumDC(i) + Diet(i, j)
            Next j
        Next i

        'Estimation of trophic levels: TTLX
        'The DC is made to sum to one, this means that it is assumed
        'that import to strict consumers has the same trophic level as
        'other prey for the group
        For i = 1 To m_data.NumGroups
            For j = 1 To m_data.NumGroups
                If m_data.PP(i) = 1 Then            'Strict Primary producer, so no diet composition (even if it may have in carbon model)
                    m_data.LHS(i, j) = 0
                ElseIf m_data.PP(i) > 0 Then            'partly a primary producer
                    m_data.LHS(i, j) = -Diet(i, j)
                    'ElseIf SumDC(i) > 0 And SumDC(i) < 1 Then 'Consumer with import
                ElseIf m_data.SumDC(i) > 0 And Math.Abs(m_data.SumDC(i) - 1) > 0.0001 Then 'Consumer with import
                    m_data.LHS(i, j) = -Diet(i, j) / m_data.SumDC(i)
                Else                          'Consumer
                    m_data.LHS(i, j) = -Diet(i, j)
                End If
                If m_data.PP(i) > 0 And m_data.PP(i) < 1 Then
                    'Mixed producer / consumer: TTLX should reflect both roles
                    m_data.LHS(i, j) = -Diet(i, j) * (1 - m_data.PP(i))
                End If
            Next j
            m_data.LHS(i, i) = 1 - Diet(i, i)
        Next i

        For i = m_data.NumLiving + 1 To m_data.NumGroups          'multidet version for
            For j = 1 To m_data.NumGroups
                m_data.LHS(i, j) = 0
            Next j
            m_data.LHS(i, i) = 1
        Next i

        ErrCode = MatSEqnS(m_data.LHS, TL)   'Inverses matrix to find

        If ErrCode = 0 Then 'no error
            For i = 1 To m_data.NumGroups : TLreturn(i) = TL(i) : Next
        End If

    End Sub


    Private Function CheckPredatorPreyTrophicLevels() As Boolean
        Dim i As Integer
        Dim j As Integer
        Dim dcsum As Single
        Dim RetVal As cFeedbackMessage.eReply = cFeedbackMessage.eReply.CANCEL
        Static done As Boolean
        Static DoneAlready As Boolean

        'ToDO_jb test CheckPredatorPreyTrophicLevels

        'JB WARNING
        'THIS HAS NEVER BEEN TESTED!!!!!!!!!!!!!!!!!!
        'It was just copied from EWE5 this could be dangerous

        If DoneAlready = False Then
            EstimateTrophicLevels(m_data.DC, m_data.TTLX)
            'Now check if any group gets more than 15% of its consumption from a group that has >= m_data.ttlx as itself:
            For i = 1 To m_data.NumLiving  'Consumers only
                dcsum = 0
                For j = 1 To m_data.NumLiving 'only living groups can have a higher TL
                    If (m_data.DC(i, j) > 0.0!) Then 'i eats j
                        If m_data.TTLX(i) <= m_data.TTLX(j) Then 'prey has higher TL
                            dcsum = dcsum + m_data.DC(i, j)
                        End If
                    End If
                Next
                If dcsum > 0.151 Then  'there are some culprits with high consumption of
                    If done = False Then

                        ' Prepare message
                        Dim strMsg As String = String.Format(My.Resources.CoreMessages.DIETCOMP_PROMPT_CORRECTTO15PERC, m_data.GroupName(i), CInt(dcsum * 100))
                        Dim msg As New cFeedbackMessage(strMsg, eMessageSource.EcoPath, eMessageImportance.Critical, cFeedbackMessage.eReplyStyle.YES_NO_CANCEL)
                        msg.Suppressable = True
                        msg.Type = eMessageType.DietComp_CorrectTo15Perc

                        ' Send message
                        NotifyCore(msg)
                        RetVal = msg.Reply
                        If RetVal = cFeedbackMessage.eReply.CANCEL Then done = True

                    Else
                        RetVal = cFeedbackMessage.eReply.NO
                    End If
                    If RetVal = cFeedbackMessage.eReply.YES Then
                        bDietsModified = True
                        DoneAlready = True
                        For j = 1 To m_data.NumGroups
                            If m_data.DC(i, j) > 0 Then
                                'Debug.Print i, j, m_data.ttlx(i), m_data.ttlx(j), m_data.dc(i, j),
                                If m_data.TTLX(i) <= m_data.TTLX(j) Then
                                    m_data.DC(i, j) = CSng(m_data.DC(i, j) * 0.15 / dcsum)
                                Else
                                    m_data.DC(i, j) = CSng(m_data.DC(i, j) * 0.85 / (1 - dcsum))
                                End If
                                'Debug.Print m_data.dc(i, j)
                            End If
                        Next
                        m_data.DC(i, 0) = CSng(m_data.DC(i, 0) * 0.85 / (1 - dcsum))
                    End If
                End If
            Next
            If DoneAlready = True Then
                checkDietsSumToOne(True)
            Else
                DoneAlready = True  'this has to be done, so it won't repeat this circus
            End If
        End If
    End Function


    Private Function DoIterationsToEstimateB() As Integer
        'VC programmed this function on 12 March 2002 to be able to estimate B for groups that are cannibalistic
        Dim i As Integer
        Dim j As Integer
        Dim Cnt As Long
        Dim GuessedBiomass() As Boolean
        Dim BIter() As Double   'B iterated
        Dim NewSum As Single
        Dim OldSum As Single
        Dim Only As Double
        Dim PartM2 As Double
        Dim NewB As Double
        Dim MaxBio As Double
        Dim Sum As Single
        'On Local Error GoTo exitSub
        'Dim NegativeB() As Boolean
        ReDim BIter(m_data.NumLiving)
        'ReDim NegativeB(m_data.numliving)
        ReDim GuessedBiomass(m_data.NumLiving)
        'This is based on EstimateB

        Try

            For j = 1 To m_data.NumLiving 'If we only lack the Biomass then let's try to guess it
                If m_data.B(j) <= 0 And m_data.PB(j) > 0 And m_data.QB(j) > 0 And m_data.EE(j) > 0 Then
                    GuessedBiomass(j) = True
                Else
                    If m_data.B(j) > MaxBio Then MaxBio = m_data.B(j)
                End If
            Next
            MaxBio = CDbl(IIf(MaxBio > 0, 10 * MaxBio, 100))
            DoIterationsToEstimateB = 0
            NewSum = 0
            OldSum = -1
            Cnt = 0
            Do While Math.Abs(NewSum - OldSum) > 0.000001 And Cnt < 1000
                OldSum = NewSum
                NewSum = 0
                Cnt = Cnt + 1
                For j = 1 To m_data.NumLiving
                    'If j = 2 Then Stop
                    If GuessedBiomass(j) Then   'Only do something if the biomass has been guessed
                        PartM2 = 0  'partM2 because cannibalism is excluded from calc.
                        For i = 1 To m_data.NumLiving
                            If m_data.DC(i, j) > 0 And i <> j Then 'this group, i, is a consumer,
                                If m_data.QB(i) < 0 Then  'we don't know the qb of this consumer so it won't work
                                    GoTo nextJ
                                ElseIf GuessedBiomass(i) Then
                                    PartM2 = PartM2 + m_data.QB(i) * m_data.DC(i, j) * BIter(i)
                                Else
                                    PartM2 = PartM2 + m_data.QB(i) * m_data.DC(i, j) * m_data.B(i)
                                End If
                            End If
                        Next i
                        '031220VC: modified to incorporate that BioAcc and emigration can be rates
                        Sum = CSng(IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0))
                        Sum = Sum + CSng(IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0))
                        Sum = Sum * m_data.B(j)
                        Only = m_data.PB(j) * m_data.EE(j) - m_data.QB(j) * m_data.DC(j, j) - Sum
                        If Only > 0 Then
                            NewB = (m_data.fCatch(j) + m_data.BA(j) + m_data.Emigration(j) - m_data.Immig(j) + PartM2) / Only
                            If NewB > MaxBio Then NewB = MaxBio
                            If NewB > 0 And Cnt > 4 And Math.Abs(NewB - BIter(j)) < 10 ^ -6 * BIter(j) And BIter(j) > 10 ^ -7 Then
                                'get the biomasses that are OK
                                GuessedBiomass(j) = False
                                DoIterationsToEstimateB = DoIterationsToEstimateB + 1
                                m_data.B(j) = CSng(NewB)
                            End If
                            If NewB = 0 Then Return 0
                            BIter(j) = NewB
                            'If NewB > 100 Then Stop
                            'Debug.Print j; BIter(j);
                            If BIter(j) = 0 Then BIter(j) = 10 ^ -8
                        Else    'Only is negative = bad EE or too high cannibalism.
                            'If NegativeB(j) = False Then
                            '    MsgBox "Cannibalism exceeds production (P/B * EE > QB * DCii) for group " + CStr(j) + ", " + Specie(j)
                            '    NegativeB(j) = True
                            'End If
                            BIter(j) = 0
                        End If
                        If BIter(j) > 0 Then
                            NewSum = CSng(NewSum + BIter(j))
                        End If
                    End If
nextJ:
                Next j
                'Debug.Print
            Loop
            'DoIterationsToEstimateB = True
            'So transfer the values we have obtained through iteration
            For i = 1 To m_data.NumLiving
                If BIter(i) > 10 ^ -7 Then m_data.B(i) = CSng(BIter(i))
            Next
        Catch ex As Exception
            Return 0
        End Try

    End Function



End Module
