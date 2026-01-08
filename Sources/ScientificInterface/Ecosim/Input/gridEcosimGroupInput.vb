' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class gridEcosimGroupInput
        Inherits cEwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            MaxRelPB
            MaxRelFeedingTime
            FeedingTimeAdjustRate
            OtherMortFeedingTime
            PredatorFeedingTime
            DenDepCatchability
            QBMaxQBO
            SwitchPower
            AddPredMortProp ' For now, new parameter is added to the end of the list
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.MaxRelPB) = New cEwEColumnHeaderCell(SharedResources.HEADER_MAXRELPB)
            Me(0, eColumnTypes.MaxRelFeedingTime) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_MAXRELFEEDINGTIME)
            Me(0, eColumnTypes.FeedingTimeAdjustRate) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_FEEDINGTIMEADJUSTRATE)
            Me(0, eColumnTypes.OtherMortFeedingTime) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_OTHERMORTFEEDINGTIME)
            Me(0, eColumnTypes.PredatorFeedingTime) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_PREDATORFEEDINGTIME)
            Me(0, eColumnTypes.DenDepCatchability) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_DENDEPCATCHABILITY)
            Me(0, eColumnTypes.QBMaxQBO) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_QBMAXQBO)
            Me(0, eColumnTypes.SwitchPower) = New cEwEColumnHeaderCell(SharedResources.HEADER_SWITCHINGPOWER_VALRANGE)
            Me(0, eColumnTypes.AddPredMortProp) = New cEwEColumnHeaderCell(My.Resources.ECOSIM_GROUPINFO_PROPADDITIVEMORT)



            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = Me.UIContext.Core
            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim iStanzaGroup(core.nLivingGroups) As Integer 'Hold the stanza group index
            Dim hgcStanza As cEwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, cEwEHierarchyGridCell)

            For i As Integer = 1 To core.nLivingGroups : iStanzaGroup(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.nLifeStages
                    source = core.EcopathGroupInputs(sg.iGroups(iStanza))
                    iStanzaGroup(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nLivingGroups
                source = core.EcosimGroupInputs(groupIndex)

                If iStanzaGroup(source.Index) = -1 Then
                    iRow = Me.AddRow
                    Me.FillInRows(iRow, source)
                Else                'If group is a stanza group

                    sg = core.StanzaGroups(iStanzaGroup(source.Index))
                    If (Not dtStanzaCells.ContainsKey(sg)) Then
                        iRow = Me.AddRow()
                        hgcStanza = New cEwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)
                        Me(iRow, eColumnTypes.DenDepCatchability) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelFeedingTime) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.MaxRelPB) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.OtherMortFeedingTime) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.PredatorFeedingTime) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.QBMaxQBO) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.SwitchPower) = New cEwERowHeaderCell()
                        Me(iRow, eColumnTypes.AddPredMortProp) = New cEwERowHeaderCell()

                        iRow = Me.AddRow
                    Else
                        hgcStanza = dtStanzaCells(sg)
                        iRow = Me.AddRow(hgcStanza.Row + hgcStanza.NumChildRows + 1)
                    End If
                    'Display group info
                    hgcStanza.AddChildRow(iRow)
                    Me.FillInRows(iRow, source, True)
                End If
            Next groupIndex

        End Sub

        Private Sub FillInRows(iRow As Integer, source As cCoreInputOutputBase, Optional isIndented As Boolean = False)
            Dim cell As cEwECellBase = Nothing
            Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            cell = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.MaxRelPB)
            cell.SuppressZero = True
            Me(iRow, eColumnTypes.MaxRelPB) = cell
            Me(iRow, eColumnTypes.MaxRelFeedingTime) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.MaxRelFeedingTime)
            Me(iRow, eColumnTypes.FeedingTimeAdjustRate) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FeedingTimeAdjRate)
            Me(iRow, eColumnTypes.OtherMortFeedingTime) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.OtherMortFeedingTime)
            Me(iRow, eColumnTypes.PredatorFeedingTime) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.PredEffectFeedingTime)
            Me(iRow, eColumnTypes.DenDepCatchability) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.DenDepCatchability)
            Me(iRow, eColumnTypes.QBMaxQBO) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.QBMaxQBio)
            Me(iRow, eColumnTypes.SwitchPower) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.SwitchingPower)
            Me(iRow, eColumnTypes.AddPredMortProp) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.AdditivePredMortProp)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
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
            Me.Columns(eColumnTypes.QBMaxQBO).Width = 78
            Me.Columns(eColumnTypes.SwitchPower).Width = 78

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next
        End Sub


    End Class

End Namespace
