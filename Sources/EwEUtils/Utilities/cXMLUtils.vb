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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'





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
        Public Shared Function NewDoc(strRootElement As String, _
                                      Optional ByRef xnRoot As XmlNode = Nothing, _
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
