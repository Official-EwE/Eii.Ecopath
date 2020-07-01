
Option Strict On
Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwEPlugin
Imports EwECore


Public Class NGOMEXResultsWriter
    Inherits cEcospaceASCBaseResultsWriter
    Implements EwEPlugin.IEcospaceResultWriterPlugin

    Private _lstGroups As List(Of Integer)
    Private _lstTimesteps As List(Of Integer)

    Private Const NGROUPS As Integer = 66

    Private _bIntialized As Boolean

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "ME"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ME"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Write results for NGOMEX"
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "NGOMEX Results"
        End Get
    End Property

    Public Overrides Property Enabled As Boolean Implements IResultsWriter.Enabled
        Get
            Return MyBase.Enabled
        End Get
        Set(value As Boolean)
            MyBase.Enabled = value
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "NGOMEXResultsWriter"
        End Get
    End Property

    Public Overloads ReadOnly Property OutputPath As String Implements IResultsWriter.OutputPath
        Get
            Return m_core.OutputPath
        End Get
    End Property

    Private ReadOnly Property IResultsWriter_DisplayName As String Implements IResultsWriter.DisplayName
        Get
            Return "NGOMEX filtered maps (ASCII format)"
        End Get
    End Property

    Public Overrides Sub EndWrite() Implements IResultsWriter.EndWrite

    End Sub

    Public Overrides Sub Init(theCore As Object) Implements IResultsWriter.Init
        MyBase.Init(theCore)
        '_core = theCore

        _bIntialized = False
        If m_core.nGroups = NGROUPS Then
            _bIntialized = True
        End If

        _lstGroups = New List(Of Integer) From {13, 16, 18, 27, 46, 53, 55, 57, 63, 65, 35}
        _lstTimesteps = New List(Of Integer)

        Dim years() As Integer = New Integer(1) {1, 5} ' {17, 34}

        'Convert the years to Ecospace Timesteps
        For Each year As Integer In years
            For i As Integer = 1 To 12
                _lstTimesteps.Add((year - 1) * 12 + i)
            Next
        Next

        '13
        'Spanish Mackerel
        ' (adults)
        '16
        'Sea Trouts(18 +)
        '18
        'Red Snapper(6 - 24)
        '27
        'Red Drum(18 - 36)
        '46
        'Gulf Menhaden(24 - 36)
        '53
        'Blue Crab
        '55
        'Brown Shrimp(adults)
        '57
        'White Shrimp(adults)
        '63
        'Zooplankton
        '65
        'Phytoplankton
        '35
        'Atlantic croaker
        ' (adults)



    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

    Public Overrides Sub StartWrite() Implements IResultsWriter.StartWrite
        MyBase.StartWrite()
    End Sub

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object) Implements IEcospaceResultsWriter.WriteResults

        If Not _bIntialized Then
            Return
        End If

        Try

            Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)
            Dim strm As StreamWriter = Nothing
            Dim strFile As String = ""

            If Not _lstTimesteps.Contains(tsData.iTimeStep) Then
                Return
            End If

            For Each igrp As Integer In _lstGroups

                System.Console.WriteLine("NGOMEX Ecospace results group = " + igrp.ToString + ", t = " + tsData.iTimeStep.ToString)

                'GetFileName() groups by default, can overridden by derived classes.
                strFile = Me.GetFileName(eVarNameFlags.EcospaceMapBiomass, igrp, Me.FileExtension(), tsData.iTimeStep)
                ' Create directory any time; user may have deleted it during a run
                If (cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True)) Then
                    'Handle file exceptions on a per file basis
                    'this way only the offending file will be skipped
                    'all other files will be written 

                    Try
                        strm = New StreamWriter(strFile, False)
                        If (strm IsNot Nothing) Then
                            Me.SaveASCFile(strm, tsData, igrp, eVarNameFlags.EcospaceMapBiomass)
                            strm.Flush()
                            strm.Close()
                            strm = Nothing
                        End If
                    Catch ex As IOException
                        cLog.Write(ex)
                    End Try
                End If 'cFileUtils.IsDirectoryAvailable()
            Next igrp

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".WriteResults Exception: " & ex.Message)
        End Try

    End Sub

End Class
