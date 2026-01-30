' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Xml
Imports System.Text



Namespace Utilities

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' XML helper methods.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cXMLUtils

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strRootElement"></param>
        ''' <param name="xnRoot"></param>
        ''' <param name="strEncoding"></param>
        ''' <returns></returns>
        Public Shared Function NewDoc(strRootElement As String,
                                      Optional ByRef xnRoot As XmlNode = Nothing,
                                      Optional strEncoding As String = "") As XmlDocument
            Dim doc As New XmlDocument()
            Dim xnData As XmlElement = Nothing
            Dim xaData As XmlAttribute = Nothing
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", strEncoding, "yes"))
            xnRoot = doc.CreateElement(strRootElement)
            doc.AppendChild(xnRoot)
            Return doc
        End Function

        Public Shared Function XMLNodeName(name As String) As String

            Dim sb As New StringBuilder()
            For i As Integer = 0 To name.Length - 1
                Dim c As Char = name(i)
                Dim bUseChar As Boolean = If(i = 0, Char.IsLetter(c), Char.IsLetterOrDigit(c))
                If (bUseChar) Then
                    sb.Append(c)
                End If
            Next i
            name = sb.ToString()

            If (String.IsNullOrWhiteSpace(name)) Then
                Return "unnamed"
            End If
            Return name

        End Function

        Private Shared INVALD_CHARS As String = """<>" & cStringUtils.vbCr & cStringUtils.vbLf

        Public Shared Function XMLNodeValue(name As String) As String

            Dim sb As New StringBuilder()
            For i As Integer = 0 To name.Length - 1
                Dim c As Char = name(i)
                Dim bUseChar As Boolean = If(i = 0, Char.IsLetter(c), Not INVALD_CHARS.Contains(c))
                If (bUseChar) Then
                    sb.Append(c)
                End If
            Next i
            name = sb.ToString()

            If (String.IsNullOrWhiteSpace(name)) Then
                Return ""
            End If
            Return name

        End Function


    End Class

End Namespace
