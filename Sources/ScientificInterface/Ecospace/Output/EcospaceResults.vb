'==============================================================================
'
' $Log: EcospaceResults.vb,v $
' Revision 1.11  2009/03/19 16:02:27  jeroens
' Added FormatProvider.Release
'
' Revision 1.10  2009/02/05 17:48:39  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.9  2009/01/16 18:37:09  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.8  2009/01/16 17:17:50  joeb
' Removed unused variable
'
' Revision 1.7  2009/01/15 22:39:56  joeb
' Moved Ecospace start and end summary periods from Parameters form to Results form
'
' Revision 1.6  2009/01/13 21:32:47  joeb
' Still making changes from merge of Summary into Output objects
'
' Revision 1.5  2009/01/13 21:18:17  joeb
' Merged Ecospace summary objects into Ecospace Output objects
'
' Revision 1.4  2009/01/12 22:59:49  joeb
' Fixed bugs 574 and 569
'
' Revision 1.3  2008/12/15 15:52:26  jeroens
' no message
'
' Revision 1.2  2008/11/20 15:19:24  jeroens
' Renamed classes
'
' Revision 1.1  2008/09/26 07:32:01  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/06/25 00:26:22  sherman
' Moved Ecospace Results into Node... still has refresh issues
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    Public Class cFormEcospaceResults

        ' The core reference
        Private m_Core As cCore = Nothing

        ' Results grid
        Private m_GridGear As cGridEcospaceResultsGear = Nothing
        Private m_GridGroup As cGridEcospaceResultsGroup = Nothing
        Private m_GridRegion As cGridEcospaceResultsRegion = Nothing

        ' Summary
        Private m_fpSumStartTime As cEwEFormatProvider = Nothing
        Private m_fpSumEndTime As cEwEFormatProvider = Nothing
        Private m_fpSumLength As cEwEFormatProvider = Nothing


        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ' Get the only core reference
            m_Core = cCore.GetInstance()

            'Initialize the results grid
            m_GridGear = New cGridEcospaceResultsGear
            m_GridGroup = New cGridEcospaceResultsGroup
            m_GridRegion = New cGridEcospaceResultsRegion

            ' Add the result grids. 
            plResultsGrid.Controls.Add(m_GridGear)
            plResultsGrid.Controls.Add(m_GridGroup)
            plResultsGrid.Controls.Add(m_GridRegion)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

        End Sub

        Private Sub cFormEcospaceResults_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed
            Me.m_fpSumStartTime.Release()
            Me.m_fpSumEndTime.Release()
            Me.m_fpSumLength.Release()
        End Sub

        Private Sub EcospaceResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles MyBase.Load

            Dim ecospaceModelParams As cEcospaceModelParameters = Me.m_Core.EcospaceModelParameters()

            Me.m_fpSumStartTime = New cPropertyFormatProvider(Me.tbSumStartTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeStart)
            Me.m_fpSumEndTime = New cPropertyFormatProvider(Me.tbSumEndTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeEnd)
            Me.m_fpSumLength = New cPropertyFormatProvider(Me.udSumLength, ecospaceModelParams, eVarNameFlags.EcospaceNumberSummaryTimeSteps)

            PopulateResults()
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
        End Sub

        Private Sub frmEcospaceResults_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            Me.CoreComponents = Nothing
        End Sub

        ''' <summary> Repopulates the variables on demand. </summary>
        Private Sub PopulateResults()
            rbGear.Checked = True

            cbGears.Items.Clear()

            Dim efo As cEcospaceFleetOutput = Nothing
            For i As Integer = 0 To m_Core.nFleets
                efo = m_Core.EcospaceFleetOutput(i)
                cbGears.Items.Add(efo.Name)
            Next
            cbGears.SelectedIndex = 0

            cbRegions.Items.Clear()
            Dim ero As cEcospaceRegionOutput = Nothing
            For i As Integer = 0 To m_Core.nRegions
                ero = m_Core.EcospaceRegionOutput(i)
                cbRegions.Items.Add(ero.Name)
            Next
            cbRegions.SelectedIndex = 0

        End Sub

        Private Sub rbResults_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGear.CheckedChanged, rbGroup.CheckedChanged, rbRegion.CheckedChanged

            If rbGear.Checked Then
                'Display gear results
                m_GridGear.Visible = True : m_GridRegion.Visible = False : m_GridGroup.Visible = False
                Me.cbGears.Enabled = False
                Me.cbRegions.Enabled = False

            ElseIf rbGroup.Checked Then
                'Display group results
                m_GridGear.Visible = False : m_GridRegion.Visible = False : m_GridGroup.Visible = True
                Me.cbGears.Enabled = True
                Me.cbRegions.Enabled = False

            ElseIf rbRegion.Checked Then
                'Display region results
                m_GridGear.Visible = False : m_GridRegion.Visible = True : m_GridGroup.Visible = False
                Me.cbGears.Enabled = False
                Me.cbRegions.Enabled = True
            End If

        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbGears.SelectedIndexChanged
            'fleets are zero based so the zero index is ok
            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex
            m_GridGroup.RefreshContent()

        End Sub

        Private Sub cbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegions.SelectedIndexChanged

            'regions are zero based so the zero index is ok
            m_GridRegion.SelRegionIndex = cbRegions.SelectedIndex
            m_GridRegion.RefreshContent()

        End Sub

        'Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '    Me.DialogResult = System.Windows.Forms.DialogResult.OK
        '    Me.Close()
        'End Sub

        ''' <summary>
        ''' Message handler for core Ecosim Datachanged message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>This updates the grids with the results if the user changed the time periods</remarks>
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If msg.DataType = eDataTypes.EcospaceModelParameter Then
                For Each var As cVariableStatus In msg.Variables
                    If var.VarName = eVarNameFlags.EcospaceSummaryTimeStart Or var.VarName = eVarNameFlags.EcospaceSummaryTimeEnd Or var.VarName = eVarNameFlags.EcospaceNumberSummaryTimeSteps Then

                        If m_GridGroup.Visible Then m_GridGroup.RefreshContent()
                        If m_GridRegion.Visible Then m_GridRegion.RefreshContent()
                        If m_GridGear.Visible Then m_GridGear.RefreshContent()

                        Exit Sub
                    End If
                Next
            End If
            MyBase.OnCoreMessage(msg)
        End Sub

    End Class

End Namespace

