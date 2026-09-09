' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports System.Security.Permissions
Imports System.Security.Policy

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous Assemblyname-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cAssemblyUtils

#Region " Internal helper classes "

        ''' <summary>EwE assembly name prefixes.</summary>
        Private Shared s_CoreNames As String() = New String() {"EwEUtils", "EwEPlugin", "EwECore", "ScientificInterfaceShared", "EwE6"}
        ''' <summary>For quick look-up</summary>
        Private Shared m_cache As New cAssemblyStateCache()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to sort an assembly name list.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class AssemblyNameComparer
            Implements IComparer(Of AssemblyName)

            Public Function Compare(x As System.Reflection.AssemblyName, y As System.Reflection.AssemblyName) As Integer _
                Implements System.Collections.Generic.IComparer(Of System.Reflection.AssemblyName).Compare

                Dim i As Integer = String.Compare(x.Name, y.Name)
                If (i = 0) Then
                    i = x.Version.CompareTo(y.Version)
                End If
                Return i
            End Function
        End Class

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to maintain the EwE state of a single assembly.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cAssemblyState

            Public ReadOnly Property AssemblyName As AssemblyName

            Public ReadOnly Property IsEwE As Boolean
            Public ReadOnly Property IsEwECore As Boolean
            Public ReadOnly Property IsEwEExt As Boolean
            Public ReadOnly Property IsDependency As Boolean

            Public ReadOnly Property Version As Version
            Public ReadOnly Property InformationalVersion As String

            Public ReadOnly Property BuildDateUtc As Nullable(Of DateTime)
            Public ReadOnly Property CommitHash As String

            Public Sub New(an As AssemblyName)

                Me.AssemblyName = an

                Dim ass As Assembly = GetAssembly(an)

                ' Cheap metadata
                Me.Version = an.Version

                ' Classification
                Me.IsEwE = DetectEwE(an)
                Me.IsEwECore = Me.IsEwE AndAlso Array.IndexOf(s_CoreNames, an.Name) >= 0
                Me.IsEwEExt = Me.IsEwE AndAlso Not Me.IsEwECore
                Me.IsDependency = Not Me.IsEwE

                ' Runtime assembly metadata
                If ass IsNot Nothing Then

                    Me.InformationalVersion = DetectInformationalVersion(ass)
                    Me.BuildDateUtc = DetectBuildDate(ass)
                    Me.CommitHash = DetectCommitHash(ass)

                Else

                    Me.InformationalVersion = String.Empty
                    Me.BuildDateUtc = Nothing
                    Me.CommitHash = String.Empty

                End If

            End Sub

        End Class

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to cache the EwE state for a range of assemblies.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cAssemblyStateCache
            Private m_info As Dictionary(Of String, cAssemblyState) = Nothing

            Public Sub New()
                Me.m_info = New Dictionary(Of String, cAssemblyState)
            End Sub

            Public Function Item(an As AssemblyName) As cAssemblyState
                Dim strName As String = an.FullName
                If Not Me.m_info.ContainsKey(strName) Then
                    Me.m_info(strName) = New cAssemblyState(an)
                End If
                Return Me.m_info(strName)
            End Function

            Public Function IsCached(an As AssemblyName) As Boolean
                Return Me.m_info.ContainsKey(an.FullName)
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
        Public Shared Function GetName(an As AssemblyName) As String
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
        Public Shared Function GetToken(an As AssemblyName) As String
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
        Public Shared Function GetVersion(Optional an As AssemblyName = Nothing) As Version
            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            Return m_cache.Item(an).Version
        End Function

        Public Shared Function GetInformationalVersion(Optional an As AssemblyName = Nothing) As String
            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            Return m_cache.Item(an).InformationalVersion
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Gets the compile date of an assembly name. If no assembly name is specified, 
        ''' the compile date of the executing assembly is returned.
        ''' </summary>
        ''' <value>The compile date.</value>
        ''' -----------------------------------------------------------------------
        Public Shared ReadOnly Property GetCompileDate(Optional an As AssemblyName = Nothing) As DateTime
            Get
                If (an Is Nothing) Then
                    an = ExecutingAssembly.GetName
                End If
                Return m_cache.Item(an).BuildDateUtc
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether an <see cref="AssemblyName"/> is not part of the EwE 
        ''' code base.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName"/> to test.</param>
        ''' <returns>True if the <see cref="AssemblyName"/> is not part of the EwE code base.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function IsDependency(an As AssemblyName) As Boolean
            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            ' Get cached info
            Dim info As cAssemblyState = m_cache.Item(an)
            Return info.IsDependency
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Legacy call, returns whether an <see cref="AssemblyName"/> is not part 
        ''' of the EwE code base. This code used to check for known .NET Microsoft 
        ''' assembly names, but with the shift to packages and .NET core, these checks
        ''' have become increasingly meaningless and problematic to keep current. 
        ''' Use <see cref="IsDependency"/> instead.
        ''' </summary>
        ''' <param name="an">The <see cref="AssemblyName"/> to test.</param>
        ''' <returns>True if the <see cref="AssemblyName"/> is not part of the EwE code base.</returns>
        ''' -----------------------------------------------------------------------
        <Obsolete("Use IsDependency instead")>
        Public Shared Function IsFramework(an As AssemblyName) As Boolean

            Return IsDependency(an)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a given assembly name is built upon one of the EwE core assemblies.
        ''' </summary>
        ''' <param name="an">The assembly name to check.</param>
        ''' <returns>True if the assembly is built upon the EwE assemblies, but is not a 
        ''' part of the EwE core libraries.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IsEwEExternal(an As AssemblyName) As Boolean

            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            ' Get cached info
            Dim info As cAssemblyState = m_cache.Item(an)
            Return info.IsEwEExt

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a given assembly name is one of the EwE core assemblies.
        ''' </summary>
        ''' <param name="an">The assembly name to check.</param>
        ''' <returns>True if the assembly is one of the EwE assemblies.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IsEwECore(an As AssemblyName) As Boolean

            If (an Is Nothing) Then
                an = ExecutingAssembly.GetName
            End If
            ' Get cached info
            Dim info As cAssemblyState = m_cache.Item(an)
            Return info.IsEwECore

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get an assembly name for a class type.
        ''' </summary>
        ''' <param name="tclass">The class to return the defining assembly name for.</param>
        ''' <returns>An AssemblyName, or nothing if the class type could not be resolved.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetAssemblyName(tclass As Type) As AssemblyName

            If (tclass Is Nothing) Then Return Nothing
            Dim ass As Assembly = Assembly.GetAssembly(tclass)
            If (ass Is Nothing) Then Return Nothing
            Return ass.GetName()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a StrongName that matches a specific assembly.
        ''' </summary>
        ''' <param name="an">Assembly name to create a StrongName for.</param>
        ''' <returns>A StrongName that matches the given assembly, or Nothing
        ''' if the assembly was not strongly named.</returns>
        ''' <remarks>
        ''' Adapted from http://blogs.msdn.com/b/shawnfa/archive/2005/08/08/449050.aspx
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetStrongName(an As AssemblyName) As StrongName

            ' Test if assembly is present
            If (an Is Nothing) Then Return Nothing

            ' Get the public key blob
            Dim publicKey As Byte() = an.GetPublicKey()

            ' Test if assembly is strongly named
            If (publicKey Is Nothing) Then Return Nothing
            If (publicKey.Length = 0) Then Return Nothing

            ' Create the StrongName
            Dim keyBlob As New StrongNamePublicKeyBlob(publicKey)
            Return New StrongName(keyBlob, an.Name, an.Version)

        End Function

#Region " Inventory "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Bitwise flags for obtaining assembly information.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Enum eSummaryFlags As Byte
            ''' <summary>EwE core assemblies.</summary>
            EwECore = 1
            ''' <summary>Assemblies built on EwE, but not part of <see cref="eSummaryFlags.EwECore"/>.</summary>
            EwEExtended = 2
            ''' <summary>.NET Framework assemblies.</summary>
            Framework = 4
            ''' <summary>Referenced assemblies.</summary>
            Referenced = 8
            ''' <summary>All possible assemblies.</summary>
            All = 255
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Reports all <see cref="AssemblyName">assemblies</see> referenced by the
        ''' current <see cref="AppDomain">application domain</see>.
        ''' </summary>
        ''' <param name="flags">Bitwise combination of <see cref="eSummaryFlags">summary
        ''' flags</see>, stating that assemblies should be included in the summary.</param>
        ''' <remarks>
        ''' The array of assembly names will be sorted by name.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetSummary(Optional flags As eSummaryFlags = eSummaryFlags.EwECore Or eSummaryFlags.EwEExtended) As AssemblyName()

            Dim hsh As New HashSet(Of String)
            Dim lSummary As New List(Of AssemblyName)
            Dim strFullName As String = ""

            ' Get a summary for all loaded assemblies
            For Each ass As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                For Each an As AssemblyName In GetSummary(ass, flags)
                    strFullName = an.FullName
                    If Not hsh.Contains(strFullName) Then
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
        ''' Recursively reports all <see cref="AssemblyName">assemblies</see> referenced 
        ''' by a given assembly.
        ''' </summary>
        ''' <param name="entry">The entry assembly to find the summary of referenced
        ''' assemblies for.</param>
        ''' <param name="flags">Bitwise combination of <see cref="eSummaryFlags">summary
        ''' flags</see>, stating that assemblies should be included in the summary.</param>
        ''' <remarks>
        ''' The array of assembly names will be sorted by name.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Shared Function GetSummary(entry As Assembly,
                                           Optional flags As eSummaryFlags = eSummaryFlags.EwECore Or eSummaryFlags.EwEExtended) As AssemblyName()

            ' List to hold collected summary data
            Dim lAssemblies As New List(Of AssemblyName)

            If (entry Is Nothing) Then
                Return lAssemblies.ToArray()
            End If

            Dim an As AssemblyName = entry.GetName()
            Dim bAddAssembly As Boolean = False
            Dim bIsEwECore As Boolean = IsEwECore(an)
            Dim bIsEwEExt As Boolean = IsEwEExternal(an)
            Dim bIsFramework As Boolean = IsFramework(an)
            Dim bIsReferenced As Boolean = (Not bIsEwECore) And (Not bIsEwEExt) And (Not bIsFramework)

            If (flags And eSummaryFlags.EwECore) = eSummaryFlags.EwECore Then
                bAddAssembly = bAddAssembly Or bIsEwECore
            End If

            If (flags And eSummaryFlags.EwEExtended) = eSummaryFlags.EwEExtended Then
                bAddAssembly = bAddAssembly Or bIsEwEExt
            End If

            If (flags And eSummaryFlags.Framework) = eSummaryFlags.Framework Then
                bAddAssembly = bAddAssembly Or bIsFramework
            End If

            If (flags And eSummaryFlags.Referenced) = eSummaryFlags.Referenced Then
                bAddAssembly = bAddAssembly Or bIsReferenced
            End If

            ' Is one we're after?
            If bAddAssembly Then
                ' #Yes: add to list
                lAssemblies.Add(an)
                ' Consider all referenced assemblies too
                For Each an In entry.GetReferencedAssemblies()
                    ' Do not too much work
                    If Not m_cache.IsCached(an) Then
                        lAssemblies.AddRange(GetSummary(GetAssembly(an), flags))
                    End If
                Next
            End If

            Return lAssemblies.ToArray()

        End Function

#End Region ' Inventory

#Region " Detection internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a given assembly is either an EwE core assembly, or
        ''' references the EwEUtils core assembly.
        ''' </summary>
        ''' <param name="an"></param>
        ''' <returns>True if the given assembly is either an EwE core assembly, 
        ''' or references the EwEUtils core assembly.</returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function DetectEwE(an As AssemblyName) As Boolean

            If (an Is Nothing) Then Return False

            ' Get EwEUtils assembly. All EwE assemblies refer to EwEUtils
            Dim anEwEUtils As AssemblyName = GetType(cAssemblyUtils).Assembly().GetName

            ' Ok if this as = EwEUtils
            If CompareNames(anEwEUtils, an) Then Return True

            ' Check if the assembly for 'an' refers to EwEUtils
            ' For this, we'll first need to find each assembly for the given assembly name. *sigh*
            For Each ass As Assembly In AppDomain.CurrentDomain.GetAssemblies
                ' Got one of ours?
                If CompareNames(ass.GetName, an) Then
                    ' #Yes: now check if this 
                    For Each anTest As AssemblyName In ass.GetReferencedAssemblies
                        If CompareNames(anTest, anEwEUtils) Then
                            Return True
                        End If
                    Next
                End If
            Next
            Return False

        End Function

        ''' <summary>
        ''' Returns the informational/product version of an assembly.
        ''' This may contain semantic version information and, for modern SDK builds,
        ''' source revision information.
        ''' </summary>
        Private Shared Function DetectInformationalVersion(ass As Assembly) As String
            Debug.Assert(ass IsNot Nothing)
            Dim attr As AssemblyInformationalVersionAttribute = ass.GetCustomAttribute(Of AssemblyInformationalVersionAttribute)()
            If attr Is Nothing Then
                Return ass.GetName().Version.ToString()
            End If
            Return attr.InformationalVersion
        End Function

        Private Shared Function DetectCommitHash(ass As Assembly) As String

            ' Prefer explicit assembly metadata
            For Each attr As AssemblyMetadataAttribute In ass.GetCustomAttributes(Of AssemblyMetadataAttribute)()

                If String.Equals(attr.Key, "CommitHash", StringComparison.OrdinalIgnoreCase) Then
                    Return attr.Value
                End If

                If String.Equals(attr.Key, "SourceRevisionId", StringComparison.OrdinalIgnoreCase) Then
                    Return attr.Value
                End If

            Next

            ' Fall back to informational version:
            ' 6.7.0+0123456789abcdef...
            Dim attrInfo = ass.GetCustomAttribute(Of AssemblyInformationalVersionAttribute)()

            If attrInfo IsNot Nothing Then

                Dim strVersion As String = attrInfo.InformationalVersion
                Dim i As Integer = strVersion.LastIndexOf("+"c)

                If i >= 0 AndAlso i < strVersion.Length - 1 Then
                    Return strVersion.Substring(i + 1)
                End If

            End If

            Return String.Empty

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Gets the compile date of the <see cref="ExecutingAssembly">currently 
        ''' executing assembly</see>.
        ''' </summary>
        ''' <value>The compile date.</value>
        ''' -----------------------------------------------------------------------
        Public Shared ReadOnly Property DetectBuildDate(Optional ass As Assembly = Nothing) As DateTime
            Get
                If ass Is Nothing Then
                    ass = ExecutingAssembly
                End If

#If NETFRAMEWORK Then
                 Return DetectBuildDateNet48(ass.Location)
#Else
                Dim dt As Nullable(Of DateTime) = DetectBuildDate(ass)
                If dt.HasValue Then Return dt.Value

                Return DateTime.MinValue
#End If
            End Get
        End Property

#If NETFRAMEWORK Then

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
        Private Shared Function DetectBuildDateNet48(strAssemblyPath As String) As System.DateTime

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

#Else

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the build date embedded in the assembly metadata.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Shared Function DetectBuildDateCore(Optional ass As Assembly = Nothing) As Nullable(Of DateTime)
            If ass Is Nothing Then
                ass = ExecutingAssembly
            End If
            For Each attr As AssemblyMetadataAttribute In ass.GetCustomAttributes(Of AssemblyMetadataAttribute)()

                If String.Equals(attr.Key, "EwEBuildDateUtc", StringComparison.OrdinalIgnoreCase) Then

                    Dim dt As DateTime
                    If DateTime.TryParse(attr.Value, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.AssumeUniversal Or Globalization.DateTimeStyles.AdjustToUniversal, dt) Then
                        Return dt
                    End If
                End If
            Next
            Return Nothing
        End Function

#End If

#End Region ' Detection internals

#Region " Internal helpers "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether two assembly names can be considered equal.
        ''' </summary>
        ''' <param name="an1"></param>
        ''' <param name="an2"></param>
        ''' <returns>True if the names equal.</returns>
        ''' <remarks>
        ''' This code does not use 
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Shared Function CompareNames(an1 As AssemblyName, an2 As AssemblyName) As Boolean
            If (an1 Is Nothing) Or (an2 Is Nothing) Then Return False
            Return (String.Compare(an1.FullName, an2.FullName, True) = 0)
        End Function

        Private Shared Function GetAssembly(an As AssemblyName) As Assembly

            For Each ass As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                If CompareNames(ass.GetName, an) Then
                    Return ass
                End If
            Next
            Return Nothing

        End Function

#End Region

    End Class

End Namespace
