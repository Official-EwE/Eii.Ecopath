#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' ===========================================================================
    ''' <summary>
    ''' Helper class to perform PropertyInfo-related smartness
    ''' </summary>
    ''' ===========================================================================
    Public Class cPropertyInfoHelper

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get all allowed properties for display in the grid.
        ''' </summary>
        ''' <param name="t">The runtime type to obtain the properties for.</param>
        ''' <returns>A sorted array of PropertyInfo instances.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetAllowedProperties(ByVal t As Type) As PropertyInfo()

            ' ToDo: perform sanity checks here if no converter defined

            Dim conv As TypeConverter = TypeDescriptor.GetConverter(t)
            Dim pdc As PropertyDescriptorCollection = conv.GetProperties(Nothing, Activator.CreateInstance(t), Nothing)
            Dim piOut As New List(Of PropertyInfo)

            For i As Integer = 0 To pdc.Count - 1
                If pdc(i).IsBrowsable Then piOut.Add(cPropertyConverter.FindOrigPropertyInfo(t, pdc(i)))
            Next

            Return piOut.ToArray

        End Function

    End Class

End Namespace
