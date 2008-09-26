'=============================================================================
'
' $Log: EditMultiStanza.vb,v $
' Revision 1.1  2008/09/26 07:31:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.40  2008/09/02 14:47:28  jeroens
' Simplified ZedGraphHelper wrap interface
'
' Revision 1.39  2008/08/02 03:04:17  jeroens
' Renamed resources
'
' Revision 1.38  2008/07/18 17:52:15  jeroens
' Changed dialog title
'
' Revision 1.37  2008/06/02 00:01:38  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.36  2008/04/07 17:57:19  jeroens
' Every ZGH extends the context menu
'
' Revision 1.35  2008/04/07 02:31:17  jeroens
' Cleaning up resources
'
' Revision 1.34  2008/04/06 03:50:08  jeroens
' Grid no longer auto-docks; needs to be done here
'
' Revision 1.33  2008/04/02 02:14:39  jeroens
' Fixed bug 439
'
' Revision 1.32  2007/12/22 22:36:06  jeroens
' * Uses ZedgraphHelper, StyleGuide to style graph
'
' Revision 1.31  2007/12/22 16:49:22  jeroens
' * Uses ZedGraphHelper
'
' Revision 1.30  2007/11/24 16:47:04  jeroens
' * Fixed compiler warnings
'
' Revision 1.29  2007/11/21 20:17:07  sherman
' Added Export to CSV to zedgraph.
'
' Revision 1.28  2007/09/10 18:08:15  jeroens
' + Added option to apply grid values to core
'
' Revision 1.27  2007/08/07 21:24:06  joeb
' Put CreateGraph back to its original state it now normalizes all the data (again)
'
' Revision 1.26  2007/08/06 19:12:41  joeb
' Changed CreateGraph() to match EwE5 output this was bug 0000119. This seems wrong to me but it matches EwE5 output.
'
' Revision 1.25  2007/07/08 07:35:46  jeroens
' * Localized
'
' Revision 1.24  2007/06/29 23:18:40  joeh
' Add hard coded strings to resource file
' Make form re-sizable
'
' Revision 1.23  2007/06/27 23:41:46  jeroens
' + NEEDS LOCALIZING
'
' Revision 1.22  2007/05/20 01:02:11  jeroens
' + [OK] click handler will save the current group info before exiting
'
' Revision 1.21  2007/05/19 03:40:13  jeroens
' * Fixed major bug: formatproviders kept being recreated
'
' Revision 1.20  2007/05/18 21:17:25  joeb
' Temp bug fix to update WmatWinf
'
' Revision 1.19  2007/04/26 17:29:34  joeh
' *Add "Namespace Ecopath"
'
' Revision 1.18  2007/04/19 00:39:47  joeh
' *Make changes due to ZedGraph is upgraded to v.5
'
' Revision 1.17  2007/04/18 23:25:48  fgao
' .Net 2.0 Version ZedGraph change
'
' Revision 1.16  2007/04/18 01:07:11  joeh
' *Fine tune EditMultiStanza UI
'
' Revision 1.15  2007/04/17 01:12:35  joeh
' *Fine tune Edit Multi Stanza
'
' Revision 1.14  2007/04/13 01:00:23  joeh
' *Implement combo box for Forcing Function
'
' Revision 1.13  2007/04/12 01:00:46  joeh
' *Implement combo box for Name of Species
'
' Revision 1.12  2007/04/11 21:50:46  joeh
' *Implement combobox for Name of Species
'
' Revision 1.11  2007/04/11 21:13:05  jeroens
' * FormatProvider values passed back correctly
'
' Revision 1.10  2007/04/11 17:20:01  jeroens
' * Replaced EwETextBox by EwEFormatProvider
'
' Revision 1.9  2007/04/06 01:02:23  joeh
' *Implement Calculate button functionality
'
' Revision 1.8  2007/04/03 23:19:00  joeh
' *Implement the graphics of Number, Weight and Biomass
'
' Revision 1.7  2007/04/02 17:56:58  joeh
' *First shot at graphing in Edit Multi Stanza
'
' Revision 1.6  2007/03/30 13:36:00  jeroens
' * Datatypes on textboxes will preserve EwE formatting
'
'=============================================================================

#Region "Imports directive"
Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports System.Windows.Forms
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Namespace Ecopath

    Public Class EditMultiStanza

#Region "Private variables"
        Private m_MultiStanzaGrid As EditMultiStanzaEwEGrid = Nothing
        Private m_Core As cCore = Nothing
        Private m_fpK As cEwEFormatProvider = Nothing
        Private m_fpRecPwr As cEwEFormatProvider = Nothing
        Private m_fpBab As cEwEFormatProvider = Nothing
        Private m_fpWmatWinf As cEwEFormatProvider = Nothing
        'Private m_fpFF As EwEFormatProvider = Nothing
        Private m_zgh As ZedGraphHelper
#End Region

#Region "Constructors"

        Public Sub New(Optional ByVal objStanzaClicked As cEcoPathGroupInput = Nothing)
            InitializeComponent()

            Me.m_Core = cCore.GetInstance()
            Me.m_zgh = New ZedGraphHelper(Me.m_zgc)

            Me.m_MultiStanzaGrid = New EditMultiStanzaEwEGrid(objStanzaClicked)
        End Sub
#End Region

#Region "Event handlers "

        Private Sub EditMultiStanza_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Create format providers once during load
            Me.m_fpK = New cEwEFormatProvider(Me.txtK, GetType(Single))
            Me.m_fpRecPwr = New cEwEFormatProvider(Me.txtRecPwr, GetType(Single))
            Me.m_fpBab = New cEwEFormatProvider(Me.txtBAB, GetType(Single))
            Me.m_fpWmatWinf = New cEwEFormatProvider(Me.txtWmatWinf, GetType(Single))

            LoadEditMultiStanza()

        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            m_MultiStanzaGrid.ResetStanzaGroupValues()

            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub btnCalculate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalculate.Click

            Me.SaveChanges(False)

            m_MultiStanzaGrid.CalculateStanzaParametrs()
            m_MultiStanzaGrid.RefreshMultiStanzaGrid()
            m_MultiStanzaGrid.RefreshGraphData()
            CreateGraph(m_zgc)

        End Sub

        Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click

            Me.SaveChanges(True)

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub
#End Region

        Private Sub CreateGraph(ByVal zgc As ZedGraphControl)

            Dim myPane As GraphPane = Me.m_zgh.ConfigurePane("", _
                My.Resources.HEADER_AGE, 0, Me.m_MultiStanzaGrid.MaxAge - 1, _
                My.Resources.HEADER_NORMALIZED_VALUE, 0, 1, _
                True)

            Dim list1 As New PointPairList()
            Dim list2 As New PointPairList()
            Dim list3 As New PointPairList()
            Dim list() As PointPairList

            Dim MaxNumber As Single = 0.0
            Dim MaxWeight As Single = 0.0
            Dim MaxBiomass As Single = 0.0

            'don't show the last value
            For intIndex As Integer = 1 To m_MultiStanzaGrid.MaxAge - 1
                If m_MultiStanzaGrid.NumberAtAge(intIndex) > MaxNumber Then MaxNumber = _
                   m_MultiStanzaGrid.NumberAtAge(intIndex)
                If m_MultiStanzaGrid.WeightAtAge(intIndex) > MaxWeight Then MaxWeight = _
                   m_MultiStanzaGrid.WeightAtAge(intIndex)
                If m_MultiStanzaGrid.BiomassAtAge(intIndex) > MaxBiomass Then MaxBiomass = _
                   m_MultiStanzaGrid.BiomassAtAge(intIndex)
            Next

            If MaxNumber = 0 Then MaxNumber = 1
            If MaxWeight = 0 Then MaxWeight = 1
            If MaxBiomass = 0 Then MaxBiomass = 1

            zgc.GraphPane.CurveList.Clear()
            For intIndex As Integer = 1 To m_MultiStanzaGrid.MaxAge - 1
                list1.Add(intIndex - 1, m_MultiStanzaGrid.NumberAtAge(intIndex) / MaxNumber)
                list2.Add(intIndex - 1, m_MultiStanzaGrid.WeightAtAge(intIndex) / MaxWeight)
                list3.Add(intIndex - 1, m_MultiStanzaGrid.BiomassAtAge(intIndex) / MaxBiomass)
            Next intIndex

            ' Generate a red curve 
            Dim myCurve1 As LineItem = myPane.AddCurve(My.Resources.HEADER_NUMBER, _
               list1, Color.Red, SymbolType.None)

            ' Generate a blue curve 
            Dim myCurve2 As LineItem = myPane.AddCurve(My.Resources.HEADER_INDIVIDUAL_WEIGHT, _
               list2, Color.Blue, SymbolType.None)

            ' Generate a black curve 
            Dim myCurve3 As LineItem = myPane.AddCurve(My.Resources.HEADER_POPULATIONBIOMASS, _
               list3, Color.Black, SymbolType.None)

            ReDim list(m_MultiStanzaGrid.NStanza)
            list(1) = New PointPairList
            list(1).Add(m_MultiStanzaGrid.StartAge(1), 0)
            list(1).Add(m_MultiStanzaGrid.StartAge(1), 1)
            myPane.AddCurve(My.Resources.ECOPATH_GRAPH_LEGEND_STANZA_SEP, _
                list(1), Color.Green, SymbolType.None)
            For intIndex As Integer = 2 To m_MultiStanzaGrid.NStanza
                list(intIndex) = New PointPairList
                list(intIndex).Add(m_MultiStanzaGrid.StartAge(intIndex), 0)
                list(intIndex).Add(m_MultiStanzaGrid.StartAge(intIndex), 1)
                myPane.AddCurve("", _
                    list(intIndex), Color.Green, SymbolType.None)
            Next

            ' Calculate the Axis Scale Ranges
            zgc.AxisChange()

            zgc.Refresh()
        End Sub

        Private Sub LoadEditMultiStanza()
            Dim bEcosimLoaded As Boolean = Me.m_Core.StateMonitor.HasEcosimLoaded()

            plMultiStanzaGrid.Controls.Add(m_MultiStanzaGrid)
            m_MultiStanzaGrid.Dock = DockStyle.Fill

            'txtNamSpc.Text = m_MultiStanzaGrid.StanzaGroupName
            cmbSpeciesName.DropDownStyle = ComboBoxStyle.DropDownList
            cmbSpeciesName.Items.Clear()
            For iIndex As Integer = 0 To m_MultiStanzaGrid.NStanzaGroup - 1
                cmbSpeciesName.Items.Add(m_MultiStanzaGrid.StanzaGroupName(iIndex))
            Next
            'cmbSpeciesName.Sorted = True
            cmbSpeciesName.Text = m_MultiStanzaGrid.ClickedStanzaGroupName

            Me.m_fpK.Value = m_MultiStanzaGrid.CurvParam
            Me.m_fpRecPwr.Value = m_MultiStanzaGrid.RecruitPower
            Me.m_fpBab.Value = m_MultiStanzaGrid.RelBiomassAccumRate
            Me.m_fpWmatWinf.Value = m_MultiStanzaGrid.WmatWinf

            'Me.m_fpFF = New Controls.EwEFormatProvider(Me
            ' Create format providers ONLY ONCE.txtFF, GetType(String))
            'If (bEcosimLoaded) Then
            '    Me.m_fpFF.Value = m_MultiStanzaGrid.ClickedForcingFunctName
            '    Me.m_fpFF.Style = StyleGuide.StyleFlags.OK
            'Else
            '    Me.m_fpFF.Value = ""
            '    Me.m_fpFF.Style = StyleGuide.StyleFlags.NotEditable
            'End If

            If bEcosimLoaded Then
                cmbFF.DropDownStyle = ComboBoxStyle.DropDownList
                cmbFF.Items.Clear()
                For iIndex As Integer = 0 To m_MultiStanzaGrid.NForcingFunction - 1
                    cmbFF.Items.Add(m_MultiStanzaGrid.ForcingFunctionName(iIndex))
                Next
                cmbFF.Items.Add(My.Resources.GENERIC_VALUE_NONE)
                cmbFF.Sorted = True
                cmbFF.Text = My.Resources.GENERIC_VALUE_NONE
            Else
                cmbFF.Text = My.Resources.ECOSIM_PROMPT_SCENARIO_REQUIRED
                cmbFF.Enabled = False
            End If

            chkFFecun.Checked = m_MultiStanzaGrid.FixedFecundity

            m_MultiStanzaGrid.CalculateStanzaParametrs()
            m_MultiStanzaGrid.RefreshMultiStanzaGrid()
            m_MultiStanzaGrid.RefreshGraphData()

            CreateGraph(m_zgc)

        End Sub

        Private Sub cmbSpeciesName_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbSpeciesName.SelectionChangeCommitted

            Me.SaveChanges(False)

            'm_MultiStanzaGrid.SelectedStanzaGroupName = cmbSpeciesName.SelectedItem.ToString
            m_MultiStanzaGrid.ClickedStanzaGroupName = cmbSpeciesName.SelectedItem.ToString
            m_MultiStanzaGrid.DetermineClickedStanzaGroupIndex()
            'FilledData() in EditMultiStanzaGrid
            m_MultiStanzaGrid.DetermineClickedStanzaGroup()
            'EditMultiStanza_Load  
            LoadEditMultiStanza()
        End Sub

        Private Sub SaveChanges(ByVal bApplyToCore As Boolean)
            ' Check ecosim status
            Dim bEcosimLoaded As Boolean = Me.m_Core.StateMonitor.HasEcosimLoaded()

            'm_MultiStanzaGrid.ClickedStanzaGroupName = cmbSpeciesName.Text
            m_MultiStanzaGrid.CurvParam = CSng(Me.m_fpK.Value)
            m_MultiStanzaGrid.RecruitPower = CSng(Me.m_fpRecPwr.Value)
            m_MultiStanzaGrid.RelBiomassAccumRate = CSng(Me.m_fpBab.Value)
            m_MultiStanzaGrid.WmatWinf = CSng(Me.m_fpWmatWinf.Value)
            If (bEcosimLoaded) Then
                m_MultiStanzaGrid.ClickedForcingFunctName = cmbFF.Text
            Else
                m_MultiStanzaGrid.ClickedForcingFunctName = ""
            End If
            m_MultiStanzaGrid.DetermineClickedForcingFunctionNumber()
            m_MultiStanzaGrid.FixedFecundity = chkFFecun.Checked
            m_MultiStanzaGrid.SetStanzaGroupValues(bApplyToCore)
        End Sub

    End Class

End Namespace
