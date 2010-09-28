#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class MortalityCoefficientsEwEGrid
        : Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            PBZ
            FishMort
            PredMort
            BioAccum
            NetMig
            OtherMort
            MortTot
            MortNat
        End Enum

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_PBZ)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_FISHINGMORTRATE)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_PREDMORTRATE)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMACCURATE2)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_NETMIGRATE)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_OTHERMORTRATE)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.MORT_FISH_TOT)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.MORT_NAT)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nLivingGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nLivingGroups : intStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(source.Index) = stanzaGroupIndex
                Next
            Next
            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupOutputs(groupIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, DirectCast(source, cEcoPathGroupOutput))
                Else 'Group is stanza
                    sg = Core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)

                        iRow = Me.AddRow()
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 9 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, DirectCast(source, cEcoPathGroupOutput), True)

                    hgcStanza.AddChildRow(iRow)

                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cEcoPathGroupOutput, Optional ByVal isIndented As Boolean = False)

            Dim cell As PropertyCell = Nothing
            Dim bMortAlert As Boolean = (source.MortCoOtherMort < 0)
            Dim bCatchAlert As Boolean = (source.MortCoFishRate > source.PBOutput)

            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If
            Me(iRow, eColumnTypes.PBZ) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PBOutput)
            Me(iRow, eColumnTypes.FishMort) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MortCoFishRate)
            Me(iRow, eColumnTypes.PredMort) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MortCoPredMort)
            Me(iRow, eColumnTypes.BioAccum) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRatePerYear)
            Me(iRow, eColumnTypes.NetMig) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MortCoNetMig)
            Me(iRow, eColumnTypes.OtherMort) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MortCoOtherMort)
            Me(iRow, eColumnTypes.MortTot) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FishMortTotMort)
            Me(iRow, eColumnTypes.MortNat) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.NatMortPerTotMort)

            Me.SetCellAlert(DirectCast(Me(iRow, eColumnTypes.Name), EwECellBase), bMortAlert)
            Me.SetCellAlert(DirectCast(Me(iRow, eColumnTypes.FishMort), EwECellBase), bMortAlert And bCatchAlert)
            Me.SetCellAlert(DirectCast(Me(iRow, eColumnTypes.OtherMort), EwECellBase), bMortAlert)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

        Private Sub SetCellAlert(ByVal cell As EwECellBase, ByVal bSetAlert As Boolean)
            If bSetAlert Then
                cell.Style = cell.Style Or cStyleGuide.eStyleFlags.Checked
            Else
                cell.Style = cell.Style And (Not cStyleGuide.eStyleFlags.Checked)
            End If
        End Sub

    End Class

End Namespace
