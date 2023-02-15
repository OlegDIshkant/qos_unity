using System;


namespace CGA
{
    /// <summary>
    /// To start a game (or any other <seealso cref="IAppCore"/>) the app generates one via the appropriate factory. 
    /// </summary>
    public interface IAppCoreFactory
    {
        IAppCore GenerateAppCore();
    }
}
