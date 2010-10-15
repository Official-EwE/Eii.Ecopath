Option Explicit On
Imports System.IO
Imports System.Xml
Imports System.Threading
Imports EwEUtils.SystemUtilities

''' <summary>
''' Class encapsulating writing of messages to a log or the interface
''' </summary>
''' <remarks></remarks>
Public Class cLog

#Region "Private Data"

    Private Shared m_xmlWriter As cXMLLogWriter
    Private Shared m_logFilename As String
    Private Shared m_modelname As String = "No Model Loaded"
    Private Shared m_lock As New ReaderWriterLock

#End Region

#Region "Overloaded Write() methods"

    ''' <summary>
    ''' Singelton interface for creating a cXMLLogWriter object
    ''' </summary>
    ''' <returns>A cXMLLogWriter object that can be Opened and written to.</returns>
    ''' <remarks>If cLog.IntLog(filename) has been call then the cXMLLogWriter will use this file. If not the default file will be used "EwELog.xml"</remarks>
    Private Shared Function getWriter() As cXMLLogWriter
        If m_xmlWriter Is Nothing Then
            If String.IsNullOrEmpty(m_logFilename) Then
                m_logFilename = Path.Combine(cSystemUtils.ApplicationSettingsPath(), "EwELog.xml")
                'm_logFilename = System.AppDomain.CurrentDomain.BaseDirectory() + "EwELog.xml"
            End If
            m_xmlWriter = New cXMLLogWriter(m_logFilename, m_modelname)
        End If
        Return m_xmlWriter
    End Function

    ''' <summary>
    ''' Start a new log file with the model name as part of the log file name
    ''' </summary>
    ''' <param name="strModelName">Name of the model that this log file is for</param>
    ''' <remarks></remarks>
    Public Shared Sub InitLog(ByVal strModelName As String)

        ' Prevent path characters in the modelname to cause problems
        strModelName = strModelName.Replace("\", "-")
        strModelName = strModelName.Replace("/", "-")
        ' Store new
        m_modelname = strModelName

        ' ToDo_JS 12Oct2010: Under windows 7 apps are officially not allowed to
        '                    make changes to files in the programs directories 
        '                    any more. See bug report http://sources.ecopath.org/trac/Ecopath/ticket/794

        ' m_logFilename = System.AppDomain.CurrentDomain.BaseDirectory() & "Log_" & ModelName & "_" & Format(Date.Now, "yy-M-d-H-m") & ".xml"
        m_logFilename = System.AppDomain.CurrentDomain.BaseDirectory() & "EwELog_" & m_modelname & ".xml"
        WriteSessionStarted()
        m_xmlWriter = Nothing
    End Sub

    Private Shared Sub WriteSessionStarted()
        Dim xmlStrm As cXMLLogWriter

        Try

            xmlStrm = getWriter()

            If xmlStrm.Open() Then
                xmlStrm.WriteElementString("Session_Started", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                xmlStrm.WriteEndDocument()

                xmlStrm.Close()
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            If Not xmlStrm Is Nothing Then
                xmlStrm.Close()
            End If
        End Try
    End Sub



    ''' <summary>
    ''' Write exception messages to the log
    ''' </summary>
    ''' <param name="theException">Exception to log </param>
    ''' <remarks>Used to log all the messages in an exception.  This is potentially hazardous as it assumes the xml file ends with the doc tag, which may not be the case.
    '''</remarks>
    Public Shared Sub Write(ByVal theException As Exception)
        Dim xmlStrm As cXMLLogWriter

        Try

            'make this thread safe
            If Not AcquireWriterLock() Then
                'failed to get a lock on the file... just skip the file write
                Exit Sub
            End If

            xmlStrm = getWriter()

            'append to the end of the stream
            If xmlStrm.Open() Then

                'now the message
                xmlStrm.WriteStartElement("Exception_Messages")
                xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))

                Dim thisEx As Exception = theException
                Do While thisEx IsNot Nothing
                    xmlStrm.WriteStartElement("Exception")
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

            Debug.Assert(False, ex.Message)
            If Not xmlStrm Is Nothing Then
                xmlStrm.Close()
            End If
        End Try

        ReleaseWriterLock()

    End Sub

    ''' <summary>
    ''' Write a cMessage object to the log
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Shared Sub Write(ByVal message As cMessage)
        Dim xmlStrm As cXMLLogWriter

        Try
            If Not AcquireWriterLock() Then
                'failed to get a lock on the file... just skip the file write
                Exit Sub
            End If

            xmlStrm = getWriter()

            'append to the end of the stream
            If xmlStrm.Open() Then

                xmlStrm.WriteStartElement(message.Importance.ToString & "_Message") '????
                xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                xmlStrm.WriteElementString("Message", message.Message)
                xmlStrm.WriteElementString("Message_Type", message.Type.ToString)
                xmlStrm.WriteElementString("Message_Source", message.Source.ToString)
                xmlStrm.WriteElementString("Message_DataType", message.DataType.ToString)
                xmlStrm.WriteEndElement() 'Msg
                xmlStrm.WriteEndDocument()

                xmlStrm.Close()
            End If

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            If Not xmlStrm Is Nothing Then
                xmlStrm.Close()
            End If

        End Try

        ReleaseWriterLock()

    End Sub

    ''' <summary>
    ''' Write a string to the application log
    ''' </summary>
    ''' <param name="msg">Message string to write</param>
    ''' <remarks></remarks>
    Public Shared Sub Write(ByVal msg As String)
        Dim xmlStrm As cXMLLogWriter

        Try

            If Not AcquireWriterLock() Then
                'failed to get a lock on the file... just skip the file write
                Exit Sub
            End If

            xmlStrm = getWriter()

            'append to the end of the stream
            If xmlStrm.Open() Then

                xmlStrm.WriteStartElement("Log_Message") '????
                xmlStrm.WriteAttributeString("Date", String.Format("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()))
                xmlStrm.WriteElementString("Message", msg)
                xmlStrm.WriteEndElement() 'Msg
                xmlStrm.WriteEndDocument()

                xmlStrm.Close()
            End If

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            If Not xmlStrm Is Nothing Then
                xmlStrm.Close()
            End If
        End Try

        ReleaseWriterLock()

    End Sub

    ''' <summary>
    ''' ReaderWriterLock.AcquireWriterLock() will throw an exception if it times out! Bitch... this keeps the exception handling out of the main code
    ''' </summary>
    ''' <returns>True if a lock was acquired.</returns>
    ''' <remarks></remarks>
    Private Shared Function AcquireWriterLock() As Boolean
        Try
            m_lock.AcquireWriterLock(1000)
            Return True
        Catch ex As Exception
            System.Console.WriteLine("Error trying to lock the Log file for writting! " & ex.Message)
            Return False
        End Try
    End Function
    Private Shared Sub ReleaseWriterLock()
        Try
            m_lock.ReleaseWriterLock()
        Catch ex As Exception
            System.Console.WriteLine("Error trying to unlock the Log file after writting! " & ex.Message)
        End Try
    End Sub


#End Region

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
        Dim strTarget As String = strFilename

        Try
            If Not String.IsNullOrEmpty(Path.GetDirectoryName(strTarget)) Then
                strTarget = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), strFilename)
            End If

            If bAppend Then
                strm = System.IO.File.AppendText(strTarget)
            Else
                strm = System.IO.File.CreateText(strTarget)
            End If

            If strHeader <> "" Then
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
            strm = System.IO.File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory() + strFilename)
            If strHeader <> "" Then
                strm.Write(strHeader)
                strm.Write(", ")
            End If

            For i = 0 To n - 1
                strm.Write(Format(array(i), "###0.00000##"))
                '       strm.Write(array(i))
                If i < n - 1 Then
                    strm.Write(", ")
                End If
            Next i
            strm.Write(ControlChars.CrLf)
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
            strm = System.IO.File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory() + strFilename)
            If strHeader <> "" Then
                strm.Write(strHeader)
                strm.Write(", ")
            End If
            For i = 0 To n - 1
                'CStr(Format(GetType(Integer), i, "00")
                strm.Write(Format(array(i), "###0.00000##"))
                If i < n - 1 Then
                    strm.Write(", ")
                End If
            Next i
            strm.Write(ControlChars.CrLf)
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
            Dim hardwiredPath As String = "C:\Documents and Settings\Me\My Documents\Projects\EcoPath Ecosim\"
            strm = System.IO.File.AppendText(hardwiredPath + strFilename)
            ' strm = System.IO.File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory() + filename)

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
                strm.Write(ControlChars.NewLine)
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
            Dim hardwiredPath As String = "C:\Documents and Settings\Me\My Documents\Projects\EcoPath Ecosim\"
            strm = System.IO.File.AppendText(hardwiredPath + strFilename)

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
#End Region

End Class
