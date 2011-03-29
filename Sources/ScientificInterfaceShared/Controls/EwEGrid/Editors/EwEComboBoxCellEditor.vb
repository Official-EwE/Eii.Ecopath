#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style
Imports SourceGrid2.DataModels

#End Region ' Imports

Namespace Controls.EwEGrid

    <CLSCompliant(False)> _
    Public Class EwEComboBoxCellEditor
        Inherits EditorComboBox

        Public Sub New(ByVal formatter As ITypeFormatter)

            MyBase.New(formatter.GetDescribedType)

            Dim mapping As New SourceLibrary.ComponentModel.Validator.ValueMapping()
            Dim lValues As New List(Of Object)
            Dim lRepresentations As New List(Of String)

            For Each key As Object In [Enum].GetValues(formatter.GetDescribedType)
                lValues.Add(key)
                lRepresentations.Add(formatter.GetDescriptor(key))
            Next

            Me.StandardValues = lValues
            Me.StandardValuesExclusive = True
            Me.AllowStringConversion = False
            Me.EditableMode = SourceGrid2.EditableMode.SingleClick Or SourceGrid2.EditableMode.Focus Or SourceGrid2.EditableMode.AnyKey

            mapping.ValueList = lValues
            mapping.DisplayStringList = lRepresentations
            mapping.BindValidator(Me)

        End Sub

        Protected Overrides Sub OnConvertingObjectToValue(ByVal e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)
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
            MyBase.OnConvertingObjectToValue(e)
        End Sub

    End Class

End Namespace
