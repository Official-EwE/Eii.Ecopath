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
Imports EwECore.Auxiliary

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class to merge any number of groups in Ecopath.
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
    ''' <param name="groups">Array of one-based group indexes to merge.</param>
    ''' <param name="strName">A suggested name for the aggregation of groups.</param>
    ''' <returns>True if the proposed merge can be executed.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CanMergeGroups(groups() As Integer, strName As String) As Boolean

        If Not (Me.CanMergeGroups(False) = True) Then Return False
        If String.IsNullOrWhiteSpace(strName) Then Return False
        If (groups.Length < 2) Then Return False

        Dim comp As Integer() = Me.CompatibleGroups(groups(0))
        For i As Integer = 1 To groups.Length - 1
            If Array.IndexOf(comp, groups(i)) = -1 Then Return False
        Next
        Return True

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
                Dim iStanza As Integer = stanzads.SpeciesCode(iGroup, 0)

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
    ''' Merge groups.
    ''' </summary>
    ''' <param name="groups">Array of one-based indexes of groups to merge.</param>
    ''' <param name="strName">The name to assign to the merged group.</param>
    ''' <param name="bMergeColors">Flag, stating if the colour of the resulting 
    ''' group must be an average of both group colours.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Merge(groups() As Integer, strName As String,
                          Optional bMergeColors As Boolean = False) As Boolean

        ' Sanity check
        If Not CanMergeGroups(groups, strName) Then Return False

        Dim ecopathds As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim taxonds As cTaxonDataStructures = Me.m_core.m_TaxonData
        Dim iTarget As Integer = groups(0)

        Dim Ptot As Single = 0
        Dim Qtot As Single = 0
        Dim Btot As Single = 0
        Dim EEtot As Single = 0
        Dim GStot As Single = 0
        Dim BaBitot As Single = 0
        Dim BAtot As Single = 0
        Dim Immigtot As Single = 0
        Dim Emigtot As Single = 0
        Dim Emigrationtot As Single = 0
        Dim Shadowtot As Single = 0
        Dim Det0tot As Single = 0
        Dim Catchtot As Single = 0
        Dim Discardtot(ecopathds.NumFleet) As Single
        Dim PropDiscardtot(ecopathds.NumFleet) As Single
        Dim PropDiscardMorttot(ecopathds.NumFleet) As Single
        Dim Landingtot(ecopathds.NumFleet) As Single
        Dim DiscardFatetot(ecopathds.NumFleet) As Single
        Dim Markettot(ecopathds.NumFleet) As Single
        Dim DCtot(ecopathds.NumGroups) As Single
        Dim DtImptot As Single
        Dim BiomassProptot(taxonds.NumTaxon) As Single
        Dim CatchProptot(taxonds.NumTaxon) As Single

        ' Color
        Dim ColorRtot As Integer = 0
        Dim ColorGtot As Integer = 0
        Dim ColorBtot As Integer = 0

        For i As Integer = 0 To groups.Length - 1
            Dim iMerge As Integer = groups(i)

            Ptot += (ecopathds.PB(iMerge) * ecopathds.B(iMerge))
            Qtot += (ecopathds.QB(iMerge) * ecopathds.B(iMerge))
            EEtot += (ecopathds.EE(iMerge) * ecopathds.B(iMerge))
            GStot += (ecopathds.GS(iMerge) * ecopathds.B(iMerge) * ecopathds.QB(iMerge))
            BaBitot += (ecopathds.BaBi(iMerge) * ecopathds.B(iMerge))
            Btot += ecopathds.B(iMerge)
            Immigtot += ecopathds.Immig(iMerge)
            Emigrationtot += ecopathds.Emigration(iMerge)
            Emigtot += (ecopathds.Emig(iMerge) * ecopathds.B(iMerge))
            Shadowtot += (ecopathds.Shadow(iMerge) * ecopathds.B(iMerge))
            Det0tot += ecopathds.det(0, iMerge)
            Catchtot += ecopathds.fCatch(iMerge)

            For iFleet As Integer = 1 To ecopathds.NumFleet
                Discardtot(iFleet) += ecopathds.Discard(iFleet, iMerge)
                Landingtot(iFleet) += ecopathds.Landing(iFleet, iMerge)

                If (ecopathds.PropDiscard(iFleet, iMerge) > 0) Then
                    PropDiscardtot(iFleet) += (ecopathds.PropDiscard(iFleet, iMerge) * ecopathds.Discard(iFleet, iMerge))
                End If
                If (ecopathds.PropDiscardMort(iFleet, iMerge) > 0) Then
                    PropDiscardMorttot(iFleet) += (ecopathds.PropDiscardMort(i, iMerge) * ecopathds.Discard(iFleet, iMerge))
                End If
                Markettot(iFleet) += (ecopathds.Market(iFleet, iMerge) * ecopathds.Landing(iFleet, iMerge))

                If iMerge > ecopathds.NumLiving Then
                    DiscardFatetot(iFleet) += (ecopathds.Discard(iFleet, iMerge) * ecopathds.DiscardFate(iFleet, iMerge - ecopathds.NumLiving))
                End If
            Next

            ' -- Diet --
            ' Is living?
            If (iTarget <= ecopathds.NumLiving) Then
                ' #Yes
                If ecopathds.QB(iMerge) > 0 Then
                    For j As Integer = 1 To ecopathds.NumGroups          'prey
                        If ecopathds.B(j) > 0 Then        'Exclude aggregated prey
                            If Qtot > 0 Then 'Eaten by the two groups of all prey
                                DCtot(j) += (ecopathds.DCInput(iMerge, j) * ecopathds.QB(iMerge) * ecopathds.B(iMerge))
                            End If
                        End If
                    Next j
                    If Qtot > 0 Then DCtot(0) += (ecopathds.DCInput(iMerge, 0) * ecopathds.QB(iMerge) * ecopathds.B(iMerge))
                End If
            Else
                ' #No: it's detritus, do dtimp
                DtImptot += ecopathds.DtImp(iMerge)
            End If

            ' -- Colors --
            Dim c As Color = cColorUtils.IntToColor(ecopathds.GroupColor(iMerge))
            ColorRtot += CInt(c.R)
            ColorGtot += CInt(c.G)
            ColorBtot += CInt(c.B)

            ' -- Taxa --
            For j As Integer = 1 To taxonds.NumTaxon
                If (taxonds.IsTaxonStanza(j) = False) Then
                    If (taxonds.TaxonTarget(j) = iMerge) Then
                        BiomassProptot(j) += (taxonds.TaxonPropBiomass(j) * ecopathds.B(iMerge))
                        CatchProptot(j) += (taxonds.TaxonPropCatch(j) * ecopathds.fCatch(iMerge))
                    End If
                End If
            Next

        Next

        ecopathds.GroupName(iTarget) = strName

        ' Merge generic fields
        ecopathds.PBinput(iTarget) = Ptot / Btot
        ecopathds.BaBi(iTarget) = BaBitot / Btot
        ecopathds.BA(iTarget) = BAtot
        ecopathds.Immig(iTarget) = Immigtot
        ecopathds.Emigration(iTarget) = Emigrationtot
        ecopathds.Emig(iTarget) = Emigtot / Btot
        ' Catch is calculated on the fly from landings and discards; no need to update
        'Catch(Target) = Catch(Target) + Catch(Agg2)
        ecopathds.det(0, iTarget) = Det0tot

        If (iTarget > ecopathds.NumLiving) Then
            For i As Integer = 1 To ecopathds.NumFleet
                If Discardtot(i) > 0 Then
                    ecopathds.DiscardFate(i, iTarget - ecopathds.NumLiving) = DiscardFatetot(i) / Discardtot(i)
                End If
            Next
        End If

        ' Living groups?
        If iTarget <= ecopathds.NumLiving Then
            ecopathds.EEinput(iTarget) = EEtot / Btot
            If Qtot > 0 Then    'Weighted after consumption
                ecopathds.GS(iTarget) = GStot / Qtot
            End If
        End If
        ecopathds.Shadow(iTarget) = Shadowtot / Btot
        ecopathds.DtImp(iTarget) = DtImptot
        ecopathds.QBinput(iTarget) = Qtot / Btot

        For i As Integer = 0 To ecopathds.NumGroups
            ecopathds.DCInput(i, iTarget) = DCtot(i)
            ecopathds.DietWasChanged(i, iTarget)
        Next i

        'Landing and discards are just summed
        For i As Integer = 1 To ecopathds.NumFleet
            If Landingtot(i) > 0 Then
                ecopathds.Market(i, iTarget) = Markettot(i) / Landingtot(i)
            End If
            ecopathds.Landing(i, iTarget) = Landingtot(i)
            ecopathds.Discard(i, iTarget) = Discardtot(i)
            ecopathds.PropDiscard(i, iTarget) = PropDiscardtot(i) / Discardtot(i)
            ecopathds.PropDiscardMort(i, iTarget) = PropDiscardMorttot(i) / Discardtot(i)
        Next

        ecopathds.Binput(iTarget) = Btot
        'Cannot know which combined area we are talking about, so use the area for the first group:
        ecopathds.BHinput(iTarget) = ecopathds.Binput(iTarget) / ecopathds.Area(iTarget)
        ' No need clearing values: groups will be deleted shortly
        'ecopathds.Binput(agg2) = 0
        'ecopathds.BHinput(agg2) = 0

        If bMergeColors Then
            ecopathds.GroupColor(iTarget) = cColorUtils.ColorToInt(Color.FromArgb(255, ColorRtot \ groups.Length, ColorGtot \ groups.Length, ColorGtot \ groups.Length))
        End If

        If Me.m_core.m_EcoPathData.StanzaGroup(iTarget) Then

            ' Perform stanza merge
            Dim stanzads As cStanzaDatastructures = Me.m_core.m_Stanza
            Dim Ztot As Single = 0
            Dim iStanza As Integer = -1

            For i As Integer = 1 To groups.Length - 1

                Dim iLifestage1 As Integer = -1
                Dim iLifestage2 As Integer = -1
                Dim iMerge As Integer = groups(i)

                For isp As Integer = 1 To stanzads.Nsplit
                    For ist As Integer = 1 To stanzads.Nstanza(isp)
                        If stanzads.EcopathCode(isp, ist) = iTarget Then
                            iLifestage1 = ist
                            iStanza = isp
                        End If
                        If stanzads.EcopathCode(isp, ist) = iMerge Then iLifestage2 = ist
                    Next
                Next

                Debug.Assert(iStanza >= 0 And iLifestage1 >= 0 And iLifestage2 >= 0)

                If stanzads.BaseStanza(iStanza) = iMerge Then stanzads.BaseStanza(iStanza) = iTarget
                If stanzads.BaseStanzaCB(iStanza) = iMerge Then stanzads.BaseStanzaCB(iStanza) = iTarget
                stanzads.Age1(iStanza, iTarget) = Math.Min(stanzads.Age1(iStanza, iTarget), stanzads.Age1(iStanza, iMerge))
                Ztot += stanzads.Stanza_Z(iStanza, iMerge)
            Next

            stanzads.Stanza_Z(iStanza, iTarget) = Ztot / groups.Length

        End If

        ' -- Taxa --
        For j As Integer = 1 To taxonds.NumTaxon
            If (BiomassProptot(j) > 0) Then
                taxonds.TaxonTarget(j) = iTarget
                BiomassProptot(j) /= Btot
                CatchProptot(j) /= Catchtot
            End If
        Next

        '  -- Auxillary data (such as remarks) --
        ' First loop: add all remarks that belong to one of the merged groups to Target
        ' Get Target remark list
        Dim dicTarget As Dictionary(Of String, cAuxiliaryData) = Me.m_core.AuxillaryData(eDataTypes.EcoPathGroupInput, ecopathds.GroupDBID(iTarget), False)
        ' For all merged groups:
        For i As Integer = 1 To groups.Length - 1
            Dim iMerge As Integer = groups(i)
            ' Get aggregated group remark list
            Dim dicMerge As Dictionary(Of String, cAuxiliaryData) = Me.m_core.AuxillaryData(eDataTypes.EcoPathGroupInput, ecopathds.GroupDBID(iMerge))
            For Each strMerge As String In dicMerge.Keys
                ' Deconstruct key to identify aux data
                Dim vid As cValueID = cValueID.FromString(strMerge)
                ' Switch key from Merge group to Target group
                vid.DBIDPrim = ecopathds.GroupDBID(iTarget)
                ' Construct target key
                Dim strTarget As String = vid.ToString()
                ' Iis this an existing target key?
                If (dicTarget.ContainsKey(strTarget)) Then
                    ' #Yes: Merge auxillary data
                    Me.m_core.AuxillaryData(strTarget).MergeWith(dicMerge(strMerge))
                Else
                    ' #No: Move auxillary data
                    Me.m_core.AuxillaryData(strTarget) = dicMerge(strMerge)
                End If
                ' Remove Merge data from the core, not needed anymore
                Me.m_core.AuxillaryData(strMerge) = Nothing
            Next
        Next

        ' Second loop: fix up all auxillary data that relate to groups via secondary indexes, and that are going to disappear
        For i As Integer = 1 To groups.Length - 1
            ' Get all auxillary data that refers to a disappearing group
            Dim dicAgg2 As Dictionary(Of String, cAuxiliaryData) = Me.m_core.AuxillaryData(eDataTypes.EcoPathGroupInput, ecopathds.GroupDBID(groups(i)), True)
            ' For all goners
            For Each strKeyOrg As String In dicAgg2.Keys
                ' Deconstruct key to identify aux data
                Dim vid As cValueID = cValueID.FromString(strKeyOrg)
                ' Is this item referring to a disappearing group?
                If vid.DataTypeSec = eDataTypes.EcoPathGroupInput And vid.DBIDSec = ecopathds.GroupDBID(groups(i)) Then
                    ' #Yes: reroute auxilary data to Target
                    vid.DBIDSec = ecopathds.GroupDBID(iTarget)
                    Me.m_core.AuxillaryData(vid.ToString) = Me.m_core.AuxillaryData(strKeyOrg)
                    Me.m_core.AuxillaryData(strKeyOrg) = Nothing
                End If
            Next
        Next

        ' Done
        Me.m_core.DataSource.SetChanged(eCoreComponentType.EcoPath)
        Me.m_core.StateMonitor.UpdateDataState(Me.m_core.DataSource)

        If Me.m_core.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath) Then
            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)

            ' Remove all merged groups in decending order
            Dim bOK As Boolean = True
            Array.Sort(groups)
            For i As Integer = groups.Length - 1 To 0 Step -1
                If (i <> iTarget) Then
                    bOK = bOK And Me.m_core.RemoveGroup(groups(i))
                End If
            Next

            Return Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, bOK)
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
        Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.EcoPath,
                                cSystemUtils.IIF(bSuccess, eMessageImportance.Information, eMessageImportance.Critical))
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region ' Internals

End Class
