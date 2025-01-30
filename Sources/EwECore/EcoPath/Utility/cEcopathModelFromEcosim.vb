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
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Class to export an Ecosim time step to a new Ecopath model.
''' </summary>
Public Class cEcopathModelFromEcosim

#Region " Private class "

    Private Class cData

        Private m_dic As New Dictionary(Of String, List(Of Single))

        Public Sub New()
            ' NOP
        End Sub

        Public Sub Clear()
            Me.m_dic.Clear()
        End Sub

        Public WriteOnly Property NextValue(var As String, i1 As Integer, Optional i2 As Integer = 0) As Single
            Set(value As Single)
                Dim key As String = Me.Key(var, i1, i2)
                If Not Me.m_dic.ContainsKey(key) Then
                    Me.m_dic(key) = New List(Of Single)
                End If
                Me.m_dic(key).Add(value)
            End Set
        End Property

        Public Function Mean(var As String, i1 As Integer, Optional i2 As Integer = 0) As Single
            Dim key As String = Me.Key(var, i1, i2)
            If Not Me.m_dic.ContainsKey(key) Then Return 0
            Return Me.m_dic(key).Average()
        End Function

        Public Property BACalcMode As eBACalcTypes

        Private Function Key(var As String, i1 As Integer, i2 As Integer) As String
            Return var & ":" & CStr(i1) & ":" & CStr(i2)
        End Function

    End Class

#End Region ' Private class

#Region " Private variables "

    ''' <summary>The core that holds the source model.</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Progress of a run.</summary>
    Private m_msgStatus As cMessage = Nothing

    Private m_data As cData = Nothing

#End Region ' Private variables

#Region " Construction "

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type indicating how BA should be calculated.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eBACalcTypes
        ''' <summary>BA is calculated from an average over X number of years.</summary>
        FromEcosimYearsAverage

        FromEcosimYearsWeightedAverage
        ''' <summary>BA is taken as the change in group biomass over the Ecosim run.</summary>
        FromEcosimStart
        ''' <summary>BA kept at Ecopath base value.</summary>
        FromEcopath
        ''' <summary>BA is set to 0.</summary>
        SetToZero
    End Enum


    Public Function InitRun(strOutputPath As String) As Boolean

        Me.m_msgStatus = New cMessage(My.Resources.CoreMessages.MODELFROMSIM_GENERATED, eMessageType.DataExport, eCoreComponentType.Ecosim, eMessageImportance.Information)
        Me.m_msgStatus.Hyperlink = strOutputPath

        Return True

    End Function

    Public Function EndRun() As Boolean

        If (Me.m_msgStatus IsNot Nothing) Then
            If (Me.m_msgStatus.Variables.Count > 0) Then
                Me.m_core.Messages.SendMessage(Me.m_msgStatus)
            End If
            Me.m_msgStatus = Nothing
        End If

        Me.m_data = Nothing

        Return True

    End Function

    Public Sub InitGeneration(BACalcMode As eBACalcTypes)

        'Clear out any data from the last time a model saved
        If Me.m_data IsNot Nothing Then
            Me.m_data.Clear()
            Me.m_data = Nothing
        End If

        Me.m_data = New cData()
        Me.m_data.BACalcMode = BACalcMode

    End Sub

    Public Sub Record(iTime As Integer)

        If (Me.m_data Is Nothing) Then Return
        Me.RecordAverages(iTime)

    End Sub

    Public Function EndGeneration(strFileName As String, strModelName As String, iTime As Integer,
                                  iNumYearsAverage As Integer, WeightPower As Single) As eDatasourceAccessType

        If (Me.m_data Is Nothing) Then Return eDatasourceAccessType.Failed_CannotSave
        Return Me.SaveModel(strFileName, strModelName, iTime, Me.m_data.BACalcMode, iNumYearsAverage, WeightPower)

    End Function



    Public Sub LogStatus(strStatus As String, status As eStatusFlags)

        Debug.Assert(Not Me.m_msgStatus Is Nothing)

        Dim vs As New cVariableStatus(status, strStatus, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
        Me.m_msgStatus.AddVariable(vs)

    End Sub

#End Region ' Public access

#Region " Internals "

    Private Sub RecordAverages(itime As Integer)

        Dim pathSrc As cEcopathDataStructures = Me.m_core.m_EcopathData
        Dim stanzaSrc As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim taxonSrc As cTaxonDataStructures = Me.m_core.m_TaxonData
        Dim simSrc As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Dim sArea As Single = Me.m_core.EwEModel.Area
        Dim simBB() As Single = Me.m_core.m_Ecosim.BB

        ' Capture group data
        For iGroup As Integer = 1 To Me.m_core.nGroups

            'jb 20-Nov-2012 remove DCPct() and populate the Ecopath variable directly from the Ecosim Variables
            'this makes it easier to tell what and how the Ecopath value are computed from the current Ecosim run
            Me.m_data.NextValue("Binput", iGroup) = simBB(iGroup) 'simSrc.DCPct(iGroup, 1)
            ' Catch(i) = Bi(i) * FishTime(i)
            Me.m_data.NextValue("fCatch", iGroup) = simBB(iGroup) * simSrc.FishTime(iGroup)

            ' PBi(i) = loss(i) / Bi(i)
            Me.m_data.NextValue("PBinput", iGroup) = simSrc.loss(iGroup) / simBB(iGroup)
            ' QBi(i) = DCPct(i, 2) 'the following has been updated: Eatenby(i) / bb(i)
            Me.m_data.NextValue("QBinput", iGroup) = simSrc.Eatenby(iGroup) / simBB(iGroup) ' simSrc.DCPct(iGroup, 2)

            ' Emigrationi(i) = Emig(i) * Bi(i) '
            Me.m_data.NextValue("Emigration", iGroup) = pathSrc.Emig(iGroup) * simBB(iGroup)
            ' BHi(i) = Bi(i) / Area(i)
            Me.m_data.NextValue("BHinput", iGroup) = simBB(iGroup) / pathSrc.Area(iGroup)

        Next

        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                If simSrc.Eatenby(iPred) > 0 Then
                    'simDCAtT(pred,prey) contains biomass eaten by a predator on a prey populated in derivt()
                    'Eatenby(pred) is the total biomass eaten by a predator
                    'DC(pred,prey) is the proportion of diet made up by a prey
                    'So get the proportion of diet 
                    Me.m_data.NextValue("DC", iPred, iPrey) = simSrc.simDCAtT(iPred, iPrey) / simSrc.Eatenby(iPred)
                End If
            Next
        Next

        'immigration is constant rate and is not changed by ecosim so no need to change
        For i As Integer = 1 To Me.m_core.nGroups
            Dim SumEf As Single = 0.0
            For j As Integer = 1 To pathSrc.NumFleet
                ' SumEf = SumEf + FishRateGear(j, itime) * FishMGear(j, i)
                SumEf += simSrc.FishRateGear(j, itime) * simSrc.FishMGear(j, i)
            Next
            For j As Integer = 1 To Me.m_core.nFleets
                Dim Sum As Single = 0
                Dim Z As Single = pathSrc.Landing(j, i) + pathSrc.Discard(j, i)
                ' If SumEf > 0 Then Sum = BB(i) * FishTime(i) * FishRateGear(j, iTime) * FishMGear(j, i) / SumEf
                If SumEf > 0 And Z > 0 Then
                    Dim BB As Single = simBB(i) 'results.Biomass(i) * simSrc.StartBiomass(i)
                    Sum = BB * simSrc.FishTime(i) * simSrc.FishRateGear(j, itime) * simSrc.FishMGear(j, i) / SumEf
                    Me.m_data.NextValue("Landing", j, i) = Sum * pathSrc.Landing(j, i) / Z
                    Me.m_data.NextValue("Discard", j, i) = Sum * pathSrc.Discard(j, i) / Z
                Else
                    Me.m_data.NextValue("Landing", j, i) = 0
                    Me.m_data.NextValue("Discard", j, i) = 0
                End If
            Next j
        Next i
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a model from the current Ecosim time step.
    ''' </summary>
    ''' <param name="strFileName">Full path to the model file to create.</param>
    ''' <param name="strModelName">Name of the model to create.</param>
    ''' <param name="iTime">The Ecosim time step to populate data from.</param>
    ''' <param name="BACalculation"><see cref="eBACalcTypes">Flag</see> 
    ''' stating how BA should be calculated.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveModel(strFileName As String,
                              strModelName As String,
                              iTime As Integer,
                              BACalculation As eBACalcTypes,
                              iNumYearsAverage As Integer,
                              WeightPower As Single) As eDatasourceAccessType

        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown

        Try

            Dim coreDest As New cCore()
            Dim db As cEwEDatabase = New cEwEAccessDatabase()
            Dim bSucces As Boolean = False

            coreDest.PluginManager = Nothing

            If String.IsNullOrEmpty(Path.GetExtension(strFileName)) Then
                strFileName &= cDataSourceFactory.GetDefaultExtension(eDataSourceTypes.Access2003)
            End If

            atResult = db.Create(strFileName, strModelName, True, strAuthor:=Me.m_core.DefaultAuthor)
            If (atResult = eDatasourceAccessType.Created) Then

                If coreDest.LoadModel(strFileName) Then
                    If Me.CreateItems(coreDest) Then
                        Me.PopulateItems(coreDest, iTime, BACalculation, iNumYearsAverage, WeightPower)
                    End If
                    coreDest.CloseModel()

                    db = Nothing
                    coreDest = Nothing

                End If
            End If

        Catch ex As Exception
            atResult = eDatasourceAccessType.Failed_Unknown
            cLog.Write(ex)
        End Try

        Return atResult

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create groups, fleets and stanza configurations in the new Ecopath model.
    ''' </summary>
    ''' <param name="coreNew">The core that holds the new Ecopath model.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function CreateItems(coreNew As cCore) As Boolean

        Dim bSuccess As Boolean = True
        Dim pathSrc As cEcopathDataStructures = Me.m_core.m_EcopathData
        Dim stanzaSrc As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim taxonSrc As cTaxonDataStructures = Me.m_core.m_TaxonData

        Dim GroupDBID(pathSrc.NumGroups) As Integer
        Dim FleetDBID(pathSrc.NumFleet) As Integer
        Dim StanzaDBID(stanzaSrc.Nsplit) As Integer
        Dim TaxonDBID(taxonSrc.NumTaxon) As Integer

        If Not coreNew.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

        ' Items are created in the new model in exactly the same order as they occur
        ' in the source model. That way a simple data structure copy is warranted
        ' for transferring object details as long as database keys are left unique to
        ' the new model.

        Try
            ' Delete default group(s) and fleet(s)
            For iGroup As Integer = 1 To coreNew.nGroups
                coreNew.RemoveGroup(iGroup)
            Next

            For iFleet As Integer = 1 To coreNew.nFleets
                coreNew.RemoveFleet(iFleet)
            Next

            For iGroup As Integer = 1 To pathSrc.NumGroups
                Dim iNew As Integer = iGroup
                Dim iIDNew As Integer = 0
                bSuccess = bSuccess And coreNew.AddGroup(pathSrc.GroupName(iGroup), pathSrc.PP(iGroup), pathSrc.vbK(iGroup), iNew, iIDNew)
                GroupDBID(iGroup) = iIDNew
            Next

            For iFleet As Integer = 1 To pathSrc.NumFleet
                Dim iNew As Integer = iFleet
                Dim iIDNew As Integer = 0
                bSuccess = bSuccess And coreNew.AddFleet(pathSrc.FleetName(iFleet), iNew, iIDNew)
                FleetDBID(iFleet) = iIDNew
            Next

            For iStanza As Integer = 1 To Me.m_core.nStanzas

                Dim NStanza As Integer = stanzaSrc.Nstanza(iStanza)
                Dim LifeStageDBID(NStanza - 1) As Integer
                Dim LifeStageAge(NStanza - 1) As Integer
                Dim iIDNew As Integer = 0

                For iLifeStage As Integer = 1 To NStanza
                    Dim iGroup As Integer = stanzaSrc.EcopathCode(iStanza, iLifeStage)
                    LifeStageDBID(iLifeStage - 1) = GroupDBID(iGroup)
                    LifeStageAge(iLifeStage - 1) = stanzaSrc.Age1(iStanza, iLifeStage)
                Next
                bSuccess = bSuccess And coreNew.AppendStanza(stanzaSrc.StanzaName(iStanza), LifeStageDBID, LifeStageAge, iIDNew)
                StanzaDBID(iStanza) = iIDNew
            Next

        Catch ex As Exception
            bSuccess = False
        End Try

        coreNew.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, bSuccess)

        ' Define taxa in a second step AFTER all groups and stanza have been created
        coreNew.SetBatchLock(cCore.eBatchLockType.Restructure)
        For iTaxon As Integer = 1 To Me.m_core.nTaxon
            Dim iIDNew As Integer = 0
            Dim data As New cTaxonSearchData(iTaxon, taxonSrc)
            If taxonSrc.IsTaxonStanza(iTaxon) Then
                bSuccess = bSuccess And coreNew.AddTaxon(taxonSrc.TaxonTarget(iTaxon), True, data, 1, 1, iIDNew)
            Else
                bSuccess = bSuccess And coreNew.AddTaxon(taxonSrc.TaxonTarget(iTaxon), False, data, taxonSrc.TaxonPropBiomass(iTaxon), taxonSrc.TaxonPropCatch(iTaxon), iIDNew)
            End If
            TaxonDBID(iTaxon) = iIDNew
        Next
        coreNew.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, bSuccess)

        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate groups, fleets and stanza configurations in a new Ecopath model.
    ''' </summary>
    ''' <param name="coreNew">The core that holds the new model.</param>
    ''' <param name="iTime">The Ecosim time step to populate data from.</param>
    ''' <param name="BACalculation"><see cref="eBACalcTypes">Flag</see> 
    ''' stating how BA should be calculated.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function PopulateItems(coreNew As cCore,
                                   iTime As Integer,
                                   BACalculation As eBACalcTypes,
                                   nNumYearsAverage As Integer,
                                   WeightPower As Single) As Boolean

        Debug.Assert(iTime >= cCore.N_MONTHS, Me.ToString & ".PopulateItems(...) iTime must fall after the first year.")

        Dim pathSrc As cEcopathDataStructures = Me.m_core.m_EcopathData
        Dim pathDest As cEcopathDataStructures = coreNew.m_EcopathData
        Dim GroupDBIDs(coreNew.nGroups) As Integer
        Dim FleetDBIDs(coreNew.nFleets + 1) As Integer

        Dim stanzaSrc As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim stanzaDest As cStanzaDatastructures = coreNew.m_Stanza
        Dim StanzaDBIDs(coreNew.nStanzas) As Integer
        Dim TaxonDBIDs(coreNew.nTaxon) As Integer

        Dim taxonSrc As cTaxonDataStructures = Me.m_core.m_TaxonData
        Dim taxonDest As cTaxonDataStructures = coreNew.m_TaxonData

        Dim simSrc As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Dim bSuccess As Boolean = True
        Dim BiomassAtT As Single

        'number of time steps to average BA over
        Dim nBAtimesteps As Integer

        'Time index for the start year of the BA averaging
        Dim iStartIndex As Integer

        Dim StepsPerYear As Integer = cCore.N_MONTHS * simSrc.StepsPerMonth

        'Number of years up to current t
        Dim nYears As Integer = iTime \ StepsPerYear

        'Set time step indexes for averaging BA
        Me.SetStartEndTimesteps(iTime, BACalculation, nNumYearsAverage, iStartIndex, nBAtimesteps)

        ' Dirty destination core
        coreNew.DataSource.SetChanged(eCoreComponentType.Ecopath)
        coreNew.StateMonitor.UpdateDataState(coreNew.DataSource)

        ' Preserve new database IDs prior to copying Ecopath data over
        Array.Copy(pathDest.GroupDBID, GroupDBIDs, pathDest.GroupDBID.Length)
        Array.Copy(pathDest.FleetDBID, FleetDBIDs, pathDest.FleetDBID.Length)
        Array.Copy(stanzaDest.StanzaDBID, StanzaDBIDs, stanzaDest.StanzaDBID.Length)
        Array.Copy(taxonDest.TaxonDBID, TaxonDBIDs, taxonDest.TaxonDBID.Length)

        ' Copy data in bulk
        pathSrc.copyTo(pathDest, False)
        stanzaSrc.copyTo(stanzaDest)
        taxonSrc.copyTo(taxonDest)

        ' Restore DBIDs
        Array.Copy(GroupDBIDs, pathDest.GroupDBID, pathDest.GroupDBID.Length)
        Array.Copy(FleetDBIDs, pathDest.FleetDBID, pathDest.FleetDBID.Length)
        Array.Copy(StanzaDBIDs, stanzaDest.StanzaDBID, stanzaDest.StanzaDBID.Length)
        Array.Copy(TaxonDBIDs, taxonDest.TaxonDBID, taxonDest.TaxonDBID.Length)

        ' Clear Ecopath data that is not going to be copied
        pathDest.NumEcosimScenarios = 0
        pathDest.NumEcospaceScenarios = 0
        pathDest.NumEcotracerScenarios = 0
        pathDest.NumPedigreeLevels = 0
        pathDest.NumPedigreeVariables = 0

        ' Populate groups
        For iGroup As Integer = 1 To Me.m_core.nGroups
            pathDest.Binput(iGroup) = Me.m_data.Mean("Binput", iGroup)
            pathDest.fCatch(iGroup) = Me.m_data.Mean("fCatch", iGroup)
            pathDest.Ex(iGroup) = Me.m_data.Mean("fCatch", iGroup)    ' Ex(i) = Catch(i)
            pathDest.PBinput(iGroup) = Me.m_data.Mean("PBinput", iGroup)
            pathDest.QBinput(iGroup) = Me.m_data.Mean("QBinput", iGroup)
            pathDest.EEinput(iGroup) = cCore.NULL_VALUE

            ' BAi(i) = (Bi(i) - DCPct(i, 0)) * StepsPerYear ' / TimeStep 'dcpct() stores the bb() from previous round
            Select Case BACalculation

                Case eBACalcTypes.FromEcosimYearsAverage
                    Dim simBB() As Single = Me.m_core.m_Ecosim.BB
                    BiomassAtT = simSrc.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGroup, iStartIndex)
                    pathDest.BAInput(iGroup) = (simBB(iGroup) - BiomassAtT) / nNumYearsAverage
                    pathDest.BaBi(iGroup) = 0

                Case eBACalcTypes.FromEcosimYearsWeightedAverage
                    Dim b1 As Single, b2 As Single, w As Single, bsum As Single, wsum As Single
                    'Inverse distance weighted average
                    For i As Integer = 0 To nBAtimesteps - 2
                        'inverse distance weight
                        w = CSng(1 / (nBAtimesteps - (i + 1)) ^ WeightPower)
                        'BA
                        b1 = simSrc.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGroup, iStartIndex + i)
                        b2 = simSrc.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGroup, iStartIndex + i + 1)
                        'sum of weighted BA
                        bsum += (b2 - b1) * w
                        'sum of weight
                        wsum += w
                    Next
                    If wsum = 0.0 Then wsum = 1
                    'Weighted monthly average converted to annual BA for Ecopath
                    pathDest.BAInput(iGroup) = CSng(bsum / wsum * StepsPerYear)
                    pathDest.BaBi(iGroup) = 0

                Case eBACalcTypes.FromEcosimStart
                    'BA is the Annual Accumulation of B 
                    'So get the annual average accumulation (B(t)-B(0))/ number of years
                    'Attributes the annual average change in Biomass to BiomassAccumulation
                    Dim simBB() As Single = Me.m_core.m_Ecosim.BB
                    pathDest.BAInput(iGroup) = (simBB(iGroup) - pathSrc.B(iGroup)) / nYears
                    pathDest.BaBi(iGroup) = 0

                Case eBACalcTypes.FromEcopath
                    'Explicitly copy BA and BA rate from the Ecopath source so you can tell it worked
                    pathDest.BAInput(iGroup) = pathSrc.BA(iGroup)
                    pathDest.BaBi(iGroup) = pathSrc.BaBi(iGroup)

                Case eBACalcTypes.SetToZero
                    pathDest.BAInput(iGroup) = 0
                    pathDest.BaBi(iGroup) = 0

            End Select

            pathDest.Emigration(iGroup) = Me.m_data.Mean("Emigration", iGroup)
            pathDest.BHinput(iGroup) = Me.m_data.Mean("BHinput", iGroup)
        Next

        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                pathDest.DC(iPred, iPrey) = Me.m_data.Mean("DC", iPred, iPrey)
            Next
        Next
        pathDest.SumDCToOne()

        'immigration is constant rate and is not changed by ecosim so no need to change
        For i As Integer = 1 To Me.m_core.nGroups
            For j As Integer = 1 To Me.m_core.nFleets
                pathDest.Landing(j, i) = Me.m_data.Mean("Landing", j, i)
                pathDest.Discard(j, i) = Me.m_data.Mean("Discard", j, i)
            Next j
        Next i

        coreNew.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Determine the start and end time steps for computing BA.
    ''' </summary>
    ''' <param name="iTime"></param>
    ''' <param name="BACalculation"></param>
    ''' <param name="nYearsAverage"></param>
    ''' <param name="iStartIndex"></param>
    ''' <param name="nBAtimesteps"></param>
    ''' -----------------------------------------------------------------------
    Private Sub SetStartEndTimesteps(iTime As Integer, BACalculation As eBACalcTypes, nYearsAverage As Integer,
                                    ByRef iStartIndex As Integer, ByRef nBAtimesteps As Integer)

        'number of Ecosim time steps per year
        Dim nStepsPerYear As Integer = cCore.N_MONTHS * Me.m_core.m_EcoSimData.StepsPerMonth

        'number of time steps to average BA over
        nBAtimesteps = nYearsAverage * nStepsPerYear
        'Time index for the start year of the BA averaging
        iStartIndex = iTime - nBAtimesteps + 1 'Time indexes are one based

        ' JS 01May13: only send out messages if BACalculation requires the time step range
        Select Case BACalculation
            Case eBACalcTypes.FromEcopath, eBACalcTypes.FromEcosimStart, eBACalcTypes.SetToZero
                Return
            Case eBACalcTypes.FromEcosimYearsAverage, eBACalcTypes.FromEcosimYearsWeightedAverage
                ' NOP
            Case Else
                Debug.Assert(False, "BA calculation type not explicitly considered")
        End Select

        'Constrain the start and end years
        If (iStartIndex < 1) Then
            Dim vs As cVariableStatus
            vs = New cVariableStatus(eStatusFlags.FailedValidation, My.Resources.CoreMessages.MODELFRIMSIM_BA_STARTYEAR_ADJ,
                                     eVarNameFlags.NotSet, eDataTypes.EwEModel, eCoreComponentType.Ecosim, -1)
            Me.m_msgStatus.AddVariable(vs)
            iStartIndex = 1
        End If

        If ((iStartIndex + nBAtimesteps - 1) > iTime) Then
            Dim vs As cVariableStatus
            vs = New cVariableStatus(eStatusFlags.FailedValidation, My.Resources.CoreMessages.MODELFRIMSIM_BA_ENDYEAR_ADJ, eVarNameFlags.NotSet, eDataTypes.EwEModel, eCoreComponentType.Ecosim, -1)
            Me.m_msgStatus.AddVariable(vs)
            nBAtimesteps = iTime - iStartIndex + 1
        End If

    End Sub

#End Region ' Internals

#Region " Original code "

#If 0 Then ' From Ecopath v5, modSimEdit

Public Sub SaveEcopathFromEcosim()
Dim i As Integer
Dim j As Integer
Dim SaveRunFile As String
Dim SBi() As Double
Dim SBHi() As Double   'habitat biomass
Dim SCatch() As Single
Dim SEx() As Single
Dim SPBi() As Double
Dim SQBi() As Double
Dim SDC() As Single
Dim SEE() As Single
Dim SBAi() As Single
Dim SEmi() As Single
Dim SImmi() As Single
Dim SLandi() As Single
Dim SDisci() As Single
Dim titi As String
Dim Response As Variant
    ReDim SBi(NumGroups) As Double
    ReDim SBHi(NumGroups) As Double   'habitat biomass
    ReDim SCatch(NumGroups) As Single
    ReDim SEx(NumGroups) As Single
    ReDim SPBi(NumGroups) As Double
    ReDim SQBi(NumGroups) As Double
    ReDim SDC(NumGroups + 1, NumGroups + 1) As Single
    ReDim SEE(NumGroups) As Single
    ReDim SBAi(NumGroups) As Single
    ReDim SEmi(NumGroups) As Single
    ReDim SImmi(NumGroups) As Single
    ReDim SLandi(NumGear, NumGroups) As Single
    ReDim SDisci(NumGear, NumGroups) As Single
    Dim t As Variant
    For i = 1 To NumGroups
        SBi(i) = Bi(i)
        Bi(i) = DCPct(i, 1)
        SCatch(i) = Catch(i)
        Catch(i) = Bi(i) * FishTime(i)
        SEx(i) = Ex(i)
        Ex(i) = Catch(i)
        SPBi(i) = PBi(i)
        PBi(i) = loss(i) / Bi(i)
        SQBi(i) = QBi(i)
        QBi(i) = DCPct(i, 2) 'the following has been updated: Eatenby(i) / bb(i)
        SEE(i) = EEi(i)
        EEi(i) = -99
        SBAi(i) = BAi(i)
        BAi(i) = (Bi(i) - DCPct(i, 0)) * StepsPerYear ' / TimeStep 'dcpct() stores the bb() from previous round
        'BAi(i) = DCPct(i, 3) * StepsPerYear '/ TimeStep
        SEmi(i) = Emigrationi(i)
        Emigrationi(i) = Emig(i) * Bi(i) '
        SBHi(i) = BHi(i)
        BHi(i) = Bi(i) / Area(i)
    Next
    For i = 1 To NumGroups
        For j = 1 To NumGroups
            SDC(i, j) = DC(i, j)
            DCi(i, j) = 0        'don't leave any dc leftovers
            If QBi(i) > 0 Then DCi(i, j) = DCMean(i, j) '/ (QBi(i) * Bi(i))
        Next
    Next
    'immigration is constant rate and is not changed by ecosim so no need to change
    For i = 1 To NumGear
        For j = 1 To NumGroups
            SLandi(i, j) = Landing(i, j)
            Landing(i, j) = DCMin(i, j)
            SDisci(i, j) = Discard(i, j)
            Discard(i, j) = DCMax(i, j)
        Next j
    Next i
    titi = modelRemarks
    modelRemarks = "Ecosim output file; " + CStr(Date) + "; " + CStr(time) + "; " + modelRemarks

    GetValidFileName SaveRunFile
    If Mid(dbFilepath, Len(dbFilepath), 1) <> "\" Then
        SaveRunFile = dbFilepath + "\" + SaveRunFile + ".eii" 'Left(lastModel, 8) + ".txt"
    Else
        SaveRunFile = dbFilepath + SaveRunFile + ".eii"  'Left(lastModel, 8) + ".txt"
    End If

    'SaveEiiFile SaveRunFile
    Response = "Ecopath file saved to " + SaveRunFile + Environment.NewLine  + Environment.NewLine  + "You can import the file as a text-file (eii) from the File menu" + Environment.NewLine  + "Do you want to keep this file?"
    Response = MsgBox(Response, vbInformation + vbYesNo, "Save Ecopath model from Ecosim")
    If Response = vbYes Then SaveEiiFile SaveRunFile
    modelRemarks = titi
    Erase DCMin(), DCMean(), DCMax()
    'Restore Ecopath parameters
    For i = 1 To NumGroups
        Bi(i) = SBi(i)
        Catch(i) = SCatch(i)
        Ex(i) = SEx(i)
        PBi(i) = SPBi(i)
        QBi(i) = SQBi(i)
        EEi(i) = SEE(i)
        BAi(i) = SBAi(i)
        Emigrationi(i) = SEmi(i)
        BHi(i) = SBHi(i)
    Next
    For i = 1 To NumGroups
        For j = 1 To NumGroups
            DCi(i, j) = SDC(i, j)
        Next
    Next
    For i = 1 To NumGear
        For j = 1 To NumGroups
            Landing(i, j) = SLandi(i, j)
            Discard(i, j) = SDisci(i, j)
        Next j
    Next i
End Sub
#End If
#End Region ' Original code

End Class
