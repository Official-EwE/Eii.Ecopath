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
Imports System.IO
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Reflection

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Factory class for creating an <see cref="IEcospaceResultsWriter"/>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceResultWriterFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all the Ecospace result writers provided by the EwE core and plug-ins
    ''' </summary>
    ''' <param name="pm">The plug-in manager instance to consult, if any.</param>
    ''' <returns>An array of all avaliable result writers.</returns>
    ''' -----------------------------------------------------------------------
    Friend Shared Function GetWriters(ByVal pm As cPluginManager) As IEcospaceResultsWriter()

        Dim writers As New List(Of IEcospaceResultsWriter)

        Try

            For Each t As Type In Assembly.GetAssembly(GetType(cCore)).GetTypes()
                If (GetType(IEcospaceResultsWriter).IsAssignableFrom(t) And Not t.IsAbstract()) Then
                    writers.Add(CType(Activator.CreateInstance(t), IEcospaceResultsWriter))
                End If
            Next

            ' Plug-in manager provided?
            If (pm IsNot Nothing) Then
                ' #Yes: see if a plug-in based writer supports the requested format
                For Each ip As IEcospaceResultWriterPlugin In pm.GetPlugins(GetType(IEcospaceResultWriterPlugin))
                    writers.Add(ip)
                Next
            End If

        Catch ex As Exception
            cLog.Write(ex, "cEcospaceResultWriterFactory.GetWriters()")
        End Try

        Return writers.ToArray()

    End Function


End Class

''' ---------------------------------------------------------------------------
''' <summary>
''' Base implementation of <see cref="EwEUtils.Core.IEcospaceResultsWriter">IEcospaceResultsWriter</see>
''' </summary>
''' <remarks>Provides directory creation and file naming functionality for derived classes</remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cEcospaceBaseResultsWriter
    Implements EwEUtils.Core.IEcospaceResultsWriter

#Region " Protected data "

    ''' <summary>Zhe core.</summary>
    Protected m_core As cCore = Nothing
    ''' <summary>The complete path to the directory containing result files.</summary>
    Protected m_OutputPath As String

    Protected vars() As eVarNameFlags

    Protected m_selGroups() As Boolean

#End Region ' Protected data

#Region " Constructor "

    Public Sub New()
        Me.FirstOutputTimeStep = 1
    End Sub

#End Region ' Constructor

#Region " IEcospaceResultsWriter implementation "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.Init"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Init(ByVal theCore As Object) _
        Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        Me.m_core = DirectCast(theCore, cCore)

        Me.FirstOutputTimeStep = Me.m_core.m_EcoSpaceData.FirstOutputTimeStep

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.StartWrite"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub StartWrite() _
        Implements EwEUtils.Core.IEcospaceResultsWriter.StartWrite

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.WriteResults"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub WriteResults(ByVal SpaceTimeStepResults As Object) _
        Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.EndWrite"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub EndWrite() _
        Implements EwEUtils.Core.IEcospaceResultsWriter.EndWrite

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property DisplayName() As String _
        Implements IEcospaceResultsWriter.DisplayName

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.DataName"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property DataName() As String _
        Implements IEcospaceResultsWriter.DataName

    <Obsolete("Since the file extension is no longer crucial for identifying writers, this method should go")>
    Public MustOverride Function FileExtension() As String

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.Enabled"/>
    ''' -----------------------------------------------------------------------  
    Public Property Enabled As Boolean Implements EwEUtils.Core.IEcospaceResultsWriter.Enabled

#End Region ' IEcospaceResultsWriter implementation

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the time stamped ouput directory.
    ''' </summary>
    ''' <remarks>
    ''' Directory will be created on the default output path in the format "Ecopace {datatype} {y-m-d h-m-s}
    ''' i.e. "Ecospace ASC 11-07-11 16-40-50".</remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function CreateOutputDir() As Boolean

        If Me.m_core.m_EcoSpaceData.UseCoreOutputDir Then
            ' Write to "Ecospace output dir\ext\"
            Dim iStr As String = Me.FileExtension()
            iStr = cStringUtils.ReplaceAll(iStr, ".", "")
            Me.m_OutputPath = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), iStr)
        Else
            'Use the output directory set by the user
            If String.IsNullOrWhiteSpace(Me.EcospaceData.EcospaceMapOutputDir) Then
                Me.m_OutputPath = Me.m_core.OutputPath
            Else
                Me.m_OutputPath = Path.Combine(Me.m_core.OutputPath, Me.EcospaceData.EcospaceMapOutputDir)
            End If
        End If

        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then
            Debug.Assert(False, Me.ToString & ".CreateOutputDir() cannot create directory")
            cLog.Write("Ecospace output writer failed to create directory " & Me.OutputDirectory)
            Return False
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the full path name of the current output directory.
    ''' </summary>
    ''' <remarks>Initialized by <see cref="CreateOutputDir"/>.</remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable ReadOnly Property OutputDirectory() As String
        Get
            Return Me.m_OutputPath
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert the variable, group index, extention and model time step into a 
    ''' valid group-based file name.
    ''' </summary>
    ''' <param name="varname">Variable, i.e. Biomass.</param>
    ''' <param name="iGrp">Index of the group.</param>
    ''' <param name="strExt">Extention of the file.</param>
    ''' <param name="iModelTimeStep">Time step for the current file. If this is 
    ''' not supplied then no time stamp will appear in the filename.</param>
    ''' <returns>A file name.</returns>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function GetGroupFileName(ByVal varname As eVarNameFlags,
                                                    ByVal iGrp As Integer,
                                                    ByVal strExt As String,
                                                    Optional ByVal iModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim Filename As String
        If Me.m_core.PluginManager.EcospaceResultsMapGroupFileName(Filename, varname, iGrp, strExt, iModelTimeStep) Then
            'File was set by the plugin
            'System.Console.WriteLine("Plugin Filename = " + Filename)

        Else
            'Ok Use the default filename
            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
            Dim strTimestep As String = ""

            ' Is there a time step in the file name?
            If (iModelTimeStep > 0) Then
                ' #Yes: include it in the file name
                strTimestep = cStringUtils.Localize("-{0:00000}", iModelTimeStep)
            End If

            Filename = EwEUtils.Utilities.cFileUtils.ToValidFileName(cStringUtils.Localize("{0}-{1}{2}.{3}",
                                                                        cin.GetVarName(varname), grpName, strTimestep, strExt.Replace(".", "")), False)
        End If

        Return System.IO.Path.Combine(Me.OutputDirectory, Filename.Replace("..", "."))

    End Function

    Protected Sub setAllGroupsSelected()
        Me.m_selGroups = New Boolean(Me.m_core.nGroups) {}
        For igrp As Integer = 1 To Me.EcopathData.NumGroups
            m_selGroups(igrp) = True
        Next igrp

    End Sub

    Protected Sub setCatchSelected()
        Me.m_selGroups = New Boolean(Me.m_core.nGroups) {}
        For igrp As Integer = 1 To Me.EcopathData.NumGroups
            For iflt As Integer = 1 To Me.EcopathData.NumFleet
                If (Me.EcopathData.Discard(iflt, igrp) + Me.EcopathData.Landing(iflt, igrp)) > 0 Then
                    m_selGroups(igrp) = True
                End If
            Next iflt
        Next igrp
    End Sub

    Protected Sub setAllFleetsSelected()
        Me.m_selGroups = New Boolean(Me.m_core.nGroups) {}
        For iflt As Integer = 1 To Me.EcopathData.NumFleet
            m_selGroups(iflt) = True
        Next iflt
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert the variable, fleet index, extention and model time step into a 
    ''' valid fleet-based file name.
    ''' </summary>
    ''' <param name="varname">Variable, i.e. Biomass.</param>
    ''' <param name="iFlt">Index of the fleet.</param>
    ''' <param name="strExt">Extention of the file WITHOUT a period.</param>
    ''' <param name="iModelTimeStep">Time step for the current file. If this is 
    ''' not supplied then no time stamp will appear in the filename.</param>
    ''' <returns>A file name.</returns>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function GetFleetFileName(ByVal varname As eVarNameFlags,
                                                    ByVal iFlt As Integer,
                                                    ByVal strExt As String,
                                                    Optional ByVal iModelTimeStep As Integer = cCore.NULL_VALUE) As String


        Dim Filename As String
        If Me.m_core.PluginManager.EcospaceResultsMapFleetFileName(Filename, varname, iFlt, strExt, iModelTimeStep) Then
            'File was set by the plugin
        Else

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim fltName As String = Me.m_core.m_EcoPathData.FleetName(iFlt)
            Dim strTimestep As String = ""

            ' Is there a time step in the file name?
            If (iModelTimeStep > 0) Then
                ' #Yes: include it in the file name
                strTimestep = cStringUtils.Localize("-{0:00000}", iModelTimeStep)
            End If

            Filename = EwEUtils.Utilities.cFileUtils.ToValidFileName(cStringUtils.Localize("{0}-{1}{2}.{3}",
                                                                     cin.GetVarName(varname), fltName, strTimestep, strExt.Replace(".", "")), False)
        End If

        Return System.IO.Path.Combine(Me.OutputDirectory, Filename.Replace("..", "."))

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cEcopathDataStructures">Ecopath data structure</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property EcopathData() As cEcopathDataStructures
        Get
            Return Me.m_core.m_EcoPathData
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cEcospaceDataStructures">Ecospace data structures</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property EcospaceData() As cEcospaceDataStructures
        Get
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

    Protected Sub WriteRunInfo(ByVal strm As StreamWriter)
        strm.Write(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
    End Sub

    ''' <summary>
    ''' Recalculate / rescale a value before it is written to the 
    ''' output file.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="SpaceTSData"></param>
    ''' <param name="iIndex"></param>
    ''' <param name="varname"></param>
    ''' <returns></returns>
    Protected Overridable Function ScaleValue(ByVal value As Double,
                                              ByVal SpaceTSData As cEcospaceTimestep,
                                              ByVal iIndex As Integer,
                                              ByVal varname As eVarNameFlags) As Double
        Return value
    End Function

    Public ReadOnly Property OutputPath As String
        Get
            Return Me.m_OutputPath
        End Get
    End Property


    Public Overridable Property FirstOutputTimeStep As Integer Implements EwEUtils.Core.IEcospaceResultsWriter.FirstOutputTimeStep

    Public Overridable Property SelectedGroups As Boolean() Implements IEcospaceResultsWriter.SelectedGroups
        Get
            Return Me.m_selGroups
        End Get
        Set(value() As Boolean)
            Me.m_selGroups = value
        End Set
    End Property

#End Region ' Internals

End Class
