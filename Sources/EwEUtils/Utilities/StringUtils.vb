Option Strict On

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Globalization

Namespace Utilities

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class offering string utilities.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class StringUtils

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Split function that supports text qualifiers.
        ''' </summary>
        ''' <param name="strExpression">String to split.</param>
        ''' <param name="strDelimiter">Delimiting character to split by.</param>
        ''' <param name="strQualifier">String qualifier, such as single or double quotes. Qualified string
        ''' segments will not be subdivided by delimiting characters.</param>
        ''' <param name="bIgnoreCase">States whether delimiter and qualifier detection should be case-sensitive.</param>
        ''' <returns>An array of strings.</returns>
        ''' <remarks>
        ''' Original code by Larry Steinle (http://www.codeproject.com/script/Articles/list_articles.asp?userid=2146039),
        ''' obtained from "Split Function that Supports Text Qualifiers", http://www.codeproject.com/useritems/TextQualifyingSplit.asp
        ''' </remarks>
        ''' ---------------------------------------------------------------------------
        Public Shared Function SplitQualified(ByVal strExpression As String, ByVal strDelimiter As String, _
            Optional ByVal strQualifier As String = """", Optional ByVal bIgnoreCase As Boolean = True) As String()

            Dim rxExpression As Regex = Nothing
            Dim strPattern As String = ""
            Dim rxo As RegexOptions = RegexOptions.None

            ' Build reg ex pattern
            strPattern = String.Format("[{0}](?=(?:[^{1}]*[{1}][^{1}]*[{1}])*(?![^{1}]*[{1}]))", Regex.Escape(strDelimiter), Regex.Escape(strQualifier))
            ' Define reg ex options
            rxo = RegexOptions.Compiled Or RegexOptions.Multiline
            If bIgnoreCase Then rxo = rxo Or RegexOptions.IgnoreCase
            ' Build reg expression
            rxExpression = New Regex(strPattern, rxo)
            ' Execute
            Return rxExpression.Split(strExpression)

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
                            iMax = Math.Max(iMax, CInt(Val(strNumber)))
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
                strOut += ChrW(AscW(c) - 1)
            Next
            Return strOut
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' method for truncating a path with elipses
        ''' </summary>
        ''' <param name="strPath">The path to truncate.</param>
        ''' <param name="font">The font to trucate with.</param>
        ''' <returns>
        ''' The truncated path
        ''' </returns>
        ''' <remarks>
        ''' Taken from http://www.dreamincode.net/code/snippet3281.htm
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function TruncatePath(ByVal strPath As String, _
                                            ByVal font As Font, _
                                            ByVal iMaxWidth As Integer) As String

            Dim strPathTruncated As String = ""
            Dim bmp As Bitmap = Nothing
            Dim g As Graphics = Nothing
            Dim szfText As SizeF = Nothing

            ' First trim and copy the path
            strPathTruncated = String.Copy(strPath.Trim())
            ' Create graphics to measure with
            bmp = New Bitmap(1, 1)
            g = Graphics.FromImage(bmp)
            ' Measure the text in its current form
            szfText = g.MeasureString(strPathTruncated, font)

            ' Replace the center of the path with elipses
            TextRenderer.MeasureText(strPathTruncated, font, _
                                     New Size(Math.Min(iMaxWidth, CInt(szfText.Width)), CInt(szfText.Height)), _
                                     TextFormatFlags.ModifyString Or TextFormatFlags.PathEllipsis)

            ' Clean up
            g.Dispose()
            bmp.Dispose()

            ' Return the modified path
            Return strPathTruncated

        End Function

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
        ''' Generic conversion helper, converts a string into an integer value using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToInteger(ByVal strNumber As String, _
                                                Optional ByVal iNullValue As Integer = -9999, _
                                                Optional ByVal strDecimalSepartor As String = ".", _
                                                Optional ByVal strThousandsSepartor As String = "") As Integer

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try

                    Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

                    ni.NumberDecimalSeparator = strDecimalSepartor
                    ni.NumberGroupSeparator = strThousandsSepartor

                    Return Convert.ToInt32(strNumber, ni)

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
        ''' <param name="strNumber"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToSingle(ByVal strNumber As String, _
                                               Optional ByVal sNullValue As Single = -9999.0!, _
                                               Optional ByVal strDecimalSepartor As String = ".", _
                                               Optional ByVal strThousandsSepartor As String = "") As Single

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try
                    Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

                    ni.NumberDecimalSeparator = strDecimalSepartor
                    ni.NumberGroupSeparator = strThousandsSepartor

                    Return Convert.ToSingle(strNumber, ni)
                Catch ex As Exception

                End Try

            End If

            Return sNullValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a string into a single value using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="strNumber"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConvertToDouble(ByVal strNumber As String, _
                                               Optional ByVal dNullValue As Double = -9999.0, _
                                               Optional ByVal strDecimalSepartor As String = ".", _
                                               Optional ByVal strThousandsSepartor As String = "") As Double

            Select Case strNumber.Trim
                Case "-", "_" : strNumber = ""
            End Select

            If Not String.IsNullOrEmpty(strNumber) Then

                Try

                    Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
                    Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

                    ni.NumberDecimalSeparator = strDecimalSepartor
                    ni.NumberGroupSeparator = strThousandsSepartor

                    Return Convert.ToDouble(strNumber, ni)
                Catch ex As Exception

                End Try

            End If

            Return dNullValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a single value into a string using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="iValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatInteger(ByVal iValue As Integer, _
                                             Optional ByVal strDecimalSepartor As String = ".", _
                                             Optional ByVal strThousandsSepartor As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSepartor
            ni.NumberGroupSeparator = strThousandsSepartor

            Return Convert.ToString(iValue, ni)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a single value into a string using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="sValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatSingle(ByVal sValue As Single, _
                                            Optional ByVal strDecimalSepartor As String = ".", _
                                            Optional ByVal strThousandsSepartor As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSepartor
            ni.NumberGroupSeparator = strThousandsSepartor

            Return Convert.ToString(sValue, ni)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic conversion helper, converts a double value into a string using
        ''' the fixed EwE number format of decimal points and NO thousands separator.
        ''' </summary>
        ''' <param name="dValue"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FormatDouble(ByVal dValue As Double, _
                                            Optional ByVal strDecimalSepartor As String = ".", _
                                            Optional ByVal strThousandsSepartor As String = "") As String

            Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
            Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            ni.NumberDecimalSeparator = strDecimalSepartor
            ni.NumberGroupSeparator = strThousandsSepartor

            Return Convert.ToString(dValue, ni)

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

    End Class

End Namespace ' Utilities
