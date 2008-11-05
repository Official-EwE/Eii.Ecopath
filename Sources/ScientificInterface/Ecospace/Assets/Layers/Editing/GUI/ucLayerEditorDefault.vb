'==============================================================================
'
' $Log: ucLayerEditorDefault.vb,v $
' Revision 1.2  2008/11/05 01:13:20  jeroens
' Fixed crash
'
' Revision 1.1  2008/11/04 04:40:34  jeroens
' Split into separate files, moved
'
' Revision 1.2  2008/10/15 17:03:57  jeroens
' Reworking
'
' Revision 1.1  2008/10/14 20:21:25  jeroens
' Initial version
'
' Revision 1.1  2008/10/10 20:08:09  jeroens
' Initial version
'
'==============================================================================

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

        Public Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            ' Sanity check
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
