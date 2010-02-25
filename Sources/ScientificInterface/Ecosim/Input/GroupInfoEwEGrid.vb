#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class GroupInfoEwEGrid
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            MaxRelPB
            MaxRelFeedingTime
            FeedingTimeAdjustRate
            OtherMortFeedingTime
            PredatorFeedingTime
            'FLimit
            DenDepCatchability
            QBMaxQBO
            SwitchPower
            SalinityOpt
            SalinitySpreadLeft
            SalinitySpreadRight
            TempOpt
            TempSpreadLeft
            TempSpreadRight
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.MaxRelPB) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXRELPB)
            Me(0, eColumnTypes.MaxRelFeedingTime) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_MAXRELFEEDINGTIME)
            Me(0, eColumnTypes.FeedingTimeAdjustRate) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_FEEDINGTIMEADJUSTRATE)
            Me(0, eColumnTypes.OtherMortFeedingTime) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_OTHERMORTFEEDINGTIME)
            Me(0, eColumnTypes.PredatorFeedingTime) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_PREDATORFEEDINGTIME)
            Me(0, eColumnTypes.DenDepCatchability) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_DENDEPCATCHABILITY)
            'Me(0, eColumnTypes.FLimit) = New EwEColumnHeaderCell(My.Resources.GENERIC_LABEL_MAXFISHINGMORTAILITY)
            Me(0, eColumnTypes.QBMaxQBO) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_QBMAXQBO)
            Me(0, eColumnTypes.SwitchPower) = New EwEColumnHeaderCell(My.Resources.HEADER_SWITCHINGPOWER_VALRANGE)
            Me(0, eColumnTypes.SalinityOpt) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_OPTSALINITY)
            Me(0, eColumnTypes.SalinitySpreadLeft) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_SALSPREADLEFT)
            Me(0, eColumnTypes.SalinitySpreadRight) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_SALSPREADRIGHT)

            Me(0, eColumnTypes.TempOpt) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_OPTTEMP)
            Me(0, eColumnTypes.TempSpreadLeft) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_TEMPSPREADLEFT)
            Me(0, eColumnTypes.TempSpreadRight) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_TEMPSPREADRIGHT)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim iStanzaGroup(core.nLivingGroups) As Integer 'Hold the stanza group index
            Dim iStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nLivingGroups : iStanzaGroup(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    iStanzaGroup(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoSimGroupInputs(groupIndex)

                If iStanzaGroup(source.Index) = -1 Then
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else                'If group is a stanza group

                    sg = core.StanzaGroups(iStanzaGroup(source.Index))
                    If iStanzaGroup(source.Index) <> iStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control

                        iRow = Me.AddRow()
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)
                        Me(iRow, eColumnTypes.DenDepCatchability) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelFeedingTime) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelPB) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinityOpt) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.OtherMortFeedingTime) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.PredatorFeedingTime) = New EwERowHeaderCell()
                        'Me(iRow, eColumnTypes.FLimit) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.QBMaxQBO) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinitySpreadLeft) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinitySpreadRight) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SwitchPower) = New EwERowHeaderCell()

                        Me(iRow, eColumnTypes.TempOpt) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.TempSpreadLeft) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.TempSpreadRight) = New EwERowHeaderCell()

                        iStanzaGroupIndexPrev = iStanzaGroup(source.Index)
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
            Dim cell As EwECellBase = Nothing
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MaxRelPB)
            cell.SuppressZero = True
            Me(iRow, eColumnTypes.MaxRelPB) = cell
            Me(iRow, eColumnTypes.MaxRelFeedingTime) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MaxRelFeedingTime)
            Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.FeedingTimeAdjRate)
            Me(iRow, eColumnTypes.OtherMortFeedingTime) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.OtherMortFeedingTime)
            Me(iRow, eColumnTypes.PredatorFeedingTime) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PredEffectFeedingTime)
            Me(iRow, eColumnTypes.DenDepCatchability) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.DenDepCatchability)
            'Me(iRow, eColumnTypes.FLimit) = New PropertyCell(me.PropertyManager, source, eVarNameFlags.EcosimGroupMaxMort)
            Me(iRow, eColumnTypes.QBMaxQBO) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.QBMaxQBio)
            Me(iRow, eColumnTypes.SwitchPower) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SwitchingPower)
            Me(iRow, eColumnTypes.SalinityOpt) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SalinityOpt)
            Me(iRow, eColumnTypes.SalinitySpreadLeft) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SalinitySpreadLeft)
            Me(iRow, eColumnTypes.SalinitySpreadRight) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SalinitySpreadRight)

            Me(iRow, eColumnTypes.TempOpt) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.TemperatureOpt)
            Me(iRow, eColumnTypes.TempSpreadLeft) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.TemperatureSpreadLeft)
            Me(iRow, eColumnTypes.TempSpreadRight) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.TemperatureSpreadRight)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Me.Rows(eColumnTypes.Index).Height = 84
            Me.Columns(eColumnTypes.Index).Width = 24
            Me.Columns(eColumnTypes.Name).Width = 120
            Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.MaxRelPB).Width = 78
            Me.Columns(eColumnTypes.MaxRelFeedingTime).Width = 78
            Me.Columns(eColumnTypes.FeedingTimeAdjustRate).Width = 78
            Me.Columns(eColumnTypes.OtherMortFeedingTime).Width = 78
            Me.Columns(eColumnTypes.PredatorFeedingTime).Width = 78
            Me.Columns(eColumnTypes.DenDepCatchability).Width = 78
            'Me.Columns(eColumnTypes.FLimit).Width = 78
            Me.Columns(eColumnTypes.QBMaxQBO).Width = 78
            Me.Columns(eColumnTypes.SwitchPower).Width = 78
            Me.Columns(eColumnTypes.SalinityOpt).Width = 78
            Me.Columns(eColumnTypes.SalinitySpreadLeft).Width = 78
            Me.Columns(eColumnTypes.SalinitySpreadRight).Width = 78

            Me.Columns(eColumnTypes.TempOpt).Width = 78
            Me.Columns(eColumnTypes.TempSpreadLeft).Width = 78
            Me.Columns(eColumnTypes.TempSpreadRight).Width = 78

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next
        End Sub


    End Class

End Namespace
