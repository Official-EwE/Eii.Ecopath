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
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Public Class cResilienceWriter

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Resilience data to use</summary>
    Private m_data As cResilienceData = Nothing

    Public Sub New(core As cCore, data As cResilienceData)
        Me.m_core = core
        Me.m_data = data
    End Sub

    Public Function SaveDataToFile() As Boolean

        Dim msg As cMessage = Nothing
        Dim bSuccess As Boolean = Me.SaveDataToFile(True) And Me.SaveDataToFile(False)

        If (bSuccess) Then
            msg = New cMessage(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputPath
        Else
            msg = New cMessage(String.Format(My.Resources.STATUS_SAVE_FAILED, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
        End If
        Me.m_core.Messages.SendMessage(msg)

        Return True

    End Function

#Region " Internals "

    Private Function SaveDataToFile(ByVal bAnnual As Boolean) As Boolean

        Dim sw As StreamWriter = Nothing
        Dim grp As cEcoPathGroupInput = Nothing
        Dim n As Integer = 0

        Try
            sw = New StreamWriter(Me.GetOutputFileName(Me.OutputPath, bAnnual))
        Catch ex As Exception
            ' ToDo: send failure message
            Return False
        End Try

        If (Me.m_core.SaveWithFileHeader) Then sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))

        ' Header
        sw.Write(cSystemUtils.IIF(bAnnual, "Year", "Time"))
        For i As Integer = 1 To Me.m_core.nGroups
            grp = Me.m_core.EcoPathGroupInputs(i)
            If grp.IsConsumer Then
                sw.Write("," & cStringUtils.ToCSVField("Supply " & grp.Name))
                sw.Write("," & cStringUtils.ToCSVField("Demand " & grp.Name))
            End If
        Next
        sw.WriteLine()

        ' Body
        n = cSystemUtils.IIF(bAnnual, Me.m_data.NumYears, Me.m_data.NumTimeSteps)
        For t As Integer = 0 To n - 1
            sw.Write(cStringUtils.ToCSVField(t))
            For i As Integer = 1 To Me.m_core.nGroups
                grp = Me.m_core.EcoPathGroupInputs(i)
                If grp.IsConsumer Then
                    If bAnnual Then
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupSupplyAtY(i, t)))
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupDemandAtY(i, t)))
                    Else
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupSupplyAtT(i, t)))
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupDemandAtT(i, t)))
                    End If
                End If
            Next i
            sw.WriteLine()
        Next t

        sw.Flush()
        sw.Close()

        Return True
    End Function

    Private ReadOnly Property OutputPath As String
        Get
            Return Me.m_core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
        End Get
    End Property

    Private Function GetOutputFileName(ByVal strPath As String, _
                                       ByVal bSaveAnnual As Boolean) As String

        Dim strFileName As String = ""
        Dim strExt As String = ".csv"

        If bSaveAnnual Then
            strFileName = "Resilience_annual"
        Else
            strFileName = "Resilience_monthly"
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False) & strExt)

    End Function

#End Region ' Internals

End Class
