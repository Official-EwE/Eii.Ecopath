'==============================================================================
'
' $Log: cResultWriter.vb,v $
' Revision 1.2  2009/05/23 11:49:33  jeroens
' Don't for get to run main network first!
'
' Revision 1.1  2009/05/19 13:22:48  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands

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

    Public Function WriteCurrentResults(ByVal strPath As String, _
                                        ByVal bAnnualAverage As Boolean) As Boolean
        If Me.m_manager.EcosimPPROn Then
            Return Me.WriteIndicesWithPPR(strPath, bAnnualAverage)
        Else
            Return Me.WriteIndicesWithoutPPR(strPath, bAnnualAverage)
        End If
    End Function

    Public Function WriteIndicesWithPPR(ByVal strPath As String, _
                                        ByVal bAnnualAverage As Boolean) As Boolean

        Dim strFileName As String = ""
        Dim strData As String = ""
        Dim bSucces As Boolean = False

        bSucces = Me.NetworkManager.RunRequiredPrimaryProd()
        ' Switch on PPR in Ecosim
        Me.NetworkManager.UseEcosimNetwork = True
        Me.NetworkManager.EcosimPPROn = True
        ' Ecosim NA run succesful?
        bSucces = bSucces And Me.NetworkManager.RunEcosimNetwork()
        Me.NetworkManager.UseEcosimNetwork = False

        If (bSucces) Then
            strData = Me.GetIndicesWithPPRData(bAnnualAverage)
            If bAnnualAverage Then
                strFileName = "EwE6-NA_annual_IndicesPPR.csv"
            Else
                strFileName = "EwE6-NA_monthly_IndicesPPR.csv"
            End If
            Return Me.WriteData(Path.Combine(strPath, strFileName), strData)
        End If
        Return False

    End Function

    Public Function WriteIndicesWithoutPPR(ByVal strPath As String, _
                                        ByVal bAnnualAverage As Boolean) As Boolean

        Dim strFileName As String = ""
        Dim strData As String = ""
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
            strData = Me.GetIndicesWithoutPPRData(bAnnualAverage)
            If bAnnualAverage Then
                strFileName = "EwE6-NA_annual_IndicesWithoutPPR.csv"
            Else
                strFileName = "EwE6-NA_monthly_IndicesWithoutPPR.csv"
            End If
            Return Me.WriteData(Path.Combine(strPath, strFileName), strData)
        End If
        Return False

    End Function

    Friend ReadOnly Property NetworkManager() As cNetworkManager
        Get
            Return Me.m_manager
        End Get
    End Property

    Private Function WriteData(ByVal strFileName As String, ByVal strData As String) As Boolean
        Dim sw As New StreamWriter(strFileName)
        If (sw IsNot Nothing) Then
            sw.Write(strData)
            sw.Close()
            Return True
        End If
        Return False
    End Function

    Private Function GetIndicesWithoutPPRData(ByVal bAnnualAverage As Boolean) As String
        Dim sb As New StringBuilder()
        Dim asValues(23) As Single
        Dim iMonth As Integer = 0
        Dim bLineAdded As Boolean = False

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
        sb.Append(My.Resources.COL_HDR_ASCEND_TOTAL)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_AMI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ENTROPY)
        sb.Append(", ")
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To 23

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
                End Select

                ' Processing annual averages?
                If (bAnnualAverage) Then
                    ' #Yes: processing december?
                    If (iMonth = (cCore.N_MONTHS - 1)) Then
                        ' #Yes: average value and add it
                        asValues(j) /= cCore.N_MONTHS
                        sb.Append(asValues(j))
                        sb.Append(", ")
                        bLineAdded = True
                    End If
                Else
                    ' #No: add value
                    sb.Append(asValues(j))
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
        Dim sb As New StringBuilder()
        Dim asValues(25) As Single
        Dim iMonth As Integer = 0
        Dim bLineAdded As Boolean = False

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
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps

            ' Calc month
            iMonth = (i - 1) Mod cCore.N_MONTHS
            bLineAdded = False

            ' For every var to output
            For j As Integer = 1 To 25

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
                End Select

                ' Processing annual averages?
                If (bAnnualAverage) Then
                    ' #Yes: processing december?
                    If (iMonth = (cCore.N_MONTHS - 1)) Then
                        ' #Yes: average value and add it
                        asValues(j) /= cCore.N_MONTHS
                        sb.Append(asValues(j))
                        sb.Append(", ")
                        bLineAdded = True
                    End If
                Else
                    ' #No: add value
                    sb.Append(asValues(j))
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

End Class
