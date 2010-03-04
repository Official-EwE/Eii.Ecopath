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

#Region " Construction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#End Region ' Construction

#Region " Overrides "

        Public Overrides Sub UpdateContent()
            MyBase.UpdateContent()

            ' Sanity check
            If (Me.m_nudValue Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.m_nudValue.DecimalPlaces = 0
            Else
                Me.m_nudValue.DecimalPlaces = Me.UIContext.StyleGuide.NumDigits
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

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.UpdateContent()
        End Sub

        Protected Overrides Sub OnHandleDestroyed(ByVal e As System.EventArgs)

            If (Me.UIContext Is Nothing) Then Return

            RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            MyBase.OnHandleDestroyed(e)
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
            If ((cf And cStyleGuide.eChangeType.NumberFormatting) > 0) Then
                Me.UpdateContent()
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace
