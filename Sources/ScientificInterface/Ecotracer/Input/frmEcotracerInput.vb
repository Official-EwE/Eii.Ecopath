' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecotracer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing the main input interface for contaminant tracing.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmEcotracerInput

#Region " Private vars "

        Private m_fpCZeroEnv As cEwEFormatProvider = Nothing
        Private m_fpCInflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCOutflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCDecayEnv As cEwEFormatProvider = Nothing

        Private m_fpMAxTS As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

#End Region ' Constructors

#Region " Events "

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            Debug.Assert(Me.UIContext IsNot Nothing)

            Dim ecotracerModelParams As cEcotracerModelParameters = Me.Core.EcotracerModelParameters()

            Me.m_fpCZeroEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCZeroEnv, ecotracerModelParams, eVarNameFlags.CZero)
            Me.m_fpCDecayEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCDecayRateEnv, ecotracerModelParams, eVarNameFlags.CPhysicalDecayRate)
            Me.m_fpCInflowEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCInflowEnv, ecotracerModelParams, eVarNameFlags.CInflow)
            Me.m_fpCOutflowEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCLossEnv, ecotracerModelParams, eVarNameFlags.COutflow)
            Me.m_fpMAxTS = New cPropertyFormatProvider(Me.UIContext, Me.m_tbMaxTS, ecotracerModelParams, eVarNameFlags.ConMaxTimeSteps)

            Me.m_grid.UIContext = Me.UIContext

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
            Me.UpdateForcingControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

            Me.m_fpCDecayEnv.Release()
            Me.m_fpCInflowEnv.Release()
            Me.m_fpCOutflowEnv.Release()
            Me.m_fpCZeroEnv.Release()
            Me.m_fpMAxTS.Release()

            Me.m_grid.UIContext = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            Me.UpdateForcingControls()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdateForcingControls()

            Dim ecotracerModelParams As cEcotracerModelParameters = Me.Core.EcotracerModelParameters()
            Dim ffm As cForcingFunctionShapeManager = Me.Core.ForcingShapeManager()
            Dim iSel As Integer = 0

            Me.m_cmbEnvInflowFF.Items.Clear()
            iSel = Me.m_cmbEnvInflowFF.Items.Add(SharedResources.GENERIC_VALUE_NONE) ' Just to be correct
            For iFF As Integer = 0 To ffm.Count - 1
                Dim ff As cForcingFunction = ffm(iFF)
                Dim iItem As Integer = Me.m_cmbEnvInflowFF.Items.Add(ff)
                If (ff.Index = ecotracerModelParams.ConForceNumber) Then iSel = iItem
            Next
            Me.m_cmbEnvInflowFF.SelectedIndex = iSel

        End Sub

        Private Sub OnFileSelected(sender As Object, e As EventArgs) Handles m_btSelectFile.Click
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(SharedResources.FILEFILTER_CSV & "|" & SharedResources.FILEFILTER_XYZ & "|" & SharedResources.FILEFILTER_TEXT)
            If cmdFO.Result = System.Windows.Forms.DialogResult.OK Then
                Dim manager As EcospaceTimeSeries.cEcospaceTimeSeriesManager = Me.Core.EcospaceTimeSeriesManager
                Dim InputFile As String = cmdFO.FileNames(0)
                If manager.Load(InputFile, "", eVarNameFlags.Concentration) Then
                    Me.m_lbConcentrationFile.Text = manager.ContaminantInputFileName
                End If ' Load with default output file name
            End If
        End Sub

        Private Sub OnClearFile(sender As Object, e As EventArgs) Handles m_btClearFile.Click
            Me.Core.EcospaceTimeSeriesManager.Clear()
            Me.m_lbConcentrationFile.Text = ""
        End Sub

        Private Sub OnFormatFF(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbEnvInflowFF.Format
            If (TypeOf e.ListItem Is cForcingFunction) Then
                Dim fmt As New cShapeDataFormatter()
                e.Value = fmt.ToString(e.ListItem)
            End If
        End Sub

        Private Sub OnEnvFFSelected(sender As Object, e As EventArgs) Handles m_cmbEnvInflowFF.SelectedIndexChanged

            Dim ecotracerModelParams As cEcotracerModelParameters = Me.Core.EcotracerModelParameters()
            Dim iFF As Integer = 0

            If (TypeOf Me.m_cmbEnvInflowFF.SelectedItem Is cForcingFunction) Then
                iFF = DirectCast(Me.m_cmbEnvInflowFF.SelectedItem, cForcingFunction).Index
            End If
            ecotracerModelParams.ConForceNumber = iFF

        End Sub

#End Region ' Internals

    End Class

End Namespace
