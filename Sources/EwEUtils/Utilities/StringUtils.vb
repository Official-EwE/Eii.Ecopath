'==============================================================================
'
' $Log: StringUtils.vb,v $
' Revision 1.2  2008/10/20 23:35:39  jeroens
' Added shift
'
' Revision 1.1  2008/09/26 07:31:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/07/16 13:27:27  jeroens
' Added EndsWith
'
' Revision 1.8  2008/07/01 19:13:12  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.7  2008/07/01 13:55:50  jeroens
' Added BeginsWithOneOf
'
' Revision 1.6  2008/04/22 08:23:45  jeroens
' Added BeginsWith
'
' Revision 1.5  2007/12/08 18:41:16  jeroens
' * Made GetNextNumber robust to any type of wrong input
'
' Revision 1.4  2007/12/08 15:09:34  jeroens
' + Added GetNextNumber
'
' Revision 1.3  2007/09/27 18:01:25  jeroens
' Changed namespace
'
' Revision 1.2  2007/05/16 04:31:01  jeroens
' * Renamed the one method
'
' Revision 1.1  2007/05/16 04:28:08  jeroens
' Intial version
'
'==============================================================================

Option Strict On

Imports System.Text.RegularExpressions

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
    End Class

End Namespace ' Utilities
