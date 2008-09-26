'==============================================================================
'
' $Log: BugReporter.vb,v $
' Revision 1.1  2008/09/26 07:32:08  sherman
' --== DELETED HISTORY ==--
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

        System.Diagnostics.Process.Start(ub.ToString())
    End Sub
End Class
