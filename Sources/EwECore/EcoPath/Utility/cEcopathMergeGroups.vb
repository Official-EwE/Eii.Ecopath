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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
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
Imports EwEUtils.SystemUtilities
Imports System.Drawing

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class to merge two groups in Ecopath.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcopathMergeGroups

#Region " Private variables "

    ''' <summary>The core that holds the source model.</summary>
    Private m_core As cCore = Nothing

    ''' <summary>Status message.</summary>
    Private m_msgStatus As cMessage = Nothing

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to operate on.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the current Ecopath model is ready to merge groups.
    ''' </summary>
    ''' <param name="bSendMessage"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function CanMergeGroups(Optional bSendMessage As Boolean = False) As Boolean

        Dim sm As cCoreStateMonitor = Me.m_core.StateMonitor

        If Not sm.HasEcopathLoaded() Then
            If bSendMessage Then Me.SendMessage(My.Resources.CoreMessages.MERGEGROUPS_ERROR_NOMODEL, False)
            Return False
        End If

        If Me.m_core.nEcosimScenarios > 0 Then
            If bSendMessage Then Me.SendMessage(My.Resources.CoreMessages.MERGEGROUPS_ERROR_HASECOSIM, False)
            Return False
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the current Ecopath model is ready to merge two specific
    ''' groups and a candidate name.
    ''' </summary>
    ''' <param name="agg1">The one-based index of the first group.</param>
    ''' <param name="agg2">The one-based index of the second group.</param>
    ''' <param name="strName">A suggested name for the aggregation of two groups.</param>
    ''' <returns>True if the proposed merge can be executed.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CanMergeGroups(agg1 As Integer, agg2 As Integer, strName As String) As Boolean

        Return (Me.CanMergeGroups(False) = True) And _
               (Array.IndexOf(Me.CompatibleGroups(agg1), agg2) > -1) And _
               (Not String.IsNullOrWhiteSpace(strName))

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of <see cref="cCoreGroupBase.Index">indexes</see> of
    ''' groups that can be merged with the provided <paramref name="iGroup">group index</paramref>.
    ''' </summary>
    ''' <param name="iGroup">The group index to find compatible groups for.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <para>Producers can be merged with producers;</para>
    ''' <para>Consumers with consumers;</para>
    ''' <para>Detritus with detritus.</para>
    ''' <para>For stanza groups, only life stages within the same stanza group 
    ''' can be merged</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function CompatibleGroups(iGroup As Integer) As Integer()

        Dim groups As New List(Of Integer)
        Dim ecopathds As cEcopathDataStructures = Me.m_core.m_EcoPathData

        If (1 <= iGroup And iGroup <= Me.m_core.nGroups) Then
            If ecopathds.StanzaGroup(iGroup) Then

                Dim stanzads As cStanzaDatastructures = Me.m_core.m_Stanza
                Dim iStanza As Integer = stanzads.BaseStanza(iGroup)

                ' Find stanza for this group
                For isp As Integer = 1 To stanzads.Nsplit 'No. of split group
                    Dim bFound As Boolean = False
                    For ist As Integer = 1 To stanzads.Nstanza(isp) ' No. of stanza in a split group
                        If stanzads.EcopathCode(isp, ist) = iGroup Then
                            bFound = True
                        Else
                            groups.Add(stanzads.EcopathCode(isp, ist))
                        End If
                    Next
                    If bFound Then Return groups.ToArray()
                    groups.Clear()
                Next
            Else

                If (iGroup <= ecopathds.NumLiving) Then
                    Dim sPP As Single = ecopathds.PP(iGroup)
                    For i As Integer = 1 To ecopathds.NumGroups
                        ' Math.Ceiling bit added to match Producer PP fractions
                        If (Math.Ceiling(ecopathds.PP(i)) = Math.Ceiling(sPP)) And (Not ecopathds.StanzaGroup(i)) Then groups.Add(i)
                    Next
                Else
                    For i As Integer = 1 To ecopathds.NumDetrit
                        groups.Add(i + ecopathds.NumLiving)
                    Next
                End If

                groups.Remove(iGroup)
            End If
        End If

        Return groups.ToArray()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a suggested name for the aggregation of two groups.
    ''' </summary>
    ''' <param name="agg1">The one-based index of the first group.</param>
    ''' <param name="agg2">The one-based index of the second group.</param>
    ''' <returns>A suggested name for the aggregation of two groups.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GroupName(agg1 As Integer, agg2 As Integer) As String

        Dim ecopathds As cEcopathDataStructures = Me.m_core.m_EcoPathData

        If (agg1 < 1) Or (agg2 < 1) Then Return ""
        If (agg1 > ecopathds.NumGroups) Or (agg2 > ecopathds.NumGroups) Then Return ""

        Dim s1 As String = ecopathds.GroupName(agg1)
        Dim s2 As String = ecopathds.GroupName(agg2)

        If (s1.Length + s2.Length) > 47 Then
            If (s1.Length > 20) Then s1 = s1.Substring(0, 20)
            If (s2.Length > 20) Then s2 = s2.Substring(0, 20)
        End If

        Return s1 & " / " & s2

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Merge two groups. This will update parameters of the <paramref name="agg1">
    ''' first group</paramref>, and will delete the <paramref name=" agg2">second
    ''' group</paramref> if the merge is successful.
    ''' </summary>
    ''' <param name="agg1">The one-based index of the first group to merge.</param>
    ''' <param name="agg2">The one-based index of the second group to merge.</param>
    ''' <param name="strName">The name to assign to the merged group.</param>
    ''' <param name="bMergeColors">Flag, stating if the colour of the resulting 
    ''' group must be an average of both group colours.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Merge(agg1 As Integer, agg2 As Integer, strName As String, _
                          Optional bMergeColors As Boolean = False) As Boolean

        ' Sanity checks
        If (Array.IndexOf(Me.CompatibleGroups(agg1), agg2) = -1) Then Return False
        If (String.IsNullOrWhiteSpace(strName)) Then Return False

        Dim ecopathds As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Merge generic fields
        ecopathds.GroupName(agg1) = strName
        ecopathds.PBinput(agg1) = (ecopathds.PB(agg1) * ecopathds.B(agg1) + ecopathds.PB(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
        ' VALIDATE_JS: BaBi allowed to be NULL?
        ecopathds.BaBi(agg1) = (ecopathds.BaBi(agg1) * ecopathds.B(agg1) + ecopathds.BaBi(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
        ecopathds.BA(agg1) = ecopathds.BA(agg1) + ecopathds.BA(agg2)
        ecopathds.Immig(agg1) = ecopathds.Immig(agg1) + ecopathds.Immig(agg2)
        ecopathds.Emigration(agg1) = ecopathds.Emigration(agg1) + ecopathds.Emigration(agg2)
        ecopathds.Emig(agg1) = (ecopathds.Emig(agg1) * ecopathds.B(agg1) + ecopathds.Emig(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
        'Catch(Agg1) = Catch(Agg1) + Catch(Agg2)
        ' fCatch calculated on the fly from landings and discards; no need to update
        ecopathds.det(0, agg1) = ecopathds.det(0, agg1) + ecopathds.det(0, agg2)

        If (agg1 > ecopathds.NumLiving) Then
            For i As Integer = 1 To ecopathds.NumFleet
                If ecopathds.Discard(i, agg1) + ecopathds.Discard(i, agg2) > 0 Then
                    ecopathds.DiscardFate(i, agg1 - ecopathds.NumLiving) = (ecopathds.Discard(i, agg1) * ecopathds.DiscardFate(i, agg1 - ecopathds.NumLiving) + ecopathds.Discard(i, agg2) * ecopathds.DiscardFate(i, agg2 - ecopathds.NumLiving)) / (ecopathds.Discard(i, agg1) + ecopathds.Discard(i, agg2))
                End If
            Next
        End If

        If (ecopathds.B(agg1) + ecopathds.B(agg2)) > 0 Then
            If agg1 <= ecopathds.NumLiving Then       'Both are living
                ecopathds.EEinput(agg1) = (ecopathds.EE(agg1) * ecopathds.B(agg1) + ecopathds.EE(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
                If ecopathds.QB(agg1) > 0 Or ecopathds.QB(agg2) > 0 Then    'Weighted after consumption
                    ecopathds.GS(agg1) = (ecopathds.GS(agg1) * ecopathds.B(agg1) * ecopathds.QB(agg1) + ecopathds.GS(agg2) * ecopathds.B(agg2) * ecopathds.QB(agg2)) / (ecopathds.B(agg1) * ecopathds.QB(agg1) + ecopathds.B(agg2) * ecopathds.QB(agg2))
                End If
            End If
            ecopathds.Shadow(agg1) = (ecopathds.Shadow(agg1) * ecopathds.B(agg1) + ecopathds.Shadow(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
        End If

        If (agg1 <= ecopathds.NumLiving) Then          'diet comp for living groups
            If ecopathds.QB(agg1) > 0 Or ecopathds.QB(agg2) > 0 Then
                Dim SumCons As Single = ecopathds.QB(agg1) * ecopathds.B(agg1) + ecopathds.QB(agg2) * ecopathds.B(agg2)
                For i As Integer = 1 To ecopathds.NumGroups          'prey
                    If ecopathds.B(i) > 0 Then        'Exclude aggregated groups
                        If SumCons > 0 Then 'Eaten by the two groups of all prey
                            ecopathds.DCInput(agg1, i) = (ecopathds.DCInput(agg1, i) * ecopathds.QB(agg1) * ecopathds.B(agg1) + ecopathds.DCInput(agg2, i) * ecopathds.QB(agg2) * ecopathds.B(agg2)) / SumCons
                            If ecopathds.DCInput(agg1, i) > 0 Then ecopathds.DietWasChanged(agg1, i)
                        End If
                    End If
                Next i
                If SumCons > 0 Then ecopathds.DCInput(agg1, 0) = (ecopathds.DCInput(agg1, 0) * ecopathds.QB(agg1) * ecopathds.B(agg1) + ecopathds.DCInput(agg2, 0) * ecopathds.QB(agg2) * ecopathds.B(agg2)) / SumCons
                ' Calculate the DC(Agg1,Import)
                If (ecopathds.B(agg1) + ecopathds.B(agg2)) > 0 Then 'QB aggregation is only for living groups
                    ecopathds.QBinput(agg1) = (ecopathds.QB(agg1) * ecopathds.B(agg1) + ecopathds.QB(agg2) * ecopathds.B(agg2)) / (ecopathds.B(agg1) + ecopathds.B(agg2))
                End If
            End If
        Else
            'It's detritus! Do dtimp:
            ecopathds.DtImp(agg1) = ecopathds.DtImp(agg1) + ecopathds.DtImp(agg2)
        End If
        For i As Integer = 1 To ecopathds.NumGroups
            ecopathds.DCInput(i, agg1) = ecopathds.DCInput(i, agg1) + ecopathds.DCInput(i, agg2)
            If ecopathds.DCInput(i, agg1) > 0 Then ecopathds.DietWasChanged(i, agg1)
        Next i                 'Eaten by predators of the two groups

        'Landing and discards are just summed
        For i As Integer = 1 To ecopathds.NumFleet
            If ecopathds.Landing(i, agg1) + ecopathds.Landing(i, agg2) > 0 Then
                ecopathds.Market(i, agg1) = (ecopathds.Market(i, agg1) * ecopathds.Landing(i, agg1) + ecopathds.Market(i, agg2) * ecopathds.Landing(i, agg2)) / (ecopathds.Landing(i, agg1) + ecopathds.Landing(i, agg2))
            End If
            ecopathds.Landing(i, agg1) = ecopathds.Landing(i, agg1) + ecopathds.Landing(i, agg2)
            ecopathds.Discard(i, agg1) = ecopathds.Discard(i, agg1) + ecopathds.Discard(i, agg2)
        Next

        'Biomasses are not required for detritus groups
        'total biomass:
        ecopathds.Binput(agg1) = ecopathds.B(agg1) + ecopathds.B(agg2)
        'Cannot know which combined area we are talking about, so use the area for the first group:
        ecopathds.BHinput(agg1) = ecopathds.Binput(agg1) / ecopathds.Area(agg1)
        ecopathds.Binput(agg2) = 0
        ecopathds.BHinput(agg2) = 0

        Dim c1 As Color = cColorUtils.IntToColor(ecopathds.GroupColor(agg1))
        Dim c2 As Color = cColorUtils.IntToColor(ecopathds.GroupColor(agg2))
        Dim cAgg As Color = Color.FromArgb(255, (CInt(c1.R) + CInt(c2.R)) \ 2, (CInt(c1.G) + CInt(c2.G)) \ 2, (CInt(c1.B) + CInt(c2.B)) \ 2)
        ecopathds.GroupColor(agg1) = cColorUtils.ColorToInt(cAgg)

        If Me.m_core.m_EcoPathData.StanzaGroup(agg1) Then

            ' Perform stanza merge

            Dim stanzads As cStanzaDatastructures = Me.m_core.m_Stanza
            Dim iStanza As Integer = -1
            Dim iLifestage1 As Integer = -1
            Dim iLifestage2 As Integer = -1

            For isp As Integer = 1 To stanzads.Nsplit
                For ist As Integer = 1 To stanzads.Nstanza(isp)
                    If stanzads.EcopathCode(isp, ist) = agg1 Then
                        iLifestage1 = ist
                        iStanza = isp
                    End If
                    If stanzads.EcopathCode(isp, ist) = agg2 Then iLifestage2 = ist
                Next
            Next

            Debug.Assert(iStanza >= 0 And iLifestage1 >= 0 And iLifestage2 >= 0)

            If stanzads.BaseStanza(iStanza) = agg2 Then stanzads.BaseStanza(iStanza) = agg1
            If stanzads.BaseStanzaCB(iStanza) = agg2 Then stanzads.BaseStanzaCB(iStanza) = agg1
            stanzads.Age1(iStanza, agg1) = Math.Min(stanzads.Age1(iStanza, agg1), stanzads.Age1(iStanza, agg2))
            stanzads.Stanza_Z(iStanza, agg1) = (stanzads.Stanza_Z(iStanza, agg1) + stanzads.Stanza_Z(iStanza, agg2)) / 2

        End If

        ''Aggregate BasicRemarks()
        'For i = 2 To 10
        '    AggregateRemarks("[BasicParam Remarks]", "groupName", "remarks", "RefCode", "paramNum", CStr(i))
        'Next
        ''Aggregate Catch
        'For i = 1 To NumGear
        '    AggregateRemarks("[Catch]", "groupName", "remarksCatch", "RefCodeCatch", "gearName", GearName(i))
        '    AggregateRemarks("[Catch]", "groupName", "remarksPrice", "RefCodePrice", "gearName", GearName(i))
        '    AggregateRemarks("[Catch]", "groupName", "remarksDiscards", "RefCodeDiscards", "gearName", GearName(i))
        'Next

        'If (agg1 > ecopathds.NumLiving) Then    'Detritus group so aggregate discardfate
        '    For i As Integer = 1 To ecopathds.NumGear
        '        AggregateRemarks("[Discard Fate]", "groupColName", "remarksCatch", "RefCodeCatch", "gearName", GearName(i))
        '    Next
        'End If

        Me.m_core.DataSource.SetChanged(eCoreComponentType.EcoPath)
        Me.m_core.StateMonitor.UpdateDataState(Me.m_core.DataSource)

        If Me.m_core.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath) Then
            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)
            Return Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, Me.m_core.RemoveGroup(agg2))
        End If

        Return False

    End Function

#End Region ' Public access

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Append a result notification to the current status message
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="bSuccess"></param>
    ''' -----------------------------------------------------------------------
    Public Sub SendMessage(strMessage As String, bSuccess As Boolean)
        Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.EcoPath, _
                                cSystemUtils.IIF(bSuccess, eMessageImportance.Information, eMessageImportance.Critical))
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region ' Internals

#Region " Original code "

#If 0 Then ' From Ecopath v5, ecoagg1.bas

Attribute VB_Name = "modAggregation"
        ' ============================================
        ' System package : ECOPATH2 (ver. 2.0)
        ' Programmers    : V. Christensen & K. Janagap
        ' Program name   : ECOAGG.BAS (Aggregate)
        ' Last revision  : 22 Nov 1991
        ' VB Version     : Jan-Feb 1995
        ' By             : Edwin de Guzmann and VC
        ' ============================================
        Option Explicit

Public Agg1 As Integer
Public Agg2 As Integer
Public NN1 As Integer
Dim K As Integer

Public Sub AggregateYourSelf()
'That is: You already named groups to be aggregated: Agg1 and Agg2
    ReDim GroupForDeletion(NumGroups) As Boolean
    'Aggregate basic parameters:
    CalcAggMan
    UpdateValuesOnInputFormAndInDataBase frmInputData.vaInput
    'Delete the Agg2 group
    GroupForDeletion(Agg2) = True
    DoDeleteGroup
    UpdateSpeciesSequence

End Sub

Sub CalcAggMan()
On Local Error Resume Next
Dim i As Integer
Dim SumCons As Single
Dim SQL As String
    SQL = "SELECT * from [Group Info] where modelName='" & lastModel & "' and groupName='" & Trim(Specie(Agg1)) & "'"
    Set g_Recordset = CCG.UpdatableRecords(SQL)
    If g_Recordset.RecordCount > 0 Then
        g_Recordset.Fields("groupName").value = Trim(Left$(Specie(Agg1), 20) + " / " + Left$(Specie(Agg2), 20))
        g_Recordset.Update
    End If
    Specie(Agg1) = Left$(Specie(Agg1), 20) + " / " + Left$(Specie(Agg2), 20)
    SetCellText frmInputData.vaInput, 1, Agg1, Specie(Agg1)

    PBi(Agg1) = (PB(Agg1) * B(Agg1) + PB(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
    BaBi(Agg1) = (BaBi(Agg1) * B(Agg1) + BaBi(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
    BAi(Agg1) = BA(Agg1) + BA(Agg2)
    Immigi(Agg1) = Immig(Agg1) + Immig(Agg2)
    Emigrationi(Agg1) = Emigrationi(Agg1) + Emigrationi(Agg2)
    Emigi(Agg1) = (Emigi(Agg1) * B(Agg1) + Emigi(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
    'EX(Agg1) = EX(Agg1) + EX(Agg2)
    Catch(Agg1) = Catch(Agg1) + Catch(Agg2)
    det(0, Agg1) = det(0, Agg1) + det(0, Agg2)

    'Discardfate: Weighted average; only for detritus groups
    If Agg1 > NumLiving Then
        For i = 1 To NumGear
            If Discard(i, Agg1) + Discard(i, Agg2) > 0 Then
                DiscardFate(i, Agg1 - NumLiving) = (Discard(i, Agg1) * DiscardFate(i, Agg1 - NumLiving) + Discard(i, Agg2) * DiscardFate(i, Agg2 - NumLiving)) / (Discard(i, Agg1) + Discard(i, Agg2))
            End If
        Next
    End If

    If (B(Agg1) + B(Agg2)) > 0 Then
        If Agg1 <= NumLiving Then       'Both are living
            EE(Agg1) = (EE(Agg1) * B(Agg1) + EE(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
            If QB(Agg1) > 0 Or QB(Agg2) > 0 Then    'Weighted after consumption
                GS(Agg1) = (GS(Agg1) * B(Agg1) * QB(Agg1) + GS(Agg2) * B(Agg2) * QB(Agg2)) / (B(Agg1) * QB(Agg1) + B(Agg2) * QB(Agg2))
            End If
        End If
        Shadow(Agg1) = (Shadow(Agg1) * B(Agg1) + Shadow(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
    End If

    If Agg1 <= NumLiving Then          'diet comp for living groups
        If QB(Agg1) > 0 Or QB(Agg2) > 0 Then
            SumCons = QB(Agg1) * B(Agg1) + QB(Agg2) * B(Agg2)
            For i = 1 To NumGroups          'prey
                If B(i) > 0 Then        'Exclude aggregated groups
                    If SumCons > 0 Then 'Eaten by the two groups of all prey
                        DCi(Agg1, i) = (DCi(Agg1, i) * QB(Agg1) * B(Agg1) + DCi(Agg2, i) * QB(Agg2) * B(Agg2)) / SumCons
                        If DCi(Agg1, i) > 0 Then DietWasChanged Agg1, i
                    End If
                End If
            Next i
            If SumCons > 0 Then DCi(Agg1, 0) = (DCi(Agg1, 0) * QB(Agg1) * B(Agg1) + DCi(Agg2, 0) * QB(Agg2) * B(Agg2)) / SumCons
            ' Calculate the DC(Agg1,Import)
            If (B(Agg1) + B(Agg2)) > 0 Then 'QB aggregation is only for living groups
                QBi(Agg1) = (QB(Agg1) * B(Agg1) + QB(Agg2) * B(Agg2)) / (B(Agg1) + B(Agg2))
            End If
        End If
    Else        'It's detritus do dtimp:
        DtImp(Agg1) = DtImp(Agg1) + DtImp(Agg2)
    End If

    For i = 1 To NumGroups
        DCi(i, Agg1) = DCi(i, Agg1) + DCi(i, Agg2)
        If DCi(i, Agg1) > 0 Then DietWasChanged i, Agg1
    Next i                 'Eaten by predators of the two groups

    'Landing and discards are just summed
    For i = 1 To NumGear
        If Landing(i, Agg1) + Landing(i, Agg2) > 0 Then
            Market(i, Agg1) = (Market(i, Agg1) * Landing(i, Agg1) + Market(i, Agg2) * Landing(i, Agg2)) / (Landing(i, Agg1) + Landing(i, Agg2))
        End If
        Landing(i, Agg1) = Landing(i, Agg1) + Landing(i, Agg2)
        Discard(i, Agg1) = Discard(i, Agg1) + Discard(i, Agg2)
    Next
    'Biomasses are not required for detritus groups
    'total biomass:
    Bi(Agg1) = B(Agg1) + B(Agg2)
    'Cannot know which combined area we are talking about, so use the area for the first group:
    BHi(Agg1) = Bi(Agg1) / Area(Agg1)
    Bi(Agg2) = 0
    BHi(Agg2) = 0
    'Aggregate BasicRemarks()
    For i = 2 To 10
        AggregateRemarks "[BasicParam Remarks]", "groupName", "remarks", "RefCode", "paramNum", CStr(i)
    Next
    'Aggregate Catch
    For i = 1 To NumGear
        AggregateRemarks "[Catch]", "groupName", "remarksCatch", "RefCodeCatch", "gearName", GearName(i)
        AggregateRemarks "[Catch]", "groupName", "remarksPrice", "RefCodePrice", "gearName", GearName(i)
        AggregateRemarks "[Catch]", "groupName", "remarksDiscards", "RefCodeDiscards", "gearName", GearName(i)
    Next

    If Agg1 > NumLiving Then    'Detritus group so aggregate discardfate
        For i = 1 To NumGear
            AggregateRemarks "[Discard Fate]", "groupColName", "remarksCatch", "RefCodeCatch", "gearName", GearName(i)
        Next
    End If

    'Ecosim scenario is lacking
    'Ecospace scenario is missing
    AggregateRemarks "[Output param]", "groupName", "remarks", "RefCode"

    'Remarks in species list are not needed and can be removed
    'AggregateRemarks "[Species List]", "groupName", "remarks", "RefCode"
    AggregateSpeciesList
End Sub

Public Sub DoDeleteGroup()
On Local Error Resume Next
Dim SQL As String
    For K = NumGroups To 1 Step -1
        If GroupForDeletion(K) Then 'go ahead with deletion from the mdb /vc170398
            SQL = "SELECT * from [Group Info] where modelName='" & lastModel & "' and groupName='" & Trim(Specie(K)) & "'"
            Set g_Recordset = CCG.UpdatableRecords(SQL)
            g_Recordset.Delete
            'Also delete from forms
            'DeleteGroupFromRemarks Trim(Specie(k))
            frmInputData.DeleteGroup K, K > NumLiving, True
        End If
    Next
End Sub

Private Sub UpdateValuesOnInputFormAndInDataBase(Grid As vaSpread)
'grid is frminputdata.vainput
Dim SQL As String
    'Groupname on form and database has been update already
    'SetCellValue Grid, 2, Agg1, Format(PP(Agg1), "0.0")
    'If Not NotInput(Agg1, 1) Then SetCellValue Grid, 3, Agg1, IIf(B(Agg1) > 0, Format(B(Agg1), GenNum), "")
    'If Not NotInput(Agg1, 2) Then SetCellValue Grid, 4, Agg1, IIf(PB(Agg1) > 0 And Agg1 <= NumLiving, Format(PB(Agg1), GenNum), "")
    'If Not NotInput(Agg1, 3) Then SetCellValue Grid, 5, Agg1, IIf(QB(Agg1) > 0 And Agg1 <= NumLiving, Format(QB(Agg1), GenNum), "")
    'If Not NotInput(Agg1, 4) Then SetCellValue Grid, 6, Agg1, IIf(EE(Agg1) >= 0 And Agg1 <= NumLiving, Format(EE(Agg1), GenNum), "")
    'If Not NotInput(Agg1, 0) Then SetCellValue Grid, 7, Agg1, IIf(GE(Agg1) > 0 And Agg1 <= NumLiving, Format(GE(Agg1), GenNum), "")
    'SetCellValue Grid, 8, Agg1, IIf(Agg1 <= NumLiving, Format(BA(Agg1), GenNum), "")
    'SetCellValue Grid, 9, Agg1, IIf(Agg1 <= NumLiving, Format(GS(Agg1), GenNum), "")
    'SetCellValue Grid, 10, Agg1, IIf(Agg1 <= NumLiving, "", Format(DtImp(Agg1), GenNum))
    'Also update the database   [Group Info]
    SaveGroupInfo Specie(Agg1), Agg1    ', True
    '(GrpName As String, Group As Integer, EditOnly As Boolean)
    UpdateDiet
    'SaveDietComp
    SaveDetritusFate
    SaveFisheryInfo lastModel
    SaveCatches 'True
    SaveStanza
    SaveDiscardFate
End Sub

Private Sub UpdateSpeciesSequence()
Dim i As Integer
Dim SQL As String
    SQL = "SELECT * from [Group Info] where modelName='" + lastModel + "'  ORDER BY [Group Info].Sequence"
    Set g_Recordset = CCG.UpdatableRecords(SQL)  ' g_databas.OpenRecordset(SQL)
    i = 1
    g_Recordset.MoveFirst
    Do While Not g_Recordset.EOF
        Specie(i) = g_Recordset.Fields("groupName").value
        g_Recordset.Fields("Sequence").value = i
        i = i + 1
        g_Recordset.Update
        g_Recordset.MoveNext
    Loop
End Sub

Private Sub AggregateRemarks(Table As String, groupField As String, RemField As String, RefField As String, Optional Field1 As String, Optional Name As String, Optional sceneField As String, Optional sceneName As String)
Dim SQL As String
Dim Remark1 As String
Dim Remark2 As String
Dim Ref1 As Long
Dim Ref2 As Long
Dim QRef As String
    'First Agg1:
    SQL = "SELECT * from " + Table + " where modelName='" + lastModel
    SQL = SQL + "' and " + groupField + "= '" + Specie(Agg1) + "'"
    If Field1 <> "" Then SQL = SQL + " and " + Field1 + "= '" + Name + "'"
    If sceneField <> "" Then SQL = SQL + " and " + sceneField + "= '" + sceneName + "'"
    Set g_Recordset = CCG.UpdatableRecords(SQL)  ' g_databas.OpenRecordset(SQL)
    If g_Recordset.RecordCount > 0 Then
        If IsNull(g_Recordset.Fields(RemField).value) Then
            Remark1 = ""
        Else
            Remark1 = g_Recordset.Fields(RemField).value
        End If
        If IsNull(g_Recordset.Fields(RefField).value) Then
            Ref1 = 0
        Else
            Ref1 = g_Recordset.Fields(RefField).value
        End If
    Else
        Remark1 = ""
        Ref1 = 0
    End If

    'Next Agg2:
    SQL = "SELECT * from " + Table + " where modelName='" + lastModel
    SQL = SQL + "' and " + groupField + "= '" + Specie(Agg1) + "'"
    If Field1 <> "" Then SQL = SQL + " and " + Field1 + "= '" + Name + "'"
    If sceneField <> "" Then SQL = SQL + " and " + sceneField + "= '" + sceneName + "'"
    Set x_Recordset = CCG.UpdatableRecords(SQL)  ' g_databas.OpenRecordset(SQL)
    If x_Recordset.RecordCount > 0 Then
        If IsNull(x_Recordset.Fields(RemField).value) Then
            Remark2 = ""
        Else
            Remark2 = x_Recordset.Fields(RemField).value
        End If
        If IsNull(x_Recordset.Fields(RefField).value) Then
            Ref2 = 0
        Else
            Ref2 = x_Recordset.Fields(RefField).value
        End If
    Else
        Remark2 = ""
        Ref2 = 0
    End If

    'Got the rem and ref so update the database:
    If Ref1 > 0 Or Ref2 > 0 Then
        If Ref1 = 0 Then    'Only a reference for Agg2:
            g_Recordset.Fields(RefField).value = Ref2
            Ref2 = 0
        Else    'A ref is present for Agg1
            g_Recordset.Fields(RefField).value = Ref1
        End If
        g_Recordset.Update
    End If
    QRef = ""
    If Remark1 <> "" Or Remark2 <> "" Then
        If Ref2 > 0 Then QRef = ";  " + frmReferences.GetQuickRef(Ref2)
        g_Recordset.Fields(RemField).value = Remark1 + "; " + Remark2 + QRef
        g_Recordset.Update
    End If

End Sub

Private Sub AggregateSpeciesList()
Dim SQL As String
Dim TaxCode As Long
Dim prop As Single
Dim Name As String
Dim Remark1 As String
Dim Remark2 As String
Dim Ref1 As Long
Dim Ref2 As Long
Dim QRef As String
    'First Agg1:
    'Agg2:
    SQL = "SELECT * from [Group Taxon] where modelName='" + lastModel
    SQL = SQL + "' and groupName= '" + Specie(Agg2) + "'"
    Set x_Recordset = CCG.UpdatableRecords(SQL)

    If x_Recordset.RecordCount > 0 Then
        x_Recordset.MoveFirst
        Do While Not x_Recordset.EOF
            TaxCode = x_Recordset.Fields("code").value
            Name = x_Recordset.Fields("name").value
            prop = x_Recordset.Fields("proportion").value
            SQL = "SELECT * from [Group Taxon] where modelName='" + lastModel
            SQL = SQL + "' and groupName= '" + Specie(Agg2)
            SQL = SQL + "' and code=" + CStr(TaxCode)
            Set g_Recordset = CCG.UpdatableRecords(SQL)
            If g_Recordset.RecordCount = 0 Then g_Recordset.AddNew
            g_Recordset.Fields("modelName").value = lastModel
            g_Recordset.Fields("groupName").value = Specie(Agg2)
            g_Recordset.Fields("code").value = TaxCode
            g_Recordset.Fields("name").value = Name
            g_Recordset.Fields("proportion").value = prop
            g_Recordset.Update
            x_Recordset.MoveNext
        Loop
    End If
End Sub

#End If
#End Region ' Original code

End Class
