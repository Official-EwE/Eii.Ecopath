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


    Public MasterPlanYear As String = "MP2017"
    Public Scenario As String = "S01"
    Public GroupID As String = "G001"
    Public CLARA As String = "C000"
    Public Uncertainty As String = "U00"
    Public Variance As String = "V00"
    Public Region As String = "SLA"
    Public FileType As String = "O"
    Public TimeSteps As String = "01-50"


#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

#End Region

#Region "Public Methods and properties"



#End Region

#Region "Private methods"

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

    Public ReadOnly Property EcoPathData As cEcopathDataStructures
        Get
            Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathData() Ecopath has not been initialized correctly.")
            Return Me.m_EcoPathData
        End Get
    End Property

    Public ReadOnly Property EcoSimData As cEcosimDatastructures
        Get
            Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcoSimData() EcoSim has not been initialized correctly.")
            Return Me.m_EcoSimData
        End Get
    End Property

    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcoSpaceData() EcoSpace has not been initialized correctly.")
            Return Me.m_EcoSpaceData
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


    Public Function ModelAreaFileName(ByRef FileName As String, DataSourceAsObject As Object, AvgType As EwEUtils.Core.eEcospaceResultsAverageType) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.ModelAreaFileName
        Try
            Dim delim As String = "_"
            Dim ds As cEcospaceResultsWriterDataSourceBase = DirectCast(DataSourceAsObject, cEcospaceResultsWriterDataSourceBase)

            Dim strPeriod As String
            Select Case AvgType
                Case eEcospaceResultsAverageType.Annual
                    strPeriod = "A"
                Case eEcospaceResultsAverageType.TimeStep
                    strPeriod = "M"
            End Select

            Dim strFNAbbrev As String = ds.FileNameAbbreviation + strPeriod

            FileName = Me.MasterPlanYear + delim + Me.Scenario + delim + Me.GroupID + delim + Me.CLARA + delim + _
                        Me.Uncertainty + delim + Me.Variance + delim + Me.Region + delim + Me.FileType + delim + _
                        TimeSteps + delim + "E" + delim + strFNAbbrev + ".csv"

        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Function MapFleetFileName(ByRef FileName As String, varname As EwEUtils.Core.eVarNameFlags,
                                     iFlt As Integer, strExt As String, iModelTimeStep As Integer) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.MapFleetFileName
        'FileName = "Fleet_" + iFlt.ToString + "_" + iModelTimeStep.ToString + strExt
        Return False
    End Function

    Public Function MapGroupFileName(ByRef FileName As String, varname As EwEUtils.Core.eVarNameFlags,
                                     iGrp As Integer, strExt As String, iModelTimeStep As Integer) As Boolean Implements EwEPlugin.IEcospaceResultWriterUtils.MapGroupFileName

        'FileName = "Group_" + iGrp.ToString + "_" + iModelTimeStep.ToString + strExt
        Return False

    End Function
End Class

