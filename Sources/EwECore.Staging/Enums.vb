' SPDX-License-Identifier: EUPL-1.2
'
' STUB - these are real enums in the actual EwECore codebase, defined
' elsewhere (not part of the files used in this isolated staging-only test
' project). Reproduced here with the member names actually referenced by
' cEwEDatabase.vb's trimmed staging code and by the test project's own
' minimal concrete subclass - not a claim that this is the complete,
' real set of members the actual enums contain.

Namespace Database

    Public Enum eDataSourceTypes
        NotSet
        Access2003
        Access2007
        EII
        EIIXML
    End Enum

    Public Enum eDatasourceAccessType
        Success
        Opened
        Failed_Unknown
        Failed_FileNotFound
        Failed_AlreadyInUse
        Failed_CannotSave
        Failed_UnknownType
        Failed_TransferTypes
        Failed_OSUnsupported
        Failed_ReadOnly
    End Enum

End Namespace
