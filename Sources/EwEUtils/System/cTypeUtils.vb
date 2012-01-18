Imports System
Imports System.Reflection

Namespace Utilities

    Public Class cTypeUtils

        Public Shared Function TypeToString(ByVal t As Type) As String

            ' Include assembly short name in the type name. This enables
            ' the OOP database logic to relocate the type from its original
            ' assembly, even if similar class names exist in similar namespaces
            ' in different asssemblies. Yes, it's far fetched, but hey...
            Return t.Assembly.GetName.Name + "!" + t.FullName()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, locates the originating type from a type string.
        ''' </summary>
        ''' <param name="strType">The type name to locate the originating type
        ''' for.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The counterpart of this method, <see cref="TypeToString"/>,
        ''' can be used to create the string for a type.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function StringToType(ByVal strType As String) As Type

            ' Split assembly short name from type name
            Dim astr As String() = strType.Split(CChar("!"))
            Dim ass As Assembly = Nothing

            For Each ass In AppDomain.CurrentDomain.GetAssemblies
                If String.Compare(ass.GetName.Name, astr(0), True) = 0 Then
                    Try
                        Return ass.GetType(astr(1))
                    Catch ex As Exception

                    End Try
                End If
            Next
            Return Nothing

        End Function
    End Class

End Namespace
