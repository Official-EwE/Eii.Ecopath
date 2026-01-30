' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace migration data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorMigration
        Inherits cLayerEditorRange
        Implements IMonthFilter

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorMigration))
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event OnFilterChanged(sender As IContentFilter) _
            Implements IMonthFilter.FilterChanged

        Public Sub [Next]()
            Me.Month = If(Me.Month = cCore.N_MONTHS, 1, Me.Month + 1)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath group whose migration data
        ''' is being edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Month() As Integer _
            Implements IMonthFilter.Month
            Get
                Dim layerMig As cEcospaceLayerMigration = CType((Me.Layer.Data), cEcospaceLayerMigration)
                Return layerMig.SecundaryIndex
            End Get
            Set(value As Integer)
                Dim layerMig As cEcospaceLayerMigration = CType((Me.Layer.Data), cEcospaceLayerMigration)
                ' Will month index change?
                If (value <> layerMig.SecundaryIndex) Then
                    ' #Yes: update month index in the underlying Ecospace layer
                    layerMig.SecundaryIndex = value
                    ' Force map update
                    Me.Layer.Update(cDisplayLayer.eChangeFlags.Map, False)

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