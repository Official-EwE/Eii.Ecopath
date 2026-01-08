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
Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.Style
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

<CLSCompliant(False)>
Public Class gridEditMultiStanza
    Inherits cEwEGrid

#Region " Private variables "

    Private m_stanzagroup As cStanzaGroup = Nothing

    Private Enum eColumnTypes
        Index = 0
        Name
        StartAge
        LeadingB
        Biomass
        Z
        LeadingCB
        CBInput
        SpawnProp
    End Enum

#End Region ' Private variables

#Region " Constructor "

    Public Sub New()
        ' NOP
    End Sub

#End Region ' Constructor

#Region " Properties "

    Public Property StanzaGroup() As cStanzaGroup
        Get
            Return Me.m_stanzagroup
        End Get
        Set(value As cStanzaGroup)
            Me.m_stanzagroup = value
            Me.RefreshContent()
        End Set
    End Property

#End Region ' Properties

#Region " Internals "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Public Sub CalculateStanzaParameters()
        ' Sanity check
        If (Me.m_stanzagroup Is Nothing) Then Return
        Me.m_stanzagroup.CalculateParameters()
    End Sub

    Protected Overrides Sub InitStyle()

        If (Me.UIContext Is Nothing) Then Return

        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.StartAge) = New cEwEColumnHeaderCell(SharedResources.HEADER_STARTAGE)
        Me(0, eColumnTypes.LeadingB) = New cEwEColumnHeaderCell(SharedResources.HEADER_LEADING_BIOMASS)
        Me(0, eColumnTypes.Biomass) = New cEwEColumnHeaderCell(eVarNameFlags.Biomass)
        Me(0, eColumnTypes.Z) = New cEwEColumnHeaderCell(eVarNameFlags.Z, eDescriptorTypes.Abbreviation)
        Me(0, eColumnTypes.LeadingCB) = New cEwEColumnHeaderCell(SharedResources.HEADER_LEADING_CB)
        Me(0, eColumnTypes.CBInput) = New cEwEColumnHeaderCell(eVarNameFlags.QBInput)
        Me(0, eColumnTypes.SpawnProp) = New cEwEColumnHeaderCell(eVarNameFlags.SpawnProp)

        Me.FixedColumnWidths = True

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return

        Dim source As cEcoPathGroupInput = Nothing
        Dim ewec As cEwECell = Nothing
        Dim iRow As Integer
        Dim bIsEcosimLoaded As Boolean = (Me.Core.ActiveEcosimScenarioIndex > -1)

        ' Remove existing rows
        Me.RowsCount = 1

        If (Me.m_stanzagroup Is Nothing) Then Return

        For iLifeStage As Integer = 1 To Me.m_stanzagroup.nLifeStages

            source = Me.Core.EcopathGroupInputs(Me.m_stanzagroup.iGroups(iLifeStage))
            iRow = Me.AddRow

            'Index
            Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)

            'Name
            Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

            'Start age
            ewec = New cEwECell(0, GetType(Integer))
            ewec.Value = Me.m_stanzagroup.GetVariable(eVarNameFlags.StartAge, iLifeStage)
            ' First group start age cannot be edited
            If (iLifeStage = 1) Then ewec.Style = cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.StartAge) = ewec
            Me(iRow, eColumnTypes.StartAge).Behaviors.Add(Me.EwEEditHandler)

            ' LeadingB
            Me(iRow, eColumnTypes.LeadingB) = New Cells.Real.CheckBox(Me.m_stanzagroup.LeadingB = iLifeStage)
            Me(iRow, eColumnTypes.LeadingB).Behaviors.Add(Me.EwEEditHandler)

            'Biomass
            ewec = New cEwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Biomass(iLifeStage)
            Me(iRow, eColumnTypes.Biomass) = ewec
            Me(iRow, eColumnTypes.Biomass).Behaviors.Add(Me.EwEEditHandler)

            'Total Mortality
            ewec = New cEwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Mortality(iLifeStage)
            Me(iRow, eColumnTypes.Z) = ewec
            Me(iRow, eColumnTypes.Z).Behaviors.Add(Me.EwEEditHandler)

            ' LeadingCB
            Me(iRow, eColumnTypes.LeadingCB) = New Cells.Real.CheckBox(Me.m_stanzagroup.LeadingCB = iLifeStage)
            Me(iRow, eColumnTypes.LeadingCB).Behaviors.Add(Me.EwEEditHandler)

            'Consumption/Biomass
            ewec = New cEwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.CB(iLifeStage)
            Me(iRow, eColumnTypes.CBInput) = ewec
            Me(iRow, eColumnTypes.CBInput).Behaviors.Add(Me.EwEEditHandler)

            ewec = New cEwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.SpawnProp(iLifeStage)
            Me(iRow, eColumnTypes.SpawnProp) = ewec
            Me(iRow, eColumnTypes.SpawnProp).Behaviors.Add(Me.EwEEditHandler)

        Next

        Me.SetLeadingGroup(Me.m_stanzagroup.LeadingB, eColumnTypes.LeadingB)
        Me.SetLeadingGroup(Me.m_stanzagroup.LeadingCB, eColumnTypes.LeadingCB)

    End Sub

    Public Sub SetStanzaGroupValues(bApplyToCore As Boolean)

        Dim iLeadingB As Integer = Me.m_stanzagroup.LeadingB
        Dim iLeadingCB As Integer = Me.m_stanzagroup.LeadingCB
        For iLifeStage As Integer = 1 To Me.m_stanzagroup.nLifeStages
            If CBool(Me(iLifeStage, eColumnTypes.LeadingB).Value) Then
                iLeadingB = iLifeStage
            End If
            If CBool(Me(iLifeStage, eColumnTypes.LeadingCB).Value) Then
                iLeadingCB = iLifeStage
            End If
        Next
        Me.m_stanzagroup.LeadingB = iLeadingB
        Me.m_stanzagroup.LeadingCB = iLeadingCB

        For iLifeStage As Integer = 1 To Me.m_stanzagroup.nLifeStages

            'Start age
            Me.m_stanzagroup.SetVariable(eVarNameFlags.StartAge, CInt(Me(iLifeStage, eColumnTypes.StartAge).Value), iLifeStage)
            'Biomass
            Me.m_stanzagroup.Biomass(iLifeStage) = CSng(Me(iLifeStage, eColumnTypes.Biomass).Value)
            'Total Mortality
            Me.m_stanzagroup.Mortality(iLifeStage) = CSng(Me(iLifeStage, eColumnTypes.Z).Value)
            'Consumption/Biomass
            Me.m_stanzagroup.CB(iLifeStage) = CSng(Me(iLifeStage, eColumnTypes.CBInput).Value)
            'Spawn prop
            Me.m_stanzagroup.SpawnProp(iLifeStage) = CSng(Me(iLifeStage, eColumnTypes.SpawnProp).Value)

        Next

        If bApplyToCore Then
            ' JS 090816: apply changes for all stanza groups, not only the last used stanza group
            For iIndex As Integer = 0 To Me.Core.nStanzas - 1
                Me.Core.StanzaGroups(iIndex).Apply()
            Next
        End If

    End Sub

    Public Sub ResetStanzaGroupValues()
        Me.m_stanzagroup.Cancel()
    End Sub

    Private m_bInUpdate As Boolean = False

    Private Sub SetLeadingGroup(iRow As Integer, col As eColumnTypes)

        Me.m_bInUpdate = True

        Dim ewec As cEwECell = Nothing
        Dim bLeading As Boolean = False
        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        For iLifeStage As Integer = 1 To Me.m_stanzagroup.nLifeStages
            bLeading = (iRow = iLifeStage)
            If bLeading Then style = cStyleGuide.eStyleFlags.OK Else style = cStyleGuide.eStyleFlags.NotEditable
            Me(iLifeStage, col).Value = (iRow = iLifeStage)
            DirectCast(Me(iLifeStage, col + 1), cEwECell).Style = style
        Next

        Me.InvalidateCells()
        Me.m_bInUpdate = False

    End Sub

    Protected Overrides Function OnCellEdited(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        Dim bOK As Boolean = MyBase.OnCellEdited(p, cell)

        If Me.m_bInUpdate Then Return True

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.StartAge
                Dim iLifeStage As Integer = p.Row - 1
                Dim iAge As Integer = CInt(Me(p.Row, eColumnTypes.StartAge).Value)
                If iLifeStage > 0 Then
                    bOK = bOK And (iAge > CInt(Me(p.Row - 1, eColumnTypes.StartAge).Value))
                End If
                If iLifeStage < Me.m_stanzagroup.nLifeStages - 1 Then
                    bOK = bOK And (iAge < CInt(Me(p.Row + 1, eColumnTypes.StartAge).Value))
                End If
        End Select
        Return bOK

    End Function

    Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        If Me.m_bInUpdate Then Return True

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.LeadingB
                Me.SetLeadingGroup(p.Row, eColumnTypes.LeadingB)
            Case eColumnTypes.LeadingCB
                Me.SetLeadingGroup(p.Row, eColumnTypes.LeadingCB)
        End Select
        Return MyBase.OnCellValueChanged(p, cell)

    End Function

#End Region ' Internals

End Class
