#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

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

        Public Overrides Sub UpdateContent()
            MyBase.UpdateContent()

            ' Sanity checks
            If (Me.Editor Is Nothing) Then Return
            If (Me.m_ucSlider Is Nothing) Then Return

            Me.m_ucSlider.Value = Me.Editor.CursorSize
        End Sub

        Private Sub OnSliderValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_ucSlider.ValueChanged

            If Me.Editor Is Nothing Then Return

            Me.Editor.CursorSize = CInt(Me.m_ucSlider.Value)
            Me.RaiseChangedEvent()

        End Sub

    End Class

End Namespace
