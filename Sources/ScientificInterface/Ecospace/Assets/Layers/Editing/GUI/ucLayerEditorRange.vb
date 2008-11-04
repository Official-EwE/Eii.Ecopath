'==============================================================================
'
' $Log: ucLayerEditorRange.vb,v $
' Revision 1.1  2008/11/04 04:40:34  jeroens
' Split into separate files, moved
'
' Revision 1.2  2008/10/15 17:03:58  jeroens
' Reworking
'
' Revision 1.1  2008/10/14 20:21:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorRange

#Region " Private vars "

        Private m_sg As StyleGuide = StyleGuide.GetInstance()

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#End Region ' Construction

#Region " Overrides "

        Public Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            ' Sanity check
            If (Me.m_nudValue Is Nothing) Then Return

            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.m_nudValue.DecimalPlaces = 0
            Else
                Me.m_nudValue.DecimalPlaces = Me.m_sg.NumDigits
            End If
        End Sub

#End Region ' Overrides

#Region " Events "

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudValue.ValueChanged
            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.Editor.CellValue = CInt(Me.m_nudValue.Value)
            Else
                Me.Editor.CellValue = CSng(Me.m_nudValue.Value)
            End If
        End Sub

        Private Sub DoLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            REM Oh wow, REM still works?! I SOOO feel like 1982 right now!
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.UpdateControls()
        End Sub

        Private Sub DoTrashMePlenty(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As StyleGuide.eChangeType)
            If ((cf And StyleGuide.eChangeType.NumDigits) > 0) Then
                Me.UpdateControls()
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace
