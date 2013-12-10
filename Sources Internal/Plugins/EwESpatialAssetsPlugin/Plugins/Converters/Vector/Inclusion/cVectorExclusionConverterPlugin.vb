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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    Public Class cVectorExclusionConverterPlugin
        Inherits cVectorConverterPlugin

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether cells are excluded when overlapping with the attached spatial data.
        ''' If True, cells are excluded when they overlap with attached spatial data. If 
        ''' False, cells are excluded when not overlapping with the spatial data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ExcludeOverlap As Boolean = True

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cVectorConverterPlugin.IsConfigured"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cVectorConverterPlugin.IsCompatible"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As EwEUtils.SpatialData.ISpatialDataSet) As Boolean
            Return MyBase.IsCompatible(ds) And (ds.VarName = eVarNameFlags.LayerExclusion)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cVectorTools.TranslateValueDelegate">Callback</see> to
        ''' determine the value to set when rasterizing for spatial feature.
        ''' </summary>
        ''' <param name="drow">The datarow for the vector object.</param>
        ''' <param name="dValueNone">The nodata value for the underlying raster.</param>
        ''' <returns>A converted value.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ToValue(drow As System.Data.DataRow, dValueNone As Double) As Double

            ' No overlap?
            If (drow Is Nothing) Then Return CDbl(Not Me.ExcludeOverlap)
            Return CDbl(Me.ExcludeOverlap)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cVectorConverterPlugin.DisplayName"/>.
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                ' ToDo: globalize this
                If Me.ExcludeOverlap Then
                    Return "Excluding overlapping cells"
                Else
                    Return "Including overlapping cells"
                End If
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cVectorConverterPlugin.PluginName"/>.
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.VectorExclusionConverter"
            End Get
        End Property
    End Class

End Namespace
