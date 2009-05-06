'==============================================================================
'
' $Log: cEcospaceLayerMPASeed.vb,v $
' Revision 1.1  2009/05/06 12:32:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerMPASeed
    Inherits cEcospaceLayerIntegerNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerMPASeed, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerMPASeed
    End Sub

End Class
