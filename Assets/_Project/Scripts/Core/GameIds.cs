using System;

namespace FOIA.Core
{
    [Serializable]
    public readonly struct NodeId : IEquatable<NodeId>
    {
        public readonly string Value;

        public NodeId(string value) => Value = value;

        public bool Equals(NodeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NodeId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }

    [Serializable]
    public readonly struct EdgeId : IEquatable<EdgeId>
    {
        public readonly string Value;

        public EdgeId(string value) => Value = value;

        public bool Equals(EdgeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EdgeId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }

    [Serializable]
    public readonly struct EdgeBlockId : IEquatable<EdgeBlockId>
    {
        public readonly string Value;

        public EdgeBlockId(string value) => Value = value;

        public bool Equals(EdgeBlockId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EdgeBlockId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }

    [Serializable]
    public readonly struct ComplaintTypeId : IEquatable<ComplaintTypeId>
    {
        public readonly string Value;

        public ComplaintTypeId(string value) => Value = value;

        public bool Equals(ComplaintTypeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ComplaintTypeId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }

    [Serializable]
    public readonly struct StaffId : IEquatable<StaffId>
    {
        public readonly string Value;

        public StaffId(string value) => Value = value;

        public bool Equals(StaffId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is StaffId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }

    [Serializable]
    public readonly struct AgencyId : IEquatable<AgencyId>
    {
        public readonly string Value;

        public AgencyId(string value) => Value = value;

        public bool Equals(AgencyId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AgencyId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }
}
