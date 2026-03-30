# ScriptableObject 완벽 가이드

---

## 1. ScriptableObject란?

ScriptableObject는 GameObject에 붙이지 않고 독립적으로 존재하는 데이터 컨테이너입니다.

### 장점

- 메모리 효율적 (여러 오브젝트가 동일 데이터 공유)
- Inspector에서 직접 편집 가능
- 코드와 데이터 분리
- 빌드 크기 감소
- 런타임 성능 향상

---

## 2. 기본 사용법

### 기본 ScriptableObject 생성

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public float weight;
}
```

---

## 3. 캐릭터 스탯 시스템

```csharp
[CreateAssetMenu(menuName = "Game/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("기본 정보")]
    public string characterName;
    public Sprite portrait;
    
    [Header("전투 스탯")]
    public float maxHP = 100f;
    public float attackDamage = 10f;
    public float defense = 5f;
    public float moveSpeed = 5f;
    public float attackCooldown = 1f;
    
    [Header("성장")]
    public int level = 1;
    public int experience = 0;
    public AnimationClip[] attackAnimations;
}
```

**사용 예제**:

```csharp
public class Character : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    private float currentHP;
    
    private void Start()
    {
        currentHP = stats.maxHP;
    }
}
```

---

## 4. 아이템 시스템

### 기본 아이템

```csharp
public enum ItemType { Weapon, Armor, Consumable, QuestItem }
public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(menuName = "Game/Items/Item")]
public class Item : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public string itemID;
    public ItemType itemType;
    public ItemRarity rarity;
    public Sprite icon;
    
    [TextArea(3, 5)]
    public string description;
    
    [Header("경제")]
    public int buyPrice;
    public int sellPrice;
    
    [Header("스택")]
    public bool isStackable = false;
    public int maxStackSize = 1;
    
    public virtual void Use(GameObject user)
    {
        Debug.Log($"{itemName} used");
    }
}
```

### 무기 아이템

```csharp
[CreateAssetMenu(menuName = "Game/Items/Weapon")]
public class WeaponItem : Item
{
    [Header("무기 스탯")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float range = 2f;
    public float criticalChance = 0.1f;
    
    [Header("특수 효과")]
    public StatusEffect[] onHitEffects;
    
    public override void Use(GameObject user)
    {
        // 무기 장착 로직
    }
}
```

### 소비 아이템

```csharp
[CreateAssetMenu(menuName = "Game/Items/Consumable")]
public class ConsumableItem : Item
{
    [Header("회복")]
    public float healthRestore = 50f;
    public float manaRestore = 0f;
    
    [Header("효과")]
    public StatusEffect[] buffs;
    public float buffDuration = 10f;
    
    public override void Use(GameObject user)
    {
        // 회복 로직
        // 버프 적용 로직
    }
}
```

---

## 5. 스킬 시스템

```csharp
public enum SkillTargetType { Self, Enemy, Ally, Ground, Direction }

[CreateAssetMenu(menuName = "Game/Skills/Skill")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    public string skillID;
    public Sprite icon;
    
    [TextArea(3, 5)]
    public string description;
    
    [Header("사용 조건")]
    public int requiredLevel = 1;
    public float manaCost = 10f;
    public float cooldown = 5f;
    
    [Header("효과")]
    public float damage;
    public float range;
    public float radius;
    
    [Header("비주얼")]
    public GameObject vfxPrefab;
    public AnimationClip castAnimation;
    public AudioClip castSound;
    
    [Header("타겟팅")]
    public SkillTargetType targetType;
    public LayerMask targetLayer;
    
    public virtual void Execute(GameObject caster, Vector3 targetPosition)
    {
        Debug.Log($"{caster.name} used {skillName}");
    }
}
```

### 발사체 스킬

```csharp
[CreateAssetMenu(menuName = "Game/Skills/Projectile")]
public class ProjectileSkill : SkillData
{
    [Header("발사체")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public int projectileCount = 1;
    public float spreadAngle = 0f;
    
    public override void Execute(GameObject caster, Vector3 targetPosition)
    {
        base.Execute(caster, targetPosition);
        
        Vector3 direction = (targetPosition - caster.transform.position).normalized;
        
        for (int i = 0; i < projectileCount; i++)
        {
            // 발사체 생성 로직
        }
    }
}
```

---

## 6. 퀘스트 시스템

```csharp
public enum QuestObjectiveType { Kill, Collect, Interact, Reach }

[CreateAssetMenu(menuName = "Game/Quest")]
public class Quest : ScriptableObject
{
    [Header("기본 정보")]
    public string questID;
    public string questName;
    public Sprite questIcon;
    
    [TextArea(3, 5)]
    public string description;
    
    [Header("조건")]
    public int requiredLevel = 1;
    public Quest[] prerequisiteQuests;
    
    [Header("목표")]
    public QuestObjective[] objectives;
    
    [Header("보상")]
    public int experienceReward;
    public int goldReward;
    public Item[] itemRewards;
    
    public bool IsCompleted()
    {
        foreach (var objective in objectives)
        {
            if (!objective.IsCompleted())
                return false;
        }
        return true;
    }
}

[System.Serializable]
public class QuestObjective
{
    public string description;
    public QuestObjectiveType type;
    public int requiredCount;
    public int currentCount;
    
    public bool IsCompleted() => currentCount >= requiredCount;
}
```

---

## 7. 대화 시스템

```csharp
[CreateAssetMenu(menuName = "Game/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Header("NPC")]
    public string npcName;
    public Sprite npcPortrait;
    
    [Header("대화")]
    public DialogueNode[] dialogueNodes;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 10)]
    public string text;
    public DialogueChoice[] choices;
    public AudioClip voiceClip;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex;
    public Quest questToStart;
}
```

---

## 8. 게임 설정 (싱글톤)

```csharp
[CreateAssetMenu(menuName = "Game/Settings/Game Config")]
public class GameConfig : ScriptableObject
{
    private static GameConfig _instance;
    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameConfig>("GameConfig");
            }
            return _instance;
        }
    }
    
    [Header("난이도")]
    public float difficultyMultiplier = 1f;
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    
    [Header("경제")]
    public int startingGold = 100;
    
    [Header("플레이어")]
    public float respawnTime = 3f;
    public int maxInventorySlots = 20;
}
```

---

## 9. 이벤트 시스템

```csharp
[CreateAssetMenu(menuName = "Game/Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private System.Action listeners;
    
    public void Raise()
    {
        listeners?.Invoke();
    }
    
    public void RegisterListener(System.Action listener)
    {
        listeners += listener;
    }
    
    public void UnregisterListener(System.Action listener)
    {
        listeners -= listener;
    }
}
```

### 파라미터 있는 이벤트

```csharp
[CreateAssetMenu(menuName = "Game/Events/Int Event")]
public class IntGameEvent : ScriptableObject
{
    private System.Action<int> listeners;
    
    public void Raise(int value)
    {
        listeners?.Invoke(value);
    }
    
    public void RegisterListener(System.Action<int> listener)
    {
        listeners += listener;
    }
    
    public void UnregisterListener(System.Action<int> listener)
    {
        listeners -= listener;
    }
}
```

### 리스너 컴포넌트

```csharp
public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private UnityEngine.Events.UnityEvent response;
    
    private void OnEnable()
    {
        gameEvent.RegisterListener(OnEventRaised);
    }
    
    private void OnDisable()
    {
        gameEvent.UnregisterListener(OnEventRaised);
    }
    
    private void OnEventRaised()
    {
        response?.Invoke();
    }
}
```

---

## 10. 오디오 컬렉션

```csharp
[CreateAssetMenu(menuName = "Game/Audio/Audio Collection")]
public class AudioCollection : ScriptableObject
{
    [Header("BGM")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip bossMusic;
    
    [Header("SFX - UI")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    
    [Header("SFX - 전투")]
    public AudioClip[] swordSwing;
    public AudioClip[] hitImpact;
    
    public AudioClip GetRandomSwordSwing()
    {
        return swordSwing[Random.Range(0, swordSwing.Length)];
    }
}
```

---

## 11. 색상 팔레트

```csharp
[CreateAssetMenu(menuName = "Game/Visual/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [Header("UI")]
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.gray;
    public Color accentColor = Color.cyan;
    
    [Header("레어도")]
    public Color commonColor = Color.white;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;
    
    public Color GetRarityColor(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => commonColor,
            ItemRarity.Uncommon => uncommonColor,
            ItemRarity.Rare => rareColor,
            ItemRarity.Epic => epicColor,
            ItemRarity.Legendary => legendaryColor,
            _ => commonColor
        };
    }
}
```

---

## 12. 상태 효과 (버프/디버프)

```csharp
public enum StatusEffectType { Buff, Debuff, Neutral }

[CreateAssetMenu(menuName = "Game/Status Effect")]
public class StatusEffect : ScriptableObject
{
    [Header("기본 정보")]
    public string effectName;
    public Sprite icon;
    public StatusEffectType effectType;
    
    [Header("지속 시간")]
    public float duration = 10f;
    public bool isPermanent = false;
    public int maxStacks = 1;
    
    [Header("스탯 변경")]
    public float healthModifier;
    public float moveSpeedModifier;
    public float attackDamageModifier;
    public float defenseModifier;
    
    [Header("지속 데미지")]
    public bool hasDamageOverTime;
    public float damagePerTick = 5f;
    public float tickInterval = 1f;
    
    [Header("비주얼")]
    public GameObject vfxPrefab;
    public Color tintColor = Color.white;
    
    public virtual void OnApply(GameObject target)
    {
        Debug.Log($"{effectName} applied to {target.name}");
    }
    
    public virtual void OnTick(GameObject target)
    {
        // 지속 효과 로직
    }
    
    public virtual void OnRemove(GameObject target)
    {
        Debug.Log($"{effectName} removed");
    }
}
```

---

## 13. 룻 테이블 (랜덤 드랍)

```csharp
[CreateAssetMenu(menuName = "Game/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] private LootEntry[] lootEntries;
    
    public System.Collections.Generic.List<Item> RollLoot()
    {
        var droppedItems = new System.Collections.Generic.List<Item>();
        
        foreach (var entry in lootEntries)
        {
            float roll = Random.Range(0f, 100f);
            
            if (roll <= entry.dropChance)
            {
                int quantity = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                for (int i = 0; i < quantity; i++)
                {
                    droppedItems.Add(entry.item);
                }
            }
        }
        
        return droppedItems;
    }
}

[System.Serializable]
public class LootEntry
{
    public Item item;
    [Range(0f, 100f)] public float dropChance = 10f;
    public int minQuantity = 1;
    public int maxQuantity = 1;
}
```

---

## 14. 변수 컨테이너 (런타임 공유 변수)

```csharp
[CreateAssetMenu(menuName = "Game/Variables/Float Variable")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] private float _value;
    
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }
    
    public System.Action<float> OnValueChanged;
    
    public void Add(float amount) => Value += amount;
    public void Subtract(float amount) => Value -= amount;
}
```

**다른 타입들**:

```csharp
[CreateAssetMenu(menuName = "Game/Variables/Int Variable")]
public class IntVariable : ScriptableObject
{
    public int Value;
}

[CreateAssetMenu(menuName = "Game/Variables/Bool Variable")]
public class BoolVariable : ScriptableObject
{
    public bool Value;
}
```

**사용 예제**:

```csharp
public class PlayerGold : MonoBehaviour
{
    [SerializeField] private IntVariable goldVariable;
    
    public void AddGold(int amount)
    {
        goldVariable.Value += amount;
    }
    
    public bool TryPurchase(int cost)
    {
        if (goldVariable.Value >= cost)
        {
            goldVariable.Value -= cost;
            return true;
        }
        return false;
    }
}
```

---

## 15. 아이템 데이터베이스

```csharp
[CreateAssetMenu(menuName = "Game/Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private System.Collections.Generic.List<Item> items;
    
    private System.Collections.Generic.Dictionary<string, Item> itemDictionary;
    
    public void Initialize()
    {
        if (itemDictionary != null) return;
        
        itemDictionary = new System.Collections.Generic.Dictionary<string, Item>();
        foreach (var item in items)
        {
            itemDictionary[item.itemID] = item;
        }
    }
    
    public Item GetItem(string itemID)
    {
        Initialize();
        return itemDictionary.TryGetValue(itemID, out var item) ? item : null;
    }
    
    public System.Collections.Generic.List<Item> GetItemsByType(ItemType type)
    {
        var result = new System.Collections.Generic.List<Item>();
        foreach (var item in items)
        {
            if (item.itemType == type)
                result.Add(item);
        }
        return result;
    }
}
```

---

## 16. 웨이브 시스템

```csharp
[CreateAssetMenu(menuName = "Game/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("웨이브 정보")]
    public int waveNumber;
    public float preparationTime = 10f;
    
    [Header("적 생성")]
    public WaveEnemyGroup[] enemyGroups;
    
    [Header("보상")]
    public int goldReward;
    public int experienceReward;
    
    [Header("특수")]
    public bool isBossWave;
    public GameObject bossPrefab;
}

[System.Serializable]
public class WaveEnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval = 1f;
}
```

---

## 17. AI 행동

```csharp
[CreateAssetMenu(menuName = "Game/AI/AI Behavior")]
public class AIBehavior : ScriptableObject
{
    [Header("감지")]
    public float detectionRange = 10f;
    public float fieldOfView = 90f;
    public LayerMask targetLayer;
    
    [Header("전투")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    
    [Header("이동")]
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float patrolSpeed = 2f;
    
    [Header("상태")]
    public float fleeHealthThreshold = 0.2f;
}
```

---

## 18. 던전 데이터

```csharp
[CreateAssetMenu(menuName = "Game/Dungeon Data")]
public class DungeonData : ScriptableObject
{
    [Header("기본 정보")]
    public string dungeonName;
    public Sprite dungeonIcon;
    
    [Header("요구사항")]
    public int requiredLevel = 1;
    public Quest requiredQuest;
    
    [Header("난이도")]
    public int recommendedPartySize = 1;
    public float difficultyRating = 1f;
    
    [Header("적")]
    public EnemySpawnData[] enemySpawns;
    public GameObject bossPrefab;
    
    [Header("보상")]
    public LootTable lootTable;
    public int baseExperience = 100;
    public int baseGold = 50;
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int minCount = 1;
    public int maxCount = 3;
    public float spawnWeight = 1f;
}
```

---

## 19. 참조 타입

```csharp
[CreateAssetMenu(menuName = "Game/References/Transform Reference")]
public class TransformReference : ScriptableObject
{
    public Transform Value;
    
    public Vector3 Position => Value ? Value.position : Vector3.zero;
    public Quaternion Rotation => Value ? Value.rotation : Quaternion.identity;
}
```

---

## 20. 리스트 컨테이너

```csharp
[CreateAssetMenu(menuName = "Game/Collections/Runtime Set")]
public class RuntimeSet : ScriptableObject
{
    [SerializeField] private System.Collections.Generic.List<GameObject> items;
    
    public System.Collections.Generic.IReadOnlyList<GameObject> Items => items;
    
    public void Add(GameObject item)
    {
        if (!items.Contains(item))
            items.Add(item);
    }
    
    public void Remove(GameObject item)
    {
        items.Remove(item);
    }
    
    public void Clear()
    {
        items.Clear();
    }
}
```

---

## 21. 주의사항

### ❌ 잘못된 사용

```csharp
// 런타임 데이터를 ScriptableObject에 직접 저장하지 마세요
[CreateAssetMenu]
public class BadExample : ScriptableObject
{
    public int currentHealth; // 모든 인스턴스가 같은 값 공유!
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // 위험!
    }
}
```

### ✅ 올바른 사용

```csharp
// 읽기 전용 데이터로 사용
[CreateAssetMenu]
public class GoodExample : ScriptableObject
{
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;
}

// 런타임 데이터는 MonoBehaviour에서 관리
public class Character : MonoBehaviour
{
    [SerializeField] private GoodExample stats;
    private int currentHealth;
    
    private void Start()
    {
        currentHealth = stats.MaxHealth;
    }
}
```

---

## 22. 에디터 전용 기능

```csharp
[CreateAssetMenu(menuName = "Game/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int price;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (price < 0)
        {
            Debug.LogWarning($"{itemName}: 가격은 0 이상이어야 합니다");
            price = 0;
        }
    }
    
    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        // ID 생성 로직
    }
#endif
}
```

---

## 23. 성능 최적화

```csharp
[CreateAssetMenu]
public class OptimizedDatabase : ScriptableObject
{
    [SerializeField] private System.Collections.Generic.List<Item> items;
    private System.Collections.Generic.Dictionary<string, Item> _cache;
    
    public void Initialize()
    {
        if (_cache != null) return; // 이미 초기화됨
        
        _cache = new System.Collections.Generic.Dictionary<string, Item>();
        foreach (var item in items)
        {
            _cache[item.itemID] = item;
        }
    }
    
    public Item GetItem(string id)
    {
        Initialize();
        return _cache.TryGetValue(id, out var item) ? item : null;
    }
}
```

---

## 24. 프로젝트 구조 예제

```
Assets/
├── ScriptableObjects/
│   ├── Characters/
│   │   ├── Player/
│   │   │   ├── WarriorStats.asset
│   │   │   ├── MageStats.asset
│   │   │   └── ArcherStats.asset
│   │   └── Enemies/
│   │       ├── GoblinStats.asset
│   │       └── OrcStats.asset
│   ├── Items/
│   │   ├── Weapons/
│   │   ├── Armor/
│   │   └── Consumables/
│   ├── Quests/
│   ├── Skills/
│   ├── Config/
│   │   └── GameConfig.asset
│   └── Events/
│       ├── OnPlayerDied.asset
│       └── OnEnemyKilled.asset
```

---

## 25. 핵심 원칙

### ✅ 해야 할 것

- 읽기 전용 데이터로 사용
- 코드와 데이터 분리
- 재사용 가능한 에셋 생성
- 타입별로 폴더 정리

### ❌ 하지 말아야 할 것

- 런타임 상태 저장
- 참조 타입 남용
- 순환 참조 생성

---

## 요약

ScriptableObject는 **데이터 중심 설계**를 가능하게 하는 강력한 도구입니다.

**활용 분야**:
- 캐릭터/아이템 스탯
- 스킬/퀘스트 시스템
- 게임 설정 및 밸런싱
- 이벤트 시스템
- AI 행동
- 오디오/비주얼 데이터

올바르게 사용하면 **유지보수가 쉽고 확장 가능한** 게임을 만들 수 있습니다!
