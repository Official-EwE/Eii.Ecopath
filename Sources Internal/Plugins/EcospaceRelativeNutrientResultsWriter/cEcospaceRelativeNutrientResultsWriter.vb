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
Imports EwEUtils.Core
Imports System.IO
Imports EwECore
Imports EwEPlugin
Imports EwEUtils

#End Region ' Imports

Public Class cEcospaceRelativeNutrientResultsWriter
    Implements IEcospaceResultWriterPlugin

    Private _core As EwECore.cCore
    Private _filename As String
    Private _bEnabled As Boolean
    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "Relative Nutrients" 'Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return IResultsWriter_DisplayName
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Save Ecospace relative nutrients to .txyz file."
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Greig Oldford, Joe Buszowski"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public Property Enabled As Boolean Implements IResultsWriter.Enabled
        Get
            Return _bEnabled
        End Get
        Set(value As Boolean)
            _bEnabled = value
            _core.m_EcoSpaceData.bSaveRelNutFile = value
        End Set
    End Property

    Public ReadOnly Property OutputPath As String Implements IResultsWriter.OutputPath
        Get
            Return ""
        End Get
    End Property

    Private ReadOnly Property IResultsWriter_DisplayName As String Implements IResultsWriter.DisplayName
        Get
            Return "Relative Nutrient (*.txyz) file"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

        Try
            _core = DirectCast(core, EwECore.cCore)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub WriteResults(SpaceTimeStepResults As Object) Implements IEcospaceResultsWriter.WriteResults

        Try

            If Not _core.m_EcoSpaceData.bSaveRelNutFile Then
                Exit Sub
            End If

            Dim t As Integer = DirectCast(SpaceTimeStepResults, cEcospaceTimestep).iTimeStep
            Dim strm As StreamWriter = New StreamWriter(_filename, True)
            Dim delim As String = ","
            For irow As Integer = 1 To _core.m_EcoSpaceData.InRow
                For icol As Integer = 1 To _core.m_EcoSpaceData.InCol
                    If _core.m_EcoSpaceData.Depth(irow, icol) > 0.0F Then
                        strm.WriteLine(t.ToString + delim + icol.ToString + delim + irow.ToString + delim + EwEUtils.Utilities.cStringUtils.ToCSVField(_core.m_EcoSpaceData.RelNutMult(irow, icol)))
                    End If
                Next
            Next

            strm.Close()

        Catch ex As Exception

        End Try

    End Sub

    Public Sub Init(theCore As Object) Implements IResultsWriter.Init

        Try
            _core = DirectCast(theCore, EwECore.cCore)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub StartWrite() Implements IResultsWriter.StartWrite
        Try

            If Not _core.m_EcoSpaceData.bSaveRelNutFile Then
                Exit Sub
            End If

            _filename = System.IO.Path.Combine(_core.DefaultOutputPath(eAutosaveTypes.EcospaceResults), "Ecospace_RelativeNutrient.txyz")
            If File.Exists(_filename) Then
                File.Delete(_filename)
            End If

            Dim strm As StreamWriter = New StreamWriter(_filename)
            strm.WriteLine(Me._core.DefaultFileHeader(eAutosaveTypes.Ecospace))
            strm.WriteLine("Time_Step,X(Column),Y(Row),Relative_Nutrient")
            strm.Close()

        Catch ex As Exception

        End Try

    End Sub

    Public Sub EndWrite() Implements IResultsWriter.EndWrite

    End Sub
End Class
