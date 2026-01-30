' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwECore.Ecosim
Imports EwECore.FitToTimeSeries
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public MustInherit Class cSFPGenericIteration
    Implements ISFPIteration

#Region " Private variables "

    ''' <summary>Calculated Sum of Squares</summary>
    Private m_ss As Single = 0
    ''' <summary>Calculated AIC</summary>
    Private m_aic As Single = 0
    ''' <summary>Calculated AICc</summary>
    Private m_aicc As Single = 0
    ''' <summary>Anomaly shape data</summary>
    Private m_anomalyshape() As Single = Nothing
    ''' <summary>Vulnerabilities data</summary>
    Private m_vulnerabilities(,) As Single = Nothing
    ''' <summary>Calculated time series SS results</summary>
    Private m_timeseriesSS As Single()

    Private m_lRunMessages As New List(Of String)

    Private m_parameters As cSFPParameters = Nothing

    Private m_report As New List(Of String)
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSFPGenericIteration)()

#End Region ' Private variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="Params"></param>
    ''' -----------------------------------------------------------------------
    Protected Sub Init(core As EwECore.cCore, params As cSFPParameters) _
        Implements ISFPIteration.Init

        'Get variables needed for SFP iteration
        Me.m_parameters = params

        ' This is expected throughout this class
        Debug.Assert(Me.Parameters.TimeSeriesDataset >= 1)

        ' Allocate memory for anomaly shape
        ReDim Me.m_anomalyshape(core.nEcosimTimeSteps)

        ' Allocate memory for vulnerabilities matrix
        ReDim Me.m_vulnerabilities(core.nGroups, core.nGroups)

        'Allocate memory for time series SS results
        ReDim Me.m_timeseriesSS(core.TimeSeriesDataset(Me.Parameters.TimeSeriesDataset).nTimeSeries)

    End Sub

    Public Sub InitRun() Implements ISFPIteration.InitRun
        Me.Clear()
        Me.m_report.Clear()
        Me.RunState = If(Me.Enabled, ISFPIteration.eRunState.Pending, ISFPIteration.eRunState.Idle)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Load"/>
    ''' <remarks>
    ''' The baseline implementation makes sure that non-fit to time series
    ''' parameters are transfered to the temporary core.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Load(core As cCore) As Boolean _
        Implements ISFPIteration.Load

        core.EcosimModelParameters.VulnerabilityCap = Me.Parameters.VulCap
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Run"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Run(core As cCore) As Boolean _
        Implements ISFPIteration.Run

        Me.Clear()

        'Run EcoSim
        Me.RunEcosim(core)

        ' Store calculated values
        Me.m_ss = Me.GetSS(core)
        Me.m_aic = Me.GetAIC(core)
        Me.m_aicc = Me.GetAICc(core)

        ' Store vulnerabilities
        For i As Integer = 1 To core.nGroups
            Dim grp As cEcosimGroupInput = core.EcosimGroupInputs(i)
            For j As Integer = 1 To core.nGroups
                Me.m_vulnerabilities(i, j) = grp.VulMult(j)
            Next
        Next

        ' Store first anomaly shape
        Me.GetAnomalyShape(core)
        ' Store time series SS
        Me.GetTimeSeriesSS(core)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Apply"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Apply(core As cCore) As Boolean _
        Implements ISFPIteration.Apply

        If (Not Me.RunState = ISFPIteration.eRunState.Completed) Then Return False

        core.SetBatchLock(cCore.eBatchLockType.Update)

        ' ToDo: add error checking!
        Try
            ' Enable time series if baseline or fishing
            Me.EnableTimeSeries(core)

            ' Restore vulnerabilities
            For i As Integer = 1 To core.nGroups
                Dim grp As cEcosimGroupInput = core.EcosimGroupInputs(i)
                For j As Integer = 1 To core.nGroups
                    grp.VulMult(j) = Me.m_vulnerabilities(i, j)
                Next
            Next

            'Restore anomaly shape
            Dim shape As cShapeData = Me.GetAppliedShape(core)
            If (shape IsNot Nothing) Then
                shape.ShapeData = Me.m_anomalyshape
                shape.Update()
            End If

        Catch ex As Exception
            ' Whoah!
            ' ToDo: add error feedback!
        End Try

        core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enable only time series specific to Baseline or Fishing and apply to the Ecosim model
    ''' </summary>
    ''' <param name="core">The run core to apply to.</param>
    ''' -----------------------------------------------------------------------
    Protected Function EnableTimeSeries(core As cCore) As Boolean

        Dim iTS As Integer = Me.Parameters.TimeSeriesDataset
        Dim dataset As cTimeSeriesDataset = core.TimeSeriesDataset(iTS)
        Dim man As cF2TSManager = core.EcosimFitToTimeSeries

        'Reset fishing effort shapes
        core.FishingEffortShapeManager.ResetToDefaults()
        Me.Report(My.Resources.REPORT_RESET_EFFORT, eReportState.Success)

        ' Quick count
        Dim nRef As Integer = 0
        Dim nDrv As Integer = 0

        Select Case Me.BaseSearchMode

            Case ISFPIteration.eBaseSearchMode.Baseline
                'Go through each time series of the time series dataset
                For i As Integer = 1 To dataset.nTimeSeries

                    Dim ts As cTimeSeries = dataset.TimeSeries(i)

                    ''If the time series type is 0, 1, 5, 6, 7 enable it
                    'Select Case ts.TimeSeriesType
                    '    Case eTimeSeriesType.BiomassRel,
                    '         eTimeSeriesType.TotalMortality,
                    '         eTimeSeriesType.Catches,
                    '         eTimeSeriesType.CatchesRel,
                    '         eTimeSeriesType.AverageWeight
                    '        ts.Enabled = Me.Parameters.OriginalTimeSeriesEnabled(i)
                    '        ts.WtType = Me.Parameters.TimeSeriesWeight(i)
                    '    Case eTimeSeriesType.BiomassAbs
                    '        ts.Enabled = (Me.Parameters.EnableAbsoluteBiomassTimeSeries And Me.Parameters.TimeSeriesEnabled(i))
                    '        ts.WtType = Me.Parameters.TimeSeriesWeight(i)
                    '    Case Else
                    '        ts.Enabled = False
                    'End Select

                    If ts.IsReference Then
                        ts.Enabled = Me.Parameters.OriginalTimeSeriesEnabled(i)
                        ts.WtType = Me.Parameters.OriginalTimeSeriesWeight(i)
                        If ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                            ts.Enabled = ts.Enabled And Me.Parameters.EnableAbsoluteBiomassTimeSeries
                            nRef += 1
                        Else
                            nRef += 1
                        End If
                    Else
                        ts.Enabled = False
                        nDrv += 0 ' Teehee
                    End If
                Next

            Case ISFPIteration.eBaseSearchMode.Fishing
                'Go through each time series of the time series dataset
                For i As Integer = 1 To dataset.nTimeSeries
                    'Enable Time Series
                    Dim ts As cTimeSeries = dataset.TimeSeries(i)
                    If (ts.IsReference) Then nRef += 1 Else nDrv += 1

                    ts.Enabled = Me.Parameters.OriginalTimeSeriesEnabled(i)
                    ts.WtType = Me.Parameters.OriginalTimeSeriesWeight(i)
                Next

            Case Else
                Debug.Assert(False, "Unsupported enum")

        End Select

        'Apply the enabled time series
        core.UpdateTimeSeries(False)

        Me.Report(cStringUtils.Localize(My.Resources.REPORT_ENABLED_TIMESERIES, nRef, nDrv, dataset.nTimeSeries), eReportState.Success)

        Return True

    End Function

#Region " Accessors "

    Public Property Parameters As cSFPParameters Implements ISFPIteration.Parameters
        Get
            Return Me.m_parameters
        End Get
        Protected Set(value As cSFPParameters)
            Me.m_parameters = value
        End Set
    End Property

    Public Property k As Integer = 0 Implements ISFPIteration.K
    Public Property EstimatedV As Integer = 0 Implements ISFPIteration.EstimatedV
    Public Property SplinePoints As Integer = 0 Implements ISFPIteration.SplinePoints
    Public Property BaseSearchMode As ISFPIteration.eBaseSearchMode Implements ISFPIteration.BaseSearchMode
    Public Property Enabled As Boolean = True Implements ISFPIteration.Enabled
    Public Property RunState As ISFPIteration.eRunState = ISFPIteration.eRunState.Idle Implements ISFPIteration.RunState
    Public Property Elapsed As TimeSpan Implements ISFPIteration.Elapsed
    Public Property Completed As Date Implements ISFPIteration.Completed

    Public ReadOnly Property RunStateMessages As String() Implements ISFPIteration.RunStateMessages
        Get
            Return Me.m_lRunMessages.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.SS"/>
    ''' -----------------------------------------------------------------------
    Public Property SS() As Single _
        Implements ISFPIteration.SS
        Get
            If (Me.RunState <> ISFPIteration.eRunState.Completed) Then Return 0
            Return Me.m_ss
        End Get
        Friend Set(value As Single)
            Me.m_ss = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.AIC"/>
    ''' -----------------------------------------------------------------------
    Public Property AIC() As Single _
        Implements ISFPIteration.AIC
        Get
            If (Me.RunState <> ISFPIteration.eRunState.Completed) Then Return 0
            Return Me.m_aic
        End Get
        Friend Set(value As Single)
            Me.m_aic = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.AICc"/>
    ''' -----------------------------------------------------------------------
    Public Property AICc() As Single _
        Implements ISFPIteration.AICc
        Get
            If (Me.RunState <> ISFPIteration.eRunState.Completed) Then Return 0
            Return Me.m_aicc
        End Get
        Friend Set(value As Single)
            Me.m_aicc = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.IsBestFit"/>
    ''' -----------------------------------------------------------------------
    Public Property IsBestFit As Boolean = False _
        Implements ISFPIteration.IsBestFit

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.AnomalyShape"/>
    ''' -----------------------------------------------------------------------
    Public Function AnomalyShape() As Single() _
        Implements ISFPIteration.AnomalyShape
        Return Me.m_anomalyshape
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Vulnerabilities"/>
    ''' -----------------------------------------------------------------------
    Public Function Vulnerabilities() As Single(,) _
        Implements ISFPIteration.Vulnerabilities
        Return Me.m_vulnerabilities
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.TimeSeriesSS"/>
    ''' -----------------------------------------------------------------------
    Public Property TimeSeriesSS As Single() _
          Implements ISFPIteration.TimeSeriesSS
        Get
            If (Me.RunState <> ISFPIteration.eRunState.Completed) Then Return Nothing
            Return Me.m_timeseriesSS
        End Get
        Friend Set(value As Single())
            Me.m_timeseriesSS = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Report()"/>
    ''' -----------------------------------------------------------------------
    Public Function Report() As String Implements ISFPIteration.Report

        If (Me.RunState <> ISFPIteration.eRunState.Completed) Or (Me.RunState <> ISFPIteration.eRunState.Error) Then Return ""

        Dim sb As New StringBuilder()
        sb.AppendLine(String.Format("Iteration '{0}', SS {1}, AIC {2}, AICC {3}", Me.Name, Me.m_ss, Me.m_aic, Me.m_aicc))
        For i As Integer = 0 To Me.m_report.Count - 1
            sb.AppendLine(" - " & Me.m_report(i))
        Next
        Return sb.ToString()

    End Function

#End Region ' Accessors

#Region " Running "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check if Ecosim has non-default vulnerabilities, and if so, reset the
    ''' vulnerabilties. 
    ''' </summary>
    ''' <returns>True if Ecosim has all default vulnerabilties.</returns>
    ''' -----------------------------------------------------------------------
    Protected Function ResetVs(core As cCore) As Boolean
        ' Skip V resetting if requested
        If (Not Me.Parameters.ResetVsOnRun) Then Return True
        ' Suppress prompt, just reset the vulnerabilities without asking
        If core.CheckResetDefaultVulnerabilities(True) Then
            Me.Report(My.Resources.REPORT_RESET_V, eReportState.Success)
            Return True
        Else
            Me.Report(My.Resources.REPORT_RESET_V, eReportState.Skipped)
            Return False
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Sensitivity search according to user input
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunSensitivityOfSSToV(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim msg As String = ""
        Dim bOK As Boolean = False

        'Set the number of blocks selected to Max K
        man.nBlockCodes = Me.Parameters.K

        'If PredOrPredPreySSToV = true then run SS2VBy Predator
        Select Case Me.Parameters.VulSearchMode
            Case ISFPIteration.eVulSearchMode.Predator
                msg = cStringUtils.Localize(My.Resources.REPORT_RUN_SENSV_PRED, Me.Parameters.K)
                If man.RunSensitivitySS2VByPredator(True, TriState.False) Then
                    Debug.Assert(Not man.IsRunning)
                    'Set vulnerabiltiy blocks
                    man.setNBlocksFromSensitivity(Me.Parameters.K)
                    Me.Report(msg, eReportState.Success)
                    bOK = True
                Else
                    Me.Report(msg, eReportState.Error)
                End If
            Case ISFPIteration.eVulSearchMode.PredPrey
                msg = cStringUtils.Localize(My.Resources.REPORT_RUN_SENSV_PREDPREY, Me.Parameters.K)
                If man.RunSensitivitySS2VByPredPrey(True, TriState.False) Then
                    Debug.Assert(Not man.IsRunning)
                    'Set vulnerabiltiy blocks
                    man.setNBlocksFromSensitivity(Me.Parameters.K)
                    Me.Report(msg, eReportState.Success)
                    bOK = True
                Else
                    Me.Report(msg, eReportState.Error)
                End If
            Case Else
                Debug.Assert(False, "Unsupported enum")
        End Select

        Return bOK

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch an Ecosim run.
    ''' </summary>
    ''' <returns>True if a run started successfully.</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunEcosim(core As cCore) As Boolean
        Dim bSuccess As Boolean = core.RunEcosim(Nothing, False)
        Me.Report(My.Resources.REPORT_RUN_ECOSIM, If(bSuccess, eReportState.Success, eReportState.Error))
        Return bSuccess
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset the FF shape. 
    ''' </summary>
    ''' <returns>Always returns true, even if there may not be a shape to reset.</returns>
    ''' -----------------------------------------------------------------------
    Protected Function ResetFF(core As cCore) As Boolean

        Dim sDefaultValue As Single = 1.0
        Dim shape As cShapeData = Me.GetAppliedShape(core)

        'Reset all applied shapes 
        If (shape IsNot Nothing) Then
            For i As Integer = 0 To shape.nPoints
                shape.ShapeData(i) = sDefaultValue
            Next i
            shape.Update()
            Me.Report(cStringUtils.Localize(My.Resources.REPORT_RESET_SPLINE, shape.Name), eReportState.Success)
        Else
            Me.Report(cStringUtils.Localize(My.Resources.REPORT_RESET_SPLINE, ""), eReportState.Error, My.Resources.REPORT_ERROR_NO_SPLINE)
        End If

        ' #1421: do not affect other shapes
        ''More than one shape can be applied so reset the other shapes 
        'Dim interactions As cMediatedInteractionManager = core.MediatedInteractionManager
        'For Each shape In core.ForcingShapeManager
        '    If interactions.IsApplied(shape) Then
        '        For i As Integer = 0 To shape.nPoints
        '            shape.ShapeData(i) = sDefaultValue
        '        Next i
        '        shape.Update()
        '    End If
        'Next

        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the actual selected PP shape.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Protected Function GetAppliedShape(core As cCore) As cShapeData
        Dim man As cForcingFunctionShapeManager = core.ForcingShapeManager
        If (Me.Parameters.AnomalyShapeIndex > 0) Then
            Return man(Me.Parameters.AnomalyShapeIndex - 1)
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Vulnerability Search iterations according to k estimated parameters
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunVulnerabilitySearch(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim msg As String = cStringUtils.Localize(My.Resources.REPORT_RUN_V, Me.EstimatedV)

        'Setup manager to do a vunerability search
        man.VulnerabilitySearch = True
        man.AnomalySearch = False
        man.VulnerabilityVariance = 10.0
        'Set the number of blocks selected (Number of parameters to estimate)
        man.nBlockCodes = Me.EstimatedV

        ' Run the search silently
        If man.RunSearch(True, TriState.False) Then
            Me.Report(msg, eReportState.Success)
            Return True
        Else
            Me.Report(msg, eReportState.Error)
            Return False
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Anomaly Search according to spline point estimated parameters. This search will only run if a FF is applied to a PP.
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunAnomalySearch(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim iTS As Integer = Me.Parameters.TimeSeriesDataset
        Dim msg As String = cStringUtils.Localize(My.Resources.REPORT_RUN_SPLINE, Me.SplinePoints)
        Dim bSuccess As Boolean = False

        'If there is no applied shape do not run search (This is already checked by the cSFPManager but just to make sure)
        If (Me.Parameters.AnomalyShapeIndex > 0) Then

            'Setup manager to do a Anomaly search
            man.AnomalySearch = True
            man.VulnerabilitySearch = False
            man.FirstYear = 1
            man.LastYear = core.TimeSeriesDataset(iTS).NumPoints
            man.PPVariance = 0.1
            'Set the number of spline points selected (Number of parameters to estimate)
            man.NumSplinePoints = Me.SplinePoints
            man.AnomalySearchShapeNumber = Me.Parameters.AnomalyShapeIndex

            ' Run the search silently
            If man.RunSearch(True, TriState.False) Then
                Debug.Assert(Not man.IsRunning)
                bSuccess = True
                Me.Report(msg, eReportState.Success)
            Else
                Me.Report(msg, eReportState.Error)
            End If
        Else
            Me.Report(msg, eReportState.Error, My.Resources.REPORT_ERROR_NO_SPLINE)
        End If

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Vunerability and Anomaly Search according estimated parameters and spline points. This search will only run if a FF is applied to a PP.
    ''' </summary>
    ''' <returns>True if iterations successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunVandASearch(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim iTS As Integer = Me.Parameters.TimeSeriesDataset
        Dim msg As String = cStringUtils.Localize(My.Resources.REPORT_RUN_SPLINE, Me.EstimatedV, Me.SplinePoints)
        Dim bSuccess As Boolean = False

        'If there is an applied shape and a sensitivity search has been ran : run the search
        If (Me.Parameters.AnomalyShapeIndex > 0) And man.HasRunSens Then

            'Setup manager to do a Vulnerability and Anomaly search
            man.AnomalySearch = True
            man.FirstYear = 1
            man.LastYear = core.TimeSeriesDataset(iTS).NumPoints
            man.PPVariance = 0.1
            'Set the number of spline points selected (Number of parameters to estimate)
            man.NumSplinePoints = Me.SplinePoints
            man.AnomalySearchShapeNumber = Me.Parameters.AnomalyShapeIndex

            man.VulnerabilitySearch = True
            'Set the number of blocks selected (Number of parameters to estimate)
            man.nBlockCodes = Me.EstimatedV
            man.VulnerabilityVariance = 10.0

            ' Run the search silently
            If man.RunSearch(True, TriState.False) Then
                Debug.Assert(Not man.IsRunning)
                bSuccess = True
                Me.Report(msg, eReportState.Success)
            Else
                Me.Report(msg, eReportState.Error)
            End If
        Else
            Me.Report(msg, eReportState.Error, My.Resources.REPORT_ERROR_NO_SPLINE)
        End If

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Wipe prior to a run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Clear()

        Me.m_lRunMessages.Clear()

        ' Do not delete memory allocate in init. Wiping array content should be enough
        'Me.m_ss = 0
        'Me.m_aic = 0
        'Me.m_aicc = 0
        'Me.m_timeseriesSS = Nothing
        'Me.m_anomalyshape = Nothing
        'Me.m_vulnerabilities = Nothing
        'Me.m_fishmortshapes.Clear()

    End Sub

#End Region ' Running

#Region " Formatting "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the iteration base name.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Function BaselineOrFishing() As String
        Select Case Me.BaseSearchMode
            Case ISFPIteration.eBaseSearchMode.Baseline
                Return My.Resources.NAME_BASELINE
            Case ISFPIteration.eBaseSearchMode.Fishing
                Return My.Resources.NAME_FISHING
            Case Else
                Debug.Assert(False, "Unsupported enum")
        End Select
        Return "?"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the name of the hypotheses for this iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property Name() As String _
        Implements ISFPIteration.Name
        Get
            'If simple run
            If (Me.EstimatedV = 0 And Me.SplinePoints = 0) Then
                Return Me.BaselineOrFishing()
            End If
            'If Vunerability Search
            If (Me.EstimatedV > 0 And Me.SplinePoints = 0) Then
                Return cStringUtils.Localize(My.Resources.NAME_BASE_AND_V, Me.BaselineOrFishing, Me.EstimatedV)
            End If
            'If Anomaly Search
            If (Me.EstimatedV = 0 And Me.SplinePoints > 0) Then
                Return cStringUtils.Localize(My.Resources.NAME_BASE_AND_SPLINE, Me.BaselineOrFishing, Me.SplinePoints)
            End If
            'Fall-through: V and A Search
            Return cStringUtils.Localize(My.Resources.NAME_BASE_AND_V_AND_SPLINE, Me.BaselineOrFishing, Me.EstimatedV, Me.SplinePoints)
        End Get
    End Property

    Public Overridable ReadOnly Property IsGroupsWithTimeSeriesOnly As Boolean Implements ISFPIteration.IsGroupsWithTimeSeriesOnly
        Get
            Return False
        End Get
    End Property

#End Region ' Formatting

#Region " Internals "

    Protected Enum eReportState As Integer
        Skipped
        Success
        [Error]
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Adds an entry to the report.
    ''' </summary>
    ''' <param name="msg">The msg to add to the report.</param>
    ''' <param name="state">Report statel</param>
    ''' <param name="detail">Optional details to add</param>
    ''' -----------------------------------------------------------------------
    Protected Sub Report(msg As String, state As eReportState, Optional detail As String = "")

        Dim succ As String = ""
        Select Case state
            Case eReportState.Error : succ = My.Resources.REPORT_ERROR
            Case eReportState.Skipped : succ = My.Resources.REPORT_SKIPPED
            Case eReportState.Success : succ = My.Resources.REPORT_SUCCESS
        End Select

        Dim entry As String = ""
        If String.IsNullOrEmpty(detail) Then
            entry = cStringUtils.Localize(My.Resources.REPORT_FORMAT, msg, succ)
        Else
            entry = cStringUtils.Localize(My.Resources.REPORT_FORMAT_DETAIL, msg, succ, detail)
        End If

        Me.m_report.Add(entry)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a run status message for the user.
    ''' </summary>
    ''' <param name="msg"></param>
    ''' -----------------------------------------------------------------------
    Protected Sub AppendRunStateMessage(msg As String)
        If String.IsNullOrWhiteSpace(msg) Then Return
        Me.m_lRunMessages.Add(msg)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current value of SS.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetSS(core As cCore) As Single

        If (Me.EstimatedV = 0 And Me.SplinePoints = 0) Then
            Return core.EcosimStats.SS
        Else
            Dim man As cF2TSManager = core.EcosimFitToTimeSeries
            Dim res As cSearchResults = DirectCast(man.Results, cSearchResults)
            Return res.IterSS
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current value of AIC.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetAIC(core As cCore) As Single

        Debug.Assert(core IsNot Nothing)
        Debug.Assert(Me.Parameters IsNot Nothing)

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim nData As Integer = Me.Parameters.NumberOfObservations

        'If simple run
        If (Me.EstimatedV = 0 And Me.SplinePoints = 0) Then
            Return man.getAIC(0, nData, Me.GetSS(core))
        End If
        'If Vunerability Search
        If (Me.EstimatedV > 0 And Me.SplinePoints = 0) Then
            Return man.getAIC(Me.EstimatedV, nData, Me.GetSS(core))
        End If
        'If Anomaly Search
        If (Me.EstimatedV = 0 And Me.SplinePoints > 0) Then
            Return man.getAIC(Me.SplinePoints, nData, Me.GetSS(core))
        Else 'V and A Search
            Return man.getAIC(Me.k, nData, Me.GetSS(core))
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current value of AICc.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetAICc(core As cCore) As Single

        Debug.Assert(core IsNot Nothing)
        Debug.Assert(Me.Parameters IsNot Nothing)

        Dim nData As Integer = Me.Parameters.NumberOfObservations
        Dim answer As Single = 0

        'If simple run
        If (Me.EstimatedV = 0 And Me.SplinePoints = 0) Then
            answer = Me.GetAIC(core) + 2.0F * 0.0F * (0.0F - 1.0F) / (nData - 0.0F - 1.0F)
            Return answer
        End If
        'If Vunerability Search
        If (Me.EstimatedV > 0 And Me.SplinePoints = 0) Then
            answer = Me.GetAIC(core) + 2.0F * Me.EstimatedV * (Me.EstimatedV - 1.0F) / (nData - Me.EstimatedV - 1.0F)
            Return answer
        End If
        'If Anomaly Search
        If (Me.EstimatedV = 0 And Me.SplinePoints > 0) Then
            answer = Me.GetAIC(core) + 2.0F * Me.SplinePoints * (Me.SplinePoints - 1.0F) / (nData - Me.SplinePoints - 1.0F)
            Return answer
        Else 'V and A Search
            Debug.Assert(Me.k <> 0, "JS debugging")
            answer = Me.GetAIC(core) + 2.0F * Me.k * (Me.k - 1.0F) / (nData - Me.k - 1.0F)
            Return answer
        End If

    End Function

    Private Sub GetTimeSeriesSS(core As cCore)

        Dim iTS As Integer = Me.Parameters.TimeSeriesDataset
        For i As Integer = 1 To core.nTimeSeries
            Me.m_timeseriesSS(i) = core.TimeSeriesDataset(iTS).TimeSeries(i).DataSS
        Next

    End Sub

    Private Sub GetAnomalyShape(core As cCore)
        Dim shape As cShapeData = Me.GetAppliedShape(core)
        If (shape IsNot Nothing) Then
            core.ForcingShapeManager.Load()
            Me.m_anomalyshape = shape.ShapeData
        End If
    End Sub

#End Region ' Internals

#Region " Serialization and output writing "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save Ecosim run results of an iteration to file.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveResults(core As cCore) As Boolean Implements ISFPIteration.SaveResults

        Dim strIterationPath As String = Path.Combine(Me.Parameters.IterationOutputFolder, cFileUtils.ToValidFileName(Me.Name, False))
        Dim bSuccess As Boolean = True

        If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Ecosim) Or
           (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then

            If cFileUtils.IsDirectoryAvailable(strIterationPath, True) Then
                Dim wsim As New Ecosim.cEcosimResultWriter(core)
                Try
                    If wsim.WriteResults(strIterationPath, bQuiet:=True) Then
                        Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_SUCCESS, My.Resources.DETAIL_ECOSIM, strIterationPath))
                        bSuccess = True
                    Else
                        Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_ECOSIM, ""))
                        bSuccess = False
                    End If
                Catch ex As Exception
                    ' This REALLY should not happen
                    m_logger.LogError(ex, "cSFPManager.SaveIterationResults(Ecosim)")
                    Debug.Assert(False, ex.Message)
                End Try
            End If
        End If

        If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Aggregated) Or
           (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then

            'Save output results in Monthly and Yearly format 
            Me.SaveAggregatedResults(core, strIterationPath, True)
            Me.SaveAggregatedResults(core, strIterationPath, False)

        End If

        Me.SaveIterationConfiguration(core)

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save specific Ecosim results (Biomass,Mortality and Yield) of iteration to a CSV file.
    ''' </summary>
    ''' <param name="bMonthly"> True for results to be saved monthly and false to save annually </param>
    ''' <returns>Always returns true.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveAggregatedResults(core As cCore, strIterationPath As String, bMonthly As Boolean) As Boolean

        For Each outputtype As cEcosimResultWriter.eResultTypes In [Enum].GetValues(GetType(cEcosimResultWriter.eResultTypes))
            Select Case outputtype
                Case cEcosimResultWriter.eResultTypes.Biomass,
                     cEcosimResultWriter.eResultTypes.Mortality,
                     cEcosimResultWriter.eResultTypes.Catch
                    Me.SaveAggregatedTypeResult(outputtype, core, strIterationPath, bMonthly)
            End Select
        Next

        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save a specific result (Biomass,Mortality or Yield) of iteration to a CSV file.
    ''' </summary>
    ''' <param name="ResultType">The Result type to save.</param>
    ''' <param name="bMonthly">True for results to be saved monthly and false to save annually.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveAggregatedTypeResult(ResultType As cEcosimResultWriter.eResultTypes, core As cCore,
                                              strIterationPath As String, bMonthly As Boolean) As Boolean

        Dim CSVfile As String = ""
        If (bMonthly) Then
            CSVfile = Path.Combine(strIterationPath, Me.Name + "_" + ResultType.ToString + ".csv")
        Else
            CSVfile = Path.Combine(strIterationPath, Me.Name + "_" + ResultType.ToString + "_Annual.csv")
        End If

        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = True
        Dim data(core.nGroups, core.nEcosimTimeSteps) As Single
        Dim grpOutput As cEcosimGroupOutput = Nothing
        Dim GroupNames As String = Me.GetAllGroupNames(core)

        If cFileUtils.IsDirectoryAvailable(strIterationPath, True) Then

            ' ToDo: clear the content of the directory?

            Try
                writer = New StreamWriter(CSVfile)
            Catch ex As Exception
                Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_ITERATION_AGGREGATED, ex.Message))
                bSuccess = False
            End Try

            If (writer IsNot Nothing) Then

                ' Include default header if needed
                If Me.Parameters.SaveHeaders Then
                    writer.WriteLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                    writer.WriteLine("Iteration Name," + Me.Name)
                    writer.WriteLine("Data," + ResultType.ToString)
                    writer.WriteLine()
                End If

                writer.WriteLine(GroupNames)

                Try

                    If (Me.RunState = ISFPIteration.eRunState.Completed) Then

                        For i As Integer = 1 To core.nGroups
                            grpOutput = core.EcosimGroupOutputs(i)
                            For j As Integer = 1 To core.nEcosimTimeSteps
                                Select Case ResultType
                                    Case cEcosimResultWriter.eResultTypes.Biomass
                                        data(i, j) = grpOutput.Biomass(j)
                                    Case cEcosimResultWriter.eResultTypes.Mortality
                                        data(i, j) = grpOutput.TotalMort(j)
                                    Case cEcosimResultWriter.eResultTypes.Catch
                                        data(i, j) = grpOutput.Catch(j)
                                End Select
                            Next
                        Next

                        'Output Monthly
                        If (bMonthly) Then
                            'Each time steps
                            For j As Integer = 1 To data.GetLength(1) - 1
                                'For every group
                                For i As Integer = 1 To data.GetLength(0) - 1
                                    If i > 1 Then writer.Write(", ")
                                    writer.Write(cStringUtils.FormatSingle(data(i, j)))
                                Next
                                writer.WriteLine()
                            Next
                        Else ' Output Yearly
                            Dim simYears As Integer = CInt(Math.Floor((data.GetLength(1) - 1) / cCore.N_MONTHS))
                            Dim nGroups As Integer = data.GetLength(0) - 1
                            Dim sum(nGroups) As Single
                            For j As Integer = 1 To simYears
                                For i As Integer = 1 To nGroups
                                    For k As Integer = 1 To cCore.N_MONTHS
                                        If (k = 1) Then sum(i) = 0
                                        sum(i) += data(i, (j - 1) * cCore.N_MONTHS + k)
                                    Next
                                    If i > 1 Then writer.Write(", ")
                                    writer.Write(cStringUtils.FormatSingle(sum(i) / cCore.N_MONTHS))
                                Next
                                writer.WriteLine()
                            Next
                        End If

                        Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_SUCCESS, My.Resources.DETAIL_ITERATION_AGGREGATED, CSVfile))
                    End If

                Catch ex As Exception
                    Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_FAILED, My.Resources.DETAIL_ITERATION_AGGREGATED, ex.Message))
                    bSuccess = False
                End Try

                writer.Close()

            End If
        Else
            ' Panic!
            Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.FAILURE_DIRECTORY, strIterationPath))
            bSuccess = False
        End If

        Return bSuccess
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all group names from Ecosim run and return them as a comma separated string
    ''' </summary>
    ''' <returns>String of comma separated group names.</returns>
    ''' -----------------------------------------------------------------------
    Private Function GetAllGroupNames(core As cCore) As String

        Dim str As New StringBuilder()

        For i As Integer = 1 To core.nGroups
            str.Append(cStringUtils.ToCSVField(core.EcosimGroupOutputs(i).Name))
            If i <> core.nGroups Then str.Append(",")
        Next

        Return str.ToString()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the configuration of an iteration to file for later reloading.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveIterationConfiguration(core As cCore) As Boolean

        ' Sanity checks

        Dim strIterationPath As String = Path.Combine(Me.Parameters.IterationOutputFolder, cFileUtils.ToValidFileName(Me.Name, False))
        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = True
        Dim dtFields As New Dictionary(Of String, String)

        ' Abort if not ran completed
        ' Note that this assumes that the directory is vigin territory... failed iterations are not obliterated. EwE always makes this harsh assumption, eek
        If (Not Me.RunState = ISFPIteration.eRunState.Completed) Then Return False

        If cFileUtils.IsDirectoryAvailable(strIterationPath, True) Then

            writer = New StreamWriter(Path.Combine(strIterationPath, ".classname"))
            writer.WriteLine(Me.GetType().ToString)
            writer.Close()

            'Save vulnerabilities configuartion
            writer = New StreamWriter(Path.Combine(strIterationPath, ".vulnerabilities"))
            If (Me.Vulnerabilities IsNot Nothing) Then
                For i As Integer = 1 To core.nGroups
                    If (i > 1) Then writer.WriteLine()
                    For j As Integer = 1 To core.nGroups
                        If (j > 1) Then writer.Write(",")
                        writer.Write(cStringUtils.ToCSVField(Me.Vulnerabilities(i, j)))
                    Next
                Next
            End If
            writer.Close()

            'Output vulnerabilities to a csv file
            'If ecosim or all output is selected save the csv file to the named iteration folder
            If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Ecosim) Or
               (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then
                writer = New StreamWriter(Path.Combine(strIterationPath, "Vulnerabilities.csv"))
                If (Me.Vulnerabilities IsNot Nothing) Then
                    ' Include default header if needed
                    If Me.Parameters.SaveHeaders Then
                        dtFields("IterationName,") = Me.Name
                        dtFields("Data") = "Vulnerabilities"

                        writer.WriteLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
                        writer.WriteLine()

                        dtFields.Clear()
                    End If

                    ' -- Write header --
                    For i As Integer = 1 To core.nGroups
                        If (i > 1) Then writer.WriteLine()
                        For j As Integer = 1 To core.nGroups
                            If (j > 1) Then writer.Write(",")
                            writer.Write(cStringUtils.ToCSVField(Me.Vulnerabilities(i, j)))
                        Next
                    Next
                End If
                writer.Close()
            End If

            'If aggregated or all output is selected save the csv file to the named iteration folder
            If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Aggregated) Or
               (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then
                Dim strPath As String = Me.Parameters.IterationOutputFolder
                writer = New StreamWriter(Path.Combine(strPath, Me.Name + "_Vulnerabilities.csv"))
                If (Me.Vulnerabilities IsNot Nothing) Then
                    ' Include default header if needed
                    If (Me.Parameters.SaveHeaders) Then
                        dtFields("IterationName,") = Me.Name
                        dtFields("Data") = "Vulnerabilities"

                        writer.WriteLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
                        writer.WriteLine()

                        dtFields.Clear()
                    End If

                    For i As Integer = 1 To core.nGroups
                        If (i > 1) Then writer.WriteLine()
                        For j As Integer = 1 To core.nGroups
                            If (j > 1) Then writer.Write(",")
                            writer.Write(cStringUtils.ToCSVField(Me.Vulnerabilities(i, j)))
                        Next
                    Next
                End If
                writer.Close()
            End If

            'Save anomaly shape configuartion
            writer = New StreamWriter(Path.Combine(strIterationPath, ".anomaly"))
            If (Me.AnomalyShape IsNot Nothing) Then
                For i As Integer = 0 To Me.AnomalyShape.Length - 1
                    If (i >= 1) Then writer.Write(",")
                    writer.Write(cStringUtils.ToCSVField(Me.AnomalyShape(i)))
                Next
            End If
            writer.Close()

            'Output Anomaly to a csv file
            'If ecosim or all output is selected save the csv file to the named iteration folder
            If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Ecosim) Or
               (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then
                writer = New StreamWriter(Path.Combine(strIterationPath, "Anomaly.csv"))
                If (Me.AnomalyShape IsNot Nothing) Then
                    ' Include default header if needed
                    If (Me.Parameters.SaveHeaders) Then
                        dtFields("IterationName,") = Me.Name
                        dtFields("Data") = "Anommaly"

                        writer.WriteLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
                        writer.WriteLine()

                        dtFields.Clear()
                    End If
                    For i As Integer = 0 To Me.AnomalyShape.Length - 1
                        If (i >= 1) Then writer.Write(",")
                        writer.Write(cStringUtils.ToCSVField(Me.AnomalyShape(i)))
                    Next
                End If
                writer.Close()
            End If

            'If aggregated or all output is selected save the csv file to the named iteration folder
            If (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.Aggregated) Or
               (Me.Parameters.AutosaveMode = cSFPParameters.eAutosaveMode.All) Then
                writer = New StreamWriter(Path.Combine(strIterationPath, Me.Name + "_Anomaly.csv"))
                If (Me.AnomalyShape IsNot Nothing) Then
                    ' Include default header if needed
                    ' Include default header if needed
                    If (Me.Parameters.SaveHeaders) Then
                        dtFields("IterationName,") = Me.Name
                        dtFields("Data") = "Anommaly"

                        writer.WriteLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
                        writer.WriteLine()

                        dtFields.Clear()
                    End If

                    For i As Integer = 0 To Me.AnomalyShape.Length - 1
                        If (i >= 1) Then writer.Write(",")
                        writer.Write(cStringUtils.ToCSVField(Me.AnomalyShape(i)))
                    Next
                End If
                writer.Close()
            End If

            Me.AppendRunStateMessage(cStringUtils.Localize(My.Resources.STATUS_SAVE_DETAIL_SUCCESS, My.Resources.DETAIL_ITERATION_CONFIG, strIterationPath))

        End If
        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Re-populate an iteration from file.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadIterationConfiguration(core As cCore) As Boolean

        Dim strSimPath As String = Path.Combine(Me.Parameters.IterationOutputFolder, cFileUtils.ToValidFileName(Me.Name, False))
        Dim bSuccess As Boolean = True

        If cFileUtils.IsDirectoryAvailable(strSimPath, False) Then

            ' -- Class name validation --
            Try
                Using reader As New StreamReader(Path.Combine(strSimPath, ".classname"))
                    Dim strClassName As String = reader.ReadLine().Trim()
                    bSuccess = (String.Compare(Me.GetType().ToString(), strClassName, True) = 0)
                    reader.Close()
                End Using
            Catch ex As Exception
                bSuccess = False
            End Try

            ' -- Vulnerabilities --
            Try
                Using reader As New StreamReader(Path.Combine(strSimPath, ".vulnerabilities"))
                    Debug.Assert(Me.Vulnerabilities IsNot Nothing)
                    For i As Integer = 1 To core.nGroups
                        Dim strLine As String = reader.ReadLine().Trim()
                        Dim astrValues As String() = cStringUtils.SplitQualified(strLine, ","c)
                        For j As Integer = 1 To core.nGroups
                            Me.Vulnerabilities(i, j) = cStringUtils.ConvertToSingle(astrValues(j - 1))
                        Next
                    Next
                End Using

            Catch ex As Exception
                ' Let this code blunder into array bounds etc. No neat error trapping for now, we can always improve this checking later
                bSuccess = False
            End Try

            ' -- Anomaly shape --
            Try
                Using reader As New StreamReader(Path.Combine(strSimPath, ".anomaly"))

                    Debug.Assert(Me.AnomalyShape IsNot Nothing)

                    Dim strLine As String = reader.ReadLine().Trim()
                    Dim astrValues As String() = cStringUtils.SplitQualified(strLine, ","c)
                    Dim shape As Single() = Me.AnomalyShape

                    For i As Integer = 0 To astrValues.Length - 1
                        shape(i) = cStringUtils.ConvertToSingle(astrValues(i))
                    Next
                    For i As Integer = astrValues.Length - 1 To shape.Length - 1
                        shape(i) = 0
                    Next

                End Using
            Catch ex As Exception
                ' Let this code blunder into array bounds etc. No neat error trapping for now, we can always improve this checking later
                bSuccess = False
            End Try
        End If

        Me.RunState = If(bSuccess, ISFPIteration.eRunState.Idle, ISFPIteration.eRunState.Error)
        Return bSuccess

    End Function

#End Region ' Serialization and output writing

End Class
