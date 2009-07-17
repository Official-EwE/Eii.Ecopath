Option Strict Off

Imports EwECore

Public Class MassBalance

    Private m_core As cCore = Nothing
    Private m_data As cEcopathDataStructures
    Private mis() As Integer
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

    'private copy of public Properties
    Private m_in_estimation_loop As Integer 'currently in the parameter estimation loop 0 = false 1 = true
    Private m_missing_param As Integer 'exit parameter estimation sub (ParamEstimate(...)) because number of missing parameter > 2 

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

    Public Function Calculate(ByVal p_EcoPathDataStructures As Object, ByVal p_EstimateFor As Integer) As eStatusFlags

        ' Sanity checks
        Debug.Assert(TypeOf p_EcoPathDataStructures Is cEcopathDataStructures)

        Dim ji As Integer
        'Dim From As Integer
        Dim LoopC As Integer, Pass As Integer, SecLoop As Integer
        Dim noMissing As Integer
        Dim ExitSen As Boolean
        Dim EstimateFor As eEstimateParameterFor
        Dim msg As cMessage = Nothing

        m_data = DirectCast(p_EcoPathDataStructures, cEcopathDataStructures)
        EstimateFor = DirectCast(p_EstimateFor, eEstimateParameterFor)

        RedimVariables()
        'Programmer: Villy Christensen
        'This is the main module for parametrization, i.e. estimation of 'missing parameter'
        'for securing mass balance
        Try

            Exit_Sub_Missing_Par = 1

Start:
            LoopC = 0
            CountMissingB_Ex()

LoopCalc:
            LoopC = LoopC + 1
            'exit strategies if the loop has executed to many times
            If LoopC > m_data.NumGroups + 2 Then

                If EstimateFor = eEstimateParameterFor.ParameterEstimation Then
                    'jb todo
                    'this exit strategy has not been implemented yet
                    Debug.Assert(False, "Exit strategy for Parameter Estimation in this case has not been implemented yet." & vbCrLf & "EcoPath model results may not reliable.")
                    Exit_Sub_Missing_Par = 0
                    InParameterEstimation = 0
                    Return CType(eStatusFlags.ErrorEncountered, Integer)


                ElseIf EstimateFor = eEstimateParameterFor.Sensitivity Then

                    msg = New cMessage(My.Resources.INFORMATION_TOO_MANY_LOOPS, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Information)
                    Me.m_core.Messages.AddMessage(msg)

                    Exit_Sub_Missing_Par = 0
                    InParameterEstimation = 0
                    Return CType(eStatusFlags.ErrorEncountered, Integer)

                End If 'If EstimateFor = eEstimateParameterFor.ParameterEstimation Then
            End If ' If LoopC > m_data.NumGroups + 2 Then


            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'jb
            'The following commented out code has exit strategies  
            'from ParamEstimate1 and SensitivLoop
            'for when the loop has executed to many times
            '
            'FROM ParamEstimate1
            'If LoopC% > NumGroups + 2 Then
            '    If CyclesDone = False Then
            '        FindCyclesWhenEstimatingBiomass(Cons())
            '        CyclesDone = True
            '        GoTo Start
            '    ElseIf CheckPredatorPreyTrophicLevels Then
            '        'Check if there are crazy values with high consumption of prey with higher TL:
            '        GoTo Start
            '    ElseIf DoIterationsToEstimateB <= 3 Then 'Try to do iterations
            '        GoTo Start
            '    Else
            '        Exit_Sub_Missing_Par = 0
            '        InParameterEstimation = 0
            '        Exit Function
            '    End If
            'End If

            'From SensitivLoop
            'If LoopC > m_data.NumGroups + 2 Then
            '    MsgBox("Too many loops. Quitting Parameter Estimation.")
            '    Exit_Sub_Missing_Par = 0
            '    InParameterEstimation = 0
            '    Return False
            'End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            EstimateGE()

            CountNoOfMissing(mis, noMissing, "ParameterEstimate")
            If Exit_Sub_Missing_Par = 0 Then
                InParameterEstimation = 0
                'jb 
                cLog.Write("Too many missing parameters. Parameter Estimation not completed successfully.")
                Return CType(eStatusFlags.MissingParameter, Integer)
            End If
            '040112VC: In case B is missing, BABi is entered, then estimate BA(i) again
            For ji = 1 To m_data.NumLiving
                If m_data.BA(ji) = 0 Then
                    m_data.BA(ji) = IIf(m_data.BaBi(ji) <> 0 And m_data.B(ji) > 0, m_data.BaBi(ji) * m_data.B(ji), m_data.BA(ji))
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
                    ' jb todo
                    'this has not been implemented yet so just assert for now
                    'see comment out code below for original behaviour
                    Debug.Assert(False)
                    Return CType(eStatusFlags.ErrorEncountered, Integer)
                End If

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'jb
                'Exit stratagy ParamEstimate1 
                'If Exit_Sub_Missing_Par = 0 Or SecLoop% = 3 Then
                '    If CyclesDone = False Then
                '        FindCyclesWhenEstimatingBiomass(Cons())
                '        If DietsModified Then checkDietsSumToOne(True)
                '        CyclesDone = True
                '        GoTo Start
                '    ElseIf CheckPredatorPreyTrophicLevels Then
                '        'Check if there are crazy values with high consumption of prey with higher TL:
                '        GoTo Start
                '    ElseIf DoIterationsToEstimateB <= 3 And TimesTried <= 10 Then 'Try to do iterations
                '        TimesTried = TimesTried + 1
                '        GoTo Start
                '    Else
                '        Exit_Sub_Missing_Par = 0
                '        InParameterEstimation = 0
                '        Exit Sub
                '    End If
                'End If
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


                'in the original code  EstimateB(Pass, from) could set "From" to -1
                'which would cause the loop to exit
                'This has been changed to set a boolean flag 'ExitSen'
                ''ExitSen' should only get set when called with EstimateFor =  eEstimateParameterFor.Sensitivity
                'see EstimateB()
                'If From = -1 Then
                If ExitSen Then
                    'GoTo NextSensL ' Couldn't est parameters
                    ' Debug.Assert(False)
                    Exit Function
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
                    Debug.Assert(False)
                    Return CType(eStatusFlags.ErrorEncountered, Integer)
                End If
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

                'Enter Generalized Inverse Method
                Pass = 0
                For ji = 1 To m_data.NumLiving                  'GIM
                    If mis(ji) > 0 Then           'IF EQ2=0 THEN B,PB,QB,EE known
                        'If Mis(ji) > 1 Then           'IF EQ2=0 THEN B,PB,QB,EE known
                        GIM()

                        If Exit_Sub_Missing_Par = 0 Then
                            InParameterEstimation = 0
                            Return CType(eStatusFlags.MissingParameter, Integer)
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
            cLog.Write(Me.ToString + ".ParamEstimate() Error while Estimating Parameters. Error: " + ex.Message)
            Return CType(eStatusFlags.ErrorEncountered, Integer)
        End Try

        Return CType(eStatusFlags.OK, Integer)
    End Function

    Private Sub RedimVariables()

        ReDim mis(m_data.NumGroups)
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

    Private Sub CountNoOfMissing(ByRef Mis() As Integer, ByRef NoMissing As Integer, ByVal From As String)
        'Private Sub CountNoOfMissing(ByRef Mis() As Integer, ByRef NoMissing As Integer, ByVal From As String, ByVal chk As Integer)

        'count the number of missing parameters for each group and store the values in the argument Mis()
        'this will have to change because the basic estimator and the Sensitivity loop count the number of missing parameters differently
        Dim Miss As Integer
        Dim i As Integer
        Static done As Boolean

        NoMissing = 0

        For i = 1 To m_data.NumLiving
            Miss = 0
            If m_data.B(i) <= 0 Then Miss = Miss + 1
            If m_data.PB(i) < 0 Then Miss = Miss + 1
            If m_data.EE(i) < 0 Then Miss = Miss + 1

            If Miss >= 2 And From = "ParameterEstimate" Then
                MsgManyMissingPar(i)
                Exit_Sub_Missing_Par = 0
                cLog.Write("'CountNoOfMissing(...)' Group " & i & " missing more then one parameter.")
                Exit Sub
            End If

            If m_data.QB(i) < 0 And m_data.PP(i) < 1 Then
                Miss = Miss + 1
            End If


            If Miss >= 2 And From = "SensitivLoop" Then
                ' From Sensitivity routine
                ' chk = 1
                If done = False Then
                    'jb todo
                    'MsgManyMissingSens(i)
                    done = True
                End If

                Exit_Sub_Missing_Par = 0
            End If

            Mis(i) = Miss
            ''DoEvents
            NoMissing = NoMissing + Miss
        Next i
    End Sub


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
                            Sum = IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0)
                            Sum = Sum + IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0)
                            Sum = Sum * m_data.B(j)
                            m_data.PB(j) = (MM2 + Sum + m_data.BA(j) + m_data.Emigration(j) - m_data.Immig(j) + m_data.fCatch(j)) / (m_data.B(j) * m_data.EE(j))
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
                Sum = IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0)
                Sum = Sum + IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0)
                Sum = Sum * m_data.B(j)

                If m_data.B(j) * m_data.PB(j) > 0 Then
                    m_data.EE(j) = (MM2 + Sum + m_data.Emigration(j) - m_data.Immig(j) + m_data.BA(j) + m_data.fCatch(j)) / (m_data.B(j) * m_data.PB(j))
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

        For j = 1 To m_data.NumLiving
            Pass = 0 'Estimate B
            If m_data.PB(j) > 0 And m_data.EE(j) > 0 And m_data.B(j) <= 0 And mis(j) = 1 Then
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
                Sum = IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0)
                Sum = Sum + IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0)
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
                m_data.B(j) = (m_data.fCatch(j) + m_data.BA(j) + m_data.Emigration(j) - m_data.Immig(j) + PartM2) / Only
                'Added mig above 15022000 per discussion with Kerim / Villy
                If m_data.B(j) > 0 Then
                    Pass = 1
                    Exit Sub
                ElseIf Not CancelPressed Then
                    If m_data.B(j) = 0 Then

                        msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_ESTIMATE_BM_0, CStr(j), m_data.GroupName(j)), _
                                eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Maintenance, eDataTypes.NotSet, cFeedbackMessage.eFeedbackType.OK_CANCEL)
                        m_core.Messages.SendMessage(msg)
                        CancelPressed = (msg.Reply = 0)

                    End If
                    If m_data.B(j) < 0 Then
                        ' ToDo_JS: Change message box behaviour to run via Core messages (as above)
                        'If Only < 0 Then
                        '    Answer = MsgBox("Ecopath has estimated the production * EE for group " + CStr(j) + ", " + m_data.GroupName(j) + ", to be (" & Only.ToString("0.000") & "). This is less than the estimated 'cannibalism' for this group.", vbInformation + vbOKCancel, "Ecopath parameter estimation failed")
                        'Else
                        '    Answer = MsgBox("Ecopath has estimated the biomass for group " + CStr(j) + ", " + m_data.GroupName(j) + ", to be (" & m_data.B(j).ToString( "0.000")) & "). This is because the fishery + migration + biomass accumulation is estimated to sum to a negative value. Please edit your data, allowing e.g. for biomass accumulation for this group.", vbInformation + vbOKCancel, "Ecopath parameter estimation failed")
                        'End If
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
        Dim RetVal As Long
        Dim Answer As Single
        Static done As Boolean

        If done = False Then
            Dim msg As cFeedbackMessage = Nothing

            ' ToDo_JS: Localize this
            str = "Your data are not consistent. In algorithm 4 your estimate of: P/Bi * EEi - Q/Bi * DCii is negative "
            str = str & "for group " & j & ", i.e. 'cannibalism' exceeds the predation mortality."
            str = str & vbNewLine + vbNewLine
            str = str & "See the description of Algorithm 4 in Appendix 4."
            str = str & vbNewLine + vbNewLine
            str = str & "Do you want to have cannibalism reduced (to 20 of used production) for all groups where this problem occurs. (Note: your input data will not be changed)"
            ' RetVal = MsgBox(str, vbQuestion + vbYesNoCancel)

            msg = New cFeedbackMessage(str, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance, eDataTypes.NotSet, cFeedbackMessage.eFeedbackType.YES_NO_CANCEL)
            m_core.Messages.SendMessage(msg)
            RetVal = CLng(msg.Reply)

        End If
        If RetVal = vbCancel Then done = True
        Abort4 = False

        If RetVal = vbYes Then
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
            RetVal = checkDietsSumToOne(True)
            Abort4 = True
        Else
            ' ToDo_JS: Remove this message box, and figure out what text to display. The current message text is not overly informative.
            MsgBox("Please edit your data.")
        End If

exitSub:
    End Function

    Private Function checkDietsSumToOne(ByVal NoQuestionsAsked As Boolean) As Boolean
        Dim pred As Integer
        Dim prey As Integer
        Dim Sum As Single
        Dim RetVal As Object
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
                        RetVal = vbYes
                    ElseIf briefQuestion Then
                        ' first time only
                        briefQuestion = False

                        msg = New cFeedbackMessage(My.Resources.PROMPT_NORMALIZE_DIET_ALL, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                        Me.m_core.Messages.SendMessage(msg)
                        RetVal = msg.Reply

                        If RetVal = vbYes Then
                            NoQuestionsAsked = True
                        Else

                            ' check rest individually
                            msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_NORMALIZE_DIET, Pred, Sum), _
                                     eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                            Me.m_core.Messages.SendMessage(msg)
                            RetVal = msg.Reply

                        End If
                    Else
                        ' dialog for each group (slow way)
                        msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_NORMALIZE_DIET, Pred, Sum), _
                                 eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Maintenance)
                        Me.m_core.Messages.SendMessage(msg)
                        RetVal = msg.Reply

                        'Text = Text & vbNewLine & vbNewLine & "Raising will only affect the calculations, not change the input. "
                        'Text = Text & vbNewLine & "To change your input data, select 'No' below, open the"
                        'Text = Text & vbNewLine & "diet input form, and select the button for raising. "

                    End If

                    'Normalize the diet for pred
                    If RetVal = vbYes Then
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
                            Sum = IIf(m_data.BaBi(i) <> 0 And m_data.BA(i) = 0, m_data.BaBi(i), 0)
                            Sum = Sum + IIf(m_data.Emig(i) > 0 And m_data.Emigration(i) = 0, m_data.Emig(i), 0)
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
            ' ToDo_JS: Remove message box!
            MsgBox("Insufficient Data, Reedit data.")
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
                        ' ToDo_JS: Remove message box!
                        MsgBox("Aborting, Missing B and QB for group " & CStr(j) & ". Please re-edit data.")
                        Exit_Sub_Missing_Par = 0
                        Exit Sub
                    End If
                    If NoBQB(j) = 10 And i = j Then Q(i) = Q(i) - m_data.B(i) * m_data.PB(i) * m_data.EE(i)
                    If NoBQB(j) = 1 And i = j Then AUL(i, j) = (m_data.PB(i) * m_data.EE(i) - m_data.QB(j) * m_data.DC(j, i))
                    ' No B:
                    If NoBQB(j) = 1 And i <> j Then AUL(i, j) = -m_data.QB(j) * m_data.DC(j, i)
                    'No QB
                    '031220VC Emigi and BABi now included as rates, will be zero if there are flows (Emigration and BA)
                    Sum = IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0)
                    Sum = Sum + IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0)
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
                    If NoBQB(i) = 1 Then m_data.B(i) = P(Kount)
                    If NoBQB(i) = 10 Then m_data.QB(i) = P(Kount)
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
            If mis(i) = 0 And m_data.DC(kq, i) > 0 Then kc = i
        Next i

        If kc = 0 Or m_data.PB(kq) <= 0 Or m_data.EE(kq) <= 0 Then
            'jb changed to return to calling code
            'as was the intent of the original code
            Exit Sub
            'GoTo returnfrom_SolveBnoQB
            'Return 'GOTO ABORT
        End If

        '031220VC: rates or flows,
        sum = IIf(m_data.BaBi(kc) <> 0 And m_data.BA(kc) = 0, m_data.BaBi(kc), 0)
        sum = sum + IIf(m_data.Emig(kc) > 0 And m_data.Emigration(kc) = 0, m_data.Emig(kc), 0)
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

            msg = New cMessage(My.Resources.INFORMATION_MISSING_PARAMETERS, eMessageType.Any, eMessageSource.EcoPath, eMessageImportance.Information)
            Me.m_core.Messages.AddMessage(msg)

            Exit_Sub_Missing_Par = 0
            Exit Sub
        End If

        '031220VC, either use BaBi or BA and either Emigi or Emigration
        If (m_data.DC(kq, kc)) > 0 And (m_data.PB(kq) * m_data.EE(kq)) > 0 Then
            sum = IIf(m_data.BaBi(kq) <> 0 And m_data.BA(kq) = 0, m_data.BaBi(kq), 0)
            sum = sum + IIf(m_data.Emig(kq) > 0 And m_data.Emigration(kq) = 0, m_data.Emig(kq), 0)
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
        Dim t As Integer, i As Integer, j As Integer, L As Integer, lhsi As Integer, K As Object, d As Single
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
                Sum = IIf(m_data.BaBi(j) <> 0 And m_data.BA(j) = 0, m_data.BaBi(j), 0)
                Sum = Sum + IIf(m_data.Emig(j) > 0 And m_data.Emigration(j) = 0, m_data.Emig(j), 0)
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
        Dim StrLong As String
        If InParameterEstimation = 0 Then
            Exit Sub
        End If

        ' ToDo_JS: Localize this
        StrLong = "The generalized inverse routine is trying to estimate " & MM & " unknown "
        StrLong = StrLong & "from " & NN & " equations. The solution will not be unique. The unknown(s) are:" & vbCrLf
        For i = 1 To NumLiving
            If NoBQB(i) = 1 Then StrLong$ = StrLong$ & " B  for group " & i & vbCrLf
            If NoBQB(i) = 10 Then StrLong$ = StrLong$ & "Q/B for group " & i & vbCrLf
        Next i
        StrLong = StrLong & vbCrLf & "Check the estimated values carefully."

    End Sub

    Private Sub MsgManyMissingPar(ByVal i As Integer)
        Dim StrLong As String

        Try
            ' ToDo_JS: Remove message box!
            StrLong = "The parameter estimation routine can work only with one of B, P/B, and EE unknown per group. "
            StrLong = StrLong & "Here, more than one of these are unknown for " & m_data.GroupName(i) & "." 'group " & i% & "."
            StrLong = StrLong & vbCrLf & vbCrLf
            StrLong = StrLong & "In addition, the Q/B may be unknown for a given predator, i.e., IF: "
            StrLong = StrLong & "B, PB, QB and EE are known for one of its prey, and IF: all groups that prey on "
            StrLong = StrLong & "these two groups have known B and QB."
            StrLong = StrLong & vbCrLf & vbCrLf
            StrLong = StrLong & " Please re-edit the input parameters."
            MsgBox(StrLong, vbCritical + vbOKOnly, "Parameter estimation failed")

        Catch ex As Exception
            cLog.Write("Error in MsgManyMissingPar(). Error: " + ex.Message())
            Debug.Assert(False)
        End Try
    End Sub

    Public Property Exit_Sub_Missing_Par() As Integer ' Implements IEcoPathParameterEstimator.Exit_Sub_Missing_Par
        Get
            Exit_Sub_Missing_Par = m_missing_param
        End Get

        Set(ByVal value As Integer)
            m_missing_param = value
        End Set

    End Property

    Public Property InParameterEstimation() As Integer ' Implements IPlugIns.IEcoPathParameterEstimator.InParameterEstimation
        Get
            InParameterEstimation = m_in_estimation_loop
        End Get

        Set(ByVal value As Integer)
            m_in_estimation_loop = value
        End Set

    End Property

End Class
