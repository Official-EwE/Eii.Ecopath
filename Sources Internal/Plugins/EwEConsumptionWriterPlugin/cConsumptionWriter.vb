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

#End Region ' Imports

Public Class cConsumptionWriter

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Ecopath data structures to use</summary>
    Private m_pathds As cEcopathDataStructures = Nothing
    ''' <summary>Ecosim data structures to use</summary>
    Private m_simds As cEcosimDatastructures = Nothing
    ''' <summary>Array for annual averaging</summary>
    Private m_annualavg As Single(,) = Nothing
    ''' <summary>States if all saving went well.</summary>
    Private m_bSuccess As Boolean = True

    Public Sub New(core As cCore, pathds As cEcopathDataStructures, simds As cEcosimDatastructures)
        Me.m_core = core
        Me.m_pathds = pathds
        Me.m_simds = simds
        ReDim Me.m_annualavg(Me.m_core.nGroups, Me.m_core.nGroups + 1)
    End Sub

    Public Function SaveDataToFile(ByVal iTime As Integer, _
                                   ByVal bAnnual As Boolean) As Boolean

        If (Not Me.m_bSuccess) Then Return False

        Dim strPath As String = Me.OutputPath()
        Dim strFileName As String = Me.GetOutputFileName(strPath, bAnnual, iTime)
        Dim strModelDetails As String = Me.GetModelDetails()
        Dim strDataDetails As String = "Data,Consumption"
        Dim data As Single(,) = Me.SimData()
        Dim nMax As Integer = Me.m_core.nLivingGroups
        Dim nGroups As Integer = Me.m_core.nGroups

        If My.Settings.IncludeDetritus Then
            nMax = Me.m_core.nGroups
        End If

        If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFileName), True) Then Return False

        If bAnnual Then
            For i As Integer = 1 To nGroups
                For j As Integer = 1 To nGroups + 1
                    If (iTime Mod cCore.N_MONTHS) = 1 Then
                        Me.m_annualavg(i, j) = 0
                    End If
                    Me.m_annualavg(i, j) += data(i, j)
                Next
            Next

            If ((iTime Mod cCore.N_MONTHS) = 0) Then
                ' Calc mean and fall through
                For i As Integer = 1 To nGroups
                    For j As Integer = 1 To nGroups + 1
                        Me.m_annualavg(i, j) /= cCore.N_MONTHS
                        data = Me.m_annualavg
                    Next
                Next
            Else
                Return True
            End If
        End If

        Try
            'Overwritten the file
            Using sw As StreamWriter = New StreamWriter(strFileName, False)

                If Me.m_core.SaveWithFileHeader Then
                    sw.WriteLine(strModelDetails)
                    sw.WriteLine(strDataDetails)
                    sw.WriteLine()
                End If

                For i As Integer = 1 To nMax
                    If i > 1 Then sw.Write(",")
                    sw.Write(cStringUtils.ToCSVField(Me.m_core.EcoPathGroupInputs(i).Name))
                Next

                If My.Settings.IncludeImportAndSum Then
                    sw.Write(",Import,Sum")
                End If
                sw.WriteLine()

                For j As Integer = 1 To nMax
                    Dim sSum As Single = 0
                    For i As Integer = 1 To nMax
                        If i > 1 Then sw.Write(",")
                        sw.Write(cStringUtils.FormatSingle(data(j, i)))
                        sSum += data(j, i)
                    Next
                    If My.Settings.IncludeImportAndSum Then
                        sw.Write(",")
                        sw.Write(cStringUtils.FormatSingle(data(j, nGroups + 1)))
                        sw.Write(",")
                        sw.Write(cStringUtils.FormatSingle(sSum))
                    End If
                    sw.WriteLine()
                Next
                sw.Close()

            End Using

        Catch ex As Exception
            Me.m_bSuccess = False
            Return False
        End Try
        Return True

    End Function

    ''' <summary>
    ''' Get if all writing went well - so far.
    ''' </summary>
    Public ReadOnly Property Success As Boolean
        Get
            Return Me.m_bSuccess
        End Get
    End Property

    Public ReadOnly Property OutputPath As String
        Get
            Return Me.m_core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
        End Get
    End Property

    Private Function GetOutputFileName(ByVal strPath As String, _
                                       ByVal bSaveAnnual As Boolean, _
                                       ByVal iTime As Integer) As String

        Dim strFileName As String = ""
        Dim strExt As String = ".csv"

        If bSaveAnnual Then
            strFileName = String.Format("Consumption_annual_{0:0000}", iTime)
        Else
            strFileName = String.Format("Consumption_{0:0000}", iTime)
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False) & strExt)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get default model details to report in output file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetModelDetails() As String
        Return Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim)
    End Function

    Private Function SimData() As Single(,)

        Dim n As Integer = Me.m_core.nGroups
        Dim data(n, n + 1) As Single

        ' Grab consumption
        For i As Integer = 1 To n
            For j As Integer = 1 To n
                data(i, j) = Me.m_simds.Consumpt(i, j)
            Next

            ' Calc imports
            data(i, n + 1) = Me.m_pathds.DtImp(i) * m_simds.Eatenby(i)
        Next

        Return data

    End Function

End Class
