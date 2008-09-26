'==============================================================================
'
' $Log: ValueExplorer.vb,v $
' Revision 1.1  2008/09/26 07:32:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/06/02 00:01:44  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.3  2008/05/29 22:23:03  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.2  2008/01/25 03:01:23  jeroens
' Removed units
'
' Revision 1.1  2007/07/03 15:15:57  jeroens
' wtf
'
' Revision 1.4  2007/07/03 15:07:10  jeroens
' Renamed, once again
'
' Revision 1.2  2007/02/14 05:46:49  jeroens
' * Replaced annoying exceptions by smarter logic
'
' Revision 1.1  2006/10/02 16:14:54  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ValueExplorer

    Private Const cRESOURCE_ID As String = "CORE_VARIABLE_{0}"

    Private Enum eResourceSegment
        Name = 0
        Description
        Detailed
    End Enum

    Private Shared Function GetResource(ByVal varName As eVarNameFlags) As String
        Dim cI As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim strKey As String = String.Format(cRESOURCE_ID, cI.GetVarName(varName)).ToUpper()
        Return My.Resources.ResourceManager.GetString(strKey, My.Resources.Culture)
    End Function

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

    Shared Function GetName(ByVal varName As eVarNameFlags) As String
        Return GetResourceSegment(varName, eResourceSegment.Name)
    End Function

    Shared Function GetDescription(ByVal varName As eVarNameFlags) As String
        Return GetResourceSegment(varName, eResourceSegment.Description)
    End Function

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
