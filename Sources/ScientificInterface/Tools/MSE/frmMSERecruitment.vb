#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports System.Windows.Forms
Imports ZedGraph
Imports EwECore.MSE

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form, implementing the Ecosim Recruitment interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmMSERecruitment

#Region " Internals "

        ''' <summary><see cref="cZedGraphHelper">Helper</see> to manipulate the graph.</summary>
        Private m_zgh As cZedGraphHelper = Nothing
        ''' <summary>Group selected in the form.</summary>
        Private m_group As cMSEGroupInput = Nothing

#End Region ' Internals

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
            Me.m_zgh.ConfigurePane("", My.Resources.HEADER_BIOMASS, My.Resources.HEADER_TFM, True)

            Me.m_zgh.AllowZoom = False
            Me.m_zgh.AllowPan = False
            Me.m_zgh.AllowEdit = True

            Me.m_grid.UIContext = Me.UIContext
            If (Core.nGroups > 0) Then
                Me.m_grid.Group = Me.Core.MSEManager.GroupInputs(1)
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            If Me.m_zgh IsNot Nothing Then
                Me.Group = Nothing
                Me.m_zgh.Detach()
                Me.m_zgh = Nothing
            End If

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub HandleGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            ' Update group selection according to user actions in the grid
            Me.Group = Me.m_grid.Group
        End Sub

        Private Sub HandlePropertyChanged(ByVal prop As cProperty, ByVal cf As cProperty.eChangeFlags)
            ' A relevant property has changed: redraw the graph
            Me.Redraw()
        End Sub

        Private Sub tsbtDefaults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbtDefaults.Click
            Try
                Me.Core.SetDefaultMSERecruitment()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group in the form
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property Group() As cMSEGroupInput
            Get
                Return Nothing 'Me.m_group
            End Get
            Set(ByVal value As cMSEGroupInput)

                Dim pm As cPropertyManager = Me.PropertyManager

                ' Unregister
                If (Me.m_group IsNot Nothing) Then
                    RemoveHandler pm.GetProperty(Me.m_group, eVarNameFlags.RHalfB0Ratio).PropertyChanged, AddressOf HandlePropertyChanged
                    RemoveHandler pm.GetProperty(Me.m_group, eVarNameFlags.MSEForcastGain).PropertyChanged, AddressOf HandlePropertyChanged
                End If

                ' Update
                Me.m_group = value

                ' Register
                If (Me.m_group IsNot Nothing) Then
                    AddHandler pm.GetProperty(Me.m_group, eVarNameFlags.RHalfB0Ratio).PropertyChanged, AddressOf HandlePropertyChanged
                    AddHandler pm.GetProperty(Me.m_group, eVarNameFlags.MSEForcastGain).PropertyChanged, AddressOf HandlePropertyChanged
                End If

                ' Redraw the glaph
                Me.Redraw()

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the points to render in the graph.
        ''' </summary>
        ''' <returns>An array of points to render in the graph.</returns>
        ''' <remarks>
        ''' Please, PLEASE change this code to something more meaningful!!!
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function GetGraphValues() As Single()
            If (Me.m_group Is Nothing) Then
                Return New Single() {}
            Else
                Return New Single() {Me.m_group.RHalfB0Ratio, Me.m_group.ForcastGain}
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Redraw the quota curve.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Redraw()

            If Me.m_zgh Is Nothing Then Return

            Dim lpts As New PointPairList
            Dim lLines As New List(Of LineItem)
            Dim values As Single() = Me.GetGraphValues

            If (Me.m_group IsNot Nothing) Then
                ' Group has data?
                If (Me.m_group.GetStatus(eVarNameFlags.MSEForcastGain) And eStatusFlags.Null) = 0 Then
                    ' #Yes: plot data
                    For i As Integer = 0 To values.Count - 1
                        lpts.Add(i, values(i))
                    Next
                    lLines.Add(Me.m_zgh.CreateLineItem(Me.Core.EcoPathGroupInputs(Me.m_group.Index), lpts))
                End If
            End If

            If lLines.Count > 0 Then
                Me.m_zgh.PlotLines(lLines.ToArray, 1, True)
                Me.m_graph.Cursor = Cursors.Default
            Else
                ' Clear graph
                Me.m_zgh.PlotLines(Nothing)
                Me.m_graph.Cursor = Cursors.No
            End If

        End Sub

#End Region ' Internals

    End Class

End Namespace
