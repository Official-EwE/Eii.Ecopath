#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace ExternalData

    ''' ===========================================================================
    ''' <summary>
    ''' Interface for implementing external data sources.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IExternalData

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether a plug-in capable of delivering external data is available.
        ''' </summary>
        ''' <param name="runtype">The core run type to check availability for.</param>
        ''' <returns>True if available.</returns>
        ''' -----------------------------------------------------------------------
        Function IsDataAvailable(ByVal runtype As IRunType) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether data delivering plug-ins are supposed to be enabled or
        ''' disabled for a certain core run type.
        ''' </summary>
        ''' <param name="runtype">The core run type to check availability for.</param>
        ''' <returns>True if available.</returns>
        ''' -----------------------------------------------------------------------
        Property EnableData(ByVal runtype As IRunType) As Boolean

    End Interface

End Namespace ' ExternalData
