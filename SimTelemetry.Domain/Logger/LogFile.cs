using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Logger
{
    public class LogFile : ILogNode
    {
        //
        public IEnumerable<ILogField> Fields { get { return new List<ILogField>(); } }

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        private readonly IList<LogGroup> _groups = new List<LogGroup>();

        private uint _groupId;
        private uint _fieldId = 0;

        #region House keeping methods
        public LogError Add(LogGroup group)
        {
            if (Groups.Any(x => x.ID == group.ID || x.Name == group.Name))
                return LogError.GroupAlreadyExists;
            _groups.Add(group);
            return LogError.OK;
        }

        public LogError Remove(LogGroup group)
        {
            if (!Groups.Any(x => x.ID == group.ID))
                return LogError.GroupNotFound;
            _groups.Remove(group);
            return LogError.OK;
        }

        public LogError Remove(string name)
        {
            var result = Groups.Where(x => x.Name == name).FirstOrDefault();
            if (result == null)
                return LogError.GroupNotFound;
            _groups.Remove(result);
            return LogError.OK;
        }

        protected uint GetFieldId(int group, string name)
        {
            var oGroup = Groups.Where(x => x.ID == group).FirstOrDefault();

            if (oGroup == null)
                return (uint)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (uint)LogError.FieldNotFound;
            else
                return oField.ID;

        }

        protected uint GetFieldId(string group, string name)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();

            if (oGroup == null)
                return (uint)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (uint)LogError.FieldNotFound;
            else
                return oField.ID;

        }

        protected uint GetGroupId(string group)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();
            if (oGroup == null)
                return (uint) LogError.GroupNotFound;
            else
                return oGroup.ID;
        }

        public LogGroup CreateGroup(string name)
        {
            var oGroup = new LogGroup(RequestNewGroupId(), name, this);
            Add(oGroup);
            return oGroup;
        }

        public uint RequestNewGroupId()
        {
            return _groupId++;
        }

        public uint RequestNewFieldId()
        {
            return _fieldId++;
        }
        #endregion


        public LogFile(string file)
        {
            // Open it
        }

        public LogFile()
        {
            // New start

        }

        public int dataSize = 0;
        public void Write(string group, string field, byte[] data)
        {
            dataSize += data.Length + 6;

        }
    }
}