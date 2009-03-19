'==============================================================================
'
' $Log: EcosimResults.vb,v $
' Revision 1.6  2009/03/19 16:02:26  jeroens
' Added FormatProvider.Release
'
' Revision 1.5  2009/02/05 17:48:37  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.4  2009/01/16 18:30:38  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2009/01/13 18:00:47  joeb
' Replace Ecosim summary objects with Ecosim Ouput objects all output data now in Fleet or Group objects
'
' Revision 1.2  2008/12/15 15:53:26  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:46  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecosim

    Public Class EcosimResults

#Region " Private variables "

        Private Enum eDisplayMode As Byte
            Groups = 0
            Fleets
        End Enum

        Private curDisplayMode As eDisplayMode

        'The core reference
        Private m_Core As cCore

        'Results grid
        Private m_GridGear As EcosimResultsGridFleet
        Private m_GridGroup As EcosimResultsGridGroup

        'format provides 
        Private m_fpStartSum As cEwEFormatProvider = Nothing
        Private m_fpEndSum As cEwEFormatProvider = Nothing
        Private m_fpNumSteps As cEwEFormatProvider = Nothing

        'core message handler
        Private m_coreMessageHandler As cMessageHandler

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()

            InitializeComponent()

            m_Core = cCore.GetInstance()

            'Initialize the results grid
            m_GridGear = New EcosimResultsGridFleet
            m_GridGroup = New EcosimResultsGridGroup

            ' Add the result grids. 
            plResultsGrid.Controls.Add(m_GridGear)
            plResultsGrid.Controls.Add(m_GridGroup)

        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub EcosimResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles MyBase.Load

            rbGear.Checked = True

            'summary
            Me.m_fpStartSum = New cPropertyFormatProvider(Me.txtSumStart, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumStart)
            Me.m_fpEndSum = New cPropertyFormatProvider(Me.txtSumEnd, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumEnd)
            Me.m_fpNumSteps = New cPropertyFormatProvider(Me.udNumTimeSteps, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumNTimeSteps)

            cbGears.Items.Clear()
            Dim efo As cEcosimFleetOutput = Nothing
            For i As Integer = 0 To m_Core.nFleets 'includes the 'combined fleets' object
                efo = m_Core.EcosimFleetOutput(i)
                cbGears.Items.Add(efo.Name)
            Next
            cbGears.SelectedIndex = 0

            curDisplayMode = eDisplayMode.Fleets
            UpdateControls()

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim}
        End Sub

        Private Sub EcosimResults_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
            Handles Me.FormClosing

            Me.m_fpEndSum.Release()
            Me.m_fpNumSteps.Release()
            Me.m_fpStartSum.Release()

            Me.CoreComponents = Nothing
        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles cbGears.SelectedIndexChanged
            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex
        End Sub

        Private Sub rbGear_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles rbGear.CheckedChanged
            If rbGear.Checked Then
                curDisplayMode = eDisplayMode.Fleets
            End If
            UpdateControls()
        End Sub

        Private Sub rbGroup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGroup.CheckedChanged
            If rbGroup.Checked Then
                curDisplayMode = eDisplayMode.Groups
            End If
            UpdateControls()
        End Sub

#End Region ' Events

#Region " Private stuff "

        ''' <summary>
        ''' Message handler for core Ecosim Datachanged message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>This updates the grids with the results if the user changed the time periods</remarks>
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If msg.DataType = eDataTypes.EcoSimModelParameter Then
                For Each var As cVariableStatus In msg.Variables
                    If var.VarName = eVarNameFlags.EcosimSumEnd Or var.VarName = eVarNameFlags.EcosimSumStart Or var.VarName = eVarNameFlags.EcosimSumNTimeSteps Then
                        m_GridGroup.UpdateData()
                        m_GridGear.updateData()
                        Exit Sub
                    End If
                Next
            End If
            MyBase.OnCoreMessage(msg)
        End Sub

        Private Sub UpdateControls()

            If curDisplayMode = eDisplayMode.Fleets Then
                cbGears.Enabled = False
                m_GridGroup.Visible = False
                m_GridGear.Visible = True
            ElseIf curDisplayMode = eDisplayMode.Groups Then
                cbGears.Enabled = True
                m_GridGear.Visible = False
                m_GridGroup.Visible = True
            End If

        End Sub

#End Region ' Private stuff

    End Class

End Namespace
