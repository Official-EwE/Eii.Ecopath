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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write network analysis results to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cResultWriter

    Private m_manager As cNetworkManager = Nothing

    Private Class cColTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eColTypes)
        End Function

        Public Function GetDescriptor(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.GetDescriptor

            Dim strVar As String = value.ToString()
            Dim strCol As String = cResourceUtils.LoadString("COL_HDR_" & strVar.ToUpper, Me.GetType.Assembly)

            If String.IsNullOrWhiteSpace(strCol) Then Return strVar
            Return strCol

        End Function

    End Class

    Private Enum eColTypes As Integer
        YEAR = 0
        THROUGHPUT
        CAPACITY_ECOSIM
        ASCEND_IMPORT
        ASCEND_FLOW
        ASCEND_EXPORT
        ASCEND_RESP
        OVERHEAD_IMPORT
        OVERHEAD_FLOW
        OVERHEAD_EXPORT
        OVERHEAD_RESP
        PCI
        FCI
        PATH_LEN
        EXPORT
        RESP_ECOSIM
        PRIM_PROD
        PROD
        BIOMASS
        [CATCH]
        PROP_FLOW_DET
        ASCEND_TOTAL
        AMI
        ENTROPY
        TLc
        KEMPTONS
        FIB
        DET_TE
        PP_TE
        TOT_TE
        CATCH_PPR
        CATCH_DET_REQ
    End Enum

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
            bSucces = Me.WriteData(Me.GetResultFileName(strPath, True, True), Me.GetIndicesWithPPRData(True)) Or _
                      Me.WriteData(Me.GetResultFileName(strPath, True, False), Me.GetIndicesWithPPRData(False))
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
        Dim strFile As String = "NA_" & _
                                CStr(IIf(bAnnual, My.Resources.HEADER_ANNUAL, My.Resources.HEADER_MONTHLY)) & "_" & _
                                CStr(IIf(bWithPPR, "IndicesPPR", "IndicesWithoutPPR")) & _
                                ".csv"
        Return Path.Combine(strPath, strFile)
    End Function

    Friend ReadOnly Property NetworkManager() As cNetworkManager
        Get
            Return Me.m_manager
        End Get
    End Property

    Private Function WriteData(ByVal strFileName As String, ByVal strData As String) As Boolean

        Dim strPath As String = Path.GetDirectoryName(strFileName)
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_NOACCESS, strPath), True, strPath)
            Return False
        End If

        Dim sw As New StreamWriter(strFileName)
        If (sw IsNot Nothing) Then
            sw.Write(strData)
            sw.Close()
            Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_SUCCESS, strFileName), False, strPath)
            Return True
        End If

        Me.SendMessage(String.Format(My.Resources.PROMPT_SAVE_FAILED, strFileName), True, strPath)
        Return False

    End Function

    Private Function GetIndicesWithoutPPRData(ByVal bAnnualAverage As Boolean) As String

        Dim cols As eColTypes() = DirectCast([Enum].GetValues(GetType(eColTypes)), eColTypes())
        Dim iNumCols As Integer = cols.Length - 2 ' Exclude PPR columns
        Dim asValues(iNumCols) As Single
        Dim iMonth As Integer = 0
        Dim iYear As Integer = 0
        Dim bLineAdded As Boolean = False
        Dim sb As New StringBuilder()
        Dim fmt As New cColTypeFormatter()

        ' Header line
        For j As Integer = 0 To iNumCols - 1
            If (j > 0) Then sb.Append(",")
            sb.Append(cStringUtils.ToCSVField(fmt.GetDescriptor(cols(j))))
        Next
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            iYear = CInt(Me.m_manager.Core.EcosimFirstYear + Math.Floor((i - 1) / cCore.N_MONTHS))

            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To iNumCols - 1

                ' Reset total when either processing monthly values OR processing January
                If (bAnnualAverage = False) Or (iMonth = 0) Then
                    asValues(j) = 0
                End If

                ' Add indicator to total
                Select Case cols(j)

                    Case eColTypes.THROUGHPUT : asValues(j) += Me.NetworkManager.ThroughputEcosim(i)
                    Case eColTypes.CAPACITY_ECOSIM : asValues(j) += Me.NetworkManager.CapacityEcosim(i)
                    Case eColTypes.ASCEND_IMPORT : asValues(j) += Me.NetworkManager.AscendImportEcosim(i)
                    Case eColTypes.ASCEND_FLOW : asValues(j) += Me.NetworkManager.AscendFlowEcosim(i)
                    Case eColTypes.ASCEND_EXPORT : asValues(j) += Me.NetworkManager.AscendExportEcosim(i)
                    Case eColTypes.ASCEND_RESP : asValues(j) += Me.NetworkManager.AscendRespEcosim(i)
                    Case eColTypes.OVERHEAD_IMPORT : asValues(j) += Me.NetworkManager.OverheadImportEcosim(i)
                    Case eColTypes.OVERHEAD_FLOW : asValues(j) += Me.NetworkManager.OverheadFlowEcosim(i)
                    Case eColTypes.OVERHEAD_EXPORT : asValues(j) += Me.NetworkManager.OverheadExportEcosim(i)
                    Case eColTypes.OVERHEAD_RESP : asValues(j) += Me.NetworkManager.OverheadRespEcosim(i)
                    Case eColTypes.PCI : asValues(j) += Me.NetworkManager.PCIEcosim(i)
                    Case eColTypes.FCI : asValues(j) += Me.NetworkManager.FCIEcosim(i)
                    Case eColTypes.PATH_LEN : asValues(j) += Me.NetworkManager.PathLengthEcosim(i)
                    Case eColTypes.EXPORT : asValues(j) += Me.NetworkManager.ExportEcosim(i)
                    Case eColTypes.RESP_ECOSIM : asValues(j) += Me.NetworkManager.RespEcosim(i)
                    Case eColTypes.PRIM_PROD : asValues(j) += Me.NetworkManager.PrimaryProdEcosim(i)
                    Case eColTypes.PROD : asValues(j) += Me.NetworkManager.ProdEcosim(i)
                    Case eColTypes.BIOMASS : asValues(j) += Me.NetworkManager.BiomassEcosim(i)
                    Case eColTypes.CATCH : asValues(j) += Me.NetworkManager.CatchEcosim(i)
                    Case eColTypes.PROP_FLOW_DET : asValues(j) += Me.NetworkManager.PropFlowDetEcosim(i)
                    Case eColTypes.ASCEND_TOTAL : asValues(j) += Me.NetworkManager.AscendTotalEcosim(i)
                    Case eColTypes.AMI : asValues(j) += Me.NetworkManager.AMIEcosim(i)
                    Case eColTypes.ENTROPY : asValues(j) += Me.NetworkManager.EntropyEcosim(i)
                    Case eColTypes.TLc : asValues(j) += Me.NetworkManager.TLCatchPlot(i)
                    Case eColTypes.KEMPTONS : asValues(j) += Me.NetworkManager.RelativeKemptonsPlot(i)
                    Case eColTypes.FIB : asValues(j) += Me.NetworkManager.FIB(i)
                    Case eColTypes.DET_TE : asValues(j) += Me.NetworkManager.DetTransferEfficiencyEcosim(i)
                    Case eColTypes.PP_TE : asValues(j) += Me.NetworkManager.PPTransferEfficiencyEcosim(i)
                    Case eColTypes.TOT_TE : asValues(j) += Me.NetworkManager.TotTransferEfficiencyEcosim(i)

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

        Dim cols As eColTypes() = DirectCast([Enum].GetValues(GetType(eColTypes)), eColTypes())
        Dim iNumCols As Integer = cols.Length - 2 ' Exclude PPR columns
        Dim asValues(iNumCols) As Single
        Dim iMonth As Integer = 0
        Dim iYear As Integer = 0
        Dim bLineAdded As Boolean = False
        Dim sb As New StringBuilder()
        Dim fmt As New cColTypeFormatter()

        ' Header line
        For j As Integer = 0 To iNumCols - 1
            If (j > 0) Then sb.Append(",")
            sb.Append(cStringUtils.ToCSVField(fmt.GetDescriptor(cols(j))))
        Next
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            iYear = CInt(Me.m_manager.Core.EcosimFirstYear + Math.Floor((i - 1) / cCore.N_MONTHS))

            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To iNumCols - 1

                ' Reset total when either processing monthly values OR processing January
                If (bAnnualAverage = False) Or (iMonth = 0) Then
                    asValues(j) = 0
                End If

                ' Add indicator to total
                Select Case cols(j)

                    Case eColTypes.THROUGHPUT : asValues(j) += Me.NetworkManager.ThroughputEcosim(i)
                    Case eColTypes.CAPACITY_ECOSIM : asValues(j) += Me.NetworkManager.CapacityEcosim(i)
                    Case eColTypes.ASCEND_IMPORT : asValues(j) += Me.NetworkManager.AscendImportEcosim(i)
                    Case eColTypes.ASCEND_FLOW : asValues(j) += Me.NetworkManager.AscendFlowEcosim(i)
                    Case eColTypes.ASCEND_EXPORT : asValues(j) += Me.NetworkManager.AscendExportEcosim(i)
                    Case eColTypes.ASCEND_RESP : asValues(j) += Me.NetworkManager.AscendRespEcosim(i)
                    Case eColTypes.OVERHEAD_IMPORT : asValues(j) += Me.NetworkManager.OverheadImportEcosim(i)
                    Case eColTypes.OVERHEAD_FLOW : asValues(j) += Me.NetworkManager.OverheadFlowEcosim(i)
                    Case eColTypes.OVERHEAD_EXPORT : asValues(j) += Me.NetworkManager.OverheadExportEcosim(i)
                    Case eColTypes.OVERHEAD_RESP : asValues(j) += Me.NetworkManager.OverheadRespEcosim(i)
                    Case eColTypes.PCI : asValues(j) += Me.NetworkManager.PCIEcosim(i)
                    Case eColTypes.FCI : asValues(j) += Me.NetworkManager.FCIEcosim(i)
                    Case eColTypes.PATH_LEN : asValues(j) += Me.NetworkManager.PathLengthEcosim(i)
                    Case eColTypes.EXPORT : asValues(j) += Me.NetworkManager.ExportEcosim(i)
                    Case eColTypes.RESP_ECOSIM : asValues(j) += Me.NetworkManager.RespEcosim(i)
                    Case eColTypes.PRIM_PROD : asValues(j) += Me.NetworkManager.PrimaryProdEcosim(i)
                    Case eColTypes.PROD : asValues(j) += Me.NetworkManager.ProdEcosim(i)
                    Case eColTypes.BIOMASS : asValues(j) += Me.NetworkManager.BiomassEcosim(i)
                    Case eColTypes.CATCH : asValues(j) += Me.NetworkManager.CatchEcosim(i)
                    Case eColTypes.PROP_FLOW_DET : asValues(j) += Me.NetworkManager.PropFlowDetEcosim(i)
                    Case eColTypes.ASCEND_TOTAL : asValues(j) += Me.NetworkManager.AscendTotalEcosim(i)
                    Case eColTypes.AMI : asValues(j) += Me.NetworkManager.AMIEcosim(i)
                    Case eColTypes.ENTROPY : asValues(j) += Me.NetworkManager.EntropyEcosim(i)
                    Case eColTypes.TLc : asValues(j) += Me.NetworkManager.TLCatchPlot(i)
                    Case eColTypes.KEMPTONS : asValues(j) += Me.NetworkManager.RelativeKemptonsPlot(i)
                    Case eColTypes.FIB : asValues(j) += Me.NetworkManager.FIB(i)
                    Case eColTypes.DET_TE : asValues(j) += Me.NetworkManager.DetTransferEfficiencyEcosim(i)
                    Case eColTypes.PP_TE : asValues(j) += Me.NetworkManager.PPTransferEfficiencyEcosim(i)
                    Case eColTypes.TOT_TE : asValues(j) += Me.NetworkManager.TotTransferEfficiencyEcosim(i)
                    Case eColTypes.CATCH_PPR : asValues(j) += Me.NetworkManager.RaiseToPPEcosim(i)
                    Case eColTypes.CATCH_DET_REQ : asValues(j) += Me.NetworkManager.RaiseToDetEcosim(i)
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

    Private Sub SendMessage(strMessage As String, Optional bError As Boolean = False, Optional strURL As String = "")
        Dim msg As New cMessage(strMessage, eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.External, _
                                DirectCast(IIf(bError, eMessageImportance.Warning, eMessageImportance.Information), eMessageImportance))
        msg.Hyperlink = strURL
        Me.m_manager.Core.Messages.SendMessage(msg)
    End Sub

End Class
