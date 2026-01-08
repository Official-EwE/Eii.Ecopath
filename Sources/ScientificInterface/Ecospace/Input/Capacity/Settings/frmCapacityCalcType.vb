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
Imports EwECore.Common
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecospace

    Public Class frmCapacityCalcType

#Region " Private vars "

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()

            Me.InitializeComponent()
            Me.Grid = Me.m_grid

        End Sub

#End Region ' Construction

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            Me.m_tsbnResetInputCapacity.Image = SharedResources.ResetHS
        End Sub

#End Region ' Overrides

#Region " Events "

        Private Sub OnResetInputCapacity(sender As Object, e As EventArgs) Handles m_tsbnResetInputCapacity.Click

            ' ToDo: prompt?
            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            For igroup As Integer = 1 To Me.Core.nGroups
                bm.LayerHabitatCapacityInput(igroup).Reset()
            Next
            Me.Core.onChanged(bm.LayerHabitatCapacityInput(1))

        End Sub

#End Region ' Events

    End Class

End Namespace
