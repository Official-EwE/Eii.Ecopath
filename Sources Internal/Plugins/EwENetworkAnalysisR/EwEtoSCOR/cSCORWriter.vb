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
Imports EwECore.Style
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <discussion>
''' I want to follow up on my last email concerning our challenge extracting the 
''' necessary data for ENA from Ecopath. I have attempted to extract the data from 
''' the EwE file for Sheila's model. I followed the following procedure to compile 
''' the data into an Excel spreadsheet (Sheila_PhD_SRB_EcoPath2SCOR.xlsx). The 
''' procedure is based on earlier work by Morag Ayers:
''' 
''' 1. Copy "Consumption" Flows from the Ecopath Parameterization. These are the 
'''    majority of flows in the network;
''' 2. Most Imports (inputs) are the second to last line of the "Consumption" Table.
'''    Imports for primary producers is determined by multiplying the biomass times 
'''    the Biomass/Production(/Year) estimates in ecopath;
''' 3. Biomass values are in the Basic Estimates tab;
''' 4. Respiration values are taken directly from Ecopath Estimates;
''' 5. Exports - these are assumed to be primarily fishing mortality. Thus, I am 
'''    calculating them as Biomass*Fishing Mortality Rate.
''' 
''' I then calculate the node input and output oriented throughflows (third worksheet) 
''' to see if the model is balanced.Usually, it is not quite balanced because there 
''' are missing exports from some nodes, especially non-living nodes (DOC, POC, etc). 
''' I usually figure out the throughflow difference and then use the detritial exports 
''' to balance the model.
''' 
''' The "enaR_Sheila1_fromxls.r" script in the analysis directory shows how I read the 
''' data in from the excel spreadsheet for analysis with enaR.I could have reformatted 
''' the data to a SCOR file, but have not done so at this time since I am trying to focus 
''' on the data.I will do the SCOR reformatting later so we can compare with the new 
''' EwE plugin later.
''' 
''' The enaR algorithms can be applied to the model data as extracted. enaR now reports 
''' a Total System ThroughFLOW value of 6814.76. While this is not the exact value of 
''' Total System ThroughPUT reported by EwE (6454.687), it is close. I am not sure 
''' exactly where the difference is coming, but interestingly it is nearly exactly the 
''' amount of DOC I needed to add as an export to balance the model thoughflow. I 
''' should also state the caveat that I am not 100% confident in the procedure I am using 
''' -- but this is the second time I have used it in an approximately successful way.
''' 
''' I need to make a note here for us about the difference between Total System 
''' ThroughFLOW and Total System ThoroughPUT. These are terms that are horribly confused 
''' in the literature and in some cases used interchangeably, but they are NOT the same.
''' Total System ThroughFLOW comes from the early input-output analysis (e.g., Patten, Finn).
''' It is the sum of all the internal transfers plus either the sum of the inputs OR outputs 
''' (respiration + exports). Ulanowicz introduced the language of Total System ThroughPUT, 
''' which is the sum of the internal flows, the boundary inputs AND boundary outputs. 
''' Although it is labeled as Total System ThroughPUT, I suspect that EwE is actually 
''' calculating Total System ThroughFLOW. Brian Fath also suggested this in a recent 
''' publication (sorry I dont have the exact reference). Importantly, this may affect the 
''' calculation of the Ascendency measures as they use Total System ThroughPUT in their 
''' calculations, not Total System ThroughFLOW. Likewise, the denominator of what we now 
''' call the Finn Cycling Index is Total System ThroughFLOW.
''' 
''' I hope that this effort will help us take the next step in connecting EwE and enaR. 
''' Please let me know if you have any questions.
''' </discussion>
Public Class cSCORWriter

    Private m_epData As cEcopathDataStructures = Nothing

    Public Sub New(ByVal epData As cEcopathDataStructures)

        Debug.Assert(epData IsNot Nothing)

        Me.m_epData = epData

    End Sub

    Public Function Write(ByVal strFileName As String) As Boolean

        Dim sw As StreamWriter = Nothing
        Dim line As Char() = Nothing
        Dim fmt As New cCurrencyUnitFormatter(Me.m_epData.ModelUnitCurrencyCustom)
        Dim sValue As Single = 0

        Try
            sw = New StreamWriter(strFileName)
        Catch ex As Exception
            cLog.Write(ex, "cSCORWriter '" & strFileName & "'")
            Return False
        End Try

        ' Do yer magics

        ' 1. Header line, with optional accuracy bytes
        line = NewLine(Me.m_epData.ModelName & ";" & cStringUtils.ToUTF8(fmt.GetDescriptor(Me.m_epData.ModelUnitCurrency)), 80)
        sw.WriteLine(line)

        ' 2. Compartments indices line
        sw.WriteLine("{0,3}{1,3}", m_epData.NumGroups, m_epData.NumLiving)

        ' 3. Group names
        For i As Integer = 1 To m_epData.NumGroups
            sw.WriteLine(Me.m_epData.GroupName(i))
        Next i

        ' 4. Data
        'a) Biomass()
        For i As Integer = 1 To m_epData.NumGroups
            sw.WriteLine("{0,3} {1}", i, cStringUtils.FormatNumber(Me.m_epData.B(i)))
        Next i

        ' b) Import
        sw.WriteLine("{0,3}\IMPORTS", -1)
        For i As Integer = 1 To m_epData.NumGroups
            sValue = Me.CalcImport(i)
            If (sValue > 0) Then
                sw.WriteLine("{0,3} {1}", i, cStringUtils.FormatNumber(sValue))
            End If
        Next i

        ' c) Export
        sw.WriteLine("{0,3}\EXPORTS", -1)
        For j As Integer = 1 To Me.m_epData.NumGroups
            Dim Q As Single = 0
            Dim R As Single = 0
            Dim Flow As Single = 0

            For i As Integer = 1 To Me.m_epData.NumGroups
                Q += Me.CalcConsumption(j, i)
                Flow += Me.CalcConsumption(i, j)
            Next
            Q += Me.CalcConsumption(j, 0)
            R = Me.m_epData.Resp(j)

            ' PP fudge - not correct
            If (m_epData.PP(j) = 1) Then Q = Flow

            ' Export = Qi - Ri - (Sum of flows i>j)
            sw.WriteLine("{0,3} {1}", j, cStringUtils.FormatNumber(Math.Max(0, Q - R - Flow)))
        Next j

        ' d) Respiration
        sw.WriteLine("{0,3}\RESPIRATION", -1)
        For i As Integer = 1 To m_epData.NumGroups
            sw.WriteLine("{0,3} {1}", i, cStringUtils.FormatNumber(Me.m_epData.Resp(i)))
        Next i

        'e) Diet
        sw.WriteLine("{0,3}\FLOWS", -1)
        For iPrey As Integer = 1 To m_epData.NumGroups
            For iPred As Integer = 1 To m_epData.NumGroups
                Dim cons As Single = Me.CalcConsumption(iPred, iPrey)
                If (cons > 0) Then
                    sw.WriteLine("{0,3}{1,3} {2}", iPrey, iPred, cStringUtils.FormatNumber(cons))
                End If
            Next iPred
        Next iPrey
        sw.WriteLine("{0,3}{0,3}", -1)

        sw.Flush()
        sw.Close()

        Return True

    End Function

#Region " Internals "

    Private Function NewLine(strText As String, Optional iLength As Integer = -1) As Char()
        If iLength <= 0 Then iLength = strText.Length
        Dim line(Math.Max(iLength - 1, 0)) As Char
        For i As Integer = 0 To line.Length - 1 : line(i) = " "c : Next
        Array.Copy(strText.ToCharArray, line, Math.Min(strText.Length, line.Length))
        Return line
    End Function

    Private Sub Insert(ByVal line As Char(), ByVal strText As String, ByVal iIndex As Integer)
        Dim iLength As Integer = Math.Min(strText.Length, line.Length - iIndex)
        Array.Copy(strText.ToCharArray, 0, line, iIndex, Math.Min(strText.Length, iLength))
    End Sub

    Private Function CalcConsumption(ByVal iPred As Integer, ByVal iPrey As Integer) As Single

        If (iPrey = 0) Then
            Return Me.m_epData.DC(iPred, iPrey) * Me.m_epData.B(iPred) * m_epData.QB(iPred)
        End If

        If (iPred <= Me.m_epData.NumLiving) Then
            Return CSng(Me.m_epData.B(iPred) * Me.m_epData.QB(iPred) * Me.m_epData.DC(iPred, iPrey))
        Else
            Return CSng(Me.m_epData.det(iPrey, iPred))
        End If

    End Function

    Private Function CalcImport(ByVal iGroup As Integer) As Single
        ' From EwE network analysis plug-in
        ' JS 18Mar13: validated
        If (iGroup > m_epData.NumLiving) Then
            Return m_epData.DtImp(iGroup)
        ElseIf (Me.m_epData.PP(iGroup) = 1.0) Then
            Return m_epData.B(iGroup) * m_epData.PB(iGroup)
        Else
            Return CSng(m_epData.B(iGroup) * m_epData.QB(iGroup) * m_epData.DC(iGroup, 0))
        End If
    End Function

#End Region ' Internals

End Class
