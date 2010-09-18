#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorVector

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity checks
            If (Me.Editor Is Nothing) Then Return
            If (Me.m_nudValue Is Nothing) Then Return

            Me.m_nudValue.Value = editor.CursorSize
        End Sub

        Public Shadows Property Editor() As cLayerEditorVector
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorVector)
            End Get
            Set(ByVal value As cLayerEditorVector)
                MyBase.Editor = value
            End Set
        End Property

        Private Sub OnValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudValue.ValueChanged

            If Me.Editor Is Nothing Then Return

            Me.Editor.ScaleFactor = CSng(Me.m_nudValue.Value)
            Me.RaiseChangedEvent()

        End Sub

    End Class

End Namespace
