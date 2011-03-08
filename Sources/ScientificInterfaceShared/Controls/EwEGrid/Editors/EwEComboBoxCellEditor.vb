#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style
Imports SourceGrid2.DataModels

#End Region ' Imports

Namespace Controls.EwEGrid

    <CLSCompliant(False)> _
    Public Class EwEComboBoxCellEditor(Of t)
        Inherits EditorComboBox

        Public Sub New(ByVal aValues() As t, ByVal formatter As ITypeFormatter(Of t))

            MyBase.New(aValues.GetType.GetElementType)

            Dim mapping As New SourceLibrary.ComponentModel.Validator.ValueMapping()
            Dim lValues As New List(Of Integer)
            Dim lRepresentations As New List(Of String)

            For i As Integer = 0 To aValues.Length - 1
                lRepresentations.Add(formatter.GetDescriptor(aValues(i)))
            Next

            Me.StandardValues = aValues
            Me.StandardValuesExclusive = True
            Me.AllowStringConversion = False

            mapping.ValueList = aValues
            mapping.DisplayStringList = lRepresentations
            mapping.BindValidator(Me)

        End Sub

        'Protected Overrides Sub OnConvertingObjectToValue(ByVal e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)
        '    ' JS: called internally when done editing the combo box
        '    MyBase.OnConvertingObjectToValue(e)
        'End Sub

        Protected Overrides Sub OnConvertingValueToDisplayString(ByVal e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)
            Try
                If Not Me.ValueType.UnderlyingSystemType.IsAssignableFrom(e.Value.GetType) Then
                    Dim iValue As Integer = 0
                    If TypeOf (e.Value) Is String Then
                        iValue = Integer.Parse(CStr(e.Value))
                    Else
                        iValue = CInt(e.Value)
                    End If
                    e.Value = [Enum].ToObject(Me.ValueType, iValue)
                End If
                MyBase.OnConvertingValueToDisplayString(e)
            Catch ex As Exception
                ' Should not occur any more
            End Try
        End Sub

        'Protected Overrides Sub OnConvertingValueToObject(ByVal e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)
        '    MyBase.OnConvertingValueToObject(e)
        'End Sub

    End Class

End Namespace
