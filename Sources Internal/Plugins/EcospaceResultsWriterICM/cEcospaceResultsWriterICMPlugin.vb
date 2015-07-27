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
'Imports System.IO
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities


#End Region

''' <summary>
''' Base code that can be used as a template to create a new plug-in.
''' </summary>
''' <remarks>
''' <para>This plugin responds to:</para>
''' <list type="bullet">
''' <item><description>loading a model,</description>></item>
''' <item><description>saving a model,</description>></item>
''' <item><description>closing a model,</description>></item>
''' <item><description>initialization of the Core,</description>></item>
''' <item><description>initialization of Ecopath,</description>></item>
''' <item><description>initialization of Ecosim,</description>></item>
''' <item><description>initialization of Ecospace.</description>></item>
''' </list>
''' <para>In order to run and test this plugin it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
''' 
Public Class cEcospaceResultsWriterICMPlugin
    Implements EwEPlugin.IPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcospaceResultWriterUtils

    'ToDo sort out the region naming. Have the CHR regions been combinded in the Ecospace regions map?
    'ToDo some basic bounds checking for ngroups and nregions.
    '   Maybe during the Init() then set a flag and bark if it fails????

#Region "Public Variables"

    Public FileNamePreFix As String
    Public MasterPlanYear As String = "MP2017"

#End Region

#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private SpCodes() As String
    Private RegionCodes() As String
    Private dctTypeCodes As Dictionary(Of eVarNameFlags, String)
    Private delim As String

#End Region

#Region "File Naming "

#Region "Implementation of Plugin Points"


    Public Function ModelAreaFileName(ByRef FileName As String, DataSourceAsObject As Object, _
                                      AvgType As EwEUtils.Core.eEcospaceResultsAverageType) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.ModelAreaFileName
        Try

            Dim ds As cEcospaceResultsWriterDataSourceBase = DirectCast(DataSourceAsObject, cEcospaceResultsWriterDataSourceBase)
            Dim nYears As Integer = Me.EcoSpaceData.nTimeSteps \ Me.EcoSpaceData.nTimeStepsPerYear
            FileName = Me.ToPrefix() + delim + ToRegionFileName(ds, AvgType) + delim + ToFormattedNumber(1) + "-" + ToFormattedNumber(nYears) + ".csv"

        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Function MapFleetFileName(ByRef FileName As String, varname As EwEUtils.Core.eVarNameFlags,
                                     iFlt As Integer, strExt As String, iModelTimeStep As Integer) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.MapFleetFileName
        FileName = Me.ToPrefix() + delim + ToDataTypeCode(varname) + ToFormattedNumber(iFlt) + ToModelMonth(iModelTimeStep) + delim + ToModelYear(iModelTimeStep) + ".asc"
        Return True
    End Function

    Public Function MapGroupFileName(ByRef FileName As String, varname As EwEUtils.Core.eVarNameFlags,
                                     iGrp As Integer, strExt As String, iModelTimeStep As Integer) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.MapGroupFileName

        FileName = Me.ToPrefix() + delim + ToVarCode(varname, iGrp, iModelTimeStep) + delim + ToModelYear(iModelTimeStep) + ".asc"
        Return True

    End Function

#End Region

#Region "Private methods"


    Private Function ToPrefix() As String
        Return Me.MasterPlanYear + delim + FileNamePreFix
    End Function

    Private Function ToVarCode(varname As eVarNameFlags, iGrp As Integer, iModelTimestep As Integer) As String
        Return ToDataTypeCode(varname) + ToSpCode(iGrp) + ToModelMonth(iModelTimestep)
    End Function

    Private Function ToModelMonth(iModelTimeStep As Integer) As String
        Return ToFormattedNumber(Me.EcoSpaceData.MonthNow)
    End Function

    Private Function ToDataTypeCode(varname As eVarNameFlags) As String
        Return Me.dctTypeCodes.Item(varname)
    End Function

    Private Function ToSpCode(iGrp As Integer) As String
        Return SpCodes(iGrp)
    End Function

    Private Function ToModelYear(iModelTimestep As Integer) As String
        Return ToFormattedNumber(Me.EcoSpaceData.YearNow) + "-" + ToFormattedNumber(Me.EcoSpaceData.YearNow)
    End Function

    Private Function ToFormattedNumber(iTime As Integer) As String
        Dim tmpStr As String = iTime.ToString
        If tmpStr.Length < 2 Then
            tmpStr = "0" + tmpStr
        End If
        Return tmpStr
    End Function

    Private Function ToRegionFileName(ds As cEcospaceResultsWriterDataSourceBase, AvgType As EwEUtils.Core.eEcospaceResultsAverageType) As String
        Return ToDataTypeCode(ds) + toAvgTypeCode(AvgType) + toRegionCode(ds)
    End Function

    Private Function toAvgTypeCode(avgType As eEcospaceResultsAverageType) As String
        Select Case avgType
            Case eEcospaceResultsAverageType.Annual
                Return "A"
            Case eEcospaceResultsAverageType.TimeStep
                Return "M"
        End Select
        Return "X"
    End Function

    Private Function ToDataTypeCode(ds As cEcospaceResultsWriterDataSourceBase) As String

        If TypeOf ds Is cBiomassResultsDataSource Then
            Return "B"
        ElseIf TypeOf ds Is cCatchResultsDataSource Then
            Return "C"
        ElseIf TypeOf ds Is cRegionBiomassResultsDataSource Then
            Return "B"
        ElseIf TypeOf ds Is cRegionCatchResultsDataSource Then
            Return "C"
        End If
        Debug.Assert(False, Me.ToString + ".ToDataTypeCode() Unsupported cEcospaceResultsWriterDataSourceBase.")
        Return "X"

    End Function


    Private Function toRegionCode(ds As cEcospaceResultsWriterDataSourceBase) As String
        Return RegionCodes(ds.AreaIndex)
    End Function


    Public Sub Init()
        SpCodes = New String() {"N/A", "JC", "AC", "JA", "AA", "BA", "BC", "JB", "AB", "JT", "AT", "JL", "AL", "JN", "AN", "DE", "DO", "GS", "JM", "AM", "JG", "AG", "KI", "JS", "AS", "MO", "OD", "SP", "SE", "SA", "PH", "JR", "AR", "SV", "BI", "JX", "AX", "JH", "AH", "JE", "AE", "SI", "JF", "AF", "JO", "AO", "JU", "AU", "JP", "AP", "JI", "AI", "JW", "AW", "ZB", "ZP"}
        RegionCodes = New String() {"TOT", "LAV", "LTB", "UBA", "BFD", "UPO", "LPO", "UTA", "MEL", "LBA", "BRE", "CAS", "CHR"}

        dctTypeCodes = New Dictionary(Of eVarNameFlags, String)
        dctTypeCodes.Add(eVarNameFlags.EcospaceMapBiomass, "B")
        dctTypeCodes.Add(eVarNameFlags.EcospaceMapCatch, "C")
        dctTypeCodes.Add(eVarNameFlags.EcospaceMapEffort, "E")

        delim = "_"

    End Sub

#End Region

#End Region

#Region "Message Logging"

    Public Sub LogMessage(ex As Exception, Optional msg As String = "")
        Try
            cLog.Write(ex, msg)
            Me.LogMessage(ex.Message)
        Catch x As Exception

        End Try
    End Sub

    Public Sub LogMessage(msg As String)
        Try
            System.Console.WriteLine(Me.ToString + " " + msg)
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "Ecopath, Ecosim and Ecospace events"

    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    Public Sub Initialize(CoreAsObject As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            m_core = DirectCast(CoreAsObject, cCore)
        Catch ex As Exception
            Me.LogMessage(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Plug-in point that is called when the core has initialized its models
    ''' Ecopath, Ecosim and Ecospace. This is the only opportunity for plug-ins to grab 
    ''' references to these models.
    ''' </summary>
    ''' <param name="EcopathAsObject"></param>
    ''' <param name="EcoSimAsObject"></param>
    ''' <param name="EcoSpaceAsObject"></param>
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Try

            m_EcoSpace = TryCast(EcoSpaceAsObject, cEcoSpace)

            Me.Init()

        Catch ex As Exception
            Me.LogMessage(ex)
        End Try

    End Sub

#End Region

#Region "Core, Ecopath, Ecosim and Ecospace Datastructures"

    Public ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".Core() EwE Core has not been initialized correctly.")
            Return Me.m_core
        End Get
    End Property

    'Public ReadOnly Property EcoPathData As cEcopathDataStructures
    '    Get
    '        Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathData() Ecopath has not been initialized correctly.")
    '        Return Me.m_EcoPathData
    '    End Get
    'End Property

    'Public ReadOnly Property EcoSimData As cEcosimDatastructures
    '    Get
    '        'Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcoSimData() EcoSim has not been initialized correctly.")
    '        Return Me.m_core.m
    '    End Get
    'End Property

    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            'Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcoSpaceData() EcoSpace has not been initialized correctly.")
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

#End Region

#Region " User Interface plug-in implementation "

#End Region ' User Interface plug-in implementation

#Region "IPlugin implementation"

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Me"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Overwrite Ecospace results file names for ICM Model."
        End Get
    End Property


    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ICM Ecospace Results Utilities"
        End Get
    End Property

#End Region

End Class

