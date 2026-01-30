' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace biomass relative forcing data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerBiomassRelativeForcing
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerBiomassRelativeForcing, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerBiomassRelativeForcing
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return d(iRow, iCol, Me.Index)
            Catch ex As Exception

            End Try
            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Dim s As Single = Convert.ToSingle(value)
                d(iRow, iCol, Me.Index) = s
                Me.Invalidate()
            Catch ex As Exception

            End Try
        End Set
    End Property

    Protected Overrides Function DefaultName() As String
        Return Me.m_core.EcopathGroupInputs(Me.Index).Name
    End Function

End Class
