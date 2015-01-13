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
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Factory class for creating an <see cref="IEcospaceResultsWriter"/>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceResultWriterFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory method.
    ''' </summary>
    ''' <param name="strExt">The file extension to find a writer for.</param>
    ''' <returns>A <see cref="IEcospaceResultsWriter"/> instance, or Nothing if
    ''' no writer could be found for the provided extension.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetWriter(ByVal strExt As String, _
                                     ByVal pm As cPluginManager) As IEcospaceResultsWriter

        Select Case strExt.ToLower
            Case ".csv" : Return New cEcospaceCSVMapResultsWriter()
            Case ".asc" : Return New cEcospaceASCMapResultsWriter()
        End Select

        ' Plug-in manager provided?
        If (pm IsNot Nothing) Then
            ' #Yes: see if a plug-in based writer supports the requested format
            For Each ip As IEcospaceResultWriterPlugin In pm.GetPlugins(GetType(IEcospaceResultWriterPlugin))
                ' Does plug-in support this format?
                If (String.Compare(strExt, ip.FileExtension, True) = 0) Then
                    ' #Yes: use it
                    Return ip
                End If
            Next
        End If

        Return Nothing

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

#End Region ' Protected data

#Region " Constructor "

    Public Sub New()
        ' NOP
    End Sub

#End Region ' Constructor

#Region " IEcospaceResultsWriter implementation "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceResultsWriter.Init"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Init(ByVal theCore As Object) _
        Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        Me.m_core = DirectCast(theCore, cCore)
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
    ''' <inheritdocs cref="IEcospaceResultsWriter.FileExtension"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function FileExtension() As String _
        Implements IEcospaceResultsWriter.FileExtension

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
    Protected Overridable Sub CreateOutputDir()

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
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() cannot create directory")
            cLog.Write("Ecospace output writer failed to create directory " & Me.OutputDirectory)
        End If

    End Sub

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
    Protected Overridable Function GetGroupFileName(ByVal varname As eVarNameFlags, _
                                                    ByVal iGrp As Integer, _
                                                    ByVal strExt As String, _
                                                    Optional ByVal iModelTimeStep As Integer = cCore.NULL_VALUE) As String _
                                                    Implements EwEUtils.Core.IEcospaceResultsWriter.GetGroupFileName

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim strTimestep As String = ""

        ' Is there a time step in the file name?
        If (iModelTimeStep > 0) Then
            ' #Yes: include it in the file name
            strTimestep = cStringUtils.Localize("-{0:00000}", iModelTimeStep)
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(cStringUtils.Localize("{0}-{1}{2}.{3}", _
                                                                                       cin.GetVarName(varname), grpName, strTimestep, strExt.Replace(".", "")), _
                                                                         False)
        Return System.IO.Path.Combine(Me.OutputDirectory, fn.Replace("..", "."))

    End Function

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
    Protected Overridable Function GetFleetFileName(ByVal varname As eVarNameFlags, _
                                                    ByVal iFlt As Integer, _
                                                    ByVal strExt As String, _
                                                    Optional ByVal iModelTimeStep As Integer = cCore.NULL_VALUE) As String _
                                                    Implements EwEUtils.Core.IEcospaceResultsWriter.GetFleetFileName

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim fltName As String = Me.m_core.m_EcoPathData.FleetName(iFlt)
        Dim strTimestep As String = ""

        ' Is there a time step in the file name?
        If (iModelTimeStep > 0) Then
            ' #Yes: include it in the file name
            strTimestep = cStringUtils.Localize("-{0:00000}", iModelTimeStep)
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(cStringUtils.Localize("{0}-{1}{2}.{3}", _
                                                                                       cin.GetVarName(varname), fltName, strTimestep, strExt.Replace(".", "")), _
                                                                         False)
        Return System.IO.Path.Combine(Me.OutputDirectory, fn.Replace("..", "."))

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
    Protected Overridable Function ScaleValue(ByVal value As Double, _
                                              ByVal SpaceTSData As cEcospaceTimestep, _
                                              ByVal iIndex As Integer, _
                                              ByVal varname As eVarNameFlags) As Double
        Return value
    End Function


    Public ReadOnly Property OutputPath As String Implements EwEUtils.Core.IEcospaceResultsWriter.OutputPath
        Get
            Return Me.m_OutputPath
        End Get
    End Property

#End Region ' Internals

End Class
