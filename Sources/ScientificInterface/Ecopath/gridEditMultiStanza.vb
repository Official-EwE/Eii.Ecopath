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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "
Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core
Imports SourceGrid2

#End Region

<CLSCompliant(False)> _
Public Class gridEditMultiStanza
    : Inherits EwEGrid

#Region "Private variables"

    Private m_stanzagroup As cStanzaGroup = Nothing

    Private Enum eColumnTypes
        Index = 0
        Name
        StartAge
        Leading
        Biomass
        PBInput
        QBInput
    End Enum

#End Region

#Region "Constructors"

    Public Sub New()
    End Sub

#End Region

#Region "Properties"

    Public Property StanzaGroup() As cStanzaGroup
        Get
            Return Me.m_stanzagroup
        End Get
        Set(ByVal value As cStanzaGroup)
            Me.m_stanzagroup = value
            Me.RefreshContent()
        End Set
    End Property

#End Region

    Public Sub CalculateStanzaParameters()
        ' Sanity check
        If (Me.m_stanzagroup Is Nothing) Then Return
        Me.m_stanzagroup.CalculateParameters()
    End Sub

    Protected Overrides Sub InitStyle()

        If (Me.UIContext Is Nothing) Then Return

        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.StartAge) = New EwEColumnHeaderCell(SharedResources.HEADER_STARTAGE)
        Me(0, eColumnTypes.Leading) = New EwEColumnHeaderCell(SharedResources.HEADER_LEADING)
        Me(0, eColumnTypes.Biomass) = New EwEColumnHeaderCell(eVarNameFlags.Biomass, SharedResources.GENERIC_LABEL_UNIT, cStyleGuide.eUnitType.Currency)
        Me(0, eColumnTypes.PBInput) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTALMORTALITY_UNIT, cStyleGuide.eUnitType.Time)
        Me(0, eColumnTypes.QBInput) = New EwEColumnHeaderCell(SharedResources.HEADER_QB_UNIT, cStyleGuide.eUnitType.Time)

        Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.FixedColumnWidths = False
    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return

        Dim source As cEcoPathGroupInput = Nothing
        Dim ewec As EwECell = Nothing
        Dim iRow As Integer
        Dim bIsEcosimLoaded As Boolean = (core.ActiveEcosimScenarioIndex > -1)

        ' Remove existing rows
        Me.RowsCount = 1

        If (Me.m_stanzagroup Is Nothing) Then Return

        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas

            source = core.EcoPathGroupInputs(Me.m_stanzagroup.iGroups(iStanza))
            iRow = Me.AddRow

            'Index
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)

            'Name
            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

            'Start age
            ewec = New EwECell(0, GetType(Integer))
            ewec.Value = Me.m_stanzagroup.GetVariable(eVarNameFlags.StartAge, iStanza)
            ' First group start age cannot be edited
            If (iStanza = 1) Then ewec.Style = cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.StartAge) = ewec
            Me(iRow, eColumnTypes.StartAge).Behaviors.Add(Me.EwEEditHandler)

            ' Leading
            Me(iRow, eColumnTypes.Leading) = New Cells.Real.CheckBox(Me.m_stanzagroup.LeadingB = iStanza)
            Me(iRow, eColumnTypes.Leading).Behaviors.Add(Me.EwEEditHandler)

            'Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Biomass(iStanza)
            Me(iRow, eColumnTypes.Biomass) = ewec
            Me(iRow, eColumnTypes.Biomass).Behaviors.Add(Me.EwEEditHandler)

            'Total Mortality
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Mortality(iStanza)
            Me(iRow, eColumnTypes.PBInput) = ewec
            Me(iRow, eColumnTypes.PBInput).Behaviors.Add(Me.EwEEditHandler)

            'Consumption/Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.CB(iStanza)
            Me(iRow, eColumnTypes.QBInput) = ewec
            Me(iRow, eColumnTypes.QBInput).Behaviors.Add(Me.EwEEditHandler)
        Next

        Me.SetLeadingGroup(Me.m_stanzagroup.LeadingB)

    End Sub

    Public Sub SetStanzaGroupValues(ByVal bApplyToCore As Boolean)

        Dim iLeading As Integer = Me.m_stanzagroup.LeadingB
        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas
            If CBool(Me(iStanza, eColumnTypes.Leading).Value) Then
                iLeading = iStanza
            End If
        Next
        Me.m_stanzagroup.LeadingB = iLeading
        Me.m_stanzagroup.LeadingCB = iLeading

        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas

            'Start age
            Me.m_stanzagroup.SetVariable(eVarNameFlags.StartAge, CInt(Me(iStanza, eColumnTypes.StartAge).Value), iStanza)
            'Biomass
            Me.m_stanzagroup.Biomass(iStanza) = CSng(Me(iStanza, eColumnTypes.Biomass).Value)
            'Total Mortality
            Me.m_stanzagroup.Mortality(iStanza) = CSng(Me(iStanza, eColumnTypes.PBInput).Value)
            'Consumption/Biomass
            Me.m_stanzagroup.CB(iStanza) = CSng(Me(iStanza, eColumnTypes.QBInput).Value)
        Next

        If bApplyToCore Then
            ' JS 090826: apply changes for all stanza groups, not only the last used stanza group
            For iIndex As Integer = 0 To Core.nStanzas - 1
                Core.StanzaGroups(iIndex).Apply()
            Next
        End If

    End Sub

    Public Sub ResetStanzaGroupValues()
        Me.m_stanzagroup.Cancel()
    End Sub

    Private m_bInUpdate As Boolean = False

    Private Sub SetLeadingGroup(ByVal iRow As Integer)

        Me.m_bInUpdate = True

        Dim ewec As EwECell = Nothing
        Dim bLeading As Boolean = False
        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas
            bLeading = (iRow = iStanza)
            If bLeading Then style = cStyleGuide.eStyleFlags.OK Else style = cStyleGuide.eStyleFlags.NotEditable
            Me(iStanza, eColumnTypes.Leading).Value = (iRow = iStanza)
            DirectCast(Me(iStanza, eColumnTypes.Biomass), EwECell).Style = style
            DirectCast(Me(iStanza, eColumnTypes.QBInput), EwECell).Style = style
        Next

        Me.InvalidateCells()
        Me.m_bInUpdate = False

    End Sub

    Protected Overrides Function OnCellEdited(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        Dim bOK As Boolean = MyBase.OnCellEdited(p, cell)

        If Me.m_bInUpdate Then Return True

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.StartAge
                Dim iStanza As Integer = p.Row - 1
                Dim iAge As Integer = CInt(Me(p.Row, eColumnTypes.StartAge).Value)
                If iStanza > 0 Then
                    bOK = bOK And (iAge > CInt(Me(p.Row - 1, eColumnTypes.StartAge).Value))
                End If
                If iStanza < Me.m_stanzagroup.NStanzas - 1 Then
                    bOK = bOK And (iAge < CInt(Me(p.Row + 1, eColumnTypes.StartAge).Value))
                End If
        End Select
        Return bOK

    End Function

    Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        If Me.m_bInUpdate Then Return True

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.Leading
                Me.SetLeadingGroup(p.Row)
        End Select
        Return MyBase.OnCellValueChanged(p, cell)

    End Function

End Class
