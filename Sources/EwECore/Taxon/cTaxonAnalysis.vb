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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
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
    ''' -----------------------------------------------------------------------
    Public Function GroupBiomassProportion(iGroup As Integer, val As Object, _
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

#Region " Filters "
#If 0 Then

    ' ToDo_JS: consider solving this using LINQ (custom IQueryable LINQ provider, custom operators, etc)

    ' This would be ideal:
    '    SELECT Taxa FROM group WHERE .Organism = eOrganismTypes.Fishes AND .IUCNStatus < eIUCNConservationStatusTypes.Endangered

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns those indexes that occur in both collections.
    ''' </summary>
    ''' <param name="taxa1"></param>
    ''' <param name="taxa2"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function [Overlap](taxa1 As ICollection(Of Integer), taxa2 As ICollection(Of Integer)) As Integer()
        Dim li As New List(Of Integer)
        For Each i As Integer In taxa1
            If taxa2.Contains(i) Then li.Add(i)
        Next
        Return li.ToArray
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns those indexes that occur in either collection.
    ''' </summary>
    ''' <param name="taxa1"></param>
    ''' <param name="taxa2"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function [Join](taxa1 As ICollection(Of Integer), taxa2 As ICollection(Of Integer)) As Integer()
        Dim li As New List(Of Integer)
        li.AddRange(taxa2)
        For Each i As Integer In taxa1
            If Not taxa2.Contains(i) Then li.Add(i)
        Next
        Return li.ToArray
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all taxa for a group that match a given condition.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="iGroup"></param>
    ''' <param name="var"></param>
    ''' <param name="op"></param>
    ''' <param name="sValue"></param>
    ''' <returns>An array of <see cref="cTaxon.Index">taxon indices</see> of taxa 
    ''' that match the given condition.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetGroupTaxa(ByVal core As cCore, _
                                        ByVal iGroup As Integer, _
                                        ByVal var As eVarNameFlags, _
                                        ByVal op As EwECore.eOperators, _
                                        ByVal sValue As Single) As Integer()

        Dim lTaxa As New List(Of Integer)
        Dim tax As cTaxon = Nothing
        Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
        Dim iTaxon As Integer = 0
        Dim comp As cOperatorBase = cOperatorManager.getOperator(op)

        For i As Integer = 1 To grp.NTaxon

            iTaxon = grp.iTaxon(i)
            tax = core.Taxon(iTaxon)

            If comp.Compare(CSng(tax.GetVariable(var)), sValue) Then
                lTaxa.Add(iTaxon)
            End If

        Next

        Return lTaxa.ToArray()

    End Function

#End If

#End Region ' Filters

#Region " Internals "

    Private Function GroupProportion(ByVal bBiomass As Boolean, _
                                     ByVal iGroup As Integer, _
                                     ByVal value As Object, _
                                     Optional ByVal op As eOperators = eOperators.EqualTo) As Single

        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0
        Dim comp As cOperatorBase = cOperatorManager.getOperator(op)
        Dim avals As Array = Nothing
        Dim sVal As Single = CSng(value)

        If TypeOf (value) Is eOrganismTypes Then
            avals = Me.m_taxonDS.TaxonOrganismType
        ElseIf TypeOf (value) Is eIUCNConservationStatusTypes Then
            avals = Me.m_taxonDS.TaxonIUCNConservationStatus
        ElseIf TypeOf (value) Is eEcologyTypes Then
            avals = Me.m_taxonDS.TaxonEcologyType
        ElseIf TypeOf (value) Is eOccurrenceStatusTypes Then
            avals = m_taxonDS.TaxonOccurrenceStatus
        End If

        Debug.Assert(avals IsNot Nothing)

        For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
            Dim iTaxon As Integer = Me.m_taxonDS.GroupTaxa(iGroup, i)
            If (comp.Compare(CSng(avals.GetValue(iTaxon)), sVal)) Then
                If bBiomass Then
                    sProportion = sProportion + Me.m_taxonDS.TaxonProp(iTaxon)
                Else
                    sProportion = sProportion + Me.m_taxonDS.TaxonPropCatch(iTaxon)
                End If
            End If
            If bBiomass Then
                sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
            Else
                sPropTot += Me.m_taxonDS.TaxonPropCatch(iTaxon)
            End If
        Next

        If (sPropTot = 0) Then Return 0
        Return sProportion / sPropTot

    End Function

#End Region ' Internals

End Class
