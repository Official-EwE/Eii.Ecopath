#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Other
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid displaying Ecopath Basic Input information.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridInputTaxa
        : Inherits EwEGrid

        Enum eColumnTypes As Integer
            Index = 0
            Name
            Ecology
            Organism
            Exploited
            Conservation
            Occurrence
            MeanLen
            MaxLen
            MeanWeight
            MeanLifeSpan
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell("Common name")
            Me(0, eColumnTypes.Ecology) = New EwEColumnHeaderCell("Ecology")
            Me(0, eColumnTypes.Organism) = New EwEColumnHeaderCell("Organism")
            Me(0, eColumnTypes.Exploited) = New EwEColumnHeaderCell("Exploited")
            Me(0, eColumnTypes.Conservation) = New EwEColumnHeaderCell("Conservation status")
            Me(0, eColumnTypes.Occurrence) = New EwEColumnHeaderCell("Occurrence status")
            Me(0, eColumnTypes.MeanLen) = New EwEColumnHeaderCell("Mean length")
            Me(0, eColumnTypes.MaxLen) = New EwEColumnHeaderCell("Max length")
            Me(0, eColumnTypes.MeanWeight) = New EwEColumnHeaderCell("Mean weight")
            Me(0, eColumnTypes.MeanLifeSpan) = New EwEColumnHeaderCell("Mean life span")

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cCoreInputOutputBase = Nothing
            Dim taxon As cTaxon = Nothing
            Dim cell As EwECellBase = Nothing
            Dim cellParent As EwEHierarchyGridCell = Nothing
            Dim iRow As Integer = -1

            ' Sort taxa by group
            Dim aiGroupTaxa(Me.Core.nGroups) As List(Of cTaxon)
            For iGroup As Integer = 0 To Me.Core.nGroups
                aiGroupTaxa(iGroup) = New List(Of cTaxon)
            Next
            For iTaxon As Integer = 1 To Me.Core.nTaxon
                taxon = Me.Core.Taxon(iTaxon)
                aiGroupTaxa(taxon.Group).Add(taxon)
            Next

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For iGroup As Integer = 1 To Core.nGroups

                group = Core.EcoPathGroupInputs(iGroup)

                ' Add group
                iRow = Me.AddRow()
                iRow = Me.AddRow()
                For i As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next

                cellParent = New EwEHierarchyGridCell()
                Me(iRow, eColumnTypes.Index) = cellParent
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, group, eVarNameFlags.Name)

                For Each t As cTaxon In aiGroupTaxa(iGroup)

                    ' Add taxon
                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, group, eVarNameFlags.CommonName)

                    cell = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.BiomassAreaInput)
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.Ecology) = cell

                    cell = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.PBInput)
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.Organism) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.Exploited) = cell

                    cell = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.QBInput)
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.Conservation) = cell

                    Me(iRow, eColumnTypes.Occurrence) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.EEInput)
                    Me(iRow, eColumnTypes.MeanLen) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.OtherMortInput)
                    Me(iRow, eColumnTypes.MeanWeight) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.GEInput)
                    Me(iRow, eColumnTypes.MeanWeight).Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.MaxLen) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.GS)
                    Me(iRow, eColumnTypes.MeanLifeSpan) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.DetImp)

                    cellParent.AddChildRow(iRow)
                Next
            Next iGroup

        End Sub

        Protected Overrides Sub OnCellDoubleClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)
            Dim dlg As EditMultiStanza = Nothing
            Dim prop As cProperty = Nothing
            Dim group As cEcoPathGroupInput = Nothing

            If Not TypeOf cell Is PropertyCell Then Return
            prop = DirectCast(cell, PropertyCell).GetProperty()
            group = DirectCast(prop.Source, cEcoPathGroupInput)

            dlg = New EditMultiStanza(Me.UIContext, group)
            dlg.ShowDialog(Me)
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Dim ci As ColumnInfo = Me.Columns(eColumnTypes.Organism)

            Me.Rows(0).Height = 60
            Me.Columns(0).Width = 24
            Me.Columns(1).Width = 120
            Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

            If Me.UIContext Is Nothing Then Return

            ci.Visible = (Me.Core.nStanzas > 0)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
