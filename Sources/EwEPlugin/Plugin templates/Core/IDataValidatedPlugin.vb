Option Strict On
Imports EwEUtils.Core

''' ===========================================================================
''' <summary>
''' Interface for implementing plug-ins that extend value validation events with 
''' the EwE Core. Whenever a user modifies a value, this value is passed to the 
''' core for validation against allowed value ranges, against other existing 
''' values, etc. Users can decide to extend this process by adding custom tests.
''' </summary>
''' ===========================================================================
Public Interface IDataValidatedPlugin
    Inherits ICorePlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point called when the core has succesfully validated a variable.
    ''' </summary>
    ''' <param name="varname">The eVarname flag identifying the variable that 
    ''' passed Core validation.</param>
    ''' <param name="dt">The eDataTypes flag identifying the core source of the
    ''' variable.</param>
    ''' -----------------------------------------------------------------------
    Sub DataValidated(ByVal varname As eVarNameFlags, ByVal dt As eDataTypes)

End Interface
