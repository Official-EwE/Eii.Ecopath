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
Imports System
Imports System.Diagnostics
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Collections.Generic
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace Utilities

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class offering string utilities.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cStringUtils

        ''' <summary><para>If true, CSV formatting is more restrictive than usual.
        ''' <list type="bullet"><item>headers will 
        ''' only be allowed to contain characters, numbers and underscores. All 
        ''' characters not matching this criteria will be replaced by underscores. 
        ''' Tools such as ArcGIS require this type of CSV formatting.</item>
        ''' </list>
        ''' </para>
        ''' </summary>
        Public Shared Property StrictCSVFormatting As Boolean = False

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Split function that supports text qualifiers.
        ''' </summary>
        ''' <param name="strExpression">String to split.</param>
        ''' <param name="strDelimiter">Delimiting character to split by.</param>
        ''' <param name="strQualifier">String qualifier, such as single or double quotes. Qualified string
        ''' segments will not be subdivided by delimiting characters.</param>
        ''' <returns>An array of strings.</returns>
        ''' <remarks>
        ''' Provided for backward compatibility reasons.
        ''' </remarks>
        ''' ---------------------------------------------------------------------------
        Public Shared Function SplitQualified(ByVal strExpression As String, _
                                              ByVal strDelimiter As String, _
                                              Optional ByVal strQualifier As String = """") As String()
            Return cStringUtils.SplitQualified(strExpression, strDelimiter(0), strQualifier(0))
        End Function


        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Split function that supports text qualifiers.
        ''' </summary>
        ''' <param name="strExpression">String to split.</param>
        ''' <param name="cDelimiter">Delimiting character to split by.</param>
        ''' <param name="cQualifier">String qualifier, such as single or double quotes. Qualified string
        ''' segments will not be subdivided by delimiting characters.</param>
        ''' <returns>An array of strings.</returns>
        ''' <remarks>
        ''' REgEx splitting is too slow. Replaced by a self-written, much faster method
        ''' </remarks>
        ''' ---------------------------------------------------------------------------
        Public Shared Function SplitQualified(ByVal strExpression As String, _
                                              ByVal cDelimiter As Char, _
                                              Optional ByVal cQualifier As Char = """"c) As String()

#If 0 Then
            ' Original code by Larry Steinle (http://www.codeproject.com/script/Articles/list_articles.asp?userid=2146039),
            ' obtained from "Split Function that Supports Text Qualifiers", http://www.codeproject.com/useritems/TextQualifyingSplit.asp

            Dim rxExpression As Regex = Nothing
            Dim strPattern As String = ""
            Dim rxo As RegexOptions = RegexOptions.Compiled Or RegexOptions.Multiline

            ' Build reg ex pattern
            strPattern = String.Format("[{0}](?=(?:[^{1}]*[{1}][^{1}]*[{1}])*(?![^{1}]*[{1}]))", Regex.Escape(cDelimiter), Regex.Escape(cQualifier))
            ' Build reg expression
            rxExpression = New Regex(strPattern, rxo)
            ' Execute
            Return rxExpression.Split(strExpression)
#Else
            Dim lstr As New List(Of String)
            Dim i, j As Integer
            Dim chrs() As Char = New Char() {cDelimiter, cQualifier}
            Dim bQuoted As Boolean = False

            j = strExpression.IndexOfAny(chrs)
            While j > -1
                If (strExpression(j) = cQualifier) Then
                    j = strExpression.IndexOf(cQualifier, j + 1)
                    If (j > -1) Then j = strExpression.IndexOfAny(chrs, j + 1)
                Else
                    lstr.Add(strExpression.Substring(i, j - i))
                    i = j + 1
                    j = strExpression.IndexOfAny(chrs, i)
                End If
            End While
            lstr.Add(strExpression.Substring(i).Replace(cQualifier, ""))
            Return lstr.ToArray
#End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the first &lt;a href=&quot;..&gt; hyperlink within a string.
        ''' </summary>
        ''' <param name="strIn">The string to scan for hyperlinks.</param>
        ''' <param name="strOut">The input string with first hyperlink removed if
        ''' <paramref name="bStripLink"/> is set to True.</param>
        ''' <param name="iStart">The start position of the hyperlink in <paramref name="strOut"/>.</param>
        ''' <param name="iEnd">The end position of the hyperlink in <paramref name="strOut"/>.</param>
        ''' <returns>An hyperlink, or an empty string if no such link was found.</returns>
        ''' <remarks>This code is very simple, and does not use regular expressions 
        ''' for performance reasons. Detection is limited to the direct sequence 'a href=' only.</remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function Hyperlink(ByVal strIn As String, _
                                         ByRef strOut As String, ByRef iStart As Integer, ByRef iEnd As Integer, _
                                         Optional ByVal bStripLink As Boolean = True) As String

            Dim strLink As String = ""
            Dim i, j As Integer
            Dim quotes As Char() = New Char() {""""c, "'"c}
            iStart = -1
            iEnd = -1

            i = strIn.IndexOf("<a href=", StringComparison.CurrentCultureIgnoreCase)
            If i > -1 Then
                Dim sbOut As New StringBuilder()
                If (i > 0) Then sbOut.Append(strIn.Substring(0, i))

                If bStripLink Then iStart = i Else iStart = j

                i = strIn.IndexOfAny(quotes, i + 8)
                j = strIn.IndexOfAny(quotes, i + 1)
                If (i > 0 And j > i) Then strLink = strIn.Substring(i + 1, j - i - 1)

                i = strIn.IndexOf(">"c, j)
                j = strIn.IndexOf("</a>", i + 1, StringComparison.CurrentCultureIgnoreCase)
                If (i > 0 And j > i) Then
                    sbOut.Append(strIn.Substring(i + 1, j - i - 1))
                    If bStripLink Then iEnd = sbOut.Length Else iEnd = j + 4
                    sbOut.Append(strIn.Substring(j + 4))
                End If
                strOut = sbOut.ToString
            End If

            If String.IsNullOrWhiteSpace(strLink) Or (iEnd = -1) Then
                iStart = -1
                iEnd = -1
                strOut = strIn
            End If

            If Not bStripLink Then
                strOut = strIn
            End If

            Return strLink

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number that exceeds the highest number in a range of 
        ''' existing autonumbered strings by one.
        ''' </summary>
        ''' <param name="astrItems">Existing autonumbered strings.</param>
        ''' <param name="strMask">Mask used to create the autonumbered strings.</param>
        ''' <param name="strMaskNumberPlaceholder">Placeholder for the number in 
        ''' the <paramref name="strMask">mask</paramref>.</param>
        ''' <returns>An integer value.</returns>
        ''' <remarks type="sidenote">
        ''' I found that using regular expressions did not really pay off as an
        ''' alternative to this maybe clumsy mothodology. Feel free to improve.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetNextNumber(ByVal astrItems() As String, ByVal strMask As String, _
                Optional ByVal strMaskNumberPlaceholder As String = "{0}") As Integer

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrEmpty(strMask), "Mask cannot be emtpy")
            Debug.Assert(Not String.IsNullOrEmpty(strMaskNumberPlaceholder), "Number placeholder cannot be emtpy")
            Debug.Assert(strMask.IndexOf(strMaskNumberPlaceholder) > -1, "Mask must contain number placeholder")

            Dim iMaskLength As Integer = 0 ' Number of chars in the mask
            Dim iMaskLeft As Integer = 0 ' Number of mask chars to the left side of the number placeholder
            Dim iMaskRight As Integer = 0 ' Number of mask chars to the right side of the number placeholder
            Dim strItem As String = "" ' Item string to analyze
            Dim iItemLength As Integer = 0 '  Number of chars in the item string
            Dim bAssessItem As Boolean = True ' States whether a given item is likely to be created with the format mask
            Dim strNumber As String = "" ' Number string extracted from items
            Dim iMax As Integer = 0 ' The max number found

            If (Not Object.ReferenceEquals(astrItems, Nothing)) Then

                ' Give this a sensible start value
                iMax = astrItems.Length

                ' Analyze mask for number placeholder
                iMaskLength = strMask.Length
                iMaskLeft = strMask.IndexOf(strMaskNumberPlaceholder)
                iMaskRight = iMaskLength - (iMaskLeft + strMaskNumberPlaceholder.Length)

                ' Try to determine the max number in each of the provided strings
                For iItem As Integer = 0 To astrItems.Length - 1
                    ' Get next string
                    strItem = astrItems(iItem)
                    ' Determine its length
                    iItemLength = strItem.Length

                    ' Assess if this item could have been generated with the format mask
                    ' - Does the item have sufficient length?
                    bAssessItem = (iItemLength > (iMaskLeft + iMaskRight))

                    ' Does the item contain all mask characters other than the number placeholder chars?
                    ' - Compare characters to the left of the likely location of the number
                    If ((bAssessItem = True) And (iMaskLeft > 0)) Then
                        ' Accept the item when it contains exactly the same chars as the mask, case independent
                        bAssessItem = strItem.StartsWith(strMask.Substring(0, iMaskLeft), StringComparison.CurrentCultureIgnoreCase)
                    End If
                    ' - Compare characters to the right of the likely location of the number
                    If (bAssessItem And iMaskRight > 0) Then
                        ' Accept the item when it contains exactly the same chars as the mask, case independent
                        bAssessItem = strItem.EndsWith(strMask.Substring(iMaskLength - iMaskRight), StringComparison.CurrentCultureIgnoreCase)
                    End If

                    ' Is this still likely to be a string generated with the mask?
                    If (bAssessItem) Then
                        ' #Yes: Attempt to extract a number
                        strNumber = astrItems(iItem).Substring(iMaskLeft, iItemLength - (iMaskLeft + iMaskRight))
                        Try
                            ' Conversion to Int may cause arithmic overflows etc so let's wear proper protection
                            iMax = Math.Max(iMax, Integer.Parse(strNumber))
                        Catch ex As Exception
                            ' Kaboom! Whoah, ignore this string, it's trouble.
                        End Try
                    End If
                Next iItem
            End If

            ' And yes, it COULD crash here if the iMax happened to hold Integer.MaxValue....
            Return (iMax + 1)

        End Function

        Public Shared Function BeginsWithOneOf(ByVal strSrc As String, ByVal astrCompareTo() As String, Optional ByVal bIgnoreCase As Boolean = True) As Boolean
            For Each strCompareTo As String In astrCompareTo
                If BeginsWith(strSrc, strCompareTo, bIgnoreCase) Then Return True
            Next
            Return False
        End Function

        Public Shared Function BeginsWith(ByVal strSrc As String, ByVal strCompareTo As String, Optional ByVal bIgnoreCase As Boolean = True) As Boolean
            Dim iLen As Integer = Math.Min(strSrc.Length, strCompareTo.Length)

            strSrc = strSrc.Substring(0, iLen)
            strCompareTo = strCompareTo.Substring(0, iLen)
            Return String.Compare(strSrc, strCompareTo, bIgnoreCase) = 0

        End Function

        Public Shared Function EndsWith(ByVal strSrc As String, ByVal strCompareTo As String, Optional ByVal bIgnoreCase As Boolean = True) As Boolean
            Dim iLen As Integer = Math.Min(strSrc.Length, strCompareTo.Length)

            strSrc = strSrc.Substring(strSrc.Length - iLen, iLen)
            strCompareTo = strCompareTo.Substring(0, iLen)
            Return String.Compare(strSrc, strCompareTo, bIgnoreCase) = 0

        End Function

        Public Shared Function Shift(ByVal strIn As String) As String
            Dim strOut As String = ""
            For Each c As Char In strIn.ToCharArray
                strOut += Convert.ToChar(Convert.ToByte(c) - 1)
            Next
            Return strOut
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts an arabic value into a roman representation.
        ''' </summary>
        ''' <param name="nArabicValue">The value to convert.</param>
        ''' <returns>A number in roman format, in upper case.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToRoman(ByVal nArabicValue As Integer) As String

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a string into an targeted type using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber">The number to convert.</param>
        ''' <param name="typeTarget">The target type.</param>
        ''' <param name="strDecimalSeparator">Separator for decimals.</param>
        ''' <param name="strThousandsSeparator">Separator for thousands (a.k.a digit grouping separator)</param>
        ''' <param name="objNullValue">Value to return in case parse failed.</param>
        ''' <returns>An number.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToNumber(ByVal strNumber As String, _
                                                ByVal typeTarget As Type, _
                                                Optional ByVal objNullValue As Object = -9999, _
                                                Optional ByVal strDecimalSeparator As String = ".", _
                                                Optional ByVal strThousandsSeparator As String = "") As Object
            If typeTarget Is GetType(Single) Then
                Return ConvertToSingle(strNumber, CSng(objNullValue), strDecimalSeparator, strThousandsSeparator)
            ElseIf typeTarget Is GetType(Double) Then
                Return ConvertToDouble(strNumber, CDbl(objNullValue), strDecimalSeparator, strThousandsSeparator)
            End If
            Return ConvertToInteger(strNumber, CInt(objNullValue), strDecimalSeparator, strThousandsSeparator)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a string into an integer value using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber">The number to convert.</param>
        ''' <param name="strDecimalSeparator">Separator for decimals.</param>
        ''' <param name="strThousandsSeparator">Separator for thousands (a.k.a digit grouping separator)</param>
        ''' <param name="iNullValue">Value to return in case parse failed.</param>
        ''' <returns>An integer value.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToInteger(ByVal strNumber As String, _
                                                Optional ByVal iNullValue As Integer = -9999, _
                                                Optional ByVal strDecimalSeparator As String = ".", _
                                                Optional ByVal strThousandsSeparator As String = "") As Integer

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try

                    Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
                    Dim iValue As Integer = iNullValue

                    ni.NumberDecimalSeparator = strDecimalSeparator
                    ni.NumberGroupSeparator = strThousandsSeparator

                    If Integer.TryParse(strNumber, NumberStyles.Any, ni, iValue) Then
                        Return iValue
                    End If

                Catch ex As Exception

                End Try

            End If

            Return iNullValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a string into a single value using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber">The number to convert.</param>
        ''' <param name="strDecimalSeparator">Separator for decimals.</param>
        ''' <param name="strThousandsSeparator">Separator for thousands (a.k.a digit grouping separator)</param>
        ''' <param name="sNullValue">Value to return in case parse failed.</param>
        ''' <returns>A single value.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToSingle(ByVal strNumber As String, _
                                               Optional ByVal sNullValue As Single = -9999.0!, _
                                               Optional ByVal strDecimalSeparator As String = ".", _
                                               Optional ByVal strThousandsSeparator As String = "") As Single

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try
                    Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
                    Dim sValue As Single = sNullValue

                    ni.NumberDecimalSeparator = strDecimalSeparator
                    ni.NumberGroupSeparator = strThousandsSeparator

                    If Single.TryParse(strNumber, NumberStyles.Any, ni, sValue) Then
                        Return sValue
                    End If

                Catch ex As Exception
                    ' Whoah!
                End Try

            End If

            Return sNullValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a string into a single value using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber">The number to convert.</param>
        ''' <param name="strDecimalSeparator">Separator for decimals.</param>
        ''' <param name="strThousandsSeparator">Separator for thousands (a.k.a digit grouping separator)</param>
        ''' <param name="dNullValue">Value to return in case parse failed.</param>
        ''' <returns>A double value.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToDouble(ByVal strNumber As String, _
                                               Optional ByVal dNullValue As Double = -9999.0#, _
                                               Optional ByVal strDecimalSeparator As String = ".", _
                                               Optional ByVal strThousandsSeparator As String = "") As Double

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try

                    Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
                    Dim dValue As Double = dNullValue

                    ni.NumberDecimalSeparator = strDecimalSeparator
                    ni.NumberGroupSeparator = strThousandsSeparator

                    If Double.TryParse(strNumber, NumberStyles.Any, ni, dValue) Then
                        Return dValue
                    End If
                Catch ex As Exception

                End Try

            End If
            Return dNullValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a number into a string using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatNumber(ByVal value As Object, _
                                            Optional ByVal strDecimalSeparator As String = ".", _
                                            Optional ByVal strThousandsSeparator As String = "") As String

            If TypeOf value Is Single Then
                Return FormatSingle(CSng(value), strDecimalSeparator, strThousandsSeparator)
            ElseIf TypeOf value Is Double Then
                Return FormatDouble(CDbl(value), strDecimalSeparator, strThousandsSeparator)
            End If
            Return FormatInteger(CInt(value), strDecimalSeparator, strThousandsSeparator)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts an integer value into a string using
        ''' the fixed EwE number format of decimal points, using custom decimal and
        ''' thousands separators.
        ''' </summary>
        ''' <param name="iValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatInteger(ByVal iValue As Integer, _
                                             Optional ByVal strDecimalSeparator As String = ".", _
                                             Optional ByVal strThousandsSeparator As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSeparator
            ni.NumberGroupSeparator = strThousandsSeparator

            Return Convert.ToString(iValue, ni)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a single value into a string using
        ''' the fixed EwE number format of decimal points, using custom decimal and
        ''' thousands separators.
        ''' </summary>
        ''' <param name="sValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatSingle(ByVal sValue As Single, _
                                            Optional ByVal strDecimalSeparator As String = ".", _
                                            Optional ByVal strThousandsSeparator As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSeparator
            ni.NumberGroupSeparator = strThousandsSeparator

            Return Convert.ToString(sValue, ni)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a double value into a string using
        ''' the fixed EwE number format of decimal points, using custom decimal and
        ''' thousands separators.
        ''' </summary>
        ''' <param name="dValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatDouble(ByVal dValue As Double, _
                                            Optional ByVal strDecimalSeparator As String = ".", _
                                            Optional ByVal strThousandsSeparator As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.CurrentCulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSeparator
            ni.NumberGroupSeparator = strThousandsSeparator

            Return Convert.ToString(dValue, ni)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a number to decimal degree notation (hours, minutes and seconds).
        ''' </summary>
        ''' <param name="dValue">The value to convert.</param>
        ''' <returns>The number in a decimal degree notation.</returns>
        ''' <remarks>
        ''' http://www.freevbcode.com/ShowCode.asp?ID=8179
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatDegrees(ByVal dValue As Double, _
                                             Optional strDegreeSymbol As String = "°", _
                                             Optional strMinuteSymbol As String = "’", _
                                             Optional strSeconds As String = """") As String
            dValue = Math.Abs(dValue)

            Dim dMinutes As Double = (dValue - Math.Truncate(dValue)) * 60
            Dim dSeconds As Double = (dMinutes - Math.Truncate(dMinutes)) * 60
            Dim sbResult As New StringBuilder()

            sbResult.Append(Math.Truncate(dValue).ToString())
            sbResult.Append(strDegreeSymbol)
            sbResult.Append(Math.Truncate(dMinutes).ToString())
            sbResult.Append(strMinuteSymbol)
            sbResult.Append(String.Format("{0:##.0000}", dSeconds))
            sbResult.Append(strSeconds)

            Return sbResult.ToString()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Format a date.
        ''' </summary>
        ''' <param name="dtValue">The date to format.</param>
        ''' <param name="strFormat">Optional date formatting flag (http://msdn.microsoft.com/en-us/library/zdtaw1bw%28v=vs.110%29.aspx)</param>
        ''' <returns>A date in en-US format.</returns>
        ''' <remarks>
        ''' http://www.w3.org/TR/NOTE-datetime
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatDate(ByVal dtValue As DateTime, _
                                          Optional ByVal strFormat As String = "dd/MM/yyyy") As String
            Return dtValue.ToString(strFormat)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read a date from a en-US formatted string.
        ''' </summary>
        ''' <param name="strDate">The date to read.</param>
        ''' <param name="strFormat">Optional date formatting flag (http://msdn.microsoft.com/en-us/library/zdtaw1bw%28v=vs.110%29.aspx)</param>
        ''' <returns>The date, of Date.MinValue if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToDate(ByVal strDate As String, _
                                              Optional ByVal strFormat As String = "dd/MM/yyyy") As DateTime
            Dim dt As DateTime
            If (DateTime.TryParseExact(strDate, strFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, dt)) Then
                Return dt
            End If
            Return Date.MinValue
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Method for determining if a string represents a valid email address.
        ''' </summary>
        ''' <param name="strEmail">Email address to validate</param>
        ''' <returns>True is valid, false if not valid</returns>
        ''' <remarks>
        ''' Uses regular expressions in this check, as it is a more thorough
        ''' way of checking an address provided.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function IsValidEmail(ByVal strEmail As String) As Boolean
            'regular expression pattern for valid email
            'addresses, allows for the following domains:
            'com,edu,info,gov,int,mil,net,org,biz,name,museum,coop,aero,pro,tv
            Dim strPattern As String = "^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\." & _
                                       "(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$"
            Dim regexCheck As New Regex(strPattern, RegexOptions.IgnorePatternWhitespace)
            Dim bIsEmailAddress As Boolean = False

            If Not String.IsNullOrEmpty(strEmail) Then
                bIsEmailAddress = regexCheck.IsMatch(strEmail)
            End If

            Return bIsEmailAddress
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a byte array to a string of hexadecimal numbers.
        ''' </summary>
        ''' <param name="bytes"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToHexString(ByVal bytes() As Byte) As String
            Dim sbHex As New StringBuilder()
            If (bytes IsNot Nothing) Then
                ' Convert public token to string
                For i As Integer = 0 To bytes.GetLength(0) - 1
                    sbHex.Append(String.Format("{0:x2}", bytes(i)))
                Next
            End If
            Return sbHex.ToString
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert a string into a base64 MD5 hash.
        ''' </summary>
        ''' <param name="strSrc">The string to hash.</param>
        ''' <returns>A base64 MD5 hash.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GenerateHash(ByVal strSrc As String) As String
            ' Create an encoding object to ensure the encoding standard for the source text
            Dim enc As New UnicodeEncoding
            ' Retrieve a byte array based on the source text
            Dim abData() As Byte = enc.GetBytes(strSrc)
            ' Instantiate an MD5 Provider object
            Dim Md5 As New MD5CryptoServiceProvider
            ' Compute the hash value from the source
            Dim abHash() As Byte = Md5.ComputeHash(abData)
            ' Return string representation
            Return Convert.ToBase64String(abHash)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' String truncation method, blatantly copied from 
        ''' http://www.codeproject.com/KB/vb/NewPathCompactPath.aspx
        ''' </summary>
        ''' <param name="strSrc">The string to truncate with path ellipses.</param>
        ''' <param name="iWidth">Allowed width of the string in pixels.</param>
        ''' <param name="ft">The font to measure the string with.</param>
        ''' <param name="tfFlags">Optional string format flags</param>
        ''' <returns>A truncated string.</returns>
        ''' <remarks>Note that this method does not modify the original string.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function CompactString(ByVal strSrc As String, _
                                             ByVal iWidth As Integer, _
                                             ByVal ft As Font, _
                                             Optional ByVal tfFlags As Windows.Forms.TextFormatFlags = TextFormatFlags.SingleLine Or TextFormatFlags.PathEllipsis Or TextFormatFlags.ModifyString) As String

            If (String.IsNullOrWhiteSpace(strSrc)) Then Return ""

            Dim strResult As String = String.Copy(strSrc)
            TextRenderer.MeasureText(strResult, ft, New Size(iWidth, 0), tfFlags Or TextFormatFlags.ModifyString)
            Return strResult

        End Function

        Private Shared CSV_SEPARATORCHARS As Char() = New Char() {","c, " "c}

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Format a value for use in a CSV file.
        ''' </summary>
        ''' <param name="objValue">The value to format.</param>
        ''' <param name="cQuote">Optional quote character to use for wrapping the value.</param>
        ''' <returns>A field fit for display in a CSV file.</returns>
        ''' <remarks>
        ''' <para>Numbers will be en-US formatted.</para>
        ''' <para>Double quotes will be removed.</para>
        ''' <para>Values containing potential CSV separator characters will be encapsulated in double quotes.</para>
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function ToCSVField(ByVal objValue As Object, _
                                          Optional ByVal cQuote As Char = """"c) As String

            Dim strValue As String = ""

            If (objValue Is Nothing) Then Return strValue

            If (TypeOf (objValue) Is String) Then
                strValue = CStr(objValue)
                If (cStringUtils.StrictCSVFormatting) Then
                    Dim sb As New StringBuilder()
                    For i As Integer = 0 To strValue.Length - 1
                        Dim c As Char = strValue(i)
                        If (Not Char.IsNumber(c)) And Not (Char.IsLetter(c)) And (Not c = "_"c) Then
                            sb.Append("_"c)
                        Else
                            sb.Append(c)
                        End If
                    Next
                    strValue = sb.ToString()
                End If
            ElseIf (TypeOf (objValue) Is DateTime) Then
                strValue = cStringUtils.FormatDate(DirectCast(objValue, DateTime))
            Else
                strValue = cStringUtils.FormatNumber(objValue)
            End If

            If strValue.IndexOf(""""c) > 0 Then
                strValue = strValue.Replace("""", "")
            End If
            If strValue.IndexOfAny(CSV_SEPARATORCHARS) > 0 Then
                strValue = cQuote & strValue & cQuote
            End If

            Return strValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts an incoming string to UTF-8 encoding.
        ''' </summary>
        ''' <param name="strIn">The string to convert.</param>
        ''' <param name="encIn">The current encoding of <paramref name="strIn"/>.</param>
        ''' <returns>A UTF-8 encoded version of the string.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToUTF8(ByVal strIn As String, _
                                      ByVal encIn As Encoding) As String
            ' Special cases
            strIn = strIn.Replace("²"c, "2"c)
            strIn = strIn.Replace("³"c, "3"c)
            ' Shazaam
            Dim data() As Byte = encIn.GetBytes(strIn)
            Return Encoding.UTF8.GetString(data)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts an incoming string to UTF-8 encoding, assuming that the
        ''' incoming string encoded as ASCII (.NET default).
        ''' </summary>
        ''' <param name="strIn">The string to convert.</param>
        ''' <returns>A UTF-8 encoded version of the string.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToUTF8(ByVal strIn As String) As String
            Return cStringUtils.ToUTF8(strIn, Encoding.ASCII)
        End Function

        ''' <summary>Default string split delimiters, in order of decreasing relevance.</summary>
        Public Shared c_DELIMITERS As Char() = New Char() {Convert.ToChar(Keys.Tab), ";"c, Convert.ToChar(Keys.Space)}

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the most likely delimiter character in a string.
        ''' </summary>
        ''' <param name="strIn">The string to explore.</param>
        ''' <param name="cQualifier">Qualifier character for enveloping non-splittable strings.</param>
        ''' <param name="candidates">An array of possible delimiter characters. If 
        ''' an empty array is provided or this parameter is omitted, the default 
        ''' array <see cref="c_DELIMITERS"/> is used.</param>
        ''' <returns>The most likely character used to split a string. If no
        ''' candidate can be found the default comma (,) is returned.</returns>
        ''' <remarks><para>This method splits <paramref name="strIn"/> by each 
        ''' delimiter character in <paramref name="candidates"/> in order. If a 
        ''' split returns more than one sub-string the split character is returned.
        ''' If no split was possible the default comma character is returned.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function FindStringDelimiter(ByVal strIn As String, _
                                                   Optional ByVal cQualifier As Char = """"c, _
                                                   Optional ByVal candidates As Char() = Nothing) As Char

            ' Ensure that there are candidate delimiters
            If (candidates Is Nothing) Then
                candidates = c_DELIMITERS
            End If

            If candidates.Length = 0 Then
                candidates = c_DELIMITERS
            End If

            ' Did receive any data to split?
            If Not String.IsNullOrWhiteSpace(strIn) Then
                ' #Yes: find most relevant split character
                For Each c As Char In candidates
                    ' Does candidate occur in string?
                    If strIn.IndexOf(c) >= 0 Then
                        ' #Yes: Does split yield more than one substring?
                        If (cStringUtils.SplitQualified(strIn, c, cQualifier).Length > 1) Then
                            ' #Yes: return this character
                            Return c
                        End If
                    End If
                Next
            End If

            ' Return default
            Return ","c

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the current time as a string to be used in file names.
        ''' </summary>
        ''' <remarks>The time stamp is formatted as 'year-month-day hour-minute-second'.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function Now() As String
            Return Date.Now.ToString("y-MM-dd HH-mm-ss")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <para>Computes the Damerau-Levenshtein Distance between two strings. This method
        ''' Includes an optional threshold which can be used to indicate the maximum 
        ''' allowable distance between the two strings.</para>
        ''' <para>http://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance</para>
        ''' </summary>
        ''' <param name="strSrc">The first string to compare.</param>
        ''' <param name="strTarget">The second string to compare.</param>
        ''' <param name="iThreshold">Maximum allowable distance</param>
        ''' <returns>Integer.MaxValue if the threshhold is exceeded; otherwise the Damerau-Leveshteim 
        ''' distance between the strings.</returns>
        ''' <remarks>
        ''' Converted from a frigtheningly smart piece of code by http://stackoverflow.com/users/842685/jmh-gr
        ''' http://stackoverflow.com/questions/9453731/how-to-calculate-distance-similarity-measure-of-given-2-strings/9454016#9454016
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function DamerauLevenshteinDistance(ByVal strSrc As String, _
                                                          ByVal strTarget As String, _
                                                          Optional ByVal iThreshold As Integer = Integer.MaxValue) As Integer

            Dim length1 As Integer = strSrc.Length
            Dim length2 As Integer = strTarget.Length

            ' Return trivial case - difference in string lengths exceeds threshhold
            If (Math.Abs(length1 - length2) > iThreshold) Then Return Integer.MaxValue

            ' Ensure arrays [i] / length1 use shorter length 
            If (length1 > length2) Then
                Dim str As String = strTarget : strTarget = strSrc : strSrc = str
                Dim i As Integer = length1 : length1 = length2 : length2 = i
            End If

            Dim maxi As Integer = length1
            Dim maxj As Integer = length2

            Dim dCurrent(maxi + 1) As Integer
            Dim dMinus1(maxi + 1) As Integer
            Dim dMinus2(maxi + 1) As Integer
            Dim dSwap() As Integer = Nothing

            For i As Integer = 0 To maxi : dCurrent(i) = i : Next

            Dim jm1 As Integer = 0
            Dim im1 As Integer = 0
            Dim im2 As Integer = -1

            For j As Integer = 1 To maxj

                ' Rotate
                dSwap = dMinus2
                dMinus2 = dMinus1
                dMinus1 = dCurrent
                dCurrent = dSwap

                ' Initialize
                Dim minDistance As Integer = Integer.MaxValue
                dCurrent(0) = j
                im1 = 0
                im2 = -1

                For i As Integer = 1 To maxi

                    Dim cost As Integer = 1
                    If (strSrc(im1) = strTarget(jm1)) Then cost = 0

                    Dim del As Integer = dCurrent(im1) + 1
                    Dim ins As Integer = dMinus1(i) + 1
                    Dim [sub] As Integer = dMinus1(im1) + cost

                    Dim min As Integer = 0
                    If (del > ins) Then
                        If (ins > [sub]) Then
                            min = [sub]
                        Else
                            min = ins
                        End If
                    Else
                        If (del > [sub]) Then
                            min = [sub]
                        Else
                            min = del
                        End If
                    End If

                    If (i > 1 And j > 1) Then
                        If (strSrc(im2) = strTarget(jm1) And strSrc(im1) = strTarget(j - 2)) Then
                            min = Math.Min(min, dMinus2(im2) + cost)
                        End If
                    End If

                    dCurrent(i) = min
                    If (min < minDistance) Then minDistance = min
                    im1 += 1
                    im2 += 1
                Next i
                jm1 += 1
                If (minDistance > iThreshold) Then Return Integer.MaxValue
            Next j

            If dCurrent(maxi) > iThreshold Then Return Integer.MaxValue
            Return dCurrent(maxi)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts an exception with all its nested inner exceptions into a 
        ''' single string.
        ''' </summary>
        ''' <param name="ex"></param>
        ''' <returns>A string</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function UnravelException(ByVal ex As Exception) As String

            Dim sb As New StringBuilder()

            Try
                Do While ex IsNot Nothing
                    sb.AppendLine(ex.Message)
                    ex = ex.InnerException
                Loop
            Catch exKaboom As Exception
                'oooppps
                Debug.Assert(False, exKaboom.Message)
            End Try
            Return sb.ToString

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts a string to proper title case, honouring reading order and
        ''' periods.
        ''' </summary>
        ''' <param name="strExpression">The string to convert.</param>
        ''' <returns>A string in proper title case.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToTitlecase(ByVal strExpression As String) As String

            Dim bR2L As Boolean = cSystemUtils.IsRightToLeft()
            Dim astrBits() As String
            Dim sbOUt As New StringBuilder()

            If bR2L Then
                astrBits = strExpression.Split(New String() {" ."}, System.StringSplitOptions.RemoveEmptyEntries)
            Else
                astrBits = strExpression.Split(New String() {". "}, System.StringSplitOptions.RemoveEmptyEntries)
            End If

            ' Protect all words that are pure upper case. The rest will be turned to lower case
            For i As Integer = 0 To astrBits.Length - 1
                If (String.Compare(astrBits(i), astrBits(i).ToUpper, False) <> 0) Then
                    astrBits(i) = astrBits(i).ToLower()
                End If
            Next

            For i As Integer = 0 To astrBits.Length - 1
                astrBits(i) = astrBits(i).Trim
                If Not String.IsNullOrWhiteSpace(astrBits(i)) Then
                    Dim c As Char() = astrBits(i).Trim.ToCharArray
                    If bR2L Then
                        c(c.Length - 1) = Char.ToUpper(c(c.Length - 1))
                        If (i > 0) Then
                            sbOUt.Append(" .")
                        End If
                    Else
                        c(0) = Char.ToUpper(c(0))
                        If (i > 0) Then
                            sbOUt.Append(". ")
                        End If
                    End If
                    sbOUt.Append(c)
                End If
            Next

            Return sbOUt.ToString()

        End Function

#Region " Replace "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Replace all occurrences of a pattern in a source string with a replacement.
        ''' </summary>
        ''' <param name="strSrc">Source string the replace all instances into.</param>
        ''' <param name="strPattern">The search pattern to replace.</param>
        ''' <param name="strReplacement">The search pattern replacement string.</param>
        ''' <returns>An amphetamine-addicted monk seal.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function ReplaceAll(ByVal strSrc As String, _
                                          ByVal strPattern As String, _
                                          ByVal strReplacement As String) As String

            ' Rerouted
            Return cStringUtils.Replace(strSrc, strPattern, strReplacement, StringComparison.CurrentCulture)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="[String].Replace"/> alternative that offers comparison
        ''' options. This method is significantly faster than RegEx equivalents.
        ''' Implementation adapted from http://www.codeproject.com/Articles/10890/Fastest-C-Case-Insenstive-String-Replace.
        ''' </summary>
        ''' <param name="strSrc">Source string the replace all instances into.</param>
        ''' <param name="strPattern">The search pattern to replace.</param>
        ''' <param name="strReplacement">The search pattern replacement string.</param>
        ''' <param name="comparisonType">The <see cref="StringComparison"/> option to use.</param>
        ''' <returns>A string.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function Replace(ByVal strSrc As String, ByVal strPattern As String, _
                                       ByVal strReplacement As String, ByVal comparisonType As StringComparison) As String

            If String.IsNullOrWhiteSpace(strSrc) Then Return String.Empty

            Dim posCurrent As Integer = 0
            Dim lenPattern As Integer = strPattern.Length
            Dim idxNext As Integer = strSrc.IndexOf(strPattern, comparisonType)
            Dim result As New StringBuilder()

            While idxNext >= 0
                result.Append(strSrc, posCurrent, idxNext - posCurrent)
                result.Append(strReplacement)

                posCurrent = idxNext + lenPattern

                idxNext = strSrc.IndexOf(strPattern, posCurrent, comparisonType)
            End While

            result.Append(strSrc, posCurrent, strSrc.Length - posCurrent)

            Return result.ToString()

        End Function

#End Region ' Replace

#Region " Map array conversions "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert a 2-dimensional (map) array to a string for database storage.
        ''' </summary>
        ''' <param name="data">Data to write to the string.</param>
        ''' <param name="dataDepth">Optional depth data mask to apply. If provided, only 
        ''' water cells or land cells are stored based on the value of <paramref name="bWaterOnly"/>.</param>
        ''' <param name="bWaterOnly">Flag, stating whether only values should be written
        ''' for water cells (true) or land cells (false), as indicated by parameter <paramref name="dataDepth"/>.</param>
        ''' <param name="valueFilter">Value to find in the data and to write to the string,
        ''' or Nothing if any value from the data must be written to the string.</param>
        ''' <param name="valueSet">Value to set, if any.</param>
        ''' <returns>The resulting converted string.</returns>
        ''' <remarks>This code is optimized to include as few characters as possible
        ''' in the output string without having to revert to run-length encoding.
        ''' Rows without any values will be left empty and are only marked by a semi-colon.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function ArrayToString(ByVal data As Array, _
                                             Optional ByVal dataDepth As Integer(,) = Nothing, _
                                             Optional ByVal bWaterOnly As Boolean = True, _
                                             Optional ByVal valueFilter As Object = Nothing, _
                                             Optional ByVal valueSet As Object = Nothing) As String

            ' Can only handle 2-dimensional arrays
            Debug.Assert(data.Rank = 2)

            Dim sb As New StringBuilder()
            Dim sbRow As New StringBuilder()
            Dim bUseCell As Boolean = False
            Dim bHasRowValues As Boolean = False
            Dim value As Object = Nothing
            Dim tData As Type = data.GetType().GetElementType

            ' For all rows
            For i As Integer = 1 To data.GetUpperBound(0) - 1

                ' Start of new rowg
                bHasRowValues = False
                sbRow.Length = 0
                bUseCell = False

                ' For all cols
                For j As Integer = 1 To data.GetUpperBound(1) - 1

                    ' Append separator after last value
                    If bUseCell Then sbRow.Append(","c)

                    ' Ignore land filter?
                    If (dataDepth Is Nothing) Then
                        ' #Yes: use cell
                        bUseCell = True
                    Else
                        ' #No: only use cell if land or water (depeding on bWaterOnly)
                        bUseCell = cSystemUtils.IIF(dataDepth(i, j) > 0, bWaterOnly, Not bWaterOnly)
                    End If

                    If (bUseCell) Then
                        ' Get value
                        value = data.GetValue(i, j)
                        ' Append value in correct type 
                        If tData Is GetType(Boolean) Then
                            ' #Boolean values are stored as 1 (true) and 0 (false)
                            sbRow.Append(cSystemUtils.IIF(CBool(value), "1", "0"))
                            bHasRowValues = bHasRowValues Or (CBool(value))
                        Else
                            ' Is an allowed value?
                            If ((value.Equals(valueFilter) Or (valueFilter Is Nothing))) Then
                                ' #Yes: convert value to a fixed en-US representation text
                                If (valueSet IsNot Nothing) Then value = valueSet
                                sbRow.Append(cStringUtils.FormatNumber(value))
                                bHasRowValues = True
                            End If
                        End If
                    End If
                Next j

                ' Add row if not empty
                If bHasRowValues Then sb.Append(sbRow.ToString)
                ' Add row delimiter
                sb.Append(";"c)

            Next i

            ' Done
            Return sb.ToString

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Read a map from a string, and poulate a 2-dimensional array with this data.
        ''' </summary>
        ''' <param name="strData">The string containing the map.</param>
        ''' <param name="data">The 2-dimensional array to populate.</param>
        ''' <param name="land">Optional land layer to use.</param>
        ''' <param name="bWaterOnly">States whether only water cells (true) or land cells (false) should be written.</param>
        ''' <param name="valueFilter">Optional value to filter map values by. If specified, only map values equalling this
        ''' filter value will be copied to the data array.</param>
        ''' <param name="valueSet">Value to set, if any.</param>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function StringToArray(ByVal strData As String, ByVal data As Array, _
                                             Optional ByVal land As Integer(,) = Nothing, _
                                             Optional ByVal bWaterOnly As Boolean = True, _
                                             Optional ByVal valueFilter As Object = Nothing, _
                                             Optional ByVal valueSet As Object = Nothing) As Boolean

            ' Need 2 dim array
            Debug.Assert(data.Rank = 2)

            Dim astrLines As String() = strData.Replace("""", "").Split(";"c)
            Dim astrValues As String() = Nothing
            Dim iColumn As Integer = 0
            Dim bUseCell As Boolean = False
            Dim value As Object = Nothing
            Dim tData As Type = data.GetType().GetElementType

            ' For all rows
            For i As Integer = 1 To data.GetUpperBound(0) - 1
                ' Still row data left?
                If (i < astrLines.Length) Then

                    ' #Yes: split row into values
                    astrValues = astrLines(i - 1).Split(","c)
                    ' For all cols
                    For j As Integer = 1 To data.GetUpperBound(1) - 1
                        ' Ignore land filter?
                        If (land Is Nothing) Then
                            ' #Yes: use cell
                            bUseCell = True
                        Else
                            ' #No: only use cell if land or water (depeding on bWaterOnly)
                            bUseCell = cSystemUtils.IIF(land(i, j) > 0, bWaterOnly, Not bWaterOnly)
                        End If

                        ' Use cell and there is cell data?
                        If bUseCell And (iColumn < astrValues.Length) Then
                            ' #Yes: is there really, really cell data?
                            If Not String.IsNullOrEmpty(astrValues(iColumn)) Then
                                Try
                                    ' #Yes: get value
                                    If tData Is GetType(Boolean) Then
                                        value = (astrValues(iColumn) = "1")
                                    Else
                                        value = cStringUtils.ConvertToNumber(astrValues(iColumn), tData)
                                    End If

                                    ' Does this value match the value to get if provided?
                                    If (value.Equals(valueFilter) Or (valueFilter Is Nothing)) Then
                                        ' #Yes: update array
                                        If (valueSet IsNot Nothing) Then value = valueSet
                                        data.SetValue(value, i, j)
                                    End If
                                Catch ex As Exception
                                    Return False
                                End Try
                            End If
                            ' Next column
                            iColumn += 1
                        End If
                    Next j
                    ' Reset column count
                    iColumn = 0
                End If
            Next i

            ' Done
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, stating where to find the data filter in a 3D map array.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eFilterIndexTypes As Integer
            FirstIndex = 0
            LastIndex
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert a 3-dimensional (map) array to a string for database storage.
        ''' </summary>
        ''' <param name="data">Data to write to the string.</param>
        ''' <param name="dataDepth">Optional depth data mask to apply. If provided, only 
        ''' water cells or land cells are stored based on the value of <paramref name="bWaterOnly"/>.</param>
        ''' <param name="bWaterOnly">Flag, stating whether only values should be written
        ''' for water cells (true) or land cells (false), as indicated by parameter <paramref name="dataDepth"/>.</param>
        ''' <param name="valueSet">Value to find in the data and to write to the string,
        ''' or Nothing if any value from the data must be written to the string.</param>
        ''' <returns>The resulting converted string.</returns>
        ''' <remarks>This code is optimized to include as few characters as possible
        ''' in the output string without having to revert to run-length encoding.
        ''' Rows without any values will be left empty and are only marked by a semi-colon.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function ArrayToString(ByVal data As Array, _
                                             ByVal iFilter As Integer, _
                                             ByVal filterIndex As eFilterIndexTypes, _
                                             ByVal InRow As Integer, _
                                             ByVal InCol As Integer, _
                                             Optional ByVal dataDepth As Integer(,) = Nothing, _
                                             Optional ByVal bWaterOnly As Boolean = True, _
                                             Optional ByVal valueSet As Object = Nothing) As String

            ' Need 3 dim array
            Debug.Assert(data.Rank = 3)

            Dim sb As New StringBuilder()
            Dim sbRow As New StringBuilder()
            Dim bHasRowValues As Boolean = False
            Dim bUseCell As Boolean = False
            Dim value As Object = Nothing
            Dim tData As Type = data.GetType().GetElementType

            Select Case filterIndex
                Case eFilterIndexTypes.FirstIndex
                    InRow = Math.Min(InRow, data.GetUpperBound(1))
                    InCol = Math.Min(InCol, data.GetUpperBound(2))
                Case eFilterIndexTypes.LastIndex
                    InRow = Math.Min(InRow, data.GetUpperBound(0))
                    InCol = Math.Min(InCol, data.GetUpperBound(1))
            End Select

            ' For all rows
            For i As Integer = 1 To InRow

                ' Start new line
                bHasRowValues = False
                sbRow.Length = 0
                bUseCell = False

                ' For all cols
                For j As Integer = 1 To InCol

                    ' Append separator if already has values on this row
                    If bUseCell Then sbRow.Append(","c)

                    ' Ignore land filter?
                    If (dataDepth Is Nothing) Then
                        ' #Yes: use cell
                        bUseCell = True
                    Else
                        ' #No: only use cell if land or water (depeding on bWaterOnly)
                        bUseCell = cSystemUtils.IIF(dataDepth(i, j) > 0, bWaterOnly, Not bWaterOnly)
                    End If

                    If bUseCell Then

                        ' Get actual cell value
                        Select Case filterIndex
                            Case eFilterIndexTypes.FirstIndex : value = data.GetValue(iFilter, i, j)
                            Case eFilterIndexTypes.LastIndex : value = data.GetValue(i, j, iFilter)
                        End Select


                        If tData Is GetType(Boolean) Then
                            sbRow.Append(cSystemUtils.IIF(CBool(value), "1", "0"))
                            bHasRowValues = bHasRowValues Or (CBool(value))
                        Else
                            ' Is not an allowed value?
                            If ((value.Equals(valueSet) Or (valueSet Is Nothing))) Then
                                sbRow.Append(cStringUtils.FormatNumber(value))
                                bHasRowValues = True
                            End If
                        End If
                    End If
                Next j

                If bHasRowValues Then sb.Append(sbRow.ToString())
                sb.Append(";"c)
            Next i

            ' Done
            Return sb.ToString

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Read a map from a string, and poulate a 3-dimensional array with this data.
        ''' </summary>
        ''' <param name="strData">The string containing the map.</param>
        ''' <param name="iFilter">Index of third dimension to set.</param>
        ''' <param name="filterIndex">Position of third dimension in the array (first, e.g. (#,,) or last (,,#))</param>
        ''' <param name="data">The 2-dimensional array to populate.</param>
        ''' <param name="land">Optional land layer to use.</param>
        ''' <param name="bWaterOnly">States whether only water cells (true) or land cells (false) should be written.</param>
        ''' <param name="valueGet">Optional value to filter map values by. If specified, only map values equalling this
        ''' filter value will be copied to the data array.</param>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function StringToArray(ByVal strData As String, _
                                            ByVal iFilter As Integer, _
                                            ByVal filterIndex As eFilterIndexTypes, _
                                            ByVal data As Array, _
                                            ByVal InRow As Integer, _
                                            ByVal InCol As Integer, _
                                            Optional ByVal land As Integer(,) = Nothing, _
                                            Optional ByVal bWaterOnly As Boolean = True, _
                                            Optional ByVal valueGet As Object = Nothing,
                                            Optional ByVal sMax As Single = Single.MaxValue) As Boolean

            ' Need 3 dim array
            Debug.Assert(data.Rank = 3)

            Dim astrLines As String() = strData.Replace("""", "").Split(";"c)
            Dim astrValues As String() = Nothing
            Dim iColumn As Integer = 0
            Dim value As Object = Nothing
            Dim tData As Type = data.GetType().GetElementType
            Dim bUseValue As Boolean = False

            Select Case filterIndex
                Case eFilterIndexTypes.FirstIndex
                    InRow = Math.Min(InRow, data.GetUpperBound(1))
                    InCol = Math.Min(InCol, data.GetUpperBound(2))
                Case eFilterIndexTypes.LastIndex
                    InRow = Math.Min(InRow, data.GetUpperBound(0))
                    InCol = Math.Min(InCol, data.GetUpperBound(1))
            End Select

            ' For all rows
            For i As Integer = 1 To InRow
                ' Still row data left?
                If (i < astrLines.Length) Then

                    ' #Yes: split row into values
                    astrValues = astrLines(i - 1).Split(","c)
                    ' For all cols
                    For j As Integer = 1 To InCol
                        ' Ignore land filter?
                        If (land Is Nothing) Then
                            ' #Yes: use cell
                            bUseValue = True
                        Else
                            ' #No: only use cell if land or water (depeding on bWaterOnly)
                            bUseValue = cSystemUtils.IIF(land(i, j) > 0, bWaterOnly, Not bWaterOnly)
                        End If

                        ' Use cell and there is cell data?
                        If bUseValue And (iColumn < astrValues.Length) Then
                            ' #Yes: is there really, really cell data?
                            If Not String.IsNullOrEmpty(astrValues(iColumn)) Then
                                Try
                                    ' #Yes: get value
                                    If tData Is GetType(Boolean) Then
                                        value = (astrValues(iColumn) = "1")
                                    Else
                                        value = Math.Min(sMax, CSng(cStringUtils.ConvertToNumber(astrValues(iColumn), tData)))
                                    End If
                                    ' Does this value match the value to get if provided?
                                    If (value.Equals(valueGet) Or (valueGet Is Nothing)) Then
                                        ' #Yes: update array
                                        Select Case filterIndex
                                            Case eFilterIndexTypes.FirstIndex : data.SetValue(value, iFilter, i, j)
                                            Case eFilterIndexTypes.LastIndex : data.SetValue(value, i, j, iFilter)
                                        End Select
                                    End If
                                Catch ex As Exception

                                End Try
                            End If
                            ' Next column
                            iColumn += 1
                        End If
                    Next j
                    ' Reset column count
                    iColumn = 0
                End If
            Next i

            ' Done
            Return True

        End Function

#End Region ' Map array conversions

#Region " Shape data conversions "

        Public Shared Function StringToShape(ByVal strMemo As String, _
                                             ByVal nItems As Integer, _
                                             ByVal sDefault As Single, _
                                             ByVal sData As Single(,), _
                                             ByVal iIndex As Integer) As Boolean

            Dim astrBits As String() = Nothing
            Dim iPts As Integer = 1

            If (Not String.IsNullOrWhiteSpace(strMemo)) Then
                astrBits = strMemo.Trim.Split(CChar(" "))
                iPts = astrBits.Length
                For j As Integer = 1 To Math.Min(nItems, iPts)
                    sData(iIndex, j) = cStringUtils.ConvertToSingle(astrBits(j - 1), sDefault)
                Next
            Else
                sData(iIndex, iPts) = sDefault
            End If

            For j As Integer = iPts + 1 To nItems
                sData(iIndex, j) = sData(iIndex, iPts)
            Next
            Return True

        End Function

#End Region ' Shape data conversions

#Region " Microsoft.VisualBasic alternatives "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a Tab character.
        ''' </summary>
        ''' <remarks>
        ''' The Microsoft.VisualBasic assembly is known to cause problems under Mono.
        ''' For Mono compliance this definition should be used instead.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared ReadOnly Property vbTab As String
            Get
                Return Convert.ToChar(9).ToString
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a Newline character.
        ''' </summary>
        ''' <remarks>
        ''' The Microsoft.VisualBasic assembly is known to cause problems under Mono.
        ''' For Mono compliance this definition should be used instead.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared ReadOnly Property vbNewline As String
            Get
                Return cStringUtils.vbCr
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a carriage return character.
        ''' </summary>
        ''' <remarks>
        ''' The Microsoft.VisualBasic assembly is known to cause problems under Mono.
        ''' For Mono compliance this definition should be used instead.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared ReadOnly Property vbCr As String
            Get
                Return Convert.ToChar(13).ToString
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a line feed character.
        ''' </summary>
        ''' <remarks>
        ''' The Microsoft.VisualBasic assembly is known to cause problems under Mono.
        ''' For Mono compliance this definition should be used instead.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared ReadOnly Property vbLf As String
            Get
                Return Convert.ToChar(10).ToString
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a carriage return + line feed character.
        ''' </summary>
        ''' <remarks>
        ''' The Microsoft.VisualBasic assembly is known to cause problems under Mono.
        ''' For Mono compliance this definition should be used instead.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared ReadOnly Property vbCrLf As String
            Get
                Return Environment.NewLine
            End Get
        End Property

#End Region ' Microsoft.VisualBasic alternatives

    End Class

End Namespace ' Utilities