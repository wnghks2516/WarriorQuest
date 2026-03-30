## 1. 전체 클래스 다이어그램

```mermaid
classDiagram
    class MonoBehaviour {
        <<Unity>>
    }
    
    class IDamageable {
        <<interface>>
        +TakeDamage(float damage)
    }
    
    class InputHandler {
        <<Component>>
        -InputSystem_Actions inputSystemAction
        -InputAction MoveAction
        -InputAction AttackAction
        -InputAction IntractAction
        +OnMoveAction$ Action~Vector2~
        +OnAttackAction$ Action
        +OnIntractAction$ Action~bool~
        -Awake()
        -OnEnable()
        -OnDisable()
        -OnMove(CallbackContext)
        -OnAttack(CallbackContext)
        -OnIntract(CallbackContext)
    }
    
    class Player {
        <<abstract>>
        #float MaxHP
        #float CurrentHP
        #float MoveSpeed
        #float AttackDamage
        #float AttackCooldown
        #bool IsDead
        #Rigidbody2D RB
        #Animator Anim
        #SpriteRenderer SpriteRenderer
        #InputHandler InputHandler
        #Transform ArmTransform
        #int HashIsMoving$
        #int HashAttack$
        #int HashHit$
        +float GetMaxHP
        +float GetCurrentHP
        +float GetMoveSpeed
        +float GetAttackDamage
        +float GetAttackCooldown
        #Awake()
        #OnEnable()
        #OnDisable()
        -FlipSprite(bool)
        +TakeDamage(float)
        #Die()
        -OnMove(Vector2)
        -OnAttack()
        -OnIntract(bool)
        #Attack()* abstract
    }
    
    class Warrior {
        -float Defense
        #Awake()
        #Attack()
        +TakeDamage(float)
    }
    
    MonoBehaviour <|-- Player
    MonoBehaviour <|-- InputHandler
    IDamageable <|.. Player
    Player <|-- Warrior
    Player o-- InputHandler
    Player o-- Rigidbody2D
    Player o-- Animator
    Player o-- SpriteRenderer
    InputHandler ..> Player
```

---

## 2. 상속 계층 구조

```mermaid
graph TD
    A[MonoBehaviour<br/>Unity Framework] --> B[Player<br/>Abstract Class]
    B --> C[Warrior<br/>Concrete Class]
    D[IDamageable<br/>Interface] -.implements.-> B
    
    style A fill:#e1f5ff
    style B fill:#fff4e1
    style C fill:#e8f5e8
    style D fill:#ffe1f5
```

---

## 3. 메서드 오버라이딩 관계

```mermaid
graph LR
    subgraph Player
        A[Awake<br/>virtual]
        B[TakeDamage<br/>virtual]
        C[Die<br/>virtual]
        D[Attack<br/>abstract]
    end
    
    subgraph Warrior
        E[Awake<br/>override]
        F[TakeDamage<br/>override]
        G[Attack<br/>override]
    end
    
    A --> E
    B --> F
    D --> G
    
    style A fill:#ffcccc
    style D fill:#ff9999
    style E fill:#ccffcc
    style F fill:#ccffcc
    style G fill:#99ff99
```

---

## 4. 이벤트 흐름도

```mermaid
sequenceDiagram
    participant User
    participant InputSystem
    participant InputHandler
    participant Player
    participant Animator
    
    User->>InputSystem: Keyboard/Mouse Input
    InputSystem->>InputHandler: InputAction Trigger
    
    alt Move Input
        InputHandler->>InputHandler: OnMove(CallbackContext)
        InputHandler->>Player: OnMoveAction Event
        Player->>Player: OnMove(Vector2)
        Player->>Player: FlipSprite()
        Player->>Animator: SetBool(IsMoving)
    end
    
    alt Attack Input
        InputHandler->>InputHandler: OnAttack(CallbackContext)
        Note right of InputHandler: EMPTY METHOD
        InputHandler--xPlayer: OnAttackAction NOT Invoked
    end
    
    alt Interact Input
        InputHandler->>InputHandler: OnIntract(CallbackContext)
        InputHandler->>Player: OnIntractAction Event
        Player->>Player: OnIntract(bool)
    end
```

---

## 5. 컴포넌트 의존성

```mermaid
graph TB
    subgraph GameObject
        W[Warrior]
        IH[InputHandler]
        RB[Rigidbody2D]
        AN[Animator]
        SR[SpriteRenderer]
        ARM[Arm Transform]
    end
    
    W -->|GetComponent| IH
    W -->|GetComponent| RB
    W -->|GetComponent| AN
    W -->|GetComponent| SR
    W -->|Find| ARM
    IH -.->|static event| W
    
    style W fill:#90EE90
    style IH fill:#FFD700
    style RB fill:#87CEEB
    style AN fill:#DDA0DD
    style SR fill:#F0E68C
    style ARM fill:#FFB6C1
```

---

## 6. 생명주기 다이어그램

```mermaid
sequenceDiagram
    participant Unity
    participant Warrior
    participant Player
    participant InputHandler
    
    Note over Unity: GameObject Created
    
    Unity->>Warrior: Awake()
    Warrior->>Player: base.Awake()
    Player->>Player: CurrentHP = MaxHP
    Player->>Player: Cache Components
    Player->>InputHandler: GetComponent
    Player-->>Warrior: Return
    Warrior->>Warrior: MaxHP = 150f
    Warrior->>Warrior: MoveSpeed = 4f
    Warrior->>Warrior: Set Other Stats
    
    Note over Unity: GameObject Enabled
    
    Unity->>InputHandler: OnEnable()
    InputHandler->>InputHandler: Enable Input System
    InputHandler->>InputHandler: Register Callbacks
    
    Unity->>Player: OnEnable()
    Player->>InputHandler: Subscribe Events
    
    Note over Unity: Game Running
    
    Note over Unity: GameObject Disabled
    
    Unity->>Player: OnDisable()
    Player->>InputHandler: Unsubscribe Events
    
    Unity->>InputHandler: OnDisable()
    InputHandler->>InputHandler: Disable Input System
```

---

## 7. 데미지 처리 흐름

```mermaid
flowchart TD
    Start([Enemy Attack]) --> W[Warrior.TakeDamage]
    W --> Calc[ActualDamage = Max<br/>damage - Defense, 5]
    Calc --> Log[Debug.Log]
    Log --> Base[base.TakeDamage<br/>damage]
    Base --> P[Player.TakeDamage]
    P --> Dead{IsDead?}
    Dead -->|Yes| Return[Return]
    Dead -->|No| Damage[CurrentHP -= damage]
    Damage --> Anim[Anim.SetTrigger Hit]
    Anim --> Check{CurrentHP <= 0?}
    Check -->|Yes| Die[Die Method]
    Check -->|No| End([End])
    Die --> End
    Return --> End
    
    style W fill:#90EE90
    style P fill:#FFE4B5
    style Die fill:#FFB6C1
```

---

## 8. 스탯 초기화 흐름

```mermaid
flowchart TD
    Start([Warrior Created]) --> WAwake[Warrior.Awake]
    WAwake --> BaseAwake[base.Awake]
    BaseAwake --> PInit[Player.Awake]
    PInit --> Set1[CurrentHP = MaxHP<br/>150 = 150]
    Set1 --> Cache[Cache Components]
    Cache --> Return[Return to Warrior]
    Return --> Set2[MaxHP = 150f]
    Set2 --> Set3[MoveSpeed = 4f]
    Set3 --> Set4[AttackDamage = 25f]
    Set4 --> Set5[AttackCooldown = 0.7f]
    Set5 --> End([Complete])
```

---

## 9. RequireComponent 의존성

```mermaid
graph TB
    P[Player<br/>MonoBehaviour]
    
    P -->|RequireComponent| RB[Rigidbody2D]
    P -->|RequireComponent| AN[Animator]
    P -->|RequireComponent| SR[SpriteRenderer]
    P -->|RequireComponent| IH[InputHandler]
    
    W[Warrior]
    W -.inherits.-> P
    W -.auto requires.-> RB
    W -.auto requires.-> AN
    W -.auto requires.-> SR
    W -.auto requires.-> IH
    
    style P fill:#FFE4B5
    style W fill:#90EE90
    style RB fill:#E0E0E0
    style AN fill:#E0E0E0
    style SR fill:#E0E0E0
    style IH fill:#FFD700
```

---

## 10. 공격 시나리오

```mermaid
sequenceDiagram
    participant User
    participant UnityInputSystem
    participant InputHandler
    participant Player
    participant Warrior
    participant Animator
    
    User->>UnityInputSystem: Attack Key
    UnityInputSystem->>InputHandler: AttackAction.performed
    InputHandler->>InputHandler: OnAttack(CallbackContext)
    InputHandler--xPlayer: OnAttackAction NOT Invoked
    Player--xPlayer: OnAttack() NOT Called
    Player--xAnimator: SetTrigger NOT Called
    Player--xWarrior: Attack() NOT Called
```

---

## 11. 데미지 처리 시나리오

```mermaid
sequenceDiagram
    participant Enemy
    participant Warrior
    participant Player
    participant Animator
    
    Enemy->>Warrior: TakeDamage(30f)
    Warrior->>Warrior: ActualDamage = 20
    Warrior->>Warrior: Debug.Log
    Warrior->>Player: base.TakeDamage(30f)
    Player->>Player: CurrentHP -= 30
    Player->>Animator: SetTrigger(Hit)
    
    alt Dead
        Player->>Player: Die()
    end
```

---

## 12. 이벤트 구독 관계도

```mermaid
graph LR
    subgraph InputHandler_Events
        E1[OnMoveAction]
        E2[OnAttackAction]
        E3[OnIntractAction]
    end
    
    subgraph Player_Methods
        M1[OnMove]
        M2[OnAttack]
        M3[OnIntract]
    end
    
    subgraph InputHandler_Callbacks
        C1[OnMove]
        C2[OnAttack<br/>EMPTY]
        C3[OnIntract]
    end
    
    C1 -.Invoke.-> E1
    C2 -.NOT Invoke.-> E2
    C3 -.Invoke.-> E3
    
    E1 -.Subscribe.-> M1
    E2 -.Subscribe.-> M2
    E3 -.Subscribe.-> M3
    
    style C2 fill:#ffcccc
    style E2 fill:#ffcccc
```

---

## 13. 추상화 레벨

```mermaid
graph TB
    subgraph Level_0["Unity Framework"]
        MB[MonoBehaviour]
    end
    
    subgraph Level_1["Interface"]
        ID[IDamageable]
    end
    
    subgraph Level_2["Abstract Class"]
        P[Player]
    end
    
    subgraph Level_3["Concrete Classes"]
        W[Warrior]
        M[Mage]
        Ar[Archer]
    end
    
    MB --> P
    ID -.-> P
    P --> W
    P -.-> M
    P -.-> Ar
    
    style MB fill:#e1f5ff
    style ID fill:#ffe1f5
    style P fill:#fff4e1
    style W fill:#e8f5e8
    style M fill:#f0f0f0,stroke-dasharray: 5 5
    style Ar fill:#f0f0f0,stroke-dasharray: 5 5
```

---

## 14. 확장 가능성

```mermaid
classDiagram
    class Player {
        <<abstract>>
        #Attack()* abstract
    }
    
    class Warrior {
        -float Defense
        #Attack() override
    }
    
    class Mage {
        -float Mana
        -float MagicPower
        #Attack() override
    }
    
    class Archer {
        -float Range
        -int ArrowCount
        #Attack() override
    }
    
    Player <|-- Warrior
    Player <|-- Mage
    Player <|-- Archer
    
    style Warrior fill:#90EE90
    style Mage fill:#DDA0DD,stroke-dasharray: 5 5
    style Archer fill:#87CEEB,stroke-dasharray: 5 5
```

---

## 15. 상세 클래스 다이어그램

```mermaid
classDiagram
    class MonoBehaviour {
        <<Unity Framework>>
        +transform
        +gameObject
        +GetComponent()
    }
    
    class IDamageable {
        <<interface>>
        +TakeDamage(float)*
    }
    
    class Player {
        <<abstract>>
        #float MaxHP
        #float CurrentHP
        #float MoveSpeed
        #float AttackDamage
        #float AttackCooldown
        #Rigidbody2D RB
        #Animator Anim
        #SpriteRenderer SpriteRenderer
        #InputHandler InputHandler
        #Transform ArmTransform
        #int HashIsMoving$
        #int HashAttack$
        #int HashHit$
        #bool IsDead
        +float GetMaxHP
        +float GetCurrentHP
        +float GetMoveSpeed
        +float GetAttackDamage
        +float GetAttackCooldown
        #Awake() virtual
        #OnEnable()
        #OnDisable()
        +TakeDamage(float) virtual
        #Die() virtual
        #Attack()* abstract
        -FlipSprite(bool)
        -OnMove(Vector2)
        -OnAttack()
        -OnIntract(bool)
    }
    
    class Warrior {
        -float Defense
        #Awake() override
        +TakeDamage(float) override
        #Attack() override
    }
    
    class InputHandler {
        -InputSystem_Actions inputSystemAction
        -InputAction MoveAction
        -InputAction AttackAction
        -InputAction IntractAction
        +OnMoveAction$ Action~Vector2~
        +OnAttackAction$ Action
        +OnIntractAction$ Action~bool~
        -Awake()
        -OnEnable()
        -OnDisable()
        -OnMove(CallbackContext)
        -OnAttack(CallbackContext)
        -OnIntract(CallbackContext)
    }
    
    class Rigidbody2D {
        <<Unity>>
        +linearVelocity
    }
    
    class Animator {
        <<Unity>>
        +SetBool(int, bool)
        +SetTrigger(int)
    }
    
    class SpriteRenderer {
        <<Unity>>
        +flipX
    }
    
    MonoBehaviour <|-- Player
    MonoBehaviour <|-- InputHandler
    IDamageable <|.. Player
    Player <|-- Warrior
    Player o-- InputHandler
    Player o-- Rigidbody2D
    Player o-- Animator
    Player o-- SpriteRenderer
    InputHandler ..> Player
```

---

## 16. 이벤트 흐름도

```mermaid
sequenceDiagram
    participant User
    participant InputSystem
    participant InputHandler
    participant Player
    participant Animator
    
    User->>InputSystem: Keyboard/Mouse Input
    InputSystem->>InputHandler: InputAction Trigger
    
    alt Move Input
        InputHandler->>InputHandler: OnMove(CallbackContext)
        InputHandler->>Player: OnMoveAction Event
        Player->>Player: OnMove(Vector2)
        Player->>Player: FlipSprite()
        Player->>Animator: SetBool(IsMoving)
    end
    
    alt Attack Input
        InputHandler->>InputHandler: OnAttack(CallbackContext)
        Note right of InputHandler: EMPTY METHOD
        InputHandler--xPlayer: OnAttackAction NOT Invoked
    end
    
    alt Interact Input
        InputHandler->>InputHandler: OnIntract(CallbackContext)
        InputHandler->>Player: OnIntractAction Event
        Player->>Player: OnIntract(bool)
    end
```

---

## 17. 컴포넌트 의존성

```mermaid
graph TB
    subgraph GameObject
        W[Warrior]
        IH[InputHandler]
        RB[Rigidbody2D]
        AN[Animator]
        SR[SpriteRenderer]
        ARM[Arm Transform]
    end
    
    W -->|GetComponent| IH
    W -->|GetComponent| RB
    W -->|GetComponent| AN
    W -->|GetComponent| SR
    W -->|Find| ARM
    IH -.->|static event| W
    
    style W fill:#90EE90
    style IH fill:#FFD700
    style RB fill:#87CEEB
    style AN fill:#DDA0DD
    style SR fill:#F0E68C
    style ARM fill:#FFB6C1
```

---

## 18. 생명주기 다이어그램

```mermaid
sequenceDiagram
    participant Unity
    participant Warrior
    participant Player
    participant InputHandler
    
    Note over Unity: GameObject Created
    
    Unity->>Warrior: Awake()
    Warrior->>Player: base.Awake()
    Player->>Player: CurrentHP = MaxHP
    Player->>Player: Cache Components
    Player->>InputHandler: GetComponent
    Player-->>Warrior: Return
    Warrior->>Warrior: MaxHP = 150f
    Warrior->>Warrior: MoveSpeed = 4f
    Warrior->>Warrior: Set Other Stats
    
    Note over Unity: GameObject Enabled
    
    Unity->>InputHandler: OnEnable()
    InputHandler->>InputHandler: Enable Input System
    InputHandler->>InputHandler: Register Callbacks
    
    Unity->>Player: OnEnable()
    Player->>InputHandler: Subscribe Events
    
    Note over Unity: Game Running
    
    Note over Unity: GameObject Disabled
    
    Unity->>Player: OnDisable()
    Player->>InputHandler: Unsubscribe Events
    
    Unity->>InputHandler: OnDisable()
    InputHandler->>InputHandler: Disable Input System
```

---

## 19. 데미지 처리 흐름

```mermaid
flowchart TD
    Start([Enemy Attack]) --> W[Warrior.TakeDamage]
    W --> Calc[ActualDamage = Max<br/>damage - Defense, 5]
    Calc --> Log[Debug.Log]
    Log --> Base[base.TakeDamage<br/>damage]
    Base --> P[Player.TakeDamage]
    P --> Dead{IsDead?}
    Dead -->|Yes| Return[Return]
    Dead -->|No| Damage[CurrentHP -= damage]
    Damage --> Anim[Anim.SetTrigger Hit]
    Anim --> Check{CurrentHP <= 0?}
    Check -->|Yes| Die[Die Method]
    Check -->|No| End([End])
    Die --> End
    Return --> End
    
    style W fill:#90EE90
    style P fill:#FFE4B5
    style Die fill:#FFB6C1
```

---

## 20. 스탯 초기화 흐름

```mermaid
flowchart TD
    Start([Warrior Created]) --> WAwake[Warrior.Awake]
    WAwake --> BaseAwake[base.Awake]
    BaseAwake --> PInit[Player.Awake]
    PInit --> Set1[CurrentHP = MaxHP<br/>150 = 150]
    Set1 --> Cache[Cache Components]
    Cache --> Return[Return to Warrior]
    Return --> Set2[MaxHP = 150f]
    Set2 --> Set3[MoveSpeed = 4f]
    Set3 --> Set4[AttackDamage = 25f]
    Set4 --> Set5[AttackCooldown = 0.7f]
    Set5 --> End([Complete])
```

---

## 21. RequireComponent 의존성

```mermaid
graph TB
    P[Player<br/>MonoBehaviour]
    
    P -->|RequireComponent| RB[Rigidbody2D]
    P -->|RequireComponent| AN[Animator]
    P -->|RequireComponent| SR[SpriteRenderer]
    P -->|RequireComponent| IH[InputHandler]
    
    W[Warrior]
    W -.inherits.-> P
    W -.auto requires.-> RB
    W -.auto requires.-> AN
    W -.auto requires.-> SR
    W -.auto requires.-> IH
    
    style P fill:#FFE4B5
    style W fill:#90EE90
    style RB fill:#E0E0E0
    style AN fill:#E0E0E0
    style SR fill:#E0E0E0
    style IH fill:#FFD700
```

---

## 22. 공격 시나리오

```mermaid
sequenceDiagram
    participant User
    participant UnityInputSystem
    participant InputHandler
    participant Player
    participant Warrior
    participant Animator
    
    User->>UnityInputSystem: Attack Key
    UnityInputSystem->>InputHandler: AttackAction.performed
    InputHandler->>InputHandler: OnAttack(CallbackContext)
    InputHandler--xPlayer: OnAttackAction NOT Invoked
    Player--xPlayer: OnAttack() NOT Called
    Player--xAnimator: SetTrigger NOT Called
    Player--xWarrior: Attack() NOT Called
```

---

## 23. 데미지 처리 시나리오

```mermaid
sequenceDiagram
    participant Enemy
    participant Warrior
    participant Player
    participant Animator
    
    Enemy->>Warrior: TakeDamage(30f)
    Warrior->>Warrior: ActualDamage = 20
    Warrior->>Warrior: Debug.Log
    Warrior->>Player: base.TakeDamage(30f)
    Player->>Player: CurrentHP -= 30
    Player->>Animator: SetTrigger(Hit)
    
    alt Dead
        Player->>Player: Die()
    end
```

---

## 24. 이벤트 구독 관계도

```mermaid
graph LR
    subgraph InputHandler_Events
        E1[OnMoveAction]
        E2[OnAttackAction]
        E3[OnIntractAction]
    end
    
    subgraph Player_Methods
        M1[OnMove]
        M2[OnAttack]
        M3[OnIntract]
    end
    
    subgraph InputHandler_Callbacks
        C1[OnMove]
        C2[OnAttack<br/>EMPTY]
        C3[OnIntract]
    end
    
    C1 -.Invoke.-> E1
    C2 -.NOT Invoke.-> E2
    C3 -.Invoke.-> E3
    
    E1 -.Subscribe.-> M1
    E2 -.Subscribe.-> M2
    E3 -.Subscribe.-> M3
    
    style C2 fill:#ffcccc
    style E2 fill:#ffcccc
```

---

## 25. 추상화 레벨

```mermaid
graph TB
    subgraph Level_0["Unity Framework"]
        MB[MonoBehaviour]
    end
    
    subgraph Level_1["Interface"]
        ID[IDamageable]
    end
    
    subgraph Level_2["Abstract Class"]
        P[Player]
    end
    
    subgraph Level_3["Concrete Classes"]
        W[Warrior]
        M[Mage]
        Ar[Archer]
    end
    
    MB --> P
    ID -.-> P
    P --> W
    P -.-> M
    P -.-> Ar
    
    style MB fill:#e1f5ff
    style ID fill:#ffe1f5
    style P fill:#fff4e1
    style W fill:#e8f5e8
    style M fill:#f0f0f0,stroke-dasharray: 5 5
    style Ar fill:#f0f0f0,stroke-dasharray: 5 5
```

---

## 26. 확장 가능성

```mermaid
classDiagram
    class Player {
        <<abstract>>
        #Attack()* abstract
    }
    
    class Warrior {
        -float Defense
        #Attack() override
    }
    
    class Mage {
        -float Mana
        -float MagicPower
        #Attack() override
    }
    
    class Archer {
        -float Range
        -int ArrowCount
        #Attack() override
    }
    
    Player <|-- Warrior
    Player <|-- Mage
    Player <|-- Archer
    
    style Warrior fill:#90EE90
    style Mage fill:#DDA0DD,stroke-dasharray: 5 5
    style Archer fill:#87CEEB,stroke-dasharray: 5 5