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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Writer to save Ecopath estimates to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcopathResultWriter

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_data As cEcopathDataStructures = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="core">The core instance to write result for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)
        Me.m_core = core
        Me.m_data = core.m_EcopathData
    End Sub

#End Region ' Constructor

#Region " Public access "

    Public Function WriteResults(Optional bQuiet As Boolean = False) As Boolean

        If (Not Me.m_core.StateMonitor.HasEcopathRan) Then Return False

        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        Dim strPath As String = Me.m_core.DefaultOutputPath(eAutosaveTypes.EcopathResults)

        ' Try to make sure that the output path is there
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_SAVE_FAILED, strPath, My.Resources.CoreMessages.OUTPUT_DIRECTORY_MISSING),
                               eMessageType.DataExport, eCoreComponentType.Ecosim, eMessageImportance.Information)
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                cLog.Write(msg)
            End If
            Return False
        End If

        Dim strFile As String = Path.Combine(strPath, "basic_estimates.csv")
        If Not Me.WriteResults(strFile) Then
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_RESULTS_SAVED_FAILED, strFile),
                               eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Warning)
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                cLog.Write(msg)
            End If
        Else
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_RESULTS_SAVED_SUCCESS, strFile),
                               eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Information)
            ' Provide hyperlink to the directory with the files
            msg.Hyperlink = strPath
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                cLog.Write(msg)
            End If
        End If
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write Ecopath estimates to a CSV file.
    ''' </summary>
    ''' <param name="strFN">The target file name.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function WriteResults(strFN As String) As Boolean

        Dim sw As StreamWriter = Nothing
        Dim bSuccess As Boolean = True

        Try
            sw = New StreamWriter(strFN)
        Catch ex As Exception
            bSuccess = False
        End Try

        If (sw IsNot Nothing) Then

            If Me.m_core.SaveWithFileHeader Then
                sw.Write(Me.m_core.DefaultFileHeader(EwEUtils.Core.eAutosaveTypes.Ecopath))
                sw.WriteLine()
            End If

            sw.WriteLine("GroupNo,Group,B,PB,QB,EE,GE,FishMort,PredMort,BioAccum,NetMig,OtherMort,NatMort")
            For i As Integer = 1 To Me.m_data.NumGroups

                Dim grp As cEcopathGroupOutput = Me.m_core.EcopathGroupOutputs(i)

                sw.Write(i)
                sw.Write(",")
                sw.Write(cStringUtils.ToCSVField(Me.m_data.GroupName(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.B(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.PB(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.QB(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.EE(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.GE(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoFishRate)) ' FishMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoPredMort)) ' PredMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.BioAccumRatePerYear)) ' BioAccum
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoNetMig)) ' NetMig
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoOtherMort)) ' OtherMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.NatMortPerTotMort)) ' NatMort
                sw.WriteLine()
            Next
            sw.Flush()
            sw.Close()
        Else
            bSuccess = False
            cLog.Write(Me.ToString + ".WriteCSV() failed to open file.")
        End If
        Return bSuccess

    End Function

#End Region ' Public access

End Class
