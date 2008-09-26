'==============================================================================
'
' $Log: IEcopathPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/03/14 00:50:08  jeroens
' - Restricted to data access interfaces now plugin-points are implemented as objects instead of function calls.
'
' Revision 1.2  2007/01/14 21:03:14  jeroens
' Discontinued iDatasourcePlugin
'
' Revision 1.1  2006/08/30 20:52:35  jeroens
' * Moved and/or created
'
' Revision 1.1  2006/08/20 21:20:06  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is invoked whenever an EwE
''' Ecopath model has been loaded or has been saved, but before the datasource is
''' closed.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execution interface for an Ecopath load model plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source from which
    ''' data is being loaded.</param>
    ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
    ''' plug-ins can respond to this event.</remarks>
    ''' -----------------------------------------------------------------------
    Sub LoadModel(ByVal dataSource As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execution interface for an Ecopath save model plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source to which
    ''' data is being saved.</param>
    ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
    ''' plug-ins can respond to this event.</remarks>
    ''' -----------------------------------------------------------------------
    Sub SaveModel(ByVal dataSource As Object)

End Interface
