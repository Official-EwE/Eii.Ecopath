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




Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities



Namespace IO

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to write a summary of fields, their type, and allowed enum values.
    ''' </summary>
    ''' <remarks>This writer adheres to <see cref="cCore.SaveWithFileHeader"/></remarks>
    ''' -----------------------------------------------------------------------
    Public Class cFieldInfoWriter

        Private m_core As cCore = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="cFieldInfoWriter"/> class.
        ''' </summary>
        ''' <param name="core">The core.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Writes all public instance properties of a given object to a text file.
        ''' </summary>
        ''' <param name="obj">The object that has the fields to write.</param>
        ''' <param name="fn">The file name to write to.</param>
        ''' <param name="excludedproperties">Optional propoerty names to exclude.
        ''' Property names are case sensitive.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' <seealso cref="Write(ICollection(Of PropertyInfo), String)"/>
        ''' <seealso cref="Write(Type, String, ICollection(Of String))"/>
        ''' -----------------------------------------------------------------------
        Public Function Write(obj As Object, fn As String, Optional excludedproperties As ICollection(Of String) = Nothing) As Boolean

            If (obj Is Nothing) Then Return False
            Return Me.Write(obj.GetType(), fn)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Writes all public instance properties of a given object to a text file.
        ''' </summary>
        ''' <param name="t">The object type with the fields to write.</param>
        ''' <param name="fn">The file name to write to.</param>
        ''' <param name="excludedproperties">Optional propoerty names to exclude.
        ''' Property names are case sensitive.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' <seealso cref="Write(Object, String, ICollection(Of String))"/>
        ''' <seealso cref="Write(ICollection(Of PropertyInfo), String)"/>
        ''' -----------------------------------------------------------------------
        Public Function Write(t As Type, fn As String, Optional excludedproperties As ICollection(Of String) = Nothing) As Boolean

            If (t Is Nothing) Then Return False
            Dim pis As New List(Of PropertyInfo)

            For Each pi As PropertyInfo In t.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                If ((excludedproperties Is Nothing) OrElse (Not excludedproperties.Contains(pi.Name))) Then
                    If (cPropertyUtils.IsWritableElemental(pi)) Then
                        pis.Add(pi)
                    End If
                End If
            Next
            Return Me.Write(pis, fn)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Writes a specified list of <see cref="PropertyInfo"/> instances to 
        ''' a text file.
        ''' </summary>
        ''' <param name="info">The <see cref="PropertyInfo"/> instances to write to a text file.</param>
        ''' <param name="fn">The file name to write to.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' <seealso cref="Write(Object, String, ICollection(Of String))"/>
        ''' <seealso cref="Write(Type, String, ICollection(Of String))"/>
        ''' -----------------------------------------------------------------------
        Public Function Write(info As ICollection(Of PropertyInfo), fn As String) As Boolean

            If (info Is Nothing) Then Return False
            If (info.Count = 0) Then Return False

            Using sw As New StreamWriter(fn)

                If (Me.m_core.SaveWithFileHeader) Then
                    sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.NotSet))
                End If

                For Each pi As PropertyInfo In info

                    Dim t As Type = pi.PropertyType

                    If t.IsEnum Then
                        Dim vt As Type = [Enum].GetUnderlyingType(t)
                        Dim tn As String = vt.ToString()
                        tn = tn.Substring(tn.LastIndexOf(".") + 1)

                        sw.WriteLine(cStringUtils.Localize(My.Resources.FIELDINFO_ENUM, pi.Name, tn))
                        For Each v As Object In [Enum].GetValues(t)
                            sw.WriteLine("{0,8}: {1}", Convert.ChangeType(v, vt), [Enum].GetName(t, v))
                        Next
                    Else
                        Dim tn As String = t.ToString()
                        tn = tn.Substring(tn.LastIndexOf(".") + 1)
                        sw.WriteLine(cStringUtils.Localize(My.Resources.FIELDINFO_VARIABLE, pi.Name, tn))
                    End If

                Next

            End Using
            Return True

        End Function

    End Class

End Namespace
