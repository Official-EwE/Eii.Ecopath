' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Utilities

#End Region ' Imports

Friend Class cEcospaceCatchTimeSeriesWriter

    Private Class cCSVKeyOrderSortThingy
        Implements IComparer(Of String)
        Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare

            Debug.Assert(Not String.IsNullOrWhiteSpace(x))
            Debug.Assert(Not String.IsNullOrWhiteSpace(y))

            Dim vx As Integer = If(Char.IsLetter(x(0)), -1, CInt(Val(x)))
            Dim vy As Integer = If(Char.IsLetter(y(0)), -1, CInt(Val(y)))

            If (vx < vy) Then Return -1
            If (vx > vy) Then Return 1
            Return String.Compare(x, y, True)

        End Function
    End Class

    Private m_core As cCore = Nothing
    Private m_catches As New Dictionary(Of String, Double())

    Public Function Init(theCore As cCore) As Boolean

        Me.m_core = theCore
        Return True

    End Function

    Public Sub AddCatch(zone As Integer, grp As Integer, year As Integer, total As Double)
        Me.AddCatch(zone.ToString("00"), grp.ToString("00"), year, total)
    End Sub

    Public Sub AddCatch(zone As Integer, grp As String, year As Integer, total As Double)
        Me.AddCatch(zone.ToString("00"), grp, year, total)
    End Sub

    Public Sub AddCatch(zone As String, grp As Integer, year As Integer, total As Double)
        Me.AddCatch(zone, grp.ToString("00"), year, total)
    End Sub

    Public Sub AddCatch(zone As String, grp As String, year As Integer, total As Double)
        Dim key As String = zone & "|" & Me.SaupGroupName(grp)
        Dim data() As Double
        If Not Me.m_catches.ContainsKey(key) Then
            ReDim data(Me.m_core.nEcospaceYears)
            Me.m_catches(key) = data
        Else
            data = Me.m_catches(key)
        End If
        data(year) += total
    End Sub

    Public Function Write(filename As String) As Boolean

        Try
            Dim keys As String() = Me.m_catches.Keys.ToArray()
            Array.Sort(keys, New cCSVKeyOrderSortThingy())

            Using sw As New StreamWriter(filename)
                If Me.m_core.SaveWithFileHeader Then
                    Dim u As New cUnits(Me.m_core)

                    Dim extra As New Dictionary(Of String, String)
                    extra("GroupNames") = "SAUP"
                    extra("Values") = "Ecospace total catch per year x Zone|Group"
                    extra("Units") = u.ToString(cUnits.Currency & " x " & cUnits.Area)
                    sw.Write(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace, extraFields:=extra))
                    sw.WriteLine()
                End If

                ' CSV header row
                sw.Write("Year")
                For i As Integer = 0 To keys.Count - 1
                    sw.Write("," & keys(i))
                Next
                sw.WriteLine()

                ' Data
                For iYear As Integer = 1 To Me.m_core.nEcospaceYears
                    sw.Write(CStr(Me.m_core.EcosimFirstYear + iYear - 1))
                    For i As Integer = 0 To keys.Count - 1
                        sw.Write("," & cStringUtils.FormatDouble(Me.m_catches(keys(i))(iYear), iNumDigits:=2))
                    Next
                    sw.WriteLine()
                Next

                sw.Flush()
                sw.Close()
            End Using

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Returns the SAUP group name for an EcoOcean group. The reference time series use the SAUP group names
    ''' </summary>
    ''' <param name="grp"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' The group structure implemented here is hard-coded to the ISIMIP3a group reshuffle.
    ''' </remarks>
    Private Function SaupGroupName(grp As String) As String
        Return grp
        Select Case grp
            Case "07" : Return "pelagic<30cm"
            Case "08" : Return "pelagic30-90cm"
            Case "09" : Return "pelagic>=90cm"
            Case "10" : Return "demersal<30cm"
            Case "11" : Return "demersal30-90cm"
            Case "12" : Return "demersal>=90cm"
            Case "13" : Return "bathypelagic<30cm"
            Case "14" : Return "bathypelagic>=90cm"
            Case "15" : Return "bathypelagic30-90cm"
            Case "16" : Return "bathydemersal<30cm"
            Case "17" : Return "bathydemersal30-90cm"
            Case "18" : Return "bathydemersal>=90cm"
            Case "19" : Return "benthopelagic<30cm"
            Case "20" : Return "benthopelagic30-90cm"
            Case "21" : Return "benthopelagic>=90cm"
            Case "22" : Return "reef-associated<30cm"
            Case "23" : Return "reef-associated30-90cm"
            Case "24" : Return "reef-associated>=90cm"
            Case "25" : Return "shark<90cm"
            Case "26" : Return "shark>=90cm"
            Case "27" : Return "rays<90cm"
            Case "28" : Return "rays>=90cm"
            Case "29" : Return "flatfish<90cm"
            Case "30" : Return "flatfish>=90cm"
            Case "31" : Return "demersalmollusc"
            Case "32" : Return "cephalopods"
            Case "33" : Return "lobsterscrab"
            Case "34" : Return "shrimp"
            Case "35" : Return "krill"
            Case "all"
                ' NOP
            Case Else
                Debug.Assert(False)
        End Select

        Return grp
    End Function

End Class
