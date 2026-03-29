# Player - Warrior 상속 관계 검토 문서

**작성일**: 2024
**프로젝트**: WarriorQuest
**목적**: 학습용 프로젝트의 상속 구조 검토

---

## 📋 목차
1. [현재 구조 개요](#현재-구조-개요)
2. [발견된 문제점](#발견된-문제점)
3. [좋은 점](#좋은-점)
4. [개선 권장사항](#개선-권장사항)
5. [결론](#결론)

---

## 현재 구조 개요

### Player 클래스 (추상 클래스)
```
Player (abstract) : MonoBehaviour, IDamageable
├── 기본 스탯: MaxHP, CurrentHP, MoveSpeed, AttackDamage, AttackCooldown
├── 컴포넌트: Rigidbody2D, Animator, SpriteRenderer, InputHandler
├── 공통 메서드: TakeDamage(), Die(), FlipSprite()
└── 추상 메서드: Attack()
```

### Warrior 클래스 (구체 클래스)
```
Warrior : Player
├── 전사 전용 스탯: Defense
├── 오버라이드: Awake(), Attack(), TakeDamage()
└── 스탯 재정의: MaxHP=150, MoveSpeed=4, AttackDamage=25, AttackCooldown=0.7
```

---

## 🚨 발견된 문제점

### 1. **[치명적] InputHandler의 OnAttack 콜백이 비어있음**
**위치**: `Assets\@Script\InputSystem\InputHandler.cs` (Line 75-78)

```csharp
private void OnAttack(InputAction.CallbackContext context)
{
    // 비어있음! 이벤트를 발생시키지 않음
}
```

**문제**: `OnAttackAction` 이벤트를 Invoke하지 않아서 공격 입력이 Player에게 전달되지 않습니다.

**해결 방법**:
```csharp
private void OnAttack(InputAction.CallbackContext context)
{
    OnAttackAction?.Invoke();
}
```

---

### 2. **[중요] Warrior.Awake()에서 CurrentHP 재설정 누락**
**위치**: `Assets\@Script\Character\Player\Warrior.cs` (Line 16-25)

**문제**:
```csharp
protected override void Awake()
{
    base.Awake();  // CurrentHP = MaxHP (100)로 설정됨
    MaxHP = 150f;  // MaxHP만 150으로 변경
    // CurrentHP는 여전히 100으로 남아있음!
}
```

**결과**: MaxHP는 150이지만 CurrentHP는 100으로 시작합니다.

**해결 방법**:
```csharp
protected override void Awake()
{
    base.Awake();
    MaxHP = 150f;
    CurrentHP = MaxHP; // 이 줄 추가!
    MoveSpeed = 4f;
    AttackDamage = 25f;
    AttackCooldown = .7f;
    
    Debug.Log($"Warrior Awake! 방어력 :{Defense} ");
}
```

---

### 3. **[중요] Warrior.TakeDamage()에서 Defense가 적용되지 않음**
**위치**: `Assets\@Script\Character\Player\Warrior.cs` (Line 34-39)

**문제**:
```csharp
public override void TakeDamage(float damage)
{
    float ActualDamage = Mathf.Max(damage - Defense, 5f); // 계산만 하고
    Debug.Log($"Warrior takes {ActualDamage} damage after defense!");
    base.TakeDamage(damage); // 원래 damage를 그대로 전달!
}
```

**결과**: Defense를 계산하지만 실제로는 적용되지 않습니다.

**해결 방법**:
```csharp
public override void TakeDamage(float damage)
{
    float ActualDamage = Mathf.Max(damage - Defense, 5f);
    Debug.Log($"Warrior takes {ActualDamage} damage after defense!");
    base.TakeDamage(ActualDamage); // ActualDamage 전달!
}
```

---

### 4. **[보통] OnEnable의 IsDead 체크가 불필요**
**위치**: `Assets\@Script\Character\Player\Player.cs` (Line 73-81)

**문제**:
```csharp
protected void OnEnable()
{
    if (!IsDead) // 이 체크는 불필요함
    {
        InputHandler.OnMoveAction += OnMove;
        // ...
    }
}
```

**이유**:
- `OnEnable()`은 처음 활성화될 때 실행되는데, 이때는 항상 살아있는 상태입니다
- 만약 죽은 후 다시 활성화된다면 `OnDisable()`에서 이벤트 구독 해제가 이미 됨
- 이 체크로 인해 죽은 상태에서 활성화되면 이벤트 구독이 안 되고, `OnDisable()`에서는 구독 해제를 시도해 불균형 발생

**해결 방법**: `if (!IsDead)` 체크를 제거하거나, `OnDisable()`에도 동일한 체크를 추가

---

### 5. **[보통] static 이벤트 사용으로 인한 잠재적 문제**
**위치**: `Assets\@Script\InputSystem\InputHandler.cs` (Line 32-34)

**문제**:
```csharp
public static event Action<Vector2> OnMoveAction;
public static event Action OnAttackAction;
public static event Action<bool> OnIntractAction;
```

**문제점**:
- static 이벤트는 모든 InputHandler 인스턴스가 공유합니다
- 멀티플레이어나 여러 플레이어가 있는 경우 문제 발생 가능
- 메모리 누수 위험: static 이벤트는 자동으로 정리되지 않음

**해결 방법**: 
- 현재 싱글플레이어 학습용이라면 괜찮음
- 추후 멀티플레이어로 확장 시 non-static으로 변경 필요

---

### 6. **[보통] Awake()에서 MaxHP 설정 후 CurrentHP 재설정 타이밍 문제**
**위치**: `Assets\@Script\Character\Player\Warrior.cs`

**문제**:
```csharp
protected override void Awake()
{
    base.Awake();  // 1. CurrentHP = MaxHP (100)
    MaxHP = 150f;  // 2. MaxHP = 150
    // 3. CurrentHP는 여전히 100
}
```

**개선 방법 (Alternative)**:
자식 클래스에서 스탯을 먼저 설정하고 base.Awake() 호출:
```csharp
protected override void Awake()
{
    MaxHP = 150f;
    MoveSpeed = 4f;
    AttackDamage = 25f;
    AttackCooldown = .7f;
    
    base.Awake(); // CurrentHP = MaxHP (150) 자동 설정
    Debug.Log($"Warrior Awake! 방어력 :{Defense} ");
}
```

---

### 7. **[낮음] FlipSprite가 private으로 선언됨**
**위치**: `Assets\@Script\Character\Player\Player.cs` (Line 99-110)

**문제**: 자식 클래스에서 접근 불가능

**개선**: 
```csharp
protected void FlipSprite(bool facingRight) // private → protected
```

---

### 8. **[낮음] ArmTransform null 체크 누락**
**위치**: `Assets\@Script\Character\Player\Player.cs` (Line 71)

**문제**:
```csharp
ArmTransform = transform.Find("Arm"); // "Arm"이 없으면 null
```

`FlipSprite()`에서 `ArmTransform.localRotation`을 호출할 때 NullReferenceException 발생 가능

**개선**:
```csharp
ArmTransform = transform.Find("Arm");
if (ArmTransform == null)
{
    Debug.LogWarning($"[{gameObject.name}] 'Arm' child transform not found!");
}
```

그리고 `FlipSprite()`에서:
```csharp
private void FlipSprite(bool facingRight)
{
    if (facingRight)
    {
        SpriteRenderer.flipX = false;
        if (ArmTransform != null)
            ArmTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    else
    {
        SpriteRenderer.flipX = true;
        if (ArmTransform != null)
            ArmTransform.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
```

---

### 9. **[낮음] 공격 쿨다운이 구현되지 않음**
**위치**: `Assets\@Script\Character\Player\Player.cs`

**문제**: `AttackCooldown` 변수가 선언되어 있지만, 실제로 쿨다운을 체크하는 로직이 없습니다.

**개선 예시**:
```csharp
private float lastAttackTime = -999f;

private void OnAttack()
{
    if (Time.time - lastAttackTime < AttackCooldown) return;
    
    lastAttackTime = Time.time;
    Anim.SetTrigger(HashAttack);
    Attack();
}
```

---

### 10. **[낮음] Die() 메서드에서 이벤트 구독 해제 누락**
**위치**: `Assets\@Script\Character\Player\Player.cs` (Line 127-131)

**문제**: 플레이어가 죽어도 입력 이벤트가 계속 구독되어 있습니다.

**개선**:
```csharp
protected virtual void Die()
{
    CurrentHP = 0;
    Debug.Log("플레이어 사망");
    
    // 입력 이벤트 구독 해제
    InputHandler.OnMoveAction -= OnMove;
    InputHandler.OnAttackAction -= OnAttack;
    InputHandler.OnIntractAction -= OnIntract;
}
```

---

## ✅ 좋은 점

### 1. **명확한 추상화**
- `Player`를 추상 클래스로 만들어 공통 기능을 정의
- `Attack()` 메서드를 추상 메서드로 선언해 자식 클래스에서 반드시 구현하도록 강제

### 2. **적절한 접근 제한자 사용**
- `protected`를 사용해 자식 클래스에서 접근 가능하도록 설정
- 외부에서 직접 수정하면 안 되는 필드들을 적절히 보호

### 3. **애니메이션 해시 최적화**
```csharp
protected static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
```
- 문자열 비교 대신 해시값을 사용해 성능 최적화

### 4. **이벤트 기반 입력 처리**
- Input System을 이벤트 기반으로 처리해 결합도를 낮춤
- 확장성이 좋은 구조

### 5. **인터페이스 활용**
- `IDamageable` 인터페이스를 구현해 다형성 활용 가능
- 적, NPC 등 다른 대미지를 받는 객체와 일관된 인터페이스 제공

### 6. **Unity 생명주기 메서드 활용**
- `Awake()`, `OnEnable()`, `OnDisable()`을 적절히 활용
- 이벤트 구독/해제를 `OnEnable/OnDisable`에서 처리하는 것은 모범 사례

### 7. **Region을 사용한 코드 정리**
- 코드를 논리적 영역으로 나누어 가독성 향상

---

## 🔧 개선 권장사항

### 우선순위: 높음

#### 1. InputHandler.OnAttack 콜백 구현
```csharp
// InputHandler.cs
private void OnAttack(InputAction.CallbackContext context)
{
    OnAttackAction?.Invoke(); // 이벤트 발생!
}
```

#### 2. Warrior.Awake()에서 CurrentHP 재설정
```csharp
// Warrior.cs
protected override void Awake()
{
    base.Awake();
    MaxHP = 150f;
    CurrentHP = MaxHP; // 추가!
    MoveSpeed = 4f;
    AttackDamage = 25f;
    AttackCooldown = .7f;
    
    Debug.Log($"Warrior Awake! 방어력 :{Defense}");
}
```

#### 3. Warrior.TakeDamage()에서 실제 데미지 적용
```csharp
// Warrior.cs
public override void TakeDamage(float damage)
{
    float ActualDamage = Mathf.Max(damage - Defense, 5f);
    Debug.Log($"Warrior takes {ActualDamage} damage after defense!");
    base.TakeDamage(ActualDamage); // ActualDamage 전달!
}
```

---

### 우선순위: 중간

#### 4. 공격 쿨다운 로직 구현
```csharp
// Player.cs
private float lastAttackTime = -999f;

private void OnAttack()
{
    if (Time.time - lastAttackTime < AttackCooldown) return;
    
    lastAttackTime = Time.time;
    Anim.SetTrigger(HashAttack);
    Attack();
}
```

#### 5. OnEnable의 IsDead 체크 제거 또는 일관성 유지
```csharp
// Player.cs - 옵션 1: 체크 제거 (권장)
protected void OnEnable()
{
    InputHandler.OnMoveAction += OnMove;
    InputHandler.OnAttackAction += OnAttack;
    InputHandler.OnIntractAction += OnIntract;
}

// Player.cs - 옵션 2: 일관성 유지
protected void OnDisable()
{
    if (!IsDead)
    {
        InputHandler.OnMoveAction -= OnMove;
        InputHandler.OnAttackAction -= OnAttack;
        InputHandler.OnIntractAction -= OnIntract;
    }
}
```

#### 6. Die() 메서드에서 이벤트 구독 해제
```csharp
// Player.cs
protected virtual void Die()
{
    CurrentHP = 0;
    Debug.Log("플레이어 사망");
    
    // 입력 비활성화
    InputHandler.OnMoveAction -= OnMove;
    InputHandler.OnAttackAction -= OnAttack;
    InputHandler.OnIntractAction -= OnIntract;
    
    // Rigidbody 정지
    RB.linearVelocity = Vector2.zero;
}
```

---

### 우선순위: 낮음

#### 7. ArmTransform null 체크 추가
```csharp
// Player.cs - Awake()
ArmTransform = transform.Find("Arm");
if (ArmTransform == null)
{
    Debug.LogWarning($"[{gameObject.name}] 'Arm' child transform not found!");
}

// Player.cs - FlipSprite()
private void FlipSprite(bool facingRight)
{
    if (facingRight)
    {
        SpriteRenderer.flipX = false;
        if (ArmTransform != null)
            ArmTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    else
    {
        SpriteRenderer.flipX = true;
        if (ArmTransform != null)
            ArmTransform.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
```

#### 8. FlipSprite를 protected로 변경
```csharp
protected void FlipSprite(bool facingRight) // private → protected
```

#### 9. 스탯 설정 방식 개선 (Alternative)
현재는 Warrior.Awake()에서 스탯을 설정하지만, 더 명확한 방법은:

**옵션 A: 생성자 활용 (Unity에서는 권장하지 않음)**
```csharp
// 사용하지 마세요 - Unity는 생성자 사용을 권장하지 않음
```

**옵션 B: 초기화 메서드 분리**
```csharp
// Player.cs
protected virtual void InitializeStats()
{
    CurrentHP = MaxHP;
}

protected virtual void Awake()
{
    InitializeStats();
    // 컴포넌트 캐싱...
}

// Warrior.cs
protected override void InitializeStats()
{
    MaxHP = 150f;
    MoveSpeed = 4f;
    AttackDamage = 25f;
    AttackCooldown = .7f;
    base.InitializeStats(); // CurrentHP = MaxHP
}
```

**옵션 C: 현재 방식 유지하고 명시적 설정 (가장 간단)**
```csharp
// Warrior.cs - 현재 코드에서 CurrentHP만 추가
protected override void Awake()
{
    base.Awake();
    MaxHP = 150f;
    CurrentHP = MaxHP; // 명시적으로 설정
    // ...
}
```

---

## 📊 상속 구조 평가

### 설계 원칙 준수 여부

| 원칙 | 상태 | 설명 |
|------|------|------|
| **단일 책임 원칙 (SRP)** | ✅ 양호 | Player는 캐릭터 기능, InputHandler는 입력 처리로 분리 |
| **개방-폐쇄 원칙 (OCP)** | ✅ 양호 | 추상 클래스와 virtual 메서드로 확장에 열려있음 |
| **리스코프 치환 원칙 (LSP)** | ⚠️ 주의 | Warrior.TakeDamage()가 Defense를 적용하지 않아 기대와 다름 |
| **인터페이스 분리 원칙 (ISP)** | ✅ 양호 | IDamageable 인터페이스가 간단명료 |
| **의존성 역전 원칙 (DIP)** | ✅ 양호 | IDamageable 인터페이스를 통한 추상화 |

---

## 🎓 학습 관점 평가

### 잘 구현된 학습 포인트
1. ✅ **추상 클래스와 상속 구조**
2. ✅ **인터페이스 구현**
3. ✅ **이벤트 기반 아키텍처**
4. ✅ **Unity의 새로운 Input System 활용**
5. ✅ **컴포넌트 기반 설계 (RequireComponent)**
6. ✅ **접근 제한자의 적절한 사용**

### 개선이 필요한 학습 포인트
1. ⚠️ **virtual 메서드 오버라이드 시 base 호출 타이밍**
2. ⚠️ **이벤트 구독/해제의 생명주기 관리**
3. ⚠️ **static 이벤트의 장단점 이해**
4. ⚠️ **null 체크와 방어적 프로그래밍**

---

## 결론

### 전체 평가: **⭐⭐⭐⭐☆ (4/5)**

**종합 의견**:
- 학습용 프로젝트로서 **상속 구조는 전반적으로 우수**합니다
- 추상 클래스, 인터페이스, 이벤트 등 객체지향 개념을 잘 적용했습니다
- 몇 가지 **로직 버그**만 수정하면 완성도 높은 코드가 될 것입니다

### 치명적 문제
- ❌ **InputHandler.OnAttack이 비어있어 공격이 작동하지 않음** (즉시 수정 필요)

### 권장 수정 순서
1. **즉시 수정**: InputHandler.OnAttack 콜백 구현
2. **즉시 수정**: Warrior.Awake에서 CurrentHP 재설정
3. **즉시 수정**: Warrior.TakeDamage에서 ActualDamage 적용
4. **선택 사항**: 공격 쿨다운 로직 구현
5. **선택 사항**: null 체크 및 방어 코드 추가

### 추후 확장 시 고려사항
- 플레이어 클래스가 늘어나면 (예: Mage, Archer) 현재 구조로 충분히 확장 가능
- 멀티플레이어 구현 시 InputHandler의 static 이벤트 재검토 필요
- 스탯 관리 시스템이 복잡해지면 별도의 StatManager 클래스 고려

---

## 📚 학습 참고자료

### Unity 생명주기
```
생성: Awake() → OnEnable() → Start()
실행: FixedUpdate() → Update() → LateUpdate()
종료: OnDisable() → OnDestroy()
```

### 이벤트 구독/해제 모범 사례
- **구독**: `OnEnable()` 또는 `Start()`
- **해제**: `OnDisable()` 또는 `OnDestroy()`
- **항상 구독과 해제를 쌍으로 처리**

### virtual vs abstract
- `virtual`: 기본 구현 제공, 오버라이드 선택 사항
- `abstract`: 구현 없음, 자식 클래스에서 반드시 구현

---

**검토 완료**: 전반적으로 좋은 학습용 코드입니다. 위의 문제점들을 수정하면 더욱 견고한 코드가 될 것입니다!
