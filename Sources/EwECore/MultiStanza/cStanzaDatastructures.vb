'==============================================================================
'
' $Log: cStanzaDatastructures.vb,v $
' Revision 1.1  2008/09/26 07:30:28  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2007/08/08 21:08:45  willw
' added copyTo function
'
' Revision 1.8  2007/07/02 17:52:31  joeb
' Changed EggProdShapeSplit() to Integer
'
' Revision 1.7  2007/06/05 21:15:28  willw
' added a number of packets multiplier
'
' Revision 1.6  2007/06/04 01:20:20  jeroens
' * Moved a couple of present variable comments into XML tags to have them show up in the dev environment
'
' Revision 1.5  2007/05/31 19:56:40  willw
' lots of changes for IBM, fixed multithreading for it
'
' Revision 1.4  2007/05/28 23:21:14  willw
' added stuff for IBM approach
'
' Revision 1.3  2007/05/18 20:37:39  willw
' corrected a comment about WmatWinf
'
' Revision 1.2  2007/03/27 16:20:56  jeroens
' * Age1, Age2 converted from Single to Integer
'
' Revision 1.1  2007/03/26 02:12:46  jeroens
' Moved
'
' Revision 1.14  2006/12/15 01:44:06  joeb
' EggProdShape
'
' Revision 1.13  2006/12/13 21:07:31  joeb
' Added EggProdIsSeasonal
'
' Revision 1.12  2006/11/30 19:54:17  joeb
' Added comments
'
' Revision 1.11  2006/11/21 16:27:18  joeb
' Changes for Ecospace
'
' Revision 1.10  2006/11/05 15:38:59  jeroens
' * BaseStanza, BaseStanzaCB integer
'
' Revision 1.9  2006/10/27 21:09:09  joeb
' Comments
'
' Revision 1.8  2006/10/25 15:36:56  joeb
' Changed FixedFuncundity() to Boolean
'
' Revision 1.7  2006/10/11 17:00:58  jeroens
' * Merged different sources of vbK
'
' Revision 1.6  2006/09/24 01:23:11  jeroens
' * vbK now stored in Stanza ds
'
' Revision 1.5  2006/09/08 21:24:09  joeb
' comments
'
' Revision 1.4  2006/07/04 04:30:52  jeroens
' * Changed EcopathCode(,) from Single to Integer
'
'==============================================================================

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

    'ToDo_jb stanza data SpeciesCode() is dimmed by nGroups I need to sort out where this should get set
    'Public SpeciesCode(,) As Integer 'species code number for each ecopath group (0 if not a stanza in a split species)
    ' Public vBMann() As Single
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


