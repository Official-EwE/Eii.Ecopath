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
    ''' Returns the proportion that a given organism type contributes to the 
    ''' biomass of a functional group.
    ''' </summary>
    ''' <param name="iGroup">The one-based index of the group to check.</param>
    ''' <param name="organism">The <see cref="eOrganismTypes">organism type</see> to check.</param>
    ''' <returns>A proportion [0, 1].</returns>
    ''' -----------------------------------------------------------------------
    Public Function GroupOrganismProportion(ByVal iGroup As Integer, _
                                            ByVal organism As eOrganismTypes) As Single

        ' Iterate over all taxa attached to iGroup
        '    if taxon matches organismType, add taxon proportion
        ' return total added proportion

        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0

        For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
            Dim iTaxon As Integer = Me.m_taxonDS.GroupTaxa(iGroup, i)
            If (Me.m_taxonDS.TaxonOrganismType(iTaxon) = organism) Then
                sProportion = sProportion + Me.m_taxonDS.TaxonProp(iTaxon)
            End If
            sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
        Next

        If (sPropTot = 0) Then Return 0
        Return sProportion / sPropTot

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the proportion that a given organism type contributes to the 
    ''' catch of a functional group.
    ''' </summary>
    ''' <param name="iGroup">The one-based index of the group to check.</param>
    ''' <param name="organism">The <see cref="eOrganismTypes">organism type</see> to check.</param>
    ''' <returns>A proportion [0, 1].</returns>
    ''' -----------------------------------------------------------------------
    Public Function GroupOrganismCatchProportion(ByVal iGroup As Integer, _
                                                 ByVal organism As eOrganismTypes) As Single

        ' Iterate over all taxa attached to iGroup
        '    if taxon matches organismType, add taxon catch proportion
        ' return total added proportion

        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0

        For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
            Dim iTaxon As Integer = Me.m_taxonDS.GroupTaxa(iGroup, i)
            If (Me.m_taxonDS.TaxonOrganismType(iTaxon) = organism) Then
                sProportion = sProportion + Me.m_taxonDS.TaxonPropCatch(iTaxon)
            End If
            sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
        Next

        If (sPropTot = 0) Then Return 0
        Return sProportion / sPropTot

    End Function

    Public Function GroupOrganismOcurrencestatus(ByVal iGroup As Integer, _
                                                 ByVal status As eOccurrenceStatusTypes) As Single

        ' Iterate over all taxa attached to iGroup
        '    if taxon matches occurrenceType, add taxon catch proportion
        ' return total added proportion

        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0

        For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
            Dim iTaxon As Integer = Me.m_taxonDS.GroupTaxa(iGroup, i)
            If (Me.m_taxonDS.TaxonOccurrenceStatus(iTaxon) = status) Then
                sProportion = sProportion + Me.m_taxonDS.TaxonPropCatch(iTaxon)
            End If
            sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
        Next

        If (sPropTot = 0) Then Return 0
        Return sProportion / sPropTot

    End Function


    Public Function GroupOrganismIUCNstatus(ByVal iGroup As Integer, _
                                            ByVal IUCN As eIUCNConservationStatusTypes) As Single

        ' Iterate over all taxa attached to iGroup
        '    if taxon matches IUCNconservationStatusTypes, add taxon catch proportion
        ' return total added proportion

        Dim sProportion As Single = 0
        Dim sPropTot As Single = 0

        For i As Integer = 1 To Me.m_taxonDS.NumGroupTaxa(iGroup)
            Dim iTaxon As Integer = Me.m_taxonDS.GroupTaxa(iGroup, i)
            If (Me.m_taxonDS.TaxonIUCNConservationStatus(iTaxon) >= IUCN) Then
                sProportion = sProportion + Me.m_taxonDS.TaxonPropCatch(iTaxon)
            End If
            sPropTot += Me.m_taxonDS.TaxonProp(iTaxon)
        Next

        If (sPropTot = 0) Then Return 0
        Return sProportion / sPropTot

    End Function

#Region " Filters "

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

#End Region ' Filters

End Class
