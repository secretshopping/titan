using System;

namespace Prem.PTC
{
    public interface ITableObject
    {
        int Id { get; }
        Database Database { get; }
        bool IsInDatabase { get; }
        bool IsUpToDate { get; }
        void Save(bool forceSave = false);
        void Reload();
        void Delete();
    }
}