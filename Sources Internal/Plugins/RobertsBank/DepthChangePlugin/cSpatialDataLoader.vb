
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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Imports EwECore.SpatialData
Imports EwESpatialAssetsPlugin

#End Region


Public Class cSpatialDataLoader

#Region "Private Variables"

    Private m_core As cCore
    Private m_plugin As cDepthChangePluginPoint
    'For now the name of the Depth Dataset is hardwired
    'this may have to change
    Private m_DepthDSName As String = "Roberts Bank Depth"
    Private m_SpatialConfigFile As String

    Private m_DepthAdapter As cDepthDataAdapter

    Private Shared m_isAdapterLoaded As Boolean = False

#End Region

#Region "Public Stuff"

#Region "Methods"

    Public Sub New(thePlugin As cDepthChangePluginPoint)

        Me.m_core = thePlugin.Core
        Me.m_plugin = thePlugin

    End Sub


    Public Function LoadSpatialConfigFile(theConfigFile As String) As Boolean
        Dim bLoaded As Boolean = False

        Try
            Me.SpatialConfigFile = theConfigFile
            If Me.ReadSpatialConfigFile() Then
                bLoaded = True
            End If 'Me.m_SpatialDataLoader.LoadSpatialConfigFile(filename)

        Catch ex As Exception
            bLoaded = False
        End Try

        Debug.Assert(bLoaded, "Failed to Configure and Load Spatial data.")
        Return bLoaded
    End Function

#End Region

#Region "Properties"

    Public Property DepthDataSetName As String
        Get
            Return Me.m_DepthDSName
        End Get
        Set(value As String)
            Me.m_DepthDSName = value
        End Set
    End Property


    Public Property SpatialConfigFile As String
        Get
            Return Me.m_SpatialConfigFile
        End Get
        Set(value As String)
            Me.m_SpatialConfigFile = value
        End Set
    End Property

    Public ReadOnly Property DataSets() As List(Of EwEUtils.SpatialData.ISpatialDataSet)
        Get
            Return Me.Plugin.Core.SpatialDataConnectionManager.DatasetManager.ToList
        End Get
    End Property

#End Region

#End Region

#Region "Private Stuff"


    Private Function ReadSpatialConfigFile() As Boolean

        Try

            If Not File.Exists(Me.SpatialConfigFile) Then
                Debug.Assert(False, Me.ToString + ".LoadSpatialConfigFile() File does not exist.")
                Return False
            End If

            If Me.m_core.SpatialDataConnectionManager.DatasetManager.Load(Me.SpatialConfigFile) Then
                Return True
            End If
            Return False

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".LoadSpatialConfigFile() Exception: " + ex.Message)
        End Try

        'oppsssss
        Return False

    End Function


    Public Function InitDepthDataSet() As Boolean
        Dim bReturn As Boolean = False

        Try
            Debug.Assert(Me.m_DepthAdapter IsNot Nothing, "Oppss... DepthChangePluginPoint not configured correctly.")
            Dim DataSet As EwEUtils.SpatialData.ISpatialDataSet

            DataSet = Me.getDataSetByName(Me.DepthDataSetName)

            If Not DataSet Is Nothing Then
                'Added the DataSet to the DepthAdapter
                Dim conn As cSpatialDataConnection = Me.m_DepthAdapter.AddConnection(0)
                conn.Dataset = DataSet
                bReturn = True
            End If

        Catch ex As Exception
            bReturn = False
        End Try

        Debug.Assert(bReturn, Me.ToString + ".InitSpatialData() Failed to initialize the spatial data.")
        Return bReturn
    End Function


    Public Sub AddedDepthAdapter()
        Try

            If cSpatialDataLoader.m_isAdapterLoaded Then
                Return
            End If
            Dim Converter As EwEUtils.SpatialData.ISpatialDataConverter

            Me.m_DepthAdapter = New cDepthDataAdapter(Me.Plugin.Core, Me.Plugin.Ecospace, Me.Plugin.Ecospace.EcoSpaceData)
            Debug.Assert(Me.m_DepthAdapter IsNot Nothing, Me.ToString + ".InitSpatialData() Failed to create Adapter.")

            'Get the Rater Converter from the core
            Converter = Me.getConverterByType(GetType(EwESpatialAssetsPlugin.SpatialData.cRasterConverterPlugin))

            If (Not Me.m_DepthAdapter Is Nothing) And (Not Converter Is Nothing) Then
                'Ok managed to create the DepthAdapter and the get the Converter from the core
                'Now hook them up
                'And add the DepthAdapter to the core spatial data manager          
                'Me.m_DepthAdapter.Converter(1, 1) = Converter
                Plugin.Core.SpatialDataConnectionManager.AddAdapter(Me.m_DepthAdapter)
                cSpatialDataLoader.m_isAdapterLoaded = True
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            cSpatialDataLoader.m_isAdapterLoaded = False
        End Try

    End Sub

    Private Function getConverterByType(ConverterType As Type) As EwEUtils.SpatialData.ISpatialDataConverter
        For Each converter In Plugin.Core.SpatialDataConnectionManager.ConverterTemplates()
            If converter.GetType Is ConverterType Then
                Return converter
            End If
        Next
        Debug.Assert(False, Me.ToString + ".getConverterByType() Failed to find Converter " + ConverterType.ToString)
        Return Nothing
    End Function



    Private Function getDataSetByName(name As String) As EwEUtils.SpatialData.ISpatialDataSet

        For Each ds As EwEUtils.SpatialData.ISpatialDataSet In Plugin.Core.SpatialDataConnectionManager.DatasetManager
            If String.Compare(ds.DisplayName, name) = 0 Then
                Return ds
            End If
        Next ds
        Debug.Assert(False, Me.ToString + ".getDataSetByName() Failed to find dataset " + name)
        Return Nothing
    End Function

    Private ReadOnly Property Plugin As cDepthChangePluginPoint
        Get
            Return Me.m_plugin
        End Get
    End Property


#End Region

End Class
