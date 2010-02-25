#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Particle Size Distribution Growth user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
      Public Class GrowthParametersEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 10) '9)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_A_IN_LW)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_B_IN_LW)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_LENGTH_INFINITY_UNIT) ', StyleGuide.eUnitType.None)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_WEIGHT_INFINITY_UNIT) ', StyleGuide.eUnitType.Monetary)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_K_VBGF_UNIT) ', StyleGuide.eUnitType.None)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_TZERO_VBGF_UNIT) ', StyleGuide.eUnitType.Time)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_AGE_FIRST_CAPTURE_UNIT) ', StyleGuide.eUnitType.Currency)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXAGE_UNIT) ', StyleGuide.eUnitType.Currency)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cEcoPathGroupInput = Nothing
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
                    group = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(group.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For rowIndex As Integer = 1 To core.nLivingGroups
                group = core.EcoPathGroupInputs(rowIndex)
                ' Is group stanza?
                If intStanzaGroupIndex(group.Index) = -1 Then
                    ' #No: display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, group)
                Else
                    '#Yes: Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(group.Index))
                    If intStanzaGroupIndex(group.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 9 : Me(iRow, i) = New EwERowHeaderCell() : Next
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

        Private Sub FillInRows(ByVal iRow As Integer, ByVal group As cEcoPathGroupInput, Optional ByVal isIndented As Boolean = False)

            ' Get the group name from EcopathInput
            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)

            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            End If

            Me(iRow, 2) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.AinLWInput)
            Me(iRow, 3) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.BinLWInput)
            Me(iRow, 4) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.LooInput)
            Me(iRow, 5) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.WinfInput)
            Me(iRow, 6) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.VBK)
            Me(iRow, 7) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.t0Input)
            Me(iRow, 8) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.TCatchInput)
            Me(iRow, 9) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.TmaxInput)

        End Sub

        Protected Overrides Sub FinishStyle()

            MyBase.FinishStyle()

            For iCol As Integer = 2 To Me.ColumnsCount - 1
                Me(0, iCol).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
