Imports EwEUtils.Utilities

Public Class dlgSpaceRun

    Public Sub New(strHist As String, iYearHist As Integer, strFore As String, iYearFore As Integer, dNoData As Double)
        Me.InitializeComponent()
        Me.m_tbxFileHist.Text = strHist
        Me.m_tbxYearHist.Text = CStr(iYearHist)
        Me.m_tbxFileFore.Text = strFore
        Me.m_tbxYearFore.Text = CStr(iYearFore)
        Me.m_tbxNoData.Text = CStr(dNoData)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.CenterToParent()
    End Sub

    Public ReadOnly Property YearHist As Integer
        Get
            Dim i As Integer
            Integer.TryParse(Me.m_tbxYearHist.Text, i)
            Return i
        End Get
    End Property

    Public ReadOnly Property RunHistorical As String
        Get
            Return cFileUtils.ToValidFileName(Me.m_tbxFileHist.Text, False)
        End Get
    End Property

    Public ReadOnly Property YearForecast As Integer
        Get
            Dim i As Integer
            Integer.TryParse(Me.m_tbxYearFore.Text, i)
            Return i
        End Get
    End Property

    Public ReadOnly Property RunForecast As String
        Get
            Return cFileUtils.ToValidFileName(Me.m_tbxFileFore.Text, False)
        End Get
    End Property

    Public ReadOnly Property NoData As Double
        Get
            Dim dNoData As Double
            Double.TryParse(Me.m_tbxNoData.Text, dNoData)
            Return dNoData
        End Get
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

End Class