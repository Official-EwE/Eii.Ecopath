' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.VisualModels

Namespace Controls.EwEGrid

    Public Class cEwEStatusCell
        Inherits cEwECellBase

        Private m_strText As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Visual model for reflecting 'Original' values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property DefaultVisualOriginal() As VisualModels.IVisualModel
            Get
                Dim vm As VisualModels.Common = New cEwECellVisualizer()
                vm.ForeColor = Color.FromArgb(255, 0, 0, 0)
                vm.TextAlignment = ContentAlignment.MiddleCenter
                vm.MakeReadOnly()
                Return vm
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Visual model for reflecting 'Added' values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property DefaultVisualAdd() As VisualModels.IVisualModel
            Get
                Dim vm As VisualModels.Common = New cEwECellVisualizer()
                vm.ForeColor = Color.FromArgb(255, 8, 128, 12)
                vm.TextAlignment = ContentAlignment.MiddleCenter
                vm.MakeReadOnly()
                Return vm
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Visual model for reflecting 'Removed' values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property DefaultVisualRemove() As VisualModels.IVisualModel
            Get
                Dim vm As VisualModels.Common = New cEwECellVisualizer()
                vm.ForeColor = Color.FromArgb(255, 255, 22, 12)
                vm.TextAlignment = ContentAlignment.MiddleCenter
                vm.MakeReadOnly()
                Return vm
            End Get
        End Property

        Public Sub New(status As eItemStatusTypes)
            MyBase.New(status, GetType(Integer))
            Me.Style = cStyleGuide.eStyleFlags.NotEditable
        End Sub

        Public Overrides Sub SetValue(pos As SourceGrid2.Position, value As Object)
            MyBase.SetValue(pos, value)

            Dim status As eItemStatusTypes = eItemStatusTypes.Invalid
            Dim vm As IVisualModel = Me.DefaultVisualOriginal
            Dim strText As String = ""

            If (TypeOf value Is eItemStatusTypes) Then
                status = DirectCast(value, eItemStatusTypes)
            End If
            Select Case status
                Case eItemStatusTypes.Original
                    ' NOP
                Case eItemStatusTypes.Added
                    strText = My.Resources.GENERIC_VALUE_CREATE_PENDING
                    vm = Me.DefaultVisualAdd
                Case eItemStatusTypes.Removed
                    strText = My.Resources.GENERIC_VALUE_DELETE_PENDING
                    vm = Me.DefaultVisualRemove
                Case eItemStatusTypes.Invalid
                    ' NOP
            End Select
            Me.m_strText = strText
            Me.VisualModel = vm
            MyBase.SetValue(pos, value)
        End Sub

        Public Overrides ReadOnly Property DisplayText As String
            Get
                Return Me.m_strText
            End Get
        End Property

    End Class

End Namespace
