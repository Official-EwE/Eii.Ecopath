
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

    Public Sub New(ByRef core As cCore, ByVal MSE As cMSE)

        ' This call is required by the designer.
        InitializeComponent()
        mCore = core
        mMSE = MSE
        StrategiesExtracted = False

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

    Private Sub btnEcopathParams_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEcopathParams.Click
        ''Test to see if ecosim can be run multiple times very easily
        'For i As Integer = 1 To 3
        '    mCore.RunEcoSim()
        '    MsgBox("Ecosim has been run " & i & " time(s)")
        'Next i

        mMSE.GenerateEcopathParamaters()
    End Sub

    Private Sub btnSample_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSample.Click
        mMSE.Create1DimParams("MaxRelFeedingTime")
        mMSE.Create1DimParams("FeedingTimeAdjustRate")
        mMSE.Create1DimParams("OtherMortFeedingTime")
        mMSE.Create1DimParams("PredEffectFeedingTime")
        mMSE.Create1DimParams("DenDepCatchability")
        mMSE.Create1DimParams("QBMaxxQBio")
        mMSE.Create1DimParams("SwitchingPower")
        'mMSE.Create2DimParams("DietComposition")
    End Sub

    Private Sub GenerateEmptyDietcsv()
        Dim sPath As String = mMSE.DataPath & "\DistributionParameters"
        Dim diet_csvout As New StreamWriter(Path.Combine(sPath & "\DietComposition.csv"), False)
        Dim upper As Single
        Dim lower As Single

        diet_csvout.Write("Predator,Prey,PredIndex,PreyIndex,Interacts,Lower,Upper")
        diet_csvout.WriteLine()

        For iPred As Integer = 1 To mCore.nLivingGroups
            If mCore.EcoPathGroupInputs(iPred).ImpDiet > 0 Then
                upper = mCore.EcoPathGroupInputs(iPred).ImpDiet + 0.1
                If upper > 1 Then upper = 1
                lower = mCore.EcoPathGroupInputs(iPred).ImpDiet - 0.1
                If lower < 0 Then lower = 0
                diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,Imports, " & iPred & ",0,1," & lower & "," & upper)
            Else
                diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,Imports, " & iPred & ",0,0,0,1")
            End If
            For iPrey As Integer = 1 To mCore.nGroups
                If mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) > 0 Then
                    upper = mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) + 0.1
                    If upper > 1 Then upper = 1
                    lower = mCore.EcoPathGroupInputs(iPred).DietComp(iPrey) - 0.1
                    If lower < 0 Then lower = 0
                    diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,""" & mCore.EcoPathGroupInputs(iPrey).Name & """," & iPred & "," & iPrey & ",1," & lower & "," & upper)
                Else
                    diet_csvout.WriteLine("""" & mCore.EcoPathGroupInputs(iPred).Name & """,""" & mCore.EcoPathGroupInputs(iPrey).Name & """," & iPred & "," & iPrey & ",0,0,1")
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

    Private Sub btnVulnerabilities_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVulnerabilities.Click
        mMSE.CreateVulnerabilities()
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim FolderBrowserDialog1 As New FolderBrowserDialog

        With FolderBrowserDialog1
            .RootFolder = Environment.SpecialFolder.Desktop
            .SelectedPath = "c:\windows"
            .Description = "Select the directory to retrieve data from and output results to"
            If .ShowDialog = DialogResult.OK Then
                mMSE.DataPath = .SelectedPath
            End If

        End With

    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim lpsolve As New cLPSolver.lpsolve55
        Dim lp As Integer
        Dim pv(5) As Double

        '* Create a new LP model */
        lp = cLPSolver.lpsolve55.make_lp(3, 2)
        If lp = Nothing Then
            Console.WriteLine("Unable to create new LP model\n")
        End If
        cLPSolver.lpsolve55.solve(lp)

        cLPSolver.lpsolve55.get_primal_solution(lp, pv)

        cLPSolver.lpsolve55.Init()

        lp = cLPSolver.lpsolve55.make_lp(0, 2)
        cLPSolver.lpsolve55.set_outputfile(lp, CurDir() & "\result_lin_prog_MSE.txt")

        cLPSolver.lpsolve55.set_timeout(lp, 0)

        cLPSolver.lpsolve55.print_str(lp, "Now we add the first constraint" & vbLf)
        cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 3 / 5, 1}, cLPSolver.lpsolve55.lpsolve_constr_types.EQ, 4)
        cLPSolver.lpsolve55.print_lp(lp)

        cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 1, 0}, cLPSolver.lpsolve55.lpsolve_constr_types.LE, 5)

        cLPSolver.lpsolve55.set_obj_fn(lp, New Double() {0, 0, 1})
        cLPSolver.lpsolve55.print_lp(lp)

        cLPSolver.lpsolve55.print_str(lp, "Now solve the problem with cLPSolver.lpsolve55.print_str(lp), solve(lp)" & vbLf)
        cLPSolver.lpsolve55.set_minim(lp)

        cLPSolver.lpsolve55.print_str(lp, cLPSolver.lpsolve55.solve(lp) & ": " & cLPSolver.lpsolve55.get_objective(lp) & vbLf)

        cLPSolver.lpsolve55.print_str(lp, "The value is 0, this means we found an optimal solution" & vbLf)
        cLPSolver.lpsolve55.print_str(lp, "We can display this solution with cLPSolver.lpsolve55.print_objective(lp) and lpsolve55.print_solution(lp)" & vbLf)
        cLPSolver.lpsolve55.print_objective(lp)
        cLPSolver.lpsolve55.print_solution(lp, 1)
        cLPSolver.lpsolve55.print_constraints(lp, 1)


        'cLPSolver.lpsolve55.print_str(lp, "Now we add the first constraint" & vbLf)
        'cLPSolver.lpsolve55.print_str(lp, "cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 120, 210}, lpsolve55.lpsolve_constr_types.LE, 15000)" & vbLf)
        'cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 120, 210}, cLPSolver.lpsolve55.lpsolve_constr_types.LE, 15000)
        'cLPSolver.lpsolve55.print_lp(lp)

        'cLPSolver.lpsolve55.print_str(lp, "...and the 2nd constraint..." & vbLf)
        'cLPSolver.lpsolve55.print_str(lp, "cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 110, 30}, lpsolve55.lpsolve_constr_types.LE, 4000)" & vbLf)
        'cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 110, 30}, cLPSolver.lpsolve55.lpsolve_constr_types.LE, 4000)
        'cLPSolver.lpsolve55.print_lp(lp)

        'cLPSolver.lpsolve55.print_str(lp, "...and the final constraint." & vbLf)
        'cLPSolver.lpsolve55.print_str(lp, "lpsolve55.add_constraint(lp, New Double() {0, 1, 1}, cLPSolver.lpsolve55.lpsolve_constr_types.LE, 75)" & vbLf)
        'cLPSolver.lpsolve55.add_constraint(lp, New Double() {0, 1, 1}, cLPSolver.lpsolve55.lpsolve_constr_types.LE, 75)
        'cLPSolver.lpsolve55.print_lp(lp)

        'cLPSolver.lpsolve55.print_str(lp, "Set the objective function" & vbLf)
        'cLPSolver.lpsolve55.print_str(lp, "cLPSolver.lpsolve55.set_obj_fn(lp, New Double() {0, 2, 3, -2, 3})" & vbLf)
        'cLPSolver.lpsolve55.set_obj_fn(lp, New Double() {0, 143, 60})
        'cLPSolver.lpsolve55.print_lp(lp)

        'cLPSolver.lpsolve55.print_str(lp, "Now solve the problem with cLPSolver.lpsolve55.print_str(lp), solve(lp)" & vbLf)
        'cLPSolver.lpsolve55.set_maxim(lp)
        'cLPSolver.lpsolve55.print_str(lp, cLPSolver.lpsolve55.solve(lp) & ": " & cLPSolver.lpsolve55.get_objective(lp) & vbLf)

        'cLPSolver.lpsolve55.print_str(lp, "The value is 0, this means we found an optimal solution" & vbLf)
        'cLPSolver.lpsolve55.print_str(lp, "We can display this solution with cLPSolver.lpsolve55.print_objective(lp) and lpsolve55.print_solution(lp)" & vbLf)
        'cLPSolver.lpsolve55.print_objective(lp)
        'cLPSolver.lpsolve55.print_solution(lp, 1)
        'cLPSolver.lpsolve55.print_constraints(lp, 1)

        Demo()
    End Sub

    Private Sub Demo()

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

    Private Sub btnEcopathParams2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEcopathParams2.Click
        mMSE.GenerateEcopathParamaters2()
    End Sub
End Class
