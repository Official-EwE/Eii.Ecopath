' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.ComponentModel
Imports System.Reflection
Imports EwEUtils.Utilities



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
        Public Shared Function GetAllowedProperties(t As Type) As PropertyInfo()

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
