' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Xml
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging


Namespace WebServices.Ecobase

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class for receiving the EcoBase data access agreement.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEcobaseDataAccessAgreement

        Private m_strAuthorAgreement As String = ""
        Private m_strUserAgreement As String = ""
        Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcobaseDataAccessAgreement)()

#Region " Variables "

        Public Property AuthorAgreement As String
            Get
                Return Me.m_strAuthorAgreement
            End Get
            Set(value As String)
                Me.m_strAuthorAgreement = cStringUtils.Unwrap(value)
            End Set
        End Property

        Public Property UserAgreement As String
            Get
                Return Me.m_strUserAgreement
            End Get
            Set(value As String)
                Me.m_strUserAgreement = cStringUtils.Unwrap(value)
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
                ' Patch up XML
                If Not strXML.StartsWith("<?") Then
                    strXML = "<?xml version=""1.0"" encoding=""utf-8""?><Agreements>" & strXML & "</Agreements>"
                End If

                Dim doc As New XmlDocument()
                doc.LoadXml(strXML)
                Dim selfie As New cEcobaseDataAccessAgreement()

                For Each node As XmlNode In doc.GetElementsByTagName("dissemination_agreement")
                    selfie.AuthorAgreement = node.InnerText
                Next

                For Each node As XmlNode In doc.GetElementsByTagName("agreement")
                    selfie.UserAgreement = node.InnerText
                Next

                Return selfie
            Catch ex As Exception
                ' Hmm
                m_logger.LogError(ex, "cEcobaseDataAccessAgreement.FromXML")
            End Try

            Return Nothing

        End Function

#End Region ' Shared access

    End Class

End Namespace
