#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous Assemblyname-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cAssemblyUtils

#Region " Internal helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to sort an assembly name list
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class AssemblyNameComparer
            Implements IComparer(Of AssemblyName)

            Public Function Compare(ByVal x As System.Reflection.AssemblyName, ByVal y As System.Reflection.AssemblyName) As Integer Implements System.Collections.Generic.IComparer(Of System.Reflection.AssemblyName).Compare
                Return String.Compare(x.Name, y.Name)
            End Function
        End Class

#End Region ' Internal helper classes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the short name of an assembly.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName">AssemblyName</see> to return
        ''' the name for.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetName(ByVal an As AssemblyName) As String
            If (an Is Nothing) Then Return String.Empty
            Return an.Name
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Extracts the public key token of an assembly and returns it as a string.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName">AssemblyName</see> to return
        ''' the public key token for.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetToken(ByVal an As AssemblyName) As String
            If (an Is Nothing) Then Return String.Empty
            Return cStringUtils.ToHexString(an.GetPublicKeyToken())
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the version number of an assembly.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName">AssemblyName</see> to return
        ''' the version for.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetVersion(ByVal an As AssemblyName) As Version
            If (an Is Nothing) Then Return Nothing
            Return an.Version
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get an assembly name for a class type.
        ''' </summary>
        ''' <param name="tclass">The class to return the defining assembly name for.</param>
        ''' <returns>An AssemblyName, or nothing if the class type could not be resolved.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetAssemblyName(ByVal tclass As Type) As AssemblyName

            If (tclass Is Nothing) Then Return Nothing
            Dim ass As Assembly = Assembly.GetAssembly(tclass)
            If (ass Is Nothing) Then Return Nothing
            Return ass.GetName()

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Reports all referenced, proprietary <see cref="AssemblyName">assemblies</see> 
        ''' for a given assembly.
        ''' </summary>
        ''' <param name="entry">The entry assembly to find the summary of referenced
        ''' assemblies for.</param>
        ''' <remarks>
        ''' The array of assembly names will be sorted by name.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetSummary(ByVal entry As Assembly) As AssemblyName()

            ' List to hold collected summary data
            Dim lAssemblies As New List(Of AssemblyName)
            ' List of assembly name prefixes NOT to include in the list. These are all .NET framework prefixes.
            Dim astrFrameworkPrefixes() As String = {"mscorlib", "system", "microsoft"}
            ' All required assemblies (not actually loaded!)
            Dim anRequired() As AssemblyName = entry.GetReferencedAssemblies()

            lAssemblies.Add(Assembly.GetEntryAssembly().GetName())

            ' Figure out which assemblies to show
            For Each an As AssemblyName In anRequired
                ' Not already accepted?
                If Not lAssemblies.Contains(an) Then
                    ' #Yes: this is a new assembly
                    ' Assume that assembly can be added
                    Dim bAddAssembly As Boolean = True
                    ' Check if this a blacklisted assembly
                    For Each strName As String In astrFrameworkPrefixes
                        ' Does name begin with a blacklisted string?
                        If an.FullName.ToLower().IndexOf(strName) = 0 Then
                            ' #Yes: can not add assembly
                            bAddAssembly = False
                        End If
                    Next
                    ' So what did the jury decide?
                    If bAddAssembly Then lAssemblies.Add(an)
                End If
            Next

            ' Sort accepted assembly list
            lAssemblies.Sort(New AssemblyNameComparer())

            Return lAssemblies.ToArray()

        End Function

    End Class

End Namespace
