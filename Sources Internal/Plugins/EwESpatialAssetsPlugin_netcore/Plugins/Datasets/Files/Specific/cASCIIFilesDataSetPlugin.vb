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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports DotSpatial.Data
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' JS 07/12/24: fixed number format to en-US standard (according to ChatGPT)
    ''' </summary>
    Public Class cASCIIFilesDataSetPlugin
        Inherits cMultiFileDataSetPlugin

        Private m_raster As ISpatialRaster = Nothing

        Public Sub New()
            MyBase.New()
            ' Default name and description
            Me.m_strName = My.Resources.DATASET_ASCII_NAME
            Me.CustomDescription = My.Resources.DATASET_ASCII_DESCRIPTION
        End Sub

#Region " Overrides "

        Public Overrides Function GetExtentAtT(dt As Date,
                                               ByRef ptfTL As System.Drawing.PointF,
                                               ByRef ptfBR As System.Drawing.PointF) As Boolean

            Dim bOK As Boolean = MyBase.GetExtentAtT(dt, ptfTL, ptfBR)

            If (bOK) Then
                ' De-spationalize
                Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
                Dim dx As Single = ptfBR.X - ptfTL.X
                Dim dy As Single = ptfTL.Y - ptfBR.Y
                ptfTL = New PointF(bm.PosTopLeft.X, bm.PosTopLeft.Y)
                ptfBR = New PointF(bm.PosTopLeft.X + dx, bm.PosTopLeft.Y - dy)
            End If

            Return bOK

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.LoadSource"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function LoadSource() As Boolean

            Dim strFileName As String = Me.SourceFileName()
            ' Already read? Ok!
            If (Me.m_raster IsNot Nothing) Then Return True

            ' File missing?
            If (Not System.IO.File.Exists(strFileName)) Then
                ' #Yes: report error
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FILENOTFOUND, strFileName), eStatusFlags.MissingParameter)
                ' Run away
                Return False
            End If

            Try
                Dim imp As New cEcospaceImportExportASCIIData(Me.m_core)
                If (imp.Read(strFileName)) Then
                    Me.m_raster = imp.ToRaster()
                End If

            Catch ex As Exception
                ' Log generic panic message
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
            End Try

            ' Report all over success
            Return (Me.m_raster IsNot Nothing)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.UnlockData"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function UnlockData() As Boolean
            Me.m_raster = Nothing
            Return MyBase.UnlockData()
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetRaster"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetRaster(converter As ISpatialDataConverter, strLayerName As String) As ISpatialRaster

            If (Not Me.IsLocked) Then Return Nothing
            Me.LoadSource()
            Return Me.m_raster

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.ConversionFormat"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property ConversionFormat As String
            Get
                ' No conversion needed
                Return String.Empty
            End Get
        End Property

#End Region ' Overrides

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Plug-in that provides direct access to ASCII files, without requiring GDAL"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DataSet.0400"
            End Get
        End Property

#End Region ' Plug-in implementation

    End Class

End Namespace
