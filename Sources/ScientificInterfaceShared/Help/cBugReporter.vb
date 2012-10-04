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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports EwEPlugin
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

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
        sbBody.AppendLine(cSysConfig.OSVersion())
        sbBody.AppendLine(cSysConfig.NETVersion())

        sbBody.AppendLine("Loaded modules:")
        For Each an In cAssemblyUtils.GetSummary(Assembly.GetExecutingAssembly)
            sbBody.AppendLine(String.Format("* {0}={2},{1}", _
                                            an.Name, cStringUtils.ToHexString(an.GetPublicKeyToken), an.Version))
        Next
        If (pm IsNot Nothing) Then
            sbBody.AppendLine("Loaded plug-ins:")
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
