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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Imports System
Imports System.IO
Imports System.Threading
Imports System.Data
Imports System.Diagnostics
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Core

    ''' <summary>
    ''' Class encapsulating writing of messages to a log file.
    ''' </summary>
    Public Class cLog

#Region " Private Data "

        Private Shared m_xmlWriter As cXMLLogWriter = Nothing
        Private Shared m_logFilename As String = ""
        Private Shared m_modelname As String = "No Model Loaded"
        Private Shared m_lock As New ReaderWriterLock()
        Private Shared m_verboselevel As eVerboseLevel = eVerboseLevel.Standard

        ''' <summary>
        ''' Max size of the log file in bytes. One megabyte
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared MAX_LOG_SIZE As Integer = 1048576

#End Region

#Region " Log methods "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the level of message detail that the log should register.
        ''' </summary>
        ''' <remarks>Several log events can be tagged with a <see cref="eVerboseLevel">verbose level</see>,
        ''' which is measured against the level of detail that a user wants to
        ''' include in the log to determine whether event will be written to the log.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Property VerboseLevel As eVerboseLevel
            Get
                Return cLog.m_verboselevel
            End Get
            Set(value As eVerboseLevel)
                cLog.m_verboselevel = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the current log file.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Shared ReadOnly Property LogFile As String
            Get
                Return cLog.m_logFilename
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Start a new log file with the model name as part of the log file name
        ''' </summary>
        ''' <param name="strModelPath">File path to the model to create a log file for.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub InitLog(ByVal strModelPath As String)

            If Not String.IsNullOrWhiteSpace(strModelPath) Then
                cLog.m_modelname = cFileUtils.ToValidFileName(Path.GetFileNameWithoutExtension(strModelPath), False)
                cLog.m_logFilename = Path.Combine(Path.GetDirectoryName(strModelPath), m_modelname & "_log.xml")
            Else
                cLog.m_modelname = ""
                cLog.m_logFilename = ""
            End If

            WriteSessionStarted()
            cLog.m_xmlWriter = Nothing
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write an <see cref="Exception"/> to the log
        ''' </summary>
        ''' <param name="theException">Exception to write to the log.</param>
        ''' <param name="level"><see cref="eVerboseLevel">Verbose level</see>.</param>
        ''' <param name="strMsg">Optional text to add.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal theException As Exception, _
                                ByVal level As eVerboseLevel, _
                                Optional ByVal strMsg As String = "")
            If (level > cLog.m_verboselevel) Then Return
            cLog.Write(theException, strMsg)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write an <see cref="Exception"/> to the log at 
        ''' <see cref="eVerboseLevel.Standard">standard verbose level</see>.
        ''' </summary>
        ''' <param name="theException">Exception to write to the log.</param>
        ''' <param name="strMsg">Optional text to add.</param>
        ''' <remarks>
        ''' This will log the exception text and all nested exceptions.
        '''</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal theException As Exception, _
                                Optional ByVal strMsg As String = "")

            If Not AcquireWriterLock() Then Return

            Dim xmlStrm As cXMLLogWriter = Nothing
            Try
                xmlStrm = getWriter()

                'append to the end of the stream
                If xmlStrm.Open() Then

                    'now the message
                    xmlStrm.WriteStartElement("Exception_Messages")
                    xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                    If Not String.IsNullOrEmpty(strMsg) Then
                        xmlStrm.WriteElementString("Detail", strMsg)
                    End If
                    Dim thisEx As Exception = theException
                    Do While thisEx IsNot Nothing
                        xmlStrm.WriteStartElement("Exception")
                        xmlStrm.WriteElementString("Type", thisEx.GetType().ToString)
                        xmlStrm.WriteElementString("Source", thisEx.Source)
                        xmlStrm.WriteElementString("Message", thisEx.Message)
                        xmlStrm.WriteElementString("StackTrace", thisEx.StackTrace)
                        xmlStrm.WriteEndElement() 'Exception

                        thisEx = thisEx.InnerException
                    Loop

                    xmlStrm.WriteEndElement() 'Msg
                    xmlStrm.WriteEndDocument()

                    xmlStrm.Close()

                End If

            Catch ex As Exception

                System.Console.WriteLine("CLog.Write() Exception: " + ex.Message)
                If Not xmlStrm Is Nothing Then
                    xmlStrm.Close()
                End If
            End Try

            ReleaseWriterLock()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write a <see cref="IMessage"/> to the log.
        ''' </summary>
        ''' <param name="message">The <see cref="IMessage"/> to write.</param>
        ''' <param name="level"><see cref="eVerboseLevel">Verbose level</see>.</param>
        ''' <param name="strMsg">Optional text to add.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal message As IMessage, _
                                ByVal level As eVerboseLevel, _
                                Optional ByVal strMsg As String = "")
            If (level > cLog.m_verboselevel) Then Return
            cLog.Write(message, strMsg)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write a <see cref="IMessage"/> to the log at <see cref="eVerboseLevel.Standard"/> level.
        ''' </summary>
        ''' <param name="message">The <see cref="IMessage"/> to write.</param>
        ''' <param name="strMsg">Optional text to add.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal message As IMessage, Optional ByVal strMsg As String = "")

            If Not AcquireWriterLock() Then Return

            Dim xmlStrm As cXMLLogWriter = Nothing
            Try
                xmlStrm = getWriter()

                'append to the end of the stream
                If xmlStrm.Open() Then

                    xmlStrm.WriteStartElement(message.Importance.ToString & "_Message") '????
                    xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                    If Not String.IsNullOrEmpty(strMsg) Then
                        xmlStrm.WriteElementString("Detail", strMsg)
                    End If
                    xmlStrm.WriteElementString("Message", message.Message)
                    xmlStrm.WriteElementString("Message_Type", message.Type.ToString)
                    xmlStrm.WriteElementString("Message_Source", message.Source.ToString)
                    xmlStrm.WriteElementString("Message_DataType", message.DataType.ToString)
                    xmlStrm.WriteEndElement() 'Msg
                    xmlStrm.WriteEndDocument()

                    xmlStrm.Close()
                End If

            Catch ex As Exception

                System.Console.WriteLine("CLog.Write() Exception: " + ex.Message)
                If Not xmlStrm Is Nothing Then
                    xmlStrm.Close()
                End If

            End Try

            ReleaseWriterLock()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write a string to the application log.
        ''' </summary>
        ''' <param name="msg">Message string to write.</param>
        ''' <param name="level"><see cref="eVerboseLevel">Verbose level</see>.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal msg As String, _
                                ByVal level As eVerboseLevel)
            If (level > cLog.m_verboselevel) Then Return
            cLog.Write(msg)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write a string to the application log.
        ''' </summary>
        ''' <param name="msg">Message string to write.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Write(ByVal msg As String)

            If Not AcquireWriterLock() Then Return

            Dim xmlStrm As cXMLLogWriter = Nothing
            Try
                xmlStrm = getWriter()

                'append to the end of the stream
                If xmlStrm.Open() Then

                    xmlStrm.WriteStartElement("Log_Message") '????
                    xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                    xmlStrm.WriteElementString("Message", msg)
                    xmlStrm.WriteEndElement() 'Log_Message
                    xmlStrm.WriteEndDocument()

                    xmlStrm.Close()
                End If

            Catch ex As Exception

                System.Console.WriteLine("CLog.Write() Exception: " + ex.Message)
                If Not xmlStrm Is Nothing Then
                    xmlStrm.Close()
                End If
            End Try

            ReleaseWriterLock()

        End Sub

#End Region ' Log methods

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Singleton interface for creating a cXMLLogWriter object
        ''' </summary>
        ''' <returns>A cXMLLogWriter object that can be opened and written to.</returns>
        ''' <remarks>If a log file has been specified via <see cref="InitLog"/> the
        ''' cXMLLogWriter will connect to this file. If not the default file 
        ''' "EwELog.xml" will be used.</remarks>
        ''' -----------------------------------------------------------------------
        Private Shared Function getWriter() As cXMLLogWriter

            If cLog.m_xmlWriter Is Nothing Then
                If String.IsNullOrWhiteSpace(cLog.m_logFilename) Then
                    cLog.m_logFilename = Path.Combine(cSystemUtils.ApplicationSettingsPath(), "EwELog.xml")
                End If

                'Before we create the new XMLLogWriter for this file
                'Check the size of the file and delete if it's to big > MAX_LOG_SIZE
                cLog.DeleteLargeLogFiles()

                cLog.m_xmlWriter = New cXMLLogWriter(cLog.m_logFilename, cLog.m_modelname)

            End If
            Return cLog.m_xmlWriter

        End Function

        Private Shared Sub WriteSessionStarted()

            If Not AcquireWriterLock() Then Return

            Dim xmlStrm As cXMLLogWriter = Nothing
            Try
                xmlStrm = cLog.getWriter()
                If xmlStrm.Open() Then
                    xmlStrm.WriteStartElement("Session_Started")
                    xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                    xmlStrm.WriteElementString("Model", cLog.m_modelname)
                    xmlStrm.WriteElementString("LogFile", cLog.m_logFilename)
                    xmlStrm.WriteEndElement() 'Session_Started
                    xmlStrm.WriteEndDocument()

                    xmlStrm.Close()
                End If

            Catch ex As Exception
                System.Console.WriteLine("CLog.Write() Exception: " + ex.Message)
                If Not xmlStrm Is Nothing Then
                    xmlStrm.Close()
                End If
            End Try

            ReleaseWriterLock()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Aquire a critical section writer lock to the log file to initiate a
        ''' write operation.
        ''' </summary>
        ''' <returns>True if a lock was acquired.</returns>
        ''' <remarks>
        ''' <para>ReaderWriterLock.AcquireWriterLock() will throw an exception if it 
        ''' times out! This keeps the exception handling out of the main code.</para>
        ''' <para>The writer lock must be released using <see cref="ReleaseWriterLock"/>.</para>
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Shared Function AcquireWriterLock() As Boolean
            Try
                'Wait 10 seconds for a lock
                cLog.m_lock.AcquireWriterLock(10000)
                Return True
            Catch ex As Exception
                System.Console.WriteLine("Error trying to lock the Log file for writting! " & ex.Message)
                Return False
            End Try
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Release a critical section writer lock to the log file previously 
        ''' obtained via <see cref="AcquireWriterLock"/>.
        ''' write operation.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Shared Sub ReleaseWriterLock()
            Try
                cLog.m_lock.ReleaseWriterLock()
            Catch ex As Exception
                System.Console.WriteLine("Error trying to unlock the Log file after writting! " & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Delete log files greater than MAX_LOG_SIZE (1mb).
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Sub DeleteLargeLogFiles()
            Try

                Dim fn As String = cLog.m_logFilename
                If File.Exists(cFileUtils.ToValidFileName(fn, True)) Then
                    Dim fi As FileInfo = New FileInfo(fn)
                    If fi.Length > MAX_LOG_SIZE Then
                        System.Console.WriteLine("cLog.DeleteLargeLogFiles() Deleting log file " + cLog.m_logFilename)
                        File.Delete(fn)
                    End If 'fi.Length > MAX_LOG_SIZE
                End If 'File.Exists(cFileUtils.ToValidFileName(fn, True))

            Catch ex As Exception
                System.Console.WriteLine("cLog.DeleteLargeLogFiles() Exception while deleting old log file: " & ex.Message)
            End Try

        End Sub

#End Region ' Internals

#Region "Interfaces for writting Debugging files"

        ''' <summary>
        ''' Writes a load of text to a text file, obliterating anything in it's way.
        ''' </summary>
        ''' <param name="strFilename"></param>
        ''' <param name="sb"></param>
        ''' <param name="strHeader"></param>
        ''' <remarks></remarks>
        Public Shared Sub WriteTextToFile(ByVal strFilename As String, ByVal sb As Text.StringBuilder, _
                Optional ByVal bAppend As Boolean = False, Optional ByVal strHeader As String = "")

            Dim strm As System.IO.StreamWriter = Nothing

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                If bAppend Then
                    strm = System.IO.File.AppendText(strTarget)
                Else
                    strm = System.IO.File.CreateText(strTarget)
                End If

                If Not String.IsNullOrWhiteSpace(strHeader) Then
                    strm.WriteLine(strHeader)
                    strm.WriteLine()
                End If
                strm.WriteLine(sb.ToString())
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteFile(...) Error: " + ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Append the contents of a singly dimensioned array to a csv file. Each call is a new line in the file.
        ''' </summary>
        ''' <param name="strFilename">Name of the file to append</param>
        ''' <param name="array">Array whose contents get written to new line in the file</param>
        ''' <remarks>Used for debugging to test the contents of an array against the original code
        ''' the data is appended so that it can be written to multiple time each call is a new line
        ''' </remarks>
        Public Shared Sub WriteArrayToFile(ByVal strFilename As String, ByVal array() As Single, Optional ByVal strHeader As String = "")
            Dim strm As System.IO.StreamWriter
            Dim n As Integer = array.GetLength(0)
            Dim i As Integer

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                strm = System.IO.File.AppendText(strFilename)
                If strHeader <> "" Then
                    strm.Write(strHeader)
                    strm.Write(", ")
                End If

                For i = 0 To n - 1
                    strm.Write(array(i).ToString("###0.00000##"))
                    '       strm.Write(array(i))
                    If i < n - 1 Then
                        strm.Write(", ")
                    End If
                Next i
                strm.Write(Environment.NewLine)
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteArrayToFile(...) Error: " + ex.Message)
            End Try

        End Sub


        ''' <summary>
        ''' Append the contents of a 3 "map" array to file. The data will be written row, col, group. Each call is a new block in the file.
        ''' </summary>
        ''' <param name="strFilename">Name of the file to append</param>
        ''' <param name="array">Array whose contents get written to new line in the file</param>
        ''' <remarks>Used for debugging to test the contents of an array against the original code
        ''' the data is appended so that it can be written to multiple time each call is a new block
        ''' </remarks>
        <CLSCompliant(False)> _
        Public Shared Sub WriteGroupMapToFile(ByVal strFilename As String, ByVal array(,,) As Single, Optional ByVal strHeader As String = "")
            Dim strm As System.IO.StreamWriter
            Dim n1 As Integer = array.GetUpperBound(0)
            Dim n2 As Integer = array.GetUpperBound(1)
            Dim n3 As Integer = array.GetUpperBound(2)
            Dim i As Integer, j As Integer, igrp As Integer

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                strm = System.IO.File.AppendText(strFilename)

                If strHeader <> "" Then
                    strm.WriteLine(strHeader)
                End If

                For igrp = 1 To n3
                    For i = 1 To n1
                        For j = 1 To n2
                            strm.Write(array(i, j, igrp).ToString())
                            strm.Write(",")
                        Next j
                        strm.WriteLine("")
                    Next i
                    strm.WriteLine("")
                    strm.WriteLine(igrp.ToString)
                Next igrp
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteArrayToFile(...) Error: " + ex.Message)
            End Try


        End Sub

        ''' <summary>
        ''' Append the contents of a singly dimensioned array to a csv file. Each call is a new line in the file.
        ''' </summary>
        ''' <param name="strFilename">Name of the file to append</param>
        ''' <param name="array">Array whose contents get written to new line in the file</param>
        ''' <remarks>Used for debugging to test the contents of an array against the original code
        ''' the data is appended so that it can be written to multiple time each call is a new line
        ''' </remarks>
        Public Shared Sub WriteArrayToFile(ByVal strFilename As String, ByVal array() As Double, Optional ByVal strHeader As String = "")
            Dim strm As System.IO.StreamWriter
            Dim n As Integer = array.GetLength(0)
            Dim i As Integer

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                strm = System.IO.File.AppendText(strFilename)
                If strHeader <> "" Then
                    strm.Write(strHeader)
                    strm.Write(", ")
                End If
                For i = 0 To n - 1
                    'CStr(Format(GetType(Integer), i, "00")
                    strm.Write(array(i).ToString("###0.00000##"))
                    If i < n - 1 Then
                        strm.Write(", ")
                    End If
                Next i
                strm.Write(Environment.NewLine)
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteArrayToFile(...) Error: " + ex.Message)
            End Try

        End Sub


        ''' <summary>
        ''' Append the contents of a matrix array to a csv file. Each call is a new block in the file.
        ''' </summary>
        ''' <param name="strFilename">Name of the file to append</param>
        ''' <param name="array">Array whose contents get written to new line in the file</param>
        ''' <remarks>Used for debugging to test the contents of an array against the original code
        ''' the data is appended so that it can be written to multiple time each call is a new block
        ''' </remarks>
        <CLSCompliant(False)> _
        Public Shared Sub WriteMatrixToFile(ByVal strFilename As String, ByVal array(,) As Single, Optional ByVal strHeader As String = "")
            Dim strm As System.IO.StreamWriter
            Dim n1 As Integer = array.GetUpperBound(0)
            Dim n2 As Integer = array.GetUpperBound(1)
            Dim i As Integer, j As Integer

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                strm = System.IO.File.AppendText(strFilename)

                If strHeader <> "" Then
                    strm.WriteLine(strHeader)
                End If

                For i = 0 To n1
                    For j = 0 To n2
                        '  strm.Write(Format(array(i, j), "###0.00000##"))
                        strm.Write(array(i, j).ToString("###0.00000##"))
                        If j < n2 Then
                            strm.Write(", ")
                        End If

                    Next j
                    strm.Write(Environment.NewLine)
                Next i
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteArrayToFile(...) Error: " + ex.Message)
            End Try

        End Sub


        ''' <summary>
        ''' Append the contents of a 3 dimensional array to a csv file. Each call is a new block in the file.
        ''' </summary>
        ''' <param name="strFilename">Name of the file to append</param>
        ''' <param name="array">Array whose contents get written to new line in the file</param>
        ''' <remarks>Used for debugging to test the contents of an array against the original code
        ''' the data is appended so that it can be written to multiple time each call is a new block
        ''' </remarks>
        <CLSCompliant(False)> _
        Public Shared Sub WriteMatrixToFile(ByVal strFilename As String, ByVal array(,,) As Single, Optional ByVal strHeader As String = "")
            Dim strm As System.IO.StreamWriter
            Dim n1 As Integer = array.GetUpperBound(0)
            Dim n2 As Integer = array.GetUpperBound(1)
            Dim n3 As Integer = array.GetUpperBound(2)
            Dim i As Integer, j As Integer, k As Integer

            Try
                Dim strTarget As String = FixDirectory(strFilename)
                strm = System.IO.File.AppendText(strFilename)

                If strHeader <> "" Then
                    strm.WriteLine(strHeader)
                End If

                For i = 0 To n1
                    For j = 0 To n2
                        For k = 0 To n3
                            '  strm.Write(Format(array(i, j), "###0.00000##"))
                            strm.Write(array(i, j, k).ToString("###0.00000##"))
                            If k < n3 Then
                                strm.Write(", ")
                            End If
                        Next k
                        strm.WriteLine("i=" & i & " j=" & j)
                    Next j
                    '    strm.Write(ControlChars.NewLine)
                    strm.WriteLine("")
                Next i
                strm.Close()

            Catch ex As Exception
                cLog.Write("Error in WriteArrayToFile(...) Error: " + ex.Message)
            End Try


        End Sub

        Private Shared Function FixDirectory(strFileName As String) As String

            Dim strDir As String = Path.GetDirectoryName(strFileName)
            If String.IsNullOrWhiteSpace(strDir) Then
                strDir = System.AppDomain.CurrentDomain.BaseDirectory()
                strFileName = Path.Combine(strDir, strFileName)
            End If

            If Not Directory.Exists(strDir) Then
                Directory.CreateDirectory(strDir)
            End If

            Return strFileName

        End Function

#End Region

    End Class

End Namespace
