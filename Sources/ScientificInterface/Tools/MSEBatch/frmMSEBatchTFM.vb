
Option Explicit On
Option Strict On


Imports EwECore
Imports EwEUtils.Core
Imports EwECore.MSEBatchManager


Public Class frmMSEBatchTFM


    Private Class cCBWrapper
        Private m_grp As cEcoPathGroupInput
        Public Sub New(Group As cEcoPathGroupInput)
            Me.m_grp = Group
        End Sub

        Public Overrides Function ToString() As String
            Return m_grp.name
        End Function

        Public ReadOnly Property theGroup As cEcoPathGroupInput
            Get
                Return Me.m_grp
            End Get
        End Property
    End Class

    Private m_BatchManager As EwECore.MSEBatchManager.cMSEBatchManager

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.grdGroups.UIContext = Me.UIContext
            Me.grdIters.UIContext = Me.UIContext
        End Set
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        m_BatchManager = Me.UIContext.Core.MSEBatchManager

        Me.txNTFM.Text = Me.m_BatchManager.Parameters.nTFMIteration.ToString


        Me.rbCalcTypePercent.Tag = eMSEBatchIterCalcTypes.Percent
        Me.rbCalcTypeValue.Tag = eMSEBatchIterCalcTypes.UpperLowerValues

        UpdateControls()


    End Sub


    Private Sub txNTFM_TextChanged(sender As System.Object, e As System.EventArgs) Handles txNTFM.TextChanged

        Dim newValue As Integer = Integer.Parse(Me.txNTFM.Text)
        If newValue > 0 And newValue <> Me.m_BatchManager.Parameters.nTFMIteration Then
            Me.m_BatchManager.Parameters.nTFMIteration = newValue
        End If

    End Sub

    Private Sub onCalcIterValues(sender As Object, e As System.EventArgs) Handles btCalcIters.Click

        Me.m_BatchManager.CalculateIterationValues()

    End Sub

    Private Sub UpDwnIter_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)

    End Sub


    Private Sub UpDwnIter_ValueChanged(sender As System.Object, e As System.EventArgs) Handles UpDwnIter.ValueChanged
        Dim iter As Integer = CInt(Me.UpDwnIter.Value)
        If Me.m_BatchManager Is Nothing Then Exit Sub
        If iter <= Me.m_BatchManager.Parameters.nTFMIteration Then
            Me.grdGroups.iCurIter = iter
        End If
    End Sub


    Private Sub OnIterCalcTypeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
          Handles rbCalcTypePercent.CheckedChanged, rbCalcTypeValue.CheckedChanged

        Try

            Dim rb As RadioButton = DirectCast(sender, RadioButton)

            If rb.Tag IsNot Nothing Then

                If rb.Checked Then
                    Me.m_BatchManager.Parameters.IterCalcType = DirectCast(rb.Tag, EwEUtils.Core.eMSEBatchIterCalcTypes)

                    Me.grdGroups.RefreshContent()
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim pars As cMSEBatchParameters = Me.m_BatchManager.Parameters
        Me.rbCalcTypePercent.Checked = (pars.IterCalcType = eMSEBatchIterCalcTypes.Percent)
        Me.rbCalcTypeValue.Checked = (pars.IterCalcType = eMSEBatchIterCalcTypes.UpperLowerValues)

        For igrp As Integer = 1 To Me.UIContext.Core.nGroups
            Dim grp As cEcoPathGroupInput = Me.UIContext.Core.EcoPathGroupInputs(igrp)
            If grp.IsFished Then
                Me.cbGroups.Items.Add(New cCBWrapper(grp))
            End If
        Next

    End Sub


    Private Sub cbGroups_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbGroups.SelectedIndexChanged
        If Me.m_BatchManager Is Nothing Then Exit Sub
        Dim grp As cEcoPathGroupInput = DirectCast(Me.cbGroups.SelectedItem, cCBWrapper).theGroup
        Me.grdIters.iSelGroup = grp.Index
    End Sub


End Class