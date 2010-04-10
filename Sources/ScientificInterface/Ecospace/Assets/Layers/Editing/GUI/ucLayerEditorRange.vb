#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Layer editor interface for editing a layer that may contain a range of values.
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorRange

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            Try
                If bDisposing Then
                    If (Me.UIContext Is Nothing) Then Return

                    RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(bDisposing)
            End Try
        End Sub

#End Region ' Construction / destruction

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

            Me.m_nudValue.Maximum = Me.Editor.CellValueMax
            Me.m_nudValue.Minimum = Me.Editor.CellValueMin
            Me.m_nudValue.Value = Math.Max(Math.Min(Me.Editor.CellValue, Me.Editor.CellValueMax), Me.Editor.CellValueMin)

        End Sub

#End Region ' Overrides

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.UpdateContent()
        End Sub

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudValue.ValueChanged

            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.Editor.CellValue = Me.m_nudValue.Value
            Else
                Me.Editor.CellValue = Me.m_nudValue.Value
            End If
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
            If ((cf And cStyleGuide.eChangeType.NumberFormatting) > 0) Then
                Me.UpdateContent()
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace
