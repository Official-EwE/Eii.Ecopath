
#Region "Import"

Imports System.IO

#End Region

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

#Region "MustOverride and Overridable IEcospaceResultsWriter Interfaces"

    Public MustOverride Sub WriteResults(ByVal SpaceTimeStepResults As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults

    Public MustOverride Sub EndWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.EndWrite

    Public MustOverride Sub StartWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.StartWrite


    Public Overridable Sub Init(ByVal theCore As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        Me.m_core = theCore
    End Sub

    Protected MustOverride ReadOnly Property OuputType() As eSpaceOutputType

#End Region

#Region "Protected methods"

    Protected Overridable Sub CreateTimeStampedDir()

        m_TimeStampDirName = System.IO.Path.Combine(Me.m_core.OutputPath, "Ecospace " & Me.getSubDirName & " " & Me.getTimeStamp)

        If Directory.Exists(Me.TimeStampDirName) Then
            Return
        End If

        Try
            Directory.CreateDirectory(TimeStampDirName)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() Exception: " & ex.Message)
        End Try

    End Sub


    Private Function getSubDirName() As String

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

    Protected Overridable Function getTimeStamp() As String
        Return Format(Date.Now, "y-MM-dd HH-mm-ss")
    End Function

    Protected Overridable ReadOnly Property TimeStampDirName()
        Get
            Return Me.m_TimeStampDirName
        End Get
    End Property


    Protected Overridable Function getFileName(ByVal VariableName As String, ByVal iGrp As Integer, ByVal Ext As String, Optional ByRef ModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim Timestep As String = ""

        'Is there a time step in the file name
        If ModelTimeStep <> cCore.NULL_VALUE Then
            'Yes so include it in the file name
            Timestep = "-" & ModelTimeStep.ToString
        End If

        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName(VariableName & "-" & grpName & Timestep & "." & Ext, False)
        Return System.IO.Path.Combine(Me.TimeStampDirName, fn)

    End Function

    Protected ReadOnly Property PathData() As cEcopathDataStructures
        Get
            Return Me.m_core.m_EcoPathData
        End Get
    End Property

    Protected ReadOnly Property SpaceData() As cEcospaceDataStructures
        Get
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

#End Region

End Class
