Imports EwEUtils.Utilities

Public Class dlgSimRun

    Public Sub New(strRun As String, iYear As Integer)
        Me.InitializeComponent()
        Me.m_tbxRun.Text = strRun
        Me.m_tbxYear.Text = CStr(iYear)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.CenterToParent()
    End Sub

    Public ReadOnly Property Year As Integer
        Get
            Dim i As Integer
            Integer.TryParse(Me.m_tbxYear.Text, i)
            Return i
        End Get
    End Property

    Public ReadOnly Property RunName As String
        Get
            Return cFileUtils.ToValidFileName(Me.m_tbxRun.Text, False)
        End Get
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class