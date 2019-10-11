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
Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region  ' Imports

Public Class gridDrivers
    Inherits EwEGrid

    Private Enum eColumnTypes
        Index
        Name
        Units
        Min
        Max
        Mean
    End Enum

    Public Sub New()

    End Sub

    Public Property Manager As IEnvironmentalResponseManager = Nothing

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalized this
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
        Me(0, eColumnTypes.Units) = New EwEColumnHeaderCell(SharedResources.HEADER_UNITS)
        Me(0, eColumnTypes.Min) = New EwEColumnHeaderCell("Min")
        Me(0, eColumnTypes.Max) = New EwEColumnHeaderCell("Max")
        Me(0, eColumnTypes.Mean) = New EwEColumnHeaderCell("Mean")

    End Sub

    Protected Overrides Sub FillData()

        If (Me.Manager Is Nothing) Then Return

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap

        For i As Integer = 1 To Me.Manager.nEnviroData
            Me.AddDriver(Me.Manager.EnviroData(i))
        Next

    End Sub

    Private Sub AddDriver(driver As IEnviroInputData)

        Dim iRow As Integer = Me.AddRow()

        Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iRow))
        Me(iRow, eColumnTypes.Name) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Units) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Min) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Max) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Mean) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
        Me.Rows(iRow).Tag = driver

        Me.UpdateDriver(iRow)

    End Sub

    Private Sub UpdateDriver(iRow As Integer)

        Dim driver As IEnviroInputData = DirectCast(Me.Rows(iRow).Tag, IEnviroInputData)

        Dim u As New cUnits(Me.Core)
        Dim units As String = ""
        Dim md As cVariableMetaData = Nothing

        If TypeOf driver Is cMediationBaseFunction Then
            md = Nothing
        ElseIf TypeOf driver Is cEnviroInputMap Then
            Dim layer As cEcospaceLayer = DirectCast(driver, cEnviroInputMap).Layer
            units = layer.Units
            If String.IsNullOrEmpty(units) Then units = u.ToString(cVariableMetaData.Get(layer.VarName))
        End If

        Me(iRow, eColumnTypes.Name).Value = driver.Name
        Me(iRow, eColumnTypes.Units).Value = units
        Me(iRow, eColumnTypes.Min).Value = driver.Min
        Me(iRow, eColumnTypes.Max).Value = driver.Max
        Me(iRow, eColumnTypes.Mean).Value = driver.Mean

    End Sub

    Public ReadOnly Property SelectedDriver As IEnviroInputData
        Get
            If Me.SelectedRow < 1 Then Return Nothing
            Return DirectCast(Me.Rows(Me.SelectedRow).Tag, IEnviroInputData)
        End Get
    End Property

End Class
