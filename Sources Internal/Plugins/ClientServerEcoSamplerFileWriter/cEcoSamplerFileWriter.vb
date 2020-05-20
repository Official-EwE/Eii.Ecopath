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
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region




Public Class cEcoSamplerFileWriter
    Private m_plugin As cEcoSamplerFileWriterPlugin
    Private Sampler As Samples.cEcopathSampleManager
    Private m_PathData As cEcopathDataStructures
    Private m_core As cCore


    Public FileName As String

    Public DefautlFilename As String

    Public Sub New(Plugin As cEcoSamplerFileWriterPlugin)

        m_plugin = Plugin
        m_core = Me.m_plugin.Core

        Sampler = m_core.SampleManager
        m_PathData = m_core.EcopathDataStructures


    End Sub


    Public Function ToCSVFile(strFilename As String) As Boolean

        FileName = strFilename

        Try
            Dim strm As IO.StreamWriter = New IO.StreamWriter(FileName)
            strm.WriteLine(ToFileHeader())

            For isamp As Integer = 1 To Sampler.nSamples
                Dim samp As Samples.cEcopathSample = Sampler.Sample(isamp)
                strm.Write(ToSampleHeader(samp, m_PathData.NumGroups, isamp))
                For igrp As Integer = 1 To m_PathData.NumGroups
                    strm.WriteLine(toSampleString(samp, igrp))
                Next igrp
            Next isamp

            strm.Close()

        Catch ex As Exception
            Me.m_core.Messages.SendMessage(New cMessage("Error while saving file. " + ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.EcopathSample, eMessageImportance.Critical))
            Return False
        End Try

        Return True


    End Function

    Private Function ToFileHeader() As String
        Dim buff As New System.Text.StringBuilder

        buff.AppendLine(m_core.DefaultFileHeader(eAutosaveTypes.Ecopath))

        buff.AppendLine("<DATA_TYPE>, ECOSAMPLER_MONTECARLO")
        buff.AppendLine("<FILE_VERSION>, 1.0")
        buff.AppendLine("<EWE_VERSION>, " + cCore.Version())

        buff.AppendLine("<MODEL_NAME>, " + m_core.EwEModel.Name)
        buff.AppendLine("<MODEL_FILE>, " + m_core.DataSource.ToString)
        buff.AppendLine("<NUMBER_OF_SAMPLES>, " + Sampler.nSamples.ToString)
        buff.AppendLine("<NUMBER_OF_GROUPS>, " + Me.m_core.nGroups.ToString)

        Return buff.ToString
    End Function

    Private Function ToSampleHeader(Sample As Samples.cEcopathSample, nGroups As Integer, iSampleIndex As Integer) As String
        Dim temp As New System.Text.StringBuilder
        temp.AppendLine()
        temp.AppendLine("<SAMPLE_INDEX>, " + iSampleIndex.ToString)
        temp.AppendLine("<SAMPLE_HASH>, " + Sample.Hash)
        temp.AppendLine("<GROUP_NAMES/VARIABLE_NAME>,B,PB,QB,BA,BaBi,EE")
        Return temp.ToString

    End Function

    Private Function toSampleString(Sample As Samples.cEcopathSample, igrp As Integer) As String
        Dim buff As New System.Text.StringBuilder
        Dim delim As String = ","

        buff.Append(Me.ToCSV(m_PathData.GroupName(igrp), delim))
        buff.Append(Me.ToCSV(Sample.B(igrp), delim))
        buff.Append(Me.ToCSV(Sample.PB(igrp), delim))
        buff.Append(Me.ToCSV(Sample.QB(igrp), delim))
        buff.Append(Me.ToCSV(Sample.BA(igrp), delim))
        buff.Append(Me.ToCSV(Sample.BaBi(igrp), delim))
        buff.Append(Me.ToCSV(Sample.EE(igrp), ""))

        Return buff.ToString

    End Function

    Private Function ToCSV(value As Single, delim As String) As String
        Return cStringUtils.ToCSVField(value) + delim
    End Function
    Private Function ToCSV(value As String, delim As String) As String
        Return cStringUtils.ToCSVField(value) + delim
    End Function


End Class
