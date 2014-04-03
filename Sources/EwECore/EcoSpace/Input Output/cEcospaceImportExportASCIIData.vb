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

Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities
Imports EwEUtils.SpatialData
Imports EwEUtils.Core

#End Region ' Imports

' ToDo: bring in ASCII reader and writer logic from SpatialAssets Plugin

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class for importing and exporting data from ASCII grid files
''' directly to and from Ecospace, without GIS intervention.
''' </summary>
''' -----------------------------------------------------------------------
Friend Class cEcospaceImportExportASCIIData
    Implements IEcospaceImportExport

    Private m_bm As cEcospaceBasemap = Nothing

#Region " Construction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Construct a new instance of this class.
    ''' </summary>
    ''' <param name="bm">The <see cref="cEcospaceBasemap"/> to operate onto.</param>
    ''' <param name="astrFields">An optional array of field names.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(bm As cEcospaceBasemap, _
                   Optional ByVal astrFields() As String = Nothing)

        Debug.Assert(bm IsNot Nothing)
        Me.m_bm = bm

        Throw New NotImplementedException("Nothing here yet")
    End Sub

#End Region ' Construction

    Public Property Value(iRow As Integer, iCol As Integer, Optional strField As String = "") As Object Implements EwEUtils.Core.IEcospaceImportExport.Value
        Get
            Throw New NotImplementedException("Nothing here yet")
        End Get
        Set(value As Object)
            Throw New NotImplementedException("Nothing here yet")
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns data in the form of a <see cref="ISpatialRaster"/>>
    ''' </summary>
    ''' <returns>A raster.</returns>
    ''' -------------------------------------------------------------------
    Public Function ToRaster(Optional ByVal strField As String = "") As ISpatialRaster _
        Implements IEcospaceImportExport.ToRaster
        Return New cEcospaceImportExportRaster(Me, Me.m_bm, strField)
    End Function

End Class
