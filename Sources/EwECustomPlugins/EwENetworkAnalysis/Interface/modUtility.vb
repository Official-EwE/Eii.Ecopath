'==============================================================================
'
' $Log: modUtility.vb,v $
' Revision 1.3  2009/04/28 16:20:55  jeroens
' Fixed graph max axis
' Graph styling done with ZedGraphHelper
' Uses true roman number converter
'
' Revision 1.2  2008/11/25 05:47:34  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:58  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/08/25 19:57:05  sherman
' Converted temp file to app temp files.
'
' Revision 1.11  2008/08/10 01:03:42  jeroens
' Updated to ds smarter open structure
'
' Revision 1.10  2008/08/02 03:04:20  jeroens
' Renamed resources
'
' Revision 1.9  2008/06/18 20:16:03  joeh
' Plot Ascendency on flow in a second pane
'
' Revision 1.8  2007/07/13 17:29:35  joeh
' Change variables to constant
'
' Revision 1.7  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.6  2007/06/26 21:14:28  joeh
' Diable sorting by column
'
' Revision 1.5  2007/06/22 00:35:32  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.4  2007/06/20 18:13:57  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text

Module modUtility
    Public Const DEFAULT_COL_WIDTH As Integer = 70
    Public Const ID_COL_WIDTH As Integer = 25
    Public Const GRP_NAME_COL_WIDTH As Integer = 110
    Public Const FIRST_ROW_HEIGHT As Integer = 45

    'Public Declare Function FindWindow& Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String)
    Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer

    Public Function CRoman(ByVal nArabicValue As Integer) As String

        Dim nThousands As Integer
        Dim nFiveHundreds As Integer
        Dim nHundreds As Integer
        Dim nFifties As Integer
        Dim nTens As Integer
        Dim nFives As Integer
        Dim nOnes As Integer
        Dim sbNumber As New StringBuilder()

        'take the value passed and split it out
        'to values representing the number of
        'ones, tens, hundreds, etc
        nOnes = nArabicValue
        nThousands = nOnes \ 1000
        nOnes = nOnes - nThousands * 1000
        nFiveHundreds = nOnes \ 500
        nOnes = nOnes - nFiveHundreds * 500
        nHundreds = nOnes \ 100
        nOnes = nOnes - nHundreds * 100
        nFifties = nOnes \ 50
        nOnes = nOnes - nFifties * 50
        nTens = nOnes \ 10
        nOnes = nOnes - nTens * 10
        nFives = nOnes \ 5
        nOnes = nOnes - nFives * 5

        'using VB's String function, create
        'a series of strings representing
        'the number of each respective denomination
        sbNumber.Append(New String("M"c, nThousands))

        'handle those cases where the denominator
        'value is on either side of a roman numeral
        If nHundreds = 4 Then
            If nFiveHundreds = 1 Then
                sbNumber.Append("CM")
            Else
                sbNumber.Append("CD")
            End If
        Else
            'not a 4, so create the string
            sbNumber.Append(New String("D"c, nFiveHundreds))
            sbNumber.Append(New String("C"c, nHundreds))
        End If

        If nTens = 4 Then
            If nFifties = 1 Then
                sbNumber.Append("XC")
            Else
                sbNumber.Append("XL")
            End If
        Else
            sbNumber.Append(New String("L"c, nFifties))
            sbNumber.Append(New String("X"c, nTens))
        End If

        If nOnes = 4 Then
            If nFives = 1 Then
                sbNumber.Append("IX")
            Else
                sbNumber.Append("IV")
            End If
        Else
            sbNumber.Append(New String("V"c, nFives))
            sbNumber.Append(New String("I"c, nOnes))
        End If

        Return sbNumber.ToString()

    End Function

    'Public Function CRoman2_ORG(ByVal intArabic As Integer) As String
    '    Select Case intArabic
    '        Case 1
    '            Return "I"
    '        Case 2
    '            Return "II"
    '        Case 3
    '            Return "III"
    '        Case 4
    '            Return "IV"
    '        Case 5
    '            Return "V"
    '        Case 6
    '            Return "VI"
    '        Case 7
    '            Return "VII"
    '        Case 8
    '            Return "VIII"
    '        Case 9
    '            Return "IX"
    '        Case 10
    '            Return "X"
    '        Case 11
    '            Return "XI"
    '        Case 12
    '            Return "XII"
    '        Case 13
    '            Return "XIII"
    '        Case 14
    '            Return "XIV"
    '        Case 15
    '            Return "XV"
    '        Case 16
    '            Return "XVI"
    '        Case 17
    '            Return "XVII"
    '        Case 18
    '            Return "XIX"
    '        Case 19
    '            Return "XX"
    '        Case Else
    '            Return "??"
    '    End Select
    'End Function

    Public Sub SetGridColumnPropertyDefault(ByVal DataGrid As Windows.Forms.DataGridView)
        DataGrid.ColumnHeadersVisible = False
        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            'DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = DEFAULT_COL_WIDTH '110
            DataGrid.Columns(intColIndex).Frozen = False
            DataGrid.Columns(intColIndex).SortMode = Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Public Function IsPlotActive(ByVal Caption As String) As Boolean
        Dim Found As Integer
        'what window to check
        'Select Case Flag
        '    Case 1
        '        Caption = "ECOPATH 3.0 - Pyramid"
        '    Case 2
        '        Caption = "ECOPATH 3.0 - Impacts"
        '    Case 3
        '        'Caption = "Flow Diagram"
        '        Caption = "ECOPATH 3.0 - Flow Diagram"
        '    Case Else
        '        Caption = ""
        'End Select

        'checks the window
        'nong found = FindWindow(0&, Caption)
        Found = FindWindow(vbNullString, Caption)
        'DoEvents
        Return CBool(IIf(Found = 0, False, True))
    End Function

    Public Sub AddCurve(ByVal strName As String, ByVal CurveVar() As Single, ByVal Pane As GraphPane, _
      ByVal MyColor As Color, Optional ByVal Symbol As SymbolType = SymbolType.None)
        Dim List As PointPairList
        Dim iNumPoints As Integer = CurveVar.GetUpperBound(0)

        List = New PointPairList()
        For iTime As Integer = 1 To iNumPoints
            List.Add(iTime, CurveVar(iTime))
        Next
        Pane.AddCurve(strName, List, MyColor, Symbol)

        Pane.XAxis.Scale.Max = iNumPoints
    End Sub

End Module
