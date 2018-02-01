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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace SpatialData

    Public Class cCookieCutConverterPlugin
        Inherits cPresenceAbsenceConverterPlugin

        Public Sub New()
            MyBase.New()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether cells are excluded when inside the cookie cutter polygon.
        ''' <para>If set to True, cells are excluded when they overlap with attached 
        ''' vector data, and are included in the model area when they do NOT overlap.</para>
        ''' <para>If set to False, cells are excluded when not overlapping with 
        ''' attached vectors, and are included when overlapping with the spatial data.</para>
        ''' </summary>
        ''' <remarks>
        ''' This is the inverse of <see cref="ExcludeOutside"/>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ExcludeInside As Boolean = True

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether cells are excluded when outside the cookie cutter polygon.
        ''' <para>If set to True, cells are included when they overlap with attached 
        ''' vector data, and are excluded in the model area when they do NOT overlap.</para>
        ''' <para>If set to False, cells are excluded when overlapping with 
        ''' attached vectors, and are excluded when overlapping with the spatial data.</para>
        ''' </summary>
        ''' <remarks>
        ''' This is the inverse of <see cref="ExcludeInside"/>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ExcludeOutside As Boolean
            Get
                Return Not Me.ExcludeInside
            End Get
            Set(value As Boolean)
                Me.ExcludeInside = Not value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cPresenceAbsenceConverterPlugin.IsConfigured"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cPresenceAbsenceConverterPlugin.IsCompatible"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As EwEUtils.SpatialData.ISpatialDataSet) As Boolean
            Return MyBase.IsCompatible(ds) And (ds.VarName = eVarNameFlags.LayerExclusion)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cVectorTools.TranslateValueDelegate">Callback</see> to
        ''' determine the value to set when rasterizing
        ''' </summary>
        ''' <param name="drow">The data row for the vector object.</param>
        ''' <param name="dValueNone">The NoData value for the underlying raster.</param>
        ''' <returns>A converted value.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ToValue(drow As System.Data.DataRow, dValueNone As Double) As Double

            ' Has overlap?
            If (drow IsNot Nothing) Then
                ' #Yes: cell overlaps with cookie cutter polygon.
                Return CDbl(Me.ExcludeInside)
            Else
                ' #No: cell does not overlap with cookie cutter polygon.
                Return CDbl(Not Me.ExcludeInside)
            End If

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cPresenceAbsenceConverterPlugin.DisplayName"/>.
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return My.Resources.CONVERTER_COOKIECUTTER_NAME
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cPresenceAbsenceConverterPlugin.PluginName"/>.
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.VectorCookieCutterPlugin"
            End Get
        End Property

    End Class

End Namespace
