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
    Protected Overridable Sub CreateTimeStampedDir()

        m_TimeStampDirName = Path.Combine(Me.m_core.OutputPath, Path.GetDirectoryName(Me.m_core.EcospaceOutputFileLocation(bIncludeTime:=True)) & " " & Me.getSubDirName())

        If (Not cFileUtils.IsDirectoryAvailable(Me.TimeStampDirName, True)) Then
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() cannot create directory")
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
        Return Format(Date.Now, "y-MM-dd HH-mm-ss")
    End Function


    ''' <summary>
    ''' Full path name of the current time stamped output directory
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Initialized by <see cref="CreateTimeStampedDir">CreateTimeStampedDir()</see></remarks>
    Protected Overridable ReadOnly Property TimeStampDirName() As String
        Get
            Return Me.m_TimeStampDirName
        End Get
    End Property

    ''' <summary>
    ''' Convert the variable, group, extention and model time step into a valid file name
    ''' </summary>
    ''' <param name="varname">Variable i.e. Biomass</param>
    ''' <param name="iGrp">Index of the group</param>
    ''' <param name="Ext">Extention of the file</param>
    ''' <param name="ModelTimeStep">Time step for the current file. If this is not supplied then no time stamp will appear in the filename </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function getFileName(ByVal varname As eVarNameFlags, ByVal iGrp As Integer, ByVal Ext As String, Optional ByRef ModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim Timestep As String = ""

        'Is there a time step in the file name
        If ModelTimeStep <> cCore.NULL_VALUE Then
            'Yes so include it in the file name
            Timestep = "-" & ModelTimeStep.ToString
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(varname.ToString() & "-" & grpName & Timestep & "." & Ext, False)
        Return System.IO.Path.Combine(Me.TimeStampDirName, fn)

    End Function


    ''' <summary>
    ''' Ecopath data structure
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
