' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

''' ===========================================================================
''' <summary>
''' Helper class for generating a bug report to be sent via the shell.
''' </summary>
''' ===========================================================================
Public Class cBugReporter

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a bug report.
    ''' </summary>
    ''' <param name="strAppName">Name of the application.</param>
    ''' <param name="strMailTo">Target email address.</param>
    ''' <param name="pm">Plug-in manager to extract components from.</param>
    ''' -----------------------------------------------------------------------
    Public Shared Function BugReport(strAppName As String,
                                     strMailTo As String,
                                     Optional pm As cPluginManager = Nothing) As String

        Dim an As AssemblyName = Nothing
        Dim ub As New cUriBuilder("mailto:" & strMailTo)
        Dim sbBody As New System.Text.StringBuilder
        Dim strURL As String = ""

        ub.QueryString("subject") = strAppName & " incident report"

        sbBody.AppendLine("I experienced the following issue:")
        sbBody.AppendLine("")
        sbBody.AppendLine("(Please provide a detailed description of the issue, and steps to reproduce if possible. If required, please zip up and attach your model)")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("------------------------------")
        sbBody.AppendLine("Configuration (do not modify):")
        sbBody.AppendLine(cSysConfig.OSVersion())
        sbBody.AppendLine(cSysConfig.NETVersion())
        For Each an In cAssemblyUtils.GetSummary(cAssemblyUtils.eSummaryFlags.EwECore)
            sbBody.AppendLine(String.Format("* {0}={2},{1}",
                                            an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
        Next

        If (pm IsNot Nothing) Then
            For Each pa As cPluginAssembly In pm.PluginAssemblies
                an = pa.AssemblyName
                sbBody.AppendLine(String.Format("- {0}={2},{1}",
                                                an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
            Next
        End If

        ub.QueryString("body") = sbBody.ToString()

        Return ub.ToString

    End Function

    'Private Shared Function SendAttachment(strAppName As String, _
    '                                strAddress As String, _
    '                                pm As cPluginManager) As Boolean

    '    Dim an As AssemblyName = Nothing
    '    Dim oMsg As New MailMessage()
    '    Dim sbBody As New System.Text.StringBuilder

    '    sbBody.AppendLine("I experienced the following issue with " & strAppName & ":")
    '    sbBody.AppendLine("")
    '    sbBody.AppendLine("(Please provide a detailed description of the issue, and steps to reproduce the error if possible. If required, please zip up and attach your model)")

    '    Dim strFile As String = Path.Combine(System.IO.Path.GetTempPath(), "EwE_config.txt")
    '    Dim swTemp As New StreamWriter(strFile)
    '    swTemp.WriteLine("EwE configuration (do not modify):")
    '    swTemp.WriteLine()
    '    swTemp.WriteLine(cSysConfig.OSVersion())
    '    swTemp.WriteLine(cSysConfig.NETVersion())
    '    swTemp.WriteLine()
    '    swTemp.WriteLine("EwE modules:")
    '    For Each an In cAssemblyUtils.GetSummary(cAssemblyUtils.eSummaryFlags.EwECore)
    '        swTemp.WriteLine(String.Format("* {0}={2},{1}", _
    '                                        an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
    '    Next

    '    If (pm IsNot Nothing) Then
    '        swTemp.WriteLine()
    '        swTemp.WriteLine("EwE plug-ins:")
    '        For Each pa As cPluginAssembly In pm.PluginAssemblies
    '            an = pa.AssemblyName
    '            swTemp.WriteLine(String.Format("- {0}={2},{1}", _
    '                                            an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
    '        Next
    '    End If
    '    swTemp.WriteLine("---------------------------------------------------")
    '    swTemp.Flush()
    '    swTemp.Close()

    '    'oMsg.From =
    '    oMsg.From = New Net.Mail.MailAddress("user@mail.com")
    '    oMsg.To.Add(New Net.Mail.MailAddress(strAddress))
    '    oMsg.Subject = strAppName & " incident report"
    '    oMsg.Body = sbBody.ToString()

    '    Dim oAttch As New Net.Mail.Attachment(strFile)
    '    oMsg.Attachments.Add(oAttch)

    '    Dim cl As New SmtpClient("127.0.0.1")
    '    Try
    '        cl.Send(oMsg)
    '    Catch ex As Exception
    '        Return False
    '    End Try

    '    Return True
    'End Function

End Class
