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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
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
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2.Cells

#End Region

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to configure Ecospace habitat fishing limitations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)>
    Public Class gridEcospaceMPAEnforcement
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            FirstMPA
        End Enum

        Private m_bInUpdate As Boolean = False
        Private WithEvents m_bpEffort As cBooleanProperty = Nothing

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

#Region " Overrides "

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_bpEffort = Nothing
                End If

                If (value IsNot Nothing) Then
                    Dim ecospaceModelParams As cEcospaceModelParameters = value.Core.EcospaceModelParameters()
                    Dim propMan As cPropertyManager = value.PropertyManager
                    Me.m_bpEffort = DirectCast(propMan.GetProperty(ecospaceModelParams, eVarNameFlags.PredictEffort), cBooleanProperty)
                End If

                MyBase.UIContext = value

            End Set
        End Property

        Protected Overrides Sub InitStyle()

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(1 + Core.nFleets, 3 + Me.Core.nMPAs)

            'Set header cells #(0,0)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)

            For i As Integer = 1 To Me.Core.nFleets
                source = Me.Core.FleetInputs(i)
                Me(i, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                Me(i, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            For i As Integer = 1 To Me.Core.nMPAs
                source = Me.Core.EcospaceMPAs(i)
                Me(0, eColumnTypes.FirstMPA + i - 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

        End Sub

        Protected Overrides Sub FillData()

            Dim bEnable As Boolean = (CBool(Me.m_bpEffort.GetValue()) = True)
            Dim cell As EwECellBase = Nothing

            For i As Integer = 1 To Me.Core.nFleets

                Dim source As cEcospaceFleet = Me.Core.EcospaceFleets(i)

                If (bEnable) Then
                    For iMPA As Integer = 1 To Me.Core.nMPAs
                        Me(i, eColumnTypes.FirstMPA + iMPA - 1) = New Cells.Real.CheckBox(Not CBool(source.MPAFishery(iMPA)))
                        Me(i, eColumnTypes.FirstMPA + iMPA - 1).Behaviors.Add(Me.EwEEditHandler)
                    Next
                Else
                    For iMPA As Integer = 1 To Me.Core.nMPAs
                        Me(i, eColumnTypes.FirstMPA + iMPA - 1) = New EwECell("", GetType(String), eStyleFlags.NotEditable Or eStyleFlags.Null)
                    Next
                End If

            Next

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

        Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean

            Dim fleet As cEcospaceFleet = Me.Core.EcospaceFleets(p.Row)
            Dim iMPA As Integer = p.Column - eColumnTypes.FirstMPA + 1
            fleet.MPAFishery(iMPA) = Not CBool(cell.GetValue(p))

        End Function

#End Region ' Overrides

#Region " Event handlers "

        Private Sub m_bpEffort_PropertyChanged(prop As cProperty, changeFlags As cProperty.eChangeFlags) _
            Handles m_bpEffort.PropertyChanged

            Try
                BeginInvoke(New MethodInvoker(AddressOf RefreshContent))
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Event handlers

    End Class

End Namespace

