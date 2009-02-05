'==============================================================================
'
' $Log: frmEcotracerInput.vb,v $
' Revision 1.3  2009/02/05 17:48:40  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.2  2009/01/16 18:30:39  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/08/10 01:43:08  jeroens
' Renamed PropertyFormatProvider
'
' Revision 1.8  2008/08/02 03:04:16  jeroens
' Renamed resources
'
' Revision 1.7  2008/06/02 00:01:35  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.6  2008/05/29 22:22:55  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.5  2008/04/07 02:31:15  jeroens
' Cleaning up resources
'
' Revision 1.4  2008/03/18 16:51:21  joeb
' Fixed bug CZero was not updating
'
' Revision 1.3  2008/01/08 11:24:15  jeroens
' Merged input parms and group grid in one screen
'
'==============================================================================

Option Strict On
Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

Namespace Ecotracer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmEcotracerInput

#Region " Private vars "

        Private m_core As cCore = Nothing
        Private m_fpCZeroEnv As cEwEFormatProvider = Nothing
        Private m_fpCInflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCOutflowEnv As cEwEFormatProvider = Nothing
        Private m_fpCDecayEnv As cEwEFormatProvider = Nothing
        Private m_fpInflowForceNumberEnv As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            Me.New("")
        End Sub

        Public Sub New(ByVal strText As String)
            MyBase.New(strText, New EcotracerInputGrid)
            Me.m_core = cCore.GetInstance()
            InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Events "

        Private Sub frmEcotracerInput_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim ecotracerModelParams As cEcotracerModelParameters = m_core.EcotracerModelParameters()

            Me.m_fpInflowForceNumberEnv = New cPropertyFormatProvider(Me.cmbEnvInflowFF, ecotracerModelParams, eVarNameFlags.ConForceNumber)
            Me.m_fpCZeroEnv = New cPropertyFormatProvider(Me.m_tbCZeroEnv, ecotracerModelParams, eVarNameFlags.CZero)
            Me.m_fpCDecayEnv = New cPropertyFormatProvider(Me.m_tbCDecayRateEnv, ecotracerModelParams, eVarNameFlags.CDecay)
            Me.m_fpCInflowEnv = New cPropertyFormatProvider(Me.m_tbCInflowEnv, ecotracerModelParams, eVarNameFlags.CInflow)
            Me.m_fpCOutflowEnv = New cPropertyFormatProvider(Me.m_tbCLossEnv, ecotracerModelParams, eVarNameFlags.COutflow)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
            Me.UpdateFFFormatProviders()

            Me.m_plGrid.Controls.Add(Me.Grid)

        End Sub

        Private Sub frmEcotracerInput_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.CoreComponents = Nothing
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Me.UpdateFFFormatProviders()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdateFFFormatProviders()
            ' Assemble list of FFs
            Dim ffm As cForcingFunctionManager = Me.m_core.ForcingShapeManager()
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
