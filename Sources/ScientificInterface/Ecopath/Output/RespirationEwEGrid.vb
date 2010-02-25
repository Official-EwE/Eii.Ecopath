#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class RespirationEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim aUnitType As cStyleGuide.eUnitType() = {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

            Me.Redim(1, 7)

            ' Define column Headers
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_RESPIRATION_UNIT, aUnitType)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_ASSIMILATION_UNIT, aUnitType)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_RESPIRATIONASSIMILATION)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_PRODRESP)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_RESPBIOM_UNIT, cStyleGuide.eUnitType.Time)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nGroups : intStanzaGroupIndex(i) = -1 : Next

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
            For groupIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupOutputs(groupIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)

                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 6 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If

                    'Display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source, True)

                    hgcStanza.AddChildRow(iRow)
                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)
            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If
            Me(iRow, 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Respiration)
            Me(iRow, 3) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Assimilation)
            Me(iRow, 4) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RespAssim)
            Me(iRow, 5) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.ProdResp)
            Me(iRow, 6) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RespBiom)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

