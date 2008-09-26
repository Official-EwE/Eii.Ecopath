'==============================================================================
'
' $Log: EcospacePlots.vb,v $
' Revision 1.1  2008/09/26 07:32:01  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/18 01:12:46  sherman
' Drew plots but needs catch and effort over time.
'
' Revision 1.1  2008/07/15 18:44:39  sherman
' Created a plot functionality
'
'==============================================================================

#Region "Imports directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph

#End Region

Namespace Ecospace


    Public Class EcospacePlots

        Private m_core As cCore = cCore.GetInstance

        Private m_lbRegion As lbRegion
        Private m_lbDataType As lbDataType
        Private m_lbSpecFlt As lbGroupFleet

        Private m_lbHolder As New List(Of lbBase)

        Private Enum plotType
            Biomass
            Catches
            Effort
        End Enum

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()


            m_lbRegion = New lbRegion(lbRegionBox)
            m_lbDataType = New lbDataType(lbDataTypeBox)
            m_lbSpecFlt = New lbGroupFleet(lbSpecFltBox)

            ' Add any initialization after the InitializeComponent() call.
            m_lbHolder.Add(m_lbRegion)
            m_lbHolder.Add(m_lbDataType)
            m_lbHolder.Add(m_lbSpecFlt)

            For Each lb As lbBase In m_lbHolder
                AddHandler lb.SelectionChange, AddressOf SelectionChangeHandler
                lb.Populate()
            Next

        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()


            For Each lb As lbBase In m_lbHolder
                RemoveHandler lb.SelectionChange, AddressOf SelectionChangeHandler
                lb = Nothing
            Next
            m_lbHolder.Clear()
        End Sub

        Public Sub SelectionChangeHandler()
            ' Now plot something in Zedgraph

            ' Zedgraph pretty stuff
            Dim myPane As GraphPane = m_zgc.GraphPane()
            myPane.Legend.IsVisible = False
            myPane.XAxis.Scale.Max = m_core.nEcospaceTimeSteps / 12
            myPane.YAxis.Scale.Min = 0

            ' Datatype has changed so change Species Fleet
            If m_lbDataType.GetListBox.SelectedItem IsNot Nothing Then
                If m_lbDataType.GetListBox.SelectedItem.ToString = "Catch" Or _
                    m_lbDataType.GetListBox.SelectedItem.ToString = "Effort" Then
                    m_lbSpecFlt.isGroups = False
                Else
                    m_lbSpecFlt.isGroups = True
                End If
            End If

            If m_lbSpecFlt.m_listBox.SelectedIndex = 0 Then
                ' Set all but the first
                m_lbSpecFlt.m_listBox.SetSelected(0, False)

                For i As Integer = 1 To m_lbSpecFlt.m_listBox.Items.Count - 1
                    m_lbSpecFlt.m_listBox.SetSelected(i, True)
                Next
            End If

            If m_lbRegion.getSelectedIndices.Count > 0 And m_lbDataType.getSelectedIndices.Count > 0 And m_lbSpecFlt.getSelectedIndices.Count > 0 Then
                Select Case m_lbDataType.m_listBox.SelectedIndex
                    Case 0
                        PlotBiomass(myPane, plotType.Biomass)
                    Case 1
                        PlotBiomass(myPane, plotType.Catches)
                    Case 2
                        PlotBiomass(myPane, plotType.Effort)
                End Select

            End If


            ' If habitat Then

            ' if all habitat
            '   select datatype and plot all graphs
        End Sub

        Private Sub PlotBiomass(ByVal p_pane As GraphPane, ByVal type As plotType, Optional ByVal GraphIndex As Integer = -1)
            p_pane.CurveList.Clear()

            ' Plot biomass by habitat
            If GraphIndex = -1 Then
                For Each g As Integer In m_lbSpecFlt.getSelectedIndices()
                    graphSingleIndex(p_pane, type, g)
                Next g
            Else
                graphSingleIndex(p_pane, type, GraphIndex)
            End If

            m_zgc.AxisChange()
            m_zgc.Invalidate()
            ' Plot biomass by all habitat
        End Sub

        Private Sub graphSingleIndex(ByVal p_pane As GraphPane, ByVal type As plotType, ByVal groupIndex As Integer)
            Dim list As PointPairList = New PointPairList()
            For t As Integer = 0 To m_core.nEcospaceTimeSteps
                ' TODO_SL: VERY DANGEROUS HARD CODING OF TIME
                Select Case type
                    Case plotType.Biomass
                        list.Add(t / 12, m_core.EcospaceGroupOutput(groupIndex).RelativeBiomass(t))
                        'Case plotType.Catches
                        '    list.Add(t / 12, m_core.EcospaceFleetSummary(groupIndex).RelativeBiomass(t))
                        'Case plotType.Effort
                        '    list.Add(t / 12, m_core.EcospaceGroupOutput(groupIndex).RelativeBiomass(t))
                End Select
            Next t

            ' Do a select case here.
            Dim curve As LineItem = p_pane.AddCurve(m_core.EcoPathGroupInputs(groupIndex).Name, list, Color.Black, SymbolType.None)
        End Sub

    End Class



#Region " Helper Classes "

    '''==============================================================================
    ''' <summary>
    ''' Base class for Storage for
    ''' </summary>
    '''==============================================================================
    Public MustInherit Class lbBase

        Friend WithEvents m_listBox As ListBox
        Friend m_core As cCore = cCore.GetInstance()
        Public Event SelectionChange()

        Public Sub New(ByVal lb As ListBox)
            m_listBox = lb
        End Sub


        Public MustOverride Sub Populate()

        Public Overridable Sub SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles m_listBox.SelectedIndexChanged
            If m_listBox.SelectionMode = SelectionMode.MultiExtended And m_listBox.SelectedIndex = 0 Then
                ' Clear all others
                m_listBox.SelectedItem = -1
                m_listBox.SelectedItem = 0
            End If

            RaiseEvent SelectionChange()

        End Sub

        Public Overridable Function getSelectedIndices() As List(Of Integer)
            Dim listIntArr As New List(Of Integer)

            For Each selIndex As Integer In m_listBox.SelectedIndices
                listIntArr.Add(selIndex)
            Next
            Return listIntArr
        End Function

        Public Property GetListBox() As ListBox
            Get
                Return m_listBox
            End Get
            Set(ByVal value As ListBox)
                m_listBox = value
            End Set
        End Property
    End Class


    '''==============================================================================
    ''' <summary>
    ''' Storage for the Region values
    ''' </summary>
    '''==============================================================================
    Public Class lbRegion
        Inherits lbBase
        Public Sub New(ByVal lb As ListBox)
            MyBase.New(lb)

        End Sub

        Public Overrides Sub Populate()
            m_listBox.Items.Clear()

            ' Populate all the variables
            m_listBox.Items.Add("All (Individual regions disabled)")
            'For iRegion As Integer = 1 To m_core.nRegions - 1
            '    m_listBox.Items.Add(m_core.EcospaceRegions(iRegion).Name)
            'Next iRegion

            If Me.GetListBox.Items.Count = 1 Then Me.GetListBox.SelectedIndex = 0
        End Sub



    End Class


    '''==============================================================================
    ''' <summary>
    ''' Storage for the DataType values
    ''' </summary>
    '''==============================================================================
    Public Class lbDataType
        Inherits lbBase
        Public Sub New(ByVal lb As ListBox)
            MyBase.New(lb)

        End Sub

        Public Overrides Sub Populate()
            m_listBox.Items.Add("test")
            m_listBox.Items.Clear()

            ' Populate all the variables
            m_listBox.Items.Add("Biomass")
            m_listBox.Items.Add("Catch")
            m_listBox.Items.Add("Effort")

        End Sub
    End Class

    '''==============================================================================
    ''' <summary>
    ''' Storage for the Group or Fleet values
    ''' </summary>
    '''==============================================================================
    Public Class lbGroupFleet
        Inherits lbBase

        Private m_isGroups As Boolean = True

        Public Sub New(ByVal lb As ListBox)
            MyBase.New(lb)

        End Sub

        Public Overrides Sub Populate()
            m_listBox.Items.Clear()

            ' Populate all the variables
            m_listBox.Items.Add("All")

            If isGroups Then
                For i As Integer = 1 To m_core.nGroups - 1
                    m_listBox.Items.Add(m_core.EcospaceGroups(i).Name)
                Next
            Else    ' Fleets
                For i As Integer = 1 To m_core.nFleets - 1
                    m_listBox.Items.Add(m_core.EcospaceFleets(i).Name)
                Next
            End If

            If Me.GetListBox.Items.Count = 1 Then Me.GetListBox.SelectedIndex = 0

        End Sub

        Public Property isGroups() As Boolean
            Get
                Return m_isGroups
            End Get
            Set(ByVal value As Boolean)
                If value <> m_isGroups Then
                    m_isGroups = value
                    Me.Populate()
                End If

            End Set
        End Property


    End Class

#End Region ' Helper Classes

End Namespace
