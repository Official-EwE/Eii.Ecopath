' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of months.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorUpwelling
        Inherits cLayerEditorRange
        Implements IMonthFilter

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorRange))
        End Sub

        Public Sub New(t As Type)
            MyBase.New(t)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event OnFilterChanged(sender As IContentFilter) Implements IMonthFilter.FilterChanged

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath group to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Month() As Integer _
            Implements IMonthFilter.Month
            Get
                Return Me.Layer.Data.SecundaryIndex
            End Get
            Set(value As Integer)
                ' Will Group index change?
                If (value <> Me.Layer.Data.SecundaryIndex) Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    Me.Layer.Data.SecundaryIndex = value
                    ' Force map update
                    Me.Layer.Update(cDisplayLayer.eChangeFlags.Map Or cDisplayLayer.eChangeFlags.Descriptive, False)

                    Try
                        RaiseEvent OnFilterChanged(Me)
                    Catch ex As Exception
                        ' NOP
                    End Try
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace