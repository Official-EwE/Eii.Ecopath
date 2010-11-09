''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing an Options page
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IOptionsPage

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating all possible results when applying the content
    ''' of an options page.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Enum eApplyResultType As Integer
        ''' <summary>Application was successful.</summary>
        Success
        ''' <summary>Application was successful but requires a restart.</summary>
        Success_restart
        ''' <summary>Application failed.</summary>
        Failed
        ''' <summary>No comment.</summary>
        ScrewedUpBeyondBeliefNowWeHaveToBloodyStartAllOverAgain
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Method to apply the content of an options page to the 'system'.
    ''' </summary>
    ''' <returns>An <see cref="eApplyResultType">apply result</see>.</returns>
    ''' -----------------------------------------------------------------------
    Function Apply() As eApplyResultType

End Interface
