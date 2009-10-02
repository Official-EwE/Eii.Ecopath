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
    ''' <returns>True if loaded succesful.</returns>
    ''' -----------------------------------------------------------------------
    Function LoadModel(ByVal dataSource As Object) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execution interface for an Ecopath save model plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source to which
    ''' data is being saved.</param>
    ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
    ''' plug-ins can respond to this event.</remarks>
    ''' -----------------------------------------------------------------------
    Function SaveModel(ByVal dataSource As Object) As Boolean

End Interface
