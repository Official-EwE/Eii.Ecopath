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

#Const VER_OLD = 1
#Const VER_NEW = 2
#Const SpatialTemp_Framework_Version = VER_OLD

Option Explicit On
Option Strict On

Imports EwECore
Imports EwECore.SpatialData
Imports System.IO
Imports System.Threading

Public Class cSpatialTemporalFileManager
    Private Core As cCore
    Private Manager As EwECore.SpatialData.cSpatialDataSetManager

    Public Sub New(theCore As cCore)
        Me.Core = theCore
        Me.Manager = Core.SpatialDataConnectionManager.DatasetManager

    End Sub

    Public Sub SwapFiles(sourceFile As String)
        'WARNING NOT IMPLEMENTED
        'In theory this is a better way to do this
        'Swap the name of the file in the dataset

        Debug.Assert(False, Me.ToString + ".SwapFiles() Not Implemented!")

        Dim ds As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin

        'Get the data set for this Layer and VarName
        'This will have to be different in the old and new version of the spatial temporal framework
        'which can/could be hidden behind preprocessor directives
        ds = Me.getDataSet()

        'Hardwire to the first file in the list
        'that will be only file
        ds.File(ds.TimeSteps(0)) = sourceFile

    End Sub

#If SpatialTemp_Framework_Version = VER_OLD Then

    Private Function getDataSet() As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        'HACK proof of concept 
        'Hardwire to get the Depth dataset
        Dim mfDs As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        Dim ds As EwEUtils.SpatialData.ISpatialDataSet
        'This will be the Depth dataset 
        'cause that's the first one in the list
        ds = Me.Manager.Item(0)

        mfDs = DirectCast(ds, EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin)
        Return mfDs

    End Function

#ElseIf SpatialTemp_Framework_Version = VER_NEW Then

      Private Function getDataSet() As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        Dim mfDs As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        Dim ds As EwEUtils.SpatialData.ISpatialDataSet

        Dim layerDSs() As EwEUtils.SpatialData.ISpatialDataSet = Me.Manager.Datasets(EwEUtils.Core.eVarNameFlags.LayerDepth)
        ds = layerDSs(0)

        mfDs = DirectCast(ds, EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin)
        Return mfDs

    End Function

#End If








End Class
