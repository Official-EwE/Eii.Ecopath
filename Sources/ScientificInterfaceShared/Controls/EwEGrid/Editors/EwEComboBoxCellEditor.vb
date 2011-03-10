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
            Me.EditableMode = SourceGrid2.EditableMode.SingleClick Or SourceGrid2.EditableMode.Focus Or SourceGrid2.EditableMode.AnyKey

            mapping.ValueList = aValues
            mapping.DisplayStringList = lRepresentations
            mapping.BindValidator(Me)

        End Sub

        Protected Overrides Sub OnConvertingValueToDisplayString(ByVal e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)
            Try
                If Not Me.ValueType.UnderlyingSystemType.IsAssignableFrom(e.Value.GetType) Then
                    Dim iValue As Integer = 0
                    If TypeOf (e.Value) Is String Then
                        iValue = Integer.Parse(CStr(e.Value))
                    Else
                        iValue = CInt(e.Value)
                    End If
                    If Me.ValueType.IsEnum Then
                        If Not [Enum].IsDefined(Me.ValueType, iValue) Then
                            ' Clear!
                            e.Value = Me.StandardValueAtIndex(0)
                        Else
                            e.Value = [Enum].ToObject(Me.ValueType, iValue)
                        End If
                    Else
                        e.Value = iValue
                    End If
                End If
                MyBase.OnConvertingValueToDisplayString(e)
            Catch ex As Exception
                ' Should not occur any more
            End Try
        End Sub

    End Class

End Namespace
