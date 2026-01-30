' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports SourceGrid2.DataModels



Namespace Controls.EwEGrid


    Public Class EwEComboBoxCellEditor
        Inherits EditorComboBox

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a combo box editor that shows a range of values obtained from 
        ''' a <see cref="ITypeFormatter"/>
        ''' </summary>
        ''' <param name="formatter">The <see cref="ITypeFormatter">type formatter</see> to link to.</param>
        ''' <param name="standardvalues">An optional (sub)set of values to present in the combo box.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(formatter As ITypeFormatter, Optional standardvalues As ICollection = Nothing)

            MyBase.New(formatter.GetDescribedType)

            Dim mapping As New SourceLibrary.ComponentModel.Validator.ValueMapping()
            Dim lValues As New List(Of Object)
            Dim lRepresentations As New List(Of String)

            ' No standard values provided?
            If (standardvalues Is Nothing) Then
                ' #Yes: formatting an enum?
                If (formatter.GetDescribedType.IsEnum) Then
                    ' #Yes: auto-extract standard values
                    For Each key As Object In [Enum].GetValues(formatter.GetDescribedType)
                        lValues.Add(key)
                        lRepresentations.Add(formatter.ToString(key))
                    Next
                End If
            Else
                ' #No: add standard values
                For Each item As Object In standardvalues
                    lValues.Add(item)
                    lRepresentations.Add(formatter.ToString(item))
                Next
            End If

            Me.StandardValues = lValues
            Me.StandardValuesExclusive = True
            Me.AllowStringConversion = False
            Me.EditableMode = SourceGrid2.EditableMode.SingleClick Or SourceGrid2.EditableMode.Focus Or SourceGrid2.EditableMode.AnyKey

            mapping.ValueList = lValues
            mapping.DisplayStringList = lRepresentations
            mapping.BindValidator(Me)

        End Sub

        Protected Overrides Sub OnConvertingObjectToValue(e As SourceLibrary.ComponentModel.ConvertingObjectEventArgs)

            If (e.Value IsNot Nothing) Then
                If Not Me.ValueType.UnderlyingSystemType.IsAssignableFrom(e.Value.GetType) Then
                    Try
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
                    Catch ex As Exception

                    End Try
                End If
            End If

            MyBase.OnConvertingObjectToValue(e)
        End Sub

    End Class

End Namespace
