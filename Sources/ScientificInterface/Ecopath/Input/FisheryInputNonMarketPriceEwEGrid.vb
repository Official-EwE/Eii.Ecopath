#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Non-Market price user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
       Public Class FisheryInputNonMarketPriceEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUE_UNIT_PER_UNIT, _
                                               New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Monetary, cStyleGuide.eUnitType.Biomass})

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cEcoPathGroupInput = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(Core.nGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Core.nGroups : intStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To Core.nStanzas - 1
                sg = Core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    group = Core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(group.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For rowIndex As Integer = 1 To Core.nGroups
                group = Core.EcoPathGroupInputs(rowIndex)
                ' Is group stanza?
                If intStanzaGroupIndex(group.Index) = -1 Then
                    ' #No: display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, group)
                Else
                    '#Yes: Group is stanza
                    sg = Core.StanzaGroups(intStanzaGroupIndex(group.Index))
                    If intStanzaGroupIndex(group.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 2 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(group.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, group, True)
                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, _
                               ByVal group As cEcoPathGroupInput, _
                               Optional ByVal isIndented As Boolean = False)

            ' Get the group name from EcopathInput
            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)

            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            End If

            Me(iRow, 2) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.NonMarketValue)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
