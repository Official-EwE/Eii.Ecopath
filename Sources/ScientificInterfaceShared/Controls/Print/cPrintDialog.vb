Public Class cPrintDialog
    Inherits PrintPreviewDialog

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides Sub OnPrint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPrint(e)
    End Sub

End Class
