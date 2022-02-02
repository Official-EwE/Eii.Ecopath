' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports EwECore.FitToTimeSeries
Imports EwEUtils.Core

#End Region ' Imports

Public MustInherit Class cSFPGenericIteration
    Implements ISFPIteration

    Protected m_iTimeSeries As Integer
    Protected m_bPredOrPredPreySSToV As ISFPIteration.eVulSearchMode

    ''' <summary>Calculated Sum of Squares</summary>
    Protected m_ss As Single = 0
    ''' <summary>Calculated AIC</summary>
    Protected m_aic As Single = 0
    ''' <summary>Calculated AICc</summary>
    Protected m_aicc As Single = 0
    ''' <summary>Anomaly shape data</summary>
    Protected m_anomalyshape() As Single = Nothing
    ''' <summary>Vulnerabilities data</summary>
    Protected m_vulnerabilities(,) As Single = Nothing
    ''' <summary>Calculated time series SS results</summary>
    Protected m_timeseriesSS As Single()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="tsi"></param>
    ''' <param name="vulsearch"></param>
    ''' <param name="Params"></param>
    ''' -----------------------------------------------------------------------
    Protected Sub Initiate(core As EwECore.cCore, tsi As Integer, vulsearch As ISFPIteration.eVulSearchMode, Params As cSFPParameters) _
        Implements ISFPIteration.Init

        'Get variables needed for SFP iteration
        Me.m_iTimeSeries = tsi
        Me.m_bPredOrPredPreySSToV = vulsearch
        Me.Parameters = Params

        ' Allocate memory for anomaly shape
        ReDim Me.m_anomalyshape(core.nEcosimTimeSteps)

        ' Allocate memory for vulnerabilities matrix
        ReDim Me.m_vulnerabilities(core.nGroups, core.nGroups)

        'Allocate memory for time series SS results
        ReDim Me.m_timeseriesSS(core.TimeSeriesDataset(Me.m_iTimeSeries).nTimeSeries)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Load"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function Load(core As cCore) As Boolean _
        Implements ISFPIteration.Load

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Run"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Run(core As cCore) As Boolean _
        Implements ISFPIteration.Run

        'Run EcoSim
        Me.RunEcosim(core)

        ' Store calculated values
        Me.m_ss = Me.GetSS(core)
        Me.m_aic = Me.GetAIC(core)
        Me.m_aicc = Me.GetAICc(core)

        ' Store vulnerabilities
        For i As Integer = 1 To core.nGroups
            Dim grp As cEcoSimGroupInput = core.EcoSimGroupInputs(i)
            For j As Integer = 1 To core.nGroups
                Me.m_vulnerabilities(i, j) = grp.VulMult(j)
            Next
        Next

        ' Store first anomaly shape
        Dim shape As cShapeData = Me.GetAppliedShape(core)
        If (shape IsNot Nothing) Then
            core.ForcingShapeManager.Load()
            Me.m_anomalyshape = shape.ShapeData
        End If

        'Store time series SS
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
                Dim grp As cEcoSimGroupInput = core.EcoSimGroupInputs(i)
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
    ''' -----------------------------------------------------------------------
    Protected Function EnableTimeSeries(core As cCore) As Boolean

        If (Me.m_iTimeSeries < 1) Then Return False

        Dim dataset As cTimeSeriesDataset = core.TimeSeriesDataset(Me.m_iTimeSeries)
        Dim man As cF2TSManager = core.EcosimFitToTimeSeries

        'Reset fishing effort shapes
        core.FishingEffortShapeManager.ResetToDefaults()

        Select Case Me.BaseSearchMode

            Case ISFPIteration.eBaseSearchMode.Baseline
                'Go through each time series of the time series dataset
                For i As Integer = 1 To dataset.nTimeSeries

                    Dim ts As cTimeSeries = dataset.TimeSeries(i)
                    'If the time series type is 0, 1, 5, 6, 7 enable it
                    Select Case ts.TimeSeriesType
                        Case eTimeSeriesType.BiomassRel,
                             eTimeSeriesType.TotalMortality,
                             eTimeSeriesType.Catches,
                             eTimeSeriesType.CatchesRel,
                             eTimeSeriesType.AverageWeight
                            ts.Enabled = True
                        Case eTimeSeriesType.BiomassAbs
                            ts.Enabled = Me.Parameters.EnableAbsoluteBiomass
                        Case Else
                            ts.Enabled = False
                    End Select
                Next

            Case ISFPIteration.eBaseSearchMode.Fishing
                'Go through each time series of the time series dataset
                For i As Integer = 1 To dataset.nTimeSeries
                    'Enable Time Series
                    Dim ts As cTimeSeries = dataset.TimeSeries(i)
                    ts.Enabled = True
                Next

            Case Else
                Debug.Assert(False, "Unsupported enum")

        End Select

        'Apply the enabled time series
        core.UpdateTimeSeries(False)

        Return True

    End Function

    Public Property Parameters As cSFPParameters Implements ISFPIteration.Parameters

    Public Property k As Integer = 0 Implements ISFPIteration.K
    Public Property EstimatedV As Integer = 0 Implements ISFPIteration.EstimatedV
    Public Property SplinePoints As Integer = 0 Implements ISFPIteration.SplinePoints
    Public Property BaseSearchMode As ISFPIteration.eBaseSearchMode Implements ISFPIteration.BaseSearchMode
    Public Property Enabled As Boolean = True Implements ISFPIteration.Enabled
    Public Property RunState As ISFPIteration.eRunState = ISFPIteration.eRunState.Idle Implements ISFPIteration.RunState
    Public Property Elapsed As TimeSpan Implements ISFPIteration.Elapsed
    Public Property Completed As Date Implements ISFPIteration.Completed

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check if Ecosim has non-default vulnerabilities, and if so, reset the
    ''' vulnerabilties. 
    ''' </summary>
    ''' <returns>True if Ecosim has all default vulnerabilties.</returns>
    ''' -----------------------------------------------------------------------
    Protected Function ResetVs(core As cCore) As Boolean
        ' Suppress prompt, just reset the vulnerabilities without asking
        Return core.CheckResetDefaultVulnerabilities(True)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Sensitivity search according to user input
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunSensitivityOfSSToV(core As cCore) As Boolean

        Dim bOK As Boolean = False
        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        'Set the number of blocks selected to Max K
        man.nBlockCodes = Me.Parameters.K

        'If PredOrPredPreySSToV = true then run SS2VBy Predator
        Select Case Me.m_bPredOrPredPreySSToV
            Case ISFPIteration.eVulSearchMode.Predator
                If man.RunSensitivitySS2VByPredator(True, TriState.False) Then
                    Debug.Assert(Not man.IsRunning)
                    'Set vulnerabiltiy blocks
                    man.setNBlocksFromSensitivity(Me.Parameters.K)
                    bOK = True
                End If
            Case ISFPIteration.eVulSearchMode.PredPrey
                If man.RunSensitivitySS2VByPredPrey(True, TriState.False) Then
                    Debug.Assert(Not man.IsRunning)
                    'Set vulnerabiltiy blocks
                    man.setNBlocksFromSensitivity(Me.Parameters.K)
                    bOK = True
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
        Return core.RunEcoSim()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Vulnerability Search iterations according to k estimated parameters
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunVulnerabilitySearch(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries

        'Setup manager to do a vunerability search
        man.VulnerabilitySearch = True
        man.AnomalySearch = False
        man.VulnerabilityVariance = 10.0
        'Set the number of blocks selected (Number of parameters to estimate)
        man.nBlockCodes = Me.EstimatedV

        ' Run the search silently
        Return man.RunSearch(True, TriState.False)

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

    Protected Function GetAppliedShape(core As cCore) As cShapeData
        Dim man As cForcingFunctionShapeManager = core.ForcingShapeManager
        If (Me.Parameters.AppliedShapeIndex > 0) Then
            Return man(Me.Parameters.AppliedShapeIndex - 1)
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run Anomaly Search according to spline point estimated parameters. This search will only run if a FF is applied to a PP.
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Protected Function RunAnomalySearch(core As cCore) As Boolean

        Dim man As cF2TSManager = core.EcosimFitToTimeSeries
        Dim bSuccess As Boolean = False

        'If there is no applied shape do not run search (This is already checked by the cSFPManager but just to make sure)
        If (Me.Parameters.AppliedShapeIndex > 0) Then

            'Setup manager to do a Anomaly search
            man.AnomalySearch = True
            man.VulnerabilitySearch = False
            man.FirstYear = 1
            man.LastYear = core.TimeSeriesDataset(Me.m_iTimeSeries).NumPoints
            man.PPVariance = 0.1
            'Set the number of spline points selected (Number of parameters to estimate)
            man.NumSplinePoints = Me.SplinePoints
            man.AnomalySearchShapeNumber = Me.Parameters.AppliedShapeIndex

            ' Run the search silently
            If man.RunSearch(True, TriState.False) Then
                Debug.Assert(Not man.IsRunning)
                bSuccess = True
            End If
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
        Dim bSuccess As Boolean = False

        'If there is an applied shape and a sensitivity search has been ran : run the search
        If (Me.Parameters.AppliedShapeIndex > 0) And man.HasRunSens Then

            'Setup manager to do a Vulnerability and Anomaly search
            man.AnomalySearch = True
            man.FirstYear = 1
            man.LastYear = core.TimeSeriesDataset(Me.m_iTimeSeries).NumPoints
            man.PPVariance = 0.1
            'Set the number of spline points selected (Number of parameters to estimate)
            man.NumSplinePoints = Me.SplinePoints
            man.AnomalySearchShapeNumber = Me.Parameters.AppliedShapeIndex

            man.VulnerabilitySearch = True
            'Set the number of blocks selected (Number of parameters to estimate)
            man.nBlockCodes = Me.EstimatedV
            man.VulnerabilityVariance = 10.0

            ' Run the search silently
            If man.RunSearch(True, TriState.False) Then
                Debug.Assert(Not man.IsRunning)
                bSuccess = True
            End If
        End If

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISFPIteration.Clear"/>
    ''' -----------------------------------------------------------------------
    Public Sub Clear() _
       Implements ISFPIteration.Clear
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return name of run 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Function BaselineOrFishing() As String
        Select Case Me.BaseSearchMode
            Case ISFPIteration.eBaseSearchMode.Baseline
                Return My.Resources.MODUS_BASELINE
            Case ISFPIteration.eBaseSearchMode.Fishing
                Return My.Resources.MODUS_FISHING
            Case Else
                Debug.Assert(False, "Unsupported enum")
        End Select
        Return "?"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return name of hypothesis 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String _
        Implements ISFPIteration.Name
        Get
            ' ToDo: globalize this

            Dim HName As String
            'If simple run
            If (Me.EstimatedV = 0 And Me.SplinePoints = 0) Then
                HName = Me.BaselineOrFishing()
                Return HName
            End If
            'If Vunerability Search
            If (Me.EstimatedV > 0 And Me.SplinePoints = 0) Then
                HName = Me.BaselineOrFishing() & " and " & Me.EstimatedV & "v"
                Return HName
            End If
            'If Anomaly Search
            If (Me.EstimatedV = 0 And Me.SplinePoints > 0) Then
                HName = Me.BaselineOrFishing() & " and " & Me.SplinePoints & "pp"
                Return HName
            Else 'If V and A Search
                HName = Me.BaselineOrFishing() & " and " & Me.EstimatedV & "v" & " + " & Me.SplinePoints & "pp"
                Return HName
            End If

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

#Region " Internals "

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
            answer = Me.GetAIC(core) + 2.0F * Me.k * (Me.k - 1.0F) / (nData - Me.k - 1.0F)
            Return answer
        End If

    End Function

    Private Sub GetTimeSeriesSS(core As cCore)

        For i As Integer = 1 To core.nTimeSeries
            Me.m_timeseriesSS(i) = core.TimeSeriesDataset(Me.m_iTimeSeries).TimeSeries(i).DataSS
        Next

    End Sub

#End Region ' Internals

End Class
