Imports EwEUtils.Utilities
Imports EwECore

Public Class dlgSimRun

    Public Sub New(core As cCore, strHist As String, iYearHist As Integer, strFore As String, iYearFore As Integer)
        Me.InitializeComponent()

        Dim parms As cEcospaceModelParameters = core.EcospaceModelParameters
        Me.m_tbxFileHist.Text = strHist
        Me.m_tbxYearHist.Text = CStr(iYearHist)
        Me.m_tbxFileFore.Text = strFore
        Me.m_tbxYearFore.Text = CStr(iYearFore)

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim bError As Boolean = cFishMIPPlugin.GetInstance().Configuration.IsEmpty
        Me.m_pbAlert.Visible = bError
        Me.m_lblError.Visible = bError

        Me.m_pbAlert.Image = ScientificInterfaceShared.My.Resources.Warning

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

    Private Sub m_btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_btnOK_Click(sender As Object, e As EventArgs) Handles m_btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

End Class