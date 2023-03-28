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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Web
Imports System.Xml
Imports EwEShell
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Emulator

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Container for <see cref="cTestset"/> instances.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cTestSetData

#Region " Private vars "

        Private m_testsets As New List(Of cTestset)
        Private m_doc As XmlDocument = Nothing
        Private m_strFile As String = ""
        Private m_strModelName As String = ""

#End Region ' Private vars

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create new <see cref="cTestSetData"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            Me.m_strFile = Me.FileName()
            If (File.Exists(Me.m_strFile)) Then
                Try
                    Me.m_doc = New XmlDocument()
                    Me.m_doc.Load(Me.m_strFile)
                Catch ex As Exception
                    ' Aargh
                    Me.m_doc = Nothing
                End Try
            End If

            If (Me.m_doc Is Nothing) Then
                Me.m_doc = cXMLUtils.NewDoc("games")
            End If

        End Sub

#End Region ' Construction

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Loads all avaliable testsets from persistent storage compatible with a 
        ''' given EwE model, ecospace scenario and MSP game.
        ''' </summary>
        ''' <param name="strModelScenario">The string model scenario.</param>
        ''' <param name="g">The g.</param>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Public Function Load(strModelScenario As String, g As cGame) As Boolean

            Me.Close()
            If (g Is Nothing) Then Return False

            Me.m_strModelName = Me.ToAttributeValue(strModelScenario)
            Try

                Dim xnModel As XmlNode = Me.m_doc.SelectSingleNode("//ewe-config[@name='" & Me.m_strModelName & "']")
                If (xnModel IsNot Nothing) Then
                    For Each xnTestset As XmlNode In xnModel.ChildNodes
                        Dim strName As String = ""
                        For Each xa As XmlAttribute In xnTestset.Attributes
                            Select Case xa.Name.ToLower()
                                Case "name" : strName = xa.InnerText
                            End Select
                        Next
                        If (Not String.IsNullOrWhiteSpace(strName)) Then
                            Dim t As New cTestset(strName, g)
                            For Each xnData As XmlNode In xnTestset.ChildNodes
                                Dim strPressure As String = ""
                                Dim strData As String = ""

                                For Each xa As XmlAttribute In xnData.Attributes
                                    Select Case xa.Name.ToLower()
                                        Case "pressure" : strPressure = xa.InnerText
                                        Case "value" : strData = xa.InnerText
                                    End Select
                                Next

                                Dim p As cPressure = g.Pressure(strPressure)
                                If (p IsNot Nothing) Then t.Testdata(p) = strData
                            Next
                            Me.m_testsets.Add(t)
                        End If
                    Next xnTestset
                End If
                Return True
            Catch ex As Exception

            End Try
            Return False

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Saves testsets back to persistent storage.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Public Function Save() As Boolean

            If (String.IsNullOrWhiteSpace(Me.m_strModelName)) Then Return False

            Dim xa As XmlAttribute = Nothing

            Try

                Dim xnModel As XmlNode = Me.m_doc.SelectSingleNode("//ewe-config[@name='" & Me.m_strModelName & "']")
                If (xnModel Is Nothing) Then
                    For Each xnRoot As XmlNode In Me.m_doc.ChildNodes
                        If (xnRoot.Name = "games") Then
                            xnModel = Me.m_doc.CreateElement("ewe-config")
                            xnRoot.AppendChild(xnModel)

                            xa = Me.m_doc.CreateAttribute("name")
                            xa.InnerText = Me.m_strModelName
                            xnModel.Attributes.Append(xa)
                        End If
                    Next
                End If

                While (xnModel.FirstChild IsNot Nothing)
                    xnModel.RemoveChild(xnModel.FirstChild)
                End While

                For Each t As cTestset In Me.m_testsets

                    Dim xnTestset As XmlNode = Me.m_doc.CreateElement("testset")

                    xa = Me.m_doc.CreateAttribute("name")
                    xa.InnerText = t.Name
                    xnTestset.Attributes.Append(xa)

                    For Each p As cPressure In t.Pressures

                        Dim xnInput As XmlNode = Me.m_doc.CreateElement("input")

                        xa = Me.m_doc.CreateAttribute("pressure")
                        xa.InnerText = p.Name
                        xnInput.Attributes.Append(xa)

                        xa = Me.m_doc.CreateAttribute("value")
                        xa.InnerText = t.Testdata(p)
                        xnInput.Attributes.Append(xa)

                        xnTestset.AppendChild(xnInput)
                    Next

                    xnModel.AppendChild(xnTestset)
                Next

                Me.m_doc.Save(m_strFile)
                Return True

            Catch ex As Exception
            End Try
            Return False

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Closes this instance and discard all testset information.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Public Function Close() As Boolean
            Me.m_testsets.Clear()
            Me.m_strModelName = ""
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the list of testsets that are currently available.
        ''' </summary>
        ''' <returns>The list of testsets that are currently available.</returns>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Testsets() As List(Of cTestset)
            Get
                Return Me.m_testsets
            End Get
        End Property

#End Region ' Public access

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Obtain the filename of the persistent testsets storage.
        ''' </summary>
        ''' <returns>The filename of the persistent testsets storage.</returns>
        ''' -----------------------------------------------------------------------
        Private Function FileName() As String
            Return Path.Combine(cSystemUtils.ApplicationSettingsPath(), "MSP_testsets.xml")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert a EwE model name fit for storage in a XML attribute using
        ''' HTTP encoding.
        ''' </summary>
        ''' <param name="strModelName">Name of the string model.</param>
        ''' <returns>The updated name</returns>
        ''' -----------------------------------------------------------------------
        Private Function ToAttributeValue(strModelName As String) As String
            Return HttpUtility.HtmlEncode(strModelName)
        End Function

#End Region ' Internals

    End Class

End Namespace
