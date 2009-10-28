Option Strict On
Imports EwECore
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class, extracts descriptive details from the project resources about
''' <see cref="eVarNameFlags">core variables</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ValueExplorer

    ''' <summary>Resource identifier for locating variable descriptive entries</summary>
    Private Const cRESOURCE_ID As String = "CORE_VARIABLE_{0}"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, indicating which entries are expected 
    ''' in descriptive entries.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Enum eResourceSegment
        ''' <summary>Legible name for a variable.</summary>
        Name = 0
        ''' <summary>Description for a variable.</summary>
        Description
        ''' <summary>Detailed description for a variable.</summary>
        Detailed
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, locates a descriptive entry for a variable in the resources
    ''' </summary>
    ''' <param name="varName">The variable to find the descriptive entry for.</param>
    ''' <returns>
    ''' A descriptive entry, or an empty string if no description was found in the resources.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function GetResource(ByVal varName As eVarNameFlags) As String
        Dim cI As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim strKey As String = String.Format(cRESOURCE_ID, cI.GetVarName(varName)).ToUpper()
        Return My.Resources.ResourceManager.GetString(strKey, My.Resources.Culture)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, splits a descriptive entry into its 
    ''' <see cref="eResourceSegment">components</see>, and returns the requested
    ''' segment.
    ''' </summary>
    ''' <param name="varName">The varname to obtain the segment for.</param>
    ''' <param name="segment">The segment to obtain.</param>
    ''' <returns>
    ''' A descriptive entry segment, or an empty string if no segment was found.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function GetResourceSegment(ByVal varName As eVarNameFlags, ByVal segment As eResourceSegment) As String
        Dim strResource As String = GetResource(varName)
        Dim astrSplit As String() = Nothing
        Dim iSplit As Integer = 0
        Dim strSegment As String = ""

        If Not String.IsNullOrEmpty(strResource) Then
            astrSplit = strResource.Split(CChar("|"))
            iSplit = astrSplit.Length
        End If

        Select Case segment
            Case eResourceSegment.Name
                If iSplit > CInt(segment) Then
                    strSegment = astrSplit(CInt(segment))
                Else
                    strSegment = cCoreEnumNamesIndex.GetInstance().GetVarName(varName)
                End If
            Case eResourceSegment.Description
                If iSplit > CInt(segment) Then
                    strSegment = astrSplit(CInt(segment))
                End If
            Case eResourceSegment.Detailed
                If iSplit > CInt(segment) Then
                    strSegment = astrSplit(CInt(segment))
                End If
        End Select
        Return strSegment
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a human-readble name for a core variable.
    ''' </summary>
    ''' <param name="varName">The variable to return a name for.</param>
    ''' <returns>A name, or an empty string if no name was found.</returns>
    ''' -----------------------------------------------------------------------
    Shared Function GetName(ByVal varName As eVarNameFlags) As String
        Return GetResourceSegment(varName, eResourceSegment.Name)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a human-readble description for a core variable.
    ''' </summary>
    ''' <param name="varName">The variable to return a description for.</param>
    ''' <returns>A description, or an empty string if no description was found.</returns>
    ''' -----------------------------------------------------------------------
    Shared Function GetDescription(ByVal varName As eVarNameFlags) As String
        Return GetResourceSegment(varName, eResourceSegment.Description)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns detailed desciptions for a core variable.
    ''' </summary>
    ''' <param name="varName">The variable to return a detailed desciptions for.</param>
    ''' <returns>Detailed desciptions, or an empty string if no detailed desciptions were found.</returns>
    ''' -----------------------------------------------------------------------
    Shared Function GetDetailedDescription(ByVal varName As eVarNameFlags, ByVal source As cCoreInputOutputBase, _
            Optional ByVal sourceSec As cCoreInputOutputBase = Nothing) As String
        Dim strField As String = GetResourceSegment(varName, eResourceSegment.Detailed)

        If strField.IndexOf("{1}") > -1 Then
            Return String.Format(strField, source.Name, sourceSec.Name)
        End If

        If strField.IndexOf("{0}") > -1 Then
            Return String.Format(strField, source.Name)
        End If

        Return strField
    End Function

End Class
