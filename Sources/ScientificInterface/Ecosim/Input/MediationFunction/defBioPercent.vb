'==============================================================================
'
' $Log: defBioPercent.vb,v $
' Revision 1.2  2008/12/15 16:02:24  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/09/19 14:14:38  jeroens
' Fixed issue 496
'
' Revision 1.13  2007/11/15 15:04:26  jeroens
' * Fixed bug 339
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ZedGraph

#End Region

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class defBioPercent

#Region " Private helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class ListBoxItem

            ''' <summary></summary>
            Private m_obj As cCoreInputOutputBase = Nothing

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="obj"></param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal obj As cCoreInputOutputBase)
                Me.m_obj = obj
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Source() As cCoreInputOutputBase
                Get
                    Return Me.m_obj
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public Overrides Function ToString() As String
                Return Me.m_obj.Name()
            End Function

        End Class ' ListBoxItem

#End Region ' Private helper classes

#Region " Private variables "

        ''' <summary>Core ref.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The med function being edited.</summary>
        Private m_medfn As cMediationFunction = Nothing
        ''' <summary>Selected object.</summary>
        Private m_objSelected As cCoreInputOutputBase = Nothing

#End Region ' Private variables

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="medfn"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal medfn As cMediationFunction)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Sanity checks
            Debug.Assert(medfn IsNot Nothing)

            ' Store medfn
            Me.m_medfn = medfn
            ' Get the only core reference
            Me.m_core = cCore.GetInstance()

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub defBioPercent_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            For iGroup As Integer = 1 To m_core.nGroups

                Dim grp As cEcoPathGroupInput = m_core.EcoPathGroupInputs(iGroup)

                For j As Integer = 0 To m_medfn.CountGroup - 1
                    Dim medGrp As cMediatingGroup = m_medfn.Group(j)
                    If iGroup = medGrp.iGroupIndex Then
                        Me.Add(iGroup, medGrp.Weight)
                        Exit For
                    End If
                Next
            Next

            For iFleet As Integer = 1 To m_core.nFleets
                Dim iIndex As Integer = m_core.nGroups + iFleet
                Dim flt As cFleetInput = m_core.FleetInputs(iFleet)

                For j As Integer = 0 To m_medfn.CountFleet - 1
                    Dim medFlt As cMediatingFleet = m_medfn.Fleet(j)
                    If iFleet = medFlt.iFleetIndex Then
                        Me.Add(iIndex, medFlt.Weight)
                        Exit For
                    End If
                Next
            Next

            Me.UpdateAvailableGroupsAndFleets(m_core.EcoPathGroupInputs(1))
            Me.UpdateGraph()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOK.Click
            If Not Me.Apply() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnAdd.Click, m_lbAvailableGroupsFleets.DoubleClick
            Me.Add(Me.m_lbAvailableGroupsFleets.SelectedIndex)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btnRemove.Click
            Me.Remove(Me.m_grid.SelectedItem)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_lbAvailableGroupsFleets_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_lbAvailableGroupsFleets.SelectedIndexChanged
            Me.UpdateControls()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_grid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_grid_OnWeightChanged(ByVal obj As EwECore.cCoreInputOutputBase, ByVal sWeight As Single) Handles m_grid.OnWeightChanged
            Me.UpdateGraph()
        End Sub

#End Region ' Event handlers

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function Apply() As Boolean

            Dim dt As Dictionary(Of cCoreInputOutputBase, Single) = Me.m_grid.Data

            Me.m_medfn.Clear()

            For Each obj As cCoreInputOutputBase In dt.Keys
                If TypeOf obj Is cEcoPathGroupInput Then
                    Me.m_medfn.AddGroup(obj.Index, dt(obj))
                ElseIf TypeOf obj Is cFleetInput Then
                    Me.m_medfn.AddFleet(obj.Index, dt(obj))
                End If
            Next
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateAvailableGroupsAndFleets(ByVal objSelected As cCoreInputOutputBase)

            Dim obj As cCoreInputOutputBase = Nothing
            Dim iSelectedIndex As Integer = -1
            Dim iIndex As Integer = -1

            Me.m_lbAvailableGroupsFleets.Items.Clear()

            For iIndex = 1 To m_core.nGroups
                obj = m_core.EcoPathGroupInputs(iIndex)
                If (Me.m_grid.Find(obj) = False) Then
                    Me.m_lbAvailableGroupsFleets.Items.Add(New ListBoxItem(obj))
                    If Object.ReferenceEquals(obj, objSelected) Then iSelectedIndex = m_lbAvailableGroupsFleets.Items.Count - 1
                End If
            Next

            For iIndex = 1 To m_core.nFleets
                obj = m_core.FleetInputs(iIndex)
                If (Me.m_grid.Find(obj) = False) Then
                    Me.m_lbAvailableGroupsFleets.Items.Add(New ListBoxItem(obj))
                    If Object.ReferenceEquals(obj, objSelected) Then iSelectedIndex = m_lbAvailableGroupsFleets.Items.Count - 1
                End If
            Next

            Me.m_lbAvailableGroupsFleets.SelectedIndex = iSelectedIndex
            Me.UpdateControls()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()
            Me.m_btnAdd.Enabled = (Me.m_lbAvailableGroupsFleets.SelectedIndex > -1)
            Me.m_btnRemove.Enabled = (Me.m_grid.SelectedItem IsNot Nothing)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Add(ByVal iIndex As Integer, Optional ByVal sWeight As Single = 1.0)
            Dim lbi As ListBoxItem = DirectCast(Me.m_lbAvailableGroupsFleets.SelectedItem, ListBoxItem)
            If Me.m_grid.Add(lbi.Source, sWeight) Then
                Me.UpdateGraph()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Remove(ByVal obj As cCoreInputOutputBase)
            If Me.m_grid.Remove(obj) Then
                Me.UpdateGraph()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateGraph()
            Me.m_bp.LoadGraphData(Me.m_grid.Data)
        End Sub

#End Region ' Internals

    End Class

End Namespace

