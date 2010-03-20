#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
Imports Microsoft.VisualBasic

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
            Return an.Version.ToString()
        End Function

    End Class

End Namespace
