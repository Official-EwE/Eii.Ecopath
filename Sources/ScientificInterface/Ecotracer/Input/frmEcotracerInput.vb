#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecotracer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmEcotracerInput

#Region " Private vars "

        Private m_fpCZeroEnv As cEwEFormatProvider = Nothing
        Private m_fpCInflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCOutflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCDecayEnv As cEwEFormatProvider = Nothing
        Private m_fpInflowForceNumberEnv As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Debug.Assert(Me.UIContext IsNot Nothing)

            Dim ecotracerModelParams As cEcotracerModelParameters = Me.UIContext.Core.EcotracerModelParameters()

            Me.m_fpInflowForceNumberEnv = New cPropertyFormatProvider(Me.UIContext, Me.cmbEnvInflowFF, ecotracerModelParams, eVarNameFlags.ConForceNumber)
            Me.m_fpCZeroEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCZeroEnv, ecotracerModelParams, eVarNameFlags.CZero)
            Me.m_fpCDecayEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCDecayRateEnv, ecotracerModelParams, eVarNameFlags.CDecay)
            Me.m_fpCInflowEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCInflowEnv, ecotracerModelParams, eVarNameFlags.CInflow)
            Me.m_fpCOutflowEnv = New cPropertyFormatProvider(Me.UIContext, Me.m_tbCLossEnv, ecotracerModelParams, eVarNameFlags.COutflow)

            Me.m_grid.UIContext = Me.UIContext

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
            Me.UpdateFFFormatProviders()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            Me.m_fpCDecayEnv.Release()
            Me.m_fpCInflowEnv.Release()
            Me.m_fpCOutflowEnv.Release()
            Me.m_fpCZeroEnv.Release()
            Me.m_fpInflowForceNumberEnv.Release()

            Me.m_grid.UIContext = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Me.UpdateFFFormatProviders()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdateFFFormatProviders()
            ' Assemble list of FFs
            Dim ffm As cForcingFunctionManager = Me.UIContext.Core.ForcingShapeManager()
            Dim aItems(ffm.Count) As Object

            aItems(0) = My.Resources.GENERIC_VALUE_NONE
            For iFF As Integer = 0 To ffm.Count - 1
                aItems(iFF + 1) = ffm(iFF)
            Next
            Me.m_fpInflowForceNumberEnv.Items = aItems
        End Sub

#End Region ' Internals

     End Class

End Namespace
