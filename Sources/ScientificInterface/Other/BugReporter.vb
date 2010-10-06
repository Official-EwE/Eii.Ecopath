#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Helper class for sending a bug report email to the EwE dev team.
''' </summary>
''' ===========================================================================
Public Class BugReporter

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch an email client with a pre-formatted bug report message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Function InvokeBugReport() As Boolean

        Dim ub As New UrlBuilder("mailto:ewedevteam@gmail.com")
        Dim sbBody As New System.Text.StringBuilder
        Dim strURL As String = ""

        ub.QueryString("subject") = "EwE incident report"

        sbBody.AppendLine("I experienced the following issue with EwE6:")
        sbBody.AppendLine("(Please provide a detailed description of the issue, and steps to reproduce the error if possible.  If required, please include your model.)")
        sbBody.AppendLine("")
        sbBody.AppendLine("---------------------------------------------------")
        sbBody.AppendLine("EwE6 configuration (do not modify):")
        For Each an As AssemblyName In cAssemblyUtils.GetSummary(Assembly.GetExecutingAssembly)
            sbBody.AppendLine(String.Format("* {0}={2},{1}", _
                                            an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
        Next
        sbBody.AppendLine("---------------------------------------------------")
        ub.QueryString("body") = sbBody.ToString()

        Try
            System.Diagnostics.Process.Start(ub.ToString())
        Catch ex As Exception
            ' Wow, no mail client installed? 
            Return False
        End Try
        Return True

    End Function

End Class
