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
Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Xml

#End Region ' Imports

Namespace SystemUtilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' INI file reader alternative using XML formats.
    ''' </summary>
    ''' <remarks>
    ''' Foundation obtained 15 April 2012 from http://content.gpwiki.org/index.php/VBNET:Class_XMLINI.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cXMLINIfile

#Region " Private vars "

        Private m_strFileName As String = ""
        Private m_doc As New XmlDocument()

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an instance of this class. If the indicated XML file exists
        ''' it is opened and loaded in the instance.
        ''' </summary>
        ''' <param name="strFileName">Name of the file to open. A ".xml" extension
        ''' is assumed if none is provide.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strFileName As String)

            ' Add extension to file name if missing
            If String.IsNullOrWhiteSpace(Path.GetExtension(strFileName)) Then
                strFileName = Path.ChangeExtension(strFileName, ".xml")
            End If

            ' Store file name prior to opening
            Me.m_strFileName = strFileName

            If (File.Exists(Me.m_strFileName)) Then
                Try
                    ' Attempt to load doc
                    Me.m_doc.Load(strFileName)
                Catch ex As Exception
                    ' Create declaration if load failed
                    Dim dec As XmlDeclaration = m_doc.CreateXmlDeclaration("1.0", Nothing, Nothing)
                    Me.m_doc.AppendChild(dec)
                End Try
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Die! Die!
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub Finalize()
            Me.Flush()
            MyBase.Finalize()
        End Sub

#End Region ' 

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a setting to the configuration file.
        ''' </summary>
        ''' <param name="strSection">Name of the section to write to. Cannot be
        ''' left empty.</param>
        ''' <param name="strKey">Name of the key to write to. Cannot be left
        ''' empty.</param>
        ''' <param name="objNewValue">The value to write.</param>
        ''' -------------------------------------------------------------------
        Public Sub SaveSetting(ByVal strSection As String, _
                               ByVal strKey As String, _
                               ByVal objNewValue As Object)
            Dim strValue As String = ""
            If (objNewValue IsNot Nothing) Then strValue = Convert.ToString(objNewValue)
            Me.SaveSetting(strSection, strKey, strValue)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a setting to the configuration file.
        ''' </summary>
        ''' <param name="strSection">Name of the section to write to. Cannot be
        ''' left empty.</param>
        ''' <param name="strKey">Name of the key to write to. Cannot be left
        ''' empty.</param>
        ''' <param name="strNewValue">The string value to write.</param>
        ''' -------------------------------------------------------------------
        Public Sub SaveSetting(ByVal strSection As String, _
                               ByVal strKey As String, _
                               ByVal strNewValue As String)

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrWhiteSpace(strSection))
            Debug.Assert(Not String.IsNullOrWhiteSpace(strKey))

            Dim node As XmlNode = Nothing
            Dim keynode As XmlNode = Nothing

            ' check for/create section
            node = m_doc.SelectSingleNode("/sections")
            If (node Is Nothing) Then
                Dim docroot As XmlElement = m_doc.CreateElement("sections")
                Me.m_doc.AppendChild(docroot)
            End If

            Try
                ' Check for/create section
                node = m_doc.SelectSingleNode("/sections/section[@name='" & strSection & "']")
                If (node Is Nothing) Then
                    Dim newnode As XmlNode = m_doc.CreateElement("section")
                    Dim att As XmlAttribute = m_doc.CreateAttribute("name")
                    att.Value = strSection
                    newnode.Attributes.Append(att)
                    node = Me.m_doc.SelectSingleNode("/sections")
                    node.AppendChild(newnode)
                    node = Me.m_doc.SelectSingleNode("/sections/section[@name='" & strSection & "']")
                End If

                Debug.Assert(node IsNot Nothing)

                ' get key
                keynode = m_doc.SelectSingleNode("/sections/section[@name='" & strSection & "']/item[@key='" & strKey & "']")
                If (keynode Is Nothing) Then
                    ' create key
                    Dim newnode As XmlNode = m_doc.CreateElement("item")
                    Dim att As XmlAttribute = m_doc.CreateAttribute("key")
                    att.Value = strKey
                    newnode.Attributes.Append(att)
                    att = m_doc.CreateAttribute("value")
                    att.Value = strNewValue
                    newnode.Attributes.Append(att)
                    node.AppendChild(newnode)
                Else
                    ' just update key value
                    keynode.Attributes("value").Value = strNewValue
                End If
            Catch ex As Exception
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Retrieve a setting from the configuration file.
        ''' </summary>
        ''' <param name="strSection">Name of the section to read from. Cannot be
        ''' left empty.</param>
        ''' <param name="strKey">Name of the key to read from. Cannot be left
        ''' empty.</param>
        ''' <param name="strDefaultValue">Default value to return if the indicated
        ''' key could not be found in the indicated section.</param>
        ''' -------------------------------------------------------------------
        Public Function GetSetting(ByVal strSection As String, _
                                   ByVal strKey As String, _
                                   ByVal strDefaultValue As String) As String

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrWhiteSpace(strSection))
            Debug.Assert(Not String.IsNullOrWhiteSpace(strKey))

            Dim node As XmlNode = Nothing

            Try
                node = m_doc.SelectSingleNode("/sections/section[@name='" & strSection & "']/item[@key='" & strKey & "']")
                If (node IsNot Nothing) Then
                    Return node.Attributes("value").Value
                End If
            Catch ex As Exception
            End Try
            Return strDefaultValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save settings to disk.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Flush()
            Try
                Me.m_doc.Save(Me.m_strFileName)
            Catch ex As Exception
                ' Whoah
            End Try
        End Sub

#End Region ' Public access

    End Class

End Namespace
