' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Windows.Forms



Namespace Utilities

    Public Class cResourceUtils

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Saves an embedded resource to a file
        ''' </summary>
        ''' <param name="strResourceName">The name of the resource, including  in the current assembly, current namespace.</param>
        ''' <param name="strFileName">The name of the file to save the resource to</param>
        ''' <param name="bOverwrite">States whether an existing file is allowed to be overwritten</param>
        ''' <param name="ass">The assembly to obtain the resource from.</param>
        ''' <param name="strNamespace">The namespace to obtain the resource from.</param>
        ''' <returns>True if successful</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function SaveResourceToFile(strResourceName As String,
                                                  strFileName As String,
                                                  Optional bOverwrite As Boolean = False,
                                                  Optional ass As Assembly = Nothing,
                                                  Optional strNamespace As String = "") As Boolean

            Dim sResource As Stream = Nothing
            Dim sFile As FileStream = Nothing
            Dim nBufLen As Integer = 256
            Dim byBuffer(nBufLen) As Byte
            Dim nBytesRead As Integer = 0

            If ass Is Nothing Then
                ass = Assembly.GetExecutingAssembly()
            End If

            If String.IsNullOrWhiteSpace(strNamespace) Then
                strNamespace = ass.GetName().Name.ToString()
            End If

            sResource = ass.GetManifestResourceStream(strNamespace & "." & strResourceName)

            ' Pre
            Debug.Assert(Not String.IsNullOrEmpty(strFileName), "Required target file name missing")
            Debug.Assert(sResource IsNot Nothing, String.Format("Resource {0} not found in {1}", strResourceName, strNamespace))

            ' Work with full path
            strFileName = Path.GetFullPath(strFileName)

            ' Make sure directory is available
            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFileName), True) Then
                Return False
            End If

            Try
                If (bOverwrite) Then
                    ' Create the file, overwriting any existing file with the same path
                    sFile = New FileStream(strFileName, FileMode.Create, FileAccess.Write)
                Else
                    ' Create the file but do not overwrite
                    sFile = New FileStream(strFileName, FileMode.CreateNew, FileAccess.Write)
                End If
            Catch ex As Exception
                ' Just so you know
                Debug.Print("Unable to create or overwrite file {0}", strFileName)
                ' Report failure
                Return False
            End Try

            ' Copy embedded resource to file
            nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
            While (nBytesRead > 0)
                sFile.Write(byBuffer, 0, nBytesRead)
                nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
            End While
            ' Done
            sFile.Close()
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load a resource string from a .NET assembly.
        ''' <seealso cref="LoadString(String, ResourceManager, CultureInfo)"/>
        ''' <seealso cref="LoadString(String, Assembly, String, CultureInfo)"/>
        ''' </summary>
        ''' <param name="strName">The name of the string resource.</param>
        ''' <param name="typeAssembly">The type for which to find the assembly.</param>
        ''' <param name="strNamespace">The namespace within the assembly, if any.</param>
        ''' <param name="culture">The culture info, if any.</param>
        ''' <returns>A string, or <paramref name="strName"/> if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Shared Function LoadString(strName As String,
                                   typeAssembly As Type,
                                   Optional strNamespace As String = "",
                                   Optional culture As CultureInfo = Nothing) As String

            Dim ass As Assembly = Assembly.GetAssembly(typeAssembly)
            Return LoadString(strName, ass, strNamespace, culture)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load a resource string from a .NET assembly.
        ''' <seealso cref="LoadString(String, ResourceManager, CultureInfo)"/>
        ''' <seealso cref="LoadString(String, Type, String, CultureInfo)"/>
        ''' </summary>
        ''' <param name="strName">The name of the string resource.</param>
        ''' <param name="ass">The assembly to load the resource from.</param>
        ''' <param name="strNamespace">The namespace within the assembly, if any.</param>
        ''' <param name="culture">The culture info, if any. If not specified, the
        ''' executing culture is used.</param>
        ''' <returns>A string, or <paramref name="strName"/> if an error occurred.</returns>
        ''' <remarks>
        ''' <para>This method will attempt to construct a resource manager for the given
        ''' assembly and namespace, and will then attempt to load the string for the 
        ''' indicated culture.</para>
        ''' <para>If the resource manager to obtain the resource from is know, please
        ''' use the much faster method <see cref="LoadString(String, ResourceManager, CultureInfo)"/> 
        ''' instead.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function LoadString(strName As String,
                                          Optional ass As Assembly = Nothing,
                                          Optional strNamespace As String = "",
                                          Optional culture As CultureInfo = Nothing) As String

            Dim rm As ResourceManager = Nothing
            Dim strRes As String = ""

            ' Provide defaults
            If (ass Is Nothing) Then ass = Assembly.GetExecutingAssembly()
            If (String.IsNullOrEmpty(strNamespace)) Then strNamespace = ass.GetName.Name & ".resources"

            Return LoadString(strName, New ResourceManager(strNamespace, ass), culture)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load a resource string from a given resource manager.
        ''' </summary>
        ''' <param name="strName">The name of the string resource.</param>
        ''' <param name="rm">The resource manager to load the string from.</param>
        ''' <param name="culture">The culture info, if any.</param>
        ''' <returns>A string, or <paramref name="strName"/> if an error occurred.</returns>
        ''' -------------------------------------------------------------------     
        Public Shared Function LoadString(strName As String, rm As ResourceManager,
                                          Optional culture As CultureInfo = Nothing) As String
            Dim strRes As String = ""
            If (culture Is Nothing) Then culture = Threading.Thread.CurrentThread.CurrentUICulture
            Try
                strRes = rm.GetString(strName, culture)
            Catch ex As Exception
                ' Whoah!
            End Try
            Return strRes

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns all the resources for the current culture.
        ''' </summary>
        ''' <param name="ass">The assembly to load the resources from. 
        ''' If left empty the current executing assembly is used.</param>
        ''' <param name="strNamespace">The namespace within the assembly, if any.
        ''' If left empty the name of the <paramref name="ass">provided assembly</paramref> 
        ''' is used.</param>
        ''' <param name="culture">The culture info, if any. If left empty the
        ''' current loaded culture is used.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetResources(Optional ass As Assembly = Nothing,
                                            Optional strNamespace As String = "",
                                            Optional culture As CultureInfo = Nothing) As Dictionary(Of String, Object)

            Dim dic As New Dictionary(Of String, Object)
            Dim e As System.Collections.IDictionaryEnumerator = Nothing
            Dim rm As ResourceManager = Nothing
            Dim strRes As String = ""

            ' Provide defaults
            If (ass Is Nothing) Then ass = Assembly.GetExecutingAssembly()
            If (culture Is Nothing) Then culture = Threading.Thread.CurrentThread.CurrentUICulture
            If (String.IsNullOrEmpty(strNamespace)) Then strNamespace = ass.GetName.Name

            rm = New ResourceManager(strNamespace & ".resources", ass)

            Using recset As Resources.ResourceSet = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, False, True)
                For Each entry As DictionaryEntry In recset
                    Dim strName As String = entry.Key.ToString
                    dic.Add(strName, My.Resources.ResourceManager.GetObject(strName))
                Next
            End Using

            Return dic

        End Function

    End Class

End Namespace
