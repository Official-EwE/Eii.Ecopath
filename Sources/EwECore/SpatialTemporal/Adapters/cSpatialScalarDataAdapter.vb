' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace SpatialData

#Region " cSpatialScalarDataAdapter "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of <see cref="cSpatialScalarDataAdapterBase"/> to scale data by 
    ''' a given scale.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialScalarDataAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Constructor "

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.SetCell"/>.
        ''' <remarks>Overridden to scale values prior to being set in the 
        ''' Ecospace data structures.</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCell(layer As cEcospaceLayer,
                                             conn As cSpatialDataConnection,
                                             iRow As Integer,
                                             iCol As Integer,
                                             sValueAtT As Double) As Boolean

            If (conn.ScaleType = eScaleType.Relative) And (sValueAtT <> cCore.NULL_VALUE) Then
                sValueAtT /= conn.Scale
            End If
            Return MyBase.SetCell(layer, conn, iRow, iCol, sValueAtT)

        End Function

#End Region ' Overrides

    End Class

#End Region

End Namespace
