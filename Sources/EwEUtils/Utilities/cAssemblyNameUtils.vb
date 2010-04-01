#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports Microsoft.VisualBasic
Imports System

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous Assemblyname-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cAssemblyUtils

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
        ''' Extracts the version number of an assembly and returns it as a string.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName">AssemblyName</see> to return
        ''' the version for.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetVersion(ByVal an As AssemblyName) As String
            If (an Is Nothing) Then Return String.Empty
            Return an.Version.ToString()
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

    End Class

End Namespace
