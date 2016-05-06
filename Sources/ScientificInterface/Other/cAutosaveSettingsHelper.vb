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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Xml
Imports EwECore
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Forms
Imports WeifenLuo.WinFormsUI
Imports ScientificInterfaceShared
Imports EwEUtils.Core

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' <para>Helper class that loads and saves core autosave settings from a XML 
''' document for persistent storage.</para>
''' </summary>
''' ===========================================================================
Friend Class cAutosaveSettingsHelper

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load autosave settings from the XML document.
    ''' </summary>
    ''' <param name="settings">The settings to analyze.</param>
    ''' -----------------------------------------------------------------------
    Public Shared Sub LoadFromSettings(ByVal settings As XmlDocument, core As cCore)

        Dim node As XmlNode = Nothing
        Dim att As XmlAttribute = Nothing

        ' Sanity checks
        If (settings Is Nothing) Then Return
        If (settings.ChildNodes.Count = 0) Then Return

        ' For every autosave type
        For Each t As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
            Try
                ' Find node
                node = settings.SelectSingleNode("/autosavesettings/" & t.ToString)
                ' Is valid?
                If (node IsNot Nothing) Then
                    ' #Yes: plunder content
                    att = node.Attributes("Enabled")
                    core.Autosave(t) = IIF(att IsNot Nothing, Boolean.Parse(att.InnerText), False)
                    att = node.Attributes("Format")
                    core.AutosaveFormat(t) = IIF(att IsNot Nothing, att.InnerText, "")
                End If
            Catch ex As Exception
                ' Woops - ignore malformed setting
            End Try
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generate XML document from local data.
    ''' </summary>
    ''' <returns>A penguin. Really.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function SaveToSettings(core As cCore) As XmlDocument

        Dim doc As New XmlDocument()
        Dim node As XmlNode = Nothing
        Dim nodeChild As XmlNode = Nothing
        Dim att As XmlAttribute = Nothing

        node = doc.CreateXmlDeclaration("1.0", "utf-16", Nothing)
        doc.AppendChild(node)

        node = doc.CreateElement("autosavesettings")
        doc.AppendChild(node)

        ' For every autosave type
        For Each t As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
            Try
                nodeChild = doc.CreateElement(t.ToString)

                att = doc.CreateAttribute("Enabled")
                att.InnerText = core.Autosave(t).ToString
                nodeChild.Attributes.Append(att)

                att = doc.CreateAttribute("Format")
                att.InnerText = core.AutosaveFormat(t)
                nodeChild.Attributes.Append(att)

                node.AppendChild(nodeChild)

            Catch ex As Exception

            End Try
        Next
        Return doc

    End Function

End Class