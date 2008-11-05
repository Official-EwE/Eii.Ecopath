'==============================================================================
'
' $Log: cEcoPathModel.vb,v $
' Revision 1.3  2008/11/05 18:16:08  joeb
' Fixed Bug that caused discards not to be include in Fishing mortality shapes FishRateNo() by moving caculation of PropDiscards() to Ecopath with calculation of PropLandings()
'
' Revision 1.2  2008/10/29 15:56:00  joeb
' Change catch_calculation() missing catch message from Imformation to Warning
'
' Revision 1.1  2008/09/26 07:30:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.73  2008/09/24 16:57:56  jeroens
' Localized all existing Ecopath Model messages
'
' Revision 1.72  2008/09/24 15:53:18  jeroens
' EcopathMassBalance once again able to send messages via the core
'
' Revision 1.71  2008/09/24 00:11:03  villyc
' f limits and others
'
' Revision 1.70  2008/09/17 01:23:52  jeroens
' Currency units used correctly by Ecopath
'
' Revision 1.69  2008/07/23 21:12:45  jeroens
' Converting more messages
'
' Revision 1.68  2008/07/23 19:17:41  jeroens
' Added suppressable message
'
' Revision 1.67  2008/07/22 20:46:00  jeroens
' Added more suppressable messages
'
' Revision 1.66  2008/05/29 22:22:42  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.65  2008/04/11 15:06:35  joeb
' Replaced MessageBoxes with AddMessage
'
' Revision 1.64  2008/01/23 16:23:36  jeroens
' Fixed ecopath parameter estimation messages
'
' Revision 1.63  2008/01/17 16:45:48  joeb
' Removed FindCyclesWhenEstimatingBiomass
'
' Revision 1.62  2008/01/17 16:29:22  joeb
' Activated MassBalance plugin
'
' Revision 1.61  2008/01/17 16:12:54  joeb
' Fixed bug in MissingParameterMessage() BA not set
'
' Revision 1.60  2007/09/07 15:24:51  joeb
' Minor changes
'
' Revision 1.59  2007/08/06 19:28:39  joeb
' Pile of stuff to do failed runs and how to tell the interface.
'
' Revision 1.58  2007/08/02 15:10:35  joeb
' Fixed Bug in EE test EE can now be = 0
'
' Revision 1.57  2007/07/13 00:08:33  joeb
' Bug fixes for the Monte Carlo simulation
'
' Revision 1.56  2007/07/08 18:23:26  jeroens
' * Fixing globalization todo's
'
' Revision 1.55  2007/06/28 16:55:10  joeb
' Changes to allow the Monte Carlo to call Ecopath
'
' Revision 1.54  2007/06/26 22:24:36  joeb
' Added suppressMessages to stop ecopath from sending messages
'
' Revision 1.53  2007/06/20 12:41:26  jeroens
' * Fixed message formatting crash
'
' Revision 1.52  2007/06/20 11:43:38  jeroens
' + Missing param message provides Fleet name
'
' Revision 1.51  2007/05/23 17:21:53  joeb
' Added Comments
'
' Revision 1.50  2007/04/30 16:32:58  jeroens
' * Ecopath messages arrive in core again
'
' Revision 1.49  2007/03/02 19:25:49  joeb
' Added ToDo's
'
' Revision 1.48  2007/02/28 21:42:27  joeb
' Update how messages are sent to the core
'
' Revision 1.47  2007/01/29 17:55:57  jeroens
' + Ecospace basemap revamped
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core
Imports EwEPlugin


Namespace Ecopath

    ''' <summary>
    ''' Class that Encapsulates the EcoPath Model
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cEcoPathModel

        Private m_Data As cEcopathDataStructures
        Private m_pluginManager As cPluginManager = Nothing
        Private m_bSuppressMsgs As Boolean
        Private m_eEstimType As eEstimateParameterFor

        Private DCNoCyc(,) As Single
        Private CycDC(,) As Single
        Private NumPath() As Long
        Private SumCycDC() As Single
        Private Cons() As Single
        Friend missing(,) As Boolean
        ' Private CheckedMissing As Boolean
        Private EstimateWhat() As Integer

        Private m_messages As New List(Of cVariableStatus)

        Private m_coreNotifier As cCore.CoreMessageDelegate
        Private m_msgPub As New cMessagePublisher

        'status flag for the estimation results
        Dim m_EstimStatus As eStatusFlags


        Public Sub New()
            m_eEstimType = eEstimateParameterFor.ParameterEstimation
        End Sub

        ''' <summary>
        ''' Results of the Parameter Estimation 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Parameter Estimation results are done as a Property instead of the return value so that a plugin can do the estimation  </remarks>
        Public ReadOnly Property EstimationStatus() As eStatusFlags
            Get
                Return m_EstimStatus
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Exposes the MessagePublisher instance so that the core can add message handlers
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Messages() As cMessagePublisher
            Get
                Return m_msgPub
            End Get
        End Property


        ''' <summary>
        ''' Do not send any messages
        ''' </summary>
        ''' <value>True no messages will be sent. False messages will be sent this is the default behaviour. </value>
        ''' <returns></returns>
        ''' <remarks>This allows Ecopath to run in a 'Silent' mode</remarks>
        Public Property suppressMessages() As Boolean
            Get
                Return m_bSuppressMsgs
            End Get
            Set(ByVal value As Boolean)
                m_bSuppressMsgs = value
            End Set
        End Property


        Public Property ParameterEstimationType() As eEstimateParameterFor
            Get
                Return m_eEstimType
            End Get
            Set(ByVal value As eEstimateParameterFor)
                m_eEstimType = value
            End Set
        End Property



        ''' <summary>
        ''' Tell the core to send this message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>Wraps the delegate instance that is use to notify the core of a message</remarks>
        Private Sub NotifyCore(ByVal msg As cMessage)

            If msg Is Nothing Then Return

            Try
                'messages can be turned off be a user
                'to speed up the running of the model as in the case of the Monte Carlo which run the model multiple times
                'this puts the model into a 'silent' mode
                If Not m_bSuppressMsgs Then
                    m_msgPub.SendMessage(msg)
                End If
            Catch ex As Exception
                cLog.Write(String.Format("cEcoPathModel.NotifyCore(...) Failed to post message {0}.", msg.ToString()))
            End Try

        End Sub

        ''' <summary>
        ''' Redimension any variables after the datasource has been read in
        ''' </summary>
        ''' <returns>True if successfull. False otherwise</returns>
        ''' <remarks>
        ''' This gets called after the datasource has initialized the data structures instance (mData) see InitFromDataSource(...).
        ''' At this time and size of the data is known mData.NumGroups.
        ''' It is supplied here as a place holder for dimensioning user defined variables
        ''' </remarks>
        Private Function DimVariables() As Boolean

            Try
                'Add dimensioning code here
                'this is a change

                Return True
            Catch ex As Exception
                cLog.Write(Me.ToString & ".DimVariables(...) Error: " & ex.Message)
                Debug.Assert(False, Me.ToString & ".DimVariables(...) Error: " & ex.Message)

                'throw the error out to the initialization routine where it can be handled and a message sent to the core
                Throw New System.Exception(Me.ToString() & ".DimVariables() Error: " + ex.Message)

                Return False 'I guess this is pointless since it throw the error above

            End Try

        End Function


        ''' <summary>
        ''' Get or Set the cEcoPathDatastructures object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' This is the wrapper that contains all the underlying data for EcoPath
        ''' </remarks>
        Public Property ModelingData() As cEcopathDataStructures

            Get
                ModelingData = m_Data
            End Get

            Set(ByVal NewParameters As cEcopathDataStructures)
                m_Data = NewParameters
            End Set

        End Property

        ''' <summary>
        ''' Estimate the unknown parameters in EcoPath
        ''' </summary>
        ''' <returns>
        ''' True if successfull 
        ''' False if something went wrong
        ''' </returns>
        ''' <remarks>
        ''' EcoPath must be initialized before this can be called
        ''' see cModelInterface.InitEcoPath(Datasource_filename) for EcoPath initialization.
        ''' Check the EstimationStatus (eStatusFlag) for failure code 
        ''' </remarks>
        Public Function EstimateParameters() As Boolean
            Dim iParamsEstimated As Integer = eStatusFlags.ErrorEncountered
            Dim msg As cMessage = Nothing

            'For development test that everything has been initialized 
            'This is for development to test that Ecopath has been initialized properly that's why it is an Assert
            Debug.Assert(Not m_Data Is Nothing, Me.ToString + ".EstimateParameters() DataSouce must be set before model is called.")
            Debug.Assert(m_Data.bInitialized, Me.ToString + ".EstimateParameters() Datasource has not been initilized.")

            m_EstimStatus = eStatusFlags.Null

            'clear out any existing error messages
            m_messages.Clear()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Paraniod double checking for the release version
            'Is there a valid Ecopath data object. There is no messages for this as it should not happen in the release version. Just write to the log???????
            If IsNothing(m_Data) Then
                cLog.Write(Me.ToString + ".EstimateParameters() Datadource must be set before model is called. Ecopath could not be run.")
                Return False
            End If
            'have the parameters been initialized
            If Not m_Data.bInitialized Then
                cLog.Write(Me.ToString + ".EstimateParameters() Datasource has not been initilized. Ecopath could not be run.")
                Return False
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'ToDo_jb Ecopath.EstimateParameter this dimensioning should get moved to a separate routine
            ReDim DCNoCyc(m_Data.NumGroups + 1, m_Data.NumGroups + 1)
            ReDim CycDC(m_Data.NumGroups + 1, m_Data.NumGroups + 1)
            ReDim NumPath(m_Data.NumLiving)
            ReDim SumCycDC(m_Data.NumGroups)
            ReDim Cons(m_Data.NumLiving)

            setEstimateWhat()

            'jb clear out missing array and recompute it in FindMissing() this does not really need to happen every run
            'only if something changes
            ReDim missing(m_Data.NumGroups, 4)
            FindMissing()

            Try

                'jb debugging
                '      cLog.WriteArrayToFile("Test.csv", mData.QB, "QB")

                'check that all diet composition DC() sum to 1
                'False flag do NOT to ask user what to do
                If Not checkDietsSumToOne(False) Then
                    Return False
                End If

                If m_eEstimType = eEstimateParameterFor.ParameterEstimation Then

                    CheckDetritusFate()
                    checkForMissingDetritusBiomass()

                    CheckForImportOnlyGroups()
                    CheckDetritusFateTooBig()
                    CheckDiscardFateZero()
                    CheckQB()

                    CalcNewExportCatch(0)
                    Catch_calculations()
                End If


                Dim bPluginFailed As Boolean = True
                'Ask the plugin manager to try and do the mass balance 
                'if it fails then run the default mass balance 
                If Me.m_pluginManager IsNot Nothing Then
                    If Me.m_pluginManager.MassBalance(m_Data, m_eEstimType, iParamsEstimated) Then
                        m_EstimStatus = DirectCast(iParamsEstimated, eStatusFlags)
                        bPluginFailed = False
                    End If
                End If

                If bPluginFailed Then
                    EcopathMassBalance.m_msgPub = Me.m_msgPub
                    EcopathMassBalance.EstimateParameters(m_Data, m_eEstimType, m_EstimStatus)
                    EcopathMassBalance.m_msgPub = Nothing
                End If

                If m_EstimStatus = eStatusFlags.OK Then
                    If m_eEstimType = eEstimateParameterFor.ParameterEstimation Then
                        'this code does not run for the sensitivty estimation

                        'parameters successfully estimated
                        CalcTotalPrimProd()
                        CheckIfEstimatesAreZero()
                        EstimEEAgain()
                        EstimateTrophicLevels(m_Data.DC, m_Data.TTLX)
                        DetritusCalculations()
                        Omniv(m_Data.DC, m_Data.TTLX, m_Data.BQB, m_Data.NumGroups)
                        CalcNichePiankaPred()
                        CalcNichePiankaPrey()
                        Chesson()

                        m_Data.onPostEcopathRun()

                        CheckIfEEsAreOK()

                    Else

                        EstimEEAgain()

                    End If

                Else

                    'failed to estimate parameters
                    'post a message if missing parameters
                    If m_EstimStatus = eStatusFlags.MissingParameter Then

                        MissingParameterMessage()

                    Else 'If ParamsEstimated  = eStatusFlags.ErrorEncountered Then

                        msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_ERROR, _
                                            eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Critical, eDataTypes.NotSet)
                        NotifyCore(msg)
                        cLog.Write("ParamEstimate(...) failed to estimate parameters because of an error.")

                    End If

                    Return False

                End If ' If parmest_returncode = eParmEstimateCodes.Success Then

                '  mParamEstimator.InParameterEstimation = 0

            Catch ex As Exception

                Debug.Assert(False)

                msg = New cMessage(My.Resources.CoreMessages.ECOPATH_RUN_ERROR, _
                                    eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Critical, eDataTypes.NotSet)
                NotifyCore(msg)

                cLog.Write(Me.ToString + ".ParamEstimate() Error during parameter estimation: " & ex.Message)
                Return False

            End Try

            Return True

        End Function

        ''' <summary>
        ''' Set the EstimateWhat(iGroup) in response to user input
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub setEstimateWhat()
            Dim strMsg As String = ""

            ReDim EstimateWhat(m_Data.NumGroups)

            EstimateGE()

            For igrp As Integer = 1 To m_Data.NumGroups
                If m_Data.B(igrp) > 0 And m_Data.PB(igrp) >= 0 And m_Data.EE(igrp) >= 0 Then
                    If m_Data.PP(igrp) = 1 Or m_Data.QB(igrp) >= 0 Then
                        If EstimateWhat(igrp) = 0 Then

                            If m_Data.PP(igrp) < 1 Then
                                strMsg = String.Format(My.Resources.CoreMessages.ECOPATH_PROMPT_ESTIMATE_BA_FOR_B_PB_QB_EE, m_Data.GroupName(igrp))
                            Else
                                strMsg = String.Format(My.Resources.CoreMessages.ECOPATH_PROMPT_ESTIMATE_BA_FOR_B_PB_EE, m_Data.GroupName(igrp))
                            End If

                            Dim fbMsg As New cFeedbackMessage(strMsg, eMessageSource.EcoPath, eMessageImportance.Information, cFeedbackMessage.eReplyStyle.YES_NO, eDataTypes.EcoPathGroupInput)
                            fbMsg.Suppressable = True
                            NotifyCore(fbMsg)

                            If fbMsg.Reply = cFeedbackMessage.eReply.YES Then
                                EstimateWhat(igrp) = 1
                            ElseIf fbMsg.Reply = cFeedbackMessage.eReply.NO Then

                                strMsg = String.Format(My.Resources.CoreMessages.ECOPATH_PROMPT_ESTIMATE_NETMIGRATION, m_Data.GroupName(igrp))
                                fbMsg.Message = strMsg
                                NotifyCore(fbMsg)

                                If fbMsg.Reply = cFeedbackMessage.eReply.YES Then
                                    EstimateWhat(igrp) = 2
                                End If

                            End If 'If fbMsg.Reply = cFeedbackMessage.eReply.YES Then


                        End If 'If EstimateWhat(igrp) = 0 Then
                    End If ' If m_Data.PP(igrp) = 1 Or m_Data.QB(igrp) >= 0 Then
                End If ' If m_Data.B(igrp) > 0 And m_Data.PB(igrp) >= 0 And m_Data.EE(igrp) >= 0 Then

            Next igrp

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

        ''' <summary>
        ''' Set all QB values that are CORE_NULL to Zero
        ''' </summary>
        ''' <remarks>QB is used by Ecosim which assumes that null values are zero</remarks>
        Private Sub CheckQB()
            Dim i As Integer
            For i = 1 To m_Data.NumLiving
                If m_Data.QB(i) < 0 And m_Data.PP(i) = 1 Then m_Data.QB(i) = 0
            Next i
        End Sub

        Private Sub EstimEEAgain()             ' Recalculate EE - Ecotrophic Efficiency
            Dim i As Integer
            Dim j As Integer
            Dim Sum As Single
            Dim MM2 As Single

            For i = 1 To m_Data.NumLiving
                Sum = CSng(IIf(m_Data.BaBi(i) <> 0 And m_Data.BA(i) = 0, m_Data.BaBi(i), 0))
                Sum = Sum + CSng(IIf(m_Data.Emig(i) > 0 And m_Data.Emigration(i) = 0, m_Data.Emig(i), 0))
                Sum = CSng(Sum * m_Data.B(i))

                MM2 = 0
                For j = 1 To m_Data.NumLiving
                    If m_Data.QB(j) > 0 Then
                        MM2 = MM2 + m_Data.B(j) * m_Data.QB(j) * m_Data.DC(j, i)
                    End If
                Next j

                'ToDo_jb EstimEEAgain EstimateWhat(i) Is never getting set to anything I need to check this with the EwE5 code
                Select Case EstimateWhat(i)
                    Case 0  'Estimate EE
                        If m_Data.B(i) > 0 And m_Data.PB(i) > 0 Then
                            '031220VC: modified to incorporate that BioAcc and emigration can be rates
                            If m_Data.StanzaGroup(i) = False Then
                                m_Data.EE(i) = (m_Data.fCatch(i) + Sum + m_Data.BA(i) + m_Data.Emigration(i) - m_Data.Immig(i) + MM2) / (m_Data.B(i) * m_Data.PB(i))
                            Else
                                m_Data.EE(i) = (m_Data.fCatch(i) + Sum + m_Data.Emigration(i) - m_Data.Immig(i) + MM2) / (m_Data.B(i) * m_Data.PB(i))
                            End If

                        End If
                    Case 1  'Estimate BA
                        m_Data.BA(i) = m_Data.B(i) * m_Data.PB(i) * m_Data.EE(i) - m_Data.fCatch(i) - Sum - m_Data.Emigration(i) + m_Data.Immig(i) - MM2
                    Case 2  'Estimate migration
                        Sum = CSng(IIf(m_Data.BaBi(i) <> 0 And m_Data.BA(i) = 0, m_Data.B(i) * m_Data.BaBi(i), 0))
                        Sum = CSng(m_Data.B(i) * m_Data.PB(i) * m_Data.EE(i) - Sum - m_Data.BA(i) - m_Data.fCatch(i) - MM2)
                        If Sum < 0 Then
                            m_Data.Immig(i) = -Sum
                        Else
                            m_Data.Emigration(i) = Sum
                        End If
                End Select
            Next i
        End Sub

        Friend Sub DetritusCalculations()

            CalcGS_Det_FlowToDet()            'det ij is flow of det from i to j
            Array.Clear(m_Data.DetEaten, 0, m_Data.NumGroups + 1)
            CalcDetEaten()
            CalcFateOfDetritus()
            CalcBAofDetritus()                'BA= Surplus * DF
            CalcEEforDetritus()               'EE=(DetEaten+DetPassedOn)/INputToDet
            CalcExportOfDetritus()            'EX=INputToDet-DetEaten-BA-DetPassedON
            CalcDCofDetritus()

        End Sub

        Private Sub CalcGS_Det_FlowToDet()
            Dim i As Integer, j As Integer

            For i = 0 To m_Data.NumGroups + m_Data.NumFleet
                For j = 1 To m_Data.NumGroups + m_Data.NumFleet
                    m_Data.det(i, j) = 0
                Next j
            Next i

            For i = 1 To m_Data.NumLiving
                m_Data.FlowToDet(i) = 0
                If m_Data.currUnitIndex = eUnitCurrencyType.Nitrogen Or m_Data.currUnitIndex = eUnitCurrencyType.Phosporous Or m_Data.currUnitIndex = eUnitCurrencyType.CustomNutrient Then
                    m_Data.GS(i) = CSng(IIf(m_Data.GE(i) > 0, (1 - m_Data.GE(i)), -99))
                Else
                    'modified 053196 eli.
                    If m_Data.GS(i) > 1 Then m_Data.GS(i) = m_Data.GS(i) / 100
                End If

                For j = m_Data.NumLiving + 1 To m_Data.NumGroups
                    m_Data.det(i, j) = m_Data.B(i) * m_Data.PB(i) * (1 - m_Data.EE(i)) * m_Data.DF(i, j - m_Data.NumLiving)
                    'Cont. from dying i-organisms to detritus j

                    m_Data.det(i, j) = m_Data.det(i, j) + m_Data.B(i) * m_Data.QB(i) * m_Data.GS(i) * m_Data.DF(i, j - m_Data.NumLiving)
                    'Cont. from egestion of i to detritus j.

                    m_Data.det(0, j) = m_Data.det(0, j) + m_Data.det(i, j)
                    'Total flow into detritus group j

                    ' Here sum all flows from living groups to each detritus group
                    m_Data.FlowToDet(i) = CSng(m_Data.FlowToDet(i) + m_Data.det(i, j))
                Next j
            Next i      'end for groups

            'Next for fleets
            If m_Data.NumFleet > 0 Then
                For i = 1 To m_Data.NumFleet
                    For j = m_Data.NumLiving + 1 To m_Data.NumGroups
                        m_Data.det(i + m_Data.NumGroups, j) = m_Data.Discard(i, 0) * m_Data.DiscardFate(i, j - m_Data.NumLiving)
                        m_Data.det(0, j) = m_Data.det(0, j) + m_Data.det(i + m_Data.NumGroups, j)
                        m_Data.FlowToDet(m_Data.NumGroups + i) = CSng(m_Data.FlowToDet(m_Data.NumGroups + i) + m_Data.det(i + m_Data.NumGroups, j))
                    Next
                Next
            End If
        End Sub

        Private Sub CalcDetEaten()
            Dim i As Integer, j As Integer

            For i = 1 To m_Data.NumGroups
                For j = m_Data.NumLiving + 1 To m_Data.NumGroups            'Detritus boxes
                    If m_Data.QB(i) > 0 Then
                        m_Data.DetEaten(j) = CSng(m_Data.DetEaten(j) + m_Data.B(i) * m_Data.QB(i) * m_Data.DC(i, j))
                    End If
                Next j
            Next i

        End Sub

        Private Sub CalcFateOfDetritus()
            'calculate fate of detritus
            'First sum flow to detritus from import and flow from living groups
            Dim i As Integer, j As Integer, Surplus As Single

            For i = m_Data.NumLiving + 1 To m_Data.NumGroups
                m_Data.InputToDet(i) = CSng(m_Data.DtImp(i) + m_Data.det(0, i))
            Next i

            For i = m_Data.NumLiving + 1 To m_Data.NumGroups
                m_Data.DetPassedOn(i) = 0
                'DetEaten(i) is amount eaten of the group by all consumers
                Surplus = m_Data.InputToDet(i) - m_Data.DetEaten(i) - m_Data.Resp(i)
                If Surplus > 0 Then    'Where do we send the surplus detr. to?
                    For j = m_Data.NumLiving + 1 To m_Data.NumGroups 'recipient
                        If i <> j Then
                            m_Data.InputToDet(j) = m_Data.InputToDet(j) + Surplus * m_Data.DF(i, j - m_Data.NumLiving)
                            m_Data.DetPassedOn(i) = m_Data.DetPassedOn(i) + Surplus * m_Data.DF(i, j - m_Data.NumLiving)
                            m_Data.det(i, j) = Surplus * m_Data.DF(i, j - m_Data.NumLiving)   'Detritus sent from j to i
                        End If
                    Next j
                End If   'Surplus > 0
                m_Data.FlowToDet(i) = m_Data.DetPassedOn(i)
            Next i
        End Sub
        Private Sub CalcBAofDetritus()
            ' Calculate BA biomass accumulation of detritus
            '27 May 2002: VC subtracted Catch from the surplus as Simone had a model where there is a 'catch' of discard,
            'which is subsequently discarded and sent to another detritus group.
            Dim i As Integer, Surplus As Single

            For i = m_Data.NumLiving + 1 To m_Data.NumGroups
                'BA(i) = 0
                'DetEaten(i) is amount eaten of the group by all consumers
                Surplus = m_Data.InputToDet(i) - m_Data.DetEaten(i) - m_Data.fCatch(i)
                'If Surplus > 0 Then   'Where do we send the surplus detr. to?
                m_Data.BA(i) = Surplus * m_Data.DF(i, i - m_Data.NumLiving)
                'End If
            Next i
        End Sub

        Private Sub CalcEEforDetritus()

            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing
            Dim str As String = ""

            'Now calculate the EE for each of the detritus groups
            For i As Integer = m_Data.NumLiving + 1 To m_Data.NumGroups
                If m_Data.InputToDet(i) > 0 Then

                    'EE(i) = (DetEaten(i) + DetPassedOn(i)) / InputToDet(i)
                    If m_Data.InputToDet(i) <> m_Data.Resp(i) Then
                        m_Data.EE(i) = m_Data.DetEaten(i) / (m_Data.InputToDet(i) - m_Data.Resp(i))
                    End If

                    If m_Data.Resp(i) >= m_Data.InputToDet(i) Then
                        If msg Is Nothing Then
                            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_RESPLARGERTHANDETIMP, _
                                               eMessageType.RespirationExceeedsDetritus, eMessageSource.EcoPath, eMessageImportance.Warning)
                            msg.Suppressable = True
                        End If

                        str = String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_RESPLARGERTHANDETIMP_GROUP, Me.m_Data.GroupName(i))
                        vs = New cVariableStatus(eStatusFlags.ErrorEncountered, str, eVarNameFlags.Name, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i)
                        msg.AddVariable(vs)
                    End If

                End If
            Next i

            If (vs IsNot Nothing) Then
                Me.NotifyCore(msg)
            End If
        End Sub

        Private Sub CalcExportOfDetritus()

            'Find export of detritus
            Dim i As Integer, j As Integer, SumDF As Single

            m_Data.Dt = 0          'Total flow to detritus
            For i = m_Data.NumLiving + 1 To m_Data.NumGroups
                SumDF = 0
                For j = m_Data.NumLiving + 1 To m_Data.NumGroups
                    SumDF = SumDF + m_Data.DF(i, j - m_Data.NumLiving)
                Next j
                m_Data.Dt = m_Data.Dt + m_Data.InputToDet(i)
                If SumDF < 1 Then
                    m_Data.Ex(i) = CSng(m_Data.InputToDet(i) - m_Data.DetEaten(i) - m_Data.BA(i) - m_Data.DetPassedOn(i) - m_Data.Resp(i))
                Else
                    m_Data.Ex(i) = 0.0
                End If
            Next i
        End Sub

        Private Sub CalcDCofDetritus()
            Dim i As Integer, j As Integer

            For i = m_Data.NumLiving + 1 To m_Data.NumGroups                    'Diet comp of detr.box(es)
                ' InputToDet(i) gives all flow to detritus, only if positive
                If m_Data.InputToDet(i) > 0 Then
                    'First for the GROUPS
                    For j = 1 To m_Data.NumGroups
                        m_Data.DC(i, j) = CSng(m_Data.det(j, i) / m_Data.InputToDet(i))
                    Next j
                    'Then for IMPORT = DtImp
                    m_Data.DC(i, 0) = m_Data.DtImp(i) / m_Data.InputToDet(i)
                    'Then for FISHERY   VCJan97
                    If m_Data.NumFleet > 0 Then
                        For j = 1 To m_Data.NumFleet
                            m_Data.DCDet(i - m_Data.NumLiving, j) = CSng(m_Data.det(m_Data.NumGroups + j, i) / m_Data.InputToDet(i))
                        Next
                    End If
                End If
            Next i
        End Sub

        ''' <summary>
        ''' Check that Diet Comp sums to one for all groups.
        ''' If Diet Comp does not sum to one the post a message to the core
        ''' </summary>
        ''' <param name="NoQuestionsAsked"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function checkDietsSumToOne(ByVal NoQuestionsAsked As Boolean) As Boolean
            'HACK WARNING
            Dim pred As Integer
            Dim prey As Integer
            Dim Sum As Single
            'Dim Text As String
            'Dim RetVal As Object
            Dim briefQuestion As Boolean
            Dim tolerance As Single
            briefQuestion = True
            tolerance = 0.001
            checkDietsSumToOne = True
            Dim return_value As Boolean
            Dim msg As cMessage

            return_value = True

            For pred = 1 To m_Data.NumLiving Step 1
                If m_Data.PP(pred) < 1 Then    'a consumer
                    Sum = 0
                    For prey = 0 To m_Data.NumGroups Step 1
                        Sum = Sum + m_Data.DC(pred, prey)
                    Next
                    If Sum <> 0 And Math.Abs(Sum - 1) > tolerance Then
                        'does not sum to one
                        If msg Is Nothing Then
                            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_DIETCOMP_NOTSUMTOONE_GENERIC, _
                                                eMessageType.DietComp, eMessageSource.EcoPath, eMessageImportance.Warning, eDataTypes.EcoPathGroupInput)
                        End If

                        msg.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation, _
                                String.Format(My.Resources.CoreMessages.ECOPATH_DIETCOMP_NOTSUMTOONE, pred), _
                                eVarNameFlags.DietComp, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, pred))

                        return_value = False

                    End If

                End If
            Next

            'if a message was generated tell the core
            If Not msg Is Nothing Then
                NotifyCore(msg)
            End If

            Return return_value

        End Function

        '--------------------------------------------------------------------------
        'CheckIfEEsAreOK
        '
        'Inputs:
        '   EE - (global) calculated ecotrophic efficiency array
        '   NumGroups - (global) number of species groups in the model
        '
        'Outputs:
        '   current database updated with input diets set to DC
        '
        'Description:
        'Check whether model is balanced (EE<1) and tell user.
        'Optionallty invoke auto mass balance feature, if user requests.
        '
        'History:
        '   May 2002    P Kavanagh      Modified to include dialog to invoke auto
        '                               mass balance facility
        '                               Also reduce EElimit from 1.005 to 1
        '                               Get rid of DontDisplay flag (unused)
        '--------------------------------------------------------------------------

        Private Sub CheckIfEEsAreOK()
            Dim i As Integer
            Dim EEMax As Single
            Dim msg As cMessage

            EEMax = 1

            For i = 1 To m_Data.NumGroups
                'only test for EE > 1
                If m_Data.EE(i) > EEMax Then
                    If msg Is Nothing Then
                        msg = New cMessage(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_EE_GENERIC, _
                                            eMessageType.EE, eMessageSource.EcoPath, eMessageImportance.Warning, eDataTypes.EcoPathGroupOutput)
                        msg.Suppressable = True
                    End If
                    msg.AddVariable(New cVariableStatus(eStatusFlags.InvalidModelResult, _
                                String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_EE, i), _
                                eVarNameFlags.EEOutput, eDataTypes.EcoPathGroupOutput, eMessageSource.EcoPath, i))

                End If
            Next

            If Not msg Is Nothing Then
                NotifyCore(msg)
            End If


        End Sub

        Private Sub EstimateTrophicLevels(ByVal Diet(,) As Single, ByVal TLreturn() As Single)
            Dim i As Integer, j As Integer
            Dim ErrCode As Integer

            Dim TL() As Single
            ReDim TL(m_Data.NumGroups)

            For i = 1 To m_Data.NumGroups
                'TTLX(i) = 1
                TL(i) = 1
                For j = 1 To m_Data.NumGroups
                    m_Data.LHS(i, j) = 0
                Next j
            Next i

            For i = 1 To m_Data.NumGroups
                m_Data.SumDC(i) = 0
                For j = 1 To m_Data.NumGroups
                    m_Data.SumDC(i) = m_Data.SumDC(i) + Diet(i, j)
                Next j
            Next i

            'Estimation of trophic levels: TTLX
            'The DC is made to sum to one, this means that it is assumed
            'that import to strict consumers has the same trophic level as
            'other prey for the group
            For i = 1 To m_Data.NumGroups
                For j = 1 To m_Data.NumGroups
                    If m_Data.PP(i) = 1 Then            'Strict Primary producer, so no diet composition (even if it may have in carbon model)
                        m_Data.LHS(i, j) = 0
                    ElseIf m_Data.PP(i) > 0 Then            'partly a primary producer
                        m_Data.LHS(i, j) = -Diet(i, j)
                        'ElseIf SumDC(i) > 0 And SumDC(i) < 1 Then 'Consumer with import
                    ElseIf m_Data.SumDC(i) > 0 And Math.Abs(m_Data.SumDC(i) - 1) > 0.0001 Then 'Consumer with import
                        m_Data.LHS(i, j) = -Diet(i, j) / m_Data.SumDC(i)
                    Else                          'Consumer
                        m_Data.LHS(i, j) = -Diet(i, j)
                    End If
                    If m_Data.PP(i) > 0 And m_Data.PP(i) < 1 Then
                        'Mixed producer / consumer: TTLX should reflect both roles
                        m_Data.LHS(i, j) = -Diet(i, j) * (1 - m_Data.PP(i))
                    End If
                Next j
                m_Data.LHS(i, i) = 1 - Diet(i, i)
            Next i

            For i = m_Data.NumLiving + 1 To m_Data.NumGroups          'multidet version for
                For j = 1 To m_Data.NumGroups
                    m_Data.LHS(i, j) = 0
                Next j
                m_Data.LHS(i, i) = 1
            Next i

            ErrCode = MatSEqnS(m_Data.LHS, TL)   'Inverses matrix to find

            If ErrCode = 0 Then 'no error
                For i = 1 To m_Data.NumGroups : TLreturn(i) = TL(i) : Next
            End If

        End Sub

        Private Sub CalcTotalPrimProd()
            Dim i As Integer

            m_Data.PProd = 0  ' Calculated primary production
            For i = 1 To m_Data.NumLiving
                If m_Data.PP(i) > 0 Then m_Data.PProd = CSng(m_Data.PProd + m_Data.PB(i) * m_Data.B(i) * m_Data.PP(i))
            Next i

        End Sub

        Private Sub CheckIfEstimatesAreZero()
            Dim msgPB0 As cMessage = Nothing
            Dim msgQB0 As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing

            For i As Integer = 1 To m_Data.NumLiving
                If m_Data.PP(i) < 1 And (m_Data.PB(i) = 0 Or m_Data.QB(i) = 0) Then
                    If m_Data.PB(i) = 0 Then

                        ' Msg for PB0 not created yet?
                        If Object.ReferenceEquals(msgPB0, Nothing) Then
                            ' #Not existing, create it
                            msgPB0 = New cMessage(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_PB0_GENERIC, _
                                     eMessageType.InvalidModel_PB0_Generic, eMessageSource.EcoPath, eMessageImportance.Warning, _
                                     eDataTypes.EcoPathGroupInput)
                            msgPB0.Suppressable = True
                        End If
                        ' Create variable information for this messages
                        vs = New cVariableStatus(eStatusFlags.InvalidModelResult, _
                                String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_PB0, i), _
                                eVarNameFlags.PBInput, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i)
                        ' Add variable info
                        msgPB0.Variables.Add(vs)

                    ElseIf m_Data.QB(i) = 0 Then

                        If Object.ReferenceEquals(msgQB0, Nothing) Then
                            ' #Not existing, create it
                            msgQB0 = New cMessage(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_QB0_GENERIC, _
                                     eMessageType.InvalidModel_QB0_Generic, eMessageSource.EcoPath, eMessageImportance.Warning, _
                                     eDataTypes.EcoPathGroupInput)
                            msgQB0.Suppressable = True
                        End If
                        ' Create variable information for this messages
                        vs = New cVariableStatus(eStatusFlags.InvalidModelResult, _
                                String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_QB0_GENERIC, i), _
                                eVarNameFlags.QBInput, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i)
                        ' Add variable info
                        msgQB0.Variables.Add(vs)
                    End If
                End If
            Next

            ' Send messages, if any
            If Not Object.ReferenceEquals(msgPB0, Nothing) Then Me.m_msgPub.SendMessage(msgPB0)
            If Not Object.ReferenceEquals(msgQB0, Nothing) Then Me.m_msgPub.SendMessage(msgQB0)
        End Sub

        Private Sub Omniv(ByVal DC(,) As Single, ByVal TTLX() As Single, ByVal BQB() As Single, ByVal NumGroups As Integer)
            Dim i As Integer, S1 As Single, j As Integer

            For i = 1 To NumGroups
                S1 = 0
                BQB(i) = 0
                If TTLX(i) <> 0 Then
                    For j = 1 To NumGroups
                        S1 = S1 + TTLX(j) * DC(i, j)
                    Next j
                    BQB(i) = S1
                End If
                ' Now BQQ(i%) is the average trophic level of the preys (j%) of i%
            Next i

            For i = 1 To NumGroups
                If TTLX(i) <> 0 Then
                    S1 = 0
                    For j = 1 To NumGroups
                        S1 = S1 + CSng(((TTLX(j) - BQB(i)) ^ 2) * DC(i, j))
                    Next j
                    BQB(i) = S1
                End If
            Next i

        End Sub

        Private Sub CalcNewExportCatch(ByVal OneOnly As Integer)
            Dim Group As Integer

            If OneOnly = 0 Then     'Do them all
                For Group = 1 To m_Data.NumGroups 'Step 1
                    UpdateExportCatch(Group)
                Next
            Else        'Really one only
                If OneOnly > 0 And OneOnly <= m_Data.NumGroups Then UpdateExportCatch(OneOnly)
            End If

        End Sub

        Private Sub UpdateExportCatch(ByVal Group As Integer)
            Dim GearCount As Integer
            Dim sumValue As Single
            Dim Sum As Single

            sumValue = 0
            For GearCount = 1 To m_Data.NumFleet ' Step 1
                sumValue = sumValue + CSng((m_Data.Landing(GearCount, Group) + m_Data.Discard(GearCount, Group)))
            Next

            m_Data.fCatch(Group) = sumValue
            m_Data.Ex(Group) = m_Data.fCatch(Group)

            Sum = CSng(IIf(m_Data.Emig(Group) > 0 And m_Data.Emigration(Group) = 0 And m_Data.B(Group) > 0, m_Data.Emig(Group) * m_Data.B(Group), 0))

            If Group <= m_Data.NumLiving Then
                m_Data.Ex(Group) = m_Data.Ex(Group) - m_Data.Immig(Group) + m_Data.Emigration(Group) + Sum
            End If

        End Sub

        Private Sub Catch_calculations()
            Dim tcost As Single
            Dim value As Single
            Dim i As Integer
            Dim j As Integer
            Dim K As Integer

            Dim msg As cMessage

            m_Data.Landing(0, 0) = 0
            m_Data.Discard(0, 0) = 0
            For j = 1 To m_Data.NumGroups
                m_Data.Landing(0, j) = 0
                m_Data.Discard(0, j) = 0
                m_Data.fCatch(j) = 0
            Next
            For i = 1 To m_Data.NumFleet
                m_Data.Landing(i, 0) = 0
                m_Data.Discard(i, 0) = 0
                For j = 1 To m_Data.NumGroups
                    'mData.fcatch by NumGear, group and total
                    'mData.fcatch(NumGroups + i) = mData.fcatch(NumGroups + i) + mData.landing(i, j) 'by NumGear
                    m_Data.fCatch(j) = CSng(m_Data.fCatch(j) + m_Data.Landing(i, j) + m_Data.Discard(i, j))        'by group
                    'mData.fcatch(0) = mData.fcatch(0) + mData.landing(i, j)             'total
                    'mData.Discards by gear, group, and total
                    m_Data.Landing(i, 0) = m_Data.Landing(i, 0) + m_Data.Landing(i, j)
                    m_Data.Landing(0, j) = m_Data.Landing(0, j) + m_Data.Landing(i, j)
                    m_Data.Landing(0, 0) = m_Data.Landing(0, 0) + m_Data.Landing(i, j)

                    m_Data.Discard(i, 0) = m_Data.Discard(i, 0) + m_Data.Discard(i, j)
                    m_Data.Discard(0, j) = m_Data.Discard(0, j) + m_Data.Discard(i, j)
                    m_Data.Discard(0, 0) = m_Data.Discard(0, 0) + m_Data.Discard(i, j)

                    If (m_Data.Discard(i, j) = 0) Or (m_Data.Landing(i, j) = 0) Then
                        If msg Is Nothing Then
                            msg = New cMessage(My.Resources.CoreMessages.ECOPATH_MISSINGPARAM_CATCH_GENERIC, _
                                    eMessageType.NoCatchForFleet, eMessageSource.EcoPath, _
                                    eMessageImportance.Warning, eDataTypes.FleetInput)
                            msg.Suppressable = True
                        End If

                        If m_Data.Landing(i, j) = 0 Then
                            ' Inform core that the sum of landing and discards is missing
                            msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, _
                                    String.Format(My.Resources.CoreMessages.ECOPATH_MISSINGPARAM_LANDING, m_Data.FleetName(i), m_Data.GroupName(j)), _
                                    eVarNameFlags.Landings, eDataTypes.FleetInput, eMessageSource.EcoPath, i, j))
                        End If
                        If m_Data.Discard(i, j) = 0 Then
                            msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, _
                                    String.Format(My.Resources.CoreMessages.ECOPATH_MISSINGPARAM_DISCARD, m_Data.FleetName(i), m_Data.GroupName(j)), _
                                    eVarNameFlags.Discards, eDataTypes.FleetInput, eMessageSource.EcoPath, i, j))
                        End If
                    End If

                Next

                'if this group has no catch then tell the user
                'Gather the data for a message
                If (m_Data.Landing(i, 0) + m_Data.Discard(i, 0)) > 0 Then
                    'this has not been tested so stop and see if it works the first time in here
                    ' Debug.Assert(False)
                    msg = Nothing
                End If
            Next

            If Not msg Is Nothing Then
                NotifyCore(msg)
            End If

            'Also calculate the average market value by group  -- average value
            For j = 1 To m_Data.NumGroups
                m_Data.Market(0, j) = 0
                value = 0
                For i = 1 To m_Data.NumFleet

                    If m_Data.Landing(i, j) > 0 Then value = CSng(value + m_Data.Landing(i, j) * m_Data.Market(i, j))
                    m_Data.PropLanded(i, j) = 0
                    m_Data.PropDiscard(i, j) = 0
                    If m_Data.Landing(i, j) + m_Data.Discard(i, j) > 0 Then
                        m_Data.PropLanded(i, j) = CSng(m_Data.Landing(i, j) / (m_Data.Landing(i, j) + m_Data.Discard(i, j)))
                        m_Data.PropDiscard(i, j) = CSng(m_Data.Discard(i, j) / (m_Data.Landing(i, j) + m_Data.Discard(i, j)))
                    End If

                Next i
                If value > 0 And m_Data.Landing(0, j) > 0 Then m_Data.Market(0, j) = CSng(value / m_Data.Landing(0, j))
                'Calculate proportion mData.Discarded by group
            Next

            'Estimate the value and cost PLUS PROFIT for the fisheries:
            '   Dim ttt As Single
            For i = 1 To m_Data.NumFleet
                value = 0
                For j = 1 To m_Data.NumLiving
                    If m_Data.Landing(i, j) > 0 Then value = CSng(value + m_Data.Landing(i, j) * m_Data.Market(i, j))
                Next
                '       ttt = ttt + value
                'Now knows the value; the costs are known as %. The profit is calculated from:
                'Fixed plus variable cost is summed to give total cost for this gear:
                tcost = m_Data.CostPct(i, eCostIndex.Fixed) + m_Data.CostPct(i, eCostIndex.CUPE) + m_Data.CostPct(i, eCostIndex.Sail) 'this will sum e.g. to 90 = 90% of value, hence:
                'If tcost > 0 Then
                For K = 1 To 3
                    m_Data.cost(i, K) = value * (m_Data.CostPct(i, K) / 100)
                Next
                m_Data.cost(i, 0) = value * (100 - tcost) / 100 'This is the profit
                m_Data.CostPct(i, eCostIndex.Profit) = 100 - tcost
                'End If
            Next

            Dim Code As Integer
            Dim Group As Integer

            'mData.fcatch codes totals are needed for scaling later
            For Group = 1 To m_Data.NumGroups
                For Code = 1 To m_Data.NumCatchCodes
                    m_Data.CatchCode(0, Group) = m_Data.CatchCode(0, Group) + m_Data.CatchCode(Code, Group)
                Next
            Next

            'ttt = 0: For i = 1 To NumGear: For j = 1 To NumLiving: ttt = ttt + mData.landing(i, j) * Market(i, j): Next: Next
        End Sub

        Private Function MissingParameterMessage() As Boolean
            Dim i As Integer
            Dim isMissing As Boolean

            For i = 1 To m_Data.NumLiving
                If m_Data.B(i) <= 0 Or m_Data.PB(i) < 0 Or m_Data.QB(i) < 0 Or m_Data.EE(i) < 0 Or m_Data.BA(i) < 0 Then
                    isMissing = True
                    Exit For
                End If
            Next i

            If isMissing Then

                Dim msg As New cMessage(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_MISSINGGENERIC, _
                                        eMessageType.TooManyMissingParameters, eMessageSource.EcoPath, _
                                        eMessageImportance.Warning, eDataTypes.EcoPathGroupInput)
                msg.Suppressable = False

                For i = 1 To m_Data.NumLiving
                    If m_Data.B(i) <= 0 Then msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_B, i), eVarNameFlags.Biomass, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i))
                    If m_Data.PB(i) < 0 Then msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_PB, i), eVarNameFlags.PBInput, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i))
                    If m_Data.QB(i) < 0 Then msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_QB, i), eVarNameFlags.QBInput, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i))
                    If m_Data.EE(i) < 0 Then
                        msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_EE, i), eVarNameFlags.EEInput, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i))
                        If m_Data.BA(i) < 0 Then msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, String.Format(My.Resources.CoreMessages.ECOPATH_PARAMESTIMATION_FAILED_EE_BA, i), eVarNameFlags.BioAccum, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i))
                    End If
                Next

                'send the message to the core
                'the core will forward it out to an interface
                NotifyCore(msg)

            End If

            Return True

        End Function

        Public Function DumpResults(ByVal filename As String) As Boolean
            Return m_Data.DumpResults(filename)
        End Function

        Function FindMissing() As Boolean
            Dim i As Integer

            'jb in Ewe this also included a test for Biomass/Area bh() for detritus groups 
            'the test is only performed once then missing values are let through??????
            'that test has been moved to cCore.checkBiomassForDetritus()

            For i = 1 To m_Data.NumLiving
                If m_Data.QB(i) < 0 And m_Data.PP(i) = 1 Then m_Data.QB(i) = 0
                missing(i, 1) = CBool(IIf(m_Data.BH(i) <= 0, True, False))
                missing(i, 2) = CBool(IIf(m_Data.PB(i) < 0 And m_Data.GE(i) < 0, True, False))
                missing(i, 3) = CBool(IIf(m_Data.QB(i) < 0 And m_Data.GE(i) < 0, True, False))

                'If i = 1 And m_Data.EE(i) > 0 Then
                '    System.Console.WriteLine("EE ")
                'End If

                missing(i, 4) = CBool(IIf(m_Data.EE(i) < 0, True, False))
            Next i
        End Function

        Private Sub CalcNichePiankaPred()
            Dim i As Integer, j As Integer, K As Integer
            Dim SumP2(m_Data.NumGroups) As Single
            Dim SumHost(m_Data.NumGroups) As Single

            '*** Pianka predator niche overlap - start
            For i = 1 To m_Data.NumLiving
                SumHost(i) = 0
                For j = 1 To m_Data.NumLiving
                    If m_Data.B(j) > 0 And m_Data.QB(j) > 0 And m_Data.DC(j, i) > 0 Then
                        m_Data.Host(i, j) = m_Data.B(j) * m_Data.QB(j) * m_Data.DC(j, i)
                        SumHost(i) = SumHost(i) + m_Data.Host(i, j)
                    Else
                        m_Data.Host(i, j) = 0
                    End If
                Next j
            Next i       'Host(ij) is amount eaten of group i by predator j
            'Here calculated not regarding detritus as a pred.

            For i = 1 To m_Data.NumLiving
                For j = 1 To m_Data.NumLiving
                    m_Data.Hlap(i, j) = 0
                    If SumHost(i) > 0 Then m_Data.Host(i, j) = m_Data.Host(i, j) / SumHost(i)
                Next j
            Next i

            For i = 1 To m_Data.NumLiving
                SumP2(i) = 0
                For j = 1 To m_Data.NumLiving
                    SumP2(i) = CSng(SumP2(i) + m_Data.Host(i, j) ^ 2)
                    For K = 1 To m_Data.NumGroups
                        m_Data.Hlap(i, j) = m_Data.Hlap(i, j) + m_Data.Host(i, K) * m_Data.Host(j, K)
                    Next K
                Next j
            Next i

            For i = 1 To m_Data.NumLiving
                For j = 1 To m_Data.NumLiving
                    If SumP2(i) > 0 And SumP2(j) > 0 Then m_Data.Hlap(i, j) = m_Data.Hlap(i, j) / (SumP2(i) + SumP2(j)) * 2
                Next j
            Next i
        End Sub

        Private Sub CalcNichePiankaPrey()
            Dim i As Integer, j As Integer, K As Integer
            Dim SumP2() As Single
            Dim SumHost() As Single

            ReDim SumP2(m_Data.NumGroups)
            ReDim SumHost(m_Data.NumGroups)

            'estimates the results
            For i = 1 To m_Data.NumGroups
                For j = 1 To m_Data.NumGroups
                    m_Data.Plap(i, j) = 0
                    If m_Data.DC(i, j) > 0 Then SumP2(i) = CSng(SumP2(i) + m_Data.DC(i, j) ^ 2)
                    For K = 1 To m_Data.NumGroups
                        m_Data.Plap(i, j) = m_Data.Plap(i, j) + m_Data.DC(i, K) * m_Data.DC(j, K)
                    Next K
                Next j
            Next i

            For i = 1 To m_Data.NumGroups
                For j = 1 To m_Data.NumGroups
                    If SumP2(i) > 0 And SumP2(j) > 0 Then m_Data.Plap(i, j) = m_Data.Plap(i, j) / (SumP2(i) + SumP2(j)) * 2
                Next j
            Next i
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>This method is borrowed from EwE5 EcoRanger since it is required for
        ''' calculating Ecopath outputs.</remarks>
        Private Sub Chesson()
            Dim LivingBio As Single
            Dim MaxBio As Single

            MaxBio = 0
            LivingBio = 0
            m_Data.SumBio = 0

            For i As Integer = 1 To m_Data.NumLiving
                If m_Data.B(i) > MaxBio Then MaxBio = m_Data.B(i)
                LivingBio = LivingBio + m_Data.B(i)
            Next i
            m_Data.SumBio = LivingBio

            'Will assume that if there is no biomass for a detritus box
            'then the biomass will correspond to the max living biomass
            'divided by the number of detritus boxes. Thus if all detritus
            'biomasses are lacking the total detritus biomass = max living biom.
            For i As Integer = m_Data.NumLiving + 1 To m_Data.NumGroups
                'If m_Data.B(i) < 0 Then
                '    m_Data.B(i) = MaxBio / (m_Data.NumGroups - m_Data.NumLiving)
                'End If

                'jb changed this to not change B() for detritus if no biomass was specified by the user
                'Changing B() for detritus messes up Ecosim
                If m_Data.B(i) < 0 Then
                    m_Data.SumBio = m_Data.SumBio + MaxBio / (m_Data.NumGroups - m_Data.NumLiving)
                Else
                    m_Data.SumBio = m_Data.SumBio + m_Data.B(i)
                End If

            Next i

            For i As Integer = 1 To m_Data.NumGroups               'CALCULATION OF PREFERENCE INDEX
                m_Data.SumR(i) = 0
                For j As Integer = 1 To m_Data.NumGroups           'FOLLOWING CHESSON (1983)
                    m_Data.Alpha(i, j) = 0
                    If m_Data.B(j) > 0 Then
                        m_Data.Alpha(i, j) = m_Data.DC(i, j) / (m_Data.B(j) / m_Data.SumBio)
                    End If
                    m_Data.SumR(i) = m_Data.SumR(i) + m_Data.Alpha(i, j)
                Next j
            Next i

            For i As Integer = 1 To m_Data.NumGroups
                For j As Integer = 1 To m_Data.NumGroups
                    If m_Data.SumR(i) > 0 Then
                        m_Data.Alpha(i, j) = m_Data.Alpha(i, j) / m_Data.SumR(i)
                    End If
                Next j               'THIS ALPHA IS THE SAME AS CHESSONS ALPHA
            Next i

            For i As Integer = 1 To m_Data.NumGroups
                If m_Data.QB(i) > 0 Then
                    For j As Integer = 1 To m_Data.NumGroups
                        m_Data.Alpha(i, j) = (m_Data.NumGroups * m_Data.Alpha(i, j) - 1) / ((m_Data.NumGroups - 2) * m_Data.Alpha(i, j) + 1)
                    Next j
                End If                     'THIS ALPHA EQUALS CHESSONS EPSILON
            Next i
        End Sub

        ''' <summary>
        ''' Warn the user if detritus has no biomass
        ''' </summary>
        ''' <remarks>In EwE5 this was part of FindMissing()</remarks>
        Private Sub checkForMissingDetritusBiomass()
            Dim msg As cMessage = Nothing

            For i As Integer = m_Data.NumLiving + 1 To m_Data.NumGroups
                If m_Data.BHinput(i) < 0 And msg Is Nothing Then
                    msg = New cMessage(My.Resources.CoreMessages.ECOPATH_PROMPT_ENTER_B_BEFORE_PROCEEDING, _
                                eMessageType.InvalidModel_B_Detritus, eMessageSource.EcoPath, eMessageImportance.Warning)
                    msg.Suppressable = True
                End If
            Next

            If msg IsNot Nothing Then NotifyCore(msg)

        End Sub

        Private Sub CheckForImportOnlyGroups()

            Dim nFound As Integer = 0
            Dim bImportOnly(m_Data.NumLiving) As Boolean
            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing

            For iGroup As Integer = 1 To m_Data.NumLiving
                If (m_Data.DC(iGroup, 0) > 0.9999999) And (m_Data.PP(iGroup) < 1) Then
                    bImportOnly(iGroup) = True
                    nFound += 1
                Else
                    bImportOnly(iGroup) = False
                End If
            Next

            If (nFound > 0) Then
                msg = New cMessage(String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_DIETIMPORTONLY, nFound), _
                                   eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Warning)
                msg.Suppressable = True

                For iGroup As Integer = 1 To m_Data.NumLiving
                    If bImportOnly(iGroup) Then msg.AddVariable(New cVariableStatus(eStatusFlags.MissingParameter, _
                        String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_DIETIMPORTONLY_GROUP, Me.m_Data.GroupName(iGroup)), _
                        eVarNameFlags.Name, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, iGroup))
                Next
                Me.NotifyCore(msg)
            End If

        End Sub

        Private Sub CheckDetritusFate()
            Dim i As Integer
            Dim j As Integer
            Dim Dummy As Integer
            Dim msg As String = ""
            Dim PFlag As Boolean
            Dim AllOne As Boolean
            Dim SumDF As Single

            Dummy = 0
            AllOne = True
            For j = 1 To m_Data.NumLiving 'row/groups
                PFlag = False
                SumDF = 0
                For i = 1 To m_Data.NumDetrit
                    If m_Data.DF(j, i) > 0 Then PFlag = True
                    SumDF = SumDF + m_Data.DF(j, i)
                Next
                If Not PFlag Then Dummy = Dummy + 1
                If SumDF < 0.99 Then AllOne = False
            Next

            'If Not AllOne Then MsgBox "Detritus fate for one or more living groups sum to less than 1. ", vbInformation + vbOKOnly, "Detritus fate sum to less than 1"
            If Dummy > 5 Then
                'ToDo_jb CheckDetritusFate message

                'msg = "Detritus fate is 0 for " & CStr(Dummy) & " living groups."
                'msg = msg & "This means that detritus from these groups will be exported from the system. "
                'msg = msg & Chr$(13) & Chr$(13) & "Force to 1 to send all detritus to the last detritus group?"
                'If MsgBox(msg, 36) = 6 Then
                '    For j = 1 To NumLiving Step 1 'row/groups
                '        If DF(j, NumDetrit) = 0 Then DF(j, NumDetrit) = 1
                '    Next
                '    DF(NumGroups, NumDetrit) = 0
                '    'Update the database:
                '    If ImportedFlag = False Then SaveDetritusFate()
                'End If
            End If
            'set DF(NumGroups) to 0  to avoid biomass accumulation which screws up EcoSim
            m_Data.DF(m_Data.NumGroups, m_Data.NumDetrit) = 0
        End Sub

        Private Sub CheckDetritusFateTooBig()

            Dim nFound As Integer = 0
            Dim DFtooBig(m_Data.NumGroups) As Boolean
            Dim SumDF As Single = 0.0!
            Dim str As String = ""
            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing

            For i As Integer = 1 To m_Data.NumGroups
                SumDF = 0
                For j As Integer = 1 To m_Data.NumDetrit
                    SumDF = SumDF + m_Data.DF(i, j)
                Next
                If SumDF > 1 Then DFtooBig(i) = True : nFound = nFound + 1
            Next

            If nFound > 0 Then
                msg = New cMessage(String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_DFLARGERTHANONE, nFound), _
                                   eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Warning)
                msg.Suppressable = True

                For i As Integer = 1 To m_Data.NumGroups
                    If DFtooBig(i) Then
                        str = String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_DFLARGERTHANONE_GROUP, Me.m_Data.GroupName(i))
                        vs = New cVariableStatus(eStatusFlags.ErrorEncountered, str, eVarNameFlags.DetritusFate, eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath, i)
                        msg.AddVariable(vs)
                    End If
                Next
                Me.NotifyCore(msg)
            End If

        End Sub

        Public Sub CheckDiscardFateZero()
            Dim nFound As Integer
            Dim bNoDiscardFate(m_Data.NumGroups) As Boolean
            Dim bHasDiscards(m_Data.NumGroups) As Boolean
            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing
            Dim str As String = ""
            Dim ig As Integer
            Dim SumDF As Single

            nFound = 0
            For i As Integer = 1 To m_Data.NumGroups
                For j As Integer = 1 To m_Data.NumFleet
                    If m_Data.Discard(j, i) > 0 Then bHasDiscards(j) = True
                Next
            Next

            ig = 0
            For i As Integer = 1 To m_Data.NumFleet
                If bHasDiscards(i) Then
                    SumDF = 0
                    ig += 1
                    For j As Integer = 1 To m_Data.NumDetrit
                        SumDF = SumDF + m_Data.DiscardFate(i, j)
                    Next
                    If SumDF = 0 Then bNoDiscardFate(i) = True : nFound += 1
                End If
            Next

            If nFound = ig And m_Data.NumDetrit = 1 Then
                'If there is only one detritus group, and if all groups with discard lacks detritus fate, then use a default
                For i As Integer = 1 To m_Data.NumFleet
                    m_Data.DiscardFate(i, 1) = 1
                Next
            ElseIf nFound > 0 Then

                str = String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_MISSINGDISCARDFATE, nFound)
                msg = New cMessage(str, eMessageType.ErrorEncountered, eMessageSource.EcoPath, eMessageImportance.Warning)
                msg.Suppressable = True

                For i As Integer = 1 To m_Data.NumFleet
                    If bNoDiscardFate(i) Then
                        str = String.Format(My.Resources.CoreMessages.ECOPATH_INVALIDMODEL_MISSINGDISCARDFATE_FLEET, m_Data.FleetName(i))
                        vs = New cVariableStatus(eStatusFlags.MissingParameter, str, eVarNameFlags.DiscardFate, eDataTypes.FleetInput, eMessageSource.EcoPath, i)
                        msg.AddVariable(vs)
                    End If
                Next

                Me.NotifyCore(msg)

            End If

        End Sub

        Public Property PluginManager() As cPluginManager
            Get
                Return Me.m_pluginManager
            End Get
            Set(ByVal pm As cPluginManager)
                Me.m_pluginManager = pm
            End Set
        End Property

    End Class

End Namespace
