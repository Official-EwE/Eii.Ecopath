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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
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

''' ---------------------------------------------------------------------------
''' <summary>
''' Writer that saves supply, demand, and resilience data to csv file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cResilienceWriter

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Resilience data to use</summary>
    Private m_data As cResilienceData = Nothing

    Private m_lstrErrors As New List(Of String)

    Public Sub New(core As cCore, data As cResilienceData)
        Me.m_core = core
        Me.m_data = data
    End Sub

    Public Function Write() As Boolean

        Me.m_lstrErrors.Clear()

        Dim msg As cMessage = Nothing
        Dim bSuccess As Boolean = Me.SaveDemandSupply(True) And _
                                  Me.SaveDemandSupply(False) And _
                                  Me.SaveResilience(True) And _
                                  Me.SaveResilience(False)

        If (bSuccess) Then
            msg = New cMessage(String.Format(My.Resources.RESIL_STATUS_SAVE_SUCCESS, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputPath
        Else
            msg = New cMessage(String.Format(My.Resources.RESIL_STATUS_SAVE_FAILED, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
            For Each str As String In Me.m_lstrErrors
                msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, str, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
            Next
        End If
        Me.m_core.Messages.SendMessage(msg)

        Return True

    End Function

#Region " Internals "

    Private ReadOnly Property OutputPath As String
        Get
            Return Me.m_core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
        End Get
    End Property

    Private Function SaveDemandSupply(ByVal bAnnual As Boolean) As Boolean

        Dim sw As StreamWriter = Me.Writer(Me.DemandSupplyFileName(Me.OutputPath, bAnnual))
        Dim grp As cEcoPathGroupInput = Nothing
        Dim n As Integer = 0
        Dim t0 As Integer = 0

        If (sw Is Nothing) Then Return False

        If (Me.m_core.SaveWithFileHeader) Then sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))

        If (bAnnual) Then
            If (Me.m_core.EcosimFirstYear > 0) Then t0 = Me.m_core.EcosimFirstYear - 1
        End If

        ' Header
        sw.Write(cSystemUtils.IIF(bAnnual, "Year", "TimeStep"))
        For i As Integer = 1 To Me.m_core.nGroups
            grp = Me.m_core.EcoPathGroupInputs(i)
            If grp.IsConsumer Then
                sw.Write("," & cStringUtils.ToCSVField("Demand " & grp.Name))
                sw.Write("," & cStringUtils.ToCSVField("Supply " & grp.Name))
            End If
        Next
        sw.WriteLine()

        ' Body
        n = cSystemUtils.IIF(bAnnual, Me.m_data.NumYears, Me.m_data.NumTimeSteps)
        For t As Integer = 1 To n
            sw.Write(cStringUtils.ToCSVField(t0 + t))
            For i As Integer = 1 To Me.m_core.nGroups
                grp = Me.m_core.EcoPathGroupInputs(i)
                If grp.IsConsumer Then
                    If bAnnual Then
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupDemandAtY(i, t)))
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupSupplyAtY(i, t)))
                    Else
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupDemandAtT(i, t)))
                        sw.Write("," & cStringUtils.ToCSVField(Me.m_data.GroupSupplyAtT(i, t)))
                    End If
                End If
            Next i
            sw.WriteLine()
        Next t

        sw.Flush()
        sw.Close()

        Return True
    End Function

    Private Function DemandSupplyFileName(ByVal strPath As String, _
                                          ByVal bSaveAnnual As Boolean) As String

        Dim strFileName As String = ""
        Dim strExt As String = ".csv"

        If bSaveAnnual Then
            strFileName = "DemandSupply_annual"
        Else
            strFileName = "DemandSupply_monthly"
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False) & strExt)

    End Function

    Private Function SaveResilience(ByVal bAnnual As Boolean) As Boolean

        Dim sw As StreamWriter = Me.Writer(Me.ResilienceFileName(Me.OutputPath, bAnnual))
        Dim grp As cEcoPathGroupInput = Nothing
        Dim n As Integer = 0
        Dim t0 As Integer = 0

        If (bAnnual) Then
            If (Me.m_core.EcosimFirstYear > 0) Then t0 = Me.m_core.EcosimFirstYear - 1
        End If

        If (sw Is Nothing) Then Return False

        If (Me.m_core.SaveWithFileHeader) Then sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim))

        ' Header
        sw.Write(cSystemUtils.IIF(bAnnual, "Year", "TimeStep"))
        sw.WriteLine(",Resilience")

        ' Body
        n = cSystemUtils.IIF(bAnnual, Me.m_data.NumYears, Me.m_data.NumTimeSteps)
        For t As Integer = 1 To n
            sw.Write(cStringUtils.ToCSVField(t0 + t))
            If bAnnual Then
                sw.Write("," & cStringUtils.ToCSVField(Me.m_data.ResilienceAtY(t)))
            Else
                sw.Write("," & cStringUtils.ToCSVField(Me.m_data.ResilienceAtT(t)))
            End If
            sw.WriteLine()
        Next t

        sw.Flush()
        sw.Close()

        Return True
    End Function

    Private Function ResilienceFileName(ByVal strPath As String, _
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

    Private Function Writer(ByVal strFile As String) As StreamWriter

        Dim strPath As String = Path.GetDirectoryName(strFile)

        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Me.m_lstrErrors.Add(cStringUtils.Localize(My.Resources.ERROR_NODIR, strPath))
            Return Nothing
        End If

        Try
            Return New StreamWriter(strFile)
        Catch ex As Exception
            Me.m_lstrErrors.Add(ex.Message)
        End Try
        Return Nothing

    End Function

#End Region ' Internals

End Class
