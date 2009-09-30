#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorFleet

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Private Sub OnFleetSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbFleet.SelectedIndexChanged
            Me.Editor.Fleet = Me.m_cmbFleet.SelectedIndex
        End Sub

        Private Sub DoLoad(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

            ' Initialize group combo 
            Dim core As cCore = cCore.GetInstance()
            Dim fleet As cFleetInput = Nothing

            Me.m_cmbFleet.Items.Clear()

            ' ToDo: this control will not respond to dynamic fleet name changes
            Me.m_cmbFleet.Items.Add(My.Resources.GENERIC_VALUE_ALL)
            For iGroup As Integer = 1 To core.nFleets
                fleet = core.FleetInputs(iGroup)
                Me.m_cmbFleet.Items.Add(fleet.Name)
            Next iGroup

            Me.UpdateControls()
        End Sub

        Public Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            If (Me.m_cmbFleet Is Nothing) Then Return

            Try
                Me.m_cmbFleet.SelectedIndex = CInt(Me.Editor.Fleet)
            Catch ex As Exception

            End Try

        End Sub

        Protected Overloads Property Editor() As cLayerEditorFleet
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorFleet)
            End Get
            Set(ByVal editor As cLayerEditorFleet)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorFleet, "ucLayerEditorFleet connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 0
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

    End Class

End Namespace

