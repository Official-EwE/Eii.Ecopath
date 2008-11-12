'==============================================================================
'
' $Log: GroupInfoEwEGrid.vb,v $
' Revision 1.2  2008/11/12 21:35:31  jeroens
' Resources!
'
' Revision 1.1  2008/09/26 07:31:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.20  2008/09/25 02:31:48  jeroens
' Moved max fishing mortaility from search datastructures to Ecosim
'
' Revision 1.19  2008/08/02 03:04:15  jeroens
' Renamed resources
'
' Revision 1.18  2008/06/02 00:01:34  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.17  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.16  2008/04/07 02:31:14  jeroens
' Cleaning up resources
'
' Revision 1.15  2008/02/28 20:32:54  joeb
' Added Left and Right Salinity
'
' Revision 1.14  2008/01/11 12:33:20  jeroens
' Fixed bug 299
'
' Revision 1.13  2007/12/04 02:23:54  jeroens
' * Columns indicated via Enum
' + Added salinity vars
'
' Revision 1.12  2007/10/10 02:59:15  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.11  2007/07/06 20:11:19  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.10  2007/06/22 18:49:17  fgao
' Finish making up this grid.. better looking now??
' Indent multi stanza group display..
'
' Revision 1.9  2007/06/21 22:23:38  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.8  2007/06/13 22:36:08  fgao
' Fixed Bug 67: Relating to Grid cell alignment.
'
' Revision 1.7  2007/04/29 03:45:13  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports EwEUtils.Core

#End Region ' Imports directive

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
            FLimit
            DenDepCatchability
            QBMaxQBO
            SwitchPower
            SalinityOpt
            SalinitySpreadLeft
            SalinitySpreadRight
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
            Me(0, eColumnTypes.FLimit) = New EwEColumnHeaderCell(My.Resources.GENERIC_LABEL_MAXFISHINGMORTAILITY)
            Me(0, eColumnTypes.QBMaxQBO) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_QBMAXQBO)
            Me(0, eColumnTypes.SwitchPower) = New EwEColumnHeaderCell(My.Resources.HEADER_SWITCHINGPOWER_VALRANGE)
            Me(0, eColumnTypes.SalinityOpt) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_OPTSALINITY)
            Me(0, eColumnTypes.SalinitySpreadLeft) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_SALSPREADLEFT)
            Me(0, eColumnTypes.SalinitySpreadRight) = New EwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_SALSPREADRIGHT)

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
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        Me(iRow, eColumnTypes.DenDepCatchability) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelFeedingTime) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelPB) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinityOpt) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.OtherMortFeedingTime) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.PredatorFeedingTime) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.FLimit) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.QBMaxQBO) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinitySpreadLeft) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SalinitySpreadRight) = New EwERowHeaderCell()
                        Me(iRow, eColumnTypes.SwitchPower) = New EwERowHeaderCell()
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
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If

            cell = New PropertyCell(source, eVarNameFlags.MaxRelPB)
            cell.SuppressZero = True
            Me(iRow, eColumnTypes.MaxRelPB) = cell
            Me(iRow, eColumnTypes.MaxRelFeedingTime) = New PropertyCell(source, eVarNameFlags.MaxRelFeedingTime)
            Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New PropertyCell(source, eVarNameFlags.FeedingTimeAdjRate)
            Me(iRow, eColumnTypes.OtherMortFeedingTime) = New PropertyCell(source, eVarNameFlags.OtherMortFeedingTime)
            Me(iRow, eColumnTypes.PredatorFeedingTime) = New PropertyCell(source, eVarNameFlags.PredEffectFeedingTime)
            Me(iRow, eColumnTypes.DenDepCatchability) = New PropertyCell(source, eVarNameFlags.DenDepCatchability)
            Me(iRow, eColumnTypes.FLimit) = New PropertyCell(source, eVarNameFlags.EcosimGroupMaxMort)
            Me(iRow, eColumnTypes.QBMaxQBO) = New PropertyCell(source, eVarNameFlags.QBMaxQBio)
            Me(iRow, eColumnTypes.SwitchPower) = New PropertyCell(source, eVarNameFlags.SwitchingPower)
            Me(iRow, eColumnTypes.SalinityOpt) = New PropertyCell(source, eVarNameFlags.SalinityOpt)
            Me(iRow, eColumnTypes.SalinitySpreadLeft) = New PropertyCell(source, eVarNameFlags.SalinitySpreadLeft)
            Me(iRow, eColumnTypes.SalinitySpreadRight) = New PropertyCell(source, eVarNameFlags.SalinitySpreadRight)
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
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
            Me.Columns(eColumnTypes.FLimit).Width = 78
            Me.Columns(eColumnTypes.QBMaxQBO).Width = 78
            Me.Columns(eColumnTypes.SwitchPower).Width = 78
            Me.Columns(eColumnTypes.SalinityOpt).Width = 78
            Me.Columns(eColumnTypes.SalinitySpreadLeft).Width = 78
            Me.Columns(eColumnTypes.SalinitySpreadRight).Width = 78

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next
        End Sub


    End Class

End Namespace
