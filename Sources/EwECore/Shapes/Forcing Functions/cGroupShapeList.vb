' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' This is a collection of cShapeGroupPair 
''' </summary>
Public Class cGroupShapeList
    Implements Collections.IEnumerable

    Private m_list As New List(Of cGroupShapePair)
    Private m_data As cEcosimDatastructures
    Private m_stanza As cStanzaDatastructures
    Private m_manager As cEggProductionShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef StanzaData As cStanzaDatastructures, ByRef EggProdManager As cEggProductionShapeManager)
        Me.m_data = EcoSimData
        Me.m_manager = EggProdManager
        Me.m_stanza = StanzaData
    End Sub

    Friend Sub Add(ByRef shapeGroupPair As cGroupShapePair)

        'ToDo_jb cAppliesToList.Add()  Make sure the shapeGroupPair.iStanzaGroup is a valid stanza group
        Me.m_list.Add(shapeGroupPair)

    End Sub

    Default Public Property Item(Index As Integer) As cGroupShapePair
        Get
            Try
                Return Me.m_list.Item(Index)
            Catch ex As Exception
                Return Nothing
            End Try
        End Get
        Set(value As cGroupShapePair)
            Try
                Me.m_list.Item(Index) = value
            Catch ex As Exception
                Return
            End Try
        End Set
    End Property

    Public Function Count() As Integer
        Return Me.m_list.Count
    End Function

    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.m_list.GetEnumerator
    End Function

End Class
