'==============================================================================
'
' $Log: cMSEManager.vb,v $
' Revision 1.7  2009/05/15 15:02:33  joeb
' Outputs constructed when the Manager is contructed
'
' Revision 1.6  2009/05/13 17:21:14  joeb
' Split outputs objects into groups and not groups
'
' Revision 1.5  2009/05/11 21:28:07  joeb
' Adding MSE data to Decision Support Tool (Multi Player Game)
'
' Revision 1.4  2009/01/16 18:30:32  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2008/12/09 19:49:15  joeb
' Ouput objects now use core data instead of buffering data
'
' Revision 1.2  2008/11/28 16:54:13  joeb
' Cleaned up ToDo's
'
' Revision 1.1  2008/09/26 07:30:27  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/07/14 01:43:03  jeroens
' Fixed crash when opening for empty Ecosim model
'
' Revision 1.13  2008/06/06 15:56:05  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.12  2008/05/20 15:38:53  joeb
' Parameter object
'
' Revision 1.11  2008/05/12 19:00:17  joeb
' Restructure of search objects to use ISearchObjective interface
'
' Revision 1.10  2008/05/08 15:51:18  joeb
' Fixed broken build setWait() and ReleaseWait()
'
' Revision 1.9  2008/05/05 16:22:32  joeb
' More changes for output object
'
' Revision 1.8  2008/05/01 20:36:16  joeb
' Added output object
'
' Revision 1.7  2008/04/24 20:04:36  joeb
' Now inherits from cThreadedManagerBase
'
' Revision 1.6  2008/04/24 14:53:41  joeb
' Added CVS Log header
'
Option Strict On

Imports EwECore
Imports EwECore.Ecosim
Imports System.Threading
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

Namespace MSE

    ''' <summary>
    ''' Manager class for the MSE (Closed loop simulator in EwE5)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cMSEManager
        Inherits cThreadWaitBase
        Implements ICoreInterface
        Implements ISearchObjective

#Region "Private data"

        Private m_core As cCore
        Private m_MSE As cMSE
        Private m_MSEdata As New cMSEDataStructures
        Private m_search As cSearchDatastructures
        Private m_searchObjective As cSearchObjective

        Private m_InterfaceCallback As MSECallBackDelegate
        Private m_SyncOb As System.Threading.SynchronizationContext
        Private m_bConnected As Boolean

        Private m_lstGroups As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.MSEGroupInput, 1)
        Private m_lstFleets As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.MSEFleetInput, 1)
        Private m_lstGroupOutputs As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.MSEGroupOutputs, 1)
        Private m_output As cMSEOutput
        Private m_parameters As cMSEParameters

        Private m_thrdRun As Thread

#End Region

#Region "Connection and Disconnection"

        Public Sub Connect(ByRef InterfaceCallBack As MSECallBackDelegate)

            m_InterfaceCallback = InterfaceCallBack
            'MSE does not listen to the Ecosim timesteps
            Me.m_core.m_EcoSim.TimeStepDelegate = Nothing
            m_bConnected = True


        End Sub


        Public Sub Disconnect()

            m_InterfaceCallback = Nothing
            m_bConnected = False

        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property FleetInputs(ByVal iFleet As Integer) As cMSEFleetInput
            Get
                Return DirectCast(Me.m_lstFleets(iFleet), cMSEFleetInput)
            End Get
        End Property

        Public ReadOnly Property GroupInputs(ByVal iGroup As Integer) As cMSEGroupInput
            Get
                Return DirectCast(Me.m_lstGroups(iGroup), cMSEGroupInput)
            End Get
        End Property

        Public ReadOnly Property FleetInputs() As cCoreInputOutputList(Of cCoreInputOutputBase)
            Get
                Return Me.m_lstFleets
            End Get
        End Property

        Public ReadOnly Property GroupInputs() As cCoreInputOutputList(Of cCoreInputOutputBase)
            Get
                Return Me.m_lstGroups
            End Get
        End Property

        Public ReadOnly Property Output() As cMSEOutput
            Get
                Return Me.m_output
            End Get
        End Property

        Public ReadOnly Property GroupOutputs() As cCoreInputOutputList(Of cCoreInputOutputBase)
            Get
                Return Me.m_lstGroupOutputs
            End Get
        End Property


        Public ReadOnly Property ModelParameters() As cMSEParameters
            Get
                Return Me.m_parameters
            End Get
        End Property

#End Region

#Region "Construction Initialization and Running of the model"

        Public Sub New(ByVal theCore As cCore)
            Me.m_output = New cMSEOutput(theCore)
            Me.m_parameters = New cMSEParameters(theCore)
        End Sub


        Public Function Run() As Boolean

            Try

                If Me.isRunning Then
                    Me.m_core.Messages.SendMessage(New cMessage("A Management Strategy Evaluation is already running. Only one evaluation can be run at a time.", _
                                                                eMessageType.ErrorEncountered, eCoreComponentType.MSE, eMessageImportance.Critical, eDataTypes.MSEManager))
                    Return False
                End If

                'set the wait object to block all calling threads
                'this will set isRunning to True
                Me.setWait()

                m_thrdRun = New Thread(AddressOf m_MSE.Run)
                m_thrdRun.Start()

            Catch ex As Exception
                cLog.Write(ex)
                Me.ReleaseWait()
                Return False
            End Try

            Return True

        End Function

        Friend Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init

            m_core = theCore
            m_searchObjective = m_core.SearchObjective

            Me.m_SyncOb = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. I'm not sure why this can happen but it was in all the samples...
            If (Me.m_SyncOb Is Nothing) Then Me.m_SyncOb = New System.Threading.SynchronizationContext()

            'Me.m_parameters = New cMSEParameters(Me.m_core)
            'Me.m_output = New cMSEOutput(Me.m_core)

            'cMSEDataStructures are not part of the core!!!!!
            'Only the MSEManager and model know about them 
            'this may have to change when the input/output object are created
            m_MSEdata.Init(theCore)

            m_MSE = New cMSE
            m_MSE.Init(m_MSEdata, m_core.m_EcoSim, m_core.m_SearchData, m_core.m_EcoPathData)

            'connect the MSE model to the manager
            m_MSE.Connect(AddressOf Me.OnMSECallBack)

            'set the MSE model in Ecosim
            'Ecosim calls MSE.AssessFs() if the Search is turned On
            m_core.m_EcoSim.InitMSE(m_MSE)

            'build the Input and Output objects
            Me.m_lstGroups.Clear()
            For igrp As Integer = 1 To m_core.nLivingGroups
                Me.m_lstGroups.Add(New cMSEGroupInput(m_core, m_core.m_EcoPathData.GroupDBID(igrp)))
            Next

            Me.m_lstFleets.Clear()
            For iflt As Integer = 1 To m_core.nFleets
                Me.m_lstFleets.Add(New cMSEFleetInput(m_core, m_core.m_EcoPathData.FleetDBID(iflt)))
            Next

            Me.m_lstGroupOutputs.Clear()
            For igrp As Integer = 1 To m_core.nLivingGroups
                Me.m_lstGroupOutputs.Add(New cMSEGroupOutput(m_core, m_core.m_EcoPathData.GroupDBID(igrp), igrp))
            Next

        End Function

        Friend Function Load() As Boolean Implements ISearchObjective.Load

            Dim coreData As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim iGroup As Integer, iFleet As Integer

            Try
                'Group inputs
                For Each mseGrp As cMSEGroupInput In Me.m_lstGroups
                    mseGrp.AllowValidation = False
                    'convert the Database ID into a group index
                    iGroup = Array.IndexOf(coreData.GroupDBID, mseGrp.DBID)

                    mseGrp.Index = iGroup
                    mseGrp.BiomassCV = Me.m_MSEdata.CVbiomEst(mseGrp.Index)
                    mseGrp.UpperRisk = Me.m_MSEdata.BioRiskValue(mseGrp.Index, 1)
                    mseGrp.LowerRisk = Me.m_MSEdata.BioRiskValue(mseGrp.Index, 0)

                    mseGrp.ResetStatusFlags()
                    mseGrp.AllowValidation = True
                Next

                'Group outputs just the index outputs will be populated in LoadOutputs() at each iteration
                For Each mseOutput As cMSEGroupOutput In Me.m_lstGroupOutputs
                    mseOutput.AllowValidation = False 'no validation of outputs
                    mseOutput.Index = Array.IndexOf(coreData.GroupDBID, mseOutput.DBID)
                Next

                'fleets
                For Each mseFlt As cMSEFleetInput In Me.m_lstFleets
                    mseFlt.AllowValidation = False
                    'convert the Database ID into a fleet index
                    iFleet = Array.IndexOf(coreData.FleetDBID, mseFlt.DBID)

                    mseFlt.Index = iFleet
                    mseFlt.QIncrease = Me.m_MSEdata.Qgrow(mseFlt.Index)

                    For igrp As Integer = 1 To m_core.nLivingGroups
                        mseFlt.FleetWeight(igrp) = Me.m_MSEdata.Fweight(mseFlt.Index, igrp)
                    Next
                    mseFlt.ResetStatusFlags()
                    mseFlt.AllowValidation = True
                Next


                m_parameters.AllowValidation = False
                m_parameters.AssessmentMethod = Me.m_MSEdata.AssessMethod
                m_parameters.AssessPower = Me.m_MSEdata.AssessPower

                'Use the first array element as the interface value
                'Copied from EwE5
                Try
                    m_parameters.ForcastGain = Me.m_MSEdata.GstockPred(1)
                    m_parameters.KalmanGain = Me.m_MSEdata.KalmanGain(1)
                Catch ex As Exception

                End Try
                m_parameters.NTrials = Me.m_MSEdata.NTrials

                m_parameters.AllowValidation = True

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".Load() Error: " & ex.Message, ex)
            End Try

        End Function

        ''' <summary>
        ''' Update the underlying core data with edits from the interface
        ''' </summary>
        ''' <remarks>This is called by the core when a variable passes validation via cCore.OnValidated()</remarks>
        Public Function Update(ByVal DataType As eDataTypes) As Boolean Implements ISearchObjective.Update

            Try
                Select Case DataType

                    Case eDataTypes.MSEGroupInput

                        For Each mseGrp As cMSEGroupInput In Me.m_lstGroups
                            Me.m_MSEdata.CVbiomEst(mseGrp.Index) = mseGrp.BiomassCV
                        Next

                    Case eDataTypes.MSEFleetInput

                        For Each mseFlt As cMSEFleetInput In Me.m_lstFleets
                            Me.m_MSEdata.Qgrow(mseFlt.Index) = mseFlt.QIncrease
                            For igrp As Integer = 1 To m_core.nLivingGroups
                                Me.m_MSEdata.Fweight(mseFlt.Index, igrp) = mseFlt.FleetWeight(igrp)
                            Next igrp
                        Next mseFlt


                    Case eDataTypes.MSEParameters

                        For igrp As Integer = 1 To m_core.nLivingGroups
                            Me.m_MSEdata.GstockPred(igrp) = Me.m_parameters.ForcastGain
                            Me.m_MSEdata.RstockPred(igrp) = (1 - Me.m_MSEdata.GstockPred(igrp)) * m_core.StartBiomass(igrp)
                        Next igrp

                        For igrp As Integer = 1 To m_core.nLivingGroups
                            Me.m_MSEdata.KalmanGain(igrp) = Me.m_parameters.KalmanGain
                        Next igrp

                        Me.m_MSEdata.AssessMethod = Me.m_parameters.AssessmentMethod()
                        Me.m_MSEdata.AssessPower = Me.m_parameters.AssessPower()
                        Me.m_MSEdata.NTrials = Me.m_parameters.NTrials()

                End Select

                System.Console.WriteLine(Me.ToString & ".Update(" & DataType.ToString & ")")

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Function

#End Region

#Region "Model communication callback delegates for model and interface"


        ''' <summary>
        ''' Callback handler called by the MSE model
        ''' </summary>
        ''' <param name="CallBackType"></param>
        ''' <remarks></remarks>
        Private Sub OnMSECallBack(ByVal CallBackType As eCallBackTypes)
            'this is called on the MSE worker thread
            'so even if the main thread has called Wait() and is blocking this code will be processed and the MSE can continue 

            Try

                'do any processing based on the type of callback even if there is no interface connected
                Me.ProcessCallBack(CallBackType)

                'At this time it is possible to run the manager without it being connected to an interface
                'This is so it can be run as a TOOL or as part of a Plugin process without calling an interface
                If m_bConnected Then

                    'make sure something didn't screwup
                    Debug.Assert(m_SyncOb IsNot Nothing And m_InterfaceCallback IsNot Nothing, Me.ToString & ".OnMSECallBack() not connected properly.")

                    'Connected so call the interface
                    m_SyncOb.Send(New System.Threading.SendOrPostCallback(AddressOf fireCallBack), CallBackType)

                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        Private Sub fireCallBack(ByVal obj As Object)
            Try
                Debug.Assert(m_SyncOb IsNot Nothing And m_InterfaceCallback IsNot Nothing, Me.ToString & ".OnMSECallBack() not connected properly.")
                Dim cbType As eCallBackTypes = DirectCast(obj, eCallBackTypes)
                m_InterfaceCallback.Invoke(cbType)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & " Error sending message to interface.")
            End Try
        End Sub


        Private Sub ProcessCallBack(ByVal CallBackType As eCallBackTypes)

            System.Console.WriteLine(Me.ToString & " Callback type = " & CallBackType.ToString)

            Select Case CallBackType

                Case eCallBackTypes.IterationCompleted

                    'populate output objects for this iteration
                    Me.LoadOutputs()

                Case eCallBackTypes.IterationStarted

                Case eCallBackTypes.Started

                Case eCallBackTypes.Stopped

                    Me.LoadOutputs()
                    'the thread has completed its task
                    'clear the signal state of the thread this will release any threads that called Wait()
                    Me.ReleaseWait()

            End Select

        End Sub

        ''' <summary>
        ''' Load results of trial(s) into output object
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub LoadOutputs()

            'Results of the trials are outputs only and are not validated AllowValidation = False
            'the Status is hardwired in .ResetStatusFlags()
            'If a Validation object is used it must be made thread safe as this is run on the MSE worker thread

            Me.m_output.TrialNumber = Me.m_MSEdata.CurrentIteration

            Me.m_output.BestTotalValue = Me.m_MSEdata.BestTotalValue
            Me.m_output.MeanTotalValue = Me.m_MSEdata.MeanTotalValue / Me.m_MSEdata.CurrentIteration
            Me.m_output.MeanEcologicalValue = Me.m_MSEdata.MeanEcoVal / Me.m_MSEdata.CurrentIteration
            Me.m_output.MeanEmployValue = Me.m_MSEdata.MeanEmploy / Me.m_MSEdata.CurrentIteration
            Me.m_output.MeanMandatedValue = Me.m_MSEdata.MeanManVal / Me.m_MSEdata.CurrentIteration

            Me.m_output.TotalValue = Me.m_MSEdata.BestTotalValue
            Me.m_output.EcologicalValue = Me.m_MSEdata.BaseEcoVal
            Me.m_output.EmployValue = Me.m_MSEdata.BaseEmployVal
            Me.m_output.MandatedValue = Me.m_MSEdata.BaseManValue

            Dim nt As Integer = m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
            For Each grp As cMSEGroupOutput In Me.m_lstGroupOutputs
                Dim igrp As Integer = grp.Index
                grp.LowerRiskCount = Me.m_MSEdata.BioRiskCount(igrp, 0)
                grp.UpperRiskCount = Me.m_MSEdata.BioRiskCount(igrp, 1)

                For t As Integer = 1 To nt
                    grp.Biomass(t) = Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, t)
                Next

            Next grp


        End Sub

#End Region

#Region "ICoreInterface"

        Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
            Get
                Return eDataTypes.MSEManager
            End Get
        End Property

        Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.EcoSpace
            End Get
        End Property

        Public Property DBID() As Integer Implements ICoreInterface.DBID
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property

        Public Function GetID() As String Implements ICoreInterface.GetID
            Return cCore.NULL_VALUE.ToString
        End Function

        Public Property Index() As Integer Implements ICoreInterface.Index
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property

        Public Property Name() As String Implements ICoreInterface.Name
            Get
                Return Me.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property

#End Region

#Region "ISearchObjective"

        Public ReadOnly Property FleetObjectives(ByVal iFleet As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
            Get
                Return Me.m_searchObjective.FleetObjectives(iFleet)
            End Get
        End Property

        Public ReadOnly Property GroupObjectives(ByVal iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
            Get
                Return Me.m_searchObjective.GroupObjectives(iGroup)
            End Get
        End Property

        Public ReadOnly Property ValueWeights() As cSearchObjectiveWeights Implements ISearchObjective.ValueWeights
            Get
                Return Me.m_searchObjective.ValueWeights
            End Get
        End Property

        Public ReadOnly Property ObjectiveParameters() As SearchObjectives.cSearchObjectiveParameters Implements SearchObjectives.ISearchObjective.ObjectiveParameters
            Get
                Return Me.m_searchObjective.ObjectiveParameters
            End Get
        End Property

#End Region


    
    End Class

End Namespace





