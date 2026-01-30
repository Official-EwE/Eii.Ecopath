' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv

Public Class cEffortLimits
    Implements IMSEData

#Region " Private variables "

    Private m_core As cCore = Nothing
    Private m_mse As cMSE = Nothing
    Private m_data As Single()
    Private m_bChanged As Boolean = False
    Private m_decaying_max_effort As Boolean

#End Region ' Private variables

#Region " Construction "

    Public Sub New(mse As cMSE, core As cCore)
        Me.m_core = core
        Me.m_mse = mse
        ReDim Me.m_data(core.nFleets - 1)
        Me.Defaults()
    End Sub

#End Region ' Construction

#Region " Public bits "

    Public Shared NoHCR_F As Single = cCore.NULL_VALUE

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IMSEData.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Sub Defaults() _
        Implements IMSEData.Defaults
        For i As Integer = 1 To Me.m_core.nFleets
            'Me.Value(i) = cEffortLimits.NoHCR_F
            Me.Value(i) = 0.1
        Next
        Me.m_bChanged = False
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IMSEData.IsChanged"/>
    ''' -----------------------------------------------------------------------
    Public Function IsChanged() As Boolean _
        Implements IMSEData.IsChanged
        Return Me.m_bChanged
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IMSEData.Load"/>
    ''' -----------------------------------------------------------------------
    Public Function Load(Optional msg As cMessage = Nothing,
                         Optional strFilename As String = "") As Boolean _
         Implements IMSEData.Load

        ' Resolve path to default, if missing
        If String.IsNullOrWhiteSpace(strFilename) Then
            strFilename = Me.DefaultFileName()
        End If

        Dim reader As StreamReader = cMSEUtils.GetReader(strFilename)

        Me.Defaults()

        If (reader IsNot Nothing) Then
            Try
                Dim EffortLimitsCSV As New CsvReader(reader, True)
                Dim iFleet As Integer = 0

                'While Not EffortLimitsCSV.EndOfStream
                '    If EffortLimitsCSV.ReadNextRecord() Then
                '        iFleet = cStringUtils.ConvertToInteger(EffortLimitsCSV(0))
                '        If (1 <= iFleet) And (iFleet <= Me.nFleets) Then
                '            Me.Value(iFleet) = cStringUtils.ConvertToSingle(EffortLimitsCSV(2))
                '            If Me.Value(iFleet) > 0.9 Then Me.Value(iFleet) = 0.9
                '        End If
                '    End If
                'End While
                'EffortLimitsCSV.Dispose()

                Do
                    If EffortLimitsCSV.ReadNextRecord() Then
                        If EffortLimitsCSV(0) = "Decaying_Max_Effort" Then
                            Me.m_decaying_max_effort = Boolean.Parse(EffortLimitsCSV(1))
                            Exit Do
                        End If
                        iFleet = cStringUtils.ConvertToInteger(EffortLimitsCSV(0))
                        If (1 <= iFleet) And (iFleet <= Me.nFleets) Then
                            Me.Value(iFleet) = cStringUtils.ConvertToSingle(EffortLimitsCSV(2))
                            If Me.Value(iFleet) > 0.9 Then Me.Value(iFleet) = 0.9
                        End If
                    Else
                        Exit Do
                    End If
                Loop

            Catch ex As Exception
                ' CSV malformed
                cMSEUtils.LogError(msg, "Effort limits cannot load from " & strFilename & ". " & ex.Message)
            End Try
        End If

        cMSEUtils.ReleaseReader(reader)
        Me.m_bChanged = False
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IMSEData.Save"/>
    ''' -----------------------------------------------------------------------
    Public Function Save(Optional strFilename As String = "") As Boolean _
        Implements IMSEData.Save

        ' Resolve path to default, if missing
        If String.IsNullOrWhiteSpace(strFilename) Then
            strFilename = Me.DefaultFileName()
        End If

        Dim writer As StreamWriter = cMSEUtils.GetWriter(strFilename)
        If (writer Is Nothing) Then Return False

        writer.WriteLine("FleetNumber,FleetName,MaxChangeEffort")
        For iFleet = 1 To Me.m_core.nFleets
            writer.WriteLine("{0},{1},{2}",
                              cStringUtils.FormatNumber(iFleet),
                              cStringUtils.ToCSVField(Me.m_core.EcopathFleetInputs(iFleet).Name),
                              cStringUtils.FormatNumber(Me.Value(iFleet)))
        Next
        writer.WriteLine("Decaying_Max_Effort," & Me.decaying_max_effort)

        cMSEUtils.ReleaseWriter(writer)
        Return True

    End Function

    Public Function FileExists(Optional strFilename As String = "") As Boolean _
        Implements IMSEData.FileExists
        If String.IsNullOrWhiteSpace(strFilename) Then
            strFilename = Me.DefaultFileName()
        End If
        Return File.Exists(strFilename)
    End Function

    Public Property Value(iFleet As Integer) As Single
        Get
            ' Sanity check
            Debug.Assert(1 <= iFleet And iFleet <= Me.m_core.nFleets)
            Return Me.m_data(iFleet - 1)
        End Get
        Set(value As Single)
            ' Sanity check
            Debug.Assert(1 <= iFleet And iFleet <= Me.m_core.nFleets)
            If (value <> Me.m_data(iFleet - 1)) Then
                Me.m_bChanged = True
            End If
            Me.m_data(iFleet - 1) = value
        End Set
    End Property

    ''' <summary>Cheat!</summary>
    Public ReadOnly Property nFleets As Integer
        Get
            Return Me.m_core.nFleets
        End Get
    End Property

    Public Property decaying_max_effort As Boolean
        Get
            Return Me.m_decaying_max_effort
        End Get
        Set(value As Boolean)
            Me.m_decaying_max_effort = value
        End Set
    End Property

#End Region ' Public bits

#Region " Internals "

    Private Function DefaultFileName() As String
        Return cMSEUtils.MSEFile(Me.m_mse.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv")
    End Function

#End Region ' Internals

End Class
