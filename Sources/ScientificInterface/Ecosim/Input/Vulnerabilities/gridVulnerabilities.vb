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

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class gridVulnerabilities
        Inherits cEwEGrid

        Private m_bmRowCol As New BehaviorModels.CustomEvents
        Private m_VisDiagonal As New SourceGrid2.VisualModels.Common

        Public Sub New()
            MyBase.New()
            Me.m_VisDiagonal.BackColor = Color.LightGray
            Me.m_VisDiagonal.TextAlignment = ContentAlignment.MiddleCenter
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            ' Define grid dimensions
            Dim source As cCoreGroupBase = Nothing
            Me.Redim(Me.Core.nGroups + 1, 2)

            ' Set header cells
            ' # (0,0)
            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To Me.Core.nGroups
                source = Me.Core.EcopathGroupInputs(i)
                ' Group index header cell
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                'Me(i, 0).Behaviors.Add(m_bmRowCol)

                ' # Group name row header cells
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                'Me(i, 1).Behaviors.Add(m_bmRowCol)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    'Me(0, columnIndex).Behaviors.Add(m_bmRowCol)
                    columnIndex = columnIndex + 1
                End If
            Next

        End Sub

        Protected Overrides Sub FillData()

            Dim grpPrey As cCoreGroupBase = Nothing
            Dim grpPred As cCoreGroupBase = Nothing
            Dim iCol As Integer = 2
            Dim prop As cProperty = Nothing
            Dim cell As cPropertyCell = Nothing
            Dim pm As cPropertyManager = Me.PropertyManager

            ' Populate grid data cells
            For iPrey As Integer = 1 To Me.Core.nGroups
                grpPrey = Me.Core.EcosimGroupInputs(iPrey)
                iCol = 2
                For iPred As Integer = 1 To Me.Core.nLivingGroups
                    ' JS 16may08: Use ecopath groups for sec indexes
                    grpPred = Me.Core.EcopathGroupInputs(iPred)

                    If grpPred.PP < 1 Then

                        prop = pm.GetProperty(grpPrey, eVarNameFlags.VulMult, grpPred)
                        cell = New cPropertyCell(prop)
                        cell.SuppressZero = True

                        If iPrey = (iCol - 1) Then
                            cell.VisualModel = Me.m_VisDiagonal
                        End If

                        ' Store cell
                        Me(iPrey, iCol) = cell

                        ' Next column
                        iCol += 1
                    End If
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace
