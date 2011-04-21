Imports System.IO

Imports EwECore.MSE
Imports EwEUtils.Core
Imports EwECore.MSECommandFile


Namespace MSEBatchManager

    Public Enum eMSEBatchRunTypes
        Any = 0
        Constant_F = 1
        Constant_Y = 2
        TFM = 3
    End Enum

    Public Enum eMSEBatchOuputTypes
        Biomass
        QB 'consumption/biomass
        FeedingTime
        FishingMortRate
        PredRate
        CatchByGroup
        NotSet
    End Enum

    Public Class cMSEBatchManager

        'ToDo_jb 17-Aug-2010 MSEBatchManager add run header to outputs
        'ToDo_jb 17-Aug-2010 MSEBatchManager Validation of model and scenario
        'ToDo_jb 17-Aug-2010 MSEBatchManager figure out how to prompt user if validation failed

        'ToDo_jb 24-Aug-2010 MSEBatchManager Why does the list box in the interface not update after the first run


        Private Enum eBatchRunState
            Running
            Idle
        End Enum

        Public Delegate Sub MSEBatchMessage(ByVal strMessage As String)

        Private m_core As cCore
        Private m_fileReader As cMSECommandFileReader
        Private m_MSE As cMSE
        Private m_MSEdata As cMSEDataStructures
        Private m_BatchData As cMSEBatchDataStructures

        Private m_curForceIter As Integer

        Private m_msgDelegate As MSEBatchMessage
        Private m_OutputWriter As cMSEBatchOutputWriter

        Private m_thrdRun As System.Threading.Thread
        Private m_SyncOb As System.Threading.SynchronizationContext
        Private m_runState As eBatchRunState


        Public Sub New()
            Me.m_SyncOb = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. 
            If (Me.m_SyncOb Is Nothing) Then Me.m_SyncOb = New System.Threading.SynchronizationContext()
        End Sub

        Public Sub Init(ByVal theCore As cCore, ByVal MSE As cMSE)

            If Me.m_SyncOb Is Nothing Then
                Me.m_SyncOb = System.Threading.SynchronizationContext.Current
                'if there is no current context then create a new one on this thread. 
                If (Me.m_SyncOb Is Nothing) Then Me.m_SyncOb = New System.Threading.SynchronizationContext()
            End If

            Me.m_runState = eBatchRunState.Idle

            Me.m_BatchData = New cMSEBatchDataStructures

            Me.m_core = theCore
            Me.m_MSE = MSE
            Me.m_MSEdata = MSE.Data

            Me.m_fileReader = New cMSECommandFileReader(Me.Core, Me)

            MSE.BatchManager = Me

            Try
                If (Me.Core.PluginManager IsNot Nothing) Then
                    Me.Core.PluginManager.MSEBatchInitialized(Me, Me.BatchData)
                End If
            Catch ex As Exception

            End Try

        End Sub


        Public Sub Clear()
            Try

                Me.m_BatchData = Nothing
                Me.m_fileReader = Nothing

                Me.m_msgDelegate = Nothing
                Me.m_SyncOb = Nothing

            Catch ex As Exception

            End Try
        End Sub


        ''' <summary>
        ''' Vary the Primary Production forcing function value of the current time step
        ''' </summary>
        ''' <param name="ForcingMultTime"></param>
        ''' <remarks></remarks>
        Public Sub varyForcing(ByRef ForcingMultTime() As Single)
            Dim iGrp As Integer = Me.BatchData.ForcingGroup(Me.m_curForceIter)
            Dim simData As cEcosimDatastructures = Me.m_core.m_EcoSimData

            For ifn As Integer = 1 To simData.MaxFunctions
                'are there any forcing functions set
                If simData.FunctionNumber(iGrp, iGrp, ifn) = 0 Then Exit For

                'is this the same function as the command file loaded
                If simData.FunctionNumber(iGrp, iGrp, ifn) = Me.BatchData.ForcingIndexes(Me.m_curForceIter) Then
                    Dim iforce As Integer = simData.FunctionNumber(iGrp, iGrp, ifn)
                    'Yes vary the forcing data for this timestep
                    'System.Console.Write("tval=" & tval(iforce).ToString & ", ")
                    ForcingMultTime(iforce) = Me.m_MSE.RandNormDist(Me.BatchData.STDevForcing, ForcingMultTime(iforce))

                    'constrain the forcing value to >= zero
                    If ForcingMultTime(iforce) < 0 Then ForcingMultTime(iforce) = 0
                    'System.Console.WriteLine("varied=" & tval(iforce).ToString)

                End If
            Next ifn

        End Sub


        Private Function ValidateData() As Boolean
            Dim bSuccess As Boolean
            Try

                bSuccess = Me.m_fileReader.ValidateData()
                If Not bSuccess Then
                    'Failed validation
                    Me.MarshallMessage("Command file failed validation.")
                    Me.MarshallMessage("Please fix all errors in the command file and try again.")
                    ' Me.MarshallMessage("")
                End If

            Catch ex As Exception
                Me.MarshallMessage("Error while validating command file data. " & ex.Message)
                bSuccess = False
            End Try

            Return bSuccess


        End Function


        Private Sub setForcing(ByVal iForcing As Integer)

            Me.LoadPPForcing(Me.BatchData.ForcingIndexes(iForcing), Me.BatchData.ForcingGroup(iForcing))

        End Sub

        Private Sub setControls(ByVal iForcing As Integer)

            For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                Me.m_MSEdata.QuotaType(iflt) = Me.BatchData.ControlType(iForcing, iflt)
            Next iflt

        End Sub

        ''' <summary>
        ''' Set Quota parameters e.g F, TFM... according to the RunType
        ''' </summary>
        ''' <param name="iParIter"></param>
        ''' <remarks>
        ''' If the RunType is Any then set any of the parameters to the values in the command file. 
        ''' For all other RunTypes set the other parameters to zero so they will not be used. </remarks>
        Private Sub setParameters(ByVal iParIter As Integer)
            Dim igrp As Integer
            Dim blim As Single, bbase As Single, fmin As Single, fmax As Single
            Dim f As Single, tac As Single
            Me.m_BatchData.iCurRun = iParIter


            For igrp = 1 To Me.m_MSEdata.NGroups

                Select Case Me.BatchData.RunType

                    Case eMSEBatchRunTypes.TFM
                        blim = Me.BatchData.tfmBlim(iParIter, igrp)
                        bbase = Me.BatchData.tfmBbase(iParIter, igrp)
                        fmax = Me.BatchData.tfmFmax(iParIter, igrp)
                        fmin = Me.BatchData.tfmFmin(iParIter, igrp)

                    Case eMSEBatchRunTypes.Constant_F
                        f = Me.BatchData.FixedF(iParIter, igrp)

                    Case eMSEBatchRunTypes.Constant_Y
                        tac = Me.BatchData.TAC(iParIter, igrp)

                    Case eMSEBatchRunTypes.Any
                        'If RunType is Any then set Any of the MSE parameters to values from the command file 
                        'If the parameter iteration is > number of parameters then use the last value from the file
                        Dim itfm As Integer = Math.Min(iParIter, Me.BatchData.nTFM)
                        blim = Me.BatchData.tfmBlim(itfm, igrp)
                        bbase = Me.BatchData.tfmBbase(itfm, igrp)
                        fmax = Me.BatchData.tfmFmax(itfm, igrp)
                        fmin = Me.BatchData.tfmFmin(itfm, igrp)

                        Dim iFx As Integer = Math.Min(iParIter, Me.BatchData.nFixedF)
                        f = Me.BatchData.FixedF(iFx, igrp)

                        Dim itac As Integer = Math.Min(iParIter, Me.BatchData.nTAC)
                        tac = Me.BatchData.TAC(itac, igrp)

                End Select

                'set the values based on the RunType selected above
                Me.m_MSEdata.Blim(igrp) = blim
                Me.m_MSEdata.Bbase(igrp) = bbase
                Me.m_MSEdata.Fopt(igrp) = fmax
                Me.m_MSEdata.Fmin(igrp) = fmin

                Me.m_MSEdata.FixedF(igrp) = f
                Me.m_MSEdata.TAC(igrp) = tac
                Me.m_MSEdata.FixedEscapement(igrp) = 0

            Next


        End Sub


        Public Function ReadCommandFile(ByVal CommandFileName As String) As Boolean

            If Me.m_runState = eBatchRunState.Running Then
                'message can't run
                Return False
            End If

            Me.BatchData.Commandfilename = CommandFileName

            If Me.m_fileReader.Read(CommandFileName) Then
                If Me.ValidateData() Then
                    If Me.updateDataStructures() Then
                        Me.checkRunType()
                        Me.postValidationMessage()
                        Me.BatchData.isInit = True
                        Return True
                    End If
                End If
                Return False
            End If

            Me.BatchData.isInit = False
            'message failed to read file
            Return False

        End Function

        Private Sub postValidationMessage()

            Try

                Me.MarshallMessage("")
                Me.MarshallMessage("Run Type:")
                Me.MarshallMessage(vbTab & Me.BatchData.RunType.ToString)

                Me.MarshallMessage("Output directory:")
                Me.MarshallMessage(vbTab & Me.BatchData.OuputDir)

                Dim endYearMsg As String = "End of Ecosim run."
                If Me.MSEData.EndYear > 0 Then
                    endYearMsg = Me.MSEData.EndYear.ToString
                End If

                Me.MarshallMessage("Last control year:")
                Me.MarshallMessage(vbTab & endYearMsg)

                Me.MarshallMessage("Primary production variation:")
                Me.MarshallMessage(vbTab & Me.BatchData.STDevForcing.ToString)

                Me.MarshallMessage("Loaded primary production forcing:")
                For iff As Integer = 1 To Me.BatchData.nForcing
                    Me.MarshallMessage(vbTab & Me.BatchData.ForcingNames(iff))
                Next

                ' Me.checkRunType()

                Me.MarshallMessage("Ready to run.")

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try


        End Sub

        Private Sub checkRunType()
            Dim igrp As Integer
            Dim bFixedEsc As Boolean
            Dim bFixedF As Boolean
            Dim bTAC As Boolean

            Select Case Me.BatchData.RunType

                Case eMSEBatchRunTypes.TFM

                    For igrp = 1 To Me.MSEData.NGroups
                        If Me.m_MSEdata.FixedEscapement(igrp) <> 0 Then bFixedEsc = True
                        If Me.m_MSEdata.FixedF(igrp) <> 0 Then bFixedF = True
                        If Me.m_MSEdata.TAC(igrp) <> 0 Then bTAC = True
                    Next

                Case eMSEBatchRunTypes.Constant_Y
                    For igrp = 1 To Me.MSEData.NGroups
                        If Me.m_MSEdata.FixedEscapement(igrp) <> 0 Then bFixedEsc = True
                        If Me.m_MSEdata.FixedF(igrp) <> 0 Then bFixedF = True
                    Next

                Case eMSEBatchRunTypes.Constant_F
                    For igrp = 1 To Me.MSEData.NGroups
                        If Me.m_MSEdata.FixedEscapement(igrp) <> 0 Then bFixedEsc = True
                        If Me.m_MSEdata.TAC(igrp) <> 0 Then bTAC = True
                    Next

            End Select


            If bFixedEsc Or bFixedF Or bTAC Then
                Me.MarshallMessage("WARNING: values for")

                If bFixedEsc Then
                    Me.MarshallMessage(vbTab & "Fixed Escapement")
                End If
                If bFixedF Then
                    Me.MarshallMessage(vbTab & "Fixed F")
                End If
                If bTAC Then
                    Me.MarshallMessage(vbTab & "Total Allowable Catch")
                End If

                Me.MarshallMessage(vbTab & "Have been set in the user interface.")
                Me.MarshallMessage(vbTab & "Please check these value(s) in the user interface to make sure this is correct.")

            End If


        End Sub


        Private Sub RunThreaded()
            Dim bSuccess As Boolean

            Try

                cLog.Write("MSE batch run started.")

                Me.BatchData.StoreMSEState(Me.MSEData)

                Me.m_runState = eBatchRunState.Running
                Me.m_MSEdata.bInBatch = True
                Me.BatchData.StopRun = False

                Me.MarshallMessage("Starting batch run.")

                Me.m_OutputWriter = New cMSEBatchOutputWriter(Me.Core, Me.m_MSEdata, Me.BatchData)
                Me.m_OutputWriter.InitBatchRun()
                Me.m_OutputWriter.WriteBatchHeader()

                Dim n As Integer = Me.m_BatchData.nForcing * Me.m_BatchData.nControlTypes * Me.m_BatchData.nParIters
                Dim iter As Integer

                For iFor As Integer = 1 To Me.m_BatchData.nForcing
                    Me.m_curForceIter = iFor
                    Me.setForcing(iFor)

                    For iCon As Integer = 1 To Me.m_BatchData.nControlTypes
                        Me.setControls(iCon)

                        For iPar As Integer = 1 To Me.m_BatchData.nParIters
                            Me.setParameters(iPar)
                            Me.m_OutputWriter.WriteIterationHeader(iFor, iCon, iPar)
                            Me.m_OutputWriter.setSimCounter()

                            iter += 1
                            bSuccess = Me.m_MSE.Run()
                            If Not bSuccess Then
                                Me.MarshallMessage("   MSE Error run " & iter.ToString & " of " & n.ToString & " run stopped.")
                                Me.BatchData.StopRun = True
                                Exit For
                            End If
                            Me.MarshallMessage("   Completed " & iter.ToString & " of " & n.ToString)

                            If Me.BatchData.StopRun Then Exit For

                            GC.Collect()

                        Next iPar

                        If Me.BatchData.StopRun Then Exit For
                    Next iCon

                    If Me.BatchData.StopRun Then Exit For
                Next iFor

            Catch ex As Exception

                cLog.Write(ex)
                Me.MarshallMessage("MSE Batch run Exception: " & ex.Message)

            End Try

            Me.m_MSEdata.bInBatch = False
            Me.m_runState = eBatchRunState.Idle

            Me.BatchData.ReStoreMSEState(Me.MSEData)

            Dim msg As String = "Batch run completed."
            If Me.BatchData.StopRun Then msg = "Batch run stopped."

            Me.MarshallMessage(msg)

        End Sub



        Public Sub Run()

            Try

                If Not Me.m_BatchData.isInit Then
                    Me.MarshallMessage("MSE Batch cannot be run. Data failed to initialize.")
                    Return
                End If

                If Me.m_runState = eBatchRunState.Running Then
                    Me.MarshallMessage("MSE Batch already running, please wait for the current run to stop before trying again.")
                    Return
                End If

                m_thrdRun = New System.Threading.Thread(AddressOf Me.RunThreaded)
                m_thrdRun.Start()

            Catch ex As Exception

            End Try

        End Sub

        Private Function updateDataStructures() As Boolean

            Return Me.m_fileReader.updateDataStructures()

        End Function

        Public ReadOnly Property OutputWriter() As IMSEOutputWriter
            Get
                Return Me.m_OutputWriter
            End Get
        End Property

        Public ReadOnly Property BatchData() As cMSEBatchDataStructures
            Get
                Return Me.m_BatchData
            End Get
        End Property


        Friend ReadOnly Property Core() As cCore
            Get
                Return Me.m_core
            End Get
        End Property


        Friend ReadOnly Property MSEData() As cMSEDataStructures
            Get
                Return Me.m_MSEdata
            End Get
        End Property


        Friend ReadOnly Property nGroups() As Integer
            Get
                Return Me.m_MSEdata.NGroups
            End Get
        End Property


        Friend ReadOnly Property nFleets() As Integer
            Get
                Return Me.m_MSEdata.nFleets
            End Get
        End Property

        ''' <summary>
        ''' Load an existing Primary Production forcing function 
        ''' </summary>
        ''' <param name="iShapeIndex">Index of the Forcing Function shape</param>
        ''' <param name="iPPGroupIndex">Ecosim index of the Primary Production group this forcing function applies to</param>
        ''' <remarks></remarks>
        Private Sub LoadPPForcing(ByVal iShapeIndex As Integer, ByVal iPPGroupIndex As Integer)
            'shapes are held in a list Indexed from 0
            iShapeIndex -= 1
            Dim shape As cForcingFunction = Me.m_core.ForcingShapeManager.Item(iShapeIndex)
            Debug.Assert(shape IsNot Nothing, "Invalid PP forcing index.")

            Dim ppi As cMediatedInteraction = Me.m_core.MediatedInteractionManager.PredPreyInteraction(iPPGroupIndex, iPPGroupIndex)
            ppi.LockUpdates = True
            ' Clear all shapes
            For i As Integer = 1 To ppi.MaxNumShapes
                ppi.setShape(i, Nothing)
            Next
            ' Set appropriate shape
            ppi.setShape(1, shape, eForcingFunctionApplication.ProductionRate)

            'Updates the underlying Ecosim data
            ppi.LockUpdates = False


        End Sub

        Public WriteOnly Property onMessageDelegate() As MSEBatchMessage
            Set(ByVal value As MSEBatchMessage)
                Me.m_msgDelegate = value
            End Set
        End Property


        Public Sub MarshallMessage(ByVal message As String)
            Try
                Debug.Assert((Me.m_msgDelegate IsNot Nothing) And (Me.m_SyncOb IsNot Nothing), Me.ToString & ".MarshallMessage() not initialized correctly")
                If (Me.m_msgDelegate IsNot Nothing) And (Me.m_SyncOb IsNot Nothing) Then
                    'marshall the message onto the main thread
                    m_SyncOb.Send(New System.Threading.SendOrPostCallback(AddressOf sendMessage), message)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Private Sub sendMessage(ByVal obj As Object)
            Try
                Dim message As String = DirectCast(obj, String)
                Me.m_msgDelegate.Invoke(message)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & " Error sending message to interface.")
            End Try
        End Sub


    End Class


End Namespace
