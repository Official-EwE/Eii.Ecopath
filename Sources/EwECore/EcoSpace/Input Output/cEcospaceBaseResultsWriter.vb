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
#Region "Import"

Option Strict On
Imports System.IO
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region


''' <summary>
''' Base implementation of <see cref="EwEUtils.Core.IEcospaceResultsWriter">IEcospaceResultsWriter</see>
''' </summary>
''' <remarks>Provides directory creation and file naming functionality for derived classes</remarks>
Public MustInherit Class cEcospaceBaseResultsWriter
    Implements EwEUtils.Core.IEcospaceResultsWriter

    Enum eSpaceOutputType
        NA
        ASC
        CSV
    End Enum

#Region "Protected data "

    Protected m_core As cCore
    Protected m_TimeStampDirName As String

    Public Sub New()
    End Sub

#End Region

#Region "IEcospaceResultsWriter Interfaces"

    Public MustOverride Sub WriteResults(ByVal SpaceTimeStepResults As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults

    Public MustOverride Sub EndWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.EndWrite

    Public MustOverride Sub StartWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.StartWrite

#End Region

#Region "MustOverride and Overridable methods of cEcospaceBaseResultsWriter "

    Public Overridable Sub Init(ByVal theCore As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        Me.m_core = DirectCast(theCore, cCore)
    End Sub

    ''' <summary>
    ''' Implementation must provide an OutputType
    ''' </summary>
    ''' <remarks>Used by <see cref="getSubDirName">getSubDirName()</see> to build the output directory by output type</remarks>
    Protected MustOverride ReadOnly Property OuputType() As eSpaceOutputType

#End Region

#Region "Protected methods"


    ''' <summary>
    ''' Create the time stamped ouput directory
    ''' </summary>
    ''' <remarks>
    ''' Directory will be created on the default output path in the format "Ecopace {datatype} {y-m-d h-m-s}
    ''' i.e. "Ecospace ASC 11-07-11 16-40-50" </remarks>
    Protected Overridable Sub CreateOutputDir(Optional bIncludeTime As Boolean = False)

        If Me.m_core.m_EcoSpaceData.bUseCoreOuputDir Then
            m_TimeStampDirName = Path.Combine(Me.m_core.OutputPath, Path.GetDirectoryName(Me.m_core.EcospaceOutputFileLocation(bIncludeTime:=bIncludeTime)) & " " & Me.getSubDirName())
        Else
            'Use the output directroy set by the user
            m_TimeStampDirName = Me.m_core.OutputPath ' 
        End If

        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() cannot create directory")
            cLog.Write("Ecospace output writer failed to create directory " & Me.OutputDirectory)
        End If

    End Sub

    ''' <summary>
    ''' Turn the OuputType into a string that can be used in the output directory name
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function getSubDirName() As String

        Select Case Me.OuputType
            Case eSpaceOutputType.NA
                Return ""
            Case eSpaceOutputType.ASC
                Return "ASC"
            Case eSpaceOutputType.CSV
                Return "CSV"
        End Select
        Return ""

    End Function

    ''' <summary>
    ''' Get the current time as a string to be used in the ouput directory name
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>format year-month-day hour-minute-second</remarks>
    Protected Overridable Function getTimeStamp() As String
        Return Date.Now.ToString("y-MM-dd HH-mm-ss")
    End Function

    ''' <summary>
    ''' Full path name of the current output directory
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Initialized by <see cref="CreateOutputDir"/></remarks>
    Protected Overridable ReadOnly Property OutputDirectory() As String
        Get
            Return Me.m_TimeStampDirName
        End Get
    End Property

    ''' <summary>
    ''' Convert the variable, group index, extention and model time step into a valid file name
    ''' </summary>
    ''' <param name="varname">Variable i.e. Biomass</param>
    ''' <param name="iGrp">Index of the group</param>
    ''' <param name="Ext">Extention of the file</param>
    ''' <param name="ModelTimeStep">Time step for the current file. If this is not supplied then no time stamp will appear in the filename </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function getGroupFileName(ByVal varname As eVarNameFlags, ByVal iGrp As Integer, ByVal Ext As String, Optional ByRef ModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim Timestep As String = ""

        'Is there a time step in the file name
        If ModelTimeStep <> cCore.NULL_VALUE Then
            'Yes so include it in the file name
            Timestep = String.Format("-{0:00000}", ModelTimeStep)
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(String.Format("{0}-{1}{2}.{3}", cin.GetVarName(varname), grpName, Timestep, Ext), False)
        Return System.IO.Path.Combine(Me.OutputDirectory, fn)

    End Function

    ''' <summary>
    ''' Convert the variable, fleet index, extention and model time step into a valid file name
    ''' </summary>
    ''' <param name="varname">Variable i.e. Biomass</param>
    ''' <param name="iFlt">Index of the fleet</param>
    ''' <param name="Ext">Extention of the file</param>
    ''' <param name="ModelTimeStep">Time step for the current file. If this is not supplied then no time stamp will appear in the filename </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function getFleetFileName(ByVal varname As eVarNameFlags, ByVal iFlt As Integer, ByVal Ext As String, Optional ByRef ModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim fltName As String = Me.m_core.m_EcoPathData.FleetName(iFlt)
        Dim Timestep As String = ""

        'Is there a time step in the file name
        If ModelTimeStep <> cCore.NULL_VALUE Then
            'Yes so include it in the file name
            Timestep = String.Format("-{0:00000}", ModelTimeStep)
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(String.Format("{0}-{1}{2}.{3}", cin.GetVarName(varname), fltName, Timestep, Ext), False)
        Return System.IO.Path.Combine(Me.OutputDirectory, fn)

    End Function


    ''' <summary>
    ''' Ecopath data structure
    ''' </summary>
    Protected ReadOnly Property PathData() As cEcopathDataStructures
        Get
            Return Me.m_core.m_EcoPathData
        End Get
    End Property


    ''' <summary>
    ''' Ecospace data structure
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property SpaceData() As cEcospaceDataStructures
        Get
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

#End Region

End Class
