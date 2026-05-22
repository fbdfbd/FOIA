using System;

namespace OneMoreSpoon.Game.Core
{
    /// <summary>
    /// 게임 내 엔티티(Entity)를 식별하기 위한 고유 ID를 표현하는 값 타입입니다.
    /// 정수 기반의 가벼운 식별자로, 참조 대신 ID를 사용해 엔티티를 안전하게 다룰 수 있습니다.
    /// </summary>
    [Serializable]
    public readonly struct EntityId : IEquatable<EntityId>
    {
        /// <summary>
        /// 유효하지 않은(빈) 엔티티를 나타내는 상수입니다. (Value = 0)
        /// 초기값이나 "엔티티 없음"을 표현할 때 사용합니다.
        /// </summary>
        public static readonly EntityId Invalid = new EntityId(0);

        /// <summary>
        /// 엔티티의 실제 식별 값입니다. 외부에서는 읽기 전용으로만 접근할 수 있습니다.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// 이 EntityId가 유효한지 여부를 반환합니다.
        /// Value가 0보다 클 때만 유효한 엔티티로 간주합니다.
        /// </summary>
        public bool IsValid => Value > 0;

        /// <summary>
        /// 지정한 정수 값으로 EntityId를 생성합니다.
        /// </summary>
        /// <param name="value">엔티티 식별 값 (0은 Invalid로 취급)</param>
        public EntityId(int value)
        {
            Value = value;
        }

        /// <summary>
        /// 다른 EntityId와 값이 같은지 비교합니다. (IEquatable 구현)
        /// 박싱을 피해 성능에 유리합니다.
        /// </summary>
        public bool Equals(EntityId other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// object와의 동등성을 비교합니다.
        /// 대상이 EntityId일 때만 값 비교를 수행합니다.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        /// <summary>
        /// 해시 코드를 반환합니다.
        /// Dictionary, HashSet 등의 컬렉션 키로 안전하게 사용할 수 있도록 Value를 그대로 사용합니다.
        /// </summary>
        public override int GetHashCode()
        {
            return Value;
        }

        /// <summary>
        /// 디버깅 및 로그 출력을 위한 문자열 표현을 반환합니다.
        /// 유효하지 않은 경우 "Entity(Invalid)"로 표시됩니다.
        /// </summary>
        public override string ToString()
        {
            return IsValid ? $"Entity({Value})" : "Entity(Invalid)";
        }

        /// <summary>
        /// 두 EntityId가 같은지 비교합니다. (== 연산자)
        /// </summary>
        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 두 EntityId가 다른지 비교합니다. (!= 연산자)
        /// </summary>
        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }
    }
}