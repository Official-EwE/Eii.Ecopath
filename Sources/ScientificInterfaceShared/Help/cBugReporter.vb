#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports EwEPlugin
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Helper class for sending a bug report email. The bug report body includes a
''' list of loaded assemblies to help diagnose problems.
''' </summary>
''' ===========================================================================
Public Class cBugReporter

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch an email client with a pre-formatted bug report message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Function InvokeBugReport(ByVal strApplication As String, _
                                           ByVal strMailTo As String, _
                                           Optional ByVal pm As cPluginManager = Nothing) As Boolean

        ' Sanity checks
        Debug.Assert(cStringUtils.IsValidEmail(strMailTo), "Valid email required")

        Dim an As AssemblyName = Nothing
        Dim ub As New UrlBuilder(strMailTo)
        Dim sbBody As New System.Text.StringBuilder
        Dim strURL As String = ""

        ub.QueryString("subject") = strApplication & " incident report"

        sbBody.AppendLine("I experienced the following issue with " & strApplication & ":")
        sbBody.AppendLine("")
        sbBody.AppendLine("(Please provide a detailed description of the issue, and steps to reproduce the error if possible. If required, please zip up and attach your model)")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("")
        sbBody.AppendLine("---------------------------------------------------")
        sbBody.AppendLine("EwE configuration (do not modify):")
        For Each an In cAssemblyUtils.GetSummary(Assembly.GetExecutingAssembly)
            sbBody.AppendLine(String.Format("* {0}={2},{1}", _
                                            an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
        Next
        If (pm IsNot Nothing) Then
            For Each pa As cPluginAssembly In pm.PluginAssemblies
                an = pa.AssemblyName
                sbBody.AppendLine(String.Format("- {0}={2},{1}", _
                                                an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
            Next
        End If

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
