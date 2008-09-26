'==============================================================================
'
' $Log: EcosimResults.vb,v $
' Revision 1.1  2008/09/26 07:31:46  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.13  2008/08/10 01:43:07  jeroens
' Renamed PropertyFormatProvider
'
' Revision 1.12  2008/07/16 16:19:29  jeroens
' Cleaned-up
'
' Revision 1.11  2008/06/02 00:01:32  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.10  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.9  2008/03/06 01:59:55  jeroens
' Fixed refresh issue
' Removed close button
'
' Revision 1.8  2007/09/20 18:54:16  joeb
' Bug Fixes
'
' Revision 1.7  2007/09/20 16:06:40  joeb
' Summary time period fixes
'
' Revision 1.6  2007/09/19 22:15:18  joeb
' Added Summary data
'
' Revision 1.5  2007/08/10 02:12:20  jeroens
' + Flagged localizables
'
'==============================================================================

#Region " Imports Directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region ' Imports Directive

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

        Private Sub EcosimResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            rbGear.Checked = True

            'summary
            Me.m_fpStartSum = New cPropertyFormatProvider(Me.txtSumStart, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumStart)
            Me.m_fpEndSum = New cPropertyFormatProvider(Me.txtSumEnd, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumEnd)
            Me.m_fpNumSteps = New cPropertyFormatProvider(Me.udNumTimeSteps, m_Core.EcoSimModelParameters, eVarNameFlags.EcosimSumNTimeSteps)

            cbGears.Items.Clear()
            Dim efo As cEcosimFleetSummary = Nothing
            For i As Integer = 0 To m_Core.nFleets 'includes the 'combined fleets' object
                efo = m_Core.EcosimFleetSummaries(i)
                cbGears.Items.Add(efo.Name)
            Next
            cbGears.SelectedIndex = 0

            curDisplayMode = eDisplayMode.Fleets
            UpdateControls()

            Me.MessageSources = New eMessageSource() {eMessageSource.EcoSim}
        End Sub

        Private Sub EcosimResults_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            Me.MessageSources = Nothing
        End Sub

        Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbGears.SelectedIndexChanged
            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex
        End Sub

        Private Sub rbGear_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGear.CheckedChanged
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
