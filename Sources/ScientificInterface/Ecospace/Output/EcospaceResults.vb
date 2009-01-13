'==============================================================================
'
' $Log: EcospaceResults.vb,v $
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

#End Region

Namespace Ecospace

    Public Class cFormEcospaceResults

        ' The core reference
        Private m_Core As cCore = Nothing

        ' Results grid
        Private m_GridGear As cGridEcospaceResultsGear = Nothing
        Private m_GridGroup As cGridEcospaceResultsGroup = Nothing
        Private m_GridRegion As cGridEcospaceResultsRegion = Nothing

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

        End Sub

        Private Sub EcospaceResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            PopulateResults()
            Me.MessageSources = New eMessageSource() {eMessageSource.EcoSpace}
        End Sub

        Private Sub frmEcospaceResults_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            Me.MessageSources = Nothing
        End Sub

        ''' <summary> Repopulates the variables on demand. </summary>
        Private Sub PopulateResults()
            rbGear.Checked = True

            txbBegin.Text = CStr(m_Core.EcospaceModelParameters.StartSummaryTime)
            txbEnd.Text = CStr(m_Core.EcospaceModelParameters.EndSummaryTime)

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

        Private Sub rbResults_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGear.CheckedChanged, rbRegion.CheckedChanged, rbGroup.CheckedChanged

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

            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex ' + 1
            m_GridGroup.RefreshContent()

        End Sub

        Private Sub cbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegions.SelectedIndexChanged

            m_GridRegion.SelRegionIndex = cbRegions.SelectedIndex ' + 1
            m_GridRegion.RefreshContent()

        End Sub

        Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub
    End Class

End Namespace

