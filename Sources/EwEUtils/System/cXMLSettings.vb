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




Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Xml



Namespace SystemUtilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' INI file reader alternative using XML formats.
    ''' </summary>
    ''' <remarks>
    ''' Foundation obtained 15 April 2012 from http://content.gpwiki.org/index.php/VBNET:Class_XMLINI.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cXMLSettings

#Region " Private vars "

        Private m_strFileName As String = ""
        Private m_doc As XmlDocument = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            ' NOP
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
        ''' Load settings from file.
        ''' </summary>
        ''' <param name="strFileName">Name of the file to open. A ".xml" extension
        ''' is assumed if none is provide.</param>
        ''' <param name="bOverwrite">If true, this will not load the target file, 
        ''' but will overwrite it instead 
        ''' if it exists. If set to false, any existing file will be loaded.</param>
        ''' -------------------------------------------------------------------
        Public Sub Create(strFileName As String, Optional bOverwrite As Boolean = False)

            ' Add extension to file name if missing
            If String.IsNullOrWhiteSpace(Path.GetExtension(strFileName)) Then
                strFileName = Path.ChangeExtension(strFileName, ".xml")
            End If

            If (File.Exists(strFileName)) Then
                Try
                    Me.EnsureHasDoc()
                    Me.m_doc.Load(strFileName)
                Catch ex As Exception
                End Try
            End If
            Me.m_strFileName = strFileName

        End Sub

        Public Sub Load(strXML As String)

            If (String.IsNullOrWhiteSpace(strXML)) Then Return

            Try
                Dim xn As XmlNode = Nothing
                Me.EnsureHasDoc()
                Me.m_doc = EwEUtils.Utilities.cXMLUtils.NewDoc("sections", xn)
                xn.InnerXml = strXML
            Catch ex As Exception

            End Try
        End Sub

        Public Event OnSettingsChanged(sender As Object, args As EventArgs)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a setting to the configuration file.
        ''' </summary>
        ''' <param name="strSection">Name of the section to write to. Cannot be
        ''' left empty.</param>
        ''' <param name="strKey">Name of the key to write to. Cannot be left
        ''' empty.</param>
        ''' <param name="value">The  value to write.</param>
        ''' -------------------------------------------------------------------
        Public Sub WriteSetting(Of T)(strSection As String,
                               strKey As String,
                               value As T)

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrWhiteSpace(strSection))
            Debug.Assert(Not String.IsNullOrWhiteSpace(strKey))

            Dim node As XmlNode = Nothing
            Dim keynode As XmlNode = Nothing
            Dim val As String = Convert.ToString(value)

            Me.EnsureHasDoc()

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
                    att.Value = val
                    newnode.Attributes.Append(att)
                    node.AppendChild(newnode)

                    RaiseEvent OnSettingsChanged(Me, New EventArgs())
                Else
                    ' Just update key value
                    If (String.Compare(keynode.Attributes("value").Value, val, False) <> 0) Then
                        keynode.Attributes("value").Value = val
                        RaiseEvent OnSettingsChanged(Me, New EventArgs())
                    End If
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
        ''' <param name="defaultValue">Default value to return if the indicated
        ''' key could not be found in the indicated section.</param>
        ''' -------------------------------------------------------------------
        Public Function ReadSetting(Of T)(strSection As String,
                                    strKey As String,
                                    defaultValue As T) As T

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrWhiteSpace(strSection))
            Debug.Assert(Not String.IsNullOrWhiteSpace(strKey))

            If (Me.m_doc Is Nothing) Then Return defaultValue

            Dim node As XmlNode = Nothing

            Try
                node = m_doc.SelectSingleNode("/sections/section[@name='" & strSection & "']/item[@key='" & strKey & "']")
                If (node IsNot Nothing) Then
                    If (GetType(T).IsEnum) Then
                        Return cEnumUtils.EnumParse(node.Attributes("value").Value, defaultValue)
                    End If
                    Return CType(Convert.ChangeType(node.Attributes("value").Value, GetType(T)), T)
                End If
            Catch ex As Exception
            End Try
            Return defaultValue

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save settings to disk.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Flush()
            Try
                If Not String.IsNullOrWhiteSpace(Me.m_strFileName) Then
                    Me.m_doc.Save(Me.m_strFileName)
                End If
            Catch ex As Exception
                ' Whoah
            End Try
        End Sub

        Public Overrides Function ToString() As String
            If (Me.m_doc Is Nothing) Then Return ""
            Return m_doc.SelectSingleNode("/sections").InnerXml
        End Function

#End Region ' Public access

#Region " Internals "

        Private Sub EnsureHasDoc()
            If (Me.m_doc Is Nothing) Then
                Me.m_doc = EwEUtils.Utilities.cXMLUtils.NewDoc("sections")
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace
