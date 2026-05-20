namespace FOIA.Core.Tags
{
    /// <summary>
    /// 게임 내에서 사용되는 태그 식별자.
    /// 부서, 사건 성격, 처리 단계 등을 분류하는 데 사용된다.
    /// </summary>
    public enum GameTag
    {
        /// <summary>태그 없음 (기본값).</summary>
        None = 0,

        // ─────────────────────────────
        // 부서 / 소속 카테고리
        // ─────────────────────────────

        /// <summary>군 관련 (국방부, 군 기록 등).</summary>
        Military,

        /// <summary>정보기관 관련 (첩보, 기밀 수집 등).</summary>
        Intelligence,

        /// <summary>보건·의료 관련 (보건복지부, 병원 기록 등).</summary>
        Health,

        /// <summary>치안·경찰 관련 (수사, 공공 안전).</summary>
        Security,

        /// <summary>기록·문서 관리 관련 (공문서, 아카이브).</summary>
        Records,

        // ─────────────────────────────
        // 사건 성격 카테고리
        // ─────────────────────────────

        /// <summary>실종자 사건.</summary>
        MissingPerson,

        /// <summary>기밀 분류 정보가 포함된 사건.</summary>
        Classified,

        /// <summary>부패·비리 관련 사건.</summary>
        Corruption,

        /// <summary>공중 보건 위협 사건 (전염병, 식품 안전 등).</summary>
        PublicHealth,

        /// <summary>언론에 노출될 위험이 큰 민감 사건.</summary>
        MediaRisk,

        // ─────────────────────────────
        // 처리 단계 카테고리
        // ─────────────────────────────

        /// <summary>접수 단계 — 청구가 처음 들어온 상태.</summary>
        Intake,

        /// <summary>법률 검토 단계 — 법적 근거나 제약 확인.</summary>
        Legal,

        /// <summary>조사·점검 단계 — 관련 기관에서 자료 확인.</summary>
        Inspection,

        /// <summary>공개 단계 — 정보가 청구인에게 공개됨.</summary>
        Disclosure,

        /// <summary>압박 단계 — 외부 요인(언론, 정치 등)으로 처리에 압력이 가해지는 상태.</summary>
        Pressure,
    }
}