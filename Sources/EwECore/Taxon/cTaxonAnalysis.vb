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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to analyze taxonomy data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTaxonAnalysis

    Private m_taxonDS As cTaxonDataStructures = Nothing
    Private m_dt As New Dictionary(Of String, Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance of the taxon analysis class.
    ''' </summary>
    ''' <param name="taxonDS">The <see cref="cTaxonDataStructures">taxonomy data structures</see>
    ''' to obtain taxon data from.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal taxonDS As cTaxonDataStructures)
        Me.m_taxonDS = taxonDS
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the biomass proportion of all taxa for a single group matching 
    ''' a specific condition.
    ''' </summary>
    ''' <param name="iGroup">The group to obtain taxa for.</param>
    ''' <param name="val">The value to test against. Supported value types are
    ''' <see cref="eOccurrenceStatusTypes"/>, <see cref="eIUCNConservationStatusTypes"/>, 
    ''' <see cref="eOrganismTypes"/> and <see cref="eEcologyTypes"/></param>
    ''' <param name="op">The <see cref="eOperators">operation</see> to perform.
    ''' If not provided <see cref="eOperators.EqualTo">'='</see> is used.</param>
    ''' <returns>The proportion of biomass.</returns>
    ''' <example lang="VB.NET">
    ''' <code>
    ''' Dim taxonanalysis As cTaxonAnalysis = Me.m_core.TaxonAnalysis
    ''' Dim Binv As Single = 0
    ''' Dim Bnt as single = 0
    ''' 
    ''' For iGroup As Integer = 1 To Me.m_core.NumGroups
    '''     ' Sum up the biomass for all invertebrates
    '''     Binv += Me.m_core.EcopathGroupOutput(iGroup).Biomass * taxonanalysis.GroupBiomassProportion(iGroup, eOrganismTypes.Invertebrates))
    '''     ' Sum up the biomass for all species with a IUCN status of near-threathened or worse
    '''     Bnt += Me.m_core.EcopathGroupOutput(iGroup).Biomass * taxonanalysis.GroupBiomassProportion(iGroup, eIUCNConservationStatusTypes.NearThreatened, eOperators.GreaterThanOrEqualTo))
    ''' Next iGroup
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------
    Public Function GroupBiomassProportion(iGroup As Integer, _
                                           val As Object, _
                                           Optional op As eOperators = eOperators.EqualTo) As Single
        Return Me.GroupProportion(True, iGroup, val, op)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the catch proportion of all taxa for a single group matching 
    ''' a specific condition.
    ''' </summary>
    ''' <param name="iGroup">The group to obtain taxa for.</param>
    ''' <param name="val">The value to test against. Supported value types are
    ''' <see cref="eOccurrenceStatusTypes"/>, <see cref="eIUCNConservationStatusTypes"/>, 
    ''' <see cref="eOrganismTypes"/> and <see cref="eEcologyTypes"/></param>
    ''' <param name="op">The <see cref="eOperators">operation</see> to perform.
    ''' If not provided <see cref="eOperators.EqualTo">'='</see> is used.</param>
    ''' <returns>The proportion of catch.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GroupCatchProportion(iGroup As Integer, _
                                       val As Object, _
                                       Optional op As eOperators = eOperators.EqualTo) As Single
        Return Me.GroupProportion(False, iGroup, val, op)
    End Function

#Region " Internals "

    Private Function GroupProportion(ByVal bBiomass As Boolean, _
                                     ByVal iGroup As Integer, _
                                     ByVal value As Object, _
                                     Optional ByVal op As eOperators = eOperators.EqualTo) As Single

        Dim iTaxon As Integer = 0
        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0
        Dim comp As cOperatorBase = cOperatorManager.getOperator(op)
        Dim avals As Array = Nothing
        Dim sVal As Single = CSng(value)
        Dim strKey As String = Me.Key(bBiomass, iGroup, sVal, op)

        If (Not Me.m_dt.ContainsKey(strKey)) Then

            If TypeOf (value) Is eOrganismTypes Then
                avals = Me.m_taxonDS.TaxonOrganismType
            ElseIf TypeOf (value) Is eIUCNConservationStatusTypes Then
                avals = Me.m_taxonDS.TaxonIUCNConservationStatus
            ElseIf TypeOf (value) Is eExploitationTypes Then
                avals = Me.m_taxonDS.TaxonExploitationStatus
            ElseIf TypeOf (value) Is eEcologyTypes Then
                avals = Me.m_taxonDS.TaxonEcologyType
            ElseIf TypeOf (value) Is eOccurrenceStatusTypes Then
                avals = m_taxonDS.TaxonOccurrenceStatus
            End If

            Debug.Assert(avals IsNot Nothing)

            For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
                iTaxon = Me.m_taxonDS.GroupTaxa(iGroup, i)
                If (comp.Compare(CSng(avals.GetValue(iTaxon)), sVal)) Then
                    If bBiomass Then
                        sProportion += Me.m_taxonDS.TaxonProp(iTaxon)
                    Else
                        sProportion += Me.m_taxonDS.TaxonPropCatch(iTaxon)
                    End If
                End If
                If bBiomass Then
                    sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
                Else
                    sPropTot += Me.m_taxonDS.TaxonPropCatch(iTaxon)
                End If
            Next

            If (sPropTot = 0) Then
                sVal = 0
            Else
                sVal = sProportion / sPropTot
            End If
            Me.m_dt(strKey) = sVal

        End If
        Return Me.m_dt(strKey)

    End Function

    Private Function Key(ByVal bBiomass As Boolean, _
                         ByVal iGroup As Integer, _
                         ByVal sVal As Single, _
                         ByVal op As eOperators) As String
        Return iGroup & "_" & sVal & "_" & op & "_" & bBiomass
    End Function

#End Region ' Internals

End Class
