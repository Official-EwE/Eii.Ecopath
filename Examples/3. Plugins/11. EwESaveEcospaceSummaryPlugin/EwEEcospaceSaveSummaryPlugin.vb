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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' This plug-in provides another type of Ecospace output format type, and shows up
''' in the EwE scientific interface, Ecospace parameters form.
''' </summary>
''' <remarks>
''' You can use this class without the EwE interface: just add an instance to the 
''' list of Ecospace output writers prior to running Ecospace.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class EwEEcospaceSaveSummaryPlugin
    Implements IEcospaceResultWriterPlugin
    Implements IEcospaceInitializedPlugin

    Private m_core As cCore = Nothing
    Private m_data As cEcospaceDataStructures = Nothing

    Private m_bWriting As Boolean = False

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Example plug-in that writes Ecospace summaries across a run to CSV files."
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = CType(core, cCore)
    End Sub

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "mapaverages_csv"
        End Get
    End Property

    Public Sub Init(theCore As Object) _
        Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        ' NOP
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized
        Try
            Me.m_data = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
        Catch ex As Exception
            ' Kaboom
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Public Sub StartWrite() _
        Implements EwEUtils.Core.IEcospaceResultsWriter.StartWrite

        Me.m_bWriting = False

        If (Me.m_data IsNot Nothing) Then
            Me.m_bWriting = (cFileUtils.IsDirectoryAvailable(Me.DataPath(), True))
        End If

    End Sub

    Public Sub WriteResults(SpaceTimeStepResults As Object) _
        Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults
        ' NOP
    End Sub

    Public Sub EndWrite() _
        Implements EwEUtils.Core.IEcospaceResultsWriter.EndWrite

        If (Not Me.m_bWriting) Then Return

        ' ToDo: globalize this method

        Dim strPath As String = Me.DataPath()
        Dim msg As cMessage = Nothing

        If Me.SaveFile(Path.Combine(strPath, "ts_absolute_biomass.csv"), eSpaceResultsGroups.Biomass) And _
           Me.SaveFile(Path.Combine(strPath, "ts_relative_biomass.csv"), eSpaceResultsGroups.RelativeBiomass) And _
           Me.SaveFile(Path.Combine(strPath, "ts_catch.csv"), eSpaceResultsGroups.CatchBio) And _
           Me.SaveFile(Path.Combine(strPath, "ts_consumption.csv"), eSpaceResultsGroups.ConsumpRate) And _
           Me.SaveFile(Path.Combine(strPath, "ts_fishing_mortality.csv"), eSpaceResultsGroups.FishingMort) Then

            msg = New cMessage(cStringUtils.Localize("Ecospace map averates have been saved to {0}", strPath), _
                               eMessageType.DataExport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
            msg.Hyperlink = strPath
        Else
            msg = New cMessage(cStringUtils.Localize("Ecospace map averates failed to save to {0}", strPath), _
                               eMessageType.DataExport, eCoreComponentType.EcoSpace, eMessageImportance.Critical)
        End If

        Me.m_core.Messages.SendMessage(msg)

    End Sub

    Private Function SaveFile(strFile As String, result As eSpaceResultsGroups) As Boolean

        Try

            Dim writer As New StreamWriter(strFile)

            ' EwE header
            If (Me.m_core.SaveWithFileHeader) Then
                writer.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
            End If

            ' Header lines
            writer.Write("group")
            For iGroup As Integer = 1 To Me.m_core.nGroups
                Dim grp As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
                writer.Write(",")
                writer.Write(cStringUtils.ToCSVField(grp.Name))
            Next
            writer.WriteLine()

            writer.Write("poolcode")
            For iGroup As Integer = 1 To Me.m_core.nGroups
                writer.Write(",")
                writer.Write(cStringUtils.ToCSVField(iGroup))
            Next
            writer.WriteLine()

            For iTime As Integer = 1 To Me.m_core.nEcospaceTimeSteps
                writer.Write(cStringUtils.ToCSVField(iTime))
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    writer.Write(",")
                    writer.Write(cStringUtils.ToCSVField(Me.m_data.ResultsByGroup(result, iGroup, iTime)))
                Next
                writer.WriteLine()
            Next

            writer.Flush()
            writer.Close()

            Return True

        Catch ex As Exception
            cLog.Write(ex, "EwESaveSummaryPlugin.SaveFile(" & strFile & ")")
        End Try

        Return False

    End Function

    Public ReadOnly Property DataName As String _
        Implements EwEUtils.Core.IEcospaceResultsWriter.DataName, IEcospaceResultWriterPlugin.DisplayName
        Get
            ' ToDo: globalize this method
            Return "Ecospace map averages (csv file)"
        End Get
    End Property

    Private Function DataPath() As String
        Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), "summary")
    End Function

    Public Property Enabled As Boolean Implements EwEUtils.Core.IEcospaceResultsWriter.Enabled


    Public Property FirstOutputTimeStep As Integer Implements EwEUtils.Core.IEcospaceResultsWriter.FirstOutputTimeStep

End Class
