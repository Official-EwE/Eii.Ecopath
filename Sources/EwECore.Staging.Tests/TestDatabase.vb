' SPDX-License-Identifier: EUPL-1.2
'
' Test-only concrete subclass of cEwEDatabase. The 12 abstract members are
' implemented trivially - none of them are exercised by the staging tests,
' which call StageLocalWorkingCopy/PublishLocalCopyToSource/
' CleanUpLocalWorkingCopy directly rather than going through Open()/Create()/
' SaveAs(). The only real purpose of this class is to make the Protected
' staging members (StageLocalWorkingCopy, CleanUpLocalWorkingCopy, and the
' two path fields) reachable from test code as Public - this is a standard
' pattern for testing Protected members, not something present in the real
' codebase.

Imports System.Data
Imports EwECore.Database

Public Class TestDatabase
    Inherits cEwEDatabase

    ' --- Public wrappers around Protected staging members ---

    Public Function Test_StageLocalWorkingCopy(strRealPath As String) As String
        Return Me.StageLocalWorkingCopy(strRealPath)
    End Function

    Public Sub Test_CleanUpLocalWorkingCopy()
        Me.CleanUpLocalWorkingCopy()
    End Sub

    Public ReadOnly Property Test_LocalWorkingCopyPath As String
        Get
            Return Me.m_strLocalWorkingCopyPath
        End Get
    End Property

    Public ReadOnly Property Test_RealFilePath As String
        Get
            Return Me.m_strRealFilePath
        End Get
    End Property

    ' --- Trivial implementations of the 12 abstract members ---
    ' None of these are exercised by the staging tests themselves.

    Private m_strName As String = ""

    Public Overrides Function Create(strDatabase As String, strModelName As String,
            Optional bOverwrite As Boolean = False,
            Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
            Optional strAuthor As String = "") As eDatasourceAccessType
        Return eDatasourceAccessType.Success
    End Function

    Public Overrides Function Open(strDatabase As String,
            Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
            Optional bReadOnly As Boolean = False) As eDatasourceAccessType
        Me.m_strName = strDatabase
        Return eDatasourceAccessType.Opened
    End Function

    Public Overrides ReadOnly Property Name As String
        Get
            Return Me.m_strName
        End Get
    End Property

    Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String,
            Optional bOverwrite As Boolean = False,
            Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
        Return eDatasourceAccessType.Success
    End Function

    Public Overrides Function MaxDBVersion() As Single
        Return 1.0F
    End Function

    Public Overrides Function Compact(strFileFrom As String, strFileTo As String) As eDatasourceAccessType
        Return eDatasourceAccessType.Success
    End Function

    Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean
        Return False
    End Function

    Public Overrides Function GetConnection() As IDbConnection
        Return Nothing
    End Function

    Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean
        Return True
    End Function

    Public Overrides ReadOnly Property Directory As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property FileName As String
        Get
            Return Me.m_strName
        End Get
    End Property

    Public Overrides ReadOnly Property Extension As String
        Get
            Return ""
        End Get
    End Property

End Class
