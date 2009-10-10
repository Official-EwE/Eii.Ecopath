#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class GrowthEstimatesEwEGrid
        : Inherits EwEGrid

        Private m_core As cCore = Nothing
        Private m_frm As Form = Nothing

        Public Sub New()
            MyBase.new()

            m_core = cCore.GetInstance

            'Don't manually run! The core execution states take care of this!
            'm_core.RunPSD(IsGroupSelected)
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

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(m_core.nLivingGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To m_core.nLivingGroups : intStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To m_core.nStanzas - 1
                sg = m_core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = m_core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For groupIndex As Integer = 1 To m_core.nLivingGroups
                source = m_core.EcoPathGroupOutputs(groupIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else 'Group is stanza
                    sg = m_core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 9 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, source, True)
                End If
            Next groupIndex

          End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)

            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If

            Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.AinLWOutput)
            Me(iRow, 3) = New PropertyCell(source, eVarNameFlags.BinLWOutput)
            Me(iRow, 4) = New PropertyCell(source, eVarNameFlags.LooOutput)
            Me(iRow, 5) = New PropertyCell(source, eVarNameFlags.WinfOutput)
            Me(iRow, 6) = New PropertyCell(source, eVarNameFlags.VBK)
            Me(iRow, 7) = New PropertyCell(source, eVarNameFlags.t0Output)
            Me(iRow, 8) = New PropertyCell(source, eVarNameFlags.TCatchOutput)
            Me(iRow, 9) = New PropertyCell(source, eVarNameFlags.TmaxOutput)

        End Sub

        Protected Overrides Sub FinishStyle()

            MyBase.FinishStyle()

            Me.Rows(0).Height = 60
            'Me.Columns(0).Width = 24
            'Me.Columns(1).Width = 120
            'Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            'Me.Columns(2).Width = 52
            'Me.Columns(3).Width = 53
            'Me.Columns(4).Width = 67
            'Me.Columns(5).Width = 58
            'Me.Columns(6).Width = 66
            'Me.Columns(7).Width = 82
            'Me.Columns(8).Width = 69
            'Me.Columns(9).Width = 76

            For iCol As Integer = 2 To Me.ColumnsCount - 1
                Me(0, iCol).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

        End Sub

        Private Function IsGroupSelected() As Boolean()
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim bGroupSelected(m_core.nLivingGroups) As Boolean

            For i As Integer = 1 To m_core.nLivingGroups
                bGroupSelected(i) = sg.GroupVisible(i)
            Next
            Return bGroupSelected
        End Function

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
