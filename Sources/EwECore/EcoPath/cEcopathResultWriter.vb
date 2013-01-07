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
    Public Sub New(ByVal core As cCore)
        Me.m_core = core
        Me.m_data = core.m_EcoPathData
    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write Ecopath estimates to a CSV file.
    ''' </summary>
    ''' <param name="strFN">The target file name.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteCSV(strFN As String) As Boolean

        ' Extracted this logic from the Ecopath datastructures 'Dump' method

        Dim strm As System.IO.StreamWriter = cFileUtils.GetStreamWriter(strFN)
        Dim bSuccess As Boolean = True

        If (strm IsNot Nothing) Then

            strm.WriteLine("EwE version," & cStringUtils.ToCSVField(cCore.Version))
            strm.WriteLine("Run date," & Date.Now.ToLongDateString & " " & Date.Now.ToLongTimeString)
            strm.WriteLine("Model," & cStringUtils.ToCSVField(Me.m_core.DataSource.FileName))
            strm.WriteLine()

            strm.WriteLine("Group,Biomass(B),Prod/Biomass(PB),Cons/Biomass(QB),Ecotrophic eff.(EE),Prod/Consum(GE)")
            For i As Integer = 1 To Me.m_data.NumGroups
                strm.Write(cStringUtils.ToCSVField(Me.m_data.GroupName(i)))
                strm.Write(",")
                strm.Write(cStringUtils.FormatSingle(Me.m_data.B(i)))
                strm.Write(",")
                strm.Write(cStringUtils.FormatSingle(Me.m_data.PB(i)))
                strm.Write(",")
                strm.Write(cStringUtils.FormatSingle(Me.m_data.QB(i)))
                strm.Write(",")
                strm.Write(cStringUtils.FormatSingle(Me.m_data.EE(i)))
                strm.Write(",")
                strm.Write(cStringUtils.FormatSingle(Me.m_data.GE(i)))
                strm.WriteLine()
            Next
            strm.Flush()
            strm.Close()
        Else
            bSuccess = False
            cLog.Write(Me.ToString + ".WriteCSV() failed to open file.")
        End If
        Return bSuccess

    End Function

#End Region ' Public access

End Class
