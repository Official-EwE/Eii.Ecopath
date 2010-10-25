#Region " Imports "

Option Strict On
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorFleet

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim fleet As cFleetInput = Nothing

            Me.m_cmbFleet.Items.Clear()

            ' ToDo: this control will not respond to dynamic fleet name changes
            Me.m_cmbFleet.Items.Add(SharedResources.GENERIC_VALUE_ALL)
            For iGroup As Integer = 1 To core.nFleets
                fleet = core.FleetInputs(iGroup)
                Me.m_cmbFleet.Items.Add(fleet.Name)
            Next iGroup

            ' Update control
            Me.m_cmbFleet.SelectedIndex = Me.FleetIndex

        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            Me.m_cmbFleet.Enabled = Me.IsAttached

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

        Protected Property FleetIndex() As Integer
            Get
                If (Not Me.IsAttached) Then Return cCore.NULL_VALUE
                Return Me.Editor.Fleet
            End Get
            Set(ByVal value As Integer)
                If (Me.IsAttached) Then
                    If (Me.Editor.Fleet <> value) Then
                        Me.Editor.Fleet = value
                    End If
                End If
            End Set
        End Property

        Private Sub OnFleetSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbFleet.SelectedIndexChanged
            Me.FleetIndex = Me.m_cmbFleet.SelectedIndex
        End Sub

    End Class

End Namespace

