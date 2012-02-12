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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports EwECore.DataSources
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write network analysis results to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cResultWriter

    Private m_manager As cNetworkManager = Nothing

    Public Sub New(ByVal manager As cNetworkManager)
        Me.m_manager = manager
    End Sub

    Public Function WriteCurrentResults(ByVal strPath As String) As Boolean
        If Me.m_manager.EcosimPPROn Then
            Return Me.WriteIndicesWithPPR(strPath)
        Else
            Return Me.WriteIndicesWithoutPPR(strPath)
        End If
    End Function

    Public Function WriteIndicesWithPPR(ByVal strPath As String) As Boolean

        Dim bSucces As Boolean = False

        bSucces = Me.NetworkManager.RunRequiredPrimaryProd()
        ' Switch on PPR in Ecosim
        Me.NetworkManager.UseEcosimNetwork = True
        Me.NetworkManager.EcosimPPROn = True
        ' Ecosim NA run succesful?
        bSucces = bSucces And Me.NetworkManager.RunEcosimNetwork()
        Me.NetworkManager.UseEcosimNetwork = False

        If (bSucces) Then
            bSucces = Me.WriteData(Me.GetResultFileName(strPath, True, True), Me.GetIndicesWithoutPPRData(True)) Or _
                      Me.WriteData(Me.GetResultFileName(strPath, True, False), Me.GetIndicesWithoutPPRData(False))
        End If
        Return bSucces

    End Function

    Public Function WriteIndicesWithoutPPR(ByVal strPath As String) As Boolean

        Dim bSucces As Boolean = False

        bSucces = Me.NetworkManager.RunMainNetwork()
        bSucces = bSucces And Me.NetworkManager.RunRequiredPrimaryProd()

        ' Switch on PPR in Ecosim
        Me.NetworkManager.UseEcosimNetwork = True
        Me.NetworkManager.EcosimPPROn = False
        ' Ecosim NA run succesful?
        bSucces = bSucces And Me.NetworkManager.RunEcosimNetwork()
        Me.NetworkManager.UseEcosimNetwork = False

        If (bSucces) Then
            bSucces = Me.WriteData(Me.GetResultFileName(strPath, False, True), Me.GetIndicesWithoutPPRData(True)) Or _
                      Me.WriteData(Me.GetResultFileName(strPath, False, False), Me.GetIndicesWithoutPPRData(False))
        End If
        Return bSucces

    End Function

    Private Function GetResultFileName(ByVal strPath As String, ByVal bWithPPR As Boolean, ByVal bAnnual As Boolean) As String
        Dim core As cCore = Me.m_manager.Core
        Dim strFile As String = core.EcosimOutputFileLocation(My.Resources.CAPTION, _
                                                          CStr(IIf(bAnnual, My.Resources.HEADER_ANNUAL, My.Resources.HEADER_MONTHLY)) & "_" & CStr(IIf(bWithPPR, "IndicesPPR", "IndicesWithoutPPR")), _
                                                          ".csv")
        Return Path.Combine(strPath, strFile)
    End Function

    Friend ReadOnly Property NetworkManager() As cNetworkManager
        Get
            Return Me.m_manager
        End Get
    End Property

    Private Function WriteData(ByVal strFileName As String, ByVal strData As String) As Boolean

        ' ToDo: globalize this method
        Dim strPath As String = Path.GetDirectoryName(strFileName)
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_NOACCESS, strPath), True)
            Return False
        End If

        Dim sw As New StreamWriter(strFileName)
        If (sw IsNot Nothing) Then
            sw.Write(strData)
            sw.Close()
            Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_SUCCESS, strFileName))
            Return True
        End If

        Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_FAILED, strFileName), True)
        Return False

    End Function

    Private Function GetIndicesWithoutPPRData(ByVal bAnnualAverage As Boolean) As String

        Const cNUMCOLS As Integer = 26

        Dim sb As New StringBuilder()
        Dim asValues(cNUMCOLS) As Single
        Dim iMonth As Integer = 0
        Dim iYear As Integer = 0
        Dim bLineAdded As Boolean = False

        sb.Append(My.Resources.COL_HDR_YEAR)            ' 0
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_THROUGHPUT)      ' 1
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CAPACITY_ECOSIM) ' 2
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_IMPORT)   ' 3
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_FLOW)     ' 4
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_EXPORT)   ' 5
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_RESP)     ' 6
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_IMPORT) ' 7
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_FLOW)   ' 8
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_EXPORT) ' 9
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_RESP)   ' 10
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PCI)             ' 11
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_FCI)             ' 12
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PATH_LEN)        ' 13
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_EXPORT)          ' 14
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_RESP_ECOSIM)     ' 15
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PRIM_PROD)       ' 16
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROD)            ' 17
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_BIOMASS)         ' 18
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH)           ' 19
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROP_FLOW_DET)   ' 20
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_TOTAL)    ' 21
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_AMI)             ' 22
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ENTROPY)         ' 23
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_TLc)             ' 24
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_KEMPTONS)        ' 25
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_FIB)             ' 26
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            iYear = CInt(Me.m_manager.Core.EcosimFirstYear + Math.Floor((i - 1) / cCore.N_MONTHS))

            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To cNUMCOLS

                ' Reset total when either processing monthly values OR processing January
                If (bAnnualAverage = False) Or (iMonth = 0) Then
                    asValues(j) = 0
                End If

                ' Add indicator to total
                Select Case j
                    Case 1 : asValues(j) += Me.NetworkManager.ThroughputEcosim(i)
                    Case 2 : asValues(j) += Me.NetworkManager.CapacityEcosim(i)
                    Case 3 : asValues(j) += Me.NetworkManager.AscendImportEcosim(i)
                    Case 4 : asValues(j) += Me.NetworkManager.AscendFlowEcosim(i)
                    Case 5 : asValues(j) += Me.NetworkManager.AscendExportEcosim(i)
                    Case 6 : asValues(j) += Me.NetworkManager.AscendRespEcosim(i)
                    Case 7 : asValues(j) += Me.NetworkManager.OverheadImportEcosim(i)
                    Case 8 : asValues(j) += Me.NetworkManager.OverheadFlowEcosim(i)
                    Case 9 : asValues(j) += Me.NetworkManager.OverheadExportEcosim(i)
                    Case 10 : asValues(j) += Me.NetworkManager.OverheadRespEcosim(i)
                    Case 11 : asValues(j) += Me.NetworkManager.PCIEcosim(i)
                    Case 12 : asValues(j) += Me.NetworkManager.FCIEcosim(i)
                    Case 13 : asValues(j) += Me.NetworkManager.PathLengthEcosim(i)
                    Case 14 : asValues(j) += Me.NetworkManager.ExportEcosim(i)
                    Case 15 : asValues(j) += Me.NetworkManager.RespEcosim(i)
                    Case 16 : asValues(j) += Me.NetworkManager.PrimaryProdEcosim(i)
                    Case 17 : asValues(j) += Me.NetworkManager.ProdEcosim(i)
                    Case 18 : asValues(j) += Me.NetworkManager.BiomassEcosim(i)
                    Case 19 : asValues(j) += Me.NetworkManager.CatchEcosim(i)
                    Case 20 : asValues(j) += Me.NetworkManager.PropFlowDetEcosim(i)
                    Case 21 : asValues(j) += Me.NetworkManager.AscendTotalEcosim(i)
                    Case 22 : asValues(j) += Me.NetworkManager.AMIEcosim(i)
                    Case 23 : asValues(j) += Me.NetworkManager.EntropyEcosim(i)
                    Case 24 : asValues(j) += Me.NetworkManager.TLCatchPlot(i)
                    Case 25 : asValues(j) += Me.NetworkManager.RelativeKemptonsPlot(i)
                    Case 26 : asValues(j) += Me.NetworkManager.FIB(i)

                End Select

                ' Processing annual averages?
                If (bAnnualAverage) Then
                    ' #Yes: processing december?
                    If (iMonth = (cCore.N_MONTHS - 1)) Then
                        ' #Yes: average value and add it
                        asValues(j) /= cCore.N_MONTHS
                        ' Add year label first
                        If (j = 1) Then
                            sb.Append(iYear)
                            sb.Append(", ")
                        End If
                        sb.Append(cStringUtils.FormatSingle(asValues(j)))
                        sb.Append(", ")
                        bLineAdded = True
                    End If
                Else
                    ' #No: add year label
                    If (j = 1) Then
                        sb.Append(iYear & ":" & (iMonth + 1))
                        sb.Append(", ")
                    End If
                    ' Add value
                    sb.Append(cStringUtils.FormatSingle(asValues(j)))
                    sb.Append(", ")
                    bLineAdded = True
                End If
            Next j

            ' Add newline when a line was added
            If (bLineAdded) Then
                sb.AppendLine()
            End If

        Next i

        Return sb.ToString()
    End Function

    Private Function GetIndicesWithPPRData(ByVal bAnnualAverage As Boolean) As String

        Const cNUMCOLS As Integer = 28

        Dim sb As New StringBuilder()
        Dim asValues(cNUMCOLS) As Single
        Dim iMonth As Integer = 0
        Dim iYear As Integer = 0
        Dim bLineAdded As Boolean = False

        sb.Append(My.Resources.COL_HDR_YEAR)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_THROUGHPUT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CAPACITY_ECOSIM)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_IMPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_FLOW)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_RESP)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_IMPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_FLOW)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_RESP)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PCI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_FCI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PATH_LEN)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_RESP_ECOSIM)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PRIM_PROD)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROD)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_BIOMASS)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROP_FLOW_DET)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH_PPR)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH_DET_REQ)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_TOTAL)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_AMI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ENTROPY)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_TLc)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_KEMPTONS)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_FIB)
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            iYear = CInt(Me.m_manager.Core.EcosimFirstYear + Math.Floor((i - 1) / cCore.N_MONTHS))

            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To cNUMCOLS

                ' Reset total when either processing monthly values OR processing January
                If (bAnnualAverage = False) Or (iMonth = 0) Then
                    asValues(j) = 0
                End If

                ' Add indicator to total
                Select Case j
                    Case 1 : asValues(j) += Me.NetworkManager.ThroughputEcosim(i)
                    Case 2 : asValues(j) += Me.NetworkManager.CapacityEcosim(i)
                    Case 3 : asValues(j) += Me.NetworkManager.AscendImportEcosim(i)
                    Case 4 : asValues(j) += Me.NetworkManager.AscendFlowEcosim(i)
                    Case 5 : asValues(j) += Me.NetworkManager.AscendExportEcosim(i)
                    Case 6 : asValues(j) += Me.NetworkManager.AscendRespEcosim(i)
                    Case 7 : asValues(j) += Me.NetworkManager.OverheadImportEcosim(i)
                    Case 8 : asValues(j) += Me.NetworkManager.OverheadFlowEcosim(i)
                    Case 9 : asValues(j) += Me.NetworkManager.OverheadExportEcosim(i)
                    Case 10 : asValues(j) += Me.NetworkManager.OverheadRespEcosim(i)
                    Case 11 : asValues(j) += Me.NetworkManager.PCIEcosim(i)
                    Case 12 : asValues(j) += Me.NetworkManager.FCIEcosim(i)
                    Case 13 : asValues(j) += Me.NetworkManager.PathLengthEcosim(i)
                    Case 14 : asValues(j) += Me.NetworkManager.ExportEcosim(i)
                    Case 15 : asValues(j) += Me.NetworkManager.RespEcosim(i)
                    Case 16 : asValues(j) += Me.NetworkManager.PrimaryProdEcosim(i)
                    Case 17 : asValues(j) += Me.NetworkManager.ProdEcosim(i)
                    Case 18 : asValues(j) += Me.NetworkManager.BiomassEcosim(i)
                    Case 19 : asValues(j) += Me.NetworkManager.CatchEcosim(i)
                    Case 20 : asValues(j) += Me.NetworkManager.PropFlowDetEcosim(i)
                    Case 21 : asValues(j) += Me.NetworkManager.RaiseToPPEcosim(i)
                    Case 22 : asValues(j) += Me.NetworkManager.RaiseToDetEcosim(i)
                    Case 23 : asValues(j) += Me.NetworkManager.AscendTotalEcosim(i)
                    Case 24 : asValues(j) += Me.NetworkManager.AMIEcosim(i)
                    Case 25 : asValues(j) += Me.NetworkManager.EntropyEcosim(i)
                    Case 26 : asValues(j) += Me.NetworkManager.TLCatchPlot(i)
                    Case 27 : asValues(j) += Me.NetworkManager.RelativeKemptonsPlot(i)
                    Case 28 : asValues(j) += Me.NetworkManager.FIB(i)

                End Select

                ' Processing annual averages?
                If (bAnnualAverage) Then
                    ' #Yes: processing december?
                    If (iMonth = (cCore.N_MONTHS - 1)) Then
                        ' #Yes: add year label first
                        If (j = 1) Then
                            sb.Append(iYear)
                            sb.Append(", ")
                        End If
                        ' Add average value and add it
                        asValues(j) /= cCore.N_MONTHS
                        sb.Append(cStringUtils.FormatSingle(asValues(j)))
                        sb.Append(", ")
                        bLineAdded = True
                    End If
                Else
                    ' #No: 
                    ' First add year
                    If (j = 1) Then
                        sb.Append(iYear & ":" & (iMonth + 1))
                        sb.Append(", ")
                    End If
                    ' Add value
                    sb.Append(cStringUtils.FormatSingle(asValues(j)))
                    sb.Append(", ")
                    bLineAdded = True
                End If

            Next j

            ' Add newline when a line was added
            If (bLineAdded) Then
                sb.AppendLine()
            End If

        Next i

        Return sb.ToString()
    End Function

    Private Sub SendMessage(strMessage As String, Optional bError As Boolean = False)
        Dim msg As New cMessage(strMessage, eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.External, _
                                DirectCast(IIf(bError, eMessageImportance.Warning, eMessageImportance.Information), eMessageImportance))
        Me.m_manager.Core.Messages.SendMessage(msg)
    End Sub

End Class
