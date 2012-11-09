' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Reflection
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

            Public Function Compare(ByVal x As System.Reflection.AssemblyName, ByVal y As System.Reflection.AssemblyName) As Integer _
                Implements System.Collections.Generic.IComparer(Of System.Reflection.AssemblyName).Compare
                Return String.Compare(x.Name, y.Name)
            End Function
        End Class

#End Region ' Internal helper classes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Gets the executing assembly.
        ''' </summary>
        ''' <value>The executing assembly.</value>
        ''' -----------------------------------------------------------------------
        Public Shared ReadOnly Property ExecutingAssembly() As System.Reflection.Assembly
            Get
                Return Assembly.GetExecutingAssembly()
            End Get
        End Property

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
        ''' the version for. If not specified, the version of the 
        ''' <see cref="Assembly.GetExecutingAssembly">executing assembly</see> is returned.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetVersion(Optional ByVal an As AssemblyName = Nothing) As Version
            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            Return an.Version
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Gets the compile date of the <see cref="ExecutingAssembly">currently 
        ''' executing assembly</see>.
        ''' </summary>
        ''' <value>The compile date.</value>
        ''' -----------------------------------------------------------------------
        Public Shared ReadOnly Property GetCompileDate(Optional ByVal an As AssemblyName = Nothing) As System.DateTime
            Get
                Dim ass As Assembly = Nothing

                Dim strFile As String = ""
                If (an Is Nothing) Then
                    ass = ExecutingAssembly
                Else
                    ass = Assembly.Load(an)
                End If
                Dim dt As DateTime = RetrieveLinkerTimestamp(ass.Location)
                If (dt = Nothing) Then dt = New DateTime()
                Return dt
            End Get
        End Property

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
        ''' Reports all <see cref="AssemblyName">assemblies</see> referenced by the
        ''' current <see cref="AppDomain">application domain</see>.
        ''' </summary>
        ''' <param name="bIncludeDotNet">Flag, stating that .NET framework assemblies 
        ''' should be included in the summary.</param>
        ''' <remarks>
        ''' The array of assembly names will be sorted by name.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetSummary(Optional bIncludeDotNet As Boolean = False) As AssemblyName()

            Dim ad As AppDomain = AppDomain.CurrentDomain
            Dim aAssemblies() As Assembly = ad.GetAssemblies()
            Dim hsh As New HashSet(Of String)
            Dim lSummary As New List(Of AssemblyName)
            Dim strFullName As String = ""

            For Each ass As Assembly In aAssemblies
                'Console.WriteLine("{0} relies on:", ass.FullName)
                For Each an As AssemblyName In GetSummary(ass, bIncludeDotNet)
                    strFullName = an.FullName
                    If Not hsh.Contains(strFullName) Then
                        'Console.WriteLine("   {0}", an.FullName)
                        lSummary.Add(an)
                        hsh.Add(strFullName)
                    End If
                Next
            Next

            lSummary.Sort(New AssemblyNameComparer())

            Return lSummary.ToArray

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Reports all <see cref="AssemblyName">assemblies</see> referenced by a 
        ''' given assembly.
        ''' </summary>
        ''' <param name="entry">The entry assembly to find the summary of referenced
        ''' assemblies for.</param>
        ''' <param name="bIncludeDotNet">Flag, stating that .NET framework assemblies 
        ''' should be included in the summary.</param>
        ''' <remarks>
        ''' The array of assembly names will be sorted by name.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetSummary(ByVal entry As Assembly, _
                                          Optional bIncludeDotNet As Boolean = False) As AssemblyName()

            ' List to hold collected summary data
            Dim lAssemblies As New List(Of AssemblyName)
            ' List of assembly name prefixes NOT to include in the list. These are all .NET framework prefixes.
            Dim astrFrameworkPrefixes() As String = {"mscorlib", "system", "microsoft", "interop", "accessibility", "office", "stdole"}
            ' All required assemblies (not actually loaded!)
            Dim ad As AppDomain = AppDomain.CurrentDomain
            Dim anRequired() As AssemblyName = entry.GetReferencedAssemblies()

            lAssemblies.Add(Assembly.GetEntryAssembly().GetName())

            ' Figure out which assemblies to show
            For Each an As AssemblyName In anRequired
                ' Not already accepted?
                If Not lAssemblies.Contains(an) Then
                    ' #Yes: this is a new assembly
                    ' Assume that assembly can be added
                    Dim bAddAssembly As Boolean = True
                    ' Need to filter out DotNet assemblies?
                    If Not bIncludeDotNet Then
                        ' #Yes: check if this a blacklisted assembly
                        For Each strName As String In astrFrameworkPrefixes
                            ' Does name begin with a blacklisted string?
                            If (an.FullName.ToLower().IndexOf(strName) = 0) Then
                                ' #Yes: can not add assembly
                                bAddAssembly = False
                            End If
                        Next
                    End If
                    ' So what did the jury decide?
                    If bAddAssembly Then lAssemblies.Add(an)
                End If
            Next

            ' Sort accepted assembly list
            lAssemblies.Sort(New AssemblyNameComparer())

            Return lAssemblies.ToArray()

        End Function

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves the linker timestamp, as written in the assembly header file
        ''' at a fixed position. This may fail one day in future .NET versions.
        ''' Ideally, the link date and time would be stored in a universal time
        ''' format in the code by the compiler.
        ''' </summary>
        ''' <param name="strAssemblyPath">Path of the assembly file to read the
        ''' build time from.</param>
        ''' <returns>The build date.</returns>
        ''' <remarks>
        ''' Taken from http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Shared Function RetrieveLinkerTimestamp(strAssemblyPath As String) As System.DateTime

            Const peHeaderOffset As Integer = 60
            Const linkerTimestampOffset As Integer = 8
            Dim b(2047) As Byte
            Dim s As System.IO.FileStream = Nothing

            Try
                s = New System.IO.FileStream(strAssemblyPath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
                s.Read(b, 0, 2048)
            Finally
                If s IsNot Nothing Then
                    s.Close()
                End If
            End Try
            Dim dt As New System.DateTime(1970, 1, 1, 0, 0, 0)

            dt = dt.AddSeconds(System.BitConverter.ToInt32(b, System.BitConverter.ToInt32(b, peHeaderOffset) + linkerTimestampOffset))
            Return dt.AddHours(System.TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours)

        End Function

#End Region

    End Class

End Namespace
