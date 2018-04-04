using Prem.PTC.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Prem.PTC
{
    /// <summary>
    /// The only class where one can obtain parsers to connect to database.
    /// Parsers should be out of ParserPool for as short period of time as possible 
    /// (acquire parser -> then use it -> then release)
    /// </summary>
    public static class ParserPool
    {
        /// <summary>
        /// Returns parser wrapper which should be used according to needs
        /// and released (via ParserPool.Release(...))
        /// </summary>
        /// <param name="type">Type of database (Client, Server) you want to connect</param>
        /// <param name="autoOpenConnection">if true - ParserPool returns open connection parser, closed parser otherwise</param>
        /// <returns></returns>
        public static ParserDisposableWrapper Acquire(Database type, bool autoOpenConnection = true)
        {
            ParserDisposableWrapper wrapper = new ParserDisposableWrapper(new Parser(type));
            if (autoOpenConnection) wrapper.Instance.Open();

            return wrapper;
        }

        /// <summary>
        /// Receives not-needed-anymore parser, handles db connection close if not closed
        /// </summary>
        /// <param name="parser">Parser to release.</param>
        public static void Release(Parser parser)
        {
            //if (parser.Conn.State != ConnectionState.Closed)
            parser.Conn.Close(); // ensure that parser is closed
        }

        /// <summary>
        /// Receives not-needed-anymore parser (as a wrapper), handles db connection close if not closed
        /// </summary>
        /// <param name="wrapper"></param>
        public static void Release(ParserDisposableWrapper wrapper)
        {
            Release(wrapper.Instance);
        }
    }


    /// <summary>
    /// Wrapper class automatically disposing parser to parser pool.
    /// Should be decorator pattern.
    /// </summary>
    public class ParserDisposableWrapper : Utils.ObjectWrapper<Parser>, IDisposable
    {
        public ParserDisposableWrapper(Parser instance) : base(instance) { }

        public void Dispose()
        {
            ParserPool.Release(Instance);
        }
    }
}