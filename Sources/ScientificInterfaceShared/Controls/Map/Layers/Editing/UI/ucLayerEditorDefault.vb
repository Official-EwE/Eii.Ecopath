#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorDefault

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity checks
            If (editor Is Nothing) Then Return
            If (Me.m_ucSlider Is Nothing) Then Return

            Dim bEnabled As Boolean = editor.IsEditable

            Me.m_ucSlider.Value = editor.CursorSize
            Me.m_ucSlider.Enabled = bEnabled
            Me.m_lblCursor.Enabled = bEnabled

        End Sub

        Private Sub OnSliderValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_ucSlider.ValueChanged

            If Me.Editor Is Nothing Then Return

            Me.Editor.CursorSize = CInt(Me.m_ucSlider.Value)
            Me.RaiseChangedEvent()

        End Sub

    End Class

End Namespace
