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
#Const SpatialTemp_Framework_Version = VER_NEW

Option Explicit On
Option Strict On

Imports EwECore
Imports EwECore.SpatialData
Imports System.IO
Imports System.Threading

Public Class cSpatialTemporalFileManager
    Private Core As cCore
    Private ConManager As EwECore.SpatialData.cSpatialDataConnectionManager

    Public Sub New(theCore As cCore)
        Me.Core = theCore
        Me.ConManager = Core.SpatialDataConnectionManager

    End Sub

    Public Sub SwapFiles(Layer As cEcospaceLayer, sourceFile As String)
        'WARNING HARDWIRED FOR THE RBT FILES

        Dim ds As EwEUtils.SpatialData.ISpatialDataSet
        Dim mfds As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin

        'Get the data set for this Layer and VarName
        'This will have to be different in the old and new version of the spatial temporal framework
        'which can/could be hidden behind preprocessor directives
        ds = Me.getDataSet(Layer)
        If ds IsNot Nothing Then
            mfds = DirectCast(ds, EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin)
            'Hardwire to the first file in the list
            'it will be only file
            mfds.File(ds.TimeSteps(0)) = sourceFile
        End If

    End Sub

#If SpatialTemp_Framework_Version = VER_OLD Then

    
    Private Function getDataSet(Layer As cEcospaceLayer) As EwEUtils.SpatialData.ISpatialDataSet
        Dim ds As EwEUtils.SpatialData.ISpatialDataSet

        For Each adt As cSpatialDataAdapter In Me.ConManager.Adapters
            If adt.VarName = Layer.VarName Then
                For iConn As Integer = 1 To cSpatialDataStructures.cMAX_CONN
                    ds = adt.Dataset(Layer.Index, iConn)
                    If ds IsNot Nothing Then
                        Return ds
                    End If
                Next
            End If
        Next

        Return Nothing

    End Function

#ElseIf SpatialTemp_Framework_Version = VER_NEW Then

    Private Function getDataSet(Layer As cEcospaceLayer) As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        'Dim mfDs As EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin
        'Dim ds As EwEUtils.SpatialData.ISpatialDataSet

        'Dim layerDSs() As EwEUtils.SpatialData.ISpatialDataSet = Me.Manager.Datasets(EwEUtils.Core.eVarNameFlags.LayerDepth)
        'ds = layerDSs(0)

        'mfDs = DirectCast(ds, EwESpatialAssetsPlugin.SpatialData.cMultiFileDataSetPlugin)
        'Return mfDs

    End Function

#End If

End Class
