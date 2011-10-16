Imports EwECore
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    Public Class ucLayerEditorHabitatCapacity

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim group As cCoreGroupBase = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            Me.m_cmbGroups.Items.Clear()

            For iGroup As Integer = 1 To core.nGroups
                group = core.EcoPathGroupInputs(iGroup)
                Me.m_cmbGroups.Items.Add(fmt.GetDescriptor(group))
            Next iGroup

            ' Update control
            Me.m_cmbGroups.SelectedIndex = Me.GroupIndex - 1

        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            Me.m_cmbGroups.Enabled = Me.IsAttached

        End Sub

        Protected Overloads Property Editor() As cLayerEditorGroup
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorGroup)
            End Get
            Set(ByVal editor As cLayerEditorGroup)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorGroup, "ucLayerEditorGroup connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 0
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

        Protected Property GroupIndex() As Integer
            Get
                If (Not Me.IsAttached) Then Return cCore.NULL_VALUE
                Return Me.Editor.Group
            End Get
            Set(ByVal value As Integer)
                If (Me.IsAttached) Then
                    If (Me.Editor.Group <> value) Then
                        Me.Editor.Group = value
                    End If
                End If
            End Set
        End Property

        Private Sub OnGroupSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbGroups.SelectedIndexChanged
            Me.GroupIndex = Me.m_cmbGroups.SelectedIndex + 1
        End Sub

    End Class

End Namespace
