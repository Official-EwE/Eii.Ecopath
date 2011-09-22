#Region " Imports "

Option Strict On
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorGroup

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim grp As cCoreGroupBase = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            Me.m_cmbGroup.Items.Clear()

            ' ToDo: this control will not respond to dynamic name changes
            For iGroup As Integer = 1 To core.nGroups
                grp = core.EcoPathGroupInputs(iGroup)
                Me.m_cmbGroup.Items.Add(fmt.GetDescriptor(grp))
            Next iGroup

            ' Update control
            Me.m_cmbGroup.SelectedIndex = Me.GroupIndex

        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            Me.m_cmbGroup.Enabled = Me.IsAttached

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

        Private Sub OnSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbGroup.SelectedIndexChanged
            Me.GroupIndex = Me.m_cmbGroup.SelectedIndex
        End Sub

    End Class

End Namespace

