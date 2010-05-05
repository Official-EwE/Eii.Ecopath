Public Class cStanzaDatastructures

    'this changes how many packets are used per age (default 0.5)
    '# packets = # cells * NPacketsMultiplier
    Public NPacketsMultiplier As Single

    'These are new IBM variables
    Public EggAtSpawn As Boolean
    Public EggCell(,,) As Single 'eggs per cell and species
    Public AgeIndex1() As Integer ' index of the age one creatures of a species in Npacket and Wpacket
    Public StanzaNo(,) As Integer
    Public Npacket(,,) As Single 'number of fish in the packet (species, age, packet#)
    Public Wpacket(,,) As Single ' weight of fish in the packet
    Public IBMMovesPerMonth() As Integer
    Public IBMdistmove(,) As Single
    Public iPacket(,,) As Single ' i position index of the packet
    Public jPacket(,,) As Single 'j position index of the packet
    Public iNursery(,) As Integer
    Public jNursery(,) As Integer
    Public Nnursery() As Integer
    Public Zcell(,,) As Single   'mortality rate by cell and species
    Public MaxAgeSpecies() As Integer
    Public Npackets As Integer  'total # of packets per age

    ''' <summary>Max number of stanazs across all the stanza groups.</summary>
    Public MaxStanza As Integer
    ''' <summary>The number of stanza groups (split groups).</summary>
    Public Nsplit As Integer
    ''' <summary>For redimensioning SpeciesCode.</summary>
    Public nGroups As Integer '

    Public StanzaDBID() As Integer

    Public BaseStanza() As Integer
    Public BaseStanzaCB() As Integer
    Public BABsplit() As Single

    ''' <summary>Number of stanzas in each split group.</summary>
    Public Nstanza() As Integer
    ''' <summary>Group index (iGroup) for this (Nsplit, nStanza).</summary>
    Public EcopathCode(,) As Integer
    Public MaxAgeSplit As Integer
    Public NumSplit(,) As Single
    Public SplitRflow(,) As Single
    ''' <summary>Numbers at age (dynamic) for split species (set in initialstate using ecopath base array SplitNo(isp,age)).</summary>
    Public NageS(,) As Single
    ''' <summary>Weights at age (dynamic)(set in initialstate) (set in initialstate using ecopath base array SplitWage(isp,age).</summary>
    Public WageS(,) As Single
    ''' <summary>Base recruitment to age 0 for split species.</summary>
    Public RzeroS() As Single
    Public SplitAlpha(,) As Single 'growth coefficients by split spp and age (set in initialstate)
    Public RscaleSplit() As Single
    Public EggsSplit(,) As Single
    Public Age1(,) As Integer
    Public Age2(,) As Integer
    Public SplitNo(,) As Single
    Public SplitWage(,) As Single
    Public WWa(,) As Single

    Public StanzaName() As String
    Public Stanza_Z(,) As Single
    Public Stanza_Bio(,) As Single
    Public Stanza_CB(,) As Single
    ' JS 200606: Disabled; GUI-only flag whose value can be deducted.
    'Public LockedParameter() As Boolean
    Public CurrentStanza As Integer

    Public WmatWinf() As Single ' weight at maturity/ weight at infinity (max weight) from EwE5 interface
    Public EggsStanza() As Single

    ''' <summary>Boolean flag set in an interface.</summary>
    ''' <remarks>Used by SplitUpdate(b)</remarks>
    Public FixedFecundity() As Boolean
    Public BaseEggsStanza() As Single

    Public RecPowerSplit() As Single

    Public vBM() As Single
    Public HatchCode() As Integer

    Public EggProdShapeSplit() As Integer

    ''' <summary>Egg production shape is seasonal.</summary>
    Public EggProdIsSeasonal() As Boolean
    ''' <summary></summary>
    ''' <remarks>
    ''' <list type="bullet">
    ''' <item>0: Ecopath group no for this stanza.</item>
    ''' <item>1: Ecopath no for leading B stanza.</item>
    ''' <item>2: Ecopath no for leading QB stanza.</item>
    ''' </list>
    ''' </remarks>
    Public SpeciesCode(,) As Single

    ''' <summary>
    ''' Redimension the stanza arrays
    ''' </summary>
    Public Sub redimStanza()

        ReDim StanzaDBID(Nsplit)

        ReDim RecPowerSplit(Nsplit)
        ReDim Nstanza(Nsplit) 'number of stanzas by split species (set in ecopath)
        ReDim BaseStanza(Nsplit) 'holds stanzano for which info is entered
        ReDim BaseStanzaCB(Nsplit)
        ReDim EcopathCode(Nsplit, MaxStanza) 'ecopath group# by split species, stanza (set in ecopath)
        ReDim StanzaName(Nsplit)
        ReDim Age1(Nsplit, MaxStanza) 'first month of age by species, stanza (set in ecopath)
        ReDim Age2(Nsplit, MaxStanza) 'last month of age by spp, stanza (set in ecopath)
        ReDim Stanza_Z(Nsplit, MaxStanza) 'mortality
        ReDim Stanza_Bio(Nsplit, MaxStanza) 'mortality
        ReDim Stanza_CB(Nsplit, MaxStanza) 'mortality
        ReDim RzeroS(Nsplit) 'base recruitment to age 0 for split species
        'redim PredS() 'effective predator abund for split species (set in ecosim splitpred)
        ReDim SplitAlpha(Nsplit, MaxAgeSplit) 'growth coefficients by split spp and age (set in initialstate)
        ReDim vBM(Nsplit)  'metabolic parameter 1-3*K by split species (set in ecopath)
        ' ReDim vBMann(Nsplit)
        ReDim WWa(Nsplit, MaxAgeSplit)
        ReDim SplitNo(Nsplit, MaxAgeSplit)
        ReDim SplitWage(Nsplit, MaxAgeSplit)
        ReDim HatchCode(Nsplit)
        ReDim WmatWinf(Nsplit)
        ReDim EggsStanza(Nsplit)
        ReDim FixedFecundity(Nsplit)
        ReDim BaseEggsStanza(Nsplit)
        ReDim EggProdShapeSplit(Nsplit)
        ReDim EggProdIsSeasonal(Nsplit)
        ReDim BABsplit(Nsplit)

        'variables by nGroups
        ReDim SpeciesCode(nGroups, 2) '0: Ecopath group no for this stanza, 1: Ecopath no for leading B stanza, 2: Ecopath no for leading QB stanza

        ReDim WmatWinf(Nsplit)

    End Sub

    Public Sub Clear()
        Me.Nsplit = 0
        Me.nGroups = 0
        Me.redimStanza()
    End Sub

    Public Sub copyTo(ByRef d As cStanzaDatastructures)
        Try
            d.MaxStanza = MaxStanza
            d.Nsplit = Nsplit
            d.nGroups = nGroups
            d.MaxAgeSplit = MaxAgeSplit

            d.redimStanza()

            NPacketsMultiplier = d.NPacketsMultiplier
            EggAtSpawn = d.EggAtSpawn
            Npackets = d.Npackets

            'EggCell.CopyTo(d.EggCell, 0)
            'AgeIndex1.CopyTo(d.AgeIndex1, 0)
            'StanzaNo.CopyTo(d.StanzaNo, 0)
            'Npacket.CopyTo(d.Npacket, 0)
            'Wpacket.CopyTo(d.Wpacket, 0)
            'IBMMovesPerMonth.CopyTo(d.IBMMovesPerMonth, 0)
            'IBMdistmove.CopyTo(d.IBMdistmove, 0)
            'iPacket.CopyTo(d.iPacket, 0)
            'jPacket.CopyTo(d.jPacket, 0)
            'iNursery.CopyTo(d.iNursery, 0)
            'jNursery.CopyTo(d.jNursery, 0)
            'Nnursery.CopyTo(d.Nnursery, 0)
            'Zcell.CopyTo(d.Zcell, 0)

            'MaxAgeSpecies.CopyTo(d.MaxAgeSpecies, 0)
            StanzaDBID.CopyTo(d.StanzaDBID, 0)

            BaseStanza.CopyTo(d.BaseStanza, 0)
            BaseStanzaCB.CopyTo(d.BaseStanzaCB, 0)
            BABsplit.CopyTo(d.BABsplit, 0)

            Nstanza.CopyTo(d.Nstanza, 0)
            d.EcopathCode = EcopathCode.Clone
            d.NumSplit = NumSplit.Clone
            d.SplitRflow = SplitRflow.Clone
            d.NageS = NageS.Clone
            d.WageS = WageS.Clone
            'RzeroS.CopyTo(d.RzeroS, 0)
            d.SplitAlpha = SplitAlpha.Clone
            d.RscaleSplit = RscaleSplit.Clone
            d.EggsSplit = EggsSplit.Clone
            d.Age1 = Age1.Clone
            d.Age2 = Age2.Clone
            d.SplitNo = SplitNo.Clone
            d.SplitWage = SplitWage.Clone
            d.WWa = WWa.Clone
            d.StanzaName = StanzaName.Clone
            d.Stanza_Z = Stanza_Z.Clone
            d.Stanza_Bio = Stanza_Bio.Clone
            d.Stanza_CB = Stanza_CB.Clone
            d.EggsStanza = EggsStanza.Clone


            d.FixedFecundity = FixedFecundity.Clone
            d.BaseEggsStanza = BaseEggsStanza.Clone
            d.RecPowerSplit = RecPowerSplit.Clone
            d.vBM = vBM.Clone
            d.HatchCode = HatchCode.Clone
            d.EggProdShapeSplit = EggProdShapeSplit.Clone
            d.EggProdIsSeasonal = EggProdIsSeasonal.Clone
            d.SpeciesCode = SpeciesCode.Clone

            CurrentStanza = d.CurrentStanza
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

End Class


