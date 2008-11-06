'==============================================================================
'
' $Log: BugReporter.vb,v $
' Revision 1.2  2008/11/06 04:57:00  jeroens
' Fixed bug 557
'
' Revision 1.1  2007/10/23 23:15:29  sherman
' Seperated BugReporter Class from AppLauncher
'
'==============================================================================

Public Class BugReporter
    Public Shared Sub InvokeBugReport()
        Dim ub As New EwEUtils.Utilities.UrlBuilder("mailto:s.lai@fisheries.ubc.ca")
        Dim sbBody As New System.Text.StringBuilder
        Dim ac As ApplicationComponents = AppLauncher.GetInstance().ApplicationComponents()
        Dim strURL As String = ""

        ub.QueryString("subject") = "EwE incident report"

        sbBody.AppendLine("I experienced the following issue with EwE6:")
        sbBody.AppendLine("(Please provide a detailed description of the issue, and steps to reproduce the error if possible.  If required, please include your model.)")
        sbBody.AppendLine("")
        sbBody.AppendLine("---------------------------------------------------")
        sbBody.AppendLine("EwE6 configuration (do not modify):")
        sbBody.AppendLine(ac.ToString())
        sbBody.AppendLine("---------------------------------------------------")
        ub.QueryString("body") = sbBody.ToString()

        Try
            System.Diagnostics.Process.Start(ub.ToString())
        Catch ex As Exception
            ' Wow, no mail client installed? 
        End Try

    End Sub

End Class
