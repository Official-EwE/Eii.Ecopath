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
Imports System.IO
Imports System.Xml.Serialization
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Text
Imports System.Xml
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports EwEUtils.SystemUtilities
Imports EwEUtils.NetUtilities

#End Region ' Imports

Namespace WebServices.Ecobase

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class for receiving the EcoBase data access agreement.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEcobaseDataAccessAgreement

        Private m_strAgreement As String = ""

#Region " Variables "

        Public Property Agreement As String
            Get
                Return Me.m_strAgreement
            End Get
            Set(value As String)
                Me.m_strAgreement = cStringUtils.Unwrap(value)
            End Set
        End Property

#End Region ' Variables

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Default contructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

#End Region ' Construction

#Region " Shared access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Factory method, create a cEcobaseDataAccessAgreement instance from WSDL output.
        ''' </summary>
        ''' <param name="strXML"></param>
        ''' <returns>A cEcobaseDataAccessAgreement instance, or nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromXML(strXML As String) As cEcobaseDataAccessAgreement

            ' Clean up
            If (String.IsNullOrWhiteSpace(strXML)) Then Return Nothing

            ' Parsing CData is no fun through XML serializers

            Try
                Dim xnData As XmlNode = Nothing

                ' Patch up XML
                If Not strXML.StartsWith("<?") Then
                    Dim xnRoot As XmlNode = Nothing
                    Dim doc As XmlDocument = cXMLUtils.NewDoc("Ecobase", xnRoot)
                    xnRoot.InnerXml = strXML
                    xnData = xnRoot.ChildNodes(0)
                Else
                    Dim doc As New XmlDocument()
                    doc.LoadXml(strXML)
                    xnData = doc.ChildNodes(0).ChildNodes(0)
                End If

                Dim selfie As New cEcobaseDataAccessAgreement()
                selfie.Agreement = xnData.InnerText.Trim()
                Return selfie
            Catch ex As Exception
                ' Hmm
                cLog.Write(ex, "cEcobaseDataAccessAgreement.FromXML")
            End Try

            Return Nothing

        End Function

#End Region ' Shared access

    End Class

End Namespace
