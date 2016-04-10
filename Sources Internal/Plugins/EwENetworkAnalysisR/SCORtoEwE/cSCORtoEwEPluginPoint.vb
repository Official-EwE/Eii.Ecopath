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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Database

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' SCOR reader to EwE model converter.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cSCORtoEwEPluginPoint
    Implements IModelImportPlugin
    Implements IDisposedPlugin

#Region " Private vars "

    Private m_sr As StreamReader = Nothing

#End Region ' Private vars

#Region " Generic implementation "

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Stuart Borrett, Sheila Heymans, Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for EwE6 that imports an Ecopath model from a SCOR file"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ImportSCOR"
        End Get
    End Property

#End Region ' Generic implementation

#Region " Database plug-in implementation "

    Public Function CanImportFrom(strSource As String) As Boolean _
        Implements EwEUtils.Database.IModelImporter.CanImportFrom

        Select Case Path.GetExtension(strSource).ToLower
            Case ".scor", ".dat"
                Return True
        End Select
        Return False

    End Function

    Public Function Open(ByVal strSource As String) As Boolean _
        Implements EwEUtils.Database.IModelImporter.Open
        Return False
    End Function

    Public Function IsOpen() As Boolean _
        Implements EwEUtils.Database.IModelImporter.IsOpen
        Return (Me.m_sr IsNot Nothing)
    End Function

    Public Sub Close() _
        Implements EwEUtils.Database.IModelImporter.Close
        Me.m_sr.Close()
        Me.m_sr.Dispose()
        Me.m_sr = Nothing
    End Sub

    Public Function Import(info As cExternalModelInfo, db As EwEUtils.Database.cEwEDatabase, ByRef strLogfileName As String) As Boolean _
        Implements EwEUtils.Database.IModelImporter.Import
        Return False
    End Function

    Public Function Models() As EwEUtils.Database.cExternalModelInfo() _
        Implements EwEUtils.Database.IModelImporter.Models
        Return New cExternalModelInfo() {}
    End Function

#End Region ' Database plug-in implementation

#Region " Disposal plug-in implementation "

    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose
        Me.Close()
    End Sub

#End Region ' Disposal plug-in implementation

End Class
