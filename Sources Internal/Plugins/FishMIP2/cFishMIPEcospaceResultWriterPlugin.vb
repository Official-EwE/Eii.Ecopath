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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin to write aggregated Ecospace results for FishMIP2.
''' </summary>
''' ===========================================================================
Public Class cFishMIPEcospaceResultWriterPlugin
    Inherits cEcospaceASCMapResultsWriter
    Implements IEcospaceResultWriterPlugin
    Implements IEcospaceInitializedPlugin

#Region " Generic bits "

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ecopathinternational@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "FishMipSpaceWriter"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP; rely on cFishMipCore instead
    End Sub

#End Region ' Generic bits

#Region " Writing "

    Private m_bSaveAnnualPreserved As Boolean = False

    Public Overrides Sub StartWrite()
        MyBase.StartWrite()
        m_bSaveAnnualPreserved = Me.m_ds.SaveAnnual
        m_ds.SaveAnnual = False
    End Sub

    Public Overrides Sub EndWrite()
        MyBase.EndWrite()
        m_ds.SaveAnnual = Me.m_bSaveAnnualPreserved
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.WriteResults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)
        Dim strFile As String = ""
        Dim strPath As String = Path.Combine(Me.OutputDirectory, "FishMIP")

        If (tsData.iTimeStep < Me.FirstOutputTimeStep) Then Return
        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then Return

        Dim config As cConfiguration = cFishMIPcore.GetInstance().Configuration
        Dim t As DateTime = core.EcospaceTimestepToAbsoluteTime(tsData.iTimeStep)

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            strFile = Path.Combine(Me.OutputDirectory, String.Format("{0}_{1:D4}-{2:D2}.asc", result.ToString(), t.Year, t.Month))

            Dim data(bm.InRow, bm.InCol) As Single
            For iRow As Integer = 1 To bm.InRow
                For icol As Integer = 1 To bm.InCol
                    data(iRow, icol) = cCore.NULL_VALUE
                    If (depth.IsWaterCell(iRow, icol)) Then
                        Dim val As Single = 0
                        For iGrp As Integer = 1 To core.nGroups
                            If config(iGrp, result) Then
                                Select Case result
                                    Case cConfiguration.eResultTypes.tsb,
                                         cConfiguration.eResultTypes.tcb,
                                         cConfiguration.eResultTypes.b10cm,
                                         cConfiguration.eResultTypes.b30cm
                                        val = tsData.BiomassMap(iRow, icol, iGrp)
                                    Case cConfiguration.eResultTypes.tc,
                                         cConfiguration.eResultTypes.tcb,
                                         cConfiguration.eResultTypes.tc30cm
                                        val = tsData.CatchMap(iRow, icol, iGrp)
                                    Case Else
                                        Debug.Assert(False, "Result type not supported")
                                End Select
                            End If
                        Next iGrp
                        If (val >= 0) Then
                            data(iRow, icol) = val
                        End If
                    End If
                Next icol
            Next iRow

            Try
                Using strm As New StreamWriter(strFile, False)
                    Me.SaveASCFile(strm, data)
                    strm.Flush()
                    strm.Close()
                End Using
            Catch ex As IOException
                cLog.Write(ex)
            End Try
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.FileExtension"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function FileExtension() As String
        Return ".asc"
    End Function

#End Region ' Writing

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write the run information file to accompany the run results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub WriteRunInfoFile()

        Try
            Dim strFN As String = Path.Combine(Me.OutputDirectory, "Ecospace RunInfo.txt")
            Dim strm As New StreamWriter(strFN, False)

            strm.WriteLine("EcoSpace .asc map output")
            Me.WriteRunInfo(strm)

            strm.Flush()
            strm.Close()
            strm = Nothing

        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write an entire ASCII file for a group, time step and variable.
    ''' </summary>
    ''' <param name="strm"></param>
    ''' -----------------------------------------------------------------------
    Protected Overloads Sub SaveASCFile(ByVal strm As StreamWriter, data As Single(,))
        Try
            Me.WriteASCIIHeader(strm)
            Me.WriteASCIIBody(strm, data)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write ESRI ASCII body block.
    ''' </summary>
    ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
    ''' -----------------------------------------------------------------------
    Protected Overloads Sub WriteASCIIBody(ByVal writer As StreamWriter, ByVal data(,) As Single)

        Dim value As Double = 0
        Dim strValue As String = ""

        Debug.Assert(data IsNot Nothing)

        For ir As Integer = 1 To Me.EcospaceData.InRow
            For ic As Integer = 1 To Me.EcospaceData.InCol
                If ic > 1 Then writer.Write(" ")
                If Me.EcospaceData.Depth(ir, ic) > 0 Then
                    value = data(ir, ic)
                Else
                    value = cCore.NULL_VALUE
                End If

                strValue = cStringUtils.FormatNumber(value)
                If (ir = 1 And ic = 1) Then
                    If (strValue.IndexOf("."c) = -1) Then
                        strValue = strValue + ".0"
                    End If
                End If

                writer.Write(strValue)
            Next
            writer.WriteLine("")
        Next

    End Sub

    Private m_ds As cEcospaceDataStructures = Nothing

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        m_ds = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "dataFishMIP"
        End Get
    End Property

#End Region ' Internals

End Class
