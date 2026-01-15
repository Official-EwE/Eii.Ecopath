' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







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
