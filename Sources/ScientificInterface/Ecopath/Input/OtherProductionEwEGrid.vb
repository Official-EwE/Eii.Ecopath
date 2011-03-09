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
    ''' Grid accepting Ecopath Other Production user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
     Public Class OtherProductionEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim aUnitType As cStyleGuide.eUnitType() = {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

            Me.Redim(1, 7)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_IMMIGRATION_UNIT, aUnitType)
            Me(0, 3) = New EwEColumnHeaderCell(SharedResources.HEADER_EMIGRATION_UNIT, aUnitType)
            Me(0, 4) = New EwEColumnHeaderCell(SharedResources.HEADER_EMIGRATIONRATE_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, 5) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_UNIT, aUnitType)
            Me(0, 6) = New EwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_RATE_ABBR_UNIT, cStyleGuide.eUnitType.Time)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim blnStanza(Core.nLivingGroups) As Boolean
            Dim intStanza(Core.nLivingGroups) As Integer 'Hold the stanza group number
            Dim intStanzaPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Core.nLivingGroups : intStanza(i) = -1 : Next

            'Remove existing rows
            Me.RowsCount = 1

            'Tag stanza group first
            For stanzaGroupIndex As Integer = 0 To Core.nStanzas - 1
                sg = Core.StanzaGroups(stanzaGroupIndex)
                For stanzaIndex As Integer = 1 To sg.NStanzas
                    source = Core.EcoPathGroupInputs(sg.iGroups(stanzaIndex))
                    blnStanza(source.Index) = True
                    intStanza(source.Index) = stanzaGroupIndex
                Next
            Next

            'Create rows for all groups
            For groupIndex As Integer = 1 To Core.nLivingGroups
                source = Core.EcoPathGroupInputs(groupIndex)

                If intStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Immig)
                    Me(iRow, 3) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Emig)
                    Me(iRow, 4) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EmigRate)
                    Me(iRow, 5) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccum)
                    Me(iRow, 6) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRate)
                Else 'Group is stanza
                    sg = Core.StanzaGroups(intStanza(source.Index))
                    If intStanza(source.Index) <> intStanzaPrev Then 'If stanza group appears the first time Then display + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)
                        'Complete row with dummy cells
                        For i As Integer = 2 To 6 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaPrev = intStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Immig)
                    Me(iRow, 3) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Emig)
                    Me(iRow, 4) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EmigRate)
                    Me(iRow, 5) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccum)
                    Me(iRow, 6) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BioAccumRate)
                End If
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
