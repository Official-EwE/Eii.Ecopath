' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A standard EwE grid cell for static values.
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cEwECell
        Inherits cEwECellBase

#Region " Construction "

        Public Sub New(objVal As Object, t As Type,
                       Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK)
            MyBase.New(objVal, t)
            ' Set value
            If objVal IsNot Nothing Then Me.Value = objVal
            ' Set style
            Me.Style = style
        End Sub

        Public Sub New(objVal As Object, Optional style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK)
            Me.New(objVal, objVal.GetType(), style)
        End Sub

        Public Overrides Sub Dispose()
            ' JS 13Dec10: Memory leaks were discovered on tooltips. Perhaps explicitly 
            '             clearing the grid tooltip text wil fix this.
            Me.ToolTipText = ""
            MyBase.Dispose()
        End Sub

#End Region ' Construction 

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <summary>Locally maintained value.</summary>
        ''' -------------------------------------------------------------------
        Private m_objValue As Object = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commonly called in response to end edit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub SetValue(p_Position As SourceGrid2.Position, p_Value As Object)
            ' JS Jun 2011: Override this method to allow behaviour models to intercept a cell edit.
            For Each bm As BehaviorModels.IBehaviorModel In Me.Behaviors
                Dim args As New PositionCancelEventArgs(p_Position, Me)
                Try
                    bm.OnEditEnded(args)
                Catch ex As Exception
                End Try
                If args.Cancel Then Return
            Next
            Me.Value = p_Value
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the locally maintained value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                Return Me.m_objValue
            End Get
            Set(objValue As Object)
                If Not Object.Equals(objValue, Me.m_objValue) Then
                    Me.m_objValue = objValue
                    Dim pos As New Position(Me.Row, Me.Column)
                    For Each bh As SourceGrid2.BehaviorModels.IBehaviorModel In Me.Behaviors
                        Try
                            bh.OnValueChanged(New SourceGrid2.PositionEventArgs(pos, Me))
                        Catch ex As Exception
                        End Try
                    Next
                End If
            End Set
        End Property

#End Region ' Data

    End Class

End Namespace
