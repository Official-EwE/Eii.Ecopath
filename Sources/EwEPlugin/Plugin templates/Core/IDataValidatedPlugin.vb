'==============================================================================
'
' $Log: IDataValidatedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:05  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/06 18:46:05  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

Public Interface IDataValidatedPlugin
    Inherits ICorePlugin

    Sub DataValidated(ByVal varname As eVarNameFlags, ByVal dt As eDataTypes)

End Interface
