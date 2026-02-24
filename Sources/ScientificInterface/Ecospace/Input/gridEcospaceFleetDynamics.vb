' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to configure Ecospace habitat fishing limitations.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class gridEcospaceHabitatDyncamis
        Inherits cEwEGrid

        Private m_bInUpdate As Boolean = False
        Private WithEvents m_bpEffort As cBooleanProperty = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            EffPower
            SEmult
        End Enum

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
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

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            'Set header cells #(0,0)
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
            Me(0, eColumnTypes.EffPower) = New cEwEColumnHeaderCell(SharedResources.HEADER_EFFPOWER)
            Me(0, eColumnTypes.SEmult) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTEFFMULTI)

        End Sub

        Protected Overrides Sub FillData()

            Me.RowsCount = 1

            For i As Integer = 1 To Me.Core.nFleets

                Dim fleet As cEcospaceFleetInput = Me.Core.EcospaceFleetInputs(i)
                Dim iRow As Integer = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, fleet, eVarNameFlags.Index)
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, fleet, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.EffPower) = New cPropertyCell(Me.PropertyManager, fleet, eVarNameFlags.EffectivePower)
                Me(iRow, eColumnTypes.SEmult) = New cPropertyCell(Me.PropertyManager, fleet, eVarNameFlags.SEmult)

            Next

            Me.UpdateEnabledState()

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecospace}
            End Get
        End Property

        Private Sub UpdateEnabledState()

            Dim cell As cEwECellBase = Nothing
            Dim bEnable As Boolean = (CBool(Me.m_bpEffort.GetValue()) = True)

            For iRow As Integer = 1 To Me.RowsCount - 1
                cell = DirectCast(Me(iRow, eColumnTypes.EffPower), cEwECellBase)
                If bEnable Then
                    cell.Style = cell.Style And Not eStyleFlags.NotEditable
                Else
                    cell.Style = cell.Style Or eStyleFlags.NotEditable
                End If
                cell = DirectCast(Me(iRow, eColumnTypes.SEmult), cEwECellBase)
                If bEnable Then
                    cell.Style = cell.Style And Not eStyleFlags.NotEditable
                Else
                    cell.Style = cell.Style Or eStyleFlags.NotEditable
                End If
            Next
            Me.InvalidateCells()

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub m_bpEffort_PropertyChanged(prop As cProperty, changeFlags As cProperty.eChangeFlags) _
            Handles m_bpEffort.PropertyChanged

            Try
                Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateEnabledState))
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Event handlers

    End Class

End Namespace

