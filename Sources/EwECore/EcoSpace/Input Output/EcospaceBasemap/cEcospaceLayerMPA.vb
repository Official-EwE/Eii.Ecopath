'==============================================================================
'
' $Log: cEcospaceLayerMPA.vb,v $
' Revision 1.1  2009/05/06 12:32:24  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerMPA
    Inherits cEcospaceLayerIntegerNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal meta As cVariableMetaData)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerMPA, cCore.NULL_VALUE, meta)
        Me.m_dataType = eDataTypes.EcospaceLayerMPA
    End Sub

End Class
