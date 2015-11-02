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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.SpatialData
Imports System.Collections.Generic
Imports System.Xml
Imports System.Text
Imports EwEUtils.Utilities
Imports EwECore.SpatialData

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="ISpatialDataConverter"/> for converting DotSpatial data to Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cSpatialDataConverter
        Implements ISpatialDataConverterPlugin

        Protected m_core As cCore = Nothing
        Protected m_mappings As New Dictionary(Of Object, Object)

        Public Sub New()
            Me.AttributeName = ""
            Me.AttributeFilter = ""
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Author"/>
        ''' -----------------------------------------------------------------------
        Public Overridable ReadOnly Property Author As String _
            Implements EwEPlugin.IPlugin.Author
            Get
                Return "EwE development team"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Contact"/>
        ''' -----------------------------------------------------------------------
        Public Overridable ReadOnly Property Contact As String _
            Implements EwEPlugin.IPlugin.Contact
            Get
                Return "mailto:ewedevteam@gmail.com"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property PluginName As String _
            Implements EwEPlugin.IPlugin.Name

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property DisplayName As String _
            Implements ISpatialDataConverter.DisplayName

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property Description As String _
            Implements ISpatialDataConverter.Description, EwEPlugin.IPlugin.Description

        Public Overrides Function ToString() As String
            Return Me.DisplayName()
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Initialize"/>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub Initialize(ByVal core As Object) _
            Implements ISpatialDataConverterPlugin.Initialize
            Me.m_core = DirectCast(core, cCore)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEUtils.SpatialData.ISpatialDataConverter.Dataset"/>
        ''' -----------------------------------------------------------------------
        Public Overridable Property Dataset As ISpatialDataSet _
            Implements ISpatialDataConverter.Dataset

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride Function IsCompatible(ds As ISpatialDataSet) As Boolean _
            Implements ISpatialDataConverter.IsCompatible

#Region " Configuration "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeFilter"/>
        ''' -----------------------------------------------------------------------
        Public Overridable Property AttributeFilter As String _
            Implements ISpatialDataConverter.AttributeFilter

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeName"/>
        ''' -----------------------------------------------------------------------
        Public Overridable Property AttributeName As String _
            Implements ISpatialDataConverter.AttributeName

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeValueMappings"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property AttributeValueMappings As Dictionary(Of Object, Object) _
            Implements ISpatialDataConverter.AttributeValueMappings
            Get
                Return Me.m_mappings
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride Function IsConfigured() As Boolean _
            Implements ISpatialDataConverter.IsConfigured

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Configuration"/>
        ''' -------------------------------------------------------------------
        Public Property Configuration(doc As XmlDocument) As XmlNode _
            Implements ISpatialDataConverter.Configuration
            Get
                Return Me.ToXML(doc)
            End Get
            Set(ByVal value As XmlNode)
                Me.FromXML(doc, value)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write configuration to XML.
        ''' </summary>
        ''' <param name="doc">The doc to generate nodes for.</param>
        ''' <returns>
        ''' An XML node that contains the configuration of the converter.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function ToXML(doc As XmlDocument) As XmlNode

            Dim xnMaster As XmlNode = doc.CreateElement("Configuration")
            Dim xn As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim obj As Object = Nothing

            If (Not String.IsNullOrWhiteSpace(Me.AttributeFilter)) Then
                xn = doc.CreateElement("AttributeFilter")
                xn.InnerText = Me.AttributeFilter
                xnMaster.AppendChild(xn)
            End If

            If (Not String.IsNullOrWhiteSpace(Me.AttributeName)) Then
                xn = doc.CreateElement("Attribute")
                xn.InnerText = Me.AttributeName
                xnMaster.AppendChild(xn)
            End If

            For Each key As Object In Me.m_mappings
                xn = doc.CreateElement("ValueMapping")
                '
                xa = doc.CreateAttribute("From")
                xa.InnerText = key.ToString
                xn.Attributes.Append(xa)
                '
                xa = doc.CreateAttribute("to")
                obj = Me.m_mappings(key)
                If (obj Is Nothing) Then
                    xa.InnerText = ""
                Else
                    xa.InnerText = obj.ToString
                End If
                xn.Attributes.Append(xa)
                '
                xnMaster.AppendChild(xn)
            Next
            Return xnMaster

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read configuration from XML.
        ''' </summary>
        ''' <param name="doc">The doc to read the configuration from.</param>
        ''' <param name="node">The node that contains the configuration of the converter.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function FromXML(doc As XmlDocument, ByVal node As XmlNode) As Boolean

            Dim xn As XmlNode = Nothing
            Dim xnFile As XmlNode = Nothing
            Dim xaFile As XmlAttribute = Nothing

            Me.m_mappings.Clear()

            If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

            Try
                For Each xn In node.ChildNodes
                    Select Case xn.Name.ToLower
                        Case "attribute" : Me.AttributeName = xn.InnerText
                        Case "attributefilter" : Me.AttributeFilter = xn.InnerText
                        Case "valuemapping"
                            Try
                                Dim xaFrom As XmlAttribute = xn.Attributes("From")
                                Dim xaTo As XmlAttribute = xn.Attributes("To")
                                ' For now only support strings. This will have to become dynamic
                                'Dim xaType As XmlAttribute = xn.Attributes("Type")
                                Me.m_mappings(xaFrom.InnerText) = xaTo.InnerText
                            Catch ex As Exception
                                Debug.Assert(False)
                            End Try
                    End Select
                Next

            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

#End Region ' Configuration

#Region " Conversion "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Convert"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride Function Convert(data As Object, _
                                             ptfNE As System.Drawing.PointF, ptfSW As System.Drawing.PointF, _
                                             dCellSize As Double, strProjToWkt As String, strFile As String) As ISpatialRaster _
            Implements ISpatialDataConverter.Convert

#End Region ' Conversion

#Region " Utilities "

        Protected Function Log() As cSpatialOperationLog
            If (Me.m_core IsNot Nothing) Then
                Return Me.m_core.SpatialOperationLog
            End If
            Return Nothing
        End Function

        Protected Sub LogMessage(strMessage As String, status As eStatusFlags)
            If (Me.m_core IsNot Nothing) Then
                Me.m_core.SpatialOperationLog.LogOperation(strMessage, status)
            End If
        End Sub

        Protected Overridable Function ToValue(drow As DataRow, dValueNone As Double) As Double

            Dim objVal As Object = Nothing

            ' No overlap?
            If (drow Is Nothing) Then
                ' #Yes: assume 'none"
                objVal = dValueNone
            Else
                ' #No: get value
                ' Has attribute provided?
                If (Not String.IsNullOrWhiteSpace(Me.AttributeName)) Then
                    ' #Yes: get attribute value
                    objVal = drow(Me.AttributeName)
                Else
                    ' #No: use simple absent / presence
                    objVal = 1
                End If
            End If

            ' Perform any mapping
            If (Me.m_mappings.Count > 0) Then

                If Me.m_mappings.ContainsKey("") Then
                    dValueNone = System.Convert.ToDouble(Me.m_mappings(""))
                End If

                If Me.m_mappings.ContainsKey(objVal) Then
                    objVal = Me.m_mappings(objVal)
                Else
                    objVal = dValueNone
                End If
            End If

            Try
                ' Convert
                Return System.Convert.ToDouble(objVal)
            Catch ex As Exception
                ' Whoah!
                Return dValueNone
            End Try

        End Function


#End Region ' Utilities

        Public ReadOnly Property Summary As String _
            Implements EwEUtils.Core.ISummarizable.Summary
            Get

                Dim sb As New StringBuilder()
                Dim strKey As String = ""
                Dim strVal As String = ""

                sb.Append("id:" & Me.GetType().ToString())
                sb.Append(",")
                sb.Append("n:" & Me.AttributeName)
                sb.Append(",")
                sb.Append("f:" & Me.AttributeFilter)
                sb.Append(",")
                sb.Append("m:")

                Dim lKeys As New List(Of String)
                For Each strKey In Me.AttributeValueMappings.Keys
                    lKeys.Add(strKey)
                Next
                lKeys.Sort()

                For i As Integer = 0 To lKeys.Count - 1
                    If (i > 0) Then sb.Append("&")
                    strKey = lKeys(i)
                    strVal = cStringUtils.FormatNumber(Me.AttributeValueMappings(strKey))
                    sb.Append(strKey & "=" & strVal)
                Next
                Return sb.ToString()

            End Get
        End Property
    End Class

End Namespace
