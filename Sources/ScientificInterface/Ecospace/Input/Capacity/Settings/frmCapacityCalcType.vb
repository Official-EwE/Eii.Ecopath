' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

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
