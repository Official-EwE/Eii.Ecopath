' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports EwEUtils.NetUtilities
Imports Microsoft.Extensions.Logging


Namespace WebServices.Ecobase

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for a single model
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEcobaseSubmission

#Region " Variables "

        <XmlElement("result")>
        Public Property Result As Integer
        <XmlElement("md5_key")>
        Public Property Hash As String
        ''' <summary>Ecobase ID.</summary>
        <XmlElement("model_number")>
        Public Property ModelNumber As String
        Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcobaseSubmission)()

        Public Enum eSubmisssionResultTypes As Integer
            Pending = 0
            Accepted = 1
            NotInEcobase = 2
        End Enum

        <XmlIgnore()>
        Public Property ResultType As eSubmisssionResultTypes
            Get
                Return DirectCast(Me.Result, eSubmisssionResultTypes)
            End Get
            Set(value As eSubmisssionResultTypes)
                Me.Result = value
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
        ''' Factory method, create a cEcobaseData instance from WSDL output.
        ''' </summary>
        ''' <param name="strXML"></param>
        ''' <returns>A cEcobaseData instance, or nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromXML(strXML As String) As cEcobaseSubmission

            ' Clean up
            If (String.IsNullOrWhiteSpace(strXML)) Then Return Nothing

            strXML = strXML.Replace(""" & vbLF && """, "")
            strXML = strXML.Replace("submission", "cEcobaseSubmission")

            Dim reader As New StringReader(strXML)
            Dim serializer As New XmlSerializer(GetType(cEcobaseSubmission))
            Dim selfie As cEcobaseSubmission = Nothing

            Try
                selfie = CType(serializer.Deserialize(reader), cEcobaseSubmission)
            Catch ex As Exception
                ' Hmm
                m_logger.LogError(ex, "cEcobaseSubmission.FromXML")
            End Try

            Return selfie

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a cEcobaseData instance to a chunk of XML for submission to EcoBase
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToXML(data As cEcobaseModelParameters) As String

            Dim writerText As New cFlexibleEncodingStringWriter()
            Dim writerXML As XmlWriter = XmlWriter.Create(writerText)
            Dim serializer As New XmlSerializer(GetType(cEcobaseModelParameters))
            serializer.Serialize(writerXML, data)
            Return writerText.ToString()

        End Function

#End Region ' Shared access

    End Class

End Namespace
