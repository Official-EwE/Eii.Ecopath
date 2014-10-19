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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecospace

    Public Class frmCapacityCalcType

#Region " Private vars "

        Private m_lProps As New List(Of cProperty)
        Private m_bDirty As Boolean = True

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()

            Me.InitializeComponent()
            Me.Grid = Me.m_grid

        End Sub

#End Region ' Construction

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_tsbnUseOnlyHabitat.Image = SharedResources.Habitat
            Me.m_tsbnUseOnlyCapacity.Image = SharedResources.CalculatorHS

            For i As Integer = 1 To Me.Core.nGroups
                Dim grp As cEcospaceGroup = Me.Core.EcospaceGroups(i)
                Dim prop As cProperty = Me.PropertyManager.GetProperty(grp, eVarNameFlags.EcospaceCapCalType)
                Me.m_lProps.Add(prop)
                AddHandler prop.PropertyChanged, AddressOf OnPropertyChanged
            Next

            Me.m_bDirty = True
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            For Each prop As cProperty In Me.m_lProps
                RemoveHandler prop.PropertyChanged, AddressOf OnPropertyChanged
            Next
            Me.m_lProps.Clear()

            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub UpdateControls()

            If (Not Me.m_bDirty) Then Return

            Dim iHab As Integer = 0
            Dim iCap As Integer = 0
            For Each prop As cProperty In Me.m_lProps
                Select Case DirectCast(prop.GetValue, eEcospaceCapacityCalType)
                    Case eEcospaceCapacityCalType.Capacity : iCap += 1
                    Case eEcospaceCapacityCalType.Habitat : iHab += 1
                End Select
            Next

            Me.m_tsbnUseOnlyHabitat.Checked = (iHab = Me.Core.nGroups)
            Me.m_tsbnUseOnlyCapacity.Checked = (iCap = Me.Core.nGroups)

            MyBase.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Events "

        Private Sub OnUseOnlyHabitat(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnUseOnlyHabitat.Click

            Try
                Me.SetAllTo(eEcospaceCapacityCalType.Habitat)
            Catch ex As Exception
                cLog.Write(ex, "frmCapacityCalcType.OnUseOnlyHabitat")
            End Try

        End Sub

        Private Sub OnUseOnlyCapacity(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnUseOnlyCapacity.Click

            Try
                Me.SetAllTo(eEcospaceCapacityCalType.Capacity)
            Catch ex As Exception
                cLog.Write(ex, "frmCapacityCalcType.OnUseOnlyCapacity")
            End Try

        End Sub

        Private Sub OnPropertyChanged(prop As cProperty, ct As cProperty.eChangeFlags)
            Try
                Me.m_bDirty = True
                Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub SetAllTo(calctype As eEcospaceCapacityCalType)

            Me.Core.SetBatchLock(cCore.eBatchLockType.Update)

            For i As Integer = 1 To Me.Core.nGroups
                Core.EcospaceGroups(i).CapacityCalculationType = calctype
            Next

            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)

        End Sub

#End Region ' Internals

    End Class

End Namespace
