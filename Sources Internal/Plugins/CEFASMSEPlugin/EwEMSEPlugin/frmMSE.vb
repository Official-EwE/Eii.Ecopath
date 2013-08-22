
Imports EwECore
Imports System.IO
Imports LumenWorks.Framework.IO.Csv

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Public Class frmMSE

    Dim m_bInitOK As Boolean
    Dim m_uic As cUIContext
    Dim mCore As cCore
    Dim mMSE As cMSE
    Dim StrategiesExtracted As Boolean 'this is a flag used to determine whether the strategies have already been loads and if so not to load them again

    Dim frmTargetF As frmTFMpolicy
    Dim frmDisParams As frmDistributionParameters

    Public Sub New(ByRef core As cCore, ByVal MSE As cMSE)

        ' This call is required by the designer.
        InitializeComponent()
        mCore = core
        mMSE = MSE
        StrategiesExtracted = False
        lblDataDirectoryPath.Text = mMSE.DataPath

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub Initialize(ByVal uic As cUIContext)
        m_bInitOK = False
        Try
            Me.m_uic = uic
            m_bInitOK = True
            System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
        Catch ex As Exception
            '  cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try
    End Sub

    Public Sub StartForm(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form)
        frmPlugin = Me
    End Sub

    Private Sub btnSample_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSample.Click
        mMSE.GenerateEcopathParamaters()
        mMSE.Create1DimParams("MaxRelFeedingTime")
        mMSE.Create1DimParams("FeedingTimeAdjustRate")
        mMSE.Create1DimParams("OtherMortFeedingTime")
        mMSE.Create1DimParams("PredEffectFeedingTime")
        mMSE.Create1DimParams("DenDepCatchability")
        mMSE.Create1DimParams("QBMaxxQBio")
        mMSE.Create1DimParams("SwitchingPower")
        'mMSE.Create2DimParams("DietComposition")
        mMSE.CreateVulnerabilities()
    End Sub

    Private Sub GenerateEmptyDietcsv()
        Dim sPath As String = mMSE.DataPath & "\DistributionParameters"
        Dim diet_csvout As New StreamWriter(Path.Combine(sPath & "\DietComposition.csv"), False)
        Dim mean As Single

        diet_csvout.Write("Predator,Prey,PredIndex,PreyIndex,Interacts,Mean")
        diet_csvout.WriteLine()

        For iPred As Integer = 1 To mCore.nLivingGroups
            If mCore.EcoPathGroupInputs(iPred).ImpDiet > 0 Then
                mean = mCore.EcoPathGroupInputs(iPred).ImpDiet
                diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,Imports," & iPred & ",0,1," & mean)
            Else
                diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,Imports," & iPred & ",0,0,0")
            End If
            For iPrey As Integer = 1 To mCore.nGroups
                If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) > 0 Then
                    mean = mCore.EcoPathGroupInputs(iPred).DietComp(iPrey)
                    diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,""" & mCore.EcoPathGroupInputs(iPrey).Name & """," & iPred & "," & iPrey & ",1," & mean)
                Else
                    diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,""" & mCore.EcoPathGroupInputs(iPrey).Name & """," & iPred & "," & iPrey & ",0,0")
                End If
            Next
        Next

        diet_csvout.Dispose()
    End Sub

    Private Sub btnGamma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGamma.Click

        GenerateEmptyDietcsv()


    End Sub

    Private Sub btnLoadSampled_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadSampled.Click
        If StrategiesExtracted = False Then 'This is to prevent it loading the strategies more than once
            mMSE.ExtractHCR()
            StrategiesExtracted = True
        End If
        mMSE.ChangeEffortFlag = True
        mMSE.LoadSampledParams()
        mMSE.ChangeEffortFlag = False
    End Sub




    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim FolderBrowserDialog1 As New FolderBrowserDialog

        With FolderBrowserDialog1
            .RootFolder = Environment.SpecialFolder.Desktop
            .SelectedPath = "c:\windows"
            .Description = "Select the directory to retrieve data from and output results to"
            If .ShowDialog = DialogResult.OK Then
                mMSE.DataPath = .SelectedPath
                lblDataDirectoryPath.Text = .SelectedPath
            End If

        End With

    End Sub


    Private Sub btShowTFMForm_Click(sender As System.Object, e As System.EventArgs) Handles btShowTFMForm.Click
        Dim bhasForm As Boolean

        'First make sure the Harvest Controls Rules have been loaded
        'this is so the interface has some data
        If StrategiesExtracted = False Then 'This is to prevent it loading the strategies more than once
            mMSE.ExtractHCR()
            StrategiesExtracted = True
        End If

        'Ok now the interface
        If Me.frmTargetF IsNot Nothing Then
            bHasForm = Not frmTargetF.IsDisposed
        End If
        If Not bHasForm Then
            frmTargetF = New frmTFMpolicy()
            frmTargetF.Init(Me.m_uic, Me.mMSE)
        End If

        frmTargetF.Show()
    End Sub


    Private Sub frmMSE_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtArea.Text = mCore.EwEModel.Area

    End Sub



    Private Sub btnAdvancedSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdvancedSettings.Click
        If Label2.Visible = False Then
            Label2.Show()
            txtTolerance.Show()
            Label6.Show()
            Panel3.Show()
            btnGamma.Show()
            btnEcopathParams2.Show()
            Button2.Show()
        Else
            Label2.Hide()
            txtTolerance.Hide()
            Label6.Hide()
            Panel3.Hide()
            btnGamma.Hide()
            btnEcopathParams2.Hide()
            Button2.Hide()
        End If

    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint

    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        mMSE.Create1DimParams("MaxRelFeedingTime")
        mMSE.Create1DimParams("FeedingTimeAdjustRate")
        mMSE.Create1DimParams("OtherMortFeedingTime")
        mMSE.Create1DimParams("PredEffectFeedingTime")
        mMSE.Create1DimParams("DenDepCatchability")
        mMSE.Create1DimParams("QBMaxxQBio")
        mMSE.Create1DimParams("SwitchingPower")
        'mMSE.Create2DimParams("DietComposition")
        mMSE.CreateVulnerabilities()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        txtArea.Text = mCore.EwEModel.Area
    End Sub

    Private Sub btnDistParams_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDistParams.Click
        Dim bhasForm As Boolean

        'Ok now the interface
        If Me.frmDisParams IsNot Nothing Then
            bhasForm = Not frmDisParams.IsDisposed
        End If
        If Not bhasForm Then
            frmDisParams = New frmDistributionParameters()
            frmDisParams.Init(Me.m_uic, Me.mMSE, mMSE.DataPath, mCore)
        End If

        frmDisParams.Show()
    End Sub
End Class
